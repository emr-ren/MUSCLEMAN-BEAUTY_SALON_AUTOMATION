using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandevuSistemi
{
    public partial class Logo : Form
    {
        public Logo()
        {
            InitializeComponent();
        }
        private void Logo_Load(object sender, EventArgs e)
        {

        }

        bool logo =false;
        private void LogoTimer_Tick(object sender, EventArgs e)
        {
            if (!logo)
            {
                this.Opacity += 0.009;
            }
            if (this.Opacity==1.0) 
            {
                logo = true;            
            }
            if (logo)
            {
                this.Opacity -= 0.009;
                if (this.Opacity==0)
                {
                    this.Opacity = 0;
                    timer1.Enabled = false;
                    this.Close(); // Formu kapat
                    GirisSayfasi girisSayfasi = new GirisSayfasi();
                    girisSayfasi.Show(); // Giriş sayfasını aç
                }
            }

        }

    }
}
