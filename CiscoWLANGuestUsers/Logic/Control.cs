using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;

namespace CiscoWLANGuestUsers
{
    public class Control
    {
        public void RunScript(string IP, string Community, string UserName, string Password, string Description, int WLAN_ID, int LifeDays = 1)
        {
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
        }
    }
}
