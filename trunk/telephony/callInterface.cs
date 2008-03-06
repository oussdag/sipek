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


using System;
using System.Collections.Generic;

namespace Common
{
  #region Enums

  public enum EUserStatus : int
  { 
  	AVAILABLE, 
    BUSY, 
    OTP, 
    IDLE, 
    AWAY, 
    BRB, 
    OFFLINE, 
    OPT_MAX
  }


  public enum ETones : int
  {
    EToneDial = 0,
    EToneCongestion,
    EToneRingback,
    EToneRing,
  }


  public enum EServiceCodes : int
  {
    SC_CD,
    SC_CFU,
    SC_CFNR,
    SC_DND,
    SC_3PTY
  }


  public enum ECallNotification : int
  { 
    CN_HOLDCONFIRM
  }
 
  #endregion

  public delegate void TimerExpiredCallback(object sender, EventArgs e);

  public abstract class ITimer
  {
    public abstract void Start();
    public abstract void Stop();

    public abstract int Interval { get; set;}

    public abstract TimerExpiredCallback Elapsed { set;}

  }

  /// <summary>
  /// AbstractFactory is an abstract interace providing interfaces for CallControl module
  /// It consists of two parts: factory methods and getter methods. First creating instances, 
  /// later returns instances. 
  /// </summary>
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


  public interface ICallProxyInterface
  {
    int makeCall(string dialedNo, int accountId);

    bool endCall(int sessionId);

    bool alerted(int sessionId);

    bool acceptCall(int sessionId);

    bool holdCall(int sessionId);

    bool retrieveCall(int sessionId);

    bool xferCall(int sessionId, string number);

    bool xferCallSession(int sessionId, int session);

    bool threePtyCall(int sessionId, int session);

    //bool serviceRequest(EServiceCodes code, int session);
    bool serviceRequest(int sessionId, int code, string dest);

    bool dialDtmf(int sessionId, string digits, int mode);
  }

  interface ITelephonyCallback
  {
    #region Methods

    void incomingCall(string callingNo, string display);

    void onAlerting();

    void onConnect();

    void onReleased();

    void onHoldConfirm();

    bool noReplyTimerExpired(int sessionId);

    bool releasedTimerExpired(int sessionId);

    #endregion Methods
  }


  public delegate void DCallStateChanged(int callId, int callState, string info);
  public delegate void DCallIncoming(int callId, string number, string info);
  public delegate void DAccountStateChanged(int accountId, int accState);
  public delegate void DMessageReceived(string from, string text);
  public delegate void DBuddyStatusChanged(int buddyId, int status, string text);
  public delegate void DDtmfDigitReceived(int callId, int digit);
  public delegate void DCallNotification(int callId, ECallNotification notFlag, string text);
  public delegate void DMessageWaitingNotification(int mwi, string text);


  public abstract class ICommonProxyInterface
  {
    #region Events
    /// <summary>
    /// Events exposed to user layers. A protected virtual method added because 
    /// derived classes cannot invoke events directly.
    /// </summary>
    /// 

    public event DCallStateChanged CallStateChanged;
    protected void BaseCallStateChanged(int callId, int callState, string info)
    {
      if (null != CallStateChanged) CallStateChanged(callId, callState, info);
    }

    public event DCallIncoming CallIncoming;
    protected void BaseIncomingCall(int callId, string number, string info)
    {
      if (null != CallIncoming) CallIncoming(callId, number, info);
    }

    public event DCallNotification CallNotification;
    protected void BaseCallNotification(int callId, ECallNotification notifFlag, string text)
    {
      if (null != CallNotification) CallNotification(callId, notifFlag, text);
    }

    public event DAccountStateChanged AccountStateChanged;
    protected void BaseAccountStateChanged(int accountId, int accState)    
    {
      if (null != AccountStateChanged) AccountStateChanged(accountId, accState);
    }

    public event DMessageReceived MessageReceived;
    protected void BaseMessageReceived(string from, string text)
    {
      if (null != MessageReceived) MessageReceived(from, text);
    }

    public event DBuddyStatusChanged BuddyStatusChanged;
    protected void BaseBuddyStatusChanged(int buddyId, int status, string text)
    {
      if (null != BuddyStatusChanged) BuddyStatusChanged(buddyId, status, text);
    }
    public event DDtmfDigitReceived DtmfDigitReceived;
    protected void BaseDtmfDigitReceived(int callId, int digit)
    {
      if (null != DtmfDigitReceived) DtmfDigitReceived(callId, digit);
    }
    public event DMessageWaitingNotification MessageWaitingIndication;
    protected void BaseMessageWaitingIndication(int mwi, string text)
    {
      if (null != MessageWaitingIndication) MessageWaitingIndication(mwi, text);
    }

    #endregion

    private bool _initialized = false;
    public bool IsInitialized
    {
      get { return _initialized; }
      protected set { _initialized = value; }
    }

    public abstract int initialize();
    public abstract int shutdown();

    public abstract int registerAccounts();
    public abstract int registerAccounts(bool renew);

    public abstract int addBuddy(string ident);

    public abstract int delBuddy(int buddyId);

    public abstract int sendMessage(string dest, string message);

    public abstract int setStatus(int accId, EUserStatus presence_state);

    public abstract void setCodecPrioroty(string item, int p);

    public abstract int getNoOfCodecs();

    public abstract string getCodec(int i);
  }

  public interface IMediaProxyInterface
  {
    //int initialize();
    //int shutdown();

    int playTone(ETones toneId);

    int stopTone();
  }



