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
            Process.Start("powershell.exe", "-c \".\\Powershell\\CiscoGuestWlan.ps1 -IP " + IP + " -Community \'" + Community + "\' -WLanID " + WLAN_ID + " -GuestUser \'" + UserName + "\' -GuestDesc \'" + Description + "\' -GuestLifeTimeMinute " + Lifetime + " -GuestPass \'" + Password + "\'\"");
        }

        public async void PrintTicket(ImmediateNetworkPrinter NetworkPrinter, string Username, string Password)
        {
            var e = new ESCPOS_NET.Emitters.EPSON();
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
                e.PrintLine("HE IT-Service Guest"),
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
            return userSettings;
        }

        public void GenerateGuestUser(string Username = null, string PW = null, int LifeTime = 1, IProgress<string> progress = null)
        {
            UserSettings userSettings = LoadSettings();

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

            foreach (WLCController wlc in userSettings.WLCControllers)
            {
                if (progress != null)
                    progress.Report("Create Guest User on " + wlc.Address);
                CreateWLCGuestUser(wlc.Address, userSettings.Community, Username, PW, "Created by SNMP", wlc.WLAN_ID, LifeTime);
                System.Threading.Thread.Sleep(5000);
            }
            if (progress != null)
                progress.Report("User: " + Username + ", PW: " + PW + " created. Printing ...");
            PrintTicket(GetNetworkPrinter(userSettings.PrinterAddress, userSettings.PrinterPort), Username, PW);
            if (progress != null)
            {
                progress.Report("User: " + Username + ", PW: " + PW + " created. Finished.");
                System.Threading.Thread.Sleep(5000);
            }
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
