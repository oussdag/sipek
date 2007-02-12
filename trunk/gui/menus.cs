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
    P_PHONEBOOK,
    P_MENU,
    P_SIPSETTINGS
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

      // Initialize handlers
      Digitkey += new UintDelegate(digitkeyHandler);
      Offhook += new NoParamDelegate(IdlePage_Offhook);
      Menu += new NoParamDelegate(IdlePage_Menu);
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
    public CPhonebookPage() : base((int)EPages.P_PHONEBOOK,"Phonebook") 
    {
    }


  }


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
    CIpAddressEdit _editproxy;
    CEditField _editport;
    CCheckBox _checkRegister;
    CEditField _editperiod;

    public CSIPSettings() 
      : base((int)EPages.P_SIPSETTINGS, "SIP Settings") 
    {
      _editproxy = new CIpAddressEdit("Proxy>");
      _editproxy.PosY = 3;
      _editproxy.Focused = true;
      _editproxy.LinkKey = _editproxy.PosY;
      add(_editproxy);

      _editport = new CEditField("Port>","",EEditMode.numeric);
      _editport.PosY = 5;
      _editport.LinkKey = _editport.PosY;
      add(_editport);

      _checkRegister = new CCheckBox("Register");
      _checkRegister.PosY = 7;
      _checkRegister.LinkKey = _checkRegister.PosY;
      add(_checkRegister);

      _editperiod = new CEditField("Period>", "", EEditMode.numeric);
      _editperiod.PosY = 9;
      _editperiod.LinkKey = _editperiod.PosY;
      add(_editperiod);

      // ok handler
      this.Ok += new NoParamDelegate(CSIPSettings_Ok);
    }

    bool CSIPSettings_Ok()
    {
      Properties.Settings.Default.cfgSipProxy = _editproxy.Caption;
      Properties.Settings.Default.cfgSipPort = int.Parse(_editport.Caption);
      Properties.Settings.Default.cfgSipRegister = _checkRegister.Checked;
      Properties.Settings.Default.cfgSipRegPeriod = int.Parse(_editperiod.Caption);

      Properties.Settings.Default.Save();

      _controller.previousPage();
      return true;
    }

    public override void onEntry()
    {
      _editproxy.Caption = Properties.Settings.Default.cfgSipProxy;
      _editport.Caption = Properties.Settings.Default.cfgSipPort.ToString();
      _checkRegister.Checked = Properties.Settings.Default.cfgSipRegister;
      _editperiod.Caption = Properties.Settings.Default.cfgSipRegPeriod.ToString();

      base.onEntry();
    }


  }

} // namespace Gui
