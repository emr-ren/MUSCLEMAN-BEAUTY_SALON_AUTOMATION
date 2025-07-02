using RandevuSistemi;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Windows.Forms;

using Twilio;
using Twilio.Rest.Api.V2010.Account;



namespace BEAUTYLIFE
{
    public partial class Randevular : Form
    {
        #region Sayfa butonları
        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            GirisSayfasi gsgiris = new GirisSayfasi();
            gsgiris.Show();
            this.Hide();
        }
        #endregion


        private System.Windows.Forms.Timer timer;

        private string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";

        public Randevular()
        {
            InitializeComponent();

        }

        #region RANDEVU SAATINDE MESAJ GONDERME

        //TWILLO HESABI 
        public void MesajGonder(string telefonNumarasi, string isim)
        {
            const string accountSid = "AC99c081fbc05dcb1adaabe50ecfccc4c9";
            const string authToken = "20f1e0408c1a15910d54dbecb4d460d4";

            try
            {
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: $"Merhaba {isim}, yarın için randevunuz olduğunu hatırlatmak isteriz.",
                    from: new Twilio.Types.PhoneNumber("+15709015102"),
                    to: new Twilio.Types.PhoneNumber(telefonNumarasi)
                );

                // Gönderme işlemi başarılıysa, mesajın SID'sini yazdır
                Console.WriteLine($"Mesaj gönderildi. SID: {message.Sid}");
            }
            catch (Exception ex)
            {
                // Hata oluşursa, hata mesajını yazdır
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }


        private string EkleTelefonKodu(string telefonNumarasi)
        {
            // Eğer numara +90 ile başlıyorsa veya 0 ile başlıyorsa, işlem yapma
            if (telefonNumarasi.StartsWith("+90"))
            {
                return telefonNumarasi;
            }

            // Numaranın başındaki 0 karakterini kaldır ve +90 ekleyerek geri döndür
            return "+90" + telefonNumarasi.Substring(1);
        }


        /*                TWILLO DENEME     Randevular.Desinger satır 276
        private void MesajGonderButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Telefon numarası ve isim bilgisini alın
                string telefonNumarasi = "05301504878"; // Örnek telefon numarası
                string isim = "Emre"; // Örnek isim

                // Telefon numarasını düzelt
                telefonNumarasi = EkleTelefonKodu(telefonNumarasi);

                // Mesajı gönder
                MesajGonder(telefonNumarasi, isim);

                // Başarı mesajı göster
                MessageBox.Show("Mesaj başarıyla gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Hata mesajını göster
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        #endregion



        private void Randevular_Load(object sender, EventArgs e)
        {
            LoadHistoryData();
        }

        private void LoadHistoryData()
        {
            // Parametreleri tanımla
            string ad = txtAdArama.Text.Trim();
            string soyad = txtSoyadArama.Text.Trim();

            string query = @"
SELECT 
    M.Ad AS Ad,
    M.Soyad AS Soyad,
    M.Telefon AS Telefon,
    CONVERT(varchar, R.RandevuTarihi, 106) AS Tarih,
    CONVERT(varchar, R.RandevuSaati, 108) AS Saat,
    STRING_AGG(
        CASE 
            WHEN Z.ZayiflamaAdi IS NOT NULL THEN Z.ZayiflamaAdi
            WHEN C.CiltBakimiAdi IS NOT NULL THEN C.CiltBakimiAdi
            WHEN B.BolgeAdi IS NOT NULL THEN B.BolgeAdi
        END, ', ') AS Hizmetler,
    MAX(P.PersonelAd) AS [Hizmeti Yapan Personeller]
FROM 
    Musteri M
LEFT JOIN 
    (
        SELECT 
            MusteriID, 
            RandevuTarihi, 
            RandevuSaati,
            ZayiflamaID,
            NULL AS CiltBakimiID,
            NULL AS BolgeID,
            PersonelID
        FROM 
            RandevuZayiflama
        UNION ALL
        SELECT 
            MusteriID, 
            RandevuTarihi, 
            RandevuSaati,
            NULL AS ZayiflamaID,
            CiltBakimiID,
            NULL AS BolgeID,
            PersonelID
        FROM 
            RandevuCiltBakimi
        UNION ALL
        SELECT 
            MusteriID, 
            RandevuTarihi, 
            RandevuSaati,
            NULL AS ZayiflamaID,
            NULL AS CiltBakimiID,
            BolgeID,
            PersonelID
        FROM 
            RandevuBolge
    ) AS R ON M.MusteriID = R.MusteriID
LEFT JOIN Zayiflama Z ON R.ZayiflamaID = Z.ZayiflamaID
LEFT JOIN CiltBakimi C ON R.CiltBakimiID = C.CiltBakimiID
LEFT JOIN Bolgeler B ON R.BolgeID = B.BolgeID
LEFT JOIN Personel P ON R.PersonelID = P.PersonelID
WHERE 
    M.Ad LIKE @Ad AND M.Soyad LIKE @Soyad
GROUP BY  
    M.Ad, M.Soyad, M.Telefon, R.RandevuTarihi, R.RandevuSaati
ORDER BY 
    R.RandevuTarihi DESC, R.RandevuSaati DESC;
";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // Assign parameter values
                command.Parameters.AddWithValue("@Ad", "%" + ad + "%");
                command.Parameters.AddWithValue("@Soyad", "%" + soyad + "%");

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridViewEskiRandevular.DataSource = dt; // Assuming dataGridViewEskiRandevular is your DataGridView
                reader.Close();
            }
        }

        private void BtnAra_Click_Click(object sender, EventArgs e)
        {
            // Filtre için ad ve soyad değerlerini al
            var ad = txtAdArama.Text.Trim().ToLower();
            var soyad = txtSoyadArama.Text.Trim().ToLower();

            bool sonucBulundu = false; // Arama sonucu bulunup bulunmadığını takip etmek için

            dataGridViewEskiRandevular.CurrentCell = null; // Mevcut seçimi temizle

            // Mevcut DataGridView içeriğinden, arama kriterlerine uyan satırları bul ve göster
            foreach (DataGridViewRow row in dataGridViewEskiRandevular.Rows)
            {
                // Her bir hücreyi kontrol ederken null kontrolü yapın
                if (row.Cells["Ad"].Value != null && row.Cells["Soyad"].Value != null)
                {
                    // AdSoyad sütununu kullanarak arama yap
                    bool gorunurluk = row.Cells["Ad"].Value.ToString().Trim().ToLower().Contains(ad) &&
                                        row.Cells["Soyad"].Value.ToString().Trim().ToLower().Contains(soyad);

                    row.Visible = gorunurluk;

                    if (gorunurluk && !sonucBulundu)
                    {
                        sonucBulundu = true; // En az bir sonuç bulundu
                    }
                }
            }

            // Eğer aranan isimde bir kullanıcı bulunamadıysa uyarı mesajı göster
            if (!sonucBulundu)
            {
                MessageBox.Show("Aradığınız kullanıcı bulunamadı", "Sonuç Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AramaSifirla_Click(object sender, EventArgs e)
        {
            // Arama kutularını temizle
            txtAdArama.Text = "";
            txtSoyadArama.Text = "";
            LoadHistoryData();
            // Tüm satırları görünür yap
            foreach (DataGridViewRow row in dataGridViewEskiRandevular.Rows)
            {
                row.Visible = true;
            }
        }



    }
}
