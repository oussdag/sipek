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

namespace Sipek
{
  public enum EPages : int
  {
    P_INIT = 1,
    P_IDLE,
    P_PHONEBOOK,
    P_PHONEBOOKEDIT,
    P_MENU,
    P_SIPSETTINGS,
    P_SIPPROXYSETTINGS,
    P_RINGMODE,
    P_CALLLOG,
    P_ACCOUNTS,
    P_SIPPROXYSETTINGSMORE,
  }
  public enum ERingModes : int
  {
    ESILENT,
    EMELODY,
    EBEEP
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
      add(scrollingtext);


      this.Ok += new VoidDelegate(okhandler);
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
    CLink _linkRinger;
    CText _statusSymbol;
    CLink _linkAccounts;

    public IdlePage()
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

      CLink mlinkPhonebook = new CLink("Phonebook", (int)EPages.P_PHONEBOOK);
      mlinkPhonebook.PosY = 8;
      mlinkPhonebook.LinkKey = mlinkPhonebook.PosY;
      this.add(mlinkPhonebook);

      _linkRinger = new CLink("Ring Mode", (int)EPages.P_RINGMODE);
      _linkRinger.Align = EAlignment.justify_right;
      _linkRinger.PosY = 7;
      _linkRinger.LinkKey = _linkRinger.PosY;
      this.add(_linkRinger);

      CLink mlinkCalls = new CLink("Calls", (int)EPages.P_CALLLOG);
      mlinkCalls.Align = EAlignment.justify_right;
      mlinkCalls.PosY = 9;
      mlinkCalls.LinkKey = mlinkCalls.PosY;
      this.add(mlinkCalls);

      _linkAccounts = new CLink("Accounts", (int)EPages.P_ACCOUNTS);
      _linkAccounts.PosY = 6;
      this.add(_linkAccounts);

      _statusSymbol = new CText("", EAlignment.justify_right);
      _statusSymbol.PosY = 0;
      this.add(_statusSymbol);

      // Initialize handlers
      Digitkey += new BoolIntDelegate(digitkeyHandler);
      Offhook += new VoidDelegate(IdlePage_Offhook);
      Menu += new VoidDelegate(IdlePage_Menu);
    }

