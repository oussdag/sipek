/* 
* Copyright (C) 2007 Sasa Coh <sasacoh@gmail.com>
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
*/

#include "pjsipDll.h"
#include <pjsua-lib/pjsua.h>
#include <string>

using namespace std;

#define THIS_FILE	"pjsipDll.cpp"
#define NO_LIMIT	(int)0x7FFFFFFF

// global function pointers
static fptr_regstate* cb_regstate = 0;
static fptr_callstate* cb_callstate = 0;
static fptr_callincoming* cb_callincoming = 0;
static fptr_getconfigdata* cb_getconfigdata = 0;
static fptr_callholdconf* cb_callholdconf = 0;
static fptr_callretrieveconf* cb_callretrieveconf = 0;
static fptr_buddystatus* cb_buddystatus = 0;
static fptr_msgrec* cb_messagereceived = 0;

// for playing files (tones)
static pjmedia_snd_port* snd_port;

enum {
    SC_Deflect,
    SC_CFU,
    SC_CFNR,
    SC_DND,
    SC_3Pty
};

//////////////////////////////////////////////////////////////////////////
int initTones();
//////////////////////////////////////////////////////////////////////////

/* Call specific data */
struct call_data
{
	pj_timer_entry	    timer;
};

/* Pjsua application data */
static struct app_config
{
	pjsua_config	    cfg;
	pjsua_logging_config    log_cfg;
	pjsua_media_config	    media_cfg;
	pj_bool_t		    no_refersub;
	pj_bool_t		    no_tcp;
	pj_bool_t		    no_udp;
	pj_bool_t		    use_tls;
	pjsua_transport_config  udp_cfg;
	pjsua_transport_config  rtp_cfg;

	unsigned		    acc_cnt;
	pjsua_acc_config	    acc_cfg[PJSUA_MAX_ACC];

	unsigned		    buddy_cnt;
	pjsua_buddy_config	    buddy_cfg[PJSUA_MAX_BUDDIES];

	struct call_data	    call_data[PJSUA_MAX_CALLS];

	pj_pool_t		   *pool;
	/* Compatibility with older pjsua */

	unsigned		    codec_cnt;
	pj_str_t		    codec_arg[32];
	pj_bool_t		    null_audio;
	unsigned		    wav_count;
	pj_str_t		    wav_files[32];
	unsigned		    tone_count;
	pjmedia_tone_desc	    tones[32];
	pjsua_conf_port_id	    tone_slots[32];
	pjsua_player_id	    wav_id;
	pjsua_conf_port_id	    wav_port;
	pj_bool_t		    auto_play;
	pj_bool_t		    auto_loop;
	pj_bool_t		    auto_conf;
	pj_str_t		    rec_file;
	pj_bool_t		    auto_rec;
	pjsua_recorder_id	    rec_id;
	pjsua_conf_port_id	    rec_port;
	unsigned		    ptime;
	unsigned		    auto_answer;
	unsigned		    duration;

#ifdef STEREO_DEMO
	pjmedia_snd_port* snd_port;
#endif

	float		    mic_level,
		speaker_level;

} app_config;


//static pjsua_acc_id	current_acc;
//#define current_acc	pjsua_acc_get_default()
static pjsua_call_id	current_call = PJSUA_INVALID_ID;

/* Set default config. */
static void default_config(struct app_config *cfg)
{
	char tmp[80];

	pjsua_config_default(&cfg->cfg);
	pj_ansi_sprintf(tmp, "SIPek on PJSUA v%s/%s", PJ_VERSION, PJ_OS_NAME);
	pj_strdup2_with_null(app_config.pool, &cfg->cfg.user_agent, tmp);

	pjsua_logging_config_default(&cfg->log_cfg);
	cfg->log_cfg.log_filename = pj_str("pjsip.log");
	pjsua_media_config_default(&cfg->media_cfg);
	pjsua_transport_config_default(&cfg->udp_cfg);
	cfg->udp_cfg.port = 5060;
	pjsua_transport_config_default(&cfg->rtp_cfg);
	cfg->rtp_cfg.port = 4000;
	cfg->duration = NO_LIMIT;
	cfg->wav_id = PJSUA_INVALID_ID;
	cfg->rec_id = PJSUA_INVALID_ID;
	cfg->wav_port = PJSUA_INVALID_ID;
	cfg->rec_port = PJSUA_INVALID_ID;
	cfg->mic_level = cfg->speaker_level = 1.0;

	cfg->wav_files[0] = pj_str("sounds\\dial.wav");
	cfg->wav_files[1] = pj_str("sounds\\congestion.wav");
	cfg->wav_files[2] = pj_str("sounds\\ringback.wav");
	cfg->wav_files[3] = pj_str("sounds\\ring.wav");
	cfg->wav_count = 4;
}

