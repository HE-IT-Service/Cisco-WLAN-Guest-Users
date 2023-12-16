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
    public partial class Settings : Form
    {
        Control crtl;
        public Settings(Control c)
        {
            InitializeComponent();
            crtl = c;
            UserSettings userSettings = crtl.LoadSettings();
            tbCommunity.Text = userSettings.Community;
            tbPrinterAddress.Text = userSettings.PrinterAddress;
            tbPrinterPort.Text = userSettings.PrinterPort.ToString();
            tbWLCAddresses.Text = userSettings.WLCAddresses;
            tbPrefix.Text = userSettings.Prefix;
            tbWLANName.Text = userSettings.WLANName;
            LoadPicture();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            try
            {
                UserSettings userSettings = new UserSettings();
                userSettings.PrinterPort = Convert.ToInt16(tbPrinterPort.Text);
                userSettings.PrinterAddress = tbPrinterAddress.Text;
                userSettings.Community = tbCommunity.Text;
                userSettings.WLCAddresses = tbWLCAddresses.Text;
                userSettings.Prefix = tbPrefix.Text;
                userSettings.WLANName = tbWLANName.Text;
                crtl.SaveSettings(userSettings);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadPicture()
        {
            Image logo = Image.FromFile(@"Images\logo.png");
            Bitmap logobmp = new Bitmap(logo);
            logoPreview.Image = logobmp;
        }

        private void changeLogo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileOk += (object senderinfo, CancelEventArgs eventargs) =>
            {
                crtl.SaveNewLogo(ofd.FileName);
                LoadPicture();
            };
            //ofd.Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff";
            ofd.Filter = "PNG|*.png|All Files|*.*";
            ofd.ShowDialog();
        }
    }
}
