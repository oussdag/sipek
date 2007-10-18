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
  /// 
  /// </summary>
  public enum EStateId  : int 
  {
    IDLE = 1, // associated with home page = P_IDLE
    CONNECTING = 1974,
    ALERTING, 
    ACTIVE,
    RELEASED,
    INCOMING,
    HOLDING
  }
  
  public enum EServiceCodes : int
  {
    SC_CD,
    SC_CFU,
    SC_CFNR,
    SC_DND,
    SC_3PTY
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
  public abstract class CAbstractState : CTelephonyInterface, CTelephonyCallback
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

    #endregion

    #region Variables

    protected CStateMachine _smref;
    protected CCommonProxyInterface _commonProxy;
    protected CMediaProxyInterface _mediaProxy;

    #endregion Variables

    #region Constructor

    public CAbstractState(CStateMachine sm)
    {
      _smref = sm;
      _commonProxy = sm.CommonProxy;
      _mediaProxy = sm.MediaProxy;
    }

    #endregion Constructor

    #region Methods

    public abstract void onEntry();
    
    public abstract void onExit();


    public virtual int makeCall(string dialedNo, int accountId)
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

    public virtual bool holdCall()
    {
      return true;
    }
    
    public virtual bool retrieveCall()
    {
      return true;
    }
    public virtual bool xferCall(string number)
    {
      return true;
    }
    public virtual bool xferCallSession(int session)
    {
      return true;
    }
    public bool threePtyCall(int session)
    {
      return true;
    }
/*    public bool serviceRequest(EServiceCodes code, int session)
    {
      _smref.SigProxy.serviceRequest(code, session);
      return true;
    }
 */ 
    public bool serviceRequest(int code, string dest)
    {
      _smref.SigProxy.serviceRequest(code, dest);
      return true;
    }

    public bool dialDtmf(int mode, string digits)
    {
      _smref.SigProxy.dialDtmf(mode, digits);
      return true;
    }

    #endregion Methods

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
      return _smref.SigProxy.makeCall(dialedNo, accountId);
    }

    public override void incomingCall(string callingNo)
    {
      _smref.CallingNo = callingNo;
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

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return base.endCall();
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
      _mediaProxy.playTone(ETones.EToneRingback);
    }

    public override void onExit()
    {
      _mediaProxy.stopTone();
    }

    public override void onConnect()
    {
      _smref.Time = System.DateTime.Now;
      _smref.changeState(EStateId.ACTIVE);
    }

    public override void onReleased()
    {
      //_smref.destroy();
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      //_smref.destroy();
      return base.endCall();
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
    
    public override bool endCall()
    {
      _smref.Duration = System.DateTime.Now.Subtract(_smref.Time);

      _smref.SigProxy.endCall();
      _smref.destroy();
      return base.endCall();
    }

    public override bool holdCall()
    {
      return _smref.SigProxy.holdCall();
    }

    public override bool xferCall(string number)
    {
      return _smref.SigProxy.xferCall(number);
    }
    public override bool xferCallSession(int session)
    {
      return _smref.SigProxy.xferCallSession(session);
    }

    public override void onHoldConfirm()
    {
      _smref.changeState(EStateId.HOLDING);
    }

    public override void onReleased()
    {
      //_smref.destroy();
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
      _mediaProxy.playTone(ETones.EToneCongestion);
    }

    public override void onExit()
    {
      _mediaProxy.stopTone();
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }
    
    public override bool endCall()
    {
      _smref.destroy();
      return base.endCall();
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

      if ((_smref.CFUFlag == true) && (_smref.CFUNumber.Length > 0))
      {
        _smref.SigProxy.serviceRequest((int)EServiceCodes.SC_CFU, _smref.CFUNumber);
      }
      else if (_smref.DNDFlag == true)
      {
        _smref.SigProxy.serviceRequest((int)EServiceCodes.SC_DND, "");
      }
      else if (CSettings.AA == true)
      {
        this.acceptCall();
      }
      else

      {
        _smref.SigProxy.alerted();
        _smref.Type = ECallType.EMissed;
        _mediaProxy.playTone(ETones.EToneRing);
      }
    }

    public override void onExit()
    {
      _mediaProxy.stopTone();
    }    

    public override bool acceptCall()
    {
      _smref.Type = ECallType.EReceived;
      _smref.Time = System.DateTime.Now;

      _smref.SigProxy.acceptCall();
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
      _smref.destroy();
    }

    public override bool xferCall(string number)
    {
      // In fact this is not Tranfser. It's Deflect => redirect...
      return _smref.SigProxy.serviceRequest((int)EServiceCodes.SC_CD, number);
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return base.endCall();
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

    public override bool retrieveCall()
    {
      _smref.SigProxy.retrieveCall();
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    public override void onReleased()
    {
      //_smref.destroy();
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall()
    {
      _smref.SigProxy.endCall();
      _smref.destroy();
      return base.endCall();
    }
  }


} // namespace Sipek