    public override void onEntry()
    {
      // get ringer mode
      switch (Properties.Settings.Default.cfgRingMode)
      {
        case (int)ERingModes.ESILENT:
          _linkRinger.Caption = "Silent";
          break;
        case (int)ERingModes.EMELODY:
          _linkRinger.Caption = "Melody";
          break;
        case (int)ERingModes.EBEEP:
          _linkRinger.Caption = "Beep";
          break;
      }      
      
      // get registration status
      ERegistrationState regState = CAccounts.getInstance().DefAccount.RegState;
      switch (regState)
      {
        case ERegistrationState.ERegistered:
          _statusSymbol.Caption = "OK";
          break;
        case ERegistrationState.ENotRegistered:
          _statusSymbol.Caption = "FAIL";
          break;
      }
      
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
    CRadioButtonGroup _radio;

    public CAccountsPage()
      : base((int)EPages.P_ACCOUNTS, "Accounts")
    {
      _radio = new CRadioButtonGroup();
      _radio.PosY = 1;
      _radio.Menu += new VoidDelegate(_radio_Menu);
      add(_radio);
    }

    bool _radio_Menu()
    {
      _controller.showPage((int)EPages.P_SIPPROXYSETTINGS);
      return false;
    }

    public override void onEntry()
    {
      _radio.removeAll();

      for (int i=0; i<CAccounts.getInstance().getSize(); i++)
      {
        bool ischecked = (i == CAccounts.getInstance().DefAccountIndex) ? true : false;
        CCheckBox item = new CCheckBox(CAccounts.getInstance()[i].Name, -1, ischecked);
        item.PosY = _radio.PosY + i;
        _radio.add(item);
      }

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
      _list.PosY = 4;
      add(_list);

      CLink addNewLink = new CLink("Add New", (int)EPages.P_PHONEBOOKEDIT);
      addNewLink.PosY = 2;
      addNewLink.LinkKey = addNewLink.PosY;
      //addNewLink.Align = EAlignment.justify_right;
      add(addNewLink);

      CLink modifyLink = new CLink("Modify");
      modifyLink.PosY = 3;
      modifyLink.LinkKey = modifyLink.PosY;
      modifyLink.Softkey += new BoolIntDelegate(modifyLink_Softkey);
      modifyLink.Align = EAlignment.justify_right;
      add(modifyLink);

      Menu += new VoidDelegate(CPhonebookPage_Menu);
    }

    public override void onEntry()
    {
      _list.removeAll();

      Collection<CPhonebookRecord> results = CPhonebook.getInstance().getList();

      foreach (CPhonebookRecord item in results)
      {
        CLink recordLink = new CLink(item.FirstName + " " + item.LastName);
        recordLink.subItems[0] = item.LastName;
        recordLink.subItems[1] = item.FirstName;
        recordLink.subItems[2] = item.Number;
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
      CPhonebookEditPage page = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);

      CLink selitem = (CLink)_list.Selected;
      page.LastName = selitem.subItems[0];
      page.FirstName = selitem.subItems[1];
      page.Number = selitem.subItems[2];

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
      CPhonebookEditPage editPage = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);
      editPage.FirstName = ((CLink)_list.Selected).Caption;
      editPage.LastName = ((CLink)_list.Selected).subItems[0];
      editPage.Number = ((CLink)_list.Selected).subItems[1];
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

    public string FirstName
    {
      set { _fname.Caption = value; }
    }
    public string LastName
    {
      set { _lname.Caption = value; }
    }
    public string Number
    {
      set { _number.Caption = value; }
    }

    public CPhonebookEditPage()
      : base((int)EPages.P_PHONEBOOKEDIT, "Editing")
    {
      _fname = new CEditField("First Name>", "", EEditMode.alphanum_high, true);
      _fname.PosY = 2;
      _fname.LinkKey = _fname.PosY;
      add(_fname);

      _lname = new CEditField("Last Name>", "", EEditMode.alphanum_high, false);
      _lname.PosY = 4;
      _lname.LinkKey = _lname.PosY;
      add(_lname);

      _number = new CEditField("Number>", "", EEditMode.numeric);
      _number.PosY = 6;
      _number.LinkKey = _number.PosY;
      add(_number);

      CLink saveLink = new CLink("Save!");
      saveLink.PosY = 8;
      saveLink.LinkKey = saveLink.PosY;
      saveLink.Align = EAlignment.justify_right;
      saveLink.Softkey += new BoolIntDelegate(saveLink_Softkey);
      add(saveLink);

      CLink deleteLink = new CLink("Delete!");
      deleteLink.PosY = 9;
      deleteLink.LinkKey = deleteLink.PosY;
      deleteLink.Softkey += new BoolIntDelegate(deleteLink_Softkey);
      add(deleteLink);
    }

    bool saveLink_Ok()
    {
      return saveLink_Softkey(0);
    }

    bool saveLink_Softkey(int keyId)
    {
      CPhonebookRecord record = new CPhonebookRecord();
      record.FirstName = _fname.Caption;
      record.LastName = _lname.Caption;
      record.Number = _number.Caption;

      CPhonebook.getInstance().addRecord(record);
      CPhonebook.getInstance().save();

      _controller.previousPage();
      return true;
    }

    bool deleteLink_Softkey(int keyId)
    {
      CPhonebook.getInstance().deleteRecord(_lname.Caption);
      CPhonebook.getInstance().save();

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
      _list = new CSelectList(7);
      _list.PosY = 4;
      add(_list);

      CLink detailsLink = new CLink("Details");
      detailsLink.PosY = 3;
      detailsLink.LinkKey = detailsLink.PosY;
      detailsLink.Align = EAlignment.justify_right;
      detailsLink.Softkey += new BoolIntDelegate(detailsLink_Softkey);
      add(detailsLink);

      Menu += new VoidDelegate(CCalllogPage_Menu);
    }

    public override void onEntry()
    {
      _list.removeAll();

      Collection<CCallRecord> results = CCallLog.getInstance().getList();

      foreach (CCallRecord item in results)
      {
        CLink recordLink = new CLink(item.Name + " " + item.Number + " " + item.Time);
        //recordLink.subItems[0] = item.LastName;
        //recordLink.subItems[1] = item.FirstName;
        recordLink.subItems[2] = item.Number;
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

    bool detailsLink_Softkey(int keyId)
    {
/*      
      CPhonebookEditPage page = (CPhonebookEditPage)_controller.getPage((int)EPages.P_PHONEBOOKEDIT);
      CLink selitem = (CLink)_list.Selected;
      page.LastName = selitem.subItems[0];
      page.FirstName = selitem.subItems[1];
      page.Number = selitem.subItems[2];
      
      _controller.showPage(page.Id);
*/
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
      linkNetwork.PosY = 5;
      add(linkNetwork);

      CLink linkSound = new CLink("Sound", 0);
      linkSound.PosY = 6;
      linkSound.Align = EAlignment.justify_right;
      add(linkSound);

      CLink linkSIP = new CLink("SIP", (int)EPages.P_SIPSETTINGS);
      linkSIP.PosY = 7;
      linkSIP.LinkKey = linkSIP.PosY;
      add(linkSIP);

    }

  }

  public class CSIPSettings : CPage
  {
    CEditField _editport;
    CSelectList _list;

    public CSIPSettings() 
      : base((int)EPages.P_SIPSETTINGS, "SIP Settings") 
    {
      _list = new CSelectList(3);
      _list.PosY = 2;
      add(_list);
      
      _editport = new CEditField("Port>", "", EEditMode.numeric);
      _editport.PosY = 7;
      _editport.LinkKey = _editport.PosY;
      add(_editport);

      CLink proxyLink = new CLink("Default Account", (int)EPages.P_SIPPROXYSETTINGS);
      proxyLink.Align = EAlignment.justify_right;
      proxyLink.PosY = 8;
      proxyLink.LinkKey = proxyLink.PosY;
      add(proxyLink);

      CLink addproxyLink = new CLink("Add New Account", 0); // todo!!!
      addproxyLink.PosY = 9;
      addproxyLink.LinkKey = addproxyLink.PosY;
      add(addproxyLink);

      // ok handler
      this.Ok += new VoidDelegate(CSIPSettings_Ok);
    }

    public override void onEntry()
    {
      CAccounts accounts = CAccounts.getInstance();

      _list.removeAll();

      for (int i = 0; i < accounts.getSize(); i++ )
      {
        CLink accLink = new CLink(accounts[i].Name, (int)EPages.P_SIPPROXYSETTINGS); // todo
        _list.add(accLink);
      }

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
    //CIpAddressEdit _editProxyAddress;
    CEditField _editProxyAddress;
    CEditField _editProxyPort;
    CCheckBox _checkRegister;
    CEditField _editperiod;
    CEditField _editDisplayName;
    CEditField _editId;

    public CSIPProxySettings()
      : base((int)EPages.P_SIPPROXYSETTINGS, "SIP Proxy Settings")
    {
      _editDisplayName = new CEditField("Name>", "",true);
      _editDisplayName.PosY = 1;
      _editDisplayName.LinkKey = _editDisplayName.PosY;
      add(_editDisplayName);

      _editId = new CEditField("Id>", "");
      _editId.PosY = 3;
      _editId.LinkKey = _editId.PosY;
      add(_editId);

      //_editProxyAddress = new CIpAddressEdit("Proxy>");
      _editProxyAddress = new CEditField("Proxy>","");
      _editProxyAddress.PosY = 5;
      _editProxyAddress.LinkKey = _editProxyAddress.PosY;
      add(_editProxyAddress);

      _editProxyPort = new CEditField("Port>", "", EEditMode.numeric);
      _editProxyPort.PosY = 7;
      _editProxyPort.LinkKey = _editProxyPort.PosY;
      add(_editProxyPort);


      _checkRegister = new CCheckBox("Register");
      _checkRegister.PosY = 7;
      _checkRegister.LinkKey = _checkRegister.PosY;
      //add(_checkRegister);

      _editperiod = new CEditField("Period>", "", EEditMode.numeric);
      _editperiod.PosY = 9;
      _editperiod.LinkKey = _editperiod.PosY;
      add(_editperiod);


      CLink moreLink = new CLink("More", (int)EPages.P_SIPPROXYSETTINGSMORE);
      moreLink.PosY = 10;
      moreLink.Align = EAlignment.justify_right;
      add(moreLink);

      this.Ok += new VoidDelegate(CSIPProxySettings_Ok);
    }

    public override void onEntry()
    {
      _editperiod.Caption = CAccounts.getInstance().DefAccount.Period.ToString(); // Properties.Settings.Default.cfgSipAccountRegPeriod.ToString();
      _editId.Caption = CAccounts.getInstance().DefAccount.Id; 
      _editDisplayName.Caption = CAccounts.getInstance().DefAccount.Name; // Properties.Settings.Default.cfgSipAccountDisplayName[0];
      _editProxyAddress.Caption = CAccounts.getInstance().DefAccount.Address;
      _editProxyPort.Caption = CAccounts.getInstance().DefAccount.Port.ToString();
      _editperiod.Caption = CAccounts.getInstance().DefAccount.Period.ToString();

      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      CAccount account = CAccounts.getInstance().DefAccount;
      account.Address = _editProxyAddress.Caption;
      account.Port = int.Parse(_editProxyPort.Caption);
      account.Name = _editDisplayName.Caption;
      account.Period = int.Parse(_editperiod.Caption);
      account.Id = _editId.Caption;

      int defIndex = CAccounts.getInstance().DefAccountIndex;
      CAccounts.getInstance()[defIndex] = account;

      CAccounts.getInstance().save();
 
      _controller.previousPage();

      return true;
    }
  }

  /// <summary>
  /// CSIPProxySettingsMore
  /// </summary>
  public class CSIPProxySettingsMore : CPage
  {
    CEditField _editUserName;
    CEditField _editPassword;

    public CSIPProxySettingsMore()
      : base((int)EPages.P_SIPPROXYSETTINGSMORE, "SIP Proxy Settings")
    {
      _editUserName = new CEditField("Username>", "", true);
      _editUserName.PosY = 1;
      _editUserName.LinkKey = _editUserName.PosY;
      add(_editUserName);

      _editPassword = new CEditField("Password>", "");
      _editPassword.PosY = 3;
      _editPassword.LinkKey = _editPassword.PosY;
      add(_editPassword);

      CLink moreLink = new CLink("Back", (int)EPages.P_SIPPROXYSETTINGS);
      moreLink.PosY = 10;
      moreLink.Align = EAlignment.justify_right;
      add(moreLink);

      this.Ok += new VoidDelegate(CSIPProxySettings_Ok);
    }

    public override void onEntry()
    {
      _editUserName.Caption = CAccounts.getInstance().DefAccount.Username;
      _editPassword.Caption = CAccounts.getInstance().DefAccount.Password; 

      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      CAccount account = CAccounts.getInstance().DefAccount; 
      account.Username = _editUserName.Caption;
      account.Password = _editPassword.Caption;

      int defIndex = CAccounts.getInstance().DefAccountIndex;
      CAccounts.getInstance()[defIndex] = account;

      CAccounts.getInstance().save();

      _controller.previousPage();
      // todo!!!
      //CCallManager.getInstance().initialize();

      return true;
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
      _silentCb.PosY = 3;
      _silentCb.LinkKey = _silentCb.PosY;
      _silentCb.Softkey += new BoolIntDelegate(_silentCb_Softkey);
      _radio.add(_silentCb);

      _melodyCb = new CCheckBox("Melody");
      _melodyCb.PosY = 4;
      _melodyCb.LinkKey = _melodyCb.PosY;
      _melodyCb.Softkey += new BoolIntDelegate(_melodyCb_Softkey);
      _radio.add(_melodyCb);
      
      _beepCb = new CCheckBox("Beep");
      _beepCb.PosY = 5;
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

} // namespace Sipek
