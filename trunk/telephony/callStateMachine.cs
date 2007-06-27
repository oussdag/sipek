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


namespace Telephony
{
  /// <summary>
  /// CStateMachine class is a telephony data container for one call. It maintains call state, 
  /// communicates with signaling via proxy and informs GUI about signaling events.
  /// A Finite State Machine is implemented upon State design pattern!
  /// </summary>
  public class CStateMachine
  {
    #region Variables

    private CAbstractState _state;

    private CIdleState _stateIdle;
    private CConnectingState _stateCalling;
    private CAlertingState _stateAlerting;
    private CActiveState _stateActive;
    private CReleasedState _stateReleased;
    private CIncomingState _stateIncoming;
    private CHoldingState _stateHolding;

    private ECallType _callType = ECallType.EUndefined;
    private System.DateTime _timestamp;
    private System.TimeSpan _duration;


    #endregion Variables

    #region Properties

    private int _session = -1;
    public int Session
    {
      get { return _session; }
      set { _session = value; }
    }

    private CCallProxy _sigproxy;
    public CCallProxy SigProxy
    {
      get { return _sigproxy; } 
    }

    private string _callingNumber = "";
    public string CallingNo
    {
      get { return _callingNumber; }
      set { _callingNumber = value; }
    }

    private bool _incoming = false;
    public bool Incoming
    {
      get { return _incoming; }
      set { _incoming = value; }
    }
    public ECallType Type
    {
      get { return _callType; }
      set { _callType = value; }
    }
    public System.DateTime Time
    {
      get { return _timestamp; }
      set { _timestamp = value; }
    }

    public System.TimeSpan Duration
    {
      set { _duration = value; }
      get { return _duration; }
    }
    public System.TimeSpan RuntimeDuration
    {
      get { return System.DateTime.Now.Subtract(Time); }
    }
    
    private bool _isHeld = false;
    public bool IsHeld
    {
      get { return _isHeld; }
      set { _isHeld = value; }
    }

    private bool _is3Pty = false;
    public bool Is3Pty
    {
      get { return _is3Pty; }
      set { _is3Pty = value; }
    }
    #endregion

    #region Constructor

    public CStateMachine(CCallProxy proxy)
    {
      _sigproxy = proxy;

      _stateIdle = new CIdleState(this);
      _stateAlerting = new CAlertingState(this);
      _stateActive = new CActiveState(this);
      _stateCalling = new CConnectingState(this);
      _stateReleased = new CReleasedState(this);
      _stateIncoming = new CIncomingState(this);
      _stateHolding = new CHoldingState(this);
      _state = _stateIdle;

      Time = System.DateTime.Now;
      Duration = System.TimeSpan.Zero;
    }

    #endregion Constructor


    #region Methods

    public CAbstractState getState()
    {
      return _state;
    }

    public EStateId getStateId()
    {
      return _state.StateId;
    }

    public string getStateName()
    {
      return _state.Name;
    }

    public void changeState(CAbstractState state)
    {
      _state.onExit();
      _state = state;
      _state.onEntry();
    }


    public void changeState(EStateId stateId)
    {
      switch (stateId) 
      {
        case EStateId.IDLE:  changeState(_stateIdle); break;
        case EStateId.CONNECTING: changeState(_stateCalling); break;
        case EStateId.ALERTING: changeState(_stateAlerting); break;
        case EStateId.ACTIVE: changeState(_stateActive); break;
        case EStateId.RELEASED: changeState(_stateReleased); break;
        case EStateId.INCOMING: changeState(_stateIncoming); break;
        case EStateId.HOLDING: changeState(_stateHolding); break;
      }
      CCallManager.getInstance().updateGui();
    }

    public void destroy()
    {
      // update call log
      if (((Type != ECallType.EDialed) || (CallingNo.Length > 0)) && (Type != ECallType.EUndefined))
      {
        CCallLog.getInstance().addCall(Type, CallingNo, Time, Duration);
        CCallLog.getInstance().save();
      }

      CallingNo = "";
      Incoming = false;
      changeState(EStateId.IDLE);
      CCallManager.getInstance().destroySession(Session);
    }

    #endregion Methods
  }

} // namespace Sipek
