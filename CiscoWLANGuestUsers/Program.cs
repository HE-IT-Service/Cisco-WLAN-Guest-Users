using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CiscoWLANGuestUsers
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else if (CheckArg(args, "auto"))
            {
                //GenerateGuestUser();
                Control control = new Control();
                control.GenerateGuestUser();
            }
        }

        static bool CheckArg(string[] args, string arg)
        {
            if (args.Contains("-" + arg) || args.Contains("/" + arg) || args.Contains(arg))
                return true;
            return false;
        }

        static async void GenerateGuestUser()
        {
            Control control = new Control();
            await Task.Run(() => control.GenerateGuestUser());
        }
    }
}
