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
using System.Collections.ObjectModel;

namespace Gui
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
    P_RINGMODE
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
    CLink _linkRinger;

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

      _linkRinger = new CLink("Ring Mode", (int)EPages.P_RINGMODE);
      _linkRinger.Align = EAlignment.justify_right;
      _linkRinger.PosY = 7;
      _linkRinger.LinkKey = _linkRinger.PosY;
      this.add(_linkRinger);

      CLink mlinkCalls = new CLink("Calls", 0);
      mlinkCalls.Align = EAlignment.justify_right;
      mlinkCalls.PosY = 9;
      mlinkCalls.LinkKey = mlinkCalls.PosY;
      this.add(mlinkCalls);

      CLink mlinkLines = new CLink("Accounts", 0);
      mlinkLines.PosY = 6;
      this.add(mlinkLines);

      // Initialize handlers
      Digitkey += new UintDelegate(digitkeyHandler);
      Offhook += new NoParamDelegate(IdlePage_Offhook);
      Menu += new NoParamDelegate(IdlePage_Menu);
    }

    public override void onEntry()
    {
      base.onEntry();
      
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
  public class CPhonebookPage : CPage
  {
    private CEditField _criteria;
    private CSelectList _list;

    public CPhonebookPage() : base((int)EPages.P_PHONEBOOK,"Phonebook") 
    {
      _criteria = new CEditField(">", "", EEditMode.alphanum_low, true);
      _criteria.PosY = 1;
      _criteria.Digitkey += new UintDelegate(_criteria_Digitkey);
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
      modifyLink.Softkey += new UintDelegate(modifyLink_Softkey);
      modifyLink.Align = EAlignment.justify_right;
      add(modifyLink);

      Menu += new NoParamDelegate(CPhonebookPage_Menu);
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
        _list.add(recordLink);
      }

      base.onEntry();
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
      saveLink.Softkey += new UintDelegate(saveLink_Softkey);
      add(saveLink);

      CLink deleteLink = new CLink("Delete!");
      deleteLink.PosY = 9;
      deleteLink.LinkKey = deleteLink.PosY;
      deleteLink.Softkey += new UintDelegate(deleteLink_Softkey);
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
    CEditField _editDisplayName;
    CEditField _editport;

    public CSIPSettings() 
      : base((int)EPages.P_SIPSETTINGS, "SIP Settings") 
    {
      _editDisplayName = new CEditField("Name>", "");
      _editDisplayName.PosY = 3;
      _editDisplayName.LinkKey = _editDisplayName.PosY;
      add(_editDisplayName);

      _editport = new CEditField("Port>", "", EEditMode.numeric);
      _editport.PosY = 5;
      _editport.LinkKey = _editport.PosY;
      add(_editport);

      CLink proxyLink = new CLink("Proxy", (int)EPages.P_SIPPROXYSETTINGS);
      proxyLink.Align = EAlignment.justify_right;
      proxyLink.PosY = 7;
      proxyLink.LinkKey = proxyLink.PosY;
      add(proxyLink);

      // ok handler
      this.Ok += new NoParamDelegate(CSIPSettings_Ok);
    }

    bool CSIPSettings_Ok()
    {
      Properties.Settings.Default.cfgSipProxy = _editDisplayName.Caption;
      Properties.Settings.Default.cfgSipPort = int.Parse(_editport.Caption);

      Properties.Settings.Default.Save();

      _controller.previousPage();
      return true;
    }

    public override void onEntry()
    {
      _editDisplayName.Caption = Properties.Settings.Default.cfgSipDisplayName;
      _editport.Caption = Properties.Settings.Default.cfgSipPort.ToString();

      base.onEntry();
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CSIPProxySettings : CPage
  {
    CIpAddressEdit _editProxyAddress;
    CEditField _editProxyPort;
    CCheckBox _checkRegister;
    CEditField _editperiod;

    public CSIPProxySettings()
      : base((int)EPages.P_SIPPROXYSETTINGS, "SIP Proxy Settings")
    {
      _editProxyAddress = new CIpAddressEdit("Proxy>");
      _editProxyAddress.PosY = 3;
      _editProxyAddress.Focused = true;
      _editProxyAddress.LinkKey = _editProxyAddress.PosY;
      add(_editProxyAddress);

      _editProxyPort = new CEditField("Port>", "", EEditMode.numeric);
      _editProxyPort.PosY = 5;
      _editProxyPort.LinkKey = _editProxyPort.PosY;
      add(_editProxyPort);


      _checkRegister = new CCheckBox("Register");
      _checkRegister.PosY = 7;
      _checkRegister.LinkKey = _checkRegister.PosY;
      add(_checkRegister);

      _editperiod = new CEditField("Period>", "", EEditMode.numeric);
      _editperiod.PosY = 9;
      _editperiod.LinkKey = _editperiod.PosY;
      add(_editperiod);

      this.Ok += new NoParamDelegate(CSIPProxySettings_Ok);
    }

    public override void onEntry()
    {
      _editProxyAddress.Caption = Properties.Settings.Default.cfgSipProxy;
      _editProxyPort.Caption = Properties.Settings.Default.cfgSipPort.ToString();
      _checkRegister.Checked = Properties.Settings.Default.cfgSipRegister;
      _editperiod.Caption = Properties.Settings.Default.cfgSipRegPeriod.ToString();

      base.onEntry();
    }

    bool CSIPProxySettings_Ok()
    {
      Properties.Settings.Default.cfgSipProxy = _editProxyAddress.Caption;
      Properties.Settings.Default.cfgSipProxyPort = int.Parse(_editProxyPort.Caption);
      Properties.Settings.Default.cfgSipRegister = _checkRegister.Checked;
      Properties.Settings.Default.cfgSipRegPeriod = int.Parse(_editperiod.Caption);

      Properties.Settings.Default.Save();

      CCallManager.getInstance().updateConfig(Properties.Settings.Default);

      _controller.previousPage();

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
      _silentCb.Softkey += new UintDelegate(_silentCb_Softkey);
      _radio.add(_silentCb);

      _melodyCb = new CCheckBox("Melody");
      _melodyCb.PosY = 4;
      _melodyCb.LinkKey = _melodyCb.PosY;
      _melodyCb.Softkey += new UintDelegate(_melodyCb_Softkey);
      _radio.add(_melodyCb);
      
      _beepCb = new CCheckBox("Beep");
      _beepCb.PosY = 5;
      _beepCb.LinkKey = _beepCb.PosY;
      _beepCb.Softkey += new UintDelegate(_beepCb_Softkey);
      _radio.add(_beepCb);

      add(_radio);
    }

    bool _beepCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = 2;
      Properties.Settings.Default.Save();
      return true;
    }

    bool _melodyCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = 1;
      Properties.Settings.Default.Save();
      return true;
    }

    bool _silentCb_Softkey(int keyId)
    {
      Properties.Settings.Default.cfgRingMode = 0;
      Properties.Settings.Default.Save();
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

} // namespace Gui
