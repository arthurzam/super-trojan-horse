using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SuperTroyanHourse
{
    /// <remarks>
    /// http://www.java2s.com/Code/CSharp/Network/ReceiveBroadcast.htm
    /// http://www.java2s.com/Code/CSharp/Network/BroadcstSample.htm
    /// </remarks>
    static class BroadCastWaiter
    {
        const int BroadcastRecievePort = 9050;
        const int BroadcastSendPort = 9051;
        //static string broadCast = string.Empty;

        public static void StartBroadcastServer()
        {
            for (; Program.isProgramAlive ; )
            {
                Socket sock = new Socket(AddressFamily.InterNetwork,
                      SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, BroadcastRecievePort);
                sock.Bind(iep);
                EndPoint ep = (EndPoint)iep;
                Console.WriteLine("Ready to receive...");

                byte[] data = new byte[1024];
                int recv = sock.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("received: {0}  from: {1}",
                                      stringData, ep.ToString());
                sendBroadcastAnswer();

                data = new byte[1024];
                recv = sock.ReceiveFrom(data, ref ep);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("received: {0}  from: {1}",
                                      stringData, ep.ToString());
                sendBroadcastAnswer();
                sock.Close();
            }
        }

        private static void sendAnswer(EndPoint ep)
        {
            try
            {
                string ipStr = ep.ToString().Split(':')[0];
                TcpClient client = new TcpClient(ipStr, 9999);
                if (client.Connected)
                {
                    Socket s = client.Client;
                    s.Send(Encoding.ASCII.GetBytes(ipStr));
                    s.Close();
                }
                client.Close();
            }
            catch(Exception ex) { Console.WriteLine("error with {0}\n{1}", ep.ToString().Split(':')[0], ex.ToString()); }
        }

        static void sendBroadcastAnswer()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep1 = new IPEndPoint(IPAddress.Broadcast, 9050);
            IPEndPoint iep2 = new IPEndPoint(Internet.GetMaxBroadcast(), BroadcastSendPort);

            byte[] data = Encoding.ASCII.GetBytes(Internet.GetLocalIP().ToString());

            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            try { sock.SendTo(data, iep1); }
            catch { }
            try { sock.SendTo(data, iep2); }
            catch { }
            sock.Close();
        }
    }
}