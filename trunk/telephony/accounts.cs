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
using System.Text;

namespace Telephony
{

  public class CAccount
  {
    // runtime data
    int _registrationState = 0;
    int _index = 0;

    // configuration data 
    private string _name;
    private int _port = 5060;
    private string _address;
    private string _id;
    private int _period = 3600;
    private string _username;
    private string _password;
    private string _domain;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public int Port
    {
      get { return _port; }
      set { _port = value; }
    }
  
    public string Address
    {
      get { return _address; }
      set { _address = value; }
    }
    public string Id
    {
      get { return _id; }
      set { _id = value; }
    }
    
    public int Period
    {
      get { return _period; }
      set { _period = value; }
    }

    public string Username
    {
      get { return _username; }
      set { _username = value; }
    }

    public string Password
    {
      get { return _password; }
      set { _password = value; }
    }

    public string Domain
    {
      get { return _domain; }
      set { _domain = value; }
    }

    // runtime data
    public int Index
    {
      get { return _index; }
      set { _index = value; }
    }

    public int RegState 
    {
      get { return _registrationState; }
      set 
      { 
        _registrationState = value;        
      }
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CAccounts
  {
    private Properties.Settings _settings = Properties.Settings.Default;

    private static CAccounts _instance = null;
    private Dictionary<int, CAccount> _accounts;
    private int _defaccountId = 0;

    public CAccount DefAccount
    {
      get { return this[_defaccountId]; }
    }

    public int DefAccountIndex
    {
      get { return _defaccountId; }
      set { _defaccountId = value;  }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static CAccounts getInstance()
    {
      if (_instance == null)
      {
        _instance = new CAccounts();
      }
      return _instance;
    }

    protected CAccounts()
    {
      _accounts = new Dictionary<int, CAccount>();

      int count = _settings.cfgSipAccountAddresses.Count;

      this.DefAccountIndex = _settings.cfgSipAccountDefault;

      for (int i = 0; i < count; i++)
      {
        CAccount account = new CAccount();
        account.Id = _settings.cfgSipAccountIds[i];
        account.Name = _settings.cfgSipAccountNames[i];
        account.Address = _settings.cfgSipAccountAddresses[i];
        account.Port = Int16.Parse(_settings.cfgSipAccountPorts[i]);
        account.Username = _settings.cfgSipAccountUsername[i];
        account.Password = _settings.cfgSipAccountPassword[i];
        account.Domain = _settings.cfgSipAccountDomains[i];
        account.Period = Int16.Parse(_settings.cfgSipAccountRegPeriod[i]);
        account.Index = i;

        _accounts.Add(i, account);
      }
    }

    public CAccount this[int index]
    {
      get
      {
        CAccount account = _accounts[index];
        account.Name = _settings.cfgSipAccountNames[index];
        account.Id = _settings.cfgSipAccountIds[index];
        account.Address = _settings.cfgSipAccountAddresses[index];
        account.Port = Int16.Parse(_settings.cfgSipAccountPorts[index]);
        account.Username = _settings.cfgSipAccountUsername[index];
        account.Password = _settings.cfgSipAccountPassword[index];
        account.Domain = _settings.cfgSipAccountDomains[index];
        account.Period = Int16.Parse(_settings.cfgSipAccountRegPeriod[index]);

        return account;
      }
      set
      {
        CAccount account = _accounts[index];

        _settings.cfgSipAccountNames[index] = account.Name = value.Name;
        _settings.cfgSipAccountIds[index] = account.Id = value.Id;
        _settings.cfgSipAccountAddresses[index] = account.Address = value.Address;
        account.Port = value.Port;
        _settings.cfgSipAccountPorts[index] = value.Port.ToString();
        _settings.cfgSipAccountUsername[index] = account.Username = value.Username;
        _settings.cfgSipAccountPassword[index] = account.Password = value.Password;
        _settings.cfgSipAccountDomains[index] = account.Domain = value.Domain;
        account.Period = value.Period;
        _settings.cfgSipAccountRegPeriod[index] = value.Period.ToString();
      }
    }

    public int getSize()
    {
      return _settings.cfgSipAccountNames.Count;
    }

    public void save()
    {
      int count = _settings.cfgSipAccountAddresses.Count;

      _settings.cfgSipAccountDefault = this.DefAccountIndex;

      for (int index = 0; index < count; index++)
      {
        _settings.cfgSipAccountAddresses[index] = this[index].Address;
        _settings.cfgSipAccountPorts[index] = this[index].Port.ToString();
        _settings.cfgSipAccountNames[index] = this[index].Name;
        _settings.cfgSipAccountRegPeriod[index] = this[index].Period.ToString();
        _settings.cfgSipAccountIds[index] = this[index].Id;
        _settings.cfgSipAccountUsername[index] = this[index].Username;
        _settings.cfgSipAccountPassword[index] = this[index].Password;
        _settings.cfgSipAccountDomains[index] = this[index].Domain;
        _settings.Save();
      }
    }


    public void reload()
    {
      _settings.Reload();
    }
  }
}
