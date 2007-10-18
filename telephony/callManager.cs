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

    private CCommonProxyInterface _sipCommonProxy;

    #endregion


    #region Properties

    public CCommonProxyInterface CommonProxy
    {
      get { return _sipCommonProxy; }
    }

    private CMediaProxyInterface _mediaProxy;
    public CMediaProxyInterface MediaProxy
    {
      get { return _mediaProxy; }
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

    private CCallManager()
    {
      // todo::: do abstraction!!!
      _sipCommonProxy = new CSipCommonProxy();
      _mediaProxy = new CMediaPlayerProxy();
      //_sipCommonProxy = new CSipSocketCommonProxy();
    }

    #endregion Constructor

    #region Events

    public delegate void CallStateChangedDelegate();  // define callback type 
    public delegate void MessageReceivedCallbackDelegate(string from, string message);  // define callback type 
    public delegate void BuddyStatusCallbackDelegate(int buddyId, int status);  // define callback type 

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
      }
      else
      {
        // todo unregister
        _sipCommonProxy.shutdown();
        _sipCommonProxy.initialize();
      }
      _sipCommonProxy.registerAccounts(); 
      _initialized = true;
      return 1;
    }

    public void shutdown()
    {
      _sipCommonProxy.shutdown();
    }

    public void updateGui()
    {
       CallStateChanged();
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
    public void createSession(string number)
    {
      int accId = CAccounts.getInstance().DefAccount.Index;
      this.createSession(number, accId);
    }

    /// <summary>
    /// Handler for outgoing calls (sessionId is not known yet).
    /// </summary>
    /// <param name="number"></param>
    /// <param name="accountId">Specified account Id </param>
    public void createSession(string number, int accountId)
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
      CStateMachine call = getCall(session);
      if (call != null) call.getState().endCall();
    }

    public void onUserAnswer()
    {
      onUserAnswer(_currentSession);
    }

    public void onUserAnswer(int session)
    {
      CStateMachine call = getCall(session);
      call.getState().acceptCall();
    }

    public void nextSession()
    {
      int newsession = _currentSession;
      bool stop = false;
      foreach (KeyValuePair<int, CStateMachine> kvp in _calls)
      {
        if (stop) 
        {
          newsession = kvp.Value.Session;
          break;
        }
        if (_currentSession == kvp.Value.Session) stop = true;
      }
      // in case last session is active choose first one
      if (newsession == _currentSession)
      {
        _currentSession = _calls.GetEnumerator().Current.Key;
      }
      else
      {
        _currentSession = newsession;
      }
      // update gui
      updateGui();
    }
    
    public void previousSession()
    {
      //
      foreach (KeyValuePair<int, CStateMachine> call in _calls)
      { 
        //call.
      }
    }
    /////////////////////////////////////////////////////////////////////

    private CStateMachine createCall(int sessionId)
    {
      CStateMachine call = new CStateMachine(this, new CSipCallProxy(sessionId), CommonProxy, MediaProxy);
      call.setCallLogInstance(CCallLog.getInstance());
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

    public void setBuddyState(int buddyId, int status)
    {
      if (BuddyStatusChanged != null) BuddyStatusChanged(buddyId, status);
    }

    public void HoldRetrieve(int session)
    {
      // check Hold or Retrieve
      CAbstractState state = this.getCall(session).getState();
      if (state.StateId == EStateId.ACTIVE)
      {
        this.getCall(session).getState().holdCall();
      }
      else if (state.StateId == EStateId.HOLDING)
      {
        this.getCall(session).getState().retrieveCall();
      }
      else
      { 
        // illegal
      }
    }    

    public void onUserTransfer(int session, string number)
    {
      this.getCall(session).getState().xferCall(number);
    }

    #endregion Methods

  }




} // namespace Sipek
