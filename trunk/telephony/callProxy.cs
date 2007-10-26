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

using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;

namespace Telephony
{
  public enum ETones : int
  {
    EToneDial = 0,
    EToneCongestion,
    EToneRingback,
    EToneRing,
  }


  /// <summary>
  /// 
  /// </summary>
  public class CSipCallProxy : CCallProxyInterface
  {
    // call API
    [DllImport("pjsipDll.dll")]
    private static extern int dll_makeCall(int accountId, string uri);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_releaseCall(int callId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_answerCall(int callId, int code);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_holdCall(int callId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_retrieveCall(int callId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_xferCall(int callId, string uri);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_xferCallWithReplaces(int callId, int dstSession);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_serviceReq(int callId, int serviceCode, string destUri);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_dialDtmf(int callId, string digits, int mode);

    // identify line

    private CCallManager Manager
    {
      get { return CCallManager.getInstance(); }
    }

    #region Constructor

    #endregion Constructor


    #region Methods

    /// <summary>
    /// Method makeCall creates call session
    /// </summary>
    /// <param name="dialedNo"></param>
    /// <param name="accountId"></param>
    /// <returns>SessionId selected by sip stack</returns>
    public int makeCall(string dialedNo, int accountId)
    {
      string uri = "sip:" + dialedNo + "@" + Manager.getAddress(accountId);
      int sessionId = dll_makeCall(accountId, uri);
      return sessionId;
    }

    public bool endCall(int sessionId)
    {
      dll_releaseCall(sessionId);
      return true;
    }

    public bool alerted(int sessionId)
    {
      dll_answerCall(sessionId, 180);
      return true;
    }

    public bool acceptCall(int sessionId)
    {
      dll_answerCall(sessionId, 200);
      return true;
    }
    
    public bool holdCall(int sessionId)
    {
      dll_holdCall(sessionId);
      return true;
    }
    
    public bool retrieveCall(int sessionId)
    {
      dll_retrieveCall(sessionId);
      return true;
    }
          
    public bool xferCall(int sessionId, string number)
    {
      string uri = "sip:" + number + "@" + Manager.getAddress(Manager.DefaultAccountIndex);
      dll_xferCall(sessionId, uri);
      return true;
    }
    
    public bool xferCallSession(int sessionId, int session)
    {
      dll_xferCallWithReplaces(sessionId, session);
      return true;
    }

    public bool threePtyCall(int sessionId, int session)
    {
      dll_serviceReq(sessionId, (int)EServiceCodes.SC_3PTY, "");
      return true;
    }
    
    public bool serviceRequest(int sessionId, int code, string dest)
    {
      string destUri = "<sip:" + dest + "@" + Manager.getAddress() + ">";
      dll_serviceReq(sessionId, (int)code, destUri);
      return true;
    }

    public bool dialDtmf(int sessionId, int mode, string digits)
    {
      dll_dialDtmf(sessionId, digits, mode);
      return true;
    }

    #endregion Methods

  }

  /// <summary>
  /// 
  /// </summary>
  public class CSipCommonProxy : CCommonProxyInterface
  {  

    #region Wrapper functions
    // callback delegates
    delegate int GetConfigData(int cfgId);
    delegate int OnRegStateChanged(int accountId, int regState);
    delegate int OnCallStateChanged(int callId, int stateId);
    delegate int OnCallIncoming(int callId, string number);
    delegate int OnCallHoldConfirm(int callId);
    delegate int OnMessageReceivedCallback(string from, string message);
    delegate int OnBuddyStatusChangedCallback(int buddyId, int status, string statusText);

    [DllImport("pjsipDll.dll")]
    private static extern int dll_init(int listenPort);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_main();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_shutdown();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_registerAccount(string uri, string reguri, string domain, string username, string password);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_addBuddy(string uri, bool subscribe);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_removeBuddy(int buddyId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_sendMessage(int buddyId, string uri, string message);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_setStatus(int accId, int presence_state);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_removeAccounts();

    // call API callbacks
    [DllImport("pjsipDll.dll")]
    private static extern int onCallStateCallback(OnCallStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onRegStateCallback(OnRegStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onCallIncoming(OnCallIncoming cb);
    [DllImport("pjsipDll.dll")]
    private static extern int getConfigDataCallback(GetConfigData cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onCallHoldConfirmCallback(OnCallHoldConfirm cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onMessageReceivedCallback(OnMessageReceivedCallback cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onBuddyStatusChangedCallback(OnBuddyStatusChangedCallback cb);

    // tones api
    [DllImport("pjsipDll.dll")]
    private static extern int dll_playTone(int toneId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_stopTone();

    #endregion Wrapper functions

    #region Variables

    static OnCallStateChanged csDel = new OnCallStateChanged(onCallStateChanged);
    static OnRegStateChanged rsDel = new OnRegStateChanged(onRegStateChanged);
    static OnCallIncoming ciDel = new OnCallIncoming(onCallIncoming);
    //static GetConfigData gdDel = new GetConfigData(getConfigData);
    static OnCallHoldConfirm chDel = new OnCallHoldConfirm(onCallHoldConfirm);
    static OnMessageReceivedCallback mrdel = new OnMessageReceivedCallback(onMessageReceived);
    static OnBuddyStatusChangedCallback bscdel = new OnBuddyStatusChangedCallback(onBuddyStatusChanged);

    private static CCallManager CallManager
    {
      get { return CCallManager.getInstance(); }
    }

    #endregion Variables

    #region Methods

    public int initialize()
    {
      // register callbacks (delegates)
      onCallIncoming( ciDel );
      onCallStateCallback( csDel );
      onRegStateCallback( rsDel );
      onCallHoldConfirmCallback(chDel);
      onBuddyStatusChangedCallback(bscdel);
      onMessageReceivedCallback(mrdel);

      // Initialize pjsip...
      int status = start();
      return status;
    }

    public int shutdown()
    {
      return dll_shutdown();
    }

    public int start()
    {
      int status = -1;
      
      int port = CallManager.SipPort;

      status = dll_init(port);
      status |= dll_main();
      return status;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    // Call API
    //
    public int registerAccounts()
    {
      return registerAccounts(false);
    }

    public int registerAccounts(bool renew)
    {
      if (renew == true)
      {
        dll_removeAccounts();
      } 

      for (int i = 0; i < CallManager.NumAccounts; i++)
      {
        // reset account state
        CallManager.setAccountState(i, 0);

        if (CallManager.getId(i).Length > 0)
        {
          if (CallManager.getAddress(i) == "0") continue;

          string displayName = CallManager.getDisplayName(); 
          // Publish do not work if display name in uri 
          string uri = displayName + "<sip:" + CallManager.getId(i) + "@" + CallManager.getAddress(i) + ">";
          //string uri = "sip:" + Manager.getId(i) + "@" + Manager.getAddress(i) + "";
          string reguri = "sip:" + CallManager.getAddress(i); // +":" + CCallManager.getInstance().SipProxyPort;

          string domain = CallManager.getDomain(i);
          string username = CallManager.getUsername(i);
          string password = CallManager.getPassword(i);

          dll_registerAccount(uri, reguri, domain, username, password);

          // todo:::check if accId corresponds to account index!!!
        }
      }
      return 1;
    }

    // Buddy list handling
    public int addBuddy(string ident)
    {
      string uri = "sip:" + ident + "@" + CallManager.getAddress();
      return dll_addBuddy(uri, true);
    }

    public int delBuddy(int buddyId)
    {
      return dll_removeBuddy(buddyId);
    }

    public int sendMessage(string dest, string message)
    {
      string uri = "sip:" + dest + "@" + CallManager.getAddress();
      return dll_sendMessage(CallManager.DefaultAccountIndex, uri, message);
    }

    public int setStatus(int accId, EUserStatus status)
    {
      return dll_setStatus(accId, (int)status);
    }

    #endregion Methods

    #region Callbacks

    private static int onCallStateChanged(int callId, int callState)
    {
//    PJSIP_INV_STATE_NULL,	    /**< Before INVITE is sent or received  */
//    PJSIP_INV_STATE_CALLING,	    /**< After INVITE is sent		    */
//    PJSIP_INV_STATE_INCOMING,	    /**< After INVITE is received.	    */
//    PJSIP_INV_STATE_EARLY,	    /**< After response with To tag.	    */
//    PJSIP_INV_STATE_CONNECTING,	    /**< After 2xx is sent/received.	    */
//    PJSIP_INV_STATE_CONFIRMED,	    /**< After ACK is sent/received.	    */
//    PJSIP_INV_STATE_DISCONNECTED,   /**< Session is terminated.		    */
      if (callState == 2) return 0;

      CStateMachine sm = CallManager.getCall(callId);
      if (sm == null) return 0;

      switch (callState)
      {
        case 1:
          //sm.getState().onCalling();
          break;
        case 2:
          //sm.getState().incomingCall("4444");
          break;
        case 3:
          sm.getState().onAlerting();
          break;
        case 4:
          sm.getState().onConnect();
          break;
        case 6:
          sm.getState().onReleased();
          break;
      }
      return 1;
    }

    private static int onCallIncoming(int callId, string uri)
    {
      string number = uri.Replace("<sip:","");

      int atPos = number.IndexOf('@');
      if (atPos >= 0)
      {
        number = number.Remove(atPos);
      }
      else 
      { 
        int semiPos = number.IndexOf(';');
        if (semiPos >= 0)
        {
          number = number.Remove(semiPos);
        }
        else 
        {
          int colPos = number.IndexOf(':');
          if (colPos >= 0)
          {
            number = number.Remove(colPos);
          }
        }

      }

      CStateMachine sm = CallManager.createSession(callId, number);
      sm.getState().incomingCall(number);
      return 1;
    }


    private static int onRegStateChanged(int accId, int regState)
    {
      CallManager.setAccountState(accId, regState);
      return 1;
    }


    private static int onCallHoldConfirm(int callId)
    {
      CStateMachine sm = CallManager.getCall(callId);
      if (sm != null) sm.getState().onHoldConfirm();
      return 1;
    }

    //////////////////////////////////////////////////////////////////////////////////

    private static int onMessageReceived(string from, string message)
    {
      CallManager.setMessageReceived(from, message);
      return 1;
    }

    private static int onBuddyStatusChanged(int buddyId, int status, string text)
    {
      CallManager.setBuddyState(buddyId, status, text);
      return 1;
    }

    #endregion Callbacks

  }


  // internal class
  public class CMediaPlayerProxy : CMediaProxyInterface
  {
    [DllImport("WinMM.dll")]
    public static extern bool PlaySound(string fname, int Mod, int flag);

    // these are the SoundFlags we are using here, check mmsystem.h for more
    public int SND_ASYNC = 0x0001; // play asynchronously
    public int SND_FILENAME = 0x00020000; // use file name
    public int SND_PURGE = 0x0040; // purge non-static events
    public int SND_LOOP = 0x0008;  // loop the sound until next sndPlaySound 

    public int playTone(ETones toneId)
    {
      string fname;
      int SoundFlags = SND_FILENAME | SND_ASYNC | SND_LOOP;

      switch (toneId)
      {
        case ETones.EToneDial:
          fname = "sounds\\dial.wav";
          break;
        case ETones.EToneCongestion:
          fname = "sounds\\congestion.wav";
          break;
        case ETones.EToneRingback:
          fname = "sounds\\ringback.wav";
          break;
        case ETones.EToneRing:
          fname = "sounds\\ring.wav";
          break;
        default:
          fname = "";
          break;
      }

      PlaySound(fname, 0, SoundFlags);
      return 1;
    }

    public int stopTone()
    {
      PlaySound(null, 0, SND_PURGE);
      return 1;
    }
  }
} // namespace Sipek