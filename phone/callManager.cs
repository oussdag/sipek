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
using MenuDesigner;


namespace Sipek
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

    #endregion


    #region Properties

    public int Count
    {
      get { return _calls.Count; }
    }

    public string SipProxy
    {
      get { return CAccounts.getInstance()[0].Address; }
    }
    public int SipProxyPort
    {
      get { return CAccounts.getInstance()[0].Port; }
    }
    public string SipName
    {
      get { return CAccounts.getInstance()[0].Name; }
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

    private CCallManager()
    {
    }
    #endregion Constructor


    #region Methods


    public void initialize()
    {
      // Create menu pages...
      new CConnectingPage();
      new CRingingPage();
      new CReleasedPage();
      new CActivePage();
      new CDialPage();
      new CIncomingPage();
      new CPreDialPage();


      // init SIP
      CCallProxy.initialize();
      CCallProxy.registerAccount(0);

      // Initialize call table
      _calls = new Dictionary<int, CStateMachine>();
    }

    public void shutdown()
    {
      CCallProxy.shutdown();
    }

    public void updateGui()
    {
      // get current session
      if (_currentSession < 0)
      {
        CComponentController.getInstance().showPage((int)2);
        return;
      }

      CStateMachine call = getCall(_currentSession);
      switch (call.getStateId())
      {
        case CAbstractState.EStateId.IDLE:
          CComponentController.getInstance().showPage((int)2);
          break;
        case CAbstractState.EStateId.CONNECTING:
          CComponentController.getInstance().showPage((int)ECallPages.P_CONNECTING);
          break;
        case CAbstractState.EStateId.ALERTING:
           CComponentController.getInstance().showPage((int)ECallPages.P_RINGING);
          break;
        case CAbstractState.EStateId.ACTIVE:
          CComponentController.getInstance().showPage((int)ECallPages.P_ACTIVE);
          break;
        case CAbstractState.EStateId.RELEASED:
          CComponentController.getInstance().showPage((int)ECallPages.P_RELEASED);
          break;
        case CAbstractState.EStateId.INCOMING:
          CComponentController.getInstance().showPage((int)ECallPages.P_INCOMING);
          break;
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


    public void createSession(string number)
    {
      CStateMachine call = createCall();
      int newsession = call.getState().makeCall(number);
      if (newsession != -1)
      {
        call.Session = newsession;
        _calls.Add(newsession, call);
        _currentSession = newsession;
      }
      updateGui();
    }
    
    public CStateMachine createSession(int sessionId, string number)
    {
      CStateMachine call = createCall();
      call.Session = sessionId;
      _calls.Add(sessionId, call);
      _currentSession = sessionId;
      updateGui();
      return call;
    }

     public void destroySession(int session)
    {
      _calls.Remove(_currentSession);
      if (_calls.Count == 0)
      {
        _currentSession = -1;
      }
      else 
      {
        // select other session
        _currentSession = _calls.GetEnumerator().Current.Key;
      }
    }

    public void onUserRelease()
    {
      CStateMachine call = getCall(_currentSession);
      call.getState().endCall();
    }

    public void onUserAnswer()
    {
      CStateMachine call = getCall(_currentSession);
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

    private CStateMachine createCall()
    {
      CStateMachine call = new CStateMachine(new CCallProxy(0));
      return call;
    }

    public void destroy(int session)
    {
      _calls.Remove(session);
    }

    #endregion Methods

  }

} // namespace Sipek
