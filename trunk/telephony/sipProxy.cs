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
    
    
    
    delegate int OnCallStateChanged(int account, string number);

    [DllImport("pjsipDll.dll")]
    private static extern int dll_init();
    [DllImport("pjsipDll.dll")]
    private static extern int dll_main();
    [DllImport("pjsipDll.dll")]
    private static extern int onCallStateCallback(OnCallStateChanged cb);
    [DllImport("pjsipDll.dll")]
    private static extern int dll_test();

    private static void dllLoad()
    {
      try
      {
        onCallStateCallback(new OnCallStateChanged(onCallStateChanged));

        int status = dll_init();

        if (status == 0)
        {
          dll_main(); // endless loop
        }
        //dll_test();
  
      }
      catch (DllNotFoundException dle) { System.Console.WriteLine(dle.Message); throw new Exception(); };
    }


    public static void initialize()
    {
      // create listening socket server for receiving SIP events...
      //new Thread(new ThreadStart(socketHandler));

      // create dll thread
      try
      {
        dllThread = new Thread(new ThreadStart(dllLoad));
        dllThread.Start();
      }
      catch (Exception e)
      {
        System.Console.WriteLine(e.Message);
      }
    }


    private static int onCallStateChanged(int account, string number)
    {
      return 1;
    }


    public static void socketHandler()
    {
      TcpListener server = null;   
      try
      {
        // Set the TcpListener on port 13000.
        Int32 port = 30000;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        
        // TcpListener server = new TcpListener(port);
        server = new TcpListener(localAddr, port);

        // Start listening for client requests.
        server.Start();
           
        // Buffer for reading data
        Byte[] bytes = new Byte[256];
        String data = null;

        // Enter the listening loop.
        while(true) 
        {
          Console.Write("Waiting for a connection... ");
          
          // Perform a blocking call to accept requests.
          // You could also user server.AcceptSocket() here.
          TcpClient client = server.AcceptTcpClient();            
          Console.WriteLine("Connected!");

          data = null;

          // Get a stream object for reading and writing
          NetworkStream stream = client.GetStream();

          int i;

          // Loop to receive all the data sent by the client.
          while((i = stream.Read(bytes, 0, bytes.Length))!=0) 
          {   
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Received: {0}", data);
         
            // Process the data sent by the client.
            data = data.ToUpper();

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

            // Send back a response.
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", data);            
          }
           
          // Shutdown and end connection
          client.Close();
        }
      }
      catch(SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e);
      }
      finally
      {
         // Stop listening for new clients.
         server.Stop();
      }

      Console.WriteLine("\nHit enter to continue...");
      Console.Read();
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

    public bool makeCall(string dialedNo)
    {
      return true;
    }

    public bool endCall()
    {
      return true;
    }

    public bool alerted()
    {
      return true;
    }

    #endregion Methods

  }




} // namespace Telephony
