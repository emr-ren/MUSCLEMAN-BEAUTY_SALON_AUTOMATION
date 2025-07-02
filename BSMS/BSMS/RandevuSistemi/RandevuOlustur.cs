using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace RandevuSistemi
{
    public partial class RandevuOlustur : Form
    {

        static readonly string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";

        public RandevuOlustur()
        {
            InitializeComponent();
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.MonthCalendar1_DateChanged_1);
            comboBoxPersonel.SelectedIndexChanged += comboBoxPersonel_SelectedIndexChanged;
            LoadPersonel();
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

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void RandevuOlustur_Load(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today; // Bugünkü tarih

            this.monthCalendar1.MinDate = startDate.AddDays(-7); // 7 gün önceki tarih
            this.monthCalendar1.MaxDate = startDate.AddYears(100); // 100 yıl sonrası

            LoadPersonel();
            AssignCheckBoxEventsForGroupBox(groupBox1, Color.PaleVioletRed);
            AssignCheckBoxEventsForGroupBox(groupBox2, Color.LightSeaGreen);
            AssignCheckBoxEventsForGroupBox(groupBox3, Color.Tan);

            MusteriGetir();
            ConfigureDataGridViewRandevular();
            dataGridView1.ClearSelection();
            SaatButonlarıClick();
            ConfigureDataGridViewForPersonel(dataGridViewPersonel1);
            ConfigureDataGridViewForPersonel(dataGridViewPersonel2);
            ConfigureDataGridViewForPersonel(dataGridViewPersonel3);
            ConfigureDataGridViewForPersonel(dataGridViewPersonel4);
            // Bugünün tarihini ayarla ve ilgili randevuları yükle
            DateTime today = DateTime.Now.Date;
            monthCalendar1.SetDate(today);  // Takvim kontrolünü bugüne ayarla
            LoadAppointmentsForToday(today);  // Bugünkü randevuları yükle
        }
        private void LoadAppointmentsForToday(DateTime selectedDate)
        {
            // Her personel için randevuları yükle
            LoadAppointmentsForPersonel(selectedDate, 1, dataGridViewPersonel1);
            LoadAppointmentsForPersonel(selectedDate, 2, dataGridViewPersonel2);
            LoadAppointmentsForPersonel(selectedDate, 3, dataGridViewPersonel3);
            LoadAppointmentsForPersonel(selectedDate, 4, dataGridViewPersonel4);

        }



        void MusteriGetir()
        {


            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("MusteriID", "Müşteri Numarası");
            dataGridView1.Columns.Add("AdColumn", "Ad");
            dataGridView1.Columns.Add("SoyadColumn", "Soyad");
            dataGridView1.Columns.Add("TelefonColumn", "Telefon");
            dataGridView1.Columns.Add("BolgeAdiColumn", "Bölge Adı");
            dataGridView1.Columns.Add("CiltBakimiAdiColumn", "Cilt Bakımı Adı");
            dataGridView1.Columns.Add("ZayiflamaAdiColumn", "Zayıflama Adı");
            dataGridView1.Columns.Add("EnSonRandevuTarihi", "Randevu");


            dataGridView1.Columns["BolgeAdiColumn"].Visible = false;
            dataGridView1.Columns["CiltBakimiAdiColumn"].Visible = false;
            dataGridView1.Columns["ZayiflamaAdiColumn"].Visible = false;

            string query = @"SELECT 
    M.MusteriID, 
    M.Ad, 
    M.Soyad, 
    M.Telefon,
    Bolgeler.Bolgeler,
    CiltBakimlar.CiltBakimlar,
    Zayiflamalar.Zayiflamalar,
    ISNULL(CONVERT(VARCHAR, MAX(R.RandevuTarihi), 106), 'Randevu Yok') AS EnSonRandevuTarihi
FROM 
    Musteri AS M 
LEFT JOIN 
    (SELECT MB.MusteriBolgeID, STRING_AGG(B.BolgeAdi, ', ') AS Bolgeler 
     FROM MusteriBolge AS MB 
     JOIN Bolgeler AS B ON MB.BolgeID = B.BolgeID 
     GROUP BY MB.MusteriBolgeID) AS Bolgeler ON M.MusteriID = Bolgeler.MusteriBolgeID 
LEFT JOIN 
    (SELECT MC.MusteriCiltBakimiID, STRING_AGG(C.CiltBakimiAdi, ', ') AS CiltBakimlar 
     FROM MusteriCiltBakimi AS MC 
     JOIN CiltBakimi AS C ON MC.CiltBakimiID = C.CiltBakimiID 
     GROUP BY MC.MusteriCiltBakimiID) AS CiltBakimlar ON M.MusteriID = CiltBakimlar.MusteriCiltBakimiID
LEFT JOIN 
    (SELECT MZ.MusteriZayiflamaID, STRING_AGG(Za.ZayiflamaAdi, ', ') AS Zayiflamalar 
     FROM MusteriZayiflama AS MZ 
     JOIN Zayiflama AS Za ON MZ.ZayiflamaID = Za.ZayiflamaID 
     GROUP BY MZ.MusteriZayiflamaID) AS Zayiflamalar ON M.MusteriID = Zayiflamalar.MusteriZayiflamaID
LEFT JOIN 
    (SELECT MusteriID, RandevuTarihi FROM RandevuZayiflama
     UNION ALL
     SELECT MusteriID, RandevuTarihi FROM RandevuCiltBakimi
     UNION ALL
     SELECT MusteriID, RandevuTarihi FROM RandevuBolge) AS R ON M.MusteriID = R.MusteriID
WHERE 
    M.IsActive = 1
GROUP BY 
    M.MusteriID, 
    M.Ad, 
    M.Soyad, 
    M.Telefon, 
    Bolgeler.Bolgeler,
    CiltBakimlar.CiltBakimlar,
    Zayiflamalar.Zayiflamalar;
";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string musteriID = reader["MusteriID"]?.ToString() ?? "";
                        string ad = reader["Ad"]?.ToString() ?? "";
                        string soyad = reader["Soyad"]?.ToString() ?? "";
                        string telefon = reader["Telefon"]?.ToString() ?? "";
                        string bolgeler = reader["Bolgeler"]?.ToString() ?? "";
                        string ciltBakim = reader["CiltBakimlar"]?.ToString() ?? "";
                        string zayiflama = reader["Zayiflamalar"]?.ToString() ?? "";
                        string enSonRandevuTarihi = reader["EnSonRandevuTarihi"]?.ToString() ?? "Randevu Yok";
                        dataGridView1.Rows.Add(new object[] { musteriID, ad, soyad, telefon, bolgeler, ciltBakim, zayiflama, enSonRandevuTarihi });
                    }
                    reader.Close();
                }
            }
            dataGridView1.ClearSelection();
        }


        #region Musteri Arama Butonlari
        private void BtnAra_Click_Click(object sender, EventArgs e)
        { // Filtre için ad, soyad ve telefon numarası değerlerini al
            var ad = txtAdArama.Text.ToLower();
            var soyad = txtSoyadArama.Text.ToLower();
            var telefon = txtTefonlaArama.Text.ToLower(); // Telefon numarası için yeni TextBox varsayımı

            bool sonucBulundu = false; // Arama sonucu bulunup bulunmadığını takip etmek için

            dataGridView1.CurrentCell = null; // Mevcut seçimi temizle

            // Mevcut DataGridView içeriğinden, arama kriterlerine uyan satırları bul ve göster
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Telefon numarası kriterini de ekleyerek arama yap
                bool gorunurluk = row.Cells["AdColumn"].Value.ToString().ToLower().Contains(ad) &&
                                  row.Cells["SoyadColumn"].Value.ToString().ToLower().Contains(soyad) &&
                                  row.Cells["TelefonColumn"].Value.ToString().ToLower().Contains(telefon); // Telefon numarasıyla eşleşme kontrolü

                row.Visible = gorunurluk;

                if (gorunurluk && !sonucBulundu)
                {
                    sonucBulundu = true; // En az bir sonuç bulundu
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
            txtTefonlaArama.Text = "";
            txtMusteriAdi.Text = "";
            txtMusteriSoyadi.Text = "";
            txtTelefonNumarasi.Text = "";
            // Tüm satırları görünür yap
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }
        }
        #endregion

        private void BtnNotAl_Click(object sender, EventArgs e)
        {

            Notlar notlarForm = new Notlar();
            notlarForm.Show(this); // 'this' şu anki formu (RandevuOlustur) gösterir.

        }

        private void ClearAllTextBoxes()
        {
            // TEXTBOXLARI SIFIRALYACAK BİR KOD GELECEK BURAYA 
        }

        private void MusteriSeansVerileriYukle(int musteriId)
        {
            ClearAllTextBoxes();   //YAPIM AŞAMASINDA

            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();

                // Cilt bakımı seans bilgileri için sorgu
                string queryCiltBakimi = @"
            SELECT C.CiltBakimiAdi, MC.CiltBakimiSeans
            FROM MusteriCiltBakimi MC
            INNER JOIN CiltBakimi C ON MC.CiltBakimiID = C.CiltBakimiID
            WHERE MC.MusteriCiltBakimiID = @MusteriID;";


                // Zayıflama seans bilgileri için sorgu
                string queryZayiflama = @"
            SELECT Z.ZayiflamaAdi, MZ.ZayiflamaSeans
            FROM MusteriZayiflama MZ
            INNER JOIN Zayiflama Z ON MZ.ZayiflamaID = Z.ZayiflamaID
            WHERE MZ.MusteriZayiflamaID = @MusteriID;
                    ";
                // Epilasyon seans bilgileri için sorgu
                string queryBolge = @"
            SELECT B.BolgeAdi, MB.BolgeSeans
            FROM MusteriBolge MB
            INNER JOIN Bolgeler B ON MB.BolgeID = B.BolgeID
            WHERE MB.MusteriBolgeID = @MusteriID;
                    ";

                SeansSorgusu(connection, queryCiltBakimi, musteriId, "CiltBakimiAdi", "CiltBakimiSeans");
                SeansSorgusu(connection, queryZayiflama, musteriId, "ZayiflamaAdi", "ZayiflamaSeans");
                SeansSorgusu(connection, queryBolge, musteriId, "BolgeAdi", "BolgeSeans");
            }
        }


        private void SeansSorgusu(SqlConnection connection, string query, int musteriId, string adiColumn, string seansColumn)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MusteriID", musteriId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _ = reader.IsDBNull(reader.GetOrdinal(adiColumn)) ? "Bilinmiyor" : reader.GetString(reader.GetOrdinal(adiColumn));
                        _ = (reader.IsDBNull(reader.GetOrdinal(seansColumn)) || reader.GetInt32(reader.GetOrdinal(seansColumn)) == 0) ? 0 : reader.GetInt32(reader.GetOrdinal(seansColumn));
                    }
                }
            }
        }

        //Checboxları ve Textboxları doldur
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) // Header satırına tıklanırsa işlem yapma
                return;

            // Tıklanan satırı al
            _ = dataGridView1.Rows[e.RowIndex];

            ClearCheckBoxes();
            ClearAllTextBoxes(); 

            // Seçilen satırın müşteri ID'sini ve diğer müşteri bilgilerini al
            var musteriID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["MusteriID"].Value);
            var musteriAdi = dataGridView1.CurrentRow.Cells["AdColumn"].Value.ToString();
            var musteriSoyadi = dataGridView1.CurrentRow.Cells["SoyadColumn"].Value.ToString();
            var telefonNumarasi = dataGridView1.CurrentRow.Cells["TelefonColumn"].Value.ToString();

            // TextBox'lara müşteri bilgilerini yaz
            txtMusteriAdi.Text = musteriAdi;
            txtMusteriSoyadi.Text = musteriSoyadi;
            txtTelefonNumarasi.Text = telefonNumarasi;

            // Müşteri ID'sini gizli bir TextBox'a yaz
            gizliId.Text = musteriID.ToString();

            // Müşteriye ait seans bilgilerini yükle
            MusteriSeansVerileriYukle(musteriID);

            // Seans bilgilerini CheckBox'lara işle
            CheckSeansBoxes(dataGridView1.CurrentRow);
        }

        #region CHECKBOX KİŞİ RANDEVU SEÇİMİ
        private void ClearCheckBoxes()
        {
            ClearAndDisableCheckBoxes(groupBox1);
            ClearAndDisableCheckBoxes(groupBox2);
            ClearAndDisableCheckBoxes(groupBox3);
        }
        private void ClearAndDisableCheckBoxes(GroupBox groupBox)
        {
            foreach (System.Windows.Forms.CheckBox checkBox in groupBox.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                checkBox.Checked = false;
                checkBox.Enabled = false;
                checkBox.BackColor = Color.Transparent; // Arka plan rengini sıfırla
            }
        }

        private void CheckSeansBoxes(DataGridViewRow currentRow)
        {
            // Tüm checkbox'ları önce devre dışı bırak ve temizle
            ClearCheckBoxes();

            // İlgili hizmetlerin checkbox'larını işaretle ve etkinleştir
            SetCheckBoxes(currentRow, "BolgeAdiColumn", groupBox1);
            SetCheckBoxes(currentRow, "CiltBakimiAdiColumn", groupBox2);
            SetCheckBoxes(currentRow, "ZayiflamaAdiColumn", groupBox3);
        }

        private void SetCheckBoxes(DataGridViewRow row, string columnName, GroupBox groupBox)
        {
            if (row.Cells[columnName].Value == null)
            {
                return;
            }
            string[] items = row.Cells[columnName].Value.ToString().Split(',');

            foreach (System.Windows.Forms.CheckBox checkBox in groupBox.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                bool isChecked = items.Any(item => item.Trim() == checkBox.Text);
                checkBox.Checked = isChecked;
                checkBox.Enabled = isChecked;
            }
        }

        #endregion

        #region Randevu Kayıt

        //SAATLER KAYDI
        private void SaatButonlarıClick()
        {
            button1.Click += (s, args) => SaatAyarlamaButon(s);
            button2.Click += (s, args) => SaatAyarlamaButon(s);
            button4.Click += (s, args) => SaatAyarlamaButon(s);
            button7.Click += (s, args) => SaatAyarlamaButon(s);
            button6.Click += (s, args) => SaatAyarlamaButon(s);
            button11.Click += (s, args) => SaatAyarlamaButon(s);
            button10.Click += (s, args) => SaatAyarlamaButon(s);
            button9.Click += (s, args) => SaatAyarlamaButon(s);
            button8.Click += (s, args) => SaatAyarlamaButon(s);
            button5.Click += (s, args) => SaatAyarlamaButon(s);
            button16.Click += (s, args) => SaatAyarlamaButon(s);
            button15.Click += (s, args) => SaatAyarlamaButon(s);
            button14.Click += (s, args) => SaatAyarlamaButon(s);
            button13.Click += (s, args) => SaatAyarlamaButon(s);
            button12.Click += (s, args) => SaatAyarlamaButon(s);
            button21.Click += (s, args) => SaatAyarlamaButon(s);
            button20.Click += (s, args) => SaatAyarlamaButon(s);
            button19.Click += (s, args) => SaatAyarlamaButon(s);
            button18.Click += (s, args) => SaatAyarlamaButon(s);
            button17.Click += (s, args) => SaatAyarlamaButon(s);
        }
        private void SaatAyarlamaButon(object sender)
        {
            if (!(sender is System.Windows.Forms.Button clickedButton) || clickedButton.Tag == null)
            {
                MessageBox.Show("Buton veya saat bilgisi hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir müşteri seçiniz.", "Müşteri Seçimi Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime randevuTarihi = monthCalendar1.SelectionRange.Start;
            TimeSpan randevuSaati = TimeSpan.Parse(clickedButton.Tag.ToString());

            // Zaman çakışmasını kontrol et
            if (OSaatteBaskaRandevuVarMi(randevuTarihi, randevuSaati))
            {
                MessageBox.Show("Bu saatte zaten bir randevu var.", "Randevu Çakışması", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int musteriID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["MusteriID"].Value);
            bool anySelected = false;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                anySelected |= ProcessCheckedAppointments(connection, musteriID, randevuTarihi, randevuSaati, groupBox1, "Bolge");
                anySelected |= ProcessCheckedAppointments(connection, musteriID, randevuTarihi, randevuSaati, groupBox2, "CiltBakimi");
                anySelected |= ProcessCheckedAppointments(connection, musteriID, randevuTarihi, randevuSaati, groupBox3, "Zayiflama");
            }

            if (anySelected)
            {
                MessageBox.Show("Randevular başarıyla kaydedildi.", "Randevu Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshCustomerUI(musteriID);
                clickedButton.Enabled = false;  // Disable the time button immediately after booking
                RefreshCustomerUI(musteriID);
                LoadAppointmentsForToday(randevuTarihi); // Optionally reload appointments to reflect changes
                //LoadAppointments(randevuTarihi); // Randevu tarihine göre randevuları yeniden yükle

                // Personel DataGridView'leri yeniden yükle
                LoadAppointmentsForPersonel(randevuTarihi, 1, dataGridViewPersonel1);
                LoadAppointmentsForPersonel(randevuTarihi, 2, dataGridViewPersonel2);
                LoadAppointmentsForPersonel(randevuTarihi, 3, dataGridViewPersonel3);
                LoadAppointmentsForPersonel(randevuTarihi, 4, dataGridViewPersonel4);

            }
        }


        private bool OSaatteBaskaRandevuVarMi(DateTime date, TimeSpan time)
        {
            foreach (DataGridViewRow row in dataGridViewRandevular.Rows)
            {
                if (row.Cells["TimeSlot"].Value.ToString() == time.ToString(@"hh\:mm") && row.Cells["MusteriID"].Value != null)
                {
                    return true; // Bu zaman dilimi zaten kullanılıyor.
                }
            }
            return false;
        }



        private void RefreshCustomerUI(int musteriID)
        {
            MusteriGetir(); // Müşteri listesini yeniden yükle
            DateTime selectedDate = monthCalendar1.SelectionRange.Start; // Takvimden seçili tarihi al

            // Müşteri bilgilerini ana DataGridView'de güncelle
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                object value = row.Cells["MusteriID"].Value;
                if (value != null && int.TryParse(value.ToString(), out int id) && id == musteriID)
                {
                    dataGridView1.CurrentCell = row.Cells[0]; // Seçili hücreyi güncelle
                    dataGridView1.CurrentRow.Selected = true;
                    MusteriSeansVerileriYukle(musteriID); // Müşterinin seans verilerini yükle
                    CheckSeansBoxes(row); // Checkboxları yeniden işaretle
                    break;
                }
            }

            // Tüm personel randevu datagrid'lerini yenile
            LoadAppointmentsForPersonel(selectedDate, 1, dataGridViewPersonel1);
            LoadAppointmentsForPersonel(selectedDate, 2, dataGridViewPersonel2);
            LoadAppointmentsForPersonel(selectedDate, 3, dataGridViewPersonel3);
            LoadAppointmentsForPersonel(selectedDate, 4, dataGridViewPersonel4);
        }




        private bool ProcessCheckedAppointments(SqlConnection connection, int musteriID, DateTime randevuTarihi, TimeSpan randevuSaati, GroupBox groupBox, string serviceType)
        {
            bool anySelected = false;
            int personelID = Convert.ToInt32(comboBoxPersonel.SelectedValue); // Personel ID'sini ComboBox'dan al
            foreach (CheckBox cb in groupBox.Controls.OfType<CheckBox>())
            {
                if (cb.Checked)
                {
                    int hizmetID = Convert.ToInt32(cb.Tag);
                    InsertRandevu(connection, musteriID, hizmetID, personelID, randevuTarihi, randevuSaati, serviceType);
                    anySelected = true;
                }
            }
            return anySelected;
        }


        private void InsertRandevu(SqlConnection connection, int musteriID, int hizmetID, int personelID, DateTime randevuTarihi, TimeSpan randevuSaati, string hizmetTipi)
        {
            string tableName = "";
            string columnIDName = "";
            string seansTableName = "";
            string seansColumnName = "";
            string seansIdColumnName = "";

            switch (hizmetTipi)
            {
                case "Bolge":
                    tableName = "RandevuBolge";
                    columnIDName = "BolgeID";
                    seansTableName = "MusteriBolge";
                    seansColumnName = "BolgeSeans";
                    seansIdColumnName = "MusteriBolgeID";
                    break;
                case "CiltBakimi":
                    tableName = "RandevuCiltBakimi";
                    columnIDName = "CiltBakimiID";
                    seansTableName = "MusteriCiltBakimi";
                    seansColumnName = "CiltBakimiSeans";
                    seansIdColumnName = "MusteriCiltBakimiID";
                    break;
                case "Zayiflama":
                    tableName = "RandevuZayiflama";
                    columnIDName = "ZayiflamaID";
                    seansTableName = "MusteriZayiflama";
                    seansColumnName = "ZayiflamaSeans";
                    seansIdColumnName = "MusteriZayiflamaID";
                    break;
            }

            // Randevu bilgilerini veritabanına ekleyin
            string insertCommandText = $"INSERT INTO {tableName} (MusteriID, {columnIDName}, RandevuTarihi, RandevuSaati, PersonelID) VALUES (@MusteriID, @HizmetID, @RandevuTarihi, @RandevuSaati, @PersonelID)";
            using (SqlCommand insertCommand = new SqlCommand(insertCommandText, connection))
            {
                insertCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                insertCommand.Parameters.AddWithValue("@HizmetID", hizmetID);
                insertCommand.Parameters.AddWithValue("@PersonelID", personelID);
                insertCommand.Parameters.AddWithValue("@RandevuTarihi", randevuTarihi);
                insertCommand.Parameters.AddWithValue("@RandevuSaati", randevuSaati);
                insertCommand.ExecuteNonQuery();
            }

            // Seans sayısını 1 azalt
            string updateSeansText = $"UPDATE {seansTableName} SET {seansColumnName} = {seansColumnName} - 1 WHERE {seansIdColumnName} = @MusteriID AND {columnIDName} = @HizmetID";
            using (SqlCommand updateSeansCommand = new SqlCommand(updateSeansText, connection))
            {
                updateSeansCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                updateSeansCommand.Parameters.AddWithValue("@HizmetID", hizmetID);
                updateSeansCommand.ExecuteNonQuery();
            }

            // Seans sayısı 0 ise, hizmet kaydını sil
            string deleteServiceText = $"DELETE FROM {seansTableName} WHERE {seansIdColumnName} = @MusteriID AND {columnIDName} = @HizmetID AND {seansColumnName} <= 0";
            using (SqlCommand deleteServiceCommand = new SqlCommand(deleteServiceText, connection))
            {
                deleteServiceCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                deleteServiceCommand.Parameters.AddWithValue("@HizmetID", hizmetID);
                deleteServiceCommand.ExecuteNonQuery();
            }
        }


        private void ConfigureDataGridViewRandevular()
        {
            dataGridViewRandevular.Columns.Clear();
            dataGridViewRandevular.Columns.Add("TimeSlot", "Saat");
            dataGridViewRandevular.Columns[0].Width = 50;

            dataGridViewRandevular.Columns.Add("Customer", "Müşteri");
            dataGridViewRandevular.Columns[1].Width = 100;

            dataGridViewRandevular.Columns.Add("Procedure", "İşlem");
            dataGridViewRandevular.Columns[2].Width = 100;
            DataGridViewColumn musteriIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "MusteriID",
                HeaderText = "Müşteri ID",
                Visible = false  // Bu sütunu gizle
            };
            dataGridViewRandevular.Columns.Add(musteriIDColumn);

            TimeSpan startTime = new TimeSpan(9, 30, 0);
            TimeSpan endTime = new TimeSpan(19, 30, 0);
            TimeSpan interval = TimeSpan.FromMinutes(30);

            while (startTime < endTime)
            {
                dataGridViewRandevular.Rows.Add(startTime.ToString(@"hh\:mm"), "", "");
                startTime = startTime.Add(interval);
            }

            dataGridViewRandevular.AllowUserToAddRows = false;
        }

        private void LoadAppointmentsForPersonel(DateTime selectedDate, int personelID, DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.Cells["Customer"].Value = "";
                row.Cells["Procedure"].Value = "";
                row.Cells["MusteriID"].Value = null;
                row.DefaultCellStyle.BackColor = Color.White;
            }

            string query = @"
SELECT 
    M.MusteriID,
    CONCAT(M.Ad, ' ', M.Soyad) AS IsimSoyisim,
    CONVERT(varchar, Hizmetler.RandevuSaati, 108) AS RandevuSaati,
    STRING_AGG(HizmetAdi, ', ') WITHIN GROUP (ORDER BY HizmetAdi) AS Hizmetler,
    SUM(Sure) AS TotalDuration
FROM 
    Musteri M
INNER JOIN 
    (
        SELECT R.MusteriID, R.RandevuTarihi, R.RandevuSaati, Z.ZayiflamaAdi AS HizmetAdi, Z.ZayiflamaSure AS Sure, R.PersonelID
        FROM RandevuZayiflama R
        INNER JOIN Zayiflama Z ON R.ZayiflamaID = Z.ZayiflamaID
        UNION ALL
        SELECT R.MusteriID, R.RandevuTarihi, R.RandevuSaati, C.CiltBakimiAdi, C.CiltBakimiSure, R.PersonelID
        FROM RandevuCiltBakimi R
        INNER JOIN CiltBakimi C ON R.CiltBakimiID = C.CiltBakimiID
        UNION ALL
        SELECT R.MusteriID, R.RandevuTarihi, R.RandevuSaati, B.BolgeAdi, B.Sure, R.PersonelID
        FROM RandevuBolge R
        INNER JOIN Bolgeler B ON R.BolgeID = B.BolgeID
    ) Hizmetler ON M.MusteriID = Hizmetler.MusteriID
WHERE 
    Hizmetler.RandevuTarihi = @SelectedDate AND Hizmetler.PersonelID = @PersonelID
GROUP BY 
    M.MusteriID, CONCAT(M.Ad, ' ', M.Soyad), Hizmetler.RandevuSaati;";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SelectedDate", selectedDate);
                command.Parameters.AddWithValue("@PersonelID", personelID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int customerId = Convert.ToInt32(reader["MusteriID"]);
                    string startTimeStr = reader["RandevuSaati"].ToString();
                    string customer = reader["IsimSoyisim"].ToString();
                    string procedures = reader["Hizmetler"].ToString();
                    int totalDuration = Convert.ToInt32(reader["TotalDuration"]);
                    Color customerColor = GetCustomerColor(customerId);

                    TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                    TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(totalDuration));

                    bool isFirstRow = true;

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        TimeSpan rowTime = TimeSpan.Parse(row.Cells["TimeSlot"].Value.ToString());
                        if (rowTime >= startTime && rowTime < endTime)
                        {
                            if (isFirstRow)
                            {
                                row.Cells["Customer"].Value = customer;
                                row.Cells["Procedure"].Value = procedures;
                                isFirstRow = false;
                            }
                            row.Cells["MusteriID"].Value = customerId;
                            row.DefaultCellStyle.BackColor = customerColor;
                        }
                    }
                }
                reader.Close();
                SecicimTemizle();
            }
        }
        private void SecicimTemizle()
        {
            dataGridViewPersonel1.ClearSelection();
            dataGridViewPersonel2.ClearSelection();
            dataGridViewPersonel3.ClearSelection();
            dataGridViewPersonel4.ClearSelection();
        }
        private void ConfigureDataGridViewForPersonel(DataGridView dataGridView)
        {
            dataGridView.Columns.Clear();
            dataGridView.Columns.Add("TimeSlot", "Saat");
            dataGridView.Columns[0].Width = 50;

            dataGridView.Columns.Add("Customer", "Müşteri");
            dataGridView.Columns[1].Width = 100;

            dataGridView.Columns.Add("Procedure", "İşlem");
            dataGridView.Columns[2].Width = 100;
            DataGridViewColumn musteriIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "MusteriID",
                HeaderText = "Müşteri ID",
                Visible = false  // Bu sütunu gizle
            };
            dataGridView.Columns.Add(musteriIDColumn);

            TimeSpan startTime = new TimeSpan(9, 30, 0);
            TimeSpan endTime = new TimeSpan(19, 30, 0);
            TimeSpan interval = TimeSpan.FromMinutes(30);

            dataGridView.Rows.Clear();
            while (startTime < endTime)
            {
                dataGridView.Rows.Add(startTime.ToString(@"hh\:mm"), "", "");
                startTime = startTime.Add(interval);
            }

            dataGridView.AllowUserToAddRows = false;
        }


        private void LoadPersonel()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string query = "SELECT PersonelID, PersonelAd FROM Personel";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Dictionary<int, string> personelList = new Dictionary<int, string>();
                while (reader.Read())
                {
                    personelList.Add(reader.GetInt32(0), reader.GetString(1));
                }
                comboBoxPersonel.DataSource = new BindingSource(personelList, null);
                comboBoxPersonel.DisplayMember = "Value";
                comboBoxPersonel.ValueMember = "Key";
            }
        }


        readonly Dictionary<int, Color> customerColors = new Dictionary<int, Color>();
        private Color GetCustomerColor(int customerId)
        {
            if (!customerColors.ContainsKey(customerId))
            {
                // Yeni rastgele renk üret ve dictionary'e ekle
                Color newColor = GenerateUniqueRandomColor();
                customerColors[customerId] = newColor;
                return newColor;
            }
            return customerColors[customerId];
        }


        //Tablo renklendirme
        private readonly List<Color> usedColors = new List<Color>();  // Kullanılan renkleri saklamak için bir liste
        private Color GenerateUniqueRandomColor()
        {
            Random rand = new Random();
            Color color;
            do
            {
                color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            } while (IsColorUsed(color));  // Bu renk daha önce kullanıldıysa, yeni bir renk üret

            usedColors.Add(color);  // Yeni üretilen rengi kullanılan renkler listesine ekle
            return color;
        }


        private bool IsColorUsed(Color color)
        {
            return usedColors.Any(usedColor => usedColor.ToArgb() == color.ToArgb());
        }

        private void AssignCheckBoxEventsForGroupBox(System.Windows.Forms.GroupBox groupBox, Color checkedColor)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is System.Windows.Forms.CheckBox checkBox)
                {
                    checkBox.CheckedChanged += (sender, args) =>
                    {
                        checkBox.BackColor = checkBox.Checked ? checkedColor : Color.Transparent;
                    };
                }
            }
        }


        private void MonthCalendar1_DateChanged_1(object sender, DateRangeEventArgs e)
        {
            DateTime selectedDate = e.Start;
            // Her personel için randevuları yükleyin ve saat butonlarını güncelleyin.
            LoadAppointmentsForToday(selectedDate);
            UpdateButtonStates(selectedDate);


        }
        private void UpdateButtonStates(DateTime selectedDate)
        {
            if (comboBoxPersonel.SelectedValue is int personelID)
            {
                List<TimeSpan> busyTimes = GetBusyTimes(personelID, selectedDate);
                DisableBusyTimeButtons(busyTimes);
            }
        }

        private void DataGridViewRandevular_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || dataGridViewRandevular.Rows[e.RowIndex].IsNewRow)
                return; // Başlık veya yeni satıra tıklanırsa işlem yapma

            // Müşteri ID'yi al
            var cellValue = dataGridViewRandevular.Rows[e.RowIndex].Cells["MusteriID"].Value;
            if (cellValue != null)
            {
                int musteriID = Convert.ToInt32(cellValue);
                DateTime randevuTarihi = monthCalendar1.SelectionRange.Start;
                string timeStr = dataGridViewRandevular.Rows[e.RowIndex].Cells["TimeSlot"].Value.ToString();
                TimeSpan randevuSaati = TimeSpan.Parse(timeStr);

                if (MessageBox.Show("Bu randevuyu iptal istediğinizden emin misiniz?", "Randevu İptal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RandevuSil(musteriID, randevuTarihi, randevuSaati);
                }
            }
            else
            {
                MessageBox.Show("Bu satır için geçerli bir müşteri ID'si bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RandevuSil(int musteriID, DateTime randevuTarihi, TimeSpan randevuSaati)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                try
                {
                    connection.Open();
                    // Randevu ile ilişkili hizmet türleri ve ID'lerini çekme
                    List<Tuple<string, int>> hizmetler = new List<Tuple<string, int>>();

                    string getHizmetlerQuery = @"
           SELECT 'Bolge' AS HizmetTipi, BolgeID AS HizmetID FROM RandevuBolge WHERE MusteriID = @MusteriID AND RandevuTarihi = @RandevuDateTime AND RandevuSaati = @RandevuTime
           UNION
           SELECT 'CiltBakimi' AS HizmetTipi, CiltBakimiID AS HizmetID FROM RandevuCiltBakimi WHERE MusteriID = @MusteriID AND RandevuTarihi = @RandevuDateTime AND RandevuSaati = @RandevuTime
           UNION
           SELECT 'Zayiflama' AS HizmetTipi, ZayiflamaID AS HizmetID FROM RandevuZayiflama WHERE MusteriID = @MusteriID AND RandevuTarihi = @RandevuDateTime AND RandevuSaati = @RandevuTime";

                    using (SqlCommand getHizmetlerCommand = new SqlCommand(getHizmetlerQuery, connection))
                    {
                        getHizmetlerCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                        getHizmetlerCommand.Parameters.AddWithValue("@RandevuDateTime", randevuTarihi.Date);
                        getHizmetlerCommand.Parameters.AddWithValue("@RandevuTime", randevuSaati);
                        using (SqlDataReader reader = getHizmetlerCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hizmetler.Add(new Tuple<string, int>(reader["HizmetTipi"].ToString(), Convert.ToInt32(reader["HizmetID"])));
                            }
                        }
                    }

                    // Randevuları sil
                    foreach (var hizmet in hizmetler)
                    {
                        string commandText = $@"
               DELETE FROM {GetRandevuTableName(hizmet.Item1)} WHERE MusteriID = @MusteriID AND RandevuTarihi = @RandevuDateTime AND RandevuSaati = @RandevuTime;";
                        using (SqlCommand command = new SqlCommand(commandText, connection))
                        {
                            command.Parameters.AddWithValue("@MusteriID", musteriID);
                            command.Parameters.AddWithValue("@RandevuDateTime", randevuTarihi.Date);
                            command.Parameters.AddWithValue("@RandevuTime", randevuSaati);
                            command.ExecuteNonQuery();
                        }

                        // Hizmetlerin seans sayısını artır
                        string updateSeansQuery = $"UPDATE {GetSeansTableName(hizmet.Item1)} SET {GetSeansColumnName(hizmet.Item1)} = {GetSeansColumnName(hizmet.Item1)} + 1 WHERE {GetIdColumnName(hizmet.Item1)} = @MusteriID AND {GetColumnIDName(hizmet.Item1)} = @HizmetID";
                        using (SqlCommand updateSeansCommand = new SqlCommand(updateSeansQuery, connection))
                        {
                            updateSeansCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                            updateSeansCommand.Parameters.AddWithValue("@HizmetID", hizmet.Item2);
                            updateSeansCommand.ExecuteNonQuery();
                        }

                        // Eğer seans sayısı 0'dan 1'e çıkıyorsa, hizmeti tekrar eklemek
                        string checkAndInsertSeansQuery = $@"
               IF NOT EXISTS(SELECT 1 FROM {GetSeansTableName(hizmet.Item1)} WHERE {GetIdColumnName(hizmet.Item1)} = @MusteriID AND {GetColumnIDName(hizmet.Item1)} = @HizmetID)
               BEGIN
                   INSERT INTO {GetSeansTableName(hizmet.Item1)} ({GetIdColumnName(hizmet.Item1)}, {GetColumnIDName(hizmet.Item1)}, {GetSeansColumnName(hizmet.Item1)}) VALUES (@MusteriID, @HizmetID, 1)
               END";
                        using (SqlCommand checkAndInsertSeansCommand = new SqlCommand(checkAndInsertSeansQuery, connection))
                        {
                            checkAndInsertSeansCommand.Parameters.AddWithValue("@MusteriID", musteriID);
                            checkAndInsertSeansCommand.Parameters.AddWithValue("@HizmetID", hizmet.Item2);
                            checkAndInsertSeansCommand.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Randevu başarıyla silindi ve seanslar güncellendi.", "Silindi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReEnableTimeButton(randevuSaati);
                    RefreshCustomerUI(musteriID);// Müşteri bilgilerini ve seans bilgilerini güncelle
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ReEnableTimeButton(TimeSpan time)
        {
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Button button && button.Tag != null)
                {
                    TimeSpan buttonTime = TimeSpan.Parse(button.Tag.ToString());
                    if (buttonTime == time)
                    {
                        button.Enabled = true; // İlgili zaman butonunu tekrar etkinleştir
                        break; // Eşleşen buton bulunduğunda döngüyü kır
                    }
                }
            }
        }
        private string GetRandevuTableName(string hizmetTipi)
        {
            switch (hizmetTipi)
            {
                case "Bolge": return "RandevuBolge";
                case "CiltBakimi": return "RandevuCiltBakimi";
                case "Zayiflama": return "RandevuZayiflama";
                default: throw new ArgumentException("Invalid hizmet tipi");
            }
        }

        private string GetSeansTableName(string hizmetTipi)
        {
            switch (hizmetTipi)
            {
                case "Bolge": return "MusteriBolge";
                case "CiltBakimi": return "MusteriCiltBakimi";
                case "Zayiflama": return "MusteriZayiflama";
                default: return null;
            }
        }


        private string GetSeansColumnName(string hizmetTipi)
        {
            switch (hizmetTipi)
            {
                case "Bolge": return "BolgeSeans";
                case "CiltBakimi": return "CiltBakimiSeans";
                case "Zayiflama": return "ZayiflamaSeans";
                default: return null;
            }
        }


        private string GetIdColumnName(string hizmetTipi)
        {
            switch (hizmetTipi)
            {
                case "Bolge": return "MusteriBolgeID";
                case "CiltBakimi": return "MusteriCiltBakimiID";
                case "Zayiflama": return "MusteriZayiflamaID";
                default: return null;
            }
        }


        private string GetColumnIDName(string hizmetTipi)
        {
            switch (hizmetTipi)
            {
                case "Bolge":
                    return "BolgeID";
                case "CiltBakimi":
                    return "CiltBakimiID";
                case "Zayiflama":
                    return "ZayiflamaID";
                default:
                    throw new ArgumentException("Invalid hizmet tipi");
            }
        }

        private void dataGridViewPersonel1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RandevuIptal(sender, e);
        }


        private void dataGridViewPersonel2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RandevuIptal(sender, e);
        }


        private void dataGridViewPersonel3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RandevuIptal(sender, e);
        }


        private void dataGridViewPersonel4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RandevuIptal(sender, e);
        }

        #endregion

        //Randevu iptal etme fonksiyonu
        private void RandevuIptal(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;

            if (e.RowIndex == -1 || dataGridView.Rows[e.RowIndex].IsNewRow)
                return; // Başlık veya yeni satıra tıklanırsa işlem yapma

            var cellValue = dataGridView.Rows[e.RowIndex].Cells["MusteriID"].Value;
            if (cellValue != null)
            {
                int musteriID = Convert.ToInt32(cellValue);
                DateTime randevuTarihi = monthCalendar1.SelectionRange.Start;
                string timeStr = dataGridView.Rows[e.RowIndex].Cells["TimeSlot"].Value.ToString();
                TimeSpan randevuSaati = TimeSpan.Parse(timeStr);

                if (MessageBox.Show("Bu randevuyu iptal etmek istediğinizden emin misiniz?", "Randevu İptal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RandevuSil(musteriID, randevuTarihi, randevuSaati);
                }
            }
            else
            {
                MessageBox.Show("Bu satır için geçerli bir müşteri ID'si bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBoxPersonel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPersonel.SelectedValue is int personelID) // Use 'is' for safe casting
            {
                DateTime selectedDate = monthCalendar1.SelectionRange.Start;
                List<TimeSpan> busyTimes = GetBusyTimes(personelID, selectedDate);
                DisableBusyTimeButtons(busyTimes);
            }
        }

        private List<TimeSpan> GetBusyTimes(int personelID, DateTime selectedDate)
        {
            List<TimeSpan> busyTimes = new List<TimeSpan>();
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string query = @"
                    SELECT RandevuSaati
                    FROM RandevuBolge 
                    WHERE PersonelID = @PersonelID AND CONVERT(date, RandevuTarihi) = @SelectedDate
                    UNION
                    SELECT RandevuSaati
                    FROM RandevuCiltBakimi 
                    WHERE PersonelID = @PersonelID AND CONVERT(date, RandevuTarihi) = @SelectedDate
                    UNION
                    SELECT RandevuSaati
                    FROM RandevuZayiflama 
                    WHERE PersonelID = @PersonelID AND CONVERT(date, RandevuTarihi) = @SelectedDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PersonelID", personelID);
                command.Parameters.AddWithValue("@SelectedDate", selectedDate.Date);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    busyTimes.Add(TimeSpan.Parse(reader["RandevuSaati"].ToString()));
                }
            }
            return busyTimes;
        }

        private void DisableBusyTimeButtons(List<TimeSpan> busyTimes)
        {
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Button button && button.Tag != null)
                {
                    TimeSpan buttonTime = TimeSpan.Parse(button.Tag.ToString());
                    button.Enabled = !busyTimes.Contains(buttonTime);
                }
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            System.Windows.Forms.ComboBox combo = sender as System.Windows.Forms.ComboBox;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush textBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.ControlText;
                Brush backBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? Brushes.Red : Brushes.White; // Seçilen öğenin arka plan rengini kırmızı yap

                e.Graphics.FillRectangle(backBrush, e.Bounds);
                e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, textBrush, e.Bounds);
            }
        }
    }
}
