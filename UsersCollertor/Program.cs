using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace UsersCollertor
{
    internal class Program
    {
        public static int Port = 0x0000270f; // = 9999

        public static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                int index = Array.IndexOf<string>(args, "-t");
                if (index != -1 && index + 1 < args.Length)
                    switch (args[index + 1])
                    {
                        case "1":
                        case "full": FullScan(); break;
                        case "2":
                        case "range": RangeScan(); break;
                        case "3":
                        case "broadcast":BroadCastScan();break;
                    }
                else
                {
                    index = Array.IndexOf<string>(args, "--type");
                    if (index != -1 && index + 1 < args.Length)
                        switch (args[index + 1])
                        {
                            case "1":
                            case "full": FullScan(); break;
                            case "2":
                            case "range": RangeScan(); break;
                            case "3":
                            case "broadcast":BroadCastScan();break;
                        }
                }
            }
            for (; ; )
            {
                Console.WriteLine("Choose the scan option: ");
                Console.WriteLine(" 1. full subnet scan (slow, but most reliable");
                Console.WriteLine(" 2. range scan");
                Console.WriteLine(" 3. broadcast scan (not so reliable (and easy to catch), but fast)");
                Console.Write("enter choice: ");
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        FullScan();
                        break;
                    case 2:
                        RangeScan();
                        break;
                    case 3:
                        BroadCastScan();
                        break;
                }
                Console.Write("finished");
                Console.ReadKey();
                Console.Clear();
            }
        }

        #region scan types
        private static void FullScan()
        {
            IPAddress[] all = GetAllLocalIP();
            IPAddress chosen;
            for (int i = 0; i < all.Length; i++)
            {
                chosen = all[i];
                if (chosen != null)
                    Console.WriteLine(" " + i + ": " + chosen.ToString() + " [" + GetSubnetMask(chosen) + "]");
            }
            Console.Write("choose the subnet: ");
            chosen = all[int.Parse(Console.ReadLine())];
            Console.WriteLine("\n\nPlease wait few secends until minute . . .");
            Console.WriteLine("Program scans for all network with open port {0} . . .\n\n", Port);
            GetAllHostsWithOpenPort(chosen);
        }
        private static void RangeScan()
        {
            Console.WriteLine("enter range in ip ([start ip]-[end ip])");
            string s = Console.ReadLine();

            Console.WriteLine("\n\nPlease wait few secends until minute . . .");
            Console.WriteLine("Program scans for all network with open port {0} . . .\n\n", Port);
            GetAllHostsWithOpenPort(IPAddress.Parse(s.Split('-')[0]), IPAddress.Parse(s.Split('-')[1]));
        }
        static void BroadCastScan()
        {
            IPAddress[] all = GetAllLocalIP();
            IPAddress chosen;
            for (int i = 0; i < all.Length; i++)
            {
                chosen = all[i];
                if (chosen != null)
                    Console.WriteLine(" " + i + ": " + chosen.ToString() + " [" + GetSubnetMask(chosen) + "]");
            }
            Console.Write("choose the subnet: ");
            chosen = all[int.Parse(Console.ReadLine())];
            Console.WriteLine("\n\nPlease wait 5 secends . . .");
            Console.WriteLine("Program scans using broadcast all network with open port {0} . . .\n\n", Port);
            BroadcastScan.Scan(chosen);
        }

        #endregion scan types

        #region

        public static IPAddress GetLocalIP()
        {
            string myHost = Dns.GetHostName();
            IPAddress myIP = IPAddress.None;
            for (int i = 0; i <= System.Net.Dns.GetHostEntry(myHost).AddressList.Length - 1; i++)
            {
                if (System.Net.Dns.GetHostEntry(myHost).AddressList[i].IsIPv6LinkLocal == false)
                {
                    myIP = Dns.GetHostEntry(myHost).AddressList[i];
                }
            }
            return myIP;
        }

        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public static IPAddress[] GetAllLocalIP()
        {
            string myHost = Dns.GetHostName();
            List<IPAddress> list = new List<IPAddress>();
            for (int i = 0; i <= System.Net.Dns.GetHostEntry(myHost).AddressList.Length - 1; i++)
            {
                if (System.Net.Dns.GetHostEntry(myHost).AddressList[i].IsIPv6LinkLocal == false)
                {
                    list.Add(Dns.GetHostEntry(myHost).AddressList[i]);
                }
            }
            return list.ToArray();
        }

        public static uint IPAddressToUInt(IPAddress ip)
        {
            byte[] list = ip.GetAddressBytes();
            Array.Reverse(list);
            return BitConverter.ToUInt32(list, 0);
        }

        #endregion

        #region

        public static IPAddress[] GetAllHostsWithOpenPort(IPAddress local)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;


            string title = Console.Title;
            uint subnet = IPAddressToUInt(GetSubnetMask(local));
            List<IPAddress> list = new List<IPAddress>();
            uint wildcard = subnet ^ 0xffffffff;
            uint subnetLocal = subnet & IPAddressToUInt(local);
            for (uint i = 0x2; i < wildcard; i++)
            {
                string[] temp = new IPAddress(i + subnetLocal).ToString().Split('.');
                string[] newTemp = { temp[3], temp[2], temp[1], temp[0] };
                if (newTemp[3] == "102")
                    if (true) { };
                IPAddress ip = IPAddress.Parse(string.Join(".", newTemp));
                Console.Title = ((double)i / wildcard * 100) + "% | " + ip.ToString();
                try
                {
                    TcpClient client = new TcpClient();
                    client.BeginConnect(ip, Port, new AsyncCallback(CallBack), client);
                }
                catch (SocketException) // no such host | port closed | port busy
                {
                    //Console.WriteLine("{0}:{1} is not ready for your commands", ip.ToString(), Port);
                }
                if ((i & 0x4fff) == 0x0)
                    System.Threading.Thread.Sleep(5000);
            }
            Console.Title = title; //0x4ff
            System.Threading.Thread.Sleep(5000);
            Console.ForegroundColor = c;
            return list.ToArray();
        }

        private static void CallBack(IAsyncResult result)
        {
            bool connected = false;
            string i = null;
            using (TcpClient client = (TcpClient)result.AsyncState)
            {
                try
                {
                    
                    client.EndConnect(result);
                    connected = client.Connected;
                    if (connected)
                        i = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                    client.Close();
                }
                catch (SocketException)
                {
                }
            }
            if (connected)
            {
                Console.WriteLine(" >> {0}:{1} is ready for your commands", i, Port);
            }
        }

        public static IPAddress[] GetAllHostsWithOpenPort(IPAddress start, IPAddress end)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;


            uint from = IPAddressToUInt(start);
            uint to = IPAddressToUInt(end);
            uint between = to - from;

            string title = Console.Title;
            List<IPAddress> list = new List<IPAddress>();

            for (uint i = from; i <= to; i++)
            {
                Console.Title = ((double)i / between * 100) + "% | " + i;
                string[] temp = new IPAddress(i).ToString().Split('.');
                string[] newTemp = { temp[3], temp[2], temp[1], temp[0] };
                IPAddress ip = IPAddress.Parse(string.Join(".", newTemp));
                try
                {
                    TcpClient client = new TcpClient();
                    client.BeginConnect(ip, Port, new AsyncCallback(CallBack), client);
                }
                catch (SocketException) // no such host | port closed | port busy
                {
                    //Console.WriteLine("{0}:{1} is not ready for your commands", ip.ToString(), Port);
                }
            }
            Console.Title = title;
            System.Threading.Thread.Sleep(5000);
            Console.ForegroundColor = c;
            return list.ToArray();
        }

        #endregion
    }
}