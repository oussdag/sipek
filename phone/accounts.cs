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

    // configuration data 
    private string _name;
    private int _port = 5060;
    private string _address;
    private string _id;
    private int _period = 3600;
    private string _username;
    private string _password;

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
    
    // runtime data
    public ERegistrationState RegState 
    {
      get { return _registrationState; }
      set 
      { 
        _registrationState = value;        
      }
    }
  }

  public class CAccounts
  {
    private static CAccounts _instance = null;
    private Dictionary<int, CAccount> _accounts;
    private int _defaccountId = 0;

    public CAccount DefAccount
    {
      get { return this[_defaccountId]; } // todo!!!
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
      int count = Properties.Settings.Default.cfgSipAccountNames.Count;
      for (int i = 0; i < count; i++)
      {
        CAccount account = new CAccount();
        account.Id = Properties.Settings.Default.cfgSipAccountIds[i];
        account.Name = Properties.Settings.Default.cfgSipAccountNames[i];
        account.Address = Properties.Settings.Default.cfgSipAccountAddresses[i];
        account.Port = Int16.Parse(Properties.Settings.Default.cfgSipAccountPorts[i]);
        account.Username = Properties.Settings.Default.cfgSipAccountUsername[i];
        account.Password = Properties.Settings.Default.cfgSipAccountPassword[i];

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

      }
    }

    public int getSize()
    {
      return Properties.Settings.Default.cfgSipAccountNames.Count;
    }

  }
}
