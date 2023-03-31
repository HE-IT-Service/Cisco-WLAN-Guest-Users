using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoWLANGuestUsers
{
    public class UserSettings
    {
        public string PrinterAddress { get; set; }
        public int PrinterPort { get; set; }
        public string Community { get; set; }
        string WLCAddressString;
        public string Prefix { get; set; }
        public string WLCAddresses
        {
            get
            {
                return WLCAddressString;
            }
            set
            {
                WLCControllers = new List<WLCController>();
                WLCAddressString = value;
                string[] servers = WLCAddressString.Split(';');
                foreach (string server in servers)
                {
                    string[] controller = server.Trim().Split(',');
                    if (controller[0].Length > 0)
                    {
                        WLCControllers.Add(new WLCController()
                        {
                            Address = controller[0].Trim(),
                            WLAN_ID = Convert.ToInt16(controller[1].Trim())
                        });
                    }
                }
            }
        }
        public List<WLCController> WLCControllers { get; private set; }
    }

    public class WLCController
    {
        public string Address { get; set; }
        public int WLAN_ID { get; set; }
    }
}
