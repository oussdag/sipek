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
using Common;


namespace CallControl
{
  /// <summary>
  /// 
  /// </summary>
  public enum EStateId  : int 
  {
    IDLE = 0x1,
    CONNECTING = 0x2,
    ALERTING = 0x4, 
    ACTIVE = 0x8,
    RELEASED = 0x10,
    INCOMING = 0x20,
    HOLDING = 0x40
  }

  public enum EDtmfMode : int
  {
    DM_Outband,
    DM_Inband,
    DM_Transparent
  }  

  /// <summary>
  /// CAbstractState implements two interfaces CTelephonyInterface and CTelephonyCallback. 
  /// The first interface is used for sending requests to call server, where the second is used to 
  /// signal event from call server. 
  /// It's a base for all call states used by CStateMachine. 
  /// </summary>
  public abstract class CAbstractState : ICallProxyInterface, ITelephonyCallback
  {

    #region Properties
    private EStateId _stateId = EStateId.IDLE;

    public EStateId StateId 
    {
      get { return _stateId;  }
      set { _stateId = value; }
    }

    public string Name
    {
      get {
        return StateId.ToString(); 
      }
    }

    public ICallProxyInterface CallProxy
    {
      get { return _smref.SigProxy; }
    }
    public IMediaProxyInterface MediaProxy
    {
      get { return _smref.MediaProxy; }
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


    public virtual int makeCall(string dialedNo, int accountId)
    {
      return -1;
    }

    public virtual bool endCall(int sesionnId)
    {
      return true;
    }

    public virtual bool acceptCall(int sesionnId)
    {
      return true;
    }


    public virtual bool alerted(int sesionnId)
    {
      return true;
    }

    public virtual bool holdCall(int sesionnId)
    {
      return true;
    }

    public virtual bool retrieveCall(int sesionnId)
    {
      return true;
    }
    public virtual bool xferCall(int sesionnId, string number)
    {
      return true;
    }
    public virtual bool xferCallSession(int sesionnId, int session)
    {
      return true;
    }
    public bool threePtyCall(int sesionnId, int session)
    {
      return true;
    }
/*    public bool serviceRequest(EServiceCodes code, int session)
    {
      CallProxy.serviceRequest(code, session);
      return true;
    }
 */ 
    public bool serviceRequest(int sesionnId, int code, string dest)
    {
      CallProxy.serviceRequest(sesionnId, code, dest);
      return true;
    }

    public bool dialDtmf(int sessionId, string digits, int mode)
    {
      CallProxy.dialDtmf(sessionId, digits, mode);
      return true;
    }

    public virtual bool noReplyTimerExpired(int sessionId)
    {
      return true;
    }

    public virtual bool releasedTimerExpired(int sessionId)
    {
      return true;
    }

    #endregion Methods

    #region Callbacks

    public virtual void incomingCall(string callingNo, string display)
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
    
    public virtual void onHoldConfirm()
    {
    }
    #endregion Callbacks
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

    public override int makeCall(string dialedNo, int accountId)
    {
      _smref.CallingNo = dialedNo;
      _smref.changeState(EStateId.CONNECTING);
      return CallProxy.makeCall(dialedNo, accountId);
    }

    public override void incomingCall(string callingNo, string display)
    {
      _smref.CallingNo = callingNo;
      _smref.CallingName = display;
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
      _smref.Type = ECallType.EDialed;
    }

    public override void onExit()
    {
    }

    public override void onReleased()
    {
      //_smref.destroy();
      _smref.changeState(EStateId.RELEASED);
    }

    public override void onAlerting()
    {
      _smref.changeState(EStateId.ALERTING);
    }


    public override void onConnect()
    {
      _smref.changeState(EStateId.ACTIVE);
    }

    public override bool endCall(int sessionId)
    {
      CallProxy.endCall(sessionId);
      _smref.destroy();
      return base.endCall(sessionId);
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
      MediaProxy.playTone(ETones.EToneRingback);
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
    }

    public override void onConnect()
    {
      _smref.Time = System.DateTime.Now;
      _smref.changeState(EStateId.ACTIVE);
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall(int sesionnId)
    {
      CallProxy.endCall(sesionnId);
      _smref.destroy();
      return base.endCall(sesionnId);
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
      _smref.Counting = true;
    }

    public override void onExit()
    {
    }

    public override bool endCall(int sesionnId)
    {
      _smref.Duration = System.DateTime.Now.Subtract(_smref.Time);

      CallProxy.endCall(sesionnId);
      _smref.destroy();
      return base.endCall(sesionnId);
    }

    public override bool holdCall(int sesionnId)
    {
      _smref.HoldRequested = true;
      return CallProxy.holdCall(sesionnId);
    }

    public override bool xferCall(int sesionnId, string number)
    {
      return CallProxy.xferCall(sesionnId, number);
    }
    public override bool xferCallSession(int sesionnId, int session)
    {
      return CallProxy.xferCallSession(sesionnId, session);
    }

    public override void onHoldConfirm()
    {
      // check if Hold requested
      if (_smref.HoldRequested)
      {
        _smref.changeState(EStateId.HOLDING);
        // activate pending action if any
        _smref.Manager.activatePendingAction();
      }
      _smref.HoldRequested = false;
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
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
      MediaProxy.playTone(ETones.EToneCongestion);
      _smref.startTimer(ETimerType.ERELEASED);
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
      _smref.stopAllTimers();
    }

    public override bool endCall(int sesionnId)
    {
      _smref.destroy();
      return true;
    }

    public override bool releasedTimerExpired(int sessionId)
    {
      _smref.destroy();
      return true;
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
      _smref.Incoming = true;

      int sessionId = _smref.Session;

      if ((_smref.Config.CFUFlag == true) && (_smref.Config.CFUNumber.Length > 0))
      {
        CallProxy.serviceRequest(sessionId, (int)EServiceCodes.SC_CFU, _smref.Config.CFUNumber);
      }
      else if (_smref.Config.DNDFlag == true)
      {
        CallProxy.serviceRequest(sessionId, (int)EServiceCodes.SC_DND, "");
      }
      else if (_smref.Config.AAFlag == true)
      {
        this.acceptCall(sessionId);
      }
      else
      {
        CallProxy.alerted(sessionId);
        _smref.Type = ECallType.EMissed;
        MediaProxy.playTone(ETones.EToneRing);
      }

      // if CFNR active start timer
      if (_smref.Config.CFNRFlag)
      {
        _smref.startTimer(ETimerType.ENOREPLY);
      }
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
    }

    public override bool acceptCall(int sesionnId)
    {
      _smref.Type = ECallType.EReceived;
      _smref.Time = System.DateTime.Now;

      CallProxy.acceptCall(sesionnId);
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
      _smref.destroy();
    }

    public override bool xferCall(int sessionId, string number)
    {
      // In fact this is not Tranfser. It's Deflect => redirect...
      return CallProxy.serviceRequest(sessionId, (int)EServiceCodes.SC_CD, number);
    }

    public override bool endCall(int sessionId)
    {
      CallProxy.endCall(sessionId);
      _smref.destroy();
      return base.endCall(sessionId);
    }

    public override bool noReplyTimerExpired(int sessionId)
    {
      CallProxy.serviceRequest(sessionId, (int)EServiceCodes.SC_CFNR, _smref.Config.CFUNumber);
      return true;
    }

  }

    /// <summary>
  /// CIdleState
  /// </summary>
  public class CHoldingState : CAbstractState
  {
    public CHoldingState(CStateMachine sm)
      : base(sm)
    {
      StateId = EStateId.HOLDING;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool retrieveCall(int sessionId)
    {
      _smref.RetrieveRequested = true;
      CallProxy.retrieveCall(sessionId);
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    // TODO implement in stack interface
    //public override onRetrieveConfirm()
    //{
    //  if (_smref.RetrieveRequested)
    //  {
    //    _smref.changeState(EStateId.ACTIVE);
    //  }
    //  _smref.RetrieveRequested = false;
    //}

    public override void onReleased()
    {
      //_smref.destroy();
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall(int sessionId)
    {
      CallProxy.endCall(sessionId);
      _smref.destroy();
      return base.endCall(sessionId);
    }
  }


} // namespace Sipek
