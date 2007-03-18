using System;
using System.Collections.Generic;
using System.Text;

namespace Sipek
{

  public class CAccount
  {
    private int _port = 5060;
    private string _address;
    private string _name;
    private int _period = 3600;

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
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    
    public int Period
    {
      get { return _period; }
      set { _period = value; }
    }
  }

  public class CAccounts
  {
    private static CAccounts _instance = null;
    
    public static CAccounts getInstance()
    {
      if (_instance == null)
      {
        _instance = new CAccounts();
      }
      return _instance;
    }

    public CAccount this[int index]
    {
      get
      {
        CAccount account = new CAccount();
        account.Name = Properties.Settings.Default.cfgSipAccountNames[index];
        account.Address = Properties.Settings.Default.cfgSipAccountAddresses[index];
        account.Port = Int16.Parse(Properties.Settings.Default.cfgSipAccountPorts[index]);
        return account;
      }
      set
      {
        Properties.Settings.Default.cfgSipAccountNames[index] = value.Name;
        Properties.Settings.Default.cfgSipAccountAddresses[index] = value.Address;
        Properties.Settings.Default.cfgSipAccountPorts[index] = value.Port.ToString();
      }
    }

    public int getSize()
    {
      return Properties.Settings.Default.cfgSipAccountNames.Count;
    }

    protected CAccounts()
    {
    }
  }
}
