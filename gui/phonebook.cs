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

using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gui
{

  public class CPhonebookRecord
  {
    private string _firstName;
    private string _lastName;
    private string _number;
    private string _email;
    private int _accountId;

    public string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    public string Number
    {
      get { return _number; }
      set { _number = value; }
    }

    public int Accountid
    {
      get { return _accountId; }
      set { _accountId = value; }
    }
  }

  /// <summary>
  /// CPhonebook
  /// </summary>
  public class CPhonebook
  {
    private static CPhonebook _instance = null;
    
    private XmlDocument _xmlDocument;

    private string XMLPhonebookFile = "phonebook.xml";


    public static CPhonebook getInstance()
    {
      if (_instance == null)
      {
        _instance = new CPhonebook();
      }
      return _instance;
    }

    public CPhonebook()
    {
      _xmlDocument = new XmlDocument();
      load();
    }

    public void load()
    {
      try
      {
        _xmlDocument.Load(XMLPhonebookFile);
      }
      catch (System.IO.FileNotFoundException ee) 
      { 
        System.Console.WriteLine(ee.Message);

        XmlNode root = _xmlDocument.CreateNode("element","Phonebook","");
        _xmlDocument.AppendChild(root);

      }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }
    }

    public void save()
    {
      try
      {
        _xmlDocument.Save(XMLPhonebookFile);
      }
      catch (System.IO.FileNotFoundException ee) { System.Console.WriteLine(ee.Message); }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }
    }

    //////////////////////////////////////////////////////////////////////////////////////
    public CPhonebookRecord getRecord(string field, string criteria)
    { 
      //string groupXpath = "[attribute::name=\"" + group + "\"]";
      //string paramXpath = "[attribute::name=\"" + param + "\"]";
      //XmlNode foundNode = xmlSettings.SelectSingleNode("/settings/group" + groupXpath);
      return null;
    }

    public Collection<CPhonebookRecord> getList()
    {
      Collection<CPhonebookRecord> result = new Collection<CPhonebookRecord>();

      XmlNodeList list = _xmlDocument.SelectNodes("/Phonebook/Record");

      foreach (XmlNode item in list)
      {
        CPhonebookRecord record = new CPhonebookRecord();
        record.FirstName = item.ChildNodes[0].InnerText;
        record.LastName = item.ChildNodes[1].InnerText;

        XmlNode node = item.ChildNodes[2];
        record.Number = node.ChildNodes[0].InnerText; 

        result.Add(record);
      }

      return result;
    }

    public void addRecord(CPhonebookRecord record)
    {
      XmlNode nodeRecord = _xmlDocument.CreateNode("element","Record","");

      XmlElement ellastname = _xmlDocument.CreateElement("LastName");
      ellastname.InnerText = record.LastName;
      nodeRecord.AppendChild(ellastname);

      XmlElement elname = _xmlDocument.CreateElement("FirstName");
      elname.InnerText = record.FirstName;
      nodeRecord.AppendChild(elname);

      XmlElement phelem = _xmlDocument.CreateElement("Phone");

        XmlElement elNumber = _xmlDocument.CreateElement("phonenumber");
        elNumber.InnerText = record.Number;
        phelem.AppendChild(elNumber);
      nodeRecord.AppendChild(phelem);

      _xmlDocument.DocumentElement.AppendChild(nodeRecord);
    }

    public void deleteRecord(string name)
    {
      string path = "/phonebook/record" + "[name='" + name + "']";
      XmlNode node = _xmlDocument.SelectSingleNode(path);
      _xmlDocument.DocumentElement.RemoveChild(node);
    }

  }

} // namespace Gui
