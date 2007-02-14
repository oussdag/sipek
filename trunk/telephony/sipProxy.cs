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
  public class CSipProxy : CTelephonyInterface
  {    
    // incoming
    delegate int GetConfigData(int cfgId);
    delegate int OnRegStateChanged(int accountId, int regState);
    delegate int OnCallStateChanged(int callId, int stateId);
    delegate int OnCallIncoming(int callId, string number);
    /// outgoing
    delegate int DoItDelegate();
    delegate int DoMakeCallDelegate(int callId, string number);
    delegate int DoReleaseCallDelegate(int callId);
    delegate int DoAnswerCallDelegate(int callId, int code);
    delegate int DoRegisterDelegate(string uri, string reguri);

    [DllImport("pjsipDll.dll")]
    private static extern int dll_init();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_main();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_shutdown();
    [DllImport("pjsipDll.dll")]
    private static extern int onCallStateCallback(OnCallStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onRegStateCallback(OnRegStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int onCallIncoming(OnCallIncoming cb);
    [DllImport("pjsipDll.dll")]
    private static extern int getConfigDataCallback(GetConfigData cb);
    
    // call API
    [DllImport("pjsipDll.dll")]
    private static extern int dll_registerAccount(string uri, string reguri);

    [DllImport("pjsipDll.dll")]
    private static extern int dll_makeCall(int callId, string number);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_releaseCall(int callId);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_answerCall(int callId, int code);


    #region Variables
    
    // identify line
    private int _line;

    // used for single threaded communication with pjsip 
    static Synchronizer m_Synchronizer;

    //static DoItDelegate sddel;
    static OnCallStateChanged csDel;
    static OnRegStateChanged rsDel;
    static OnCallIncoming ciDel;
    static GetConfigData gdDel;

    #endregion Variables


    #region Constructor

    public CSipProxy(int line)
    {
      _line = line;
    }

    #endregion Constructor


    #region Methods

    public static void initialize()
    {
      // create pjsip thread synchronizer
      m_Synchronizer = new Synchronizer();

      ciDel = new OnCallIncoming(onCallIncoming);
      csDel = new OnCallStateChanged(onCallStateChanged);
      rsDel = new OnRegStateChanged(onRegStateChanged);
      gdDel = new GetConfigData(getConfigData);

      // register callbacks (delegates)
      onCallIncoming( ciDel );
      onCallStateCallback( csDel );
      onRegStateCallback( rsDel );

      // Initialize pjsip...
      m_Synchronizer.Invoke(new DoItDelegate(dll_init),null);
      m_Synchronizer.Invoke(new DoItDelegate(dll_main),null);
    }

    public static bool shutdown()
    {
      m_Synchronizer.Invoke(new DoItDelegate(dll_shutdown), null);
      m_Synchronizer.Dispose();
      return true;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    // Call API
    //
    public static int registerAccount(int accountId)
    {
      string uri = "sip:" + CCallManager.getInstance().SipName + "@" + CCallManager.getInstance().SipProxy;
      string reguri = "sip:" + CCallManager.getInstance().SipProxy + ":"+CCallManager.getInstance().SipProxyPort;
      object res = m_Synchronizer.Invoke(new DoRegisterDelegate(dll_registerAccount), new object[2] { uri, reguri });
      return 1;
    }

    public int makeCall(string dialedNo)
    {
      string uri = "sip:" + dialedNo + "@" + CCallManager.getInstance().SipProxy;
      object res = m_Synchronizer.Invoke(new DoMakeCallDelegate(dll_makeCall), new object[2] { 0, uri});
      _line = (int)res;
      return _line;
    }

    public bool endCall()
    {
      m_Synchronizer.Invoke(new DoReleaseCallDelegate(dll_releaseCall), new object[1] { _line });
      return true;
    }

    public bool alerted()
    {
      m_Synchronizer.Invoke(new DoAnswerCallDelegate(dll_answerCall), new object[2] { _line, 180 });
      return true;
    }

    public bool acceptCall()
    {
      m_Synchronizer.Invoke(new DoAnswerCallDelegate(dll_answerCall), new object[2] { _line, 200 });
      return true;
    }

    #endregion Methods


    #region Callbacks

    //private static int onCallStateChanged(int account, string number)
    private static int onCallStateChanged(int callId, int callState)
    {
//    PJSIP_INV_STATE_NULL,	    /**< Before INVITE is sent or received  */
//    PJSIP_INV_STATE_CALLING,	    /**< After INVITE is sent		    */
//    PJSIP_INV_STATE_INCOMING,	    /**< After INVITE is received.	    */
//    PJSIP_INV_STATE_EARLY,	    /**< After response with To tag.	    */
//    PJSIP_INV_STATE_CONNECTING,	    /**< After 2xx is sent/received.	    */
//    PJSIP_INV_STATE_CONFIRMED,	    /**< After ACK is sent/received.	    */
//    PJSIP_INV_STATE_DISCONNECTED,   /**< Session is terminated.		    */

      CStateMachine sm = CCallManager.getInstance().getCall(callId);
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

    private static int onCallIncoming(int callId, string number)
    {
      CStateMachine sm = CCallManager.getInstance().createSession(callId, number);
      sm.getState().incomingCall(number);
      return 1;
    }


    private static int onRegStateChanged(int accId, int regState)
    {
      return 1;
    }


    private static int getConfigData(int cfgId)
    {

      return 1;
    }

    #endregion Callbacks


  }

} // namespace Telephony
