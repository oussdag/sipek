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

using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;


namespace Telephony
{
  public class CSipProxy : CTelephonyInterface
  {
    private static Thread dllThread; // thread which initializes pjsip.dll

    delegate int DoShutdownDelegate();
    
    //delegate int OnCallStateChanged(int account, string number);
    delegate int OnCallStateChanged(int account);

    [DllImport("pjsipDll.dll")]
    private static extern int dll_init();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_main();
    [DllImport("pjsipDll.dll")]
    private static extern int onCallStateCallback(OnCallStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_test();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_shutdown();

    [DllImport("pjsipDll.dll")]
    private static extern int dll_makeCall(int callId, string number);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_releaseCall(int callId);

    ///
    delegate int DoMakeCallDelegate(int callId, string number);
    delegate int DoReleaseCallDelegate(int callId);


    static Synchronizer m_Synchronizer;

    private static void dllLoad()
    {
      try
      {
        onCallStateCallback(new OnCallStateChanged(onCallStateChanged));

        int status = dll_init();
        //m_Synchronizer.Invoke(new DoShutdownDelegate(dll_init), null);

        if (status == 0)
        {
          status = dll_main(); // endless loop
        }
        //dll_test();
  
      }
      catch (DllNotFoundException dle) { System.Console.WriteLine(dle.Message); throw new Exception(); };
    }


    public static void initialize()
    {
      m_Synchronizer = new Synchronizer();
      m_Synchronizer.Invoke(new DoShutdownDelegate(dll_init), null);

      onCallStateCallback(new OnCallStateChanged(onCallStateChanged));

      // create listening socket server for receiving SIP events...
      //new Thread(new ThreadStart(socketHandler));

      // create dll thread
      try
      {
        //dllThread = new Thread(new ThreadStart(dllLoad));
        //dllThread.Start();
      }
      catch (Exception e)
      {
        System.Console.WriteLine(e.Message);
      }
    }


    //private static int onCallStateChanged(int account, string number)
    private static int onCallStateChanged(int account)
    {
      return 1;
    }


    #region Variables
    
    private int _line;

    #endregion Variables


    #region Constructor

    public CSipProxy(int line)
    {
      _line = line;
    }

    #endregion Constructor


    #region Methods

    public bool shutdown()
    {
      //doshutdown.BeginInvoke(3000,
      //dllThread.Invoke(doshutdown);
      m_Synchronizer.Invoke(new DoShutdownDelegate(dll_shutdown), null);
      m_Synchronizer.Dispose();
      return true;
    }

    public bool makeCall(string dialedNo)
    {
      object[] args = new object[2];
      args[0] = 1;
      args[1] = "1234";
      object res = m_Synchronizer.Invoke(new DoMakeCallDelegate( dll_makeCall), args);
      return true;
    }

    public bool endCall()
    {
      object[] args = new object[1];
      args[0] = 0;
      m_Synchronizer.Invoke(new DoReleaseCallDelegate( dll_releaseCall), args);
      return true;
    }

    public bool alerted()
    {
      return true;
    }

    #endregion Methods

  }




} // namespace Telephony
