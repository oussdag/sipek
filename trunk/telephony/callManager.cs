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
using System;

namespace Telephony
{

  public delegate void TimerExpiredCallback(object sender, EventArgs e);

  public abstract class ITimer
  {
    public abstract void Start();
    public abstract void Stop();

    public abstract int Interval { get; set;}

    public abstract TimerExpiredCallback Elapsed { set;}

  }

  public abstract class IConfiguratorInterface
  {
    public abstract bool CFUFlag { get;  }
    public abstract string CFUNumber { get; }
    public abstract bool CFNRFlag { get; }
    public abstract string CFNRNumber { get; }
    public abstract bool DNDFlag { get; }
    public abstract bool AAFlag { get; }
    public abstract int SipPort { get; }   
  }

  public interface AbstractFactory
  {
    // factory methods
    ITimer createTimer();
    
    ICallProxyInterface createCallProxy();

    // getters
    IMediaProxyInterface getMediaProxy();

    ICallLogInterface getCallLogger();

    IConfiguratorInterface getConfigurator();

    ICommonProxyInterface getCommonProxy();
  }

  //////////////////////////////////////////////////////////////////////////
  #region Null patterns
  public class NullTimer : ITimer
  {
    #region ITimer Members
    public override void Start() { }
    public override void Stop() { }
    public override int Interval
    {
      get { return 100; }
      set { }
    }

    public override TimerExpiredCallback Elapsed
    {
      set { }
    }
    #endregion
  }

  public class NullConfigurator : IConfiguratorInterface
  {
    #region IConfiguratorInterface Members

    public override bool CFUFlag
    {
      get { return false; }
    }

    public override string CFUNumber
    {
      get { return ""; }
    }

    public override bool CFNRFlag
    {
      get { return false; }
    }

    public override string CFNRNumber
    {
      get { return ""; }
    }

    public override bool DNDFlag
    {
      get { return false; }
    }

    public override bool AAFlag
    {
      get { return false; }
    }

    public override int SipPort
    {
      get { return 5060; }
    }
    #endregion
  }

  public class NullCallProxy : ICallProxyInterface
  {
    #region ICallProxyInterface Members

    public int makeCall(string dialedNo, int accountId)
    {
      return 1;
    }

    public bool endCall(int sessionId)
    {
      return false;
    }

    public bool alerted(int sessionId)
    {
      return false;
    }

    public bool acceptCall(int sessionId)
    {
      return false;
    }

    public bool holdCall(int sessionId)
    {
      return false;
    }

    public bool retrieveCall(int sessionId)
    {
      return false;
    }

    public bool xferCall(int sessionId, string number)
    {
      return false;
    }

    public bool xferCallSession(int sessionId, int session)
    {
      return false;
    }

    public bool threePtyCall(int sessionId, int session)
    {
      return false;
    }

    public bool serviceRequest(int sessionId, int code, string dest)
    {
      return false;
    }

    public bool dialDtmf(int sessionId, string digits, int mode)
    {
      return false;
    }

    #endregion
  }

  public class NullCommonProxy : ICommonProxyInterface
  {
    #region ICommonProxyInterface Members

    public int initialize()
    {
      return 1;    
    }

    public int shutdown()
    {
      return 1;
    }

    public int registerAccounts()
    {
      return 1;
    }

    public int registerAccounts(bool renew)
    {
      return 1;
    }

    public int addBuddy(string ident)
    {
      return 1;
    }

    public int delBuddy(int buddyId)
    {
      return 1;
    }

    public int sendMessage(string dest, string message)
    {
      return 1;
    }

    public int setStatus(int accId, EUserStatus presence_state)
    {
      return 1;
    }

    #endregion
  }

  public class NullMediaProxy : IMediaProxyInterface
  {
    #region IMediaProxyInterface Members

  public int  playTone(ETones toneId)
  {
    return 1;
  }

  public int  stopTone()
  {
    return 1;
  }

  #endregion
  }

  public class NullCallLogger : ICallLogInterface
  {
    #region ICallLogInterface Members

    public void  addCall(ECallType type, string number, string name, DateTime time, TimeSpan duration)
    {
    }

    public void  save()
    {
    }

    public Stack<CCallRecord>  getList()
    {
 	    return new Stack<CCallRecord>();
    }

    public Stack<CCallRecord>  getList(ECallType type)
    {
      return new Stack<CCallRecord>();
    }

    public void  deleteRecord(CCallRecord record)
    {
    }

    #endregion
  }


  /// <summary>
  /// Null Factory implementation
  /// </summary>
  public class NullFactory : AbstractFactory
  {
    IConfiguratorInterface _config = new NullConfigurator();
    ICommonProxyInterface _common = new NullCommonProxy();
    IMediaProxyInterface _media = new NullMediaProxy();
    ICallLogInterface _logger = new NullCallLogger();

    #region AbstractFactory members
    // factory methods
    public ITimer createTimer()
    {
      return new NullTimer();
    }

    //TODO
    public ICallProxyInterface createCallProxy() 
    {
      return new NullCallProxy();
    }

    public ICommonProxyInterface getCommonProxy()
    {
      return _common;
    }

    public IConfiguratorInterface getConfigurator()
    {
      return _config;
    }

    // Implement getters
    public IMediaProxyInterface getMediaProxy() 
    { 
      return _media; 
    }

