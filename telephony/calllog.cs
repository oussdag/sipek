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
using System.Xml;
using System.Collections.ObjectModel;

namespace Telephony
{
  public enum ECallType : int
  {
    EDialed,
    EReceived,
    EMissed,
    EUndefined
  }  
  
  public class CCallRecord
  { 
    private ECallType _type;
    private string _name;
    private string _number;
    private DateTime _time;
    private TimeSpan _duration;
    private int _count;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    public string Number
    {
      get { return _number; }
      set { _number = value; }
    }
    public ECallType Type
    {
      get { return _type; }
      set { _type = value; }
    }
    public TimeSpan Duration
    {
      get { return _duration; }
      set { _duration = value; }
    }
    public DateTime Time
    {
      get { return _time; }
      set { _time = value; }
    }
    public int Count
    {
      get { return _count; }
      set { _count = value; }
    }
  }

  public class CCallLog
  {
    private static CCallLog _instance = null;
    
    private XmlDocument _xmlDocument;

    private string XMLCallLogFile = "calllog.xml";


    public static CCallLog getInstance()
    {
      if (_instance == null)
      {
        _instance = new CCallLog();
      }
      return _instance;
    }

    public CCallLog()
    {
      _xmlDocument = new XmlDocument();
      load();
    }

    public void load()
    {
      try
      {
        _xmlDocument.Load(XMLCallLogFile);
      }
      catch (System.IO.FileNotFoundException ee)
      {
        System.Console.WriteLine(ee.Message);

        XmlNode root = _xmlDocument.CreateNode("element", "Calllog", "");
        _xmlDocument.AppendChild(root);

      }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }
    }

    public void save()
    {
      try
      {
        _xmlDocument.Save(XMLCallLogFile);
      }
      catch (System.IO.FileNotFoundException ee) { System.Console.WriteLine(ee.Message); }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////

    public Collection<CCallRecord> getList()
    {
      Collection<CCallRecord> result = new Collection<CCallRecord>();

      XmlNodeList list = _xmlDocument.SelectNodes("/Calllog/Record");
      
      foreach (XmlNode item in list)
      {
        CCallRecord record = new CCallRecord();
        record.Type = (ECallType)int.Parse(item.ChildNodes[0].InnerText);
        record.Name = item.ChildNodes[1].InnerText;
        record.Number = item.ChildNodes[2].InnerText;
        record.Time = DateTime.Parse(item.ChildNodes[3].InnerText);
        record.Duration = TimeSpan.Parse(item.ChildNodes[4].InnerText);
        record.Count = int.Parse(item.ChildNodes[5].InnerText);

        result.Add(record);
      }
      
      return result;
    }

    public XmlNode findRecord(string number)
    { 
      XmlNodeList list = _xmlDocument.SelectNodes("/Calllog/Record");

      foreach (XmlNode item in list)
      {
        if (item.ChildNodes[2].InnerText == number)
        {
          return item;
        }
      }
      return null;
    }

    protected void addRecord(CCallRecord record)
    {
      int count = 1;

      XmlNode searchRes = findRecord(record.Number);

      if (searchRes != null)
      {
        try
        {
          count = int.Parse(searchRes.ChildNodes[5].InnerText);
          count++;
        }
        catch (Exception e)
        {
          count = 1;
          XmlElement xmlcount = _xmlDocument.CreateElement("Count");
          searchRes.AppendChild(xmlcount);
        }
        searchRes.ChildNodes[5].InnerText = count.ToString();
        return;
      }

      XmlNode nodeRecord = _xmlDocument.CreateNode("element", "Record", "");

      XmlElement eltype = _xmlDocument.CreateElement("Type");
      eltype.InnerText = ((int)record.Type).ToString();
      nodeRecord.AppendChild(eltype);

      XmlElement ellastname = _xmlDocument.CreateElement("Name");
      ellastname.InnerText = record.Name;
      nodeRecord.AppendChild(ellastname);

      XmlElement elname = _xmlDocument.CreateElement("Number");
      elname.InnerText = record.Number;
      nodeRecord.AppendChild(elname);

      XmlElement eltime = _xmlDocument.CreateElement("Time");
      eltime.InnerText = record.Time.ToString();
      nodeRecord.AppendChild(eltime);

      XmlElement eldur = _xmlDocument.CreateElement("Duration");
      eldur.InnerText = record.Duration.ToString();
      nodeRecord.AppendChild(eldur);

      XmlElement elcount = _xmlDocument.CreateElement("Count");
      elcount.InnerText = count.ToString();
      nodeRecord.AppendChild(elcount);

      _xmlDocument.DocumentElement.AppendChild(nodeRecord);
    }

    public void deleteRecord(string name)
    {
      string path = "/phonebook/record" + "[name='" + name + "']";
      XmlNode node = _xmlDocument.SelectSingleNode(path);
      _xmlDocument.DocumentElement.RemoveChild(node);

      save();
    }

    public void clearAll()
    {
      XmlNodeList list = _xmlDocument.SelectNodes("/Calllog/Record");
      _xmlDocument.DocumentElement.RemoveAll();

      save();
    }

    /////////////////////////////////////////////////////////////////////////////////////
    public void addCall(ECallType type, string number, DateTime time, TimeSpan duration)
    {
      int delta = (int)duration.TotalSeconds;

      CCallRecord record = new CCallRecord();
      record.Name = "";
      record.Number = number;
      record.Duration = duration;
      record.Type = type;
      record.Time = time;

      addRecord(record);
    }

  }
}
