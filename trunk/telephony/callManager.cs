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

    public int Count
    {
      get { return _calls.Count; }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Configuration settings
    public bool CFUFlag
    {
      get { return false;  }
    }

    public string CFUNumber
    {
      get { return "";  }
    }

    public bool DNDFlag
    {
      get { return false;  }
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

    #region Callback

    CallbackDelegate onRefreshCallback; // function pointer variable
    public delegate void CallbackDelegate();  // define callback type 

    // dummy callback method in case no other registered
    private void dummy()
    {
    }

    /// <summary>
    /// Register callback function
    /// </summary>
    /// <param name="method"></param>
    public void registerOnRefreshCallback(CallbackDelegate method)
    {
      onRefreshCallback = new CallbackDelegate(method);
    }

    #endregion


    #region Methods

    public bool isInitialized()
    {
      return _initialized;
    }

    public void initialize()
    {
      ///
      if (!_initialized)
      {
        if (onRefreshCallback == null) onRefreshCallback = new CallbackDelegate(dummy);

        // Initialize call table
        _calls = new Dictionary<int, CStateMachine>(); 
        
        CPjSipProxy.initialize();
      }
      else
      {
        // todo unregister
        CPjSipProxy.restart();
      }
      CPjSipProxy.registerAccounts(); 
      _initialized = true;
    }

    public void shutdown()
    {
      CPjSipProxy.shutdown();
    }

    public void updateGui()
    {
      // get current session
      if (_currentSession < 0)
      {
        // todo::: showHomePage
        //CComponentController.getInstance().showPage(CComponentController.getInstance().HomePageId);
        onRefreshCallback();
        return;
      }

      CStateMachine call = getCall(_currentSession);
      if (call != null)
      {
        int stateId = (int)call.getStateId();
        if (stateId == (int)EStateId.IDLE)
        {
          if (Count == 0)
          {
            //CComponentController.getInstance().showPage(stateId);
            onRefreshCallback();
          }
          else
          {
            _calls.GetEnumerator().MoveNext();
            _currentSession = _calls.GetEnumerator().Current.Key;

            CStateMachine nextcall = getCall(_currentSession);
            //if (nextcall != null) CComponentController.getInstance().showPage((int)nextcall.getStateId());
            onRefreshCallback();
          }
        }
        else
        {
          //CComponentController.getInstance().showPage(stateId);
          onRefreshCallback();
        }
      }
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
      CStateMachine call = getCall(_currentSession);
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
      CStateMachine call = new CStateMachine(new CCallProxy(sessionId));
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

    #endregion Methods

  }




} // namespace Sipek