    public ICallLogInterface getCallLogger() 
    { 
      return _logger;
    }
    #endregion
  }

  #endregion

  //////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// CCallManager
  /// Main telephony class
  /// </summary>
  public class CCallManager
  {
    #region Variables

    private static CCallManager _instance = null;

    private Dictionary<int, CStateMachine> _calls;  //!< Call table

    private AbstractFactory _factory = new NullFactory();

    delegate void PendingDelegate(int sessionId); // for onUserAnswer

    PendingAction _pendingAction;
 
    #endregion


    #region Properties

    public AbstractFactory Factory
    {
      get { return _factory; }
      set { _factory = value; }
    }

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

    public int Count
    {
      get { return _calls.Count; }
    }

    public bool Is3Pty
    {
      get 
      {
        return (getNoCallsInState(EStateId.ACTIVE) == 2) ? true : false;
      }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Configuration settings


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

    protected CCallManager()
    {
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

    class PendingAction
    {
      private PendingDelegate _delegate;
      private int _sessionId;
      private string _number;

      public PendingAction(PendingDelegate fctr, int sessionId)
        : this(fctr, sessionId, "")
      { }

      public PendingAction(PendingDelegate fctr, int sessionId, string number)
      {
        _delegate = fctr;
        _sessionId = sessionId;
        _number = number;
      }

      public void Activate()
      {
        if (null != _delegate) _delegate(_sessionId);
        _delegate = null;
      }
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
        
        Factory.getCommonProxy().initialize();

        Factory.getCommonProxy().registerAccounts(false);
      }
      else
      {       
        // reregister 
        Factory.getCommonProxy().registerAccounts(true); 
      }
      _initialized = true;
      return 1;
    }

    public void shutdown()
    {
      this.CallList.Clear();
      Factory.getCommonProxy().shutdown();
    }

    /////////////////////////////////////////////////////////////////////////////////////////
    // Call handling routines

    /// <summary>
    /// Handler for outgoing calls (accountId is not known).
    /// </summary>
    /// <param name="number">Number to call</param>
    public CStateMachine createOutboundCall(string number)
    {
      int accId = CAccounts.getInstance().DefAccount.Index;
      return this.createOutboundCall(number, accId);
    }

    /// <summary>
    /// Handler for outgoing calls (sessionId is not known yet).
    /// </summary>
    /// <param name="number">Number to call</param>
    /// <param name="accountId">Specified account Id </param>
    public CStateMachine createOutboundCall(string number, int accountId)
    {
      // check if current call automatons allow session creation.
      // if at least 1 connected try to put it on hold
      if (this.getNoCallsInState(EStateId.ACTIVE) == 0)
      {
        // create state machine
        // TODO check max calls!!!!
        CStateMachine call = new CStateMachine(this);

        // make call request (stack provides new sessionId)
        int newsession = call.getState().makeCall(number, accountId);
        if (newsession == -1)
        {
          return null;
        }
        // update call table
        call.Session = newsession;
        _calls.Add(newsession, call);
        return call;
      }
      else // we have at least one ACTIVE call
      {
        // put connected call on hold
        // TODO
      }
      return null;
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
      CStateMachine call = new CStateMachine(this);

      if (null == call) return null; 

      // save session parameters
      call.Session = sessionId;
      _calls.Add(sessionId, call);
      
      // notify GUI
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public CStateMachine getCall(int session)
    {
      if ((_calls.Count == 0) || (!_calls.ContainsKey(session))) return null;
      return _calls[session];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="stateId"></param>
    /// <returns></returns>
    public CStateMachine getCallInState(EStateId stateId)
    {
      if (_calls.Count == 0)  return null;
      foreach (KeyValuePair<int, CStateMachine> call in _calls)
      {
        if (call.Value.getStateId() == stateId) return call.Value;
      }
      return null;
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
      // should not be more than 1 call active
      if (list.Count > 0)
      {
        // put it on hold
        CStateMachine sm = list[0];
        if (null != sm) sm.getState().holdCall(sm.Session);

        // set ANSWER event pending for HoldConfirm
        // TODO
        _pendingAction = new PendingAction(onUserAnswer, session);
        return;
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
        // execute retrieve
        // check if any ACTIVE calls
        if (this.getNoCallsInState(EStateId.ACTIVE) > 0)
        {
          // get 1st and put it on hold
          CStateMachine sm = ((List<CStateMachine>)enumCallsInState(EStateId.ACTIVE))[0];
          if (null != sm) sm.getState().holdCall(sm.Session);

          // set Retrieve event pending for HoldConfirm
          _pendingAction = new PendingAction(onUserHoldRetrieve, session);
          return;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    public void onUserConference(int session)
    {
      // check preconditions: 1 call active, other held
      // 1st if current call is held -> search if any active -> execute retrieve
      if ((getNoCallsInState(EStateId.ACTIVE) == 1)&&(getNoCallsInState(EStateId.HOLDING) >= 1))
      {
        CStateMachine call = getCallInState(EStateId.HOLDING);
        call.getState().retrieveCall(call.Session);
        // set conference flag
        return;
      }
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

    ////////////////////////////////////////////////////////////////////////////////////////
    public void activatePendingAction()
    {
      if (null != _pendingAction) _pendingAction.Activate();
      _pendingAction = null;
    }

    #endregion Methods

  }

} // namespace Sipek
