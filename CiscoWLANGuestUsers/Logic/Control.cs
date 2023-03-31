using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Management.Automation;
using System.IO;
using System.Diagnostics;
using ESCPOS_NET;

namespace CiscoWLANGuestUsers
{
    public class Control
    {
        public void RunScript(string IP, string Community, string UserName, string Password, string Description, int WLAN_ID, int LifeDays = 1)
        {
            string Lifetime = (LifeDays * 60 * 24).ToString();
            string command = "powershell.exe -c \".\\Powershell\\CiscoGuestWlan.ps1 -IP " + IP + " -Community \'" + Community + "\' -WLanID " + WLAN_ID + " -GuestUser \'" + UserName + "\' -GuestDesc \'" + Description + "\' -GuestLifeTimeMinute " + Lifetime + " -GuestPass \'" + Password + "\'\"";
            Process.Start("powershell.exe", "-c \".\\Powershell\\CiscoGuestWlan.ps1 -IP " + IP + " -Community \'" + Community + "\' -WLanID " + WLAN_ID + " -GuestUser \'" + UserName + "\' -GuestDesc \'" + Description + "\' -GuestLifeTimeMinute " + Lifetime + " -GuestPass \'" + Password + "\'\"");
            /*
            PowerShell ps = PowerShell.Create();
            ps.AddScript(File.ReadAllText("@Powershell\\CiscoGuestWlan.ps1"), true);
            ps.AddParameter("Community", Community);
            ps.AddParameter("IP", IP);
            ps.AddParameter("WLanID", WLAN_ID);
            ps.AddParameter("GuestUser", UserName);
            ps.AddParameter("GuestDesc", Description);
            ps.AddParameter("GuestLifeTimeMinute", LifeDays * 24 * 60);
            ps.AddParameter("GuestPass", Password);
            ps.Invoke<string>();
            */
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
                e.SetStyles(ESCPOS_NET.Emitters.PrintStyle.None),
                e.FullCut()
                )) ;
        }

        public ImmediateNetworkPrinter GetNetworkPrinter(string HostName_or_IP, int port = 9100)
        {
            return new ImmediateNetworkPrinter(new ImmediateNetworkPrinterSettings() { ConnectionString = $"{HostName_or_IP}:{port}", PrinterName = "EPSON TM-m30" });
        }
    }
}
