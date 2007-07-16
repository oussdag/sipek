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
    private const string NAME = "Name";
    private const string NUMBER = "Number";
    private const string DATETIME = "Time";
    private const string TYPE = "Type";
    private const string DURATION = "Duration";
    private const string COUNT = "Count";

    private static CCallLog _instance = null;
    
    private string XMLCallLogFile = "calllog.xml";

    private Stack<CCallRecord> _callList;

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
    }
    public void load()
    {
      this.load(XMLCallLogFile);
    }

    public void load(string fileName)
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load(fileName);
      }
      catch (System.IO.FileNotFoundException ee)
      {
        System.Console.WriteLine(ee.Message);

        XmlNode root = xmlDocument.CreateNode("element", "Calllog", "");
        xmlDocument.AppendChild(root);

      }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }

      XmlNodeList list = xmlDocument.SelectNodes("/Calllog/Record");

      // create list 
      _callList = new Stack<CCallRecord>();
      foreach (XmlNode item in list)
      {
        CCallRecord record = new CCallRecord();

        XmlNode snode = item.SelectSingleNode(NAME);
        if ((snode != null) && (null != snode.FirstChild) &&(snode.FirstChild.Value != null)) record.Name = snode.FirstChild.Value;

        snode = item.SelectSingleNode(NUMBER);
        if ((snode != null) && (snode.FirstChild != null) && (snode.FirstChild.Value != null)) record.Number = snode.FirstChild.Value;

        snode = item.SelectSingleNode(DATETIME);
        if ((snode != null) && (snode.FirstChild != null) && (snode.FirstChild.Value != null)) record.Time = DateTime.Parse(snode.FirstChild.Value);

        snode = item.SelectSingleNode(DURATION);
        if ((snode != null) && (snode.FirstChild != null) && (snode.FirstChild.Value != null)) record.Duration = TimeSpan.Parse(snode.FirstChild.Value);

        snode = item.SelectSingleNode(COUNT);
        if ((snode != null) && (snode.FirstChild != null) && (snode.FirstChild.Value != null)) record.Count = int.Parse(snode.FirstChild.Value);

        snode = item.SelectSingleNode(TYPE);
        if ((snode != null) && (snode.FirstChild != null) && (snode.FirstChild.Value != null)) record.Type = (ECallType)int.Parse(snode.FirstChild.Value);

        _callList.Push(record);
      }

    }

    public void save()
    {
      CCallRecord[] tmplist = _callList.ToArray();
      List<CCallRecord> list = new List<CCallRecord>(tmplist);
      list.Reverse();

      try
      {
        // serialize data
        XmlDocument xmldoc = new XmlDocument();

        XmlNode nodeRoot = xmldoc.CreateNode("element", "Calllog", "");

        foreach (CCallRecord item in list)
        {
          XmlNode nodeRecord = xmldoc.CreateNode("element", "Record", "");

          XmlElement elname = xmldoc.CreateElement(NAME);
          elname.InnerText = item.Name;
          nodeRecord.AppendChild(elname);

          XmlElement elnumber = xmldoc.CreateElement(NUMBER);
          elnumber.InnerText = item.Number;
          nodeRecord.AppendChild(elnumber);

          XmlElement eldur = xmldoc.CreateElement(DURATION);
          eldur.InnerText = item.Duration.ToString();
          nodeRecord.AppendChild(eldur);
        
          XmlElement eltime = xmldoc.CreateElement(DATETIME);
          eltime.InnerText = item.Time.ToString();
          nodeRecord.AppendChild(eltime);

          XmlElement elcount = xmldoc.CreateElement(COUNT);
          elcount.InnerText = item.Count.ToString();
          nodeRecord.AppendChild(elcount);

          XmlElement eltype = xmldoc.CreateElement(TYPE);
          eltype.InnerText = ((int)item.Type).ToString();
          nodeRecord.AppendChild(eltype);

          // add to xml
          nodeRoot.AppendChild(nodeRecord);
        }
        xmldoc.AppendChild(nodeRoot);

        xmldoc.Save(XMLCallLogFile);
      }
      catch (System.IO.FileNotFoundException ee) { System.Console.WriteLine(ee.Message); }
      catch (System.Xml.XmlException e) { System.Console.WriteLine(e.Message); }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////
    public Stack<CCallRecord> getList(ECallType type)
    {
      // todo!!!
      return _callList;
    }

    public Stack<CCallRecord> getList()
    {
      return _callList;
    }

    private CCallRecord findRecord(string number, ECallType type)
    {
      foreach (CCallRecord item in _callList)
      {
        if ((item.Number == number)&&(item.Type == type))
        {
          return item;
        }
      }
      return null;
    }

    protected void addRecord(CCallRecord record)
    {
      CCallRecord calllogItem = findRecord(record.Number, record.Type);
      if ((null == calllogItem)||(record.Type != calllogItem.Type))
      {
        _callList.Push(record);
      }
      else 
      {
        deleteRecord(record);
        record.Count = calllogItem.Count + 1;
        _callList.Push(record);
      }
    }

    public void deleteRecord(CCallRecord record)
    {
      this.deleteRecord(record.Number, record.Type);
    }

    public void deleteRecord(string number, ECallType type)
    {
      CCallRecord[] tmplist = _callList.ToArray();
      List<CCallRecord> list = new List<CCallRecord>(tmplist);
      list.Reverse();
      _callList.Clear();

      foreach (CCallRecord item in list)
      {
        if ((item.Number == number) && (item.Type == type))
        {
          continue;
        }
        _callList.Push(item);
      }
    }

    public void clearAll()
    {
      _callList.Clear();
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
