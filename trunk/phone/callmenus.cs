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

using MenuDesigner;
using System;
using System.Collections.Generic;
using Telephony;

namespace Sipek
{

  /// <summary>
  /// Associate call menu id's with call states.
  /// </summary>
  public enum ECallPages : int
  {
    P_DIALING = 100,
    P_PREDIALING,
    P_CONNECTING = EStateId.CONNECTING,
    P_RINGING = EStateId.ALERTING,
    P_ACTIVE = EStateId.ACTIVE,
    P_RELEASED = EStateId.RELEASED,
    P_INCOMING = EStateId.INCOMING,
    P_HOLDING = EStateId.HOLDING,
    P_XFERLIST,
    P_XFERDIAL,
    P_3PTYLIST,
    P_DEFLECT,
    P_CALLOPTIONS,
    P_SETVOLUME,
    P_DTMFOPTIONS,
  }

  public abstract class CTelephonyPage : CPage
  {

    protected CText _info;
    protected CText _digits;
    protected CText _name;
    protected CLink _endCall;
    protected CLink _sessions;

    protected CStateMachine _currentCall = null;
    protected CCallManager _callManager = CCallManager.getInstance();

    static int _currentCallIdx = -1;
    public static int CurrentCallIdx
    {
      get { return _currentCallIdx; }
      set { _currentCallIdx = value; }
    }

    public CTelephonyPage(ECallPages pageId, string pageName) 
      : base((int)pageId, false,true)
	  {
      setText(pageName);

	    CText title = new CText(pageName);
	    title.PosY = 0;
	    add(title);
	    // sessions link
      _sessions = new CLink("0/0");
      _sessions.Align = EAlignment.justify_right;
      _sessions.Softkey += new BoolIntDelegate(sessionHandler);
      _sessions.PosY = 0;
      this.add(_sessions);

	    _digits = new CText("");
      _digits.Align = EAlignment.justify_center;
	    _digits.PosY = 1;
	    this.add(_digits);

      _name = new CText("");
      _name.Align = EAlignment.justify_center;
      _name.PosY = 2;
      this.add(_name);

	    _info = new CText("");
      _info.Align = EAlignment.justify_center;
	    _info.PosY = 7;
	    this.add(_info);

	    _endCall = new CLink("End Call");
      _endCall.Align = EAlignment.justify_right;
      _endCall.Softkey += new BoolIntDelegate(endCallHandler);
      _endCall.PosY = 10;
      _endCall.LinkKey = _endCall.PosY;
      this.add(_endCall);
      
      // assign page handlers....
      this.Onhook += new VoidDelegate(onhookHandler);
      this.Speaker += new VoidDelegate(onspeakerHandler);
      this.Menu += new VoidDelegate(menuHandler);

	  }

    public override void onEntry() 
    {
      base.onEntry();

      // get current call instance
      //_currentCall = CCallManager.getInstance().getCurrentCall();
      _currentCall = CCallManager.getInstance()[CurrentCallIdx];

      if (_currentCall == null) return;

	    // info...
	    string sinfo = ""/*currentCall->GetCallInfo()*/;
	    _info.Caption = sinfo;

	    // digits...
	    _digits.Caption = _currentCall.CallingNo;

      // get name from phonebook...
      //_id.Caption = currentCall.CallingNo;

	    // call sessions...
      _sessions.Caption = CurrentCallIdx.ToString()
        + "/" + _callManager.Count.ToString();
    }


    bool endCallHandler(int id) 
    {
      return onhookHandler(); 
    }

    bool menuHandler() 
    {
      _controller.showPage((int)ECallPages.P_CALLOPTIONS);
      return true; 
    }

    bool onhookHandler() 
    {
      CCallManager.getInstance().onUserRelease();
      return true; 
    }

    public virtual bool onspeakerHandler() 
    { 
      return true; 
    }

    bool sessionHandler(int id) 
    {
      CCallManager.getInstance().nextSession();
      return true; 
    }

	  // services
    bool holdHandler(int id) 
    { 
      return true; 
    }

    bool transferHandler(int id) 
    { 
      return true; 
    }

