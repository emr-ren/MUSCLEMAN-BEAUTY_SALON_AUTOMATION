using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RandevuSistemi
{
    public partial class InaktifMusteriListesi: Form
    {
        static readonly string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
        public InaktifMusteriListesi()
        {
            InitializeComponent();
        }

        private void InaktifMusteriListesi_Load(object sender, EventArgs e)
        {
            LoadInactiveCustomers();
            dataGridViewInactiveCustomers.ClearSelection();
        }

        private void LoadInactiveCustomers()
        {
            dataGridViewInactiveCustomers.Font = new Font("Agency FB", 12, FontStyle.Bold);
            dataGridViewInactiveCustomers.Columns.Clear();
            dataGridViewInactiveCustomers.Columns.Add("MusteriID", "Müşteri Numarası");
            dataGridViewInactiveCustomers.Columns["MusteriID"].Visible = false;
            dataGridViewInactiveCustomers.Columns.Add("AdColumn", "Ad");
            dataGridViewInactiveCustomers.Columns.Add("SoyadColumn", "Soyad");
            dataGridViewInactiveCustomers.Columns.Add("TelefonColumn", "Telefon");

            string query = @"
SELECT 
    MusteriID, 
    Ad, 
    Soyad, 
    Telefon
FROM 
    Musteri 
WHERE 
    IsActive = 0;";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int musteriId = reader.GetInt32(reader.GetOrdinal("MusteriID"));
                        string ad = reader.GetString(reader.GetOrdinal("Ad"));
                        string soyad = reader.GetString(reader.GetOrdinal("Soyad"));
                        string telefon = reader.IsDBNull(reader.GetOrdinal("Telefon")) ? "" : reader.GetString(reader.GetOrdinal("Telefon"));

                        dataGridViewInactiveCustomers.Rows.Add(musteriId, ad, soyad, telefon);
                    }
                    reader.Close();
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Yedekleme yedek = new Yedekleme();
            yedek.Show();
            this.Hide();
        }

        private void DataGridViewInactiveCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            object value = dataGridViewInactiveCustomers.Rows[e.RowIndex].Cells["MusteriID"].Value;
            if (value == null) return; // Null kontrolü

            int musteriId = Convert.ToInt32(value);
            string musteriAdi = dataGridViewInactiveCustomers.Rows[e.RowIndex].Cells["AdColumn"].Value?.ToString() ?? ""; // Null kontrolü
            string musteriSoyadi = dataGridViewInactiveCustomers.Rows[e.RowIndex].Cells["SoyadColumn"].Value?.ToString() ?? ""; // Null kontrolü

            var confirmationResult = MessageBox.Show($"{musteriAdi} {musteriSoyadi} adlı müşteriyi aktif hale getirmek istediğinize emin misiniz?", "Müşteri Aktifleştirme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmationResult == DialogResult.Yes)
            {
                string query = "UPDATE Musteri SET IsActive = 1 WHERE MusteriID = @MusteriID";
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MusteriID", musteriId);
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show($"{musteriAdi} {musteriSoyadi} adlı müşteri başarıyla aktif hale getirildi.");
                            LoadInactiveCustomers();
                        }
                        else
                        {
                            MessageBox.Show("Müşteri aktif hale getirilirken bir hata oluştu.");
                        }
                    }
                }
            }
        }



        private void FilterInactiveCustomers()
        {
            string ad = txtAdArama.Text.ToLower();
            string soyad = txtSoyadArama.Text.ToLower();
            string telefon = txtTefonlaArama.Text.ToLower();

            foreach (DataGridViewRow row in dataGridViewInactiveCustomers.Rows)
            {
                // Yeni ve tamamlanmamış satırları atla
                if (row.IsNewRow)
                    continue;

                string rowAd = row.Cells["AdColumn"].Value?.ToString().ToLower() ?? "";
                string rowSoyad = row.Cells["SoyadColumn"].Value?.ToString().ToLower() ?? "";
                string rowTelefon = row.Cells["TelefonColumn"].Value?.ToString().ToLower() ?? "";

                bool adMatch = rowAd.Contains(ad);
                bool soyadMatch = rowSoyad.Contains(soyad);
                bool telefonMatch = rowTelefon.Contains(telefon);

                row.Visible = adMatch && soyadMatch && telefonMatch;
            }
        }





        private void BtnAramaSifirla_Click(object sender, EventArgs e)
        {
            txtAdArama.Text = "";
            txtSoyadArama.Text = "";
            txtTefonlaArama.Text = "";

            foreach (DataGridViewRow row in dataGridViewInactiveCustomers.Rows)
            {
                row.Visible = true;
            }
        }

        private void BtnAra_Click_1(object sender, EventArgs e)
        {
            FilterInactiveCustomers();
        }

    }
}

