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
using Telephony;

namespace Gui
{
  public enum EPages : int
  {
    P_INIT = 1,
    P_IDLE,
    P_PREDIALING,
    P_PHONEBOOK
  }



  public class CInitPage : CPage
  {
    public CInitPage()
      : base((int)EPages.P_INIT)
    {
      this.forgetPage(true);

      CLink link = new CLink("Initializing", (int)EPages.P_INIT);
      link.Align = EAlignment.justify_center;
      link.PosY = 2;
      link.LinkKey = link.PosY;
      this.add(link);

      CText text = new CText("Sipek Phone", EAlignment.justify_center);
      text.PosY = 3;
      this.add(text);

      this.Ok += new NoParamDelegate(okhandler);
    }

    private bool okhandler()
    {
      _controller.showPage((int)EPages.P_IDLE);
      return true; 
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class IdlePage : CPage
  {
    CText _timedate;

    public IdlePage()
      : base((int)EPages.P_IDLE)
    {
      _timedate = new CText("");
      _timedate.PosY = 0;
      _timedate.Caption = System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
      CTimeoutDecorator timeDecor = new CTimeoutDecorator(_timedate, 1000, true);
      timeDecor.OnTimeout += new NoParamDelegate(timeDateHandler);
      this.add(timeDecor);

      CText title = new CText("SIPek", EAlignment.justify_center);
      title.PosY = 3;
      add(title);

      CLink mlinkPhonebook = new CLink("Phonebook", (int)EPages.P_PHONEBOOK);
      mlinkPhonebook.PosY = 8;
      mlinkPhonebook.LinkKey = mlinkPhonebook.PosY;
      this.add(mlinkPhonebook);

      CLink mlinkRinger = new CLink("Ringer", 0);
      mlinkRinger.Align = EAlignment.justify_right;
      mlinkRinger.PosY = 7;
      mlinkRinger.LinkKey = mlinkRinger.PosY; 
      this.add(mlinkRinger);

      CLink mlinkCalls = new CLink("Calls", 0);
      mlinkCalls.Align = EAlignment.justify_right;
      mlinkCalls.PosY = 9;
      mlinkCalls.LinkKey = mlinkCalls.PosY;
      this.add(mlinkCalls);

      CLink mlinkLines = new CLink("Accounts", 0);
      mlinkLines.PosY = 6;
      this.add(mlinkLines);

      Digitkey += new UintDelegate(digitkeyHandler);
      Offhook += new NoParamDelegate(IdlePage_Offhook);
    }

    bool IdlePage_Offhook()
    {
      _controller.showPage((int)EPages.P_PREDIALING);
      return true;
    }

    private bool digitkeyHandler(int id)
    {
      CPreDialPage page = (CPreDialPage)_controller.getPage((int)EPages.P_PREDIALING);

      page.setDigits(id.ToString());
      _controller.showPage((int)EPages.P_PREDIALING);
      return true;
    }

    private bool timeDateHandler()
    {
      string seperator;

      if (_flip)
        seperator = ":";
      else
        seperator = " ";

      _flip = !_flip;

      _timedate.Caption = System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
      _timedate.Caption = _timedate.Caption.Remove(_timedate.Caption.Length - 3, 1);
      _timedate.Caption = _timedate.Caption.Insert(_timedate.Caption.Length - 2, seperator);
      return true;
    }

    private bool _flip = false;

  }

  //////////////////////////////////////////////////////////////////////////
  // Pre Dialing page
  //////////////////////////////////////////////////////////////////////////
  public class CPreDialPage : CPage
  {
    public CPreDialPage()
      : base((int)EPages.P_PREDIALING, "Dialing...")
    {
      CLink linkHide = new CLink("Hide Number", 0);
      linkHide.PosY = 6;
      this.add(linkHide);

      CLink dialing_phbook = new CLink("Phonebook"/*, P_PBOOK*/);
      dialing_phbook.PosY = 8;
      this.add(dialing_phbook);

      CLink linkCall = new CLink("Calls");
      linkCall.Align = EAlignment.justify_right;
      linkCall.Softkey += new UintDelegate(callHandler);
      linkCall.PosY = 7;
      this.add(linkCall);

      CLink linkSave = new CLink("Save");
      linkSave.Align = EAlignment.justify_right;
      linkSave.PosY = 9;
      this.add(linkSave);

      _editField = new CEditField(">", "", EEditMode.numeric, true);
      _editField.PosY = 2;
      this.add(_editField);

      // page handlers
      //this->OnOkKeyFPtr = &this->okHandlerFctr;
      //this->OnOffhookKeyFPtr = &this->okHandlerFctr;
      //this->OnSpeakerKeyFPtr = &this->okHandlerFctr;

      this.Ok += new NoParamDelegate(okHandler);
      Offhook += new NoParamDelegate(CPreDialPage_Offhook);
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

  /// <summary>
  /// 
  /// </summary>
  public class CPhonebookPage : CPage
  { 
    public CPhonebookPage() : base((int)EPages.P_PHONEBOOK,"Phonebook") 
    {
    }


  }

} // namespace Gui
