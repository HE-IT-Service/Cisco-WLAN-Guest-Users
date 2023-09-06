using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using ESCPOS_NET;

namespace CiscoWLANGuestUsers
{
    public class Control
    {
        public void CreateWLCGuestUser(string IP, string Community, string UserName, string Password, string Description, int WLAN_ID, int LifeDays = 1)
        {
            string Lifetime = (LifeDays * 60 * 24).ToString();
            //string command = "powershell.exe -c \".\\Powershell\\CiscoGuestWlan.ps1 -IP " + IP + " -Community \'" + Community + "\' -WLanID " + WLAN_ID + " -GuestUser \'" + UserName + "\' -GuestDesc \'" + Description + "\' -GuestLifeTimeMinute " + Lifetime + " -GuestPass \'" + Password + "\'\"";
            Process.Start("powershell.exe", "-WindowStyle hidden -ExecutionPolicy Bypass -c \".\\Powershell\\CiscoGuestWlan.ps1 -IP " + IP + " -Community \'" + Community + "\' -WLanID " + WLAN_ID + " -GuestUser \'" + UserName + "\' -GuestDesc \'" + Description + "\' -GuestLifeTimeMinute " + Lifetime + " -GuestPass \'" + Password + "\'\"");
        }

        public void CreateShortcut()
        {
            Process.Start("powershell.exe", "-WindowStyle hidden -ExecutionPolicy Bypass -c \".\\Powershell\\CreateShortcut.ps1\"");
            //Process p2 = Process.Start("powershell.exe");
        }

        public async void PrintTicket(ImmediateNetworkPrinter NetworkPrinter, string Username, string Password, string WLANName)
        {
            var e = new ESCPOS_NET.Emitters.EPSON();
            string WLANCode = "WIFI:T:nopass;S:" + WLANName + ";P:;H:;;"
            await NetworkPrinter.WriteAsync(
             ESCPOS_NET.Utilities.ByteSplicer.Combine(
                e.SetBarWidth(ESCPOS_NET.Emitters.BarWidth.Default),
                e.CenterAlign(),
                e.PrintImage(File.ReadAllBytes("Images/HEITService.png"), true),
                e.PrintLine(""),
                //e.LeftAlign(),
                e.PrintLine(""),
                //e.PrintLine("1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijk"),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.DoubleHeight | ESCPOS_NET.Emitters.PrintStyle.DoubleWidth | ESCPOS_NET.Emitters.PrintStyle.Underline | ESCPOS_NET.Emitters.PrintStyle.Bold),
                e.PrintLine("Gast WLAN Zugang"),
                e.PrintLine(""),
                e.PrintLine(""),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.Bold),
                e.PrintLine("SSID:"),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.DoubleHeight | ESCPOS_NET.Emitters.PrintStyle.DoubleWidth),
                e.PrintLine(WLANName),
                e.PrintLine(""),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.Bold),
                e.PrintLine("Username:"),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.DoubleHeight | ESCPOS_NET.Emitters.PrintStyle.DoubleWidth),
                e.PrintLine(Username),
                e.PrintLine(""),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.Bold),
                e.PrintLine("Password:"),
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.DoubleHeight | ESCPOS_NET.Emitters.PrintStyle.DoubleWidth),
                e.PrintLine(Password),
                e.PrintLine(""),
                //e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.None),
                e.FullCutAfterFeed(5)
                )) ;
        }

        public async void OpenCashDrawer(ImmediateNetworkPrinter NetworkPrinter)
        {
            var e = new ESCPOS_NET.Emitters.EPSON();
            await NetworkPrinter.WriteAsync(
             ESCPOS_NET.Utilities.ByteSplicer.Combine(
                e.CashDrawerOpenPin2()
            ));
        }

        public ImmediateNetworkPrinter GetNetworkPrinter(string HostName_or_IP, int port = 9100)
        {
            return new ImmediateNetworkPrinter(new ImmediateNetworkPrinterSettings() { ConnectionString = $"{HostName_or_IP}:{port}", PrinterName = "EPSON TM-m30" });
        }

        public void SaveSettings(UserSettings userSettings)
        {
            General.Default.PrinterAddress = userSettings.PrinterAddress;
            General.Default.PrinterPort = userSettings.PrinterPort;
            General.Default.WLCAdresses = userSettings.WLCAddresses;
            General.Default.WLCCommunity = userSettings.Community;
            General.Default.Prefix = userSettings.Prefix;
            General.Default.WLANName = userSettings.WLANName;
            General.Default.Save();
        }

        public UserSettings LoadSettings()
        {
            UserSettings userSettings = new UserSettings();
            userSettings.PrinterAddress = General.Default.PrinterAddress;
            userSettings.PrinterPort = General.Default.PrinterPort;
            userSettings.WLCAddresses = General.Default.WLCAdresses;
            userSettings.Community = General.Default.WLCCommunity;
            userSettings.Prefix = General.Default.Prefix;
            userSettings.WLANName = General.Default.WLANName;
            return userSettings;
        }

        public void GenerateGuestUser(string Username = null, string PW = null, int LifeTime = 1, IProgress<string> progress = null, bool print = true)
        {
            Console.WriteLine("Load User Settings");
            UserSettings userSettings = LoadSettings();
            Console.WriteLine("Printer Address: " + userSettings.PrinterAddress);

            if (progress != null)
                progress.Report("Create New User ...");
            if (Username == null)
            {
                Username = userSettings.Prefix + "_" + General.Default.UserCounter.ToString();
                General.Default.UserCounter++;
                General.Default.Save();
            }
            if (PW == null)
            {
                PW = CreatePassword(8);
            }
            Console.WriteLine("Username: " + Username);
            Console.WriteLine("PW: " + PW);
            Console.WriteLine("Add User to WLCs");
            foreach (WLCController wlc in userSettings.WLCControllers)
            {
                if (progress != null)
                    progress.Report("Create Guest User on " + wlc.Address);
                CreateWLCGuestUser(wlc.Address, userSettings.Community, Username, PW, "Created by SNMP", wlc.WLAN_ID, LifeTime);
                System.Threading.Thread.Sleep(5000);
            }
            if (print)
            {
                Console.WriteLine("Print User Data");
                if (progress != null)
                    progress.Report("User: " + Username + ", PW: " + PW + " created. Printing ...");
                PrintTicket(GetNetworkPrinter(userSettings.PrinterAddress, userSettings.PrinterPort), Username, PW, userSettings.WLANName);
            }
            if (progress != null)
            {
                progress.Report("User: " + Username + ", PW: " + PW + " created. Finished.");
                System.Threading.Thread.Sleep(5000);
            }
            Console.WriteLine("Finished");
            return;
        }

        string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