//////////////////////////////////////////////////////////////////////////
// Callbacks
//////////////////////////////////////////////////////////////////////////

static void on_call_state(pjsua_call_id call_id, pjsip_event *e)
{
	pjsua_call_info call_info;

	PJ_UNUSED_ARG(e);

	pjsua_call_get_info(call_id, &call_info);

	if (cb_regstate != 0) cb_callstate(call_id, call_info.state);
}

static void on_incoming_call(pjsua_acc_id acc_id, pjsua_call_id call_id,
														 pjsip_rx_data *rdata)
{
	pjsua_call_info call_info;

	pjsua_call_get_info(call_id, &call_info);

  if (cb_callincoming != 0) cb_callincoming(call_id, call_info.remote_contact.ptr);
}

/*
* Callback on media state changed event.
* The action may connect the call to sound device, to file, or
* to loop the call.
*/
static void on_call_media_state(pjsua_call_id call_id)
{
    pjsua_call_info call_info;

    pjsua_call_get_info(call_id, &call_info);

    if (call_info.media_status == PJSUA_CALL_MEDIA_ACTIVE) {
	pj_bool_t connect_sound = PJ_TRUE;

	/* Loopback sound, if desired */
	if (app_config.auto_loop) {
	    pjsua_conf_connect(call_info.conf_slot, call_info.conf_slot);
	    connect_sound = PJ_FALSE;

	    /* Automatically record conversation, if desired */
	    if (app_config.auto_rec && app_config.rec_port != PJSUA_INVALID_ID) {
		pjsua_conf_connect(call_info.conf_slot, app_config.rec_port);
	    }
	}

	/* Stream a file, if desired */
	if (app_config.auto_play && app_config.wav_port != PJSUA_INVALID_ID) {
	    pjsua_conf_connect(app_config.wav_port, call_info.conf_slot);
	    connect_sound = PJ_FALSE;
	}

	/* Put call in conference with other calls, if desired */
	if (app_config.auto_conf) {
	    pjsua_call_id call_ids[PJSUA_MAX_CALLS];
	    unsigned call_cnt=PJ_ARRAY_SIZE(call_ids);
	    unsigned i;

	    /* Get all calls, and establish media connection between
	     * this call and other calls.
	     */
	    pjsua_enum_calls(call_ids, &call_cnt);

	    for (i=0; i<call_cnt; ++i) {
		if (call_ids[i] == call_id)
		    continue;
		
		if (!pjsua_call_has_media(call_ids[i]))
		    continue;

		pjsua_conf_connect(call_info.conf_slot,
				   pjsua_call_get_conf_port(call_ids[i]));
		pjsua_conf_connect(pjsua_call_get_conf_port(call_ids[i]),
				   call_info.conf_slot);

		/* Automatically record conversation, if desired */
		if (app_config.auto_rec && app_config.rec_port != PJSUA_INVALID_ID) {
		    pjsua_conf_connect(pjsua_call_get_conf_port(call_ids[i]), 
				       app_config.rec_port);
		}

	    }

	    /* Also connect call to local sound device */
	    connect_sound = PJ_TRUE;
	}

	/* Otherwise connect to sound device */
	if (connect_sound) {
	    pjsua_conf_connect(call_info.conf_slot, 0);
	    pjsua_conf_connect(0, call_info.conf_slot);

	    /* Automatically record conversation, if desired */
	    if (app_config.auto_rec && app_config.rec_port != PJSUA_INVALID_ID) {
		pjsua_conf_connect(call_info.conf_slot, app_config.rec_port);
		pjsua_conf_connect(0, app_config.rec_port);
	    }
	}

	PJ_LOG(3,(THIS_FILE, "Media for call %d is active", call_id));

    } else if (call_info.media_status == PJSUA_CALL_MEDIA_LOCAL_HOLD) {
	PJ_LOG(3,(THIS_FILE, "Media for call %d is suspended (hold) by local",
		  call_id));
  
  cb_callholdconf(call_id);


    } else if (call_info.media_status == PJSUA_CALL_MEDIA_REMOTE_HOLD) {
	PJ_LOG(3,(THIS_FILE, 
		  "Media for call %d is suspended (hold) by remote",
		  call_id));
    } else {
	PJ_LOG(3,(THIS_FILE, 
		  "Media for call %d is inactive",
		  call_id));
    }
}


