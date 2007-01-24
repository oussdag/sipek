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


using System.Collections.Generic;
using MenuDesigner;


namespace Telephony
{

  public class CCallManager
  {

    #region Variables

    private static CCallManager _instance = null;

    private Dictionary<int, CStateMachine> _calls;  //!< Call table

    private int _currentSession = 0;

    #endregion



    public static CCallManager getInstance()
    { 
      if (_instance == null)
      {
        _instance = new CCallManager();
      }
      return _instance;
    }

    #region Constructor

    private CCallManager()
    {
      _calls = new Dictionary<int, CStateMachine>();
    }
    #endregion Constructor


    #region Methods


    public void updateGui()
    {
      // get current session
      CStateMachine call = getCall(_currentSession);
      switch (call.getStateId())
      {
        case CAbstractState.EStateId.IDLE:
          CComponentController.getInstance().showPage((int)2);
          break;
        case CAbstractState.EStateId.CALLING:
          CComponentController.getInstance().showPage((int)ECallPages.P_CONNECTING);
          break;
        case CAbstractState.EStateId.ACTIVE:
          CComponentController.getInstance().showPage((int)ECallPages.P_ACTIVE);
          break;
        case CAbstractState.EStateId.RELEASED:
          CComponentController.getInstance().showPage((int)ECallPages.P_RELEASED);
          break;
      }
    }

    public void createSession(string number)
    {
      CStateMachine call = createCall();
      call.getState().makeCall(number);
    }

    public void destroySession()
    {
      CStateMachine call = getCall(_currentSession);
      call.getState().endCall();
      _calls.Remove(_currentSession);
    }


    public void onUserOffhook()
    { 
    }

    public void onUserOnhook()
    {
    }

    public void onUserRelease()
    {
    }

    public void onUserAnswer()
    {
    }

    /////////////////////////////////////////////////////////////////////

    private CStateMachine createCall()
    {
      CStateMachine call = new CStateMachine(new CSipProxy(0));
      _calls.Add(0, call);
      _currentSession = 0;
      return _calls[0];
    }

    public void destroy(int session)
    {
      _calls.Remove(session);
    }

    private CStateMachine getCall(int session)
    {
      return _calls[session];
    }

    #endregion Methods

  }

} // namespace Telephony
