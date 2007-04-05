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

namespace Sipek
{
  public enum ECallPages : int
  {
    P_DIALING = 100,
    P_PREDIALING,
    P_CALLING,
    P_CONNECTING,
    P_RINGING,
    P_ACTIVE,
    P_RELEASED,
    P_INCOMING
  }

  public abstract class CTelephonyPage : CPage
  {

    protected CText _info;
    protected CText _digits;
    protected CText _name;
    protected CLink _endCall;
    protected CLink _sessions;
  	

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
	    _digits.PosY = 5;
	    this.add(_digits);

      _name = new CText("");
      _name.Align = EAlignment.justify_center;
      _name.PosY = 6;
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
      CStateMachine currentCall = CCallManager.getInstance().getCurrentCall();

      if (currentCall == null) return;

	    // info...
	    string sinfo = ""/*currentCall->GetCallInfo()*/;
	    _info.Caption = sinfo;

	    // digits...
	    _digits.Caption = currentCall.CallingNo;

      // get name from phonebook...
      //_id.Caption = currentCall.CallingNo;

	    // call sessions...
      _sessions.Caption = (CCallManager.getInstance().getCurrentCallIndex()).ToString() 
          + "/" + CCallManager.getInstance().Count.ToString();
    }


    bool endCallHandler(int id) 
    {
      return onhookHandler(); 
    }

    bool menuHandler() 
    { 
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
      linkHide.PosY = 5;
      this.add(linkHide);

      CLink dialing_phbook = new CLink("Phonebook"/*, P_PBOOK*/);
      dialing_phbook.PosY = 7;
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
    }

    bool _hold_Softkey(int keyId)
    {
      return true;
    }

    bool decor_OnTimeout()
    {
      int seconds = CCallManager.getInstance().getCurrentCall().RuntimeDuration.Seconds;
      int minutes = CCallManager.getInstance().getCurrentCall().RuntimeDuration.Minutes;
      _duration.Caption = String.Format("{0:00}:{1:00}", minutes, seconds);
      return true;
    }

    public override void  onEntry()
    {
 	    base.onEntry();
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
    }

    bool accept_call_Softkey(int keyId)
    {
      CCallManager.getInstance().onUserAnswer();
      return true;
    }
  }

} // namespace Sipek
