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
namespace Telephony
{
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

  public interface ICommonProxyInterface
  {
    int initialize(); 
    int shutdown();

    int registerAccounts();
    int registerAccounts(bool renew);

    int addBuddy(string ident);

    int delBuddy(int buddyId);

    int sendMessage(string dest, string message);

    int setStatus(int accId, EUserStatus presence_state);
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
    // Telephony interface
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

} // namespace Sipek
