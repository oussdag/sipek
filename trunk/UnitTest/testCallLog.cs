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
      Assert.Ignore();
    }

    [Test]
    public void testAddCallRecord()
    {
      Assert.Ignore();
    }

    [Test]
    public void testClearRecords()
    {
      Assert.Ignore();
    }

    [Test]
    public void testRemoveCallRecord()
    {
      Assert.Ignore();
    }

    [Test]
    public void testDuplicateRecord()
    {
      Assert.Ignore();
    }

    [Test]
    public void testAddRecordMax()
    {
      Assert.Ignore();
    }


  }

}