static void on_reg_state(pjsua_acc_id acc_id)
{
  pjsua_acc_info accinfo;

  pjsua_acc_get_info(acc_id, &accinfo);

	// callback
	if (cb_regstate != 0) cb_regstate(acc_id, accinfo.status);
}

/*
 * Handler on buddy state changed.
 */
static void on_buddy_state(pjsua_buddy_id buddy_id)
{
    pjsua_buddy_info info;
    pjsua_buddy_get_info(buddy_id, &info);

    PJ_LOG(3,(THIS_FILE, "%.*s status is %.*s",
	      (int)info.uri.slen,
	      info.uri.ptr,
	      (int)info.status_text.slen,
	      info.status_text.ptr));

	// callback
  if (cb_buddystatus != 0) cb_buddystatus(buddy_id, info.status);
}


/**
 * Incoming IM message (i.e. MESSAGE request)!
 */
static void on_pager(pjsua_call_id call_id, const pj_str_t *from, 
		     const pj_str_t *to, const pj_str_t *contact,
		     const pj_str_t *mime_type, const pj_str_t *text)
{
    /* Note: call index may be -1 */
    PJ_UNUSED_ARG(call_id);
    PJ_UNUSED_ARG(to);
    PJ_UNUSED_ARG(contact);
    PJ_UNUSED_ARG(mime_type);

    PJ_LOG(3,(THIS_FILE,"MESSAGE from %.*s: %.*s",
	      (int)from->slen, from->ptr,
	      (int)text->slen, text->ptr));

    if (cb_messagereceived != 0) (*cb_messagereceived)(from->ptr, text->ptr);
}


/**
 * Received typing indication
 */
static void on_typing(pjsua_call_id call_id, const pj_str_t *from,
		      const pj_str_t *to, const pj_str_t *contact,
		      pj_bool_t is_typing)
{
    PJ_UNUSED_ARG(call_id);
    PJ_UNUSED_ARG(to);
    PJ_UNUSED_ARG(contact);

    PJ_LOG(3,(THIS_FILE, "IM indication: %.*s %s",
	      (int)from->slen, from->ptr,
	      (is_typing?"is typing..":"has stopped typing")));
}

