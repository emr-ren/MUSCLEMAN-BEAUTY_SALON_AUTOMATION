using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RandevuSistemi
{
    public partial class Yedekleme : Form
    {
        static readonly string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
        public Yedekleme()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            GirisSayfasi gsgiris = new GirisSayfasi();
            gsgiris.Show();
            this.Hide();
        }

        private void PasiflesmisMusteriler_Click(object sender, EventArgs e)
        {
            InaktifMusteriListesi inaktifListe = new InaktifMusteriListesi();
            inaktifListe.Show();
            this.Hide();
        }

        #region Veri Tabanı Yedeği 
        private void VeriTabaniYedegi_Click(object sender, EventArgs e)
        {
            string serverName = "INCUBUS\\SQLEXPRESS01";
                                    
            string databaseName = "GuzellikDB";
            BackupDatabase(serverName, databaseName);
        }

        private void BackupDatabase(string serverName, string databaseName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string musterilerFolder = Path.Combine(desktopPath, "Müşteriler");
            string databaseFolder = Path.Combine(musterilerFolder, "Veri Tabanı");

            if (!Directory.Exists(musterilerFolder))
            {
                Directory.CreateDirectory(musterilerFolder);
            }

            if (!Directory.Exists(databaseFolder))
            {
                Directory.CreateDirectory(databaseFolder);
            }

            string backupFilePath = Path.Combine(databaseFolder, $"{databaseName}.bak");

            try
            {
                // Server nesnesi doğrudan sunucu ismi ile oluşturuluyor
                Server sqlServer = new Server(serverName);
                Database database = sqlServer.Databases[databaseName];

                Backup backup = new Backup
                {
                    Action = BackupActionType.Database,
                    Database = databaseName,
                    Initialize = true,
                    LogTruncation = BackupTruncateLogType.Truncate
                };

                backup.Devices.AddDevice(backupFilePath, DeviceType.File);
                backup.SqlBackup(sqlServer);

                MessageBox.Show($"Database backed up successfully to {backupFilePath}", "Backup Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during backup: {ex.Message}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        #endregion
        #region YEDEKLEME-CSV
        private void ExportDataToCSV()
        {
            try
            {
                // Masaüstündeki csvMusteri klasörünün tam yolunu al
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string csvMusteriPath = Path.Combine(desktopPath, "Müşteriler");

                // Eğer csvMusteri klasörü yoksa, oluştur
                if (!Directory.Exists(csvMusteriPath))
                {
                    Directory.CreateDirectory(csvMusteriPath);
                }

                // CSV dosya yolunu belirle
                string csvFilePath = Path.Combine(csvMusteriPath, "musteriData.csv");

                StringBuilder csvContent = new StringBuilder();
                // CSV başlıklarını güncelle
                csvContent.AppendLine("MusteriID,Ad,Soyad,Telefon,Epilasyon Hizmetleri,Masaj Hizmetleri,Zayiflama Hizmetleri,Odenen,Odenecek");



                using (SqlConnection connection = new SqlConnection(conString))
                {
                    string query = @"
SELECT 
    M.MusteriID,
    M.Ad,
    M.Soyad,
    M.Telefon,
    Epilasyon.EpilasyonHizmetleri,
    CiltBakimi.CiltBakimiHizmetleri,  
    Zayiflama.ZayiflamaHizmetleri,
    ISNULL((SELECT SUM(F.Odenen) FROM [GuzellikDB].[dbo].[Fatura] F WHERE F.MusteriFaturaID = M.MusteriID), 0) AS ToplamOdenen,
    (SELECT TOP 1 F.Odenecek FROM [GuzellikDB].[dbo].[Fatura] F WHERE F.MusteriFaturaID = M.MusteriID ORDER BY F.FaturaID DESC) AS Odenecek
FROM 
    [GuzellikDB].[dbo].[Musteri] M
LEFT JOIN 
    (SELECT MB.MusteriBolgeID, STRING_AGG(B.BolgeAdi + ' Seans: ' + CAST(MB.BolgeSeans AS VARCHAR(10)), ', ') WITHIN GROUP (ORDER BY B.BolgeAdi) AS EpilasyonHizmetleri
     FROM [GuzellikDB].[dbo].[MusteriBolge] MB
     JOIN [GuzellikDB].[dbo].[Bolgeler] B ON MB.BolgeID = B.BolgeID
     GROUP BY MB.MusteriBolgeID) Epilasyon ON M.MusteriID = Epilasyon.MusteriBolgeID
LEFT JOIN 
    (SELECT MC.MusteriCiltBakimiID, STRING_AGG(CB.CiltBakimiAdi + ' Seans: ' + CAST(MC.CiltBakimiSeans AS VARCHAR(10)), ', ') WITHIN GROUP (ORDER BY CB.CiltBakimiAdi) AS CiltBakimiHizmetleri 
     FROM [GuzellikDB].[dbo].[MusteriCiltBakimi] MC
     JOIN [GuzellikDB].[dbo].[CiltBakimi] CB ON MC.CiltBakimiID = CB.CiltBakimiID
     GROUP BY MC.MusteriCiltBakimiID) CiltBakimi ON M.MusteriID = CiltBakimi.MusteriCiltBakimiID
LEFT JOIN 
    (SELECT MZ.MusteriZayiflamaID, STRING_AGG(Z.ZayiflamaAdi + ' Seans: ' + CAST(MZ.ZayiflamaSeans AS VARCHAR(10)), ', ') WITHIN GROUP (ORDER BY Z.ZayiflamaAdi) AS ZayiflamaHizmetleri
     FROM [GuzellikDB].[dbo].[MusteriZayiflama] MZ
     JOIN [GuzellikDB].[dbo].[Zayiflama] Z ON MZ.ZayiflamaID = Z.ZayiflamaID
     GROUP BY MZ.MusteriZayiflamaID) Zayiflama ON M.MusteriID = Zayiflama.MusteriZayiflamaID
";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var musteriID = reader["MusteriID"].ToString();
                                var ad = reader["Ad"].ToString();
                                var soyad = reader["Soyad"].ToString();
                                var telefon = reader["Telefon"].ToString();
                                var epilasyonHizmetleri = reader["EpilasyonHizmetleri"].ToString();
                                var ciltBakimiHizmetleri = reader["CiltBakimiHizmetleri"].ToString();  // Masaj yerine Cilt Bakımı
                                var zayiflamaHizmetleri = reader["ZayiflamaHizmetleri"].ToString();
                                var odenen = reader["ToplamOdenen"].ToString();
                                var odenecek = reader["Odenecek"].ToString();

                                // Eğer değer boşsa '-' ile değiştir
                                epilasyonHizmetleri = string.IsNullOrEmpty(epilasyonHizmetleri) ? "-" : epilasyonHizmetleri;
                                ciltBakimiHizmetleri = string.IsNullOrEmpty(ciltBakimiHizmetleri) ? "-" : ciltBakimiHizmetleri;  // Masaj yerine Cilt Bakımı
                                zayiflamaHizmetleri = string.IsNullOrEmpty(zayiflamaHizmetleri) ? "-" : zayiflamaHizmetleri;
                                odenen = string.IsNullOrEmpty(odenen) ? "-" : odenen;
                                odenecek = string.IsNullOrEmpty(odenecek) ? "-" : odenecek;

                                // CSV içeriğine eklemek için kullanılan StringBuilder örneği (csvContent) burada kullanılmalıdır.
                                csvContent.AppendLine($"{musteriID},{ad},{soyad},{telefon},\"{epilasyonHizmetleri}\",\"{ciltBakimiHizmetleri}\",\"{zayiflamaHizmetleri}\",{odenen},{odenecek}");
                            }
                        }
                    }
                }


                File.WriteAllText(csvFilePath, csvContent.ToString(), Encoding.UTF8);
                MessageBox.Show($"Veriler {csvFilePath} yoluna başarıyla aktarıldı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        private void CSVkaydet(object sender, EventArgs e)
        {
            ExportDataToCSV();
        }



        #endregion

        private void Yedekleme_Load(object sender, EventArgs e)
        {

        }
    }
}