    bool threeptyHandler(int id) 
    { 
      return true; 
    }

    bool completionHandler(int id) 
    { 
      return true; 
    }

    bool newcallHandler(int id) 
    { 
      return true; 
    }

    bool muteHandler(int id) 
    { 
      return true; 
    }

    bool threeptysplitHandler(int id) 
    { 
      return true; 
    }

    bool swapHandler(uint id) 
    { 
      return true; 
    }

  }

  //////////////////////////////////////////////////////////////////////////
  // Pre Dialing page
  //////////////////////////////////////////////////////////////////////////
  public class CPreDialPage : CTelephonyPage
  {
    public CPreDialPage()
      : base(ECallPages.P_PREDIALING, "Dialing...")
    {
      clearHistory(false);

      CLink linkHide = new CLink("Hide Number", 0);
      linkHide.PosY = 7;
      this.add(linkHide);

      CLink dialing_phbook = new CLink("Phonebook"/*, P_PBOOK*/);
      dialing_phbook.PosY = 9;
      this.add(dialing_phbook);

      CLink linkCall = new CLink("Calls");
      linkCall.Align = EAlignment.justify_right;
      linkCall.Softkey += new BoolIntDelegate(callHandler);
      linkCall.PosY = 8;
      this.add(linkCall);

      CLink linkSave = new CLink("Save");
      linkSave.Align = EAlignment.justify_right;
      linkSave.PosY = 10;
      this.add(linkSave);

      _editField = new CEditField(">", "", EEditMode.numeric, true);
      _editField.PosY = 2;
      this.add(_editField);

      // page handlers
      //this->OnOkKeyFPtr = &this->okHandlerFctr;
      //this->OnOffhookKeyFPtr = &this->okHandlerFctr;
      //this->OnSpeakerKeyFPtr = &this->okHandlerFctr;

      this.Ok += new VoidDelegate(okHandler);
      Offhook += new VoidDelegate(CPreDialPage_Offhook);
    }

    bool CPreDialPage_Offhook()
    {
      CCallManager.getInstance().createSession(_editField.Caption);
      return true;
    }

    void setPhonebookHandler()
    {
    }

    // Overridden methods
    public override void onEntry()
    {
      base.onEntry();
    }

    public override void onExit()
    {
      base.onExit();
    }

    public void setDigits(string digit)
    {
      _editField.Caption = digit;
    }

    protected CEditField _editField;

    //////////////////////////////////////////////////
    // handlers

    private bool menuHandler()
    {
      return true;
    }

    private bool okHandler()
    {
      //CDialPage* page = (CDialPage*)_controller->getPage(P_DIALING);
      //page->makeCall(mEditField->getCaption());
      CCallManager.getInstance().createSession(_editField.Caption);
      return true;
    }

    private bool onhookHandler()
    {
      return true;
    }

    private bool callHandler(int id)
    {
      return true;
    }

    private bool phbHandler()
    {
      return true;
    }

    //CPhonebookListPage* _phbPartnerPage;

  }

  /////////////////////////////////////////////////////////////////////////////////////////


  public class CDialPage : CTelephonyPage
  {
    protected CEditField mEditField;
    //private CPhonebookPage _phbpage;

    public CDialPage() 
      : base(ECallPages.P_DIALING, "Dialing...")
	  {
		  
	  }

	  public void setDigits(string digits) 
    {
		  mEditField.Caption = digits;
	  }

	  public int makeCall(string number)
    {
      return -1;
    }

    ////////////////////////////////////////////////////////////////////

	  private bool okHandler()
    {
      return true;
    }

	  private bool phbHandler(int id)
    {
      return true;
    }

    private bool digitHandler(int id)
    {
      return true;
    }

  }

