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
    P_ENTRY = 0,
    P_INIT,
    P_IDLE,
    P_PREDIALING,
    P_LIST,
    P_DETAILS,
    P_ADDITEM,
    P_MENU,
    P_LANGUAGES
  }



  public class EntryPage : CPage
  {
    public EntryPage()
      : base((int)EPages.P_ENTRY)
    {
      this.forgetPage(true);

      CText text = new CText("Sipek Phone", EAlignment.justify_left);
      this.add(text);

      CLink link = new CLink("Press any key", (int)EPages.P_INIT);
      link.PosY = 3;
      link.LinkKey = link.PosY;
      this.add(link);

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
    public IdlePage()
      : base((int)EPages.P_IDLE)
    {
      CLink mlinkPhonebook = new CLink("Phonebook", 0);
      mlinkPhonebook.PosY = 5;
      this.add(mlinkPhonebook);

      CLink mlinkRinger = new CLink("Ringer", 0);
      mlinkRinger.Align = EAlignment.justify_right;
      mlinkRinger.PosY = 4;
      this.add(mlinkRinger);

      CLink mlinkCalls = new CLink("Calls", 0);
      mlinkCalls.Align = EAlignment.justify_right;
      mlinkCalls.PosY = 6;
      this.add(mlinkCalls);

      CLink mlinkLines = new CLink("Lines", 0);
      mlinkLines.PosY = 3;
      this.add(mlinkLines);

      Digitkey += new UintDelegate(digitkeyHandler);
    }

    private bool digitkeyHandler(int id)
    {
      CPreDialPage page = (CPreDialPage)_controller.getPage((int)EPages.P_PREDIALING);

      page.setDigits(id.ToString());
      _controller.showPage((int)EPages.P_PREDIALING);
      return true;
    }

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
      linkHide.PosY = 3;
      this.add(linkHide);

      CLink dialing_phbook = new CLink("Phonebook"/*, P_PBOOK*/);
      dialing_phbook.PosY = 5;
      this.add(dialing_phbook);

      CLink linkCall = new CLink("Calls");
      linkCall.Align = EAlignment.justify_right;
      linkCall.Softkey += new UintDelegate(callHandler);
      linkCall.PosY = 4;
      this.add(linkCall);

      CLink linkSave = new CLink("Save");
      linkSave.Align = EAlignment.justify_right;
      linkSave.PosY = 6;
      this.add(linkSave);

      _editField = new CEditField(">", "", EEditMode.numeric, true);
      _editField.PosY = 1;
      this.add(_editField);

      // page handlers
      //this->OnOkKeyFPtr = &this->okHandlerFctr;
      //this->OnOffhookKeyFPtr = &this->okHandlerFctr;
      //this->OnSpeakerKeyFPtr = &this->okHandlerFctr;
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

} // namespace Gui