//////////////////////////////////////////////////////////////////////////
// DLL functions...
PJSIPDLL_DLL_API int dll_init(int listenPort)
{
	pjsua_transport_id transport_id = -1;
	unsigned i;
	pj_status_t status;

	/* Create pjsua */
	status = pjsua_create();
	if (status != PJ_SUCCESS)
		return status;

	/* Create pool for application */
	app_config.pool = pjsua_pool_create("pjsua", 4000, 4000);

	/* Initialize default config */
	default_config(&app_config);

  // set listening port
  app_config.udp_cfg.port = listenPort;

	/* Parse the arguments */
//	status = parse_args(argc, argv, &app_config, &uri_arg);
//	if (status != PJ_SUCCESS)
//		return status;

	/* Copy udp_cfg STUN config to rtp_cfg */
//	app_config.rtp_cfg.use_stun = app_config.udp_cfg.use_stun;
//	app_config.rtp_cfg.stun_config = app_config.udp_cfg.stun_config;


	/* Initialize application callbacks */
	app_config.cfg.cb.on_call_state = &on_call_state;
	app_config.cfg.cb.on_call_media_state = &on_call_media_state;
	app_config.cfg.cb.on_incoming_call = &on_incoming_call;
//	app_config.cfg.cb.on_dtmf_digit = &call_on_dtmf_callback;
	app_config.cfg.cb.on_reg_state = &on_reg_state;
	app_config.cfg.cb.on_buddy_state = &on_buddy_state;
	app_config.cfg.cb.on_pager = &on_pager;
	app_config.cfg.cb.on_typing = &on_typing;
//	app_config.cfg.cb.on_call_transfer_status = &on_call_transfer_status;
//	app_config.cfg.cb.on_call_replaced = &on_call_replaced;

	/* Initialize pjsua */
	status = pjsua_init(&app_config.cfg, &app_config.log_cfg,	&app_config.media_cfg);
	if (status != PJ_SUCCESS)
		return status;

#ifdef STEREO_DEMO
	stereo_demo();
#endif

	/* Initialize calls data */
	for (i=0; i<PJ_ARRAY_SIZE(app_config.call_data); ++i) {
		app_config.call_data[i].timer.id = PJSUA_INVALID_ID;
//		app_config.call_data[i].timer.cb = &call_timeout_callback;
	}

	//////////////////////////////////////////////////////////////////////////////
	// Initialiye Wav file player
	//pj_str_t label = pj_str("label");
	//pjsua_playlist_create( app_config.wav_files, app_config.wav_count, &label, 0, &wav_id);

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	/* Add UDP transport unless it's disabled. */
	if (!app_config.no_udp) {
		pjsua_acc_id aid;

		status = pjsua_transport_create(PJSIP_TRANSPORT_UDP,
			&app_config.udp_cfg, 
			&transport_id);
		if (status != PJ_SUCCESS)
			goto on_error;
	}

	/* Add TCP transport unless it's disabled */
	if (!app_config.no_tcp) {
		status = pjsua_transport_create(PJSIP_TRANSPORT_TCP,
			&app_config.udp_cfg, 
			&transport_id);
		if (status != PJ_SUCCESS)
			goto on_error;

		/* Add local account */
//		pjsua_acc_add_local(transport_id, PJ_TRUE, NULL);
//		pjsua_acc_set_online_status(current_acc, PJ_TRUE);
	}


#if defined(PJSIP_HAS_TLS_TRANSPORT) && PJSIP_HAS_TLS_TRANSPORT!=0
	/* Add TLS transport when application wants one */
	if (app_config.use_tls) {

		pjsua_acc_id acc_id;

		/* Set TLS port as TCP port+1 */
		app_config.udp_cfg.port++;
		status = pjsua_transport_create(PJSIP_TRANSPORT_TLS,
			&app_config.udp_cfg, 
			&transport_id);
		app_config.udp_cfg.port--;
		if (status != PJ_SUCCESS)
			goto on_error;

		/* Add local account */
		pjsua_acc_add_local(transport_id, PJ_FALSE, &acc_id);
		pjsua_acc_set_online_status(acc_id, PJ_TRUE);
	}
#endif

	if (transport_id == -1) {
		PJ_LOG(3,(THIS_FILE, "Error: no transport is configured"));
		status = -1;
		goto on_error;
	}

	/* Optionally set codec orders */
	for (i=0; i<app_config.codec_cnt; ++i) {
		pjsua_codec_set_priority(&app_config.codec_arg[i],
			(pj_uint8_t)(PJMEDIA_CODEC_PRIO_NORMAL+i+9));
	}

	/* Add RTP transports */
	status = pjsua_media_transports_create(&app_config.rtp_cfg);
	if (status != PJ_SUCCESS)
		goto on_error;

	/* Use null sound device? */
#ifndef STEREO_DEMO
	if (app_config.null_audio) {
		status = pjsua_set_null_snd_dev();
		if (status != PJ_SUCCESS)
			return status;
	}
#endif

	return PJ_SUCCESS;

on_error:
	pjsua_destroy();
	return status;
}


PJSIPDLL_DLL_API int dll_main(void)
{
	pj_status_t status;

	/* Start pjsua */
	status = pjsua_start();

	if (status != PJ_SUCCESS) 
	{
		pjsua_destroy();
		return status;
	}

	return PJ_SUCCESS;
}

//////////////////////////////////////////////////////////////////////////

PJSIPDLL_DLL_API int onRegStateCallback(fptr_regstate cb)
{
	cb_regstate = cb;
	return 1;
}
 
PJSIPDLL_DLL_API int onCallStateCallback(fptr_callstate cb)
{
	cb_callstate = cb;
	return 1;
}

PJSIPDLL_DLL_API int onCallIncoming(fptr_callincoming cb)
{
	cb_callincoming = cb;
	return 1;
}

PJSIPDLL_DLL_API int getConfigDataCallback(fptr_getconfigdata cb)
{
	cb_getconfigdata = cb;
	return 1;
}

PJSIPDLL_DLL_API int onCallHoldConfirmCallback(fptr_callholdconf cb)
{
  cb_callholdconf = cb;
	return 1;
}

PJSIPDLL_DLL_API int onMessageReceivedCallback(fptr_msgrec cb)
{
  cb_messagereceived = cb;
  return 1;
}

PJSIPDLL_DLL_API int onBuddyStatusChangedCallback(fptr_buddystatus cb)
{
  cb_buddystatus = cb;
  return 1;
}

