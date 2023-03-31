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

        async void GenerateUser(string username = null, string PW = null, int LifeTime = 1, bool print = true)
        {
            Loading l = new Loading();
            l.Show();
            IProgress<string> progress = new Progress<string>(v =>
            {
                l.SetStatus(v);
            });

            await Task.Run(() => crtl.GenerateGuestUser(username, PW, LifeTime, progress, print));

            l.Close();

            tbUser.Text = "";
            tbPW.Text = "";
            tbLifetime.Text = "";
            tbUser.Focus();
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

        private void btCreate_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateUser(tbUser.Text, tbPW.Text, Convert.ToInt16(tbLifetime.Text), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbLifetime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            {
                try
                {
                    GenerateUser(tbUser.Text, tbPW.Text, Convert.ToInt16(tbLifetime.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    GenerateUser(tbUser.Text, tbPW.Text, Convert.ToInt16(tbLifetime.Text), false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btCreatePrint_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateUser(tbUser.Text, tbPW.Text, Convert.ToInt16(tbLifetime.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void createShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            crtl.CreateShortcut();
        }
    }
}
