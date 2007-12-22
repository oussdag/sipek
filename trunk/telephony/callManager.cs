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

using System.Collections;
using System.Collections.Generic;

namespace Telephony
{
  /// <summary>
  /// 
  /// </summary>
  public class CCallManager
  {
    #region Variables

    private static CCallManager _instance = null;

    private Dictionary<int, CStateMachine> _calls;  //!< Call table

    #endregion


    #region Properties

    public CStateMachine this[int index]
    {
      get
      {
        if (!_calls.ContainsKey(index)) return null;
        return _calls[index];
      }
    }

    public Dictionary<int, CStateMachine> CallList
    {
      get { return _calls; }
    }

    private static CCommonProxyInterface _sipCommonProxy;
    public static CCommonProxyInterface CommonProxy
    {
      get { return _sipCommonProxy; }
      set { _sipCommonProxy = value; }
    }

    private static CCallProxyInterface _sipCallProxy;
    public static CCallProxyInterface CallProxy
    {
      get { return _sipCallProxy; }
      set { _sipCallProxy = value; }
    }

    private static CMediaProxyInterface _mediaProxy;
    public static CMediaProxyInterface MediaProxy
    {
      get { return _mediaProxy; }
      set { _mediaProxy = value; }
    }

    private static ICallLogInterface _callLogInstance;
    public static ICallLogInterface CallLog
    {
      get { return _callLogInstance; }
      set { _callLogInstance = value; }
    }