//////////////////////////////////////////////////////////////////////////


PJSIPDLL_DLL_API int dll_shutdown()
{
	pjsua_destroy();
	return 0;
}

//////////////////////////////////////////////////////////////////////////
int dll_registerAccount(char* uri, char* reguri, char* domain, char* username, char* password)
{
	pjsua_acc_config thisAccountsCfg; // = mSipConfiguration->getAccountCfg(aAccId);
	pjsua_acc_config_default(&thisAccountsCfg);

	//4.) set parameters 
	thisAccountsCfg.id = pj_str(uri);
	thisAccountsCfg.reg_timeout = 3600;
	thisAccountsCfg.reg_uri = pj_str(reguri);

	// AUTHENTICATION
	thisAccountsCfg.cred_count = 1;
	thisAccountsCfg.cred_info[0].username = pj_str(username);
	thisAccountsCfg.cred_info[0].realm = pj_str(domain);
	thisAccountsCfg.cred_info[0].scheme = pj_str("digest");
	thisAccountsCfg.cred_info[0].data_type = PJSIP_CRED_DATA_PLAIN_PASSWD;
	thisAccountsCfg.cred_info[0].data = pj_str(password);
	
	pjsua_acc_id pjAccId= -1;
	int status = pjsua_acc_add(&thisAccountsCfg, PJ_TRUE, &pjAccId);
  
  pjsua_acc_set_online_status(pjAccId, PJ_TRUE);

  return pjAccId;
}

///////////////////////////////////////////////////////////////////////
// Call API

int dll_makeCall(int accountId, char* uri)
{
int newcallId = -1; 
	pj_str_t tmp = pj_str(uri);
	pjsua_call_make_call( accountId, &tmp, 0, NULL, NULL, &newcallId);

	return newcallId;
}

int dll_releaseCall(int callId)
{
	pjsua_call_hangup(callId, 0, NULL, NULL);
	return 0;
}


int dll_answerCall(int callId, int code)
{
	pjsua_call_answer(callId, code, NULL, NULL);
	return 1;
}

int dll_holdCall(int callId)
{
  pjsua_call_set_hold(callId, NULL);	
  return 1;
}

int dll_retrieveCall(int callId)
{
  pjsua_call_reinvite(callId, PJ_TRUE, NULL);
  return 1;
}

int dll_xferCall(int callid, char* uri)
{
  pjsua_msg_data msg_data;
  pjsip_generic_string_hdr refer_sub;
  pj_str_t STR_REFER_SUB = { "Refer-Sub", 9 };
  pj_str_t STR_FALSE = { "false", 5 };
  pjsua_call_info ci;

  pjsua_call_get_info(callid, &ci);

  //ui_input_url("Transfer to URL", buf, sizeof(buf), &result);

  pjsua_msg_data_init(&msg_data);
  if (app_config.no_refersub) 
  {
    // Add Refer-Sub: false in outgoing REFER request
    pjsip_generic_string_hdr_init2(&refer_sub, &STR_REFER_SUB, &STR_FALSE);
    pj_list_push_back(&msg_data.hdr_list, &refer_sub);
  }

  pj_str_t tmp = pj_str(uri);
  pj_status_t st = pjsua_call_xfer( callid, &tmp, &msg_data);
  return st;
}

int dll_xferCallWithReplaces(int callId, int dstSession)
{
int call = callId;
pjsua_msg_data msg_data;
pjsip_generic_string_hdr refer_sub;
pj_str_t STR_REFER_SUB = { "Refer-Sub", 9 };
pj_str_t STR_FALSE = { "false", 5 };
pjsua_call_id ids[PJSUA_MAX_CALLS];
pjsua_call_info ci;

	pjsua_msg_data_init(&msg_data);
	if (app_config.no_refersub) {
	    /* Add Refer-Sub: false in outgoing REFER request */
	    pjsip_generic_string_hdr_init2(&refer_sub, &STR_REFER_SUB, &STR_FALSE);
	    pj_list_push_back(&msg_data.hdr_list, &refer_sub);
	}

	pjsua_call_xfer_replaces(call, dstSession, 0, &msg_data);

  return 1;
}

