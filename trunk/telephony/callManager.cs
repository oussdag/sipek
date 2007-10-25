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

    private int _currentSession = -1;

    private bool _initialized = false;


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

    #endregion


    #region Methods

    public bool isInitialized()
    {
      return _initialized;
    }

    public int initialize()
    {
      ///
      if (!_initialized)
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

    public void updateGui()
    {
      if (null != CallStateChanged) CallStateChanged();
    }

    public CStateMachine getCurrentCall()
    {
      if (_currentSession < 0) return null;
      return _calls[_currentSession];
    }

    public int getCurrentCallIndex()
    {
      int index = 0;
      foreach (KeyValuePair<int, CStateMachine> kvp in _calls)
      { 
        index++;
        if (kvp.Value.Session == _currentSession) break;
      }
      return index;
    }

    public CStateMachine getCall(int session)
    {
      if ((_calls.Count == 0)||(!_calls.ContainsKey(session))) return null;
      return _calls[session];
    }

    public Dictionary<int, CStateMachine> getCallList()
    {
      return _calls;
    }

    /// <summary>
    /// Handler for outgoing calls (sessionId is not known yet).
    /// </summary>
    /// <param name="number"></param>
    public CStateMachine createSession(string number)
    {
      int accId = CAccounts.getInstance().DefAccount.Index;
      return this.createSession(number, accId);
    }

    /// <summary>
    /// Handler for outgoing calls (sessionId is not known yet).
    /// </summary>
    /// <param name="number"></param>
    /// <param name="accountId">Specified account Id </param>
    public CStateMachine createSession(string number, int accountId)
    {
      CStateMachine call = createCall(0);
      int newsession = call.getState().makeCall(number, accountId);
      if (newsession != -1)
      {
        call.Session = newsession;
        _calls.Add(newsession, call);
        _currentSession = newsession;
      }
      
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
      call.Session = sessionId;
      _calls.Add(sessionId, call);
      _currentSession = sessionId;
      updateGui();
      return call;
    }

    public void destroySession(int session)
    {
      _calls.Remove(session);
      if (_calls.Count == 0)
      {
        _currentSession = -1;
      }
      else 
      {
        // select other session
        _currentSession = _calls.GetEnumerator().Current.Key;
      }
      updateGui();
    }

    public void onUserRelease()
    {
      onUserRelease(_currentSession);
    }

    public void onUserRelease(int session)
    {
      this[session].getState().endCall(session);
    }

    public void onUserAnswer()
    {
      onUserAnswer(_currentSession);
    }

    /// <summary>
    /// Accept call
    /// In case of multi call put current active call to Hold
    /// </summary>
    /// <param name="session"></param>
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


    /////////////////////////////////////////////////////////////////////

    private CStateMachine createCall(int sessionId)
    {
      CStateMachine call = new CStateMachine(this, CallProxy, MediaProxy);
      return call;
    }

    public void destroy(int session)
    {
      _calls.Remove(session);
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

    /////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accId"></param>
    /// <param name="regState"></param>
    public void setAccountState(int accId, int regState)
    {
      CAccounts.getInstance()[accId].RegState = regState;
      updateGui();
    }
    
    public void SetMessageReceived(string from, string message)
    {
      if (MessageReceived!= null) MessageReceived(from, message);
    }

    public void setBuddyState(int buddyId, int status, string text)
    {
      if (BuddyStatusChanged != null) BuddyStatusChanged(buddyId, status, text);
    }

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

    public void onUserTransfer(int session, string number)
    {
      this.getCall(session).getState().xferCall(session, number);
    }

    #endregion Methods

  }




} // namespace Sipek
