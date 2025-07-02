using System;
using System.Collections.Generic;
using System.Management;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using BEAUTYLIFE;

namespace RandevuSistemi
{
    public partial class GirisSayfasi : Form
    {
        public GirisSayfasi()
        {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
           Lisans.MUSCLEMAN_Lisanslama();
        }

        private void RandevuIslem_Click(object sender, EventArgs e)
		{
            RandevuOlustur randevuOlustur = new RandevuOlustur();
            randevuOlustur.Show();
            this.Hide();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void FaturaIslemleri_Click(object sender, EventArgs e)
        {
            FaturaIslemleri faturaIslem = new FaturaIslemleri();
            faturaIslem.Show();
            this.Hide();
        }

        private void MusteriIslemleri_Click(object sender, EventArgs e)
        {
            MusteriIslemleri musteriIslem = new MusteriIslemleri();
            musteriIslem.Show();
            this.Hide();
        }


        private void GecmisRandevular_Click(object sender, EventArgs e)
        {
            Randevular gecmisRandevular = new Randevular();
            gecmisRandevular.Show();
            this.Hide();
        }
    

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            Yedekleme yedekleme = new Yedekleme();
            yedekleme.Show();
            this.Hide();
        }

    }
}