  ///////////////////////////////////////////////////////////////////////////////////////////////
  public enum ECallType : int
  {
    EDialed,
    EReceived,
    EMissed,
    EAll,
    EUndefined
  }

  public class CCallRecord
  {
    private ECallType _type;
    private string _name = "";
    private string _number = "";
    private DateTime _time;
    private TimeSpan _duration;
    private int _count;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    public string Number
    {
      get { return _number; }
      set { _number = value; }
    }
    public ECallType Type
    {
      get { return _type; }
      set { _type = value; }
    }
    public TimeSpan Duration
    {
      get { return _duration; }
      set { _duration = value; }
    }
    public DateTime Time
    {
      get { return _time; }
      set { _time = value; }
    }
    public int Count
    {
      get { return _count; }
      set { _count = value; }
    }
  }

  public interface ICallLogInterface
  {
    // CallControl interface
    void addCall(ECallType type, string number, string name, System.DateTime time, System.TimeSpan duration);

    // GUI interface
    void save();

    Stack<CCallRecord> getList();
    Stack<CCallRecord> getList(ECallType type);

    void deleteRecord(CCallRecord record);
  }

  public class CNullCallLog : ICallLogInterface
  {
    public void addCall(ECallType type, string number, string name, System.DateTime time, System.TimeSpan duration) { }

    public void save() { }
    public Stack<CCallRecord> getList() { return null;}
    public Stack<CCallRecord> getList(ECallType type) { return null;}
    public void deleteRecord(CCallRecord record) {}
  }

  // Accounts
  public abstract class IAccount
  {
    public abstract string AccountName { get; set;}
    public abstract string HostName { get; set;}
    public abstract string Id { get; set;}
    public abstract string UserName { get; set;}
    public abstract string Password { get; set;}
    public abstract string DisplayName { get; set;}
    public abstract string DomainName { get; set;}
    public abstract int Port { get; set;}
    public abstract int RegState { get; set;}

  }

  /// <summary>
  /// IConfiguratorInterface
  /// </summary>
  public abstract class IConfiguratorInterface
  {
    public abstract bool DNDFlag { get; set; }
    public abstract bool AAFlag { get; set; }
    public abstract bool CFUFlag { get; set; }
    public abstract string CFUNumber { get; set; }
    public abstract bool CFNRFlag { get; set; }
    public abstract string CFNRNumber { get; set; }
    public abstract bool CFBFlag { get; set; }
    public abstract string CFBNumber { get; set; }
    public abstract int SIPPort { get; set; }
    public abstract int DefaultAccountIndex { get; set; }
    public abstract int NumOfAccounts { get; set; }
    public abstract List<string> CodecList { get; set; }
    
    public IAccount getAccount() { return getAccount(DefaultAccountIndex); }
    public abstract IAccount getAccount(int index);

    #region Methods
    public abstract void Save();
    #endregion Methods
  }

  #region Null Implementations

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
    public class NullAccount : IAccount
    {
      public override string AccountName
      {
        get { return ""; }
        set { }
      }

      public override string HostName
      {
        get { return ""; }
        set { }
      }

      public override string Id
      {
        get { return ""; }
        set {}
      }

      public override string UserName
      {
        get { return ""; }
        set {}
      }

      public override string Password
      {
        get { return ""; }
        set {}
      }

      public override string DisplayName
      {
        get { return ""; }
        set {}
      }

      public override string DomainName
      {
        get { return""; }
        set {}
      }

      public override int Port
      {
        get { return 0; }
        set {}
      }

      public override int RegState
      {
        get { return 0; }
        set {}
      }
    }

    #region IConfiguratorInterface Members

    public override bool CFUFlag
    {
      get { return false; }
      set { }
    }

    public override string CFUNumber
    {
      get { return ""; }
      set { }
    }

    public override bool CFNRFlag
    {
      get { return false; }
      set { }
    }

    public override string CFNRNumber
    {
      get { return ""; }
      set { }
    }

    public override bool CFBFlag
    {
      get { return false; }
      set { }
    }

    public override string CFBNumber
    {
      get { return ""; }
      set { }
    }

    public override bool DNDFlag
    {
      get { return false; }
      set { }
    }

    public override bool AAFlag
    {
      get { return false; }
      set { }
    }
    public override int SIPPort
    {
      get { return 5060; }
      set { }
    }
    public override int DefaultAccountIndex
    {
      get { return 0; }
      set { }
    }

    public override int NumOfAccounts
    {
      get { return 1; }
      set { }
    }

    public override IAccount getAccount(int index)
    {
      return new NullAccount();
    }

    public override void Save()
    { }

    public override List<string> CodecList { get { return null; } set { } }

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

    public override int initialize()
    {
      return 1;
    }

    public override int shutdown()
    {
      return 1;
    }

    public override int registerAccounts()
    {
      return 1;
    }

    public override int registerAccounts(bool renew)
    {
      return 1;
    }

    public override int addBuddy(string ident)
    {
      return 1;
    }

    public override int delBuddy(int buddyId)
    {
      return 1;
    }

    public override int sendMessage(string dest, string message)
    {
      return 1;
    }

    public override int setStatus(int accId, EUserStatus presence_state)
    {
      return 1;
    }

    public override void setCodecPrioroty(string item, int p)
    {
    }
    public override int getNoOfCodecs() { return 0; }

    public override string getCodec(int i) { return ""; }

    #endregion
  }

  public class NullMediaProxy : IMediaProxyInterface
  {
    #region IMediaProxyInterface Members

    public int playTone(ETones toneId)
    {
      return 1;
    }

    public int stopTone()
    {
      return 1;
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
    ICallLogInterface _logger = new CNullCallLog();

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


} // namespace Common
