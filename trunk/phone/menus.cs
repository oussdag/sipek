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
using Sipek;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Telephony;

namespace Sipek
{
  public enum EPages : int
  {
    P_IDLE = 1,
    P_INIT,
    P_PHONEBOOK,
    P_PHONEBOOKEDIT,
    P_MENU,
    P_SIPSETTINGS,
    P_SIPPROXYSETTINGS,
    P_SIPPROXYSETTINGS_1st,
    P_SIPPROXYSETTINGS_2nd,
    P_SIPPROXYSETTINGS_3rd,
    P_RINGMODE,
    P_CALLLOG,
    P_ACCOUNTS,
    P_SIPPROXYSETTINGSMORE,
    P_SERVICES,
    P_REDIRECT,
    P_MESSAGEBOX,
    P_MESSAGERECEIVEDBOX,
  }

  public enum ERingModes : int
  {
    ESILENT,
    EMELODY,
    EBEEP
  }

  public enum EStatusFlag : int
  {
    ERegStatus = 0x1,
    EDirectCall = 0x2,
    ELocked = 0x4,
    EIncomingCallDisabled = 0x8,
    ESilent = 0x10,
    ECallMissed = 0x20,
    EAlarmMissed = 0x40,
    EAll = 0xF,
  }
  
  /// <summary>
  /// 
  /// </summary>
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

      CText txt = new CText(".....");
      txt.PosY = 5;
      txt.PosX = 10;
      CHorizontalScroller scrollingtext = new CHorizontalScroller(txt, 10, 200);
      // add(scrollingtext); // solve problem with remaining dots 

