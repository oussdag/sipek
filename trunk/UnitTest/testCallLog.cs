using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Telephony;

namespace UnitTest
{

  [TestFixture]
  public class testCallLog
  {
    [Test]
    public void testInit()
    {
      CCallLog.getInstance().load();
      Stack<CCallRecord> list = CCallLog.getInstance().getList();
      Assert.AreEqual(0, list.Count);
    }

    [Test]
    public void testCheckRecordContent()
    {
      CCallLog.getInstance().load();
      Stack<CCallRecord> list = CCallLog.getInstance().getList();
      Assert.AreEqual(0, list.Count);
      CCallRecord rec = new CCallRecord();
      rec.Count = 1;
      rec.Duration = new TimeSpan(0,0,4);
      rec.Name = "test";
      rec.Number = "1234";
      rec.Time = new DateTime(2007,7,20,11,50,45);
      rec.Type = ECallType.EDialed;
      list.Push(rec);
      Assert.AreEqual(1, list.Count);
      
      CCallRecord realrec = list.Peek();
      Assert.AreEqual(1, realrec.Count);
      Assert.AreEqual(4, realrec.Duration.Seconds);
      Assert.AreEqual("test", realrec.Name);
      Assert.AreEqual("1234", realrec.Number);
      Assert.AreEqual(2007, realrec.Time.Year);
      Assert.AreEqual(7,realrec.Time.Month);
      Assert.AreEqual(20, realrec.Time.Day);
      Assert.AreEqual(11,realrec.Time.Hour);
      Assert.AreEqual(50, realrec.Time.Minute);
      Assert.AreEqual(45, realrec.Time.Second);
      Assert.AreEqual(ECallType.EDialed, realrec.Type);
    }

    [Test]
    public void testAddCallRecords()
    {
      CCallLog.getInstance().load();
      Stack<CCallRecord> list = CCallLog.getInstance().getList();
      Assert.AreEqual(0, list.Count);

      CCallRecord rec = new CCallRecord();
      rec.Count = 1;
      rec.Duration = new TimeSpan(0, 0, 4);
      rec.Name = "test";
      rec.Number = "1234";
      rec.Time = new DateTime(2007, 7, 20, 11, 50, 45);
      rec.Type = ECallType.EDialed;

      CCallLog.getInstance().addCall(rec.Type,rec.Number,rec.Time,rec.Duration);
      Assert.AreEqual(1, list.Count);
    }

    [Test]
    public void testClearRecords()
    {
      testAddCallRecords();
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());

      // only two entries because of same type and number
      Assert.AreEqual(2, CCallLog.getInstance().Count);
      
      CCallLog.getInstance().clearAll();

      Assert.AreEqual(0, CCallLog.getInstance().Count);
    }

    [Test]
    public void testRemoveCallRecord()
    {
      testAddCallRecords();
      Assert.AreEqual(1, CCallLog.getInstance().Count);
      CCallLog.getInstance().deleteRecord("1234", ECallType.EDialed);
      Assert.AreEqual(0, CCallLog.getInstance().Count);
    }

    [Test]
    public void testDuplicateRecord()
    {
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      CCallLog.getInstance().addCall(ECallType.EMissed, "1111", new DateTime(), new TimeSpan());
      Assert.AreEqual(1, CCallLog.getInstance().Count);



    }

    [Test]
    public void testAddRecordMax()
    {
      Assert.Ignore();
    }


  }

}
