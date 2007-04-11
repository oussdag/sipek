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

namespace Sipek
{
  public enum ERegistrationState
  {
    ERegistered,
    ENotRegistered
  }

  public class CAccount
  {
    // runtime data
    ERegistrationState _registrationState = ERegistrationState.ENotRegistered;
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

    public ERegistrationState RegState 
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
      int count = Properties.Settings.Default.cfgSipAccountAddresses.Count;
      for (int i = 0; i < count; i++)
      {
        CAccount account = new CAccount();
        account.Id = Properties.Settings.Default.cfgSipAccountIds[i];
        account.Name = Properties.Settings.Default.cfgSipAccountNames[i];
        account.Address = Properties.Settings.Default.cfgSipAccountAddresses[i];
        account.Port = Int16.Parse(Properties.Settings.Default.cfgSipAccountPorts[i]);
        account.Username = Properties.Settings.Default.cfgSipAccountUsername[i];
        account.Password = Properties.Settings.Default.cfgSipAccountPassword[i];
        account.Domain = Properties.Settings.Default.cfgSipAccountDomains[i];
        account.Period = Int16.Parse(Properties.Settings.Default.cfgSipAccountRegPeriod[i]);
        account.Index = i;

        _accounts.Add(i, account);
      }
    }

    public CAccount this[int index]
    {
      get
      {
        CAccount account = _accounts[index];
        account.Name = Properties.Settings.Default.cfgSipAccountNames[index];
        account.Id = Properties.Settings.Default.cfgSipAccountIds[index];
        account.Address = Properties.Settings.Default.cfgSipAccountAddresses[index];
        account.Port = Int16.Parse(Properties.Settings.Default.cfgSipAccountPorts[index]);
        account.Username = Properties.Settings.Default.cfgSipAccountUsername[index];
        account.Password = Properties.Settings.Default.cfgSipAccountPassword[index];
        account.Domain = Properties.Settings.Default.cfgSipAccountDomains[index];
        account.Period = Int16.Parse(Properties.Settings.Default.cfgSipAccountRegPeriod[index]);

        return account;
      }
      set
      {
        CAccount account = _accounts[index];

        Properties.Settings.Default.cfgSipAccountNames[index] = account.Name = value.Name;
        Properties.Settings.Default.cfgSipAccountIds[index] = account.Id = value.Id;
        Properties.Settings.Default.cfgSipAccountAddresses[index] = account.Address = value.Address;
        account.Port = value.Port;
        Properties.Settings.Default.cfgSipAccountPorts[index] = value.Port.ToString();
        Properties.Settings.Default.cfgSipAccountUsername[index] = account.Username = value.Username;
        Properties.Settings.Default.cfgSipAccountPassword[index] = account.Password = value.Password;
        Properties.Settings.Default.cfgSipAccountDomains[index] = account.Domain = value.Domain;
        account.Period = value.Period;
        Properties.Settings.Default.cfgSipAccountRegPeriod[index] = value.Period.ToString();
      }
    }

    public int getSize()
    {
      return Properties.Settings.Default.cfgSipAccountNames.Count;
    }

    public void save()
    {
      int count = Properties.Settings.Default.cfgSipAccountAddresses.Count;
      for (int index = 0; index < count; index++)
      {
        Properties.Settings.Default.cfgSipAccountAddresses[index] = this[index].Address;
        Properties.Settings.Default.cfgSipAccountPorts[index] = this[index].Port.ToString();
        Properties.Settings.Default.cfgSipAccountNames[index] = this[index].Name;
        Properties.Settings.Default.cfgSipAccountRegPeriod[index] = this[index].Period.ToString();
        Properties.Settings.Default.cfgSipAccountIds[index] = this[index].Id;
        Properties.Settings.Default.cfgSipAccountUsername[index] = this[index].Username;
        Properties.Settings.Default.cfgSipAccountPassword[index] = this[index].Password;
        Properties.Settings.Default.cfgSipAccountDomains[index] = this[index].Domain;
        Properties.Settings.Default.Save();
      }
    }

  }
}
