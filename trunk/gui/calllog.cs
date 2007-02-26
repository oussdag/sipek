using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Gui
{
  public enum ECallType : int
  {
    EDialed,
    EReceived,
    EMissed
  }  
  
  public class CCallRecord
  { 

    
    private ECallType _type;
    private string _number;
    private DateTime _duration;
    private int _count;


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

        XmlNode root = _xmlDocument.CreateNode("element", "calllog", "");
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
        record.FirstName = item.ChildNodes[0].InnerText;
        record.LastName = item.ChildNodes[1].InnerText;

        XmlNode node = item.ChildNodes[2];
        record.Number = node.ChildNodes[0].InnerText;

        result.Add(record);
      }

      return result;
    }

    public void addRecord(CCallRecord record)
    {
      XmlNode nodeRecord = _xmlDocument.CreateNode("element", "Record", "");

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


    /////////////////////////////////////////////////////////////////////////////////////
    public void addCall(ECallType type, string number, DateTime time, int duration)
    {

    }

  }
}
