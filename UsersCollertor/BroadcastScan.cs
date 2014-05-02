using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace UsersCollertor
{
    static class BroadcastScan
    {
        const int BroadcastRecievePort = 0x235B; //=9051
        const int BroadcastSendPort = 0x235A; //=9050
        private static bool ServerRun;

        public static void Scan(IPAddress local)
        {
            
            byte[] data = System.Text.Encoding.ASCII.GetBytes(local.ToString());

            IPAddress subnet = Program.GetSubnetMask(local);
            uint wildcard = Program.IPAddressToUInt(subnet) ^ 0xffffffff;
            string[] temp = new IPAddress(wildcard + (Program.IPAddressToUInt(subnet) & Program.IPAddressToUInt(local))).ToString().Split('.');
            string[] newTemp = { temp[3], temp[2], temp[1], temp[0] };
            IPAddress broadCast = IPAddress.Parse(string.Join(".", newTemp));
            ServerRun = true;
            // start server
            Thread thread_task = new Thread(new ThreadStart(Server));
            thread_task.Start();

            Socket sock;
            IPEndPoint iep1, iep2;

            for (int i = 0; i < 10; i++)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                iep1 = new IPEndPoint(IPAddress.Broadcast, BroadcastSendPort);
                iep2 = new IPEndPoint(broadCast, BroadcastSendPort);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                try { sock.SendTo(data, iep1); }
                catch { }
                try { sock.SendTo(data, iep2); }
                catch { }
                sock.Close();
                Thread.Sleep(500);
            }
            ServerRun = false;
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(" >> {0}:{1} is ready for your commands", list[i], Program.Port);
            }
            Console.ForegroundColor = c;
            list.Clear();
        }
        static List<IPAddress> list;
        private static void Server()
        {
            list = new List<IPAddress>();
            const ushort data_size = 0x400; // = 1024
            byte[] data;
            while (ServerRun)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork,
                          SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, BroadcastRecievePort);
                try
                {
                    sock.Bind(iep);
                    EndPoint ep = (EndPoint)iep;

                    data = new byte[data_size];
                    if (!ServerRun) break;
                    int recv = sock.ReceiveFrom(data, ref ep);
                    string stringData = System.Text.Encoding.ASCII.GetString(data, 0, recv);
                    if (!list.Contains(IPAddress.Parse(ep.ToString().Split(':')[0])))
                        list.Add(IPAddress.Parse(ep.ToString().Split(':')[0]));

                    data = new byte[data_size];
                    if (!ServerRun) break;
                    recv = sock.ReceiveFrom(data, ref ep);
                    stringData = System.Text.Encoding.ASCII.GetString(data, 0, recv);
                    if (!list.Contains(IPAddress.Parse(ep.ToString().Split(':')[0])))
                        list.Add(IPAddress.Parse(ep.ToString().Split(':')[0]));

                    sock.Close();
                }
                catch { }
            }

            
        }
    }
}
