using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CiscoWLANGuestUsers
{
    public partial class Form1 : Form
    {
        Control crtl;
        public Form1()
        {
            InitializeComponent();
            crtl = new Control();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings s = new Settings(crtl);
            s.Show();
        }

        private void btGenAuto_Click(object sender, EventArgs e)
        {
            GenerateUser();
        }

        async void GenerateUser(string username = null, string PW = null, int LifeTime = 1)
        {
            Loading l = new Loading();
            l.Show();
            IProgress<string> progress = new Progress<string>(v =>
            {
                l.SetStatus(v);
            });

            await Task.Run(() => crtl.GenerateGuestUser(username, PW, LifeTime, progress));

            l.Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
                GenerateUser();
        }

        private void openCashDrawerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSettings userSettings = crtl.LoadSettings();
            crtl.OpenCashDrawer(crtl.GetNetworkPrinter(userSettings.PrinterAddress, userSettings.PrinterPort));
        }
    }
}
