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
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        public async void CreateUser(Control c, string username = null, string PW = null, int LifeTime = 1)
        {
            IProgress<string> progress = new Progress<string>(v =>
            {
                lbStatus.Text = v;
            });

            await Task.Run(() => c.GenerateGuestUser(username, PW, LifeTime, progress));

            this.Close();
        }

        public void SetStatus(string Status)
        {
            lbStatus.Text = Status;
        }
    }
}
