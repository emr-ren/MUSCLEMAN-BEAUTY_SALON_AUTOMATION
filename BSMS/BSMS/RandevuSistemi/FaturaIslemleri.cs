using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Globalization;

namespace RandevuSistemi
{
    public partial class FaturaIslemleri : Form
    {
        public FaturaIslemleri()
        {
            InitializeComponent();
        }

        string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
        void MusteriGetir()
        {
            CultureInfo ci = new CultureInfo("tr-TR");

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("AdColumn", "Ad");
            dataGridView1.Columns.Add("SoyadColumn", "Soyad");
            dataGridView1.Columns.Add("Odenen", "Ödenen Tutar");
            dataGridView1.Columns.Add("Odenecek", "Ödenecek Tutar");
            dataGridView1.Columns.Add("FaturaTarih", "En Son Yapılan Ödeme Tarihi");

            string query = @"SELECT M.MusteriID, M.Ad, M.Soyad,
       SUM(F.Odenen) AS Odenen,
       (SELECT TOP 1 F.Odenecek FROM Fatura F WHERE F.MusteriFaturaID = M.MusteriID ORDER BY F.FaturaID DESC) AS SonOdenecek,
       MAX(F.FaturaTarih) AS SonFaturaTarihi
    FROM Musteri M
    LEFT JOIN Fatura F ON M.MusteriID = F.MusteriFaturaID
    GROUP BY M.MusteriID, M.Ad, M.Soyad;";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var ad = reader["Ad"]?.ToString() ?? "";
                        var soyad = reader["Soyad"]?.ToString() ?? "";
                        var odenen = reader["Odenen"] != DBNull.Value ? Convert.ToDecimal(reader["Odenen"]) : 0;
                        var odenecekToplam = reader["SonOdenecek"] != DBNull.Value ? Convert.ToDecimal(reader["SonOdenecek"]).ToString("C2", ci) : "0.00 ₺";
                        var sonFaturaTarihi = reader["SonFaturaTarihi"] != DBNull.Value ? Convert.ToDateTime(reader["SonFaturaTarihi"]).ToString("dd/MM/yyyy") : "-";
                        var odenenStr = odenen < 0 ? odenen.ToString("C2", ci).Insert(1, " ") :  odenen.ToString("C2", ci).Insert(1, " ");
                        dataGridView1.Rows.Add(ad, soyad, odenenStr, odenecekToplam, sonFaturaTarihi);
                    }
                }
            }
        }



        private void FaturaIslemleri_Load(object sender, EventArgs e)
        {
            this.TopMost = false; // alt tab engelle
            MusteriGetir();
            dataGridView1.ClearSelection();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void PictureBox3_Click_1(object sender, EventArgs e)
        {
            GirisSayfasi GirisSayfasi = new GirisSayfasi();
            GirisSayfasi.Show();
            this.Hide();
        }



        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Seçilen satırın diğer verilerini TextBox'lara aktar

            txtMusteriAdi.Text = dataGridView1.CurrentRow.Cells["AdColumn"].Value.ToString();
            txtMusteriSoyadi.Text = dataGridView1.CurrentRow.Cells["SoyadColumn"].Value.ToString();
            //txtMusteriOdenenTutari.Text = dataGridView1.CurrentRow.Cells["Odenen"].Value.ToString();    Fatura işlemlerinde text boxu doldurmasın
            txtMusteriOdenecekTutari.Text = dataGridView1.CurrentRow.Cells["Odenecek"].Value.ToString();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // DataGridView yüklediğinizde veya verileri güncellediğinizde
            // varsayılan olarak seçili olan satırın seçimini temizleyin
        }



        private void OdemeKayitBtn_Click(object sender, EventArgs e)
        {
            // Ödeme tutarının girilip girilmediğini kontrol et
            if (string.IsNullOrWhiteSpace(txtMusteriOdenenTutari.Text))
            {
                MessageBox.Show("Lütfen ödeme tutarını giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Ödeme tutarı girilmediyse işlemi durdur
            }

            try
            {
                using (SqlConnection baglanti = new SqlConnection(conString))
                {
                    baglanti.Open();
                    // Müşteri ID ve mevcut Odenecek değerini almak için sorgu
                    SqlCommand command = new SqlCommand("SELECT MusteriID, (SELECT TOP 1 Odenecek FROM Fatura WHERE MusteriFaturaID = Musteri.MusteriID ORDER BY FaturaID DESC) AS Odenecek FROM Musteri WHERE Ad = @Ad AND Soyad = @Soyad", baglanti);
                    command.Parameters.AddWithValue("@Ad", txtMusteriAdi.Text);
                    command.Parameters.AddWithValue("@Soyad", txtMusteriSoyadi.Text);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int customerId = reader.GetInt32(0);
                            decimal odenecekTutar = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                            decimal odenenTutar = Convert.ToDecimal(txtMusteriOdenenTutari.Text);
                            if (odenenTutar > odenecekTutar)
                            {
                                MessageBox.Show("Girilen tutar ödenecek tutardan fazla olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return; // Girilen tutar geçersizse işlemi durdur
                            }
                            decimal yeniOdenecekTutar = odenecekTutar - odenenTutar;

                            reader.Close();

                            command.Parameters.Clear();
                            // Yeni ödeme kaydı ekleme ve Odenecek değeri güncelleme
                            command.CommandText = "INSERT INTO Fatura (MusteriFaturaID, Odenen, FaturaTarih, Odenecek) VALUES(@MusteriID, @Odenen, @FaturaTarih, @YeniOdenecek)";
                            command.Parameters.AddWithValue("@MusteriID", customerId);
                            command.Parameters.AddWithValue("@Odenen", odenenTutar);
                            command.Parameters.AddWithValue("@FaturaTarih", DateTime.Now);
                            command.Parameters.AddWithValue("@YeniOdenecek", yeniOdenecekTutar);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Ödeme bilgisi başarıyla eklendi ve ödenecek tutar güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen ad ve soyada sahip bir müşteri bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }

            // Arayüzü ve DataGridView'i güncelle
            TemizleVeGuncelle();
        }

        private void TemizleVeGuncelle()
        {
            txtMusteriAdi.Clear();
            txtMusteriOdenenTutari.Clear();
            txtMusteriSoyadi.Clear();
            txtMusteriOdenecekTutari.Clear();
            dataGridView1.Rows.Clear();
            MusteriGetir(); // DataGridView'i güncellenmiş verilerle yeniden doldurur
        }

        private void OdemeMusteriAra_Click(object sender, EventArgs e)
        {

            //Filtre için ad ve soyad değerlerini al
            var ad = txtMusteriAdi.Text.ToLower();
            var soyad = txtMusteriSoyadi.Text.ToLower();
            bool sonucBulundu = false; // Arama sonucu bulunup bulunmadığını takip etmek için

            dataGridView1.CurrentCell = null; // Mevcut seçimi temizle

            // Mevcut DataGridView içeriğinden, arama kriterlerine uyan satırları bul ve göster
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool gorunurluk = row.Cells["AdColumn"].Value.ToString().ToLower().Contains(ad) &&
                                  row.Cells["SoyadColumn"].Value.ToString().ToLower().Contains(soyad);
                row.Visible = gorunurluk;

                if (gorunurluk && !sonucBulundu)
                {
                    sonucBulundu = true; // En az bir sonuç bulundu
                }
            }

        }

        private void AramaSifirla_Click(object sender, EventArgs e)
        {
            
                // Arama kutularını temizle
                txtMusteriAdi.Text = "";
                txtMusteriSoyadi.Text = "";

                // Verileri yeniden yükle
                MusteriGetir();

                // Tüm satırları görünür yap
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Visible = true;
                }
            
        }


        private void FaturaGecmisi_Click(object sender, EventArgs e)
        {
           
                FaturaGecmisi faturaGecmisi = new FaturaGecmisi();
                faturaGecmisi.Show();
                this.Hide();
            
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void MaxButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            GirisSayfasi gsgiris = new GirisSayfasi();
            gsgiris.Show();
            this.Hide();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }


    }
}
    


