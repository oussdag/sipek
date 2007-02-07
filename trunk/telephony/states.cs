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
  /// CAbstractState implements two interfaces CTelephonyInterface and CTelephonyCallback. 
  /// The first interface is used for sending requests to call server, where the second is used to 
  /// signal event from call server. 
  /// It's a base for all call states used by CStateMachine. 
  /// </summary>
  public abstract class CAbstractState : CTelephonyInterface, CTelephonyCallback
  {
    public enum EStateId
    {
      IDLE,
      CONNECTING,
      ALERTING, 
      ACTIVE,
      RELEASED,
      INCOMING
    }

    #region Properties
    private EStateId _stateId = EStateId.IDLE;

    public EStateId StateId 
    {
      get { return _stateId;  }
      set { _stateId = value; }
    }

    #endregion

    #region Variables

    protected CStateMachine _smref;


    #endregion Variables


    #region Constructor

    public CAbstractState(CStateMachine sm)
    {
      _smref = sm;
    }

    #endregion Constructor


    #region Methods

    public abstract void onEntry();
    
    public abstract void onExit();
    

    public virtual int makeCall(string dialedNo)
    {
      return -1;
    }

    public virtual bool endCall()
    {
      return true;
    }

    public virtual bool acceptCall()
    {
      return true;
    }


    public virtual bool alerted()
    {
      return true;
    }


    #region Callbacks

    public virtual void incomingCall(string callingNo)
    { 
    }

    public virtual void onAlerting()
    {
    }

    public virtual void onConnect()
    {
    }
    
    public virtual void onReleased()
    { 
    }


    #endregion Callbacks

    #endregion Methods
  }





  /// <summary>
  /// CIdleState
  /// </summary>
  public class CIdleState : CAbstractState
  {
    public CIdleState(CStateMachine sm) 
      : base(sm)
    {
      StateId = EStateId.IDLE;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override int makeCall(string dialedNo)
    {
      _smref.DialedNo = dialedNo;
      _smref.changeState(EStateId.CONNECTING);
      return _smref.SigProxy.makeCall(dialedNo);
    }

    public override bool endCall()
    {
      _smref.destroy();

      return _smref.SigProxy.endCall();
    }


    public override void incomingCall(string callingNo)
    {
      _smref.SigProxy.alerted();
      _smref.CallingNo = callingNo;
      _smref.Incoming = true;
      _smref.changeState(EStateId.INCOMING);
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CConnectingState : CAbstractState
  {
    public CConnectingState(CStateMachine sm) 
      : base(sm)
    {
      StateId = EStateId.CONNECTING;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return true;
    }

    public override void onReleased()
    {
      _smref.destroy();
    }

    public override void onAlerting()
    {
      _smref.changeState(EStateId.ALERTING);
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CAlertingState : CAbstractState
  {
    public CAlertingState(CStateMachine sm)
      : base(sm)
    {
      StateId = EStateId.ALERTING;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return true;
    }

    public override void onConnect()
    {
      _smref.changeState(EStateId.ACTIVE);
    }

    public override void onReleased()
    {
      _smref.destroy();
    }

  }


  /// <summary>
  /// CActiveState
  /// </summary>
  public class CActiveState : CAbstractState
  {
    public CActiveState(CStateMachine sm) 
      : base(sm)
    {
      StateId = EStateId.ACTIVE;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }
    
    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return true;
    }

    public override void onReleased()
    {
      _smref.destroy();
    }

  }


  /// <summary>
  /// CReleasedState
  /// </summary>
  public class CReleasedState : CAbstractState
  {
    public CReleasedState(CStateMachine sm)
      : base(sm)
    {
      StateId = EStateId.RELEASED;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

  }


  /// <summary>
  /// CIncomingState
  /// </summary>
  public class CIncomingState : CAbstractState
  {
    public CIncomingState(CStateMachine sm)
      : base(sm)
    {
      StateId = EStateId.INCOMING;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return true;
    }

    public override bool acceptCall()
    {
      _smref.SigProxy.acceptCall();
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }



  }


} // namespace Telephony