  /////////////////////////////////////////////////////////////////////////////////////////
  /// <summary>
  /// 
  /// </summary>
  public class CConnectingPage : CTelephonyPage
  {
    public CConnectingPage()
      : base(ECallPages.P_CONNECTING, "Connecting...")
    {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CRingingPage : CTelephonyPage
  {
    public CRingingPage() 
      : base(ECallPages.P_RINGING, "Ringing...")
	  {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CReleasedPage : CTelephonyPage
  {
    public CReleasedPage()
      : base(ECallPages.P_RELEASED, "Released...")
    {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CActivePage : CTelephonyPage
  {
    CText _duration;
    CLink _hold;
    CLink _xfer;
    CLink _3pty;

    public CActivePage()
      : base(ECallPages.P_ACTIVE, "Connected...")
    {
      _duration = new CText("00:00");
      _duration.PosY = 2;
      _duration.Align = EAlignment.justify_right;

      CTimeoutDecorator decor = new CTimeoutDecorator(_duration, 1000, true);
      decor.OnTimeout += new VoidDelegate(decor_OnTimeout);

      add(decor);

      _hold = new CLink("Hold");
      _hold.Softkey += new BoolIntDelegate(_hold_Softkey);
      _hold.PosY = 7;
      add(_hold);

      _3pty = new CLink("3Pty");
      _3pty.Align = EAlignment.justify_right;
      _3pty.PosY = 8;
      //add(_3pty);

      _xfer = new CLink("Transfer");
      //_xfer.Softkey += new BoolIntDelegate(_xfer_Softkey);
      _xfer.PosY = 9;
      add(_xfer);

      this.Digitkey += new BoolIntDelegate(CActivePage_Digitkey);
    }

    bool CActivePage_Digitkey(int keyId)
    {
      _callManager.getCurrentCall().getState().dialDtmf((int)EDtmfMode.DM_Inband, keyId.ToString());
      return true;
    }

    bool _hold_Softkey(int keyId)
    {
      _currentCall.getState().holdCall();
      return true;
    }

    bool decor_OnTimeout()
    {
      int seconds = _currentCall.RuntimeDuration.Seconds;
      int minutes = _currentCall.RuntimeDuration.Minutes;
      _duration.Caption = String.Format("{0:00}:{1:00}", minutes, seconds);
      return true;
    }

    public override void  onEntry()
    {
      remove(_3pty);
      _currentCall = _callManager.getCurrentCall();

      // check if held
      if (_currentCall.IsHeld)
      {
        this.mText = "Held...";
      }
      else
      {
        mText = "Connected...";
      }

      // multicall options
      if (_callManager.Count > 1)
      {
        // check if 3pty => 2 connected calls
        if (_currentCall.Is3Pty)
        {
          _hold.Caption = "3Pty Split";
          this.mText = "3-Party";
        }
        else 
        {
          // show 3pty link...
          add(_3pty);
        }

        if (_callManager.Count == 2)
        {
          // Hold/swap
          _hold.Caption = "Hold";
        }

        // not blind transfer
        _xfer.Link = (int)ECallPages.P_XFERLIST;
      }
      else
      {
        // Hold/swap
        _hold.Caption = "Hold";

        // blind transfer
        _xfer.Link = (int)ECallPages.P_XFERDIAL;
      }

      base.onEntry();
    }

  }



  /// <summary>
  /// 
  /// </summary>
  public class CHoldingPage : CTelephonyPage
  {
    CLink _hold;
    CLink _xfer;

    public CHoldingPage()
      : base(ECallPages.P_HOLDING, "Holding...")
    {
      _hold = new CLink("Retrieve");
      _hold.Softkey += new BoolIntDelegate(_hold_Softkey);
      _hold.PosY = 7;
      add(_hold);

      _xfer = new CLink("Transfer");
      _xfer.Softkey += new BoolIntDelegate(_xfer_Softkey);
      _xfer.PosY = 9;
      add(_xfer);

      Digitkey += new BoolIntDelegate(CHoldingPage_Digitkey);
    }

    bool _xfer_Softkey(int keyId)
    {

      return true;
    }

    public override void onEntry()
    {
      base.onEntry();

      if (_callManager.Count > 1)
      {
        // trigger transfer handling...
        _xfer.Link = (int)ECallPages.P_XFERLIST;
        // check if 1 call in active => show swap!
        if (_callManager.getNoCallsInState(EStateId.ACTIVE) == 1)
        {
          _hold.Caption = "Swap";
          //_hold.SoftKeyFPtr = &swapFctr;
        }

      }
      else if (1 == _callManager.Count)
      {
        _hold.Caption = "Retrieve";
        // open dial number edit!!!
        _xfer.Link = (int)ECallPages.P_XFERDIAL;
      }
    }

    bool CHoldingPage_Digitkey(int keyId)
    {
      CPreDialPage page = (CPreDialPage)_controller.getPage((int)ECallPages.P_PREDIALING);
      page.setDigits(keyId.ToString());
      _controller.showPage(page.Id);
      return true;
    }

    bool _hold_Softkey(int keyId)
    {
      _currentCall.getState().retrieveCall();
      return true;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CIncomingPage : CTelephonyPage
  {
    public CIncomingPage()
      : base(ECallPages.P_INCOMING, "Incoming...")
    {
      CLink accept_call = new CLink("Accept");
      accept_call.Softkey += new BoolIntDelegate(accept_call_Softkey);
      accept_call.PosY = 7;
      accept_call.LinkKey = accept_call.PosY;
      this.add(accept_call);

      CLink deflectLink = new CLink("Deflect", (int)ECallPages.P_DEFLECT);
      deflectLink.PosY = 8;
      deflectLink.Align = EAlignment.justify_right;
      this.add(deflectLink);
    }

    bool accept_call_Softkey(int keyId)
    {
      CCallManager.getInstance().onUserAnswer();
      return true;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CXferDialingPage : CTelephonyPage
  {
    private CEditField _editField;

    public CXferDialingPage()
      : base(ECallPages.P_XFERDIAL, "Transfer...")
    {
      this.clearHistory(false);

      CLink phbookLink = new CLink("Phonebook", (int)EPages.P_PHONEBOOK);
      phbookLink.PosY = 9;
      this.add(phbookLink);

      CLink callRegLink = new CLink("Calls", (int)EPages.P_CALLLOG);
      callRegLink.Align = EAlignment.justify_right;
      callRegLink.PosY = 8;
      this.add(callRegLink);

      _editField = new CEditField(">", "", EEditMode.numeric, true);
      _editField.PosY = 2;
      _editField.Ok += new VoidDelegate(_editField_Ok);
      this.add(_editField);

      // remove unneeded controls
      remove(this._name);
      remove(this._sessions);
    }

    bool _editField_Ok()
    {
      // blind transfer
      if (_editField.Caption.Length > 0)
      {
        _currentCall.getState().xferCall(_editField.Caption);
      }
      _controller.previousPage();
      return true;
    }
  }


  public class CXferListPage : CTelephonyPage
  {

    private CSelectList _xfer2List;

    public CXferListPage()
      : base(ECallPages.P_XFERLIST, "Transfer to...")
    {
      this.clearHistory(false);

      // remove end call link from CTelephony
      this.remove(_endCall);
      this.remove(_sessions);
      this.remove(_name);
      this.remove(_digits);

      _xfer2List = new CSelectList(5);
      _xfer2List.PosY = 1;
      this.add(_xfer2List);

      CLink blind = new CLink("Enter number", (int)ECallPages.P_XFERDIAL);
      blind.Align = EAlignment.justify_right;
      blind.PosY = 10;
      this.add(blind);

      // handlers
      Ok += new VoidDelegate(CXferListPage_Ok);
    }

    bool CXferListPage_Ok()
    {
      CLink selection = (CLink)_xfer2List.Selected;
      if (selection == null) return false;

      string sesIndStr = selection.subItems[0];

      _currentCall.getState().xferCallSession(int.Parse(sesIndStr));
      return true;
    }

    public override void onEntry()
    {
      int callnums = _callManager.getNoCallsInState(EStateId.HOLDING | EStateId.ACTIVE);

      _xfer2List.removeAll();
      
      Dictionary<int, CStateMachine> list = _callManager.CallList;
      foreach (KeyValuePair<int, CStateMachine> kvp in list)
      {
        CStateMachine call = kvp.Value;
        if ((call.Session != _callManager.getCurrentCall().Session) && 
              ((call.getStateId() == EStateId.HOLDING)||(call.getStateId() == EStateId.ACTIVE)))
        {
          string dn = call.CallingNo;
          CLink link = new CLink(dn);
          link.Align = EAlignment.justify_left;
          link.subItems[0] = call.Session.ToString();
          _xfer2List.add(link);
        }
      }
     
      base.onEntry();
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class C3PtyListPage : CTelephonyPage
  {

    private CSelectList _3pty2List;

    public C3PtyListPage()
      : base(ECallPages.P_3PTYLIST, "Conference with...")
    {
      this.clearHistory(false);

      // remove end call link from CTelephony
      this.remove(_endCall);
      this.remove(_sessions);
      this.remove(_name);
      this.remove(_digits);

      _3pty2List = new CSelectList(5);
      _3pty2List.PosY = 1;
      this.add(_3pty2List);

      // handlers
      Ok += new VoidDelegate(C3PtyListPage_Ok);
    }

    bool C3PtyListPage_Ok()
    {
      CLink selection = (CLink)_3pty2List.Selected;
      if (selection == null) return false;

      string sesIndStr = selection.subItems[0];

      _currentCall.getState().threePtyCall(int.Parse(sesIndStr));
      return true;
    }

    public override void onEntry()
    {
      int callnums = _callManager.getNoCallsInState(EStateId.HOLDING);

      _3pty2List.removeAll();

      Dictionary<int, CStateMachine> list = _callManager.CallList;
      foreach (KeyValuePair<int, CStateMachine> kvp in list)
      {
        CStateMachine call = kvp.Value;
        if ((call.Session != _callManager.getCurrentCall().Session) && (call.getStateId() == EStateId.HOLDING))
        {
          string dn = call.CallingNo;
          CLink link = new CLink(dn);
          link.Align = EAlignment.justify_left;
          link.subItems[0] = call.Session.ToString();
          _3pty2List.add(link); 
        }
      }

      base.onEntry();
    }
  }

  public class CDeflectPage : CTelephonyPage
  {
    CEditField _editField;

    public CDeflectPage()
      : base(ECallPages.P_DEFLECT, "Deflect to...")
    {
      clearHistory(false);

      this.remove(_name);
      this.remove(_digits);

      _editField = new CEditField(">", "", EEditMode.numeric, true);
      _editField.PosY = 1;
      _editField.Ok += new VoidDelegate(_editField_Ok);
      this.add(_editField);
    }

    bool _editField_Ok()
    {
      _currentCall.getState().serviceRequest((int)EServiceCodes.SC_CD, _editField.Caption);
      return true;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CCallOptionsPage : CTelephonyPage
  {
    private CLink _linkDtmf;
	  private CCheckBox _mute;

    public	CCallOptionsPage() 
      : base(ECallPages.P_CALLOPTIONS, "Options...")
    {
      clearHistory(false);

      forgetPage(false);

      // remove all CTelephony controls.
      removeAll();

      _mute = new CCheckBox("Mute");
      _mute.PosY = 5;
      _mute.Softkey += new BoolIntDelegate(_mute_Softkey);
      this.add(_mute);

      CLink volume = new CLink("Volume", (int)ECallPages.P_SETVOLUME);
      volume.PosY = 6;
      volume.Align = EAlignment.justify_right;
      this.add(volume);

      _linkDtmf = new CLink("DTMF mode", (int)ECallPages.P_DTMFOPTIONS);
      _linkDtmf.PosY = 7;
      this.add(_linkDtmf);

      CLink calls = new CLink("Calls", (int)EPages.P_CALLLOG);
      calls.PosY = 8;
      calls.Align = EAlignment.justify_right;
      this.add(calls);

      CLink phonebook = new CLink("Phonebook", (int)EPages.P_PHONEBOOK);
      phonebook.PosY = 9;
      this.add(phonebook);

      CLink menu = new CLink("Menu", (int)EPages.P_MENU);
      menu.PosY = 10;
      menu.Align = EAlignment.justify_right;
      this.add(menu);
    }

    bool _mute_Softkey(int keyId)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override void onEntry()
    {
      base.onEntry();

    }

  }

} // namespace Sipek