    public int Count
    {
      get { return _calls.Count; }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Configuration settings
    public bool CFUFlag
    {
      get { return CSettings.CFU;  }
    }

    public string CFUNumber
    {
      get { return CSettings.CFUNumber;  }
    }

    public bool DNDFlag
    {
      get { return CSettings.DND;  }
    }
    public bool AAFlag
    {
      get { return CSettings.AA; }
    }
    public int SipPort
    {
      get { return 5060; }
    }   

    /////////////////////////////////////////////////////////////////////
    // Accounts
    public int DefaultAccountIndex
    {
      get { return CAccounts.getInstance().DefAccount.Index; }
    }

    public string getAddress()
    {
      return getAddress(this.DefaultAccountIndex);
    }

    public string getAddress(int accId)
    {
      return CAccounts.getInstance()[accId].Address;
    }

    public string getId(int accId)
    {
      return CAccounts.getInstance()[accId].Id;
    }
    public string getUsername(int accId)
    {
      return CAccounts.getInstance()[accId].Username;
    }
    public string getPassword(int accId)
    {
      return CAccounts.getInstance()[accId].Password;
    }
    public string getDomain(int accId)
    {
      return CAccounts.getInstance()[accId].Domain;
    }

    public string getDisplayName()
    {
      return CAccounts.getInstance()[this.DefaultAccountIndex].DisplayName;
    }

    public int NumAccounts
    {
      get { return CAccounts.getInstance().getSize(); }
    }

    ///////////////////////////////////////////////////////////////////////////
    private bool _initialized = false;
    public bool isInitialized
    {
      get { return _initialized; }
    }

    #endregion Properties


    #region Constructor

    public static CCallManager getInstance()
    { 
      if (_instance == null)
      {
        _instance = new CCallManager();
      }
      return _instance;
    }

    #endregion Constructor

    #region Events

    public delegate void CallStateChangedDelegate();  // define callback type 
    public delegate void MessageReceivedCallbackDelegate(string from, string message);  // define callback type 
    public delegate void BuddyStatusCallbackDelegate(int buddyId, int status, string text);  // define callback type 

    public event CallStateChangedDelegate CallStateChanged;
    public event MessageReceivedCallbackDelegate MessageReceived;
    public event BuddyStatusCallbackDelegate BuddyStatusChanged;

    // dummy callback method in case no other registered
    private void dummy()
    {
    }

    #endregion Events


    #region Methods

    ///////////////////////////////////////////////////////////////////
    /// Common routines

    public int initialize()
    {
      ///
      if (!isInitialized)
      {
        CallStateChanged += dummy;

        // Initialize call table
        _calls = new Dictionary<int, CStateMachine>(); 
        
        _sipCommonProxy.initialize();
      
        _sipCommonProxy.registerAccounts(false);
      }
      else
      {       
        // reregister 
        _sipCommonProxy.registerAccounts(true); 
      }
      _initialized = true;
      return 1;
    }

    public void shutdown()
    {
      _sipCommonProxy.shutdown();
    }

    /////////////////////////////////////////////////////////////////////////////////////////
    // Call handling routines

    /// <summary>
    /// Create an instance of state machine
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    private CStateMachine createCall(int sessionId)
    {
      CStateMachine call = new CStateMachine(this, CallProxy, MediaProxy);
      return call;
    }

    /// <summary>
    /// Handler for outgoing calls (accountId is not known).
    /// </summary>
    /// <param name="number">Number to call</param>
    public CStateMachine createSession(string number)
    {
      int accId = CAccounts.getInstance().DefAccount.Index;
      return this.createSession(number, accId);
    }

    /// <summary>
    /// Handler for outgoing calls (sessionId is not known yet).
    /// </summary>
    /// <param name="number">Number to call</param>
    /// <param name="accountId">Specified account Id </param>
    public CStateMachine createSession(string number, int accountId)
    {
      CStateMachine call = createCall(0);
      int newsession = call.getState().makeCall(number, accountId);
      if (newsession != -1)
      {
        call.Session = newsession;
        _calls.Add(newsession, call);
      }

      // Call callback method to update GUI
      updateGui();

      return call;
    }
    
    /// <summary>
    /// Handler for incoming calls (sessionId is known).
    /// Check for forwardings or DND
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="number"></param>
    /// <returns>call instance</returns>
    public CStateMachine createSession(int sessionId, string number)
    {
      CStateMachine call = createCall(sessionId);

      if (null == call) return null; 

      // save session parameters
      call.Session = sessionId;
      _calls.Add(sessionId, call);
      
      updateGui();

      return call;
    }

    /// <summary>
    /// Destroy call 
    /// </summary>
    /// <param name="session">session identification</param>
    public void destroySession(int session)
    {
      _calls.Remove(session);

      updateGui();
    }

    public CStateMachine getCall(int session)
    {
      if ((_calls.Count == 0) || (!_calls.ContainsKey(session))) return null;
      return _calls[session];
    }

    public int getNoCallsInState(EStateId stateId)
    {
      int cnt = 0;
      foreach (KeyValuePair<int, CStateMachine> kvp in _calls)
      {
        if (stateId == kvp.Value.getStateId())
        {
          cnt++;
        }
      }
      return cnt;
    }

    /// <summary>
    /// Collect statemachines being in a given state
    /// </summary>
    /// <param name="stateId">state machine state</param>
    /// <returns>List of state machines</returns>
    public ICollection<CStateMachine> enumCallsInState(EStateId stateId)
    {
      List<CStateMachine> list = new List<CStateMachine>();

      foreach (KeyValuePair<int, CStateMachine> kvp in _calls)
      {
        if (stateId == kvp.Value.getStateId())
        {
          list.Add(kvp.Value);
        }
      }
      return list;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // User handling routines

    /// <summary>
    /// User triggers a call release for a given session
    /// </summary>
    /// <param name="session">session identification</param>
    public void onUserRelease(int session)
    {
      this[session].getState().endCall(session);
    }

    /// <summary>
    /// User accepts call for a given session
    /// In case of multi call put current active call to Hold
    /// </summary>
    /// <param name="session">session identification</param>
    public void onUserAnswer(int session)
    {
      List<CStateMachine> list = (List<CStateMachine>)this.enumCallsInState(EStateId.ACTIVE);
      // should not be more than 1
      if (list.Count > 0)
      {
        // put it on hold
        CStateMachine sm = list[0];
        if (null != sm) sm.getState().holdCall(sm.Session);
      }
      this[session].getState().acceptCall(session);
    }

    /// <summary>
    /// User put call on hold or retrieve 
    /// </summary>
    /// <param name="session">session identification</param>
    public void onUserHoldRetrieve(int session)
    {
      // check Hold or Retrieve
      CAbstractState state = this[session].getState();
      if (state.StateId == EStateId.ACTIVE)
      {
        this.getCall(session).getState().holdCall(session);
      }
      else if (state.StateId == EStateId.HOLDING)
      {
        List<CStateMachine> list = (List<CStateMachine>)this.enumCallsInState(EStateId.ACTIVE);
        // should not be more than 1
        if (list.Count > 0)
        {
          // put it on hold
          CStateMachine sm = list[0];
          if (null != sm) sm.getState().holdCall(sm.Session);
        }
        this[session].getState().retrieveCall(session);
      }
      else
      {
        // illegal
      }
    }

    /// <summary>
    /// User starts a call transfer
    /// </summary>
    /// <param name="session">session identification</param>
    /// <param name="number">number to transfer</param>
    public void onUserTransfer(int session, string number)
    {
      this[session].getState().xferCall(session, number);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="digits"></param>
    /// <param name="mode"></param>
    public void onUserDialDigit(int session, string digits, int mode)
    {
      this[session].getState().dialDtmf(session, digits, 0);
    }
    /////////////////////////////////////////////////////////////////////////
    // Callback handlers

    /// <summary>
    /// Inform GUI to be refreshed 
    /// </summary>
    public void updateGui()
    {
      if (null != CallStateChanged) CallStateChanged();
    }
    
    /// <summary>
    /// Account state changed
    /// </summary>
    /// <param name="accId">account identification</param>
    /// <param name="regState">state of account</param>
    public void setAccountState(int accId, int regState)
    {
      CAccounts.getInstance()[accId].RegState = regState;
      updateGui();
    }
    
    /// <summary>
    /// Inform about new incoming message 
    /// </summary>
    /// <param name="from">identification of sender</param>
    /// <param name="message">content of message</param>
    public void setMessageReceived(string from, string message)
    {
      if (MessageReceived!= null) MessageReceived(from, message);
    }

    /// <summary>
    /// Buddy presence state changed
    /// </summary>
    /// <param name="buddyId">buddy identification</param>
    /// <param name="status">buddy status</param>
    /// <param name="text">buddy status additional text</param>
    public void setBuddyState(int buddyId, int status, string text)
    {
      if (BuddyStatusChanged != null) BuddyStatusChanged(buddyId, status, text);
    }

    #endregion Methods

  }

} // namespace Sipek