      this.Ok += new VoidDelegate(okhandler);
    }

    public override void  onEntry()
    {
 	    base.onEntry();

      // Create call menu pages...
      new CConnectingPage();
      new CRingingPage();
      new CReleasedPage();
      new CActivePage();
      new CDialPage();
      new CIncomingPage();
      new CPreDialPage();
      new CHoldingPage();
      new CXferDialingPage();
      new CXferListPage();
      new C3PtyListPage();
      new CDeflectPage();
      new CCallOptionsPage();

      // initialize telephony...
      CCallManager.getInstance().initialize();
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
  public class CIdlePage : CPage
  {
    CText _timedate;
    CLink _linkRinger;
    CLink _linkAccounts;
    CText _displayName;
    CStatusBar _statusBar;

    public CIdlePage()
      : base((int)EPages.P_IDLE)
    {
      _timedate = new CText("");
      _timedate.PosY = 0;
      _timedate.Caption = System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
      CTimeoutDecorator timeDecor = new CTimeoutDecorator(_timedate, 1000, true);
      timeDecor.OnTimeout += new VoidDelegate(timeDateHandler);
      this.add(timeDecor);

      CText title = new CText("SIPek", EAlignment.justify_center);
      title.PosY = 3;
      add(title);

      // status indicator
      _statusBar = new CStatusBar(EAlignment.justify_right);
      _statusBar.PosY = 0;
      add(_statusBar);

      _displayName = new CText("");
      _displayName.PosY = 4;
      _displayName.Align = EAlignment.justify_center;
      add(_displayName);

      CLink mlinkPhonebook = new CLink("Buddies", (int)EPages.P_PHONEBOOK);
      mlinkPhonebook.PosY = 9;
      mlinkPhonebook.LinkKey = mlinkPhonebook.PosY;
      this.add(mlinkPhonebook);

      _linkRinger = new CLink("Ring Mode", (int)EPages.P_RINGMODE);
      _linkRinger.Align = EAlignment.justify_right;
      _linkRinger.PosY = 8;
      _linkRinger.LinkKey = _linkRinger.PosY;
      this.add(_linkRinger);

      CLink mlinkCalls = new CLink("Calls", (int)EPages.P_CALLLOG);
      mlinkCalls.Align = EAlignment.justify_right;
      mlinkCalls.PosY = 10;
      mlinkCalls.LinkKey = mlinkCalls.PosY;
      this.add(mlinkCalls);

      _linkAccounts = new CLink("Accounts", (int)EPages.P_ACCOUNTS);
      _linkAccounts.PosY = 7;
      this.add(_linkAccounts);

      // Initialize handlers
      Digitkey += new BoolIntDelegate(digitkeyHandler);
      Charkey += new BoolIntDelegate(CIdlePage_Charkey);
      Offhook += new VoidDelegate(IdlePage_Offhook);
      Menu += new VoidDelegate(IdlePage_Menu);
      Ok += new VoidDelegate(IdlePage_Ok);
    }

    bool CIdlePage_Charkey(int keyId)
    {
      CPreDialPage page = (CPreDialPage)_controller.getPage((int)ECallPages.P_PREDIALING);
      page.setDigits(((char)keyId).ToString());
      _controller.showPage((int)ECallPages.P_PREDIALING);
      return true;
    }

    bool IdlePage_Ok()
    {
      _controller.showPage((int)EPages.P_CALLLOG);
      return true;
    }

    public override void onEntry()
    {
      if (!CCallManager.getInstance().isInitialized())
      {
        // initialize telephony...
        CCallManager.getInstance().initialize();
      }

      _displayName.Caption = CAccounts.getInstance().DefAccount.Id;

	    // check forwardings
      bool isForwardActive = CCallManager.getInstance().CFUFlag; //Properties.Settings.Default.cfgCFUFlag;
	    if (!isForwardActive)
	    {
        isForwardActive = CCallManager.getInstance().DNDFlag; //Properties.Settings.Default.cfgDNDFlag;
	    }
      //int isDirectCallActive = ;
      //int isAlarmActive = ;
      //int isKeyboardLocked = ;
      //int isCallMissed = CCallLog::Instance()->getCallsMissed();

      int status = 0;
      if (isForwardActive)
      {
        status = status + (int)EStatusFlag.EIncomingCallDisabled;
      }
/*      if (isKeyboardLocked == 1)
      {
	      status = status + EStatusFlag.ELocked;
      }
      if (isDirectCallActive == 1)
      {
	      status = status + EStatusFlag.EDirectCall;
      }
      if (isCallMissed == 1)
      {
	      status = status + EStatusFlag.ECallMissed;
      }
      if (isAlarmActive == 2)
      {
        status = status + EStatusFlag.EAlarmMissed;
      }
 */ 

      // get ringer mode
      switch (Properties.Settings.Default.cfgRingMode)
      {
        case (int)ERingModes.ESILENT:
          _linkRinger.Caption = "Silent";
          // assign to status
          status = status + (int)EStatusFlag.ESilent;
          break;
        case (int)ERingModes.EMELODY:
          _linkRinger.Caption = "Melody";
          break;
        case (int)ERingModes.EBEEP:
          _linkRinger.Caption = "Beep";
          break;
      }      
      
      // get registration status
      int regState = CAccounts.getInstance().DefAccount.RegState;
      switch (regState)
      {
        case 200:
          // assign to status
          status = status + (int)EStatusFlag.ERegStatus;
          break;
        default:
          break;
      }

      // update status bar!!!
      _statusBar.setStatus(status);

      // get default account
      _linkAccounts.Caption = CAccounts.getInstance().DefAccount.Name;

      base.onEntry();
    }

    bool IdlePage_Menu()
    {
      _controller.showPage((int)EPages.P_MENU);
      return true;
    }

    bool IdlePage_Offhook()
    {
      _controller.showPage((int)ECallPages.P_PREDIALING);
      return true;
    }

    private bool digitkeyHandler(int id)
    {
      CPreDialPage page = (CPreDialPage)_controller.getPage((int)ECallPages.P_PREDIALING);

      page.setDigits(id.ToString());
      _controller.showPage((int)ECallPages.P_PREDIALING);
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

  /// <summary>
  /// 
  /// </summary>
  public class CAccountsPage : CPage
  {
    CSelectList _list;
    CRadioButtonGroup _radio;

    public CAccountsPage()
      : base((int)EPages.P_ACCOUNTS, "Accounts")
    {
      _radio = new CRadioButtonGroup();

      _list = new CSelectList(_radio, 5);
      _list.PosY = 3;
      add(_list);

      CLink linkConfigure = new CLink("Edit");
      linkConfigure.Align = EAlignment.justify_right;
      linkConfigure.PosY = 10;
      linkConfigure.Softkey += new BoolIntDelegate(linkConfigure_Softkey);
      add(linkConfigure);

      CLink linkSave = new CLink("Save changes");
      linkSave.PosY = 9;
      linkSave.Softkey += new BoolIntDelegate(linkSave_Softkey);
      add(linkSave);

      this.Menu += new VoidDelegate(_radio_Menu);
      this.Ok += new VoidDelegate(CAccountsPage_Ok);
      this.Esc += new VoidDelegate(CAccountsPage_Esc);
    }

    bool CAccountsPage_Esc()
    {
      // discard changes
      CAccounts.getInstance().reload();
      _controller.previousPage();
      return true;
    }

    bool linkSave_Softkey(int keyId)
    {
      CAccounts.getInstance().save();
      return true;
    }

    bool CAccountsPage_Ok()
    {
      if (_list.Selected == null) return true;
      int aindex = int.Parse(_list.Selected.subItems[0]);
      CAccounts.getInstance().DefAccountIndex = aindex;
      CAccounts.getInstance().save();

      CCallManager.getInstance().initialize();
      return true;
    }

    bool linkConfigure_Softkey(int keyId)
    {
      return _radio_Menu();
    }

    bool _radio_Menu()
    {
      CSIPProxySettings page = (CSIPProxySettings)_controller.getPage((int)EPages.P_SIPPROXYSETTINGS);
      int aindex = int.Parse(_list.Selected.subItems[0]);
      page.setAccountIndex(aindex);
      _controller.showPage((int)EPages.P_SIPPROXYSETTINGS);
      return true;
    }

    public override void onEntry()
    {
      int cnt = 0;

      _radio.removeAll();

      for (int i=0; i<CAccounts.getInstance().getSize(); i++)
      {
        bool ischecked = (i == CAccounts.getInstance().DefAccountIndex) ? true : false;
        CCheckBox item = new CCheckBox(CAccounts.getInstance()[i].Name, -1, ischecked);
        if (item.Caption.Length == 0)
        {
          item.Caption = "--- empty ---";
        }
        else
        {
          if (CAccounts.getInstance()[i].RegState == 200)
            item.Caption += " (Reg)";
          else
            item.Caption += " (Not reg)";
        }
        item.subItems[0] = i.ToString();
        //item.Softkey += new BoolIntDelegate(item_Softkey);
        _radio.add(item);
         if (ischecked) cnt++;
      }
      
      mText = "Accounts (" + cnt + ")";

      base.onEntry();
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CPhonebookPage : CPage
  {
    private CEditField _criteria;
    private CSelectList _list;

    public CPhonebookPage() : base((int)EPages.P_PHONEBOOK,"Phonebook") 
    {
      _criteria = new CEditField(">", "", EEditMode.alphanum_low, true);
      _criteria.PosY = 1;
      _criteria.Digitkey += new BoolIntDelegate(_criteria_Digitkey);
      add(_criteria);

      _list = new CSelectList(7);
      _list.PosY = 3;
      add(_list);

      CLink addNewLink = new CLink("Add New", (int)EPages.P_PHONEBOOKEDIT);
      addNewLink.PosY = 9;
      //addNewLink.Align = EAlignment.justify_right;
      add(addNewLink);

      CLink modifyLink = new CLink("Modify");
      modifyLink.PosY = 10;
      modifyLink.Softkey += new BoolIntDelegate(modifyLink_Softkey);
      modifyLink.Align = EAlignment.justify_right;
      add(modifyLink);

      CLink messageLink = new CLink("Message");
      messageLink.Align = EAlignment.justify_right;
      messageLink.PosY = 8;
      messageLink.Softkey += new BoolIntDelegate(messageLink_Softkey);
      add(messageLink);

      Menu += new VoidDelegate(CPhonebookPage_Menu);
    }

    bool messageLink_Softkey(int keyId)
    {
      CLink item = (CLink)_list.Selected;
      if (item == null) return false;

      CMessageBoxPage page = (CMessageBoxPage)_controller.getPage((int)EPages.P_MESSAGEBOX);
      page.BuddyId = int.Parse(item.subItems[0]);
      _controller.showPage(page.Id);
      return true;
    }

    public override void onEntry()
    {
      _list.removeAll();

      Dictionary<int, CBuddyRecord> results = CBuddyList.getInstance().getList();

      foreach (KeyValuePair<int, CBuddyRecord> kvp in results)
      {
        CLink recordLink = new CLink(kvp.Value.FirstName + " " + kvp.Value.LastName);
        recordLink.subItems[0] = kvp.Value.Id.ToString();

        recordLink.Ok += new VoidDelegate(recordLink_Ok);
        recordLink.Softkey += new BoolIntDelegate(recordLink_Softkey);
        _list.add(recordLink);
      }

      base.onEntry();
    }

    bool recordLink_Softkey(int keyId)
    {
      return recordLink_Ok();
    }

    bool recordLink_Ok()
    {
      CLink item = (CLink)_list.Selected;
      if (item == null) return false;

      CCallManager.getInstance().createSession(item.subItems[2]);
      return true;
    }

    bool modifyLink_Softkey(int keyId)
    {
      CLink selitem = (CLink)_list.Selected;
      if (selitem == null) return true;      
      
      CPhonebookEditPage page = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);
      // get record
      string indstr = selitem.subItems[0];
      page.BuddyId = int.Parse(indstr);

      _controller.showPage(page.Id);
      
      return true;
    }

    bool _criteria_Digitkey(int keyId)
    {
      _controller.drawComponent(this);
      return true;
    }

    bool CPhonebookPage_Menu()
    {
      string indstr = _list.Selected.subItems[0];

      CPhonebookEditPage editPage = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);
      editPage.BuddyId = int.Parse(indstr);

      _controller.showPage(editPage.Id);
      return true;
    }

  }

  ///////////////////////////////////////////////////////////////////////////////
  /// <summary>
  /// 
  /// </summary>
  public class CPhonebookEditPage : CPage
  {
    private CEditField _fname;
    private CEditField _lname;
    private CEditField _number;
    private int _id = -1;

    public int BuddyId
    {
      get { return _id;  }
      set { _id = value; }
    }

    public CPhonebookEditPage()
      : base((int)EPages.P_PHONEBOOKEDIT, "Editing")
    {
      _fname = new CEditField("First Name>", "", EEditMode.alphanum_high, true);
      _fname.PosY = 3;
      _fname.LinkKey = _fname.PosY;
      add(_fname);

      _lname = new CEditField("Last Name>", "", EEditMode.alphanum_high, false);
      _lname.PosY = 5;
      _lname.LinkKey = _lname.PosY;
      add(_lname);

      _number = new CEditField("Number>", "", EEditMode.numeric);
      _number.PosY = 7;
      _number.LinkKey = _number.PosY;
      add(_number);

      CLink saveLink = new CLink("Save!");
      saveLink.PosY = 8;
      saveLink.LinkKey = saveLink.PosY;
      saveLink.Align = EAlignment.justify_right;
      saveLink.Softkey += new BoolIntDelegate(saveLink_Softkey);
      add(saveLink);

      CLink deleteLink = new CLink("Delete!");
      deleteLink.PosY = 10;
      deleteLink.Align = EAlignment.justify_right;
      deleteLink.LinkKey = deleteLink.PosY;
      deleteLink.Softkey += new BoolIntDelegate(deleteLink_Softkey);
      add(deleteLink);
    }

    public override void onEntry()
    {
      base.onEntry();

      CBuddyRecord record = CBuddyList.getInstance().getRecord(BuddyId);

      if (record == null) return;
 
      _fname.Caption = record.FirstName;
      _lname.Caption = record.LastName;
      _number.Caption = record.Number;
    }

    public override void onExit()
    {
      base.onExit();
      BuddyId = -1;
    }

    bool saveLink_Ok()
    {
      return saveLink_Softkey(0);
    }

    bool saveLink_Softkey(int keyId)
    {
      CBuddyRecord record;

      if (BuddyId >= 0)
      {
        record = CBuddyList.getInstance().getRecord(BuddyId);
        CBuddyList.getInstance().deleteRecord(BuddyId);
      }
      else 
      {
        record = new CBuddyRecord();
      }
      record.FirstName = _fname.Caption;
      record.LastName = _lname.Caption;
      record.Number = _number.Caption;
      
      CBuddyList.getInstance().addRecord(record);
      CBuddyList.getInstance().save();

      _controller.previousPage();
      return true;
    }

    bool deleteLink_Softkey(int keyId)
    {
      CBuddyList.getInstance().deleteRecord(BuddyId);
      CBuddyList.getInstance().save();

      _controller.previousPage();
      return true;
    }

  }


  //////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// 
  /// </summary>
  public class CCalllogPage : CPage
  {
    private CSelectList _list;

    public CCalllogPage()
      : base((int)EPages.P_CALLLOG, "Call Register")
    {
      _list = new CSelectList(8,2);
      _list.PosY = 1;
      add(_list);

      CLink clearLink = new CLink("Clear All");
      clearLink.PosY = 10;
      clearLink.LinkKey = clearLink.PosY;
      clearLink.Align = EAlignment.justify_right;
      clearLink.Softkey += new BoolIntDelegate(clearLink_Softkey);
      add(clearLink);

      Menu += new VoidDelegate(CCalllogPage_Menu);
    }

    public override void onEntry()
    {
      _list.removeAll();

      Stack<CCallRecord> results = CCallLog.getInstance().getList();

      foreach (CCallRecord item in results)
      {
        CDoubleLink recordLink = new CDoubleLink(item.Number + " " + item.Name , " " + item.Time.ToString());
        //recordLink.subItems[0] = item.LastName;
        //recordLink.subItems[1] = item.FirstName;
        recordLink.subItems[2] = item.Number;
        recordLink.Ok += new VoidDelegate(recordLink_Ok);
        recordLink.Softkey += new BoolIntDelegate(recordLink_Softkey);
        _list.add(recordLink);
      }
      if (results.Count > 3)
      {
        CDoubleLink separator1 = new CDoubleLink("------------------------", "------------------------", EAlignment.justify_center, EAlignment.justify_center);
        _list.add(separator1);
      }

      base.onEntry();
    }

    bool recordLink_Softkey(int keyId)
    {
      if (10 > keyId) return recordLink_Ok();
      return false;
    }

    bool recordLink_Ok()
    {
      CLink item = (CLink)_list.Selected;
      if (item == null) return false;

      CCallManager.getInstance().createSession(item.subItems[2]);
      return true;
    }

    bool clearLink_Softkey(int keyId)
    {
      if (10 == keyId) CCallLog.getInstance().clearAll();
      return true;
    }

    bool CCalllogPage_Menu()
    {
/*      CPhonebookEditPage editPage = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);
      editPage.FirstName = ((CLink)_list.Selected).Caption;
      editPage.LastName = ((CLink)_list.Selected).subItems[0];
      editPage.Number = ((CLink)_list.Selected).subItems[1];
      _controller.showPage(editPage.Id);
 */ 
      return true;
    }

  }

  ///
  //////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////

  public class CMenuPage : CPage 
  {
    public CMenuPage()     
      : base((int)EPages.P_MENU, "Settings")
    {
      CLink linkNetwork = new CLink("Network", 0);
      linkNetwork.PosY = 7;
      add(linkNetwork);

      CLink linkSound = new CLink("Sound", 0);
      linkSound.PosY = 8;
      linkSound.Align = EAlignment.justify_right;
      add(linkSound);

      CLink linkSIP = new CLink("SIP", (int)EPages.P_SIPSETTINGS);
      linkSIP.PosY = 9;
      add(linkSIP);

      CLink linkServices = new CLink("Services", (int)EPages.P_SERVICES); 
      linkServices.PosY = 10;
      linkServices.Align = EAlignment.justify_right;
      add(linkServices);
    }

  }

  public class CServicesPage : CPage
  {
    public CServicesPage()
      : base((int)EPages.P_SERVICES, "Services")
    {
      CLink linkRedirect = new CLink("Redirect", (int)EPages.P_REDIRECT);
      linkRedirect.PosY = 7;
      add(linkRedirect);

      CCheckBox chbDND = new CCheckBox("Do Not Disturb", 0);
      chbDND.PosY = 8;
      chbDND.Align = EAlignment.justify_right;
      chbDND.OnChecked += new VoidDelegate(chbDND_OnChecked);
      chbDND.OnUnchecked += new VoidDelegate(chbDND_OnUnchecked);
      add(chbDND);

      CCheckBox linkAA = new CCheckBox("Auto Answer", 0);
      linkAA.PosY = 9;
      add(linkAA);

    }

    bool chbDND_OnUnchecked()
    {
      //Properties.Settings.Default.cfgDNDFlag = false;
      //Properties.Settings.Default.Save();
      return true;
    }

    bool chbDND_OnChecked()
    {
      //Properties.Settings.Default.cfgDNDFlag = true;
      //Properties.Settings.Default.Save();
      return true;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CRedirectPage : CPage
  {
    CCheckBox _checkCFU;
    CEditField _editCFUNumber;
    CCheckBox _checkCFNR;
    CEditField _editCFNRNumber;
    CCheckBox _checkCFB;
    CEditField _editCFBNumber;

    public CRedirectPage()
      : base((int)EPages.P_REDIRECT, "Redirections")
    {
      _checkCFU = new CCheckBox("Unconditional");
      _checkCFU.PosY = 5;
      add(_checkCFU);

      _editCFUNumber = new CEditField("Number>", "", true);
      _editCFUNumber.PosY = 6;
      add(_editCFUNumber);

      _checkCFNR = new CCheckBox("On No Reply");
      _checkCFNR.PosY = 7;
      add(_checkCFNR);

      _editCFNRNumber = new CEditField("Number>", "");
      _editCFNRNumber.PosY = 8;
      add(_editCFNRNumber);


      _checkCFB = new CCheckBox("On Busy");
      _checkCFB.PosY = 9;
      add(_checkCFB);

      _editCFBNumber = new CEditField("Number>", "");
      _editCFBNumber.PosY = 10;
      add(_editCFBNumber);

      this.Ok += new VoidDelegate(CRedirectPage_Ok);
    }

    bool CRedirectPage_Ok()
    {
/*
      Properties.Settings.Default.cfgCFUFlag = _checkCFU.Checked;
      Properties.Settings.Default.cfgCFNRFlag = _checkCFNR.Checked;
      Properties.Settings.Default.cfgCFBFlag = _checkCFB.Checked;

      Properties.Settings.Default.cfgCFUNumber = _editCFUNumber.Caption;
      Properties.Settings.Default.cfgCFNRNumber = _editCFNRNumber.Caption;
      Properties.Settings.Default.cfgCFBNumber = _editCFBNumber.Caption;

      Properties.Settings.Default.Save();
*/
      _controller.previousPage();

      return true;
    }

    public override void onEntry()
    {
      _checkCFU.Checked = CCallManager.getInstance().CFUFlag; //Properties.Settings.Default.cfgCFUFlag;
      //_checkCFNR.Checked = CCallManager.getInstance()//Properties.Settings.Default.cfgCFNRFlag;
      //_checkCFB.Checked = Properties.Settings.Default.cfgCFBFlag;

      _editCFUNumber.Caption = CCallManager.getInstance().CFUNumber; //Properties.Settings.Default.cfgCFUNumber;
      //_editCFNRNumber.Caption = //Properties.Settings.Default.cfgCFNRNumber;
      //_editCFBNumber.Caption = //Properties.Settings.Default.cfgCFBNumber;
      
      base.onEntry();
    }


  }

  /// <summary>
  /// 
  /// </summary>
  public class CSIPSettings : CPage
  {
    CEditField _editport;
    CSelectList _list;
    CLink _accountsLink;

    public CSIPSettings() 
      : base((int)EPages.P_SIPSETTINGS, "SIP Settings") 
    {
      _list = new CSelectList(3);
      _list.PosY = 2;
      add(_list);
      
      _editport = new CEditField("Local Port>", "", EEditMode.numeric);
      _editport.PosY = 7;
      _editport.LinkKey = _editport.PosY;
      add(_editport);

      _accountsLink = new CLink("Accounts", (int)EPages.P_ACCOUNTS);
      _accountsLink.Align = EAlignment.justify_right;
      _accountsLink.PosY = 8;
      _accountsLink.LinkKey = _accountsLink.PosY;
      add(_accountsLink);

      // ok handler
      this.Ok += new VoidDelegate(CSIPSettings_Ok);
    }

    public override void onEntry()
    {
      //_accountsLink.Caption = CAccounts.getInstance().DefAccount.Name;

      _editport.Caption = Properties.Settings.Default.cfgSipPort.ToString();

      base.onEntry();
    }

    bool CSIPSettings_Ok()
    {
      Properties.Settings.Default.cfgSipPort = int.Parse(_editport.Caption);

      Properties.Settings.Default.Save();

      _controller.previousPage();
      return true;
    }


  }

  /// <summary>
  /// 
  /// </summary>
  public class CSIPProxySettings : CPage
  {
    int _accountIndex = -1;
    //CIpAddressEdit _editProxyAddress;
    CEditField _editProxyAddress;
    CEditField _editProxyPort;
    //CCheckBox _checkRegister;
    CEditField _editDisplayName;
    CEditField _editId;

    public CSIPProxySettings()
      : base((int)EPages.P_SIPPROXYSETTINGS,"Settings")
    {
      // create 1st page
      _editDisplayName = new CEditField("Name>", "",true);
      _editDisplayName.PosY = 3;
      _editDisplayName.LinkKey = _editDisplayName.PosY;
      this.add(_editDisplayName);

      _editId = new CEditField("Id>", "");
      _editId.PosY = 5;
      _editId.LinkKey = _editId.PosY;
      this.add(_editId);

      //_editProxyAddress = new CIpAddressEdit("Proxy>");
      _editProxyAddress = new CEditField("Proxy>","");
      _editProxyAddress.PosY = 7;
      _editProxyAddress.LinkKey = _editProxyAddress.PosY;
      this.add(_editProxyAddress);

      _editProxyPort = new CEditField("Port>", "", EEditMode.numeric);
      _editProxyPort.PosY = 9;
      _editProxyPort.LinkKey = _editProxyPort.PosY;
      this.add(_editProxyPort);

      CLink linkNext = new CLink("More...");
      linkNext.PosY = 10;
      linkNext.Align = EAlignment.justify_right;
      linkNext.Softkey += new BoolIntDelegate(linkNext_Softkey);
      add(linkNext);

      this.Ok += new VoidDelegate(CSIPProxySettings_Ok);
      this.Esc += new VoidDelegate(CSIPProxySettings_Esc);
    }

    bool CSIPProxySettings_Esc()
    {
      CAccounts.getInstance().reload();
      return false;
    }

    bool linkNext_Softkey(int keyId)
    {
      // store data
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Address = _editProxyAddress.Caption;
      account.Port = int.Parse(_editProxyPort.Caption);
      account.Name = _editDisplayName.Caption;
      account.Id = _editId.Caption;
      CAccounts.getInstance()[_accountIndex] = account;

      CSIPProxySettings2nd page = (CSIPProxySettings2nd)_controller.getPage((int)EPages.P_SIPPROXYSETTINGS_2nd);
      page.setAccountIndex(_accountIndex);
      _controller.showPage((int)EPages.P_SIPPROXYSETTINGS_2nd);
      return true;
    }

    public override void onEntry()
    {
      if (_accountIndex == -1) _accountIndex = CAccounts.getInstance().DefAccount.Index;

      _editId.Caption = CAccounts.getInstance()[_accountIndex].Id;
      _editDisplayName.Caption = CAccounts.getInstance()[_accountIndex].Name; // Properties.Settings.Default.cfgSipAccountDisplayName[0];
      _editProxyAddress.Caption = CAccounts.getInstance()[_accountIndex].Address;
      _editProxyPort.Caption = CAccounts.getInstance()[_accountIndex].Port.ToString();
      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Address = _editProxyAddress.Caption;
      account.Port = int.Parse(_editProxyPort.Caption);
      account.Name = _editDisplayName.Caption;
      account.Id = _editId.Caption;
      CAccounts.getInstance()[_accountIndex] = account;

      CAccounts.getInstance().save();

      _controller.previousPage();
      return true;
    }



    public void setAccountIndex(int index)
    {
      _accountIndex = index;
    }
  }
 

    /// <summary>
  /// 
  /// </summary>
  public class CSIPProxySettings2nd : CPage
  {
    // 2nd
    private CEditField _editUserName;
    private CEditField _editPassword;
    private int _accountIndex = -1;

    
    public CSIPProxySettings2nd()
      : base((int)EPages.P_SIPPROXYSETTINGS_2nd, "Settings")
    {
      this.forgetPage(true);
      ////////////
      // 2nd page
      _editUserName = new CEditField("Username>", "", true);
      _editUserName.PosY = 5;
      _editUserName.LinkKey = _editUserName.PosY;
      this.add(_editUserName);

      _editPassword = new CEditField("Password>", "");
      _editPassword.PosY = 7;
      _editPassword.LinkKey = _editPassword.PosY;
      this.add(_editPassword);

      CLink linkNext = new CLink("More...");
      linkNext.PosY = 10;
      linkNext.Align = EAlignment.justify_right;
      linkNext.Softkey += new BoolIntDelegate(linkNext_Softkey);
      add(linkNext);

      this.Ok += new VoidDelegate(CSIPProxySettings_Ok);
    }

    bool linkNext_Softkey(int keyId)
    {
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Username = _editUserName.Caption;
      account.Password = _editPassword.Caption;
      CAccounts.getInstance()[_accountIndex] = account;

      CSIPProxySettings3rd page = (CSIPProxySettings3rd)_controller.getPage((int)EPages.P_SIPPROXYSETTINGS_3rd);
      page.setAccountIndex(_accountIndex);
      _controller.showPage((int)EPages.P_SIPPROXYSETTINGS_3rd);
      return true;
    }

    public override void onEntry()
    {
      if (_accountIndex == -1) _accountIndex = CAccounts.getInstance().DefAccount.Index;

      _editUserName.Caption = CAccounts.getInstance()[_accountIndex].Username;
      _editPassword.Caption = CAccounts.getInstance()[_accountIndex].Password;

      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Username = _editUserName.Caption;
      account.Password = _editPassword.Caption;
      CAccounts.getInstance()[_accountIndex] = account;
      
      CAccounts.getInstance().save();
      
      _controller.previousPage();
      return true;
    }
    public void setAccountIndex(int index)
    {
      _accountIndex = index;
    }
  }

  public class CSIPProxySettings3rd : CPage
  {
    // 3rd
    private CEditField _editDomain;
    private CEditField _editperiod;
    private int _accountIndex = -1;

    public CSIPProxySettings3rd()
      : base((int)EPages.P_SIPPROXYSETTINGS_3rd, "Settings")
    {
      this.forgetPage(true);

      _editDomain = new CEditField("Domain>", "", true);
      _editDomain.PosY = 5;
      _editDomain.LinkKey = _editDomain.PosY;
      this.add(_editDomain);

      _editperiod = new CEditField("Reg. Period>", "", EEditMode.numeric);
      _editperiod.PosY = 7;
      _editperiod.LinkKey = _editperiod.PosY;
      this.add(_editperiod);
      
      CLink linkNext = new CLink("More...");
      linkNext.PosY = 10;
      linkNext.Align = EAlignment.justify_right;
      linkNext.Softkey += new BoolIntDelegate(linkNext_Softkey);
      add(linkNext);

      this.Ok += new VoidDelegate(CSIPProxySettings_Ok);
    }

    bool linkNext_Softkey(int keyId)
    {
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Period = int.Parse(_editperiod.Caption);
      account.Domain = _editDomain.Caption;
      CAccounts.getInstance()[_accountIndex] = account;

      CSIPProxySettings page = (CSIPProxySettings)_controller.getPage((int)EPages.P_SIPPROXYSETTINGS);
      page.setAccountIndex(_accountIndex);
      _controller.showPage((int)EPages.P_SIPPROXYSETTINGS);
      return true;
    }

    public override void onEntry()
    {
      if (_accountIndex == -1) _accountIndex = CAccounts.getInstance().DefAccount.Index;

      _editperiod.Caption = CAccounts.getInstance()[_accountIndex].Period.ToString();
      _editDomain.Caption = CAccounts.getInstance()[_accountIndex].Domain.ToString();

      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      CAccount account = CAccounts.getInstance()[_accountIndex];
      account.Period = int.Parse(_editperiod.Caption);
      account.Domain = _editDomain.Caption;
      CAccounts.getInstance()[_accountIndex] = account;

      CAccounts.getInstance().save();
      _controller.previousPage();
      return true;
    }
    public void setAccountIndex(int index)
    {
      _accountIndex = index;
    }
  }
  /////////////////////////////////////////////////////////////////////////////////////////////////////

  public class CRingModePage : CPage
  {
    CRadioButtonGroup _radio;
    CCheckBox _silentCb;
    CCheckBox _melodyCb;
    CCheckBox _beepCb;

    public CRingModePage() 
      : base((int)EPages.P_RINGMODE,"Ringer Mode")
    {
      _radio = new CRadioButtonGroup();

      _silentCb = new CCheckBox("Silent");
      _silentCb.PosY = 5;
      _silentCb.LinkKey = _silentCb.PosY;
      _silentCb.Softkey += new BoolIntDelegate(_silentCb_Softkey);
      _radio.add(_silentCb);

      _melodyCb = new CCheckBox("Melody");
      _melodyCb.PosY = 7;
      _melodyCb.LinkKey = _melodyCb.PosY;
      _melodyCb.Softkey += new BoolIntDelegate(_melodyCb_Softkey);
      _radio.add(_melodyCb);
      
      _beepCb = new CCheckBox("Beep");
      _beepCb.PosY = 9;
      _beepCb.LinkKey = _beepCb.PosY;
      _beepCb.Softkey += new BoolIntDelegate(_beepCb_Softkey);
      _radio.add(_beepCb);

      add(_radio);
    }

    bool _beepCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = (int)ERingModes.EBEEP;
      Properties.Settings.Default.Save();
      _controller.previousPage();
      return true;
    }

    bool _melodyCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = (int)ERingModes.EMELODY;
      Properties.Settings.Default.Save();
      _controller.previousPage();
      return true;
    }

    bool _silentCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = (int)ERingModes.ESILENT;
      Properties.Settings.Default.Save();
      _controller.previousPage();
      return true;
    }

    public override void onEntry()
    {
      base.onEntry();

      switch (Properties.Settings.Default.cfgRingMode)
      {
        case (int)ERingModes.ESILENT:
          _radio.Checked = _silentCb;
          break;
        case (int)ERingModes.EMELODY:
          _radio.Checked = _melodyCb;
          break;
        case (int)ERingModes.EBEEP:
          _radio.Checked = _beepCb;
          break;
      }

    }
  
  }

  /// <summary>
  /// Menu page for message text input.
  /// Buddy data should be set prior to page entry.
  /// </summary>
  public class CMessageBoxPage : CPage
  {
    private CText _titleText;
    private CLink _buddyName;
    private CTextBox _textBox;
    private CEditBox _editBox;
    private int _buddyId = -1;

    public int BuddyId
    {
      get { return _buddyId; }
      set { _buddyId = value; }
    }

    public CMessageBoxPage()
      : base((int)EPages.P_MESSAGEBOX, "Send Message To...")
    {
      _titleText = new CText("Buddy");
      _titleText.PosY = 1;
      add(_titleText);

      _textBox = new CTextBox("");
      _textBox.PosY = 2;
      add(_textBox);

      _editBox = new CEditBox(">", "");
      _editBox.PosY = 5;
      add(_editBox);

      CLink sendLink = new CLink("Send");
      sendLink.PosY = 10;
      sendLink.Align = EAlignment.justify_right;
      sendLink.Softkey += new BoolIntDelegate(sendLink_Softkey);
      this.add(sendLink);

      _buddyName = new CLink("");
      _buddyName.PosY = 3;
      add(_buddyName);

      this.Ok += new VoidDelegate(CMessageBoxPage_Ok);
    }

    public override void onEntry()
    {
      if (_buddyId != -1)
      {
        // get buddy data form _buddyId
        CBuddyRecord buddy = CBuddyList.getInstance()[_buddyId];
        if (buddy != null)
        {
          _titleText.Caption = buddy.FirstName + ", " + buddy.LastName;

          //_buddyName.Caption = buddy.Id;

          _textBox.Caption = buddy.LastMessage;

        }
      }

      base.onEntry();
    }

    bool sendLink_Softkey(int keyId)
    {
      if (_buddyId == -1) return true;
      // get buddy data form _buddyId
      CBuddyRecord buddy = CBuddyList.getInstance()[_buddyId];
      if (buddy != null)
      {
        // Invoke SIP stack wrapper function to send message
        CPjSipProxy.sendMessage(buddy.Number, _editBox.Caption);
      }
      _controller.previousPage();
      return true;
    }

    bool CMessageBoxPage_Ok()
    {
      return sendLink_Softkey(10);
    }
  }

    
  /// <summary>
  /// Menu page for message text input.
  /// Buddy data should be set prior to page entry.
  /// </summary>
  public class CMessageReceivedPage : CPage
  {
    private CText _titleText;
    private CLink _replyLink;
    private CTextBox _textBox;

    private string _textString;
    private string _fromString;
    private int _buddyId = -1;

    public string Message
    {
      set { _textString = value; }
    }
    public string From
    {
      set { _fromString = value; }
    }

    public CMessageReceivedPage()
      : base((int)EPages.P_MESSAGERECEIVEDBOX, "Message Received")
    {
      _titleText = new CText("");
      _titleText.PosY = 1;
      add(_titleText);

      _textBox = new CTextBox("");
      _textBox.PosY = 3;
      add(_textBox);

      _replyLink = new CLink("Reply");
      _replyLink.PosY = 9;
      _replyLink.Softkey += new BoolIntDelegate(_replyLink_Softkey);
      add(_replyLink);
    }

    public override void onEntry()
    {
      _buddyId = -1;

      // parse from string
      string buddykey = parseFrom(_fromString);
      int id = CBuddyList.getInstance().getBuddy(buddykey);
      if (id >= 0)
      {
        _buddyId = id;
        
        CBuddyRecord buddy = CBuddyList.getInstance()[_buddyId];
        _titleText.Caption = buddy.FirstName + ", " + buddy.LastName;
      }

      _textBox.Caption = _textString;

      base.onEntry();
    }

    bool _replyLink_Softkey(int keyId)
    {
      CMessageBoxPage page = (CMessageBoxPage)_controller.getPage((int)EPages.P_MESSAGEBOX);

      if (_buddyId >= 0)
      {
        page.BuddyId = _buddyId;
        _controller.showPage((int)EPages.P_MESSAGEBOX);
      }
      return true;
    }

    private string parseFrom(string from)
    {
      string number = from.Replace("<sip:", "");

      int atPos = number.IndexOf('@');
      if (atPos >= 0)
      {
        number = number.Remove(atPos);
      }
      else
      {
        int semiPos = number.IndexOf(';');
        if (semiPos >= 0)
        {
          number = number.Remove(semiPos);
        }
        else
        {
          int colPos = number.IndexOf(':');
          if (colPos >= 0)
          {
            number = number.Remove(colPos);
          }
        }
      }
      return number;
    }

  }

} // namespace Sipek