int dll_serviceReq(int callId, int serviceCode, const char* destUri)
{
  int status = !PJ_SUCCESS; //default status is ERROR!!
  switch(serviceCode)
  {
    case SC_3Pty:
      {//as this is only local 3PTY that's all we have to do ....
        status = dll_retrieveCall(callId);
      }
  	  break;
    case SC_CFU:
    //case SC_CFB:
    case SC_CFNR:
    case SC_Deflect:
      {
        //1.) build sip target Uri  
        pj_str_t contact_header_to_call = pj_str((char*)destUri);
        
        //2.) Fill pjsua_msg_data with correct Contact header ...
        pjsua_msg_data aStruct;
        pjsua_msg_data_init(&aStruct);//Initialize ...
        
        pjsip_generic_string_hdr warn;
        pj_str_t hname = pj_str("Contact");
        pj_str_t hvalue = contact_header_to_call;
        pjsip_generic_string_hdr_init2(&warn, &hname, &hvalue);
        warn.type = PJSIP_H_CONTACT;
        pj_list_push_back(&aStruct.hdr_list, &warn);
        
        //3.) Forward this call...
        //convert callId from abstract one (UI/CC) into concrete one (PJSIP) !!!
        status = pjsua_call_hangup(callId, 302, NULL, &(aStruct));
      }
      break;
    case SC_DND:
      {
        //this->handleReleaseReq(aAbstrCallId, 486);// sends 486 Busy here and releases this call instance ...
        status = pjsua_call_hangup(callId, 486, NULL, NULL);
      }
      break;
  }//switch(serviceCode)
  return status;
}

int dll_dialDtmf(int callId, char* digits, int mode)
{
  pj_str_t dg = pj_str(digits);
	int status = pjsua_call_dial_dtmf(callId, &dg);
  return status;
}

////////////////////////////////////////////////////////////////////////////////////////////////
int dll_addBuddy(char* uri, bool subscribe)
{
pj_status_t status;
pjsua_buddy_config buddy_cfg;

  buddy_cfg.uri = pj_str(uri);
  buddy_cfg.subscribe = (subscribe == true) ? 1 : 0;
  // Add buddy
  int buddyId = -1;
	status = pjsua_buddy_add(&buddy_cfg, &buddyId);
  return buddyId;
}

int dll_removeBuddy(int buddyId)
{
  return pjsua_buddy_del(buddyId);
}

int dll_sendMessage(int accId, char* uri, char* message)
{
  pj_str_t tmp_uri = pj_str(uri);
  pj_str_t tmp = pj_str(message);
	return pjsua_im_send(accId, &tmp_uri, NULL, &tmp, NULL, NULL);
}

//////////////////////////////////////////////////////////////////////////////

int dll_playTone(int toneId)
{
int status = -1;
pjmedia_port *file_port;

/*
     pjsua_player_id player_id;
     
     status = pjsua_player_create(&app_config.wav_files[toneId], 0, &player_id);
     if (status != PJ_SUCCESS)
        return status;
*/

    /* Create file media port from the WAV file */
    status = pjmedia_wav_player_port_create( app_config.pool,	/* memory pool	    */
					      app_config.wav_files[toneId].ptr,	/* file to play	    */
					      20,	/* ptime.	    */
					      0,	/* flags	    */
					      0,	/* default buffer   */
					      &file_port/* returned port    */
					      );

    if (status != PJ_SUCCESS) {
		//app_perror(THIS_FILE, "Unable to use WAV file", status);
		return 1;
    }

	     /* Create sound player port. */
    status = pjmedia_snd_port_create_player( 
		 app_config.pool,				    /* pool		    */
		 -1,				    /* use default dev.	    */
		 file_port->info.clock_rate,	    /* clock rate.	    */
		 file_port->info.channel_count,	    /* # of channels.	    */
		 file_port->info.samples_per_frame, /* samples per frame.   */
		 file_port->info.bits_per_sample,   /* bits per sample.	    */
		 0,				    /* options		    */
		 &snd_port			    /* returned port	    */
		 );

    if (status != PJ_SUCCESS) {
		//app_perror(THIS_FILE, "Unable to open sound device", status);
		return 1;
    }

    /* Connect file port to the sound player.
     * Stream playing will commence immediately.
     */
    status = pjmedia_snd_port_connect( snd_port, file_port);

	return status;
}

int dll_stopTone()
{
	pj_status_t status;
    /* Start deinitialization: */

    /* Disconnect sound port from file port */
    status = pjmedia_snd_port_disconnect(snd_port);
    PJ_ASSERT_RETURN(status == PJ_SUCCESS, 1);
	
	return status;
}


