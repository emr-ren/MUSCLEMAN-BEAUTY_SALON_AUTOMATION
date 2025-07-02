using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandevuSistemi
{
    public partial class FaturaGecmisi : Form
    {
        string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
        public FaturaGecmisi()
        {
            InitializeComponent();
        }

        private void FaturaGecmisi_Load(object sender, EventArgs e)
        {
            this.TopMost = false; // alt tab engelle
            FaturaGetir();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FaturaIslemleri gsgiris = new FaturaIslemleri();
            gsgiris.Show();
            this.Hide();
        }

        void FaturaGetir()
        {
            dataGridView1.Columns.Clear(); // Önceki sütunları temizleyin

            // Sütunları ekleme
            dataGridView1.Columns.Add("AdColumn", "Ad");
            dataGridView1.Columns.Add("SoyadColumn", "Soyad");
            dataGridView1.Columns.Add("Odenen", "Ödenen Tutar / Durum");
            dataGridView1.Columns.Add("Tarih", "Tarih");

            string query = @"
            SELECT M.MusteriID, M.Ad, M.Soyad, F.Odenen, F.FaturaTarih,
                   ROW_NUMBER() OVER (PARTITION BY M.MusteriID ORDER BY F.FaturaTarih ASC) AS SeqNum
            FROM Musteri AS M 
            INNER JOIN Fatura AS F ON M.MusteriID = F.MusteriFaturaID
            ORDER BY F.FaturaTarih DESC";

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
                        var tarih = reader["FaturaTarih"] != DBNull.Value ? Convert.ToDateTime(reader["FaturaTarih"]).ToString("dd/MM/yyyy") : "";
                        int seqNum = Convert.ToInt32(reader["SeqNum"]);

                        // Ödenen tutar 0 ise ve sıra numarası 1 ise "Kayıt Oldu", aksi halde "Güncellendi"
                        var odenenDurum = odenen == 0 ? (seqNum == 1 ? "Kayıt Oldu" : "Güncellendi") :
                                          odenen.ToString("F2");  // F2 formatını kullanarak ödemeye girilen '-' değeri datagrirde gösterdim

                        dataGridView1.Rows.Add(ad, soyad, odenenDurum, tarih);
                    }

                    reader.Close();
                }
            }
        }







        private void BtnAra_Click_Click(object sender, EventArgs e)
        {
            
                //Filtre için ad ve soyad değerlerini al
                var ad = txtAdArama.Text.ToLower();
                var soyad = txtSoyadArama.Text.ToLower();
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

                // Eğer aranan isimde bir kullanıcı bulunamadıysa uyarı mesajı göster
                if (!sonucBulundu)
                {
                    MessageBox.Show("Aradığınız kullanıcı bulunamadı, yeni müşteri olarak ekleyebilirsiniz.", "Sonuç Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                // Hata mesajı olmayan

                /*
                var ad = txtAdArama.Text.ToLower();
                var soyad = txtSoyadArama.Text.ToLower();

                // Mevcut DataGridView içeriğinden, arama kriterlerine uyan satırları bul
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Visible = row.Cells["AdColumn"].Value.ToString().ToLower().Contains(ad) &&
                                  row.Cells["SoyadColumn"].Value.ToString().ToLower().Contains(soyad);
                }
                */
            

        }

        private void AramaSifirla_Click(object sender, EventArgs e)
        {
            // Arama kutularını temizle
            txtAdArama.Text = "";
            txtSoyadArama.Text = "";

            // Verileri yeniden yükle
            FaturaGetir();

            // Tüm satırları görünür yap
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
