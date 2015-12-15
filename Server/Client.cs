using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkProgramming
{
    class Client
    {
        private static String nick;
        private static List<EndPoint> serversList;
        private static Socket s;

        static void UdpSend()
        {
            IPEndPoint send = new IPEndPoint(IPAddress.Parse("224.1.1.1"), 7);
            s.SendTo(Encoding.ASCII.GetBytes("DISCOVER"), send);
            Console.WriteLine("Discover message sent to the broadcast address");
        }

        static bool UdpReceive()
        {
             EndPoint recv = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
            // s.ReceiveTimeout = 10000;

             byte[] buff = new byte[20];

             Console.WriteLine("Waiting for offer...");
             try
             {
                 s.ReceiveFrom(buff, ref recv);
                 String msg = Encoding.ASCII.GetString(buff).TrimEnd('\0');
                 if (msg.Equals("OFFER"))
                 {
                     Console.WriteLine("Received offer from {0}", recv.ToString());
                     serversList.Add(recv);
                 }
             }
             catch (SocketException e)
             {
                 if (e.SocketErrorCode == SocketError.TimedOut)
                 {
                     Console.WriteLine("No server was found. Trying once again.");
                     return false;
                 }
                 else
                     return false;
             }
             finally
             {
                 s.Close();
             }
            return true;
        }

        private static EndPoint selectServer()
        {
            EndPoint server = null;
            Console.WriteLine("\nAvaliable servers list:");
            for (int i = 0; i < serversList.Count; i++)
            {
                Console.WriteLine("{0}. IP address and port: {1}", i + 1, serversList.ElementAt(i).ToString());
            }
            Console.WriteLine("\nEnter number from 1 to {0} of server you want to select", serversList.Count);
            String value = null;
            bool flag = false;
            while (!flag)
            {
                value = Console.ReadLine();
                try
                {
                    int number = Convert.ToInt32(value);

                    if (number > serversList.Count || number < 1)
                        Console.WriteLine("Wrong number, try again");
                    else
                    {
                        flag = true;
                        server = serversList.ElementAt(number - 1);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("It's not a number, try again");
                }
            }
            return server;
        }

  

        static void Main(string[] args)
        {
            serversList = new List<EndPoint>();
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
          //  s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 7);
           // s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
          //  s.Bind(endPoint);

            

            Console.WriteLine("Enter your nick:");
            nick = Console.ReadLine();

            bool serverFound = false;

            while (!serverFound)
            {
                UdpSend();
                serverFound = UdpReceive();
            }

            EndPoint server = selectServer();


            /*  while (!done)
            {
                try
                {
                    Console.WriteLine("Waiting for offer...");
                    if (s.Available != 0)
                    {
                        s.ReceiveFrom(buff, ref recv);
                        String msg = Encoding.ASCII.GetString(buff).TrimEnd('\0');
                        if (msg.Equals("OFFER"))
                        {
                            Console.WriteLine("Received offer from {0}\n", recv.ToString());
                            done = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("No server was found");
                    done = true;
                }
            }*/



            Console.ReadKey();
        }
    }
}
