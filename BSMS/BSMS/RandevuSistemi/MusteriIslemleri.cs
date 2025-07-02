using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.IO;
using System.Text.RegularExpressions;


namespace RandevuSistemi
{
    public partial class MusteriIslemleri : Form
    {

        static readonly string conString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";


        readonly List<int> selectedBolgeIDs = new List<int>();
        readonly List<int> selectedCiltBakimiIDs = new List<int>();
        readonly List<int> selectedZayiflamaIDs = new List<int>();

        public SqlConnection Connect { get; private set; }

        public MusteriIslemleri()
        {
            InitializeDatabase();
            InitializeComponent();
        }

        void MusteriGetir()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("MusteriID", "Müşteri Numarası");
            dataGridView1.Columns.Add("AdColumn", "Ad");
            dataGridView1.Columns.Add("SoyadColumn", "Soyad");
            dataGridView1.Columns.Add("TelefonColumn", "Telefon");
            dataGridView1.Columns.Add("BolgeAdiColumn", "Bölge Adı");
            dataGridView1.Columns.Add("CiltBakimiAdiColumn", "Cilt Bakımı Adı");
            dataGridView1.Columns.Add("ZayiflamaAdiColumn", "Zayıflama Adı");
            dataGridView1.Columns.Add("Odenen", "Ödenen Tutar");
            dataGridView1.Columns.Add("Odenecek", "Toplam Ödenecek");

            string query = @"
    SELECT 
        M.MusteriID, 
        M.Ad, 
        M.Soyad, 
        M.Telefon, 
        Bolgeler.Bolgeler,
        CiltBakimlar.CiltBakimlar,
        Zayiflamalar.Zayiflamalar,
        (SELECT TOP 1 F.Odenecek FROM Fatura F  
         WHERE F.MusteriFaturaID = M.MusteriID  
         ORDER BY F.FaturaID DESC) AS OdenecekToplam, 
        SUM(F.Odenen) AS OdenenToplam 
    FROM 
        Musteri AS M 
    LEFT JOIN 
        (SELECT MB.MusteriBolgeID, STRING_AGG(B.BolgeAdi, ', ') AS Bolgeler 
         FROM MusteriBolge AS MB 
         JOIN Bolgeler AS B ON MB.BolgeID = B.BolgeID 
         GROUP BY MB.MusteriBolgeID) AS Bolgeler ON M.MusteriID = Bolgeler.MusteriBolgeID 
    LEFT JOIN 
        Fatura AS F ON M.MusteriID = F.MusteriFaturaID
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
    WHERE 
        M.IsActive = 1
    GROUP BY 
        M.MusteriID, 
        M.Ad, 
        M.Soyad, 
        M.Telefon, 
        Bolgeler.Bolgeler,
        CiltBakimlar.CiltBakimlar,
        Zayiflamalar.Zayiflamalar";

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
                        string ciltBakimlar = reader["CiltBakimlar"]?.ToString() ?? "";  // Updated
                        string zayiflama = reader["Zayiflamalar"]?.ToString() ?? "";
                        decimal odenenToplam = reader["OdenenToplam"] != DBNull.Value ? Convert.ToDecimal(reader["OdenenToplam"]) : 0m;
                        decimal odenecekToplam = reader["OdenecekToplam"] != DBNull.Value ? Convert.ToDecimal(reader["OdenecekToplam"]) : 0m;

                        // Update the DataGridView rows add
                        dataGridView1.Rows.Add(new object[] { musteriID, ad, soyad, telefon, bolgeler, ciltBakimlar, zayiflama, odenenToplam, odenecekToplam });
                    }
                    // Close the reader and connection
                    reader.Close();
                }
            }


        }

        private void InitializeDatabase()
        {
            Connect = new SqlConnection(conString);
        }

        private void MusteriIslemleri_Load(object sender, EventArgs e)
        {
            this.TopMost = true;   // alt tab engelle 

            CheckboxRenklendir();
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

        private bool Uyarılar()
        {
            #region HATA SEANS/HİZMET UYARILARI 
            List<string> seanslar = new List<string>();
            #region Epilasyon seans sayıları kontrolü

            if (checkBoxCene.Checked && (!int.TryParse(txtSeansCene.Text, out _) || string.IsNullOrWhiteSpace(txtSeansCene.Text) || txtSeansCene.Text == "0"))
                seanslar.Add("Çene için seans sayısı");

            if (!checkBoxCene.Checked && !string.IsNullOrWhiteSpace(txtSeansCene.Text))
                seanslar.Add("Çene hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxBiyik.Checked && (!int.TryParse(txtSeansBiyik.Text, out _) || string.IsNullOrWhiteSpace(txtSeansBiyik.Text) || txtSeansBiyik.Text == "0"))
                seanslar.Add("Bıyık için seans sayısı");
            if (!checkBoxBiyik.Checked && !string.IsNullOrWhiteSpace(txtSeansBiyik.Text))
                seanslar.Add("Bıyık hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxCeneBiyik.Checked && (!int.TryParse(txtSeansCeneBiyik.Text, out _) || string.IsNullOrWhiteSpace(txtSeansCeneBiyik.Text) || txtSeansCeneBiyik.Text == "0"))
                seanslar.Add("Çene-Bıyık için seans sayısı");
            if (!checkBoxCeneBiyik.Checked && !string.IsNullOrWhiteSpace(txtSeansCeneBiyik.Text))
                seanslar.Add("Çene-Bıyık hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxYuz.Checked && (!int.TryParse(txtSeansYuz.Text, out _) || string.IsNullOrWhiteSpace(txtSeansYuz.Text) || txtSeansYuz.Text == "0"))
                seanslar.Add("Yüz için seans sayısı");
            if (!checkBoxYuz.Checked && !string.IsNullOrWhiteSpace(txtSeansYuz.Text))
                seanslar.Add("Yüz hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxFaul.Checked && (!int.TryParse(txtSeansFaul.Text, out _) || string.IsNullOrWhiteSpace(txtSeansFaul.Text) || txtSeansFaul.Text == "0"))
                seanslar.Add("Faul hizmeti için seans sayısı");
            if (!checkBoxFaul.Checked && !string.IsNullOrWhiteSpace(txtSeansFaul.Text))
                seanslar.Add("Faul hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGenital.Checked && (!int.TryParse(txtSeansGenital.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGenital.Text) || txtSeansGenital.Text == "0"))
                seanslar.Add("Genital hizmeti için seans sayısı");
            if (!checkBoxGenital.Checked && !string.IsNullOrWhiteSpace(txtSeansGenital.Text))
                seanslar.Add("Genital hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxTumKol.Checked && (!int.TryParse(txtSeansTumKol.Text, out _) || string.IsNullOrWhiteSpace(txtSeansTumKol.Text) || txtSeansTumKol.Text == "0"))
                seanslar.Add("Tüm kol hizmeti için seans sayısı");
            if (!checkBoxTumKol.Checked && !string.IsNullOrWhiteSpace(txtSeansTumKol.Text))
                seanslar.Add("Tüm kol hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxTumBacak.Checked && (!int.TryParse(txtSeansTumBacak.Text, out _) || string.IsNullOrWhiteSpace(txtSeansTumBacak.Text) || txtSeansTumBacak.Text == "0"))
                seanslar.Add("Tüm bacak hizmeti için seans sayısı");
            if (!checkBoxTumBacak.Checked && !string.IsNullOrWhiteSpace(txtSeansTumBacak.Text))
                seanslar.Add("Tüm bacak hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGobek.Checked && (!int.TryParse(txtSeansGobek.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGobek.Text) || txtSeansGobek.Text == "0"))
                seanslar.Add("Göbek hizmeti için seans sayısı");
            if (!checkBoxGobek.Checked && !string.IsNullOrWhiteSpace(txtSeansGobek.Text))
                seanslar.Add("Göbek hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxSirtKomple.Checked && (!int.TryParse(txtSeansSirtKomple.Text, out _) || string.IsNullOrWhiteSpace(txtSeansSirtKomple.Text) || txtSeansSirtKomple.Text == "0"))
                seanslar.Add("Sırt komple hizmeti için seans sayısı");
            if (!checkBoxSirtKomple.Checked && !string.IsNullOrWhiteSpace(txtSeansSirtKomple.Text))
                seanslar.Add("Sırt komple hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxKolAlti.Checked && (!int.TryParse(txtSeansKolAlti.Text, out _) || string.IsNullOrWhiteSpace(txtSeansKolAlti.Text) || txtSeansKolAlti.Text == "0"))
                seanslar.Add("Kol altı hizmeti için seans sayısı");
            if (!checkBoxKolAlti.Checked && !string.IsNullOrWhiteSpace(txtSeansKolAlti.Text))
                seanslar.Add("Kol altı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxPopo.Checked && (!int.TryParse(txtSeansPopo.Text, out _) || string.IsNullOrWhiteSpace(txtSeansPopo.Text) || txtSeansPopo.Text == "0"))
                seanslar.Add("Popo hizmeti için seans sayısı");
            if (!checkBoxPopo.Checked && !string.IsNullOrWhiteSpace(txtSeansPopo.Text))
                seanslar.Add("Popo hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGobekCizgisi.Checked && (!int.TryParse(txtSeansGobekCizgisi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGobekCizgisi.Text) || txtSeansGobekCizgisi.Text == "0"))
                seanslar.Add("Göbek çizgisi hizmeti için seans sayısı");
            if (!checkBoxGobekCizgisi.Checked && !string.IsNullOrWhiteSpace(txtSeansGobekCizgisi.Text))
                seanslar.Add("Göbek çizgisi hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxYarimBacak.Checked && (!int.TryParse(txtSeansYarimBacak.Text, out _) || string.IsNullOrWhiteSpace(txtSeansYarimBacak.Text) || txtSeansYarimBacak.Text == "0"))
                seanslar.Add("Yarım bacak hizmeti için seans sayısı");
            if (!checkBoxYarimBacak.Checked && !string.IsNullOrWhiteSpace(txtSeansYarimBacak.Text))
                seanslar.Add("Yarım bacak hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxYarimKol.Checked && (!int.TryParse(txtSeansYarimKol.Text, out _) || string.IsNullOrWhiteSpace(txtSeansYarimKol.Text) || txtSeansYarimKol.Text == "0"))
                seanslar.Add("Yarım kol hizmeti için seans sayısı");
            if (!checkBoxYarimKol.Checked && !string.IsNullOrWhiteSpace(txtSeansYarimKol.Text))
                seanslar.Add("Yarım kol hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxBel.Checked && (!int.TryParse(txtSeansBel.Text, out _) || string.IsNullOrWhiteSpace(txtSeansBel.Text) || txtSeansBel.Text == "0"))
                seanslar.Add("Bel hizmeti için seans sayısı");
            if (!checkBoxBel.Checked && !string.IsNullOrWhiteSpace(txtSeansBel.Text))
                seanslar.Add("Bel hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGogusKomple.Checked && (!int.TryParse(txtSeansGogusKomple.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGogusKomple.Text) || txtSeansGogusKomple.Text == "0"))
                seanslar.Add("Göğüs komple hizmeti için seans sayısı");
            if (!checkBoxGogusKomple.Checked && !string.IsNullOrWhiteSpace(txtSeansGogusKomple.Text))
                seanslar.Add("Göğüs komple hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGogusArasi.Checked && (!int.TryParse(txtSeansGogusArasi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGogusArasi.Text) || txtSeansGogusArasi.Text == "0"))
                seanslar.Add("Göğüs arası hizmeti için seans sayısı");
            if (!checkBoxGogusArasi.Checked && !string.IsNullOrWhiteSpace(txtSeansGogusArasi.Text))
                seanslar.Add("Göğüs arası hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGogusUcu.Checked && (!int.TryParse(txtSeansGogusUcu.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGogusUcu.Text) || txtSeansGogusUcu.Text == "0"))
                seanslar.Add("Göğüs ucu hizmeti için seans sayısı");
            if (!checkBoxGogusUcu.Checked && !string.IsNullOrWhiteSpace(txtSeansGogusUcu.Text))
                seanslar.Add("Göğüs ucu hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxOzelBolge.Checked && (!int.TryParse(txtSeansOzelBolge.Text, out _) || string.IsNullOrWhiteSpace(txtSeansOzelBolge.Text) || txtSeansOzelBolge.Text == "0"))
                seanslar.Add("Özel bölge hizmeti için seans sayısı");
            if (!checkBoxOzelBolge.Checked && !string.IsNullOrWhiteSpace(txtSeansOzelBolge.Text))
                seanslar.Add("Özel bölge hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxEnse.Checked && (!int.TryParse(txtSeansEnse.Text, out _) || string.IsNullOrWhiteSpace(txtSeansEnse.Text) || txtSeansEnse.Text == "0"))
                seanslar.Add("Ense hizmeti için seans sayısı");
            if (!checkBoxEnse.Checked && !string.IsNullOrWhiteSpace(txtSeansEnse.Text))
                seanslar.Add("Ense hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxYanak.Checked && (!int.TryParse(txtSeansYanak.Text, out _) || string.IsNullOrWhiteSpace(txtSeansYanak.Text) || txtSeansYanak.Text == "0"))
                seanslar.Add("Yanak hizmeti için seans sayısı");
            if (!checkBoxYanak.Checked && !string.IsNullOrWhiteSpace(txtSeansYanak.Text))
                seanslar.Add("Yanak hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxBoyun.Checked && (!int.TryParse(txtSeansBoyun.Text, out _) || string.IsNullOrWhiteSpace(txtSeansBoyun.Text) || txtSeansBoyun.Text == "0"))
                seanslar.Add("Boyun hizmeti için seans sayısı");
            if (!checkBoxBoyun.Checked && !string.IsNullOrWhiteSpace(txtSeansBoyun.Text))
                seanslar.Add("Boyun hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxKasUstuOrtasi.Checked && (!int.TryParse(txtSeansKasUstuOrtasi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansKasUstuOrtasi.Text) || txtSeansKasUstuOrtasi.Text == "0"))
                seanslar.Add("Kaş üstü-ortası hizmeti için seans sayısı");
            if (!checkBoxKasUstuOrtasi.Checked && !string.IsNullOrWhiteSpace(txtSeansKasUstuOrtasi.Text))
                seanslar.Add("Kaş üstü-ortası hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxKulak.Checked && (!int.TryParse(txtSeansKulak.Text, out _) || string.IsNullOrWhiteSpace(txtSeansKulak.Text) || txtSeansKulak.Text == "0"))
                seanslar.Add("Kulak hizmeti için seans sayısı");
            if (!checkBoxKulak.Checked && !string.IsNullOrWhiteSpace(txtSeansKulak.Text))
                seanslar.Add("Kulak hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxOmuz.Checked && (!int.TryParse(txtSeansOmuz.Text, out _) || string.IsNullOrWhiteSpace(txtSeansOmuz.Text) || txtSeansOmuz.Text == "0"))
                seanslar.Add("Omuz hizmeti için seans sayısı");
            if (!checkBoxOmuz.Checked && !string.IsNullOrWhiteSpace(txtSeansOmuz.Text))
                seanslar.Add("Omuz hizmeti için kutucuk işaretlenmedi.");
            #endregion

            #region Zayıflama seans sayıları kontrolü
            if (checkBoxEMSBodyPro.Checked && (!int.TryParse(txtSeansEMSBodyPro.Text, out _) || string.IsNullOrWhiteSpace(txtSeansEMSBodyPro.Text) || txtSeansEMSBodyPro.Text == "0"))
                seanslar.Add("EMS Body Pro hizmeti için seans sayısı");
            if (!checkBoxEMSBodyPro.Checked && !string.IsNullOrWhiteSpace(txtSeansEMSBodyPro.Text))
                seanslar.Add("EMS Body Pro için kutucuk işaretlenmedi.");

            if (checkBoxLeafDrenaji.Checked && (!int.TryParse(txtSeansLeafDrenaji.Text, out _) || string.IsNullOrWhiteSpace(txtSeansLeafDrenaji.Text) || txtSeansLeafDrenaji.Text == "0"))
                seanslar.Add("Leaf Drenaji hizmeti için seans sayısı");
            if (!checkBoxLeafDrenaji.Checked && !string.IsNullOrWhiteSpace(txtSeansLeafDrenaji.Text))
                seanslar.Add("Leaf Drenaji hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxPasifJimnastik.Checked && (!int.TryParse(txtSeansPasifJimnastik.Text, out _) || string.IsNullOrWhiteSpace(txtSeansPasifJimnastik.Text) || txtSeansPasifJimnastik.Text == "0"))
                seanslar.Add("Pasif Jimnastik hizmeti için seans sayısı");
            if (!checkBoxPasifJimnastik.Checked && !string.IsNullOrWhiteSpace(txtSeansPasifJimnastik.Text))
                seanslar.Add("Pasif Jimnastik için kutucuk işaretlenmedi.");

            if (checkBoxG5Masaji.Checked && (!int.TryParse(txtSeansG5Masajı.Text, out _) || string.IsNullOrWhiteSpace(txtSeansG5Masajı.Text) || txtSeansG5Masajı.Text == "0"))
                seanslar.Add("G5 Masajı hizmeti için seans sayısı");
            if (!checkBoxG5Masaji.Checked && !string.IsNullOrWhiteSpace(txtSeansG5Masajı.Text))
                seanslar.Add("G5 Masajı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxMagicSound.Checked && (!int.TryParse(txtSeansMagicSound.Text, out _) || string.IsNullOrWhiteSpace(txtSeansMagicSound.Text) || txtSeansMagicSound.Text == "0"))
                seanslar.Add("Magic Sound hizmeti için seans sayısı");
            if (!checkBoxMagicSound.Checked && !string.IsNullOrWhiteSpace(txtSeansMagicSound.Text))
                seanslar.Add("Magic Sound için kutucuk işaretlenmedi.");
            #endregion

            #region Bakım seans sayıları kontrolü
            if (checkBoxKlasikCiltBakimi.Checked && (!int.TryParse(txtSeansKlasikCiltBakimi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansKlasikCiltBakimi.Text) || txtSeansKlasikCiltBakimi.Text == "0"))
                seanslar.Add("Klasik Cilt Bakımı hizmeti için seans sayısı");
            if (!checkBoxKlasikCiltBakimi.Checked && !string.IsNullOrWhiteSpace(txtSeansKlasikCiltBakimi.Text))
                seanslar.Add("Klasik Cilt Bakımı için kutucuk işaretlenmedi.");

            if (checkBoxHydrafacialCiltBakimi.Checked && (!int.TryParse(txtSeansHydrafacialCiltBakimi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansHydrafacialCiltBakimi.Text) || txtSeansHydrafacialCiltBakimi.Text == "0"))
                seanslar.Add("Hydrafacial Cilt Bakımı hizmeti için seans sayısı");
            if (!checkBoxHydrafacialCiltBakimi.Checked && !string.IsNullOrWhiteSpace(txtSeansHydrafacialCiltBakimi.Text))
                seanslar.Add("Hydrafacial Cilt Bakımı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxGoldRenovationARCSystemRepair.Checked && (!int.TryParse(txtSeansGoldRenovationARCSystemRepair.Text, out _) || string.IsNullOrWhiteSpace(txtSeansGoldRenovationARCSystemRepair.Text) || txtSeansGoldRenovationARCSystemRepair.Text == "0"))
                seanslar.Add("Gold Renovation ARC hizmeti için seans sayısı");
            if (!checkBoxGoldRenovationARCSystemRepair.Checked && !string.IsNullOrWhiteSpace(txtSeansGoldRenovationARCSystemRepair.Text))
                seanslar.Add("Gold Renovation ARC hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxTirnakBakimi.Checked && (!int.TryParse(txtSeansTirnakBakimi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansTirnakBakimi.Text) || txtSeansTirnakBakimi.Text == "0"))
                seanslar.Add("Tırnak Bakımı hizmeti için seans sayısı");
            if (!checkBoxTirnakBakimi.Checked && !string.IsNullOrWhiteSpace(txtSeansTirnakBakimi.Text))
                seanslar.Add("Tırnak Bakımı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxCatlakBakimi.Checked && (!int.TryParse(txtSeansCatlakBakimi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansCatlakBakimi.Text) || txtSeansCatlakBakimi.Text == "0"))
                seanslar.Add("Çatlak Bakımı hizmeti için seans sayısı");
            if (!checkBoxCatlakBakimi.Checked && !string.IsNullOrWhiteSpace(txtSeansCatlakBakimi.Text))
                seanslar.Add("Çatlak Bakımı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxYosunPeeling.Checked && (!int.TryParse(txtSeansYosunPeeling.Text, out _) || string.IsNullOrWhiteSpace(txtSeansYosunPeeling.Text) || txtSeansYosunPeeling.Text == "0"))
                seanslar.Add("Yosun Peeling Bakımı hizmeti için seans sayısı");
            if (!checkBoxYosunPeeling.Checked && !string.IsNullOrWhiteSpace(txtSeansYosunPeeling.Text))
                seanslar.Add("Yosun Peeling Bakımı hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxCollagenIpProtokolu.Checked && (!int.TryParse(txtSeansCollagenIpProtokolu.Text, out _) || string.IsNullOrWhiteSpace(txtSeansCollagenIpProtokolu.Text) || txtSeansCollagenIpProtokolu.Text == "0"))
                seanslar.Add("Collagen İp Protokolü hizmeti için seans sayısı");
            if (!checkBoxCollagenIpProtokolu.Checked && !string.IsNullOrWhiteSpace(txtSeansCollagenIpProtokolu.Text))
                seanslar.Add("Collagen İp Protokolü hizmeti için kutucuk işaretlenmedi.");

            if (checkBoxElUstuBakimi.Checked && (!int.TryParse(txtSeansElUstuBakimi.Text, out _) || string.IsNullOrWhiteSpace(txtSeansElUstuBakimi.Text) || txtSeansElUstuBakimi.Text == "0")   )
                seanslar.Add("El Üstü Bakımı hizmeti için seans sayısı");
            if (!checkBoxElUstuBakimi.Checked && !string.IsNullOrWhiteSpace(txtSeansElUstuBakimi.Text))
                seanslar.Add("El Üstü Bakımı hizmeti için kutucuk işaretlenmedi.");

            #endregion

            // Eğer eksik bilgi varsa, kullanıcıya bildir
            if (seanslar.Count > 0)
            {
                MessageBox.Show("Lütfen aşağıdaki hizmetler için gerekli bilgileri giriniz:\n" + string.Join("\n", seanslar));
                 return false;
            }
            return true;
            
         
        }

        private bool BilgilerGecerliMi()
        {
            // Ad, soyad, telefon ve ödenecek tutar alanlarının dolu olup olmadığını kontrol et.
            if (string.IsNullOrWhiteSpace(txtMusteriAdi.Text))
            {
                MessageBox.Show("Ad alanı boş bırakılamaz!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMusteriSoyadi.Text))
            {
                MessageBox.Show("Soyad alanı boş bırakılamaz!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefonNumarasi.Text))
            {
                MessageBox.Show("Telefon numarası alanı boş bırakılamaz!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMusteriOdenecekTutari.Text))
            {
                MessageBox.Show("Ödenecek tutar alanı boş bırakılamaz!");
                return false;
            }

            // Telefon numarasının geçerli bir sayı olup olmadığını kontrol et.
            if (!Regex.IsMatch(txtTelefonNumarasi.Text, @"^\d+$"))
            {
                MessageBox.Show("Telefon numarası yalnızca sayılardan oluşmalıdır.");
                return false;
            }

            // Ödenecek tutarın sayısal bir değer olduğundan emin ol
            if (!decimal.TryParse(txtMusteriOdenecekTutari.Text, out _))
            {
                MessageBox.Show("Ödenecek tutar sayısal bir değer olmalıdır.");
                return false;
            }

            return true; // Tüm kontrollerden geçtiyse true döndür.
        }
        #endregion
        private void MusteriEkleBtn_Click(object sender, EventArgs e)//kaydet
        {
            if (!BilgilerGecerliMi())
            {
                return;  // Eğer bilgiler geçerli değilse, daha fazla işlem yapmadan çık.
            }
            if (!Uyarılar())
            {
                return;  // Uyarılarda hata varsa işlemi sonlandır
            }

            SqlCommand command = new SqlCommand();
            SqlConnection baglanti = new SqlConnection(conString);
            baglanti.Open();

            // Önce müşterinin adı ve soyadı ile veritabanında olup olmadığını kontrol et
            string checkIfMusteriExistsQuery = @"
            SELECT IsActive FROM Musteri WHERE Ad = @musteriAd AND Soyad = @musteriSoyad";
            using (SqlCommand checkIfMusteriExistsCommand = new SqlCommand(checkIfMusteriExistsQuery, baglanti))
            {
                checkIfMusteriExistsCommand.Parameters.AddWithValue("@musteriAd", txtMusteriAdi.Text);
                checkIfMusteriExistsCommand.Parameters.AddWithValue("@musteriSoyad", txtMusteriSoyadi.Text);

                object isActive = checkIfMusteriExistsCommand.ExecuteScalar();
                if (isActive != null)
                {
                    if ((bool)isActive)
                    {
                        MessageBox.Show("Bu isme ve soyisme sahip bir kullanıcı zaten var.");
                    }
                    else
                    {
                        MessageBox.Show("Bu isme ve soyisme sahip bir müşteri pasifleştirilmiş durumda.");
                    }
                    baglanti.Close();
                    return; // Metodu burada sonlandır
                }
            }
            string kontrolSorgusu = "SELECT COUNT(*) FROM Musteri WHERE Telefon = @Telefon";
            using (SqlCommand kontrolKomutu = new SqlCommand(kontrolSorgusu, baglanti))
            {
                kontrolKomutu.Parameters.AddWithValue("@Telefon", txtTelefonNumarasi.Text);
                int kayitSayisi = (int)kontrolKomutu.ExecuteScalar();

                if (kayitSayisi > 0)
                {
                    // Telefon numarası daha önce kullanılmış
                    MessageBox.Show("Bu telefon numarasıyla kayıtlı bir müşteri zaten var. Eğer aynı numarayı kullanıyorsanız lütfen numaranın başına '0' ekleyiniz");
                    return; // İşlemi burada sonlandır
                }
            }

            if (!Regex.IsMatch(txtTelefonNumarasi.Text, @"^\d+$"))
            {
                MessageBox.Show("Telefon numarası yalnızca sayılardan oluşmalıdır.");
                return; // Eğer telefon numarası sadece sayılardan oluşmuyorsa işlemi durdur
            }

            //telefon karakter sayisi hatasını onlemek için
            if (txtTelefonNumarasi.Text.Length > 14)
            {
                MessageBox.Show("Telefon için karakter sınırını geçtiniz.");
                return; // Metodu sonlandır ve daha fazla işlem yapma
            }


            // Ödenecek için kontroller (maximum değer ve string ifader hata mesajı )
            if (decimal.TryParse(txtMusteriOdenecekTutari.Text, out decimal odenecekTutar))
            {
                // Odenecek tutarın maksimum sınırları kontrol et
                if (odenecekTutar > 99999999.99m)
                {
                    MessageBox.Show("Girilen tutar maksimum sınırları aşıyor. Lütfen daha küçük bir tutar giriniz.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Girilen ödenecek tutarı sayısal değer olarak giriniz.");
                return;
            }






            command.Connection = baglanti; // Set the connection for the command

            command.CommandText = "INSERT INTO Musteri (Ad, Soyad, Telefon) VALUES(@Ad, @Soyad, @Telefon)";
            command.Parameters.AddWithValue("@Ad", txtMusteriAdi.Text);
            command.Parameters.AddWithValue("@Soyad", txtMusteriSoyadi.Text);
            command.Parameters.AddWithValue("@Telefon", txtTelefonNumarasi.Text);
            command.Parameters.AddWithValue("@Odenecek", txtMusteriOdenecekTutari.Text);

            command.ExecuteNonQuery();

            command.Parameters.Clear();

            command.CommandText = "SELECT MusteriID FROM Musteri WHERE Telefon = @telefon";
            command.Parameters.AddWithValue("@telefon", txtTelefonNumarasi.Text);

            object result = command.ExecuteScalar();
            int customerId = 0;
            if (result != null)
            {
                customerId = Convert.ToInt32(result);
            }

            #region Epilasyon Seçimleri
            Dictionary<int, int> bolgelerSeanslari = new Dictionary<int, int>();

            if (checkBoxCene.Checked)
            {
                int cene = Convert.ToInt32(checkBoxCene.Tag);
                selectedBolgeIDs.Add(cene);
                int ceneSeans = string.IsNullOrWhiteSpace(txtSeansCene.Text) ? 0 : Convert.ToInt32(txtSeansCene.Text);
                bolgelerSeanslari.Add(cene, ceneSeans);
            }
            if (checkBoxBiyik.Checked)
            {
                int biyik = Convert.ToInt32(checkBoxBiyik.Tag);
                selectedBolgeIDs.Add(biyik);
                int biyikSeans = string.IsNullOrWhiteSpace(txtSeansBiyik.Text) ? 0 : Convert.ToInt32(txtSeansBiyik.Text);
                bolgelerSeanslari.Add(biyik, biyikSeans);
            }
            if (checkBoxCeneBiyik.Checked)
            {
                int ceneBiyik = Convert.ToInt32(checkBoxCeneBiyik.Tag);
                selectedBolgeIDs.Add(ceneBiyik);
                int ceneBiyikSeans = string.IsNullOrWhiteSpace(txtSeansCeneBiyik.Text) ? 0 : Convert.ToInt32(txtSeansCeneBiyik.Text);
                bolgelerSeanslari.Add(ceneBiyik, ceneBiyikSeans);
            }
            if (checkBoxYuz.Checked)
            {
                int yuz = Convert.ToInt32(checkBoxYuz.Tag);
                selectedBolgeIDs.Add(yuz);
                int yuzSeans = string.IsNullOrWhiteSpace(txtSeansYuz.Text) ? 0 : Convert.ToInt32(txtSeansYuz.Text);
                bolgelerSeanslari.Add(yuz, yuzSeans);
            }
            if (checkBoxFaul.Checked)
            {
                int faul = Convert.ToInt32(checkBoxFaul.Tag);
                selectedBolgeIDs.Add(faul);
                int faulSeans = string.IsNullOrWhiteSpace(txtSeansFaul.Text) ? 0 : Convert.ToInt32(txtSeansFaul.Text);
                bolgelerSeanslari.Add(faul, faulSeans);
            }
            if (checkBoxGenital.Checked)
            {
                int genital = Convert.ToInt32(checkBoxGenital.Tag);
                selectedBolgeIDs.Add(genital);
                int genitalSeans = string.IsNullOrWhiteSpace(txtSeansGenital.Text) ? 0 : Convert.ToInt32(txtSeansGenital.Text);
                bolgelerSeanslari.Add(genital, genitalSeans);
            }
            if (checkBoxTumKol.Checked)
            {
                int tumKol = Convert.ToInt32(checkBoxTumKol.Tag);
                selectedBolgeIDs.Add(tumKol);
                int tumKolSeans = string.IsNullOrWhiteSpace(txtSeansTumKol.Text) ? 0 : Convert.ToInt32(txtSeansTumKol.Text);
                bolgelerSeanslari.Add(tumKol, tumKolSeans);
            }
            if (checkBoxTumBacak.Checked)
            {
                int tumBacak = Convert.ToInt32(checkBoxTumBacak.Tag);
                selectedBolgeIDs.Add(tumBacak);
                int tumBacakSeans = string.IsNullOrWhiteSpace(txtSeansTumBacak.Text) ? 0 : Convert.ToInt32(txtSeansTumBacak.Text);
                bolgelerSeanslari.Add(tumBacak, tumBacakSeans);
            }
            if (checkBoxGobek.Checked)
            {
                int gobek = Convert.ToInt32(checkBoxGobek.Tag);
                selectedBolgeIDs.Add(gobek);
                int gobekSeans = string.IsNullOrWhiteSpace(txtSeansGobek.Text) ? 0 : Convert.ToInt32(txtSeansGobek.Text);
                bolgelerSeanslari.Add(gobek, gobekSeans);
            }
            if (checkBoxSirtKomple.Checked)
            {
                int sirtKomple = Convert.ToInt32(checkBoxSirtKomple.Tag);
                selectedBolgeIDs.Add(sirtKomple);
                int sirtKompleSeans = string.IsNullOrWhiteSpace(txtSeansSirtKomple.Text) ? 0 : Convert.ToInt32(txtSeansSirtKomple.Text);
                bolgelerSeanslari.Add(sirtKomple, sirtKompleSeans);

            }
            if (checkBoxKolAlti.Checked)
            {
                int kolAlti = Convert.ToInt32(checkBoxKolAlti.Tag);
                selectedBolgeIDs.Add(kolAlti);
                int kolAltiSeans = string.IsNullOrWhiteSpace(txtSeansKolAlti.Text) ? 0 : Convert.ToInt32(txtSeansKolAlti.Text);
                bolgelerSeanslari.Add(kolAlti, kolAltiSeans);
            }

            if (checkBoxPopo.Checked)
            {
                int popo = Convert.ToInt32(checkBoxPopo.Tag);
                selectedBolgeIDs.Add(popo);
                int popoSeans = string.IsNullOrWhiteSpace(txtSeansPopo.Text) ? 0 : Convert.ToInt32(txtSeansPopo.Text);
                bolgelerSeanslari.Add(popo, popoSeans);
            }

            if (checkBoxGobekCizgisi.Checked)
            {
                int gobekCizgisi = Convert.ToInt32(checkBoxGobekCizgisi.Tag);
                selectedBolgeIDs.Add(gobekCizgisi);
                int gobekCizgisiSeans = string.IsNullOrWhiteSpace(txtSeansGobekCizgisi.Text) ? 0 : Convert.ToInt32(txtSeansGobekCizgisi.Text);
                bolgelerSeanslari.Add(gobekCizgisi, gobekCizgisiSeans);
            }
            if (checkBoxYarimBacak.Checked)
            {
                int yarimBacak = Convert.ToInt32(checkBoxYarimBacak.Tag);
                selectedBolgeIDs.Add(yarimBacak);
                int yarimBacakSeans = string.IsNullOrWhiteSpace(txtSeansYarimBacak.Text) ? 0 : Convert.ToInt32(txtSeansYarimBacak.Text);
                bolgelerSeanslari.Add(yarimBacak, yarimBacakSeans);
            }

            if (checkBoxYarimKol.Checked)
            {
                int yarimKol = Convert.ToInt32(checkBoxYarimKol.Tag);
                selectedBolgeIDs.Add(yarimKol);
                int yarimKolSeans = string.IsNullOrWhiteSpace(txtSeansYarimKol.Text) ? 0 : Convert.ToInt32(txtSeansYarimKol.Text);
                bolgelerSeanslari.Add(yarimKol, yarimKolSeans);
            }

            if (checkBoxBel.Checked)
            {
                int bel = Convert.ToInt32(checkBoxBel.Tag);
                selectedBolgeIDs.Add(bel);
                int belSeans = string.IsNullOrWhiteSpace(txtSeansBel.Text) ? 0 : Convert.ToInt32(txtSeansBel.Text);
                bolgelerSeanslari.Add(bel, belSeans);
            }

            if (checkBoxGogusKomple.Checked)
            {
                int göğüsKomple = Convert.ToInt32(checkBoxGogusKomple.Tag);
                selectedBolgeIDs.Add(göğüsKomple);
                int göğüsKompleSeans = string.IsNullOrWhiteSpace(txtSeansGogusKomple.Text) ? 0 : Convert.ToInt32(txtSeansGogusKomple.Text);
                bolgelerSeanslari.Add(göğüsKomple, göğüsKompleSeans);
            }

            if (checkBoxGogusArasi.Checked)
            {
                int göğüsArası = Convert.ToInt32(checkBoxGogusArasi.Tag);
                selectedBolgeIDs.Add(göğüsArası);
                int göğüsArasıSeans = string.IsNullOrWhiteSpace(txtSeansGogusArasi.Text) ? 0 : Convert.ToInt32(txtSeansGogusArasi.Text);
                bolgelerSeanslari.Add(göğüsArası, göğüsArasıSeans);
            }

            if (checkBoxGogusUcu.Checked)
            {
                int göğüsUcu = Convert.ToInt32(checkBoxGogusUcu.Tag);
                selectedBolgeIDs.Add(göğüsUcu);
                int göğüsUcuSeans = string.IsNullOrWhiteSpace(txtSeansGogusUcu.Text) ? 0 : Convert.ToInt32(txtSeansGogusUcu.Text);
                bolgelerSeanslari.Add(göğüsUcu, göğüsUcuSeans);
            }

            if (checkBoxOzelBolge.Checked)
            {
                int özelBölge = Convert.ToInt32(checkBoxOzelBolge.Tag);
                selectedBolgeIDs.Add(özelBölge);
                int özelBölgeSeans = string.IsNullOrWhiteSpace(txtSeansOzelBolge.Text) ? 0 : Convert.ToInt32(txtSeansOzelBolge.Text);
                bolgelerSeanslari.Add(özelBölge, özelBölgeSeans);
            }

            if (checkBoxEnse.Checked)
            {
                int ense = Convert.ToInt32(checkBoxEnse.Tag);
                selectedBolgeIDs.Add(ense);
                int enseSeans = string.IsNullOrWhiteSpace(txtSeansEnse.Text) ? 0 : Convert.ToInt32(txtSeansEnse.Text);
                bolgelerSeanslari.Add(ense, enseSeans);
            }

            if (checkBoxYanak.Checked)
            {
                int yanak = Convert.ToInt32(checkBoxYanak.Tag);
                selectedBolgeIDs.Add(yanak);
                int yanakSeans = string.IsNullOrWhiteSpace(txtSeansYanak.Text) ? 0 : Convert.ToInt32(txtSeansYanak.Text);
                bolgelerSeanslari.Add(yanak, yanakSeans);
            }

            if (checkBoxBoyun.Checked)
            {
                int boyun = Convert.ToInt32(checkBoxBoyun.Tag);
                selectedBolgeIDs.Add(boyun);
                int boyunSeans = string.IsNullOrWhiteSpace(txtSeansBoyun.Text) ? 0 : Convert.ToInt32(txtSeansBoyun.Text);
                bolgelerSeanslari.Add(boyun, boyunSeans);
            }

            if (checkBoxKasUstuOrtasi.Checked)
            {
                int kaşÜstüOrtası = Convert.ToInt32(checkBoxKasUstuOrtasi.Tag);
                selectedBolgeIDs.Add(kaşÜstüOrtası);
                int kaşÜstüOrtasıSeans = string.IsNullOrWhiteSpace(txtSeansKasUstuOrtasi.Text) ? 0 : Convert.ToInt32(txtSeansKasUstuOrtasi.Text);
                bolgelerSeanslari.Add(kaşÜstüOrtası, kaşÜstüOrtasıSeans);
            }

            if (checkBoxKulak.Checked)
            {
                int kulak = Convert.ToInt32(checkBoxKulak.Tag);
                selectedBolgeIDs.Add(kulak);
                int kulakSeans = string.IsNullOrWhiteSpace(txtSeansKulak.Text) ? 0 : Convert.ToInt32(txtSeansKulak.Text);
                bolgelerSeanslari.Add(kulak, kulakSeans);
            }

            if (checkBoxOmuz.Checked)
            {
                int omuz = Convert.ToInt32(checkBoxOmuz.Tag);
                selectedBolgeIDs.Add(omuz);
                int omuzSeans = string.IsNullOrWhiteSpace(txtSeansOmuz.Text) ? 0 : Convert.ToInt32(txtSeansOmuz.Text);
                bolgelerSeanslari.Add(omuz, omuzSeans);
            }
            foreach (var item in bolgelerSeanslari)
            {
                if (item.Value > 0)
                {  // Eğer seans sayısı 0'dan büyükse
                    command.CommandText = "INSERT INTO MusteriBolge (MusteriBolgeID, BolgeId, BolgeSeans) VALUES(@BolgeMusteriID, @BolgeId, @BolgeSeans)";
                    command.Parameters.AddWithValue("@BolgeMusteriID", customerId);
                    command.Parameters.AddWithValue("@BolgeId", item.Key);
                    command.Parameters.AddWithValue("@BolgeSeans", item.Value);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                else
                {  // Eğer seans sayısı yoksa veya 0 ise
                    command.CommandText = "INSERT INTO MusteriBolge (MusteriBolgeID, BolgeId) VALUES(@BolgeMusteriID, @BolgeId)";
                    command.Parameters.AddWithValue("@BolgeMusteriID", customerId);
                    command.Parameters.AddWithValue("@BolgeId", item.Key);


                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            #endregion


            #region Cilt Bakımı Seçimleri
            Dictionary<int, int> ciltBakimiVeSeanslar = new Dictionary<int, int>();

            if (checkBoxKlasikCiltBakimi.Checked)
            {
                int emsBodyProID = Convert.ToInt32(checkBoxKlasikCiltBakimi.Tag);
                selectedCiltBakimiIDs.Add(emsBodyProID);
                int emsBodyProSeans = string.IsNullOrWhiteSpace(txtSeansKlasikCiltBakimi.Text) ? 0 : Convert.ToInt32(txtSeansKlasikCiltBakimi.Text);
                ciltBakimiVeSeanslar.Add(emsBodyProID, emsBodyProSeans);
            }

            if (checkBoxHydrafacialCiltBakimi.Checked)
            {
                int hydraFacialID = Convert.ToInt32(checkBoxHydrafacialCiltBakimi.Tag);
                selectedCiltBakimiIDs.Add(hydraFacialID);
                int hydraFacialSeans = string.IsNullOrWhiteSpace(txtSeansHydrafacialCiltBakimi.Text) ? 0 : Convert.ToInt32(txtSeansHydrafacialCiltBakimi.Text);
                ciltBakimiVeSeanslar.Add(hydraFacialID, hydraFacialSeans);
            }

            if (checkBoxGoldRenovationARCSystemRepair.Checked)
            {
                int grarcsID = Convert.ToInt32(checkBoxGoldRenovationARCSystemRepair.Tag);
                selectedCiltBakimiIDs.Add(grarcsID);
                int grarcsSeans = string.IsNullOrWhiteSpace(txtSeansGoldRenovationARCSystemRepair.Text) ? 0 : Convert.ToInt32(txtSeansGoldRenovationARCSystemRepair.Text);
                ciltBakimiVeSeanslar.Add(grarcsID, grarcsSeans);
            }
            if (checkBoxTirnakBakimi.Checked)
            {
                int tirnakBakimiID = Convert.ToInt32(checkBoxTirnakBakimi.Tag);
                selectedCiltBakimiIDs.Add(tirnakBakimiID);
                int tirnakBakimiSeans = string.IsNullOrWhiteSpace(txtSeansTirnakBakimi.Text) ? 0 : Convert.ToInt32(txtSeansTirnakBakimi.Text);
                ciltBakimiVeSeanslar.Add(tirnakBakimiID, tirnakBakimiSeans);
            }
            if (checkBoxCatlakBakimi.Checked)
            {
                int catlakBakimiID = Convert.ToInt32(checkBoxCatlakBakimi.Tag);
                selectedCiltBakimiIDs.Add(catlakBakimiID);
                int catlakBakimiSeans = string.IsNullOrWhiteSpace(txtSeansCatlakBakimi.Text) ? 0 : Convert.ToInt32(txtSeansCatlakBakimi.Text);
                ciltBakimiVeSeanslar.Add(catlakBakimiID, catlakBakimiSeans);
            }

            if (checkBoxYosunPeeling.Checked)
            {
                int yosunPeelingID = Convert.ToInt32(checkBoxYosunPeeling.Tag);
                selectedCiltBakimiIDs.Add(yosunPeelingID);
                int yosunPeelingSeans = string.IsNullOrWhiteSpace(txtSeansYosunPeeling.Text) ? 0 : Convert.ToInt32(txtSeansYosunPeeling.Text);
                ciltBakimiVeSeanslar.Add(yosunPeelingID, yosunPeelingSeans);
            }

            if (checkBoxCollagenIpProtokolu.Checked)
            {
                int collagenIpProtokoluID = Convert.ToInt32(checkBoxCollagenIpProtokolu.Tag);
                selectedCiltBakimiIDs.Add(collagenIpProtokoluID);
                int collagenIpProtokoluSeans = string.IsNullOrWhiteSpace(txtSeansCollagenIpProtokolu.Text) ? 0 : Convert.ToInt32(txtSeansCollagenIpProtokolu.Text);
                ciltBakimiVeSeanslar.Add(collagenIpProtokoluID, collagenIpProtokoluSeans);
            }

            if (checkBoxElUstuBakimi.Checked)
            {
                int elUstunuBakimiID = Convert.ToInt32(checkBoxElUstuBakimi.Tag);
                selectedCiltBakimiIDs.Add(elUstunuBakimiID);
                int elUstunuBakimiSeans = string.IsNullOrWhiteSpace(txtSeansElUstuBakimi.Text) ? 0 : Convert.ToInt32(txtSeansElUstuBakimi.Text);
                ciltBakimiVeSeanslar.Add(elUstunuBakimiID, elUstunuBakimiSeans);
            }

            // Veritabanına cilt bakımı kayıtlarını ekleme
            foreach (var bakim in ciltBakimiVeSeanslar)
            {
                if (bakim.Value > 0)
                {
                    command.CommandText = "INSERT INTO MusteriCiltBakimi (MusteriCiltBakimiID, CiltBakimiID, CiltBakimiSeans) VALUES(@MusteriID, @CiltBakimiID, @CiltBakimiSeans)";
                    command.Parameters.AddWithValue("@MusteriID", customerId);
                    command.Parameters.AddWithValue("@CiltBakimiID", bakim.Key);
                    command.Parameters.AddWithValue("@CiltBakimiSeans", bakim.Value);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            #endregion


            #region Zayiflama Seçimleri
            Dictionary<int, int> zayiflamaVeSeanslar = new Dictionary<int, int>();

            if (checkBoxEMSBodyPro.Checked)
            {
                int emsBodyProID = Convert.ToInt32(checkBoxEMSBodyPro.Tag);
                selectedZayiflamaIDs.Add(emsBodyProID);
                int emsBodyProSeans = string.IsNullOrWhiteSpace(txtSeansEMSBodyPro.Text) ? 0 : Convert.ToInt32(txtSeansEMSBodyPro.Text);
                zayiflamaVeSeanslar.Add(emsBodyProID, emsBodyProSeans);
            }

            if (checkBoxLeafDrenaji.Checked)
            {
                int leafDrenajiID = Convert.ToInt32(checkBoxLeafDrenaji.Tag);
                selectedZayiflamaIDs.Add(leafDrenajiID);
                int leafDrenajiSeans = string.IsNullOrWhiteSpace(txtSeansLeafDrenaji.Text) ? 0 : Convert.ToInt32(txtSeansLeafDrenaji.Text);
                zayiflamaVeSeanslar.Add(leafDrenajiID, leafDrenajiSeans);
            }

            if (checkBoxPasifJimnastik.Checked)
            {
                int pasifJimnastikID = Convert.ToInt32(checkBoxPasifJimnastik.Tag);
                selectedZayiflamaIDs.Add(pasifJimnastikID);
                int pasifJimnastikSeans = string.IsNullOrWhiteSpace(txtSeansPasifJimnastik.Text) ? 0 : Convert.ToInt32(txtSeansPasifJimnastik.Text);
                zayiflamaVeSeanslar.Add(pasifJimnastikID, pasifJimnastikSeans);
            }


            if (checkBoxG5Masaji.Checked)
            {
                int g5MasajiID = Convert.ToInt32(checkBoxG5Masaji.Tag);
                selectedZayiflamaIDs.Add(g5MasajiID);
                int g5MasajiSeans = string.IsNullOrWhiteSpace(txtSeansG5Masajı.Text) ? 0 : Convert.ToInt32(txtSeansG5Masajı.Text);
                zayiflamaVeSeanslar.Add(g5MasajiID, g5MasajiSeans);
            }


            if (checkBoxMagicSound.Checked)
            {
                int MagicSoundID = Convert.ToInt32(checkBoxMagicSound.Tag);
                selectedZayiflamaIDs.Add(MagicSoundID);
                int magicSoundSeans = string.IsNullOrWhiteSpace(txtSeansMagicSound.Text) ? 0 : Convert.ToInt32(txtSeansMagicSound.Text);
                zayiflamaVeSeanslar.Add(MagicSoundID, magicSoundSeans);
            }

            // Zayiflama kayıtlarını ekleme
            foreach (var zayiflama in zayiflamaVeSeanslar)
            {
                if (zayiflama.Value > 0)
                {
                    command.CommandText = "INSERT INTO MusteriZayiflama (MusteriZayiflamaID, ZayiflamaID, ZayiflamaSeans) VALUES(@MusteriID, @ZayiflamaID, @ZayiflamaSeans)";
                    command.Parameters.AddWithValue("@MusteriID", customerId);
                    command.Parameters.AddWithValue("@ZayiflamaID", zayiflama.Key);
                    command.Parameters.AddWithValue("@ZayiflamaSeans", zayiflama.Value);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            #endregion

            MessageBox.Show("Müşteri başarıyla eklendi.");

            if (!string.IsNullOrWhiteSpace(txtMusteriOdenecekTutari.Text))
            {
                // Tekrar temizle
                command.Parameters.Clear();
                // Yeni bir Fatura kaydı ekleyin, burada Odenen için 0 ve o anın tarihini ekleyin
                command.CommandText = "INSERT INTO Fatura (MusteriFaturaID, Odenecek, Odenen, FaturaTarih) VALUES(@MusteriID, @Odenecek, 0, @FaturaTarih)";
                command.Parameters.AddWithValue("@MusteriID", customerId);
                command.Parameters.AddWithValue("@Odenecek", Convert.ToDecimal(txtMusteriOdenecekTutari.Text));
                // Odenen değeri için 0 ekleyin
                // FaturaTarih için o anın tarihini ekleyin
                command.Parameters.AddWithValue("@FaturaTarih", DateTime.Now);

                command.ExecuteNonQuery();

                // command.Parameters.Clear(); // Gerekli ise parametreleri temizleyin
            }

            baglanti.Close();
            KayitSonuTemizlik();
        }

        private void KayitSonuTemizlik()
        {
            txtMusteriAdi.Text = "";
            txtMusteriOdenecekTutari.Text = "";
            txtMusteriSoyadi.Text = "";
            txtTelefonNumarasi.Text = "";
            TumSeansCheckBoxlariniBosalt();
            TumSeansTextleriniBosalt();
            DataGridTemizlemeVeMusteriGetir();
        }
   

        private void MusteriGuncelleBtn_Click(object sender, EventArgs e)
        {
            if (!BilgilerGecerliMi())
            {
                return;  // Eğer bilgiler geçerli değilse, daha fazla işlem yapmadan çık.
            }

            if (!Uyarılar())
            {
                return;  // Uyarılarda hata varsa işlemi sonlandır
            }
            GuncelleFonksiyon();
            
        }


        #region GÜNCELLEME 
        private void GuncelleFonksiyon()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();


                //Kaydet yerine güncellemeye tıklanırsa...
                string checkIfMusteriExistsQuery = "SELECT COUNT(*) FROM Musteri WHERE MusteriID = @musteriId";
                using (SqlCommand checkIfMusteriExistsCommand = new SqlCommand(checkIfMusteriExistsQuery, connection))
                {
                    checkIfMusteriExistsCommand.Parameters.AddWithValue("@musteriId", gizliId.Text);

                    int result = Convert.ToInt32(checkIfMusteriExistsCommand.ExecuteScalar());
                    if (result == 0)
                    {

                        MessageBox.Show("Böyle bir müşteri bulunamadı, isterseniz kaydedebilirsiniz.");
                        return;
                    }
                }

                //telefon karakter sayisi hatasını onlemek için
                if (txtTelefonNumarasi.Text.Length > 15)
                {
                    MessageBox.Show("Telefon için karakter sınırını geçtiniz.");
                    return; // Metodu sonlandır ve daha fazla işlem yapma
                }

                // Müşteri bilgilerini güncelle
                string updateQuery = "UPDATE Musteri SET Ad = @ad, Soyad = @soyad, Telefon = @telefon WHERE MusteriID = @id";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@id", gizliId.Text);
                    updateCommand.Parameters.AddWithValue("@ad", txtMusteriAdi.Text);
                    updateCommand.Parameters.AddWithValue("@soyad", txtMusteriSoyadi.Text);
                    updateCommand.Parameters.AddWithValue("@telefon", txtTelefonNumarasi.Text);
                    updateCommand.ExecuteNonQuery();
                }

                #region EPILASYON
                // Mevcut epilasyon kayıtlarını al
                List<int> existingBolgeIDs = new List<int>();
                string selectBolgeQuery = "SELECT BolgeId FROM MusteriBolge WHERE MusteriBolgeID = @id";
                using (SqlCommand selectBolgeCommand = new SqlCommand(selectBolgeQuery, connection))
                {
                    selectBolgeCommand.Parameters.AddWithValue("@id", gizliId.Text);
                    using (SqlDataReader reader = selectBolgeCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingBolgeIDs.Add(Convert.ToInt32(reader["BolgeId"]));
                        }
                    }
                }




                // Yeni seçilen epilasyon kayıtlarını al
                List<int> selectedBolgeIDs = new List<int>();
                foreach (System.Windows.Forms.CheckBox checkBox in groupBox1.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    if (checkBox.Checked)
                    {
                        selectedBolgeIDs.Add(Convert.ToInt32(checkBox.Tag));
                    }
                }




                // Mevcut epilasyon kayıtlarını güncelle
                foreach (var bolge in existingBolgeIDs)
                {
                    if (!selectedBolgeIDs.Contains(bolge))
                    {
                        // Mevcut epilasyon kaydını kaldır
                        string deleteQuery = "DELETE FROM MusteriBolge WHERE MusteriBolgeID = @id AND BolgeId = @bolgeId";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@id", gizliId.Text);
                            deleteCommand.Parameters.AddWithValue("@bolgeId", bolge);
                            deleteCommand.ExecuteNonQuery();
                        }
                    }
                }

                //EPILASYON
                foreach (var bolge in selectedBolgeIDs)
                {
                    if (!existingBolgeIDs.Contains(bolge))
                    {
                        // Yeni epilasyon ekle
                        string insertQuery = "INSERT INTO MusteriBolge (MusteriBolgeID, BolgeId) VALUES(@BolgeMusteriID, @BolgeId)";
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@BolgeMusteriID", gizliId.Text);
                            insertCommand.Parameters.AddWithValue("@BolgeId", bolge);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                #endregion

                #region CİLT BAKIMI
                // Mevcut cilt bakımı kayıtlarını al
                List<int> existingCiltBakimiIDs = new List<int>();
                string selectCiltBakimiQuery = "SELECT CiltBakimiID FROM MusteriCiltBakimi WHERE MusteriCiltBakimiID = @id";
                using (SqlCommand selectCiltBakimiCommand = new SqlCommand(selectCiltBakimiQuery, connection))
                {
                    selectCiltBakimiCommand.Parameters.AddWithValue("@id", gizliId.Text);
                    using (SqlDataReader reader = selectCiltBakimiCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingCiltBakimiIDs.Add(Convert.ToInt32(reader["CiltBakimiID"]));
                        }
                    }
                }
                // Yeni seçilen cilt bakımı kayıtlarını al
                List<int> selectedCiltBakimiIDs = new List<int>();
                foreach (System.Windows.Forms.CheckBox checkBox in groupBox2.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    if (checkBox.Checked)
                    {
                        selectedCiltBakimiIDs.Add(Convert.ToInt32(checkBox.Tag));
                    }
                }
                // Mevcut cilt bakımı kayıtlarını güncelle
                foreach (int bakimID in existingCiltBakimiIDs)
                {
                    if (!selectedCiltBakimiIDs.Contains(bakimID))
                    {
                        string deleteCiltBakimiQuery = "DELETE FROM MusteriCiltBakimi WHERE MusteriCiltBakimiID = @id AND CiltBakimiID = @bakimId";
                        using (SqlCommand deleteCiltBakimiCommand = new SqlCommand(deleteCiltBakimiQuery, connection))
                        {
                            deleteCiltBakimiCommand.Parameters.AddWithValue("@id", gizliId.Text);
                            deleteCiltBakimiCommand.Parameters.AddWithValue("@bakimId", bakimID);
                            deleteCiltBakimiCommand.ExecuteNonQuery();
                        }
                    }
                }
                // Yeni cilt bakımı kayıtlarını ekle
                foreach (int bakimID in selectedCiltBakimiIDs)
                {
                    if (!existingCiltBakimiIDs.Contains(bakimID))
                    {
                        string insertCiltBakimiQuery = "INSERT INTO MusteriCiltBakimi (MusteriCiltBakimiID, CiltBakimiID) VALUES(@MusteriID, @CiltBakimiID)";
                        using (SqlCommand insertCiltBakimiCommand = new SqlCommand(insertCiltBakimiQuery, connection))
                        {
                            insertCiltBakimiCommand.Parameters.AddWithValue("@MusteriID", gizliId.Text);
                            insertCiltBakimiCommand.Parameters.AddWithValue("@CiltBakimiID", bakimID);
                            insertCiltBakimiCommand.ExecuteNonQuery();
                        }
                    }
                }
                #endregion

                #region ZAYIFLAMA
                // Mevcut zayıflama kayıtlarını al
                List<int> existingZayiflamaIDs = new List<int>();
                string selectZayiflamaQuery = "SELECT ZayiflamaID FROM MusteriZayiflama WHERE MusteriZayiflamaID = @id";
                using (SqlCommand selectZayiflamaCommand = new SqlCommand(selectZayiflamaQuery, connection))
                {
                    selectZayiflamaCommand.Parameters.AddWithValue("@id", gizliId.Text);
                    using (SqlDataReader reader = selectZayiflamaCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingZayiflamaIDs.Add(Convert.ToInt32(reader["ZayiflamaID"]));
                        }
                    }
                }
                // Yeni seçilen zayıflama kayıtlarını al
                List<int> selectedZayiflamaIDs = new List<int>();
                foreach (System.Windows.Forms.CheckBox checkBox in groupBox3.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    if (checkBox.Checked)
                    {
                        selectedZayiflamaIDs.Add(Convert.ToInt32(checkBox.Tag));
                    }
                }

                // Mevcut zayıflama kayıtlarını güncelle
                foreach (int zayiflama in existingZayiflamaIDs)
                {
                    if (!selectedZayiflamaIDs.Contains(zayiflama))
                    {
                        string deleteZayiflamaQuery = "DELETE FROM MusteriZayiflama WHERE MusteriZayiflamaID = @id AND ZayiflamaID = @zayiflamaId";
                        using (SqlCommand deleteZayiflamaCommand = new SqlCommand(deleteZayiflamaQuery, connection))
                        {
                            deleteZayiflamaCommand.Parameters.AddWithValue("@id", gizliId.Text);
                            deleteZayiflamaCommand.Parameters.AddWithValue("@zayiflamaId", zayiflama);
                            deleteZayiflamaCommand.ExecuteNonQuery();
                        }
                    }
                }


                //ZAYIFLAMA
                foreach (int zayiflama in selectedZayiflamaIDs)
                {
                    if (!existingZayiflamaIDs.Contains(zayiflama))
                    {
                        string insertZayiflamaQuery = "INSERT INTO MusteriZayiflama (MusteriZayiflamaID, ZayiflamaID) VALUES(@MusteriID, @ZayiflamaID)";
                        using (SqlCommand insertZayiflamaCommand = new SqlCommand(insertZayiflamaQuery, connection))
                        {
                            insertZayiflamaCommand.Parameters.AddWithValue("@MusteriID", gizliId.Text);
                            insertZayiflamaCommand.Parameters.AddWithValue("@ZayiflamaID", zayiflama);
                            insertZayiflamaCommand.ExecuteNonQuery();
                        }
                    }
                }

                #endregion


                // Ödenecek için kontroller (maximum değer ve string ifader hata mesajı )
                bool isConversionSuccessful = decimal.TryParse(txtMusteriOdenecekTutari.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal odenecekTutar);

                if (decimal.TryParse(txtMusteriOdenecekTutari.Text, out odenecekTutar))
                {
                    // Odenecek tutarın maksimum sınırları kontrol et
                    if (odenecekTutar > 999999999.99m)
                    {
                        MessageBox.Show("Girilen tutar maksimum sınırları aşıyor. Destek için 0530 150 48 78'i arayınız");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Girilen ödenecek tutarı sayısal değer olarak giriniz.");
                    return;
                }


                int customerId = Convert.ToInt32(gizliId.Text); // Müşteri ID'sini al

                // Mevcut ödenecek tutarı güncelle
                string updateFaturaQuery = "UPDATE Fatura SET Odenecek = @odenecek WHERE MusteriFaturaID = @musteriId";
                using (SqlCommand updateFaturaCommand = new SqlCommand(updateFaturaQuery, connection))
                {
                    updateFaturaCommand.Parameters.AddWithValue("@odenecek", Convert.ToDecimal(txtMusteriOdenecekTutari.Text));
                    updateFaturaCommand.Parameters.AddWithValue("@musteriId", customerId);
                    updateFaturaCommand.ExecuteNonQuery();
                }


                if (decimal.TryParse(txtMusteriOdenecekTutari.Text, out odenecekTutar) && odenecekTutar > 0)
                {
                    // Ödenecek tutar pozitif ise yeni bir fatura kaydı ekle
                    string insertFaturaQuery = "INSERT INTO Fatura (MusteriFaturaID, Odenecek, Odenen,FaturaTarih) VALUES(@MusteriID, @Odenecek, 0,@FaturaTarih)";
                    using (SqlCommand insertFaturaCommand = new SqlCommand(insertFaturaQuery, connection))
                    {
                        insertFaturaCommand.Parameters.AddWithValue("@MusteriID", Convert.ToInt32(gizliId.Text));
                        insertFaturaCommand.Parameters.AddWithValue("@Odenecek", odenecekTutar);
                        insertFaturaCommand.Parameters.AddWithValue("@FaturaTarih", DateTime.Now);
                        insertFaturaCommand.ExecuteNonQuery();
                    }
                }
                SeansGuncelle();
                // Güncelleme işlemi başarılı mesajı göster
                MessageBox.Show("Müşteri bilgileri başarıyla güncellendi.");

                // DataGridView'i güncelle
                DataGridTemizlemeVeMusteriGetir();
            }
        }
        private void SeansGuncelle()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                #region SEANSLARI GÜNCELLE
                int musteriId = Convert.ToInt32(gizliId.Text); // Müşteri ID'sini alın

                //Epilasyon seanslarını güncelle
                UpdateSeans(connection, musteriId, checkBoxCene, txtSeansCene.Text, checkBoxCene.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxBiyik, txtSeansBiyik.Text, checkBoxBiyik.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxCeneBiyik, txtSeansCeneBiyik.Text, checkBoxCeneBiyik.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxYuz, txtSeansYuz.Text, checkBoxYuz.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxFaul, txtSeansFaul.Text, checkBoxFaul.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGenital, txtSeansGenital.Text, checkBoxGenital.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxTumKol, txtSeansTumKol.Text, checkBoxTumKol.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxTumBacak, txtSeansTumBacak.Text, checkBoxTumBacak.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGobek, txtSeansGobek.Text, checkBoxGobek.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxSirtKomple, txtSeansSirtKomple.Text, checkBoxSirtKomple.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxKolAlti, txtSeansKolAlti.Text, checkBoxKolAlti.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxPopo, txtSeansPopo.Text, checkBoxPopo.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGobekCizgisi, txtSeansGobekCizgisi.Text, checkBoxGobekCizgisi.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxYarimBacak, txtSeansYarimBacak.Text, checkBoxYarimBacak.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxYarimKol, txtSeansYarimKol.Text, checkBoxYarimKol.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxBel, txtSeansBel.Text, checkBoxBel.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGogusKomple, txtSeansGogusKomple.Text, checkBoxGogusKomple.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGogusArasi, txtSeansGogusArasi.Text, checkBoxGogusArasi.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxGogusUcu, txtSeansGogusUcu.Text, checkBoxGogusUcu.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxOzelBolge, txtSeansOzelBolge.Text, checkBoxOzelBolge.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxEnse, txtSeansEnse.Text, checkBoxEnse.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxYanak, txtSeansYanak.Text, checkBoxYanak.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxBoyun, txtSeansBoyun.Text, checkBoxBoyun.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxKasUstuOrtasi, txtSeansKasUstuOrtasi.Text, checkBoxKasUstuOrtasi.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxKulak, txtSeansKulak.Text, checkBoxKulak.Tag.ToString());
                UpdateSeans(connection, musteriId, checkBoxOmuz, txtSeansOmuz.Text, checkBoxOmuz.Tag.ToString());



                // Cilt Bakımı Bakım seanslarını güncelle
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxKlasikCiltBakimi, txtSeansKlasikCiltBakimi.Text, checkBoxKlasikCiltBakimi.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxHydrafacialCiltBakimi, txtSeansHydrafacialCiltBakimi.Text, checkBoxHydrafacialCiltBakimi.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxGoldRenovationARCSystemRepair, txtSeansGoldRenovationARCSystemRepair.Text, checkBoxGoldRenovationARCSystemRepair.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxTirnakBakimi, txtSeansTirnakBakimi.Text, checkBoxTirnakBakimi.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxCatlakBakimi, txtSeansCatlakBakimi.Text, checkBoxCatlakBakimi.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxYosunPeeling, txtSeansYosunPeeling.Text, checkBoxYosunPeeling.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxCollagenIpProtokolu, txtSeansCollagenIpProtokolu.Text, checkBoxCollagenIpProtokolu.Tag.ToString());
                UpdateCiltBakimiSeans(connection, musteriId, checkBoxElUstuBakimi, txtSeansElUstuBakimi.Text, checkBoxElUstuBakimi.Tag.ToString());


                // Zayıflama seanslarını güncelle
                UpdateZayiflamaSeans(connection, musteriId, checkBoxEMSBodyPro, txtSeansEMSBodyPro.Text, checkBoxEMSBodyPro.Tag.ToString());
                UpdateZayiflamaSeans(connection, musteriId, checkBoxLeafDrenaji, txtSeansLeafDrenaji.Text, checkBoxLeafDrenaji.Tag.ToString());
                UpdateZayiflamaSeans(connection, musteriId, checkBoxPasifJimnastik, txtSeansPasifJimnastik.Text, checkBoxPasifJimnastik.Tag.ToString());
                UpdateZayiflamaSeans(connection, musteriId, checkBoxG5Masaji, txtSeansG5Masajı.Text, checkBoxG5Masaji.Tag.ToString());
                UpdateZayiflamaSeans(connection, musteriId, checkBoxMagicSound, txtSeansMagicSound.Text, checkBoxMagicSound.Tag.ToString());

            }

            #endregion
        }
        //Epilasyon
        private void UpdateSeans(SqlConnection connection, int musteriId, System.Windows.Forms.CheckBox checkBox, string seansSayisi, string hizmetId)
        {
            if (checkBox.Checked)
            {
                if (int.TryParse(seansSayisi, out int seans))
                {
                    string query = "UPDATE MusteriBolge SET BolgeSeans = @Seans WHERE MusteriBolgeID = @MusteriID AND BolgeId = @HizmetId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Seans", seans);
                        command.Parameters.AddWithValue("@MusteriID", musteriId);
                        command.Parameters.AddWithValue("@HizmetId", hizmetId);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir seans sayısı giriniz: " + checkBox.Text);
                }
            }
        }
        //Cilt Bakımı
        private void UpdateCiltBakimiSeans(SqlConnection connection, int musteriId, System.Windows.Forms.CheckBox checkBox, string seansSayisi, string hizmetId)
        {
            if (checkBox.Checked)
            {
                if (int.TryParse(seansSayisi, out int seans))
                {
                    string query = "UPDATE MusteriCiltBakimi  SET CiltBakimiSeans  = @Seans WHERE MusteriCiltBakimiID  = @MusteriID AND CiltBakimiID  = @HizmetId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Seans", seans);
                        command.Parameters.AddWithValue("@MusteriID", musteriId);
                        command.Parameters.AddWithValue("@HizmetId", hizmetId);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir cilt bakimi seans sayısı giriniz: " + checkBox.Text);
                }
            }
        }
        //Zayıflama
        private void UpdateZayiflamaSeans(SqlConnection connection, int musteriId, System.Windows.Forms.CheckBox checkBox, string seansSayisi, string hizmetId)
        {
            if (checkBox.Checked)
            {
                if (int.TryParse(seansSayisi, out int seans))
                {
                    string query = "UPDATE MusteriZayiflama SET ZayiflamaSeans = @Seans WHERE MusteriZayiflamaID = @MusteriID AND ZayiflamaID = @HizmetId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Seans", seans);
                        command.Parameters.AddWithValue("@MusteriID", musteriId);
                        command.Parameters.AddWithValue("@HizmetId", hizmetId);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir zayıflama seans sayısı giriniz: " + checkBox.Text);
                }
            }
        }
        #endregion
 

        private void MusteriSilBtn_Click(object sender, EventArgs e)
        {
            SilFonksiyon();
        }
        #region Kayıt Silme

        private void SilFonksiyon()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string checkQuery = "SELECT COUNT(*) FROM Musteri WHERE MusteriID = @id AND IsActive = 1";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction);
                    checkCommand.Parameters.AddWithValue("@id", gizliId.Text);
                    int rowCount = (int)checkCommand.ExecuteScalar();

                    if (rowCount == 0)
                    {
                        MessageBox.Show("Bu müşteri numarasına sahip aktif bir müşteri kaydı bulunamadı.");
                        return;
                    }

                    var confirmationResult = MessageBox.Show("Bu müşteriyi pasifleştirmek istediğinize emin misiniz?", "Müşteri Pasifleştirme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirmationResult == DialogResult.No)
                    {
                        return;
                    }

                    // Müşteriyi pasifleştir
                    string deactivateQuery = "UPDATE Musteri SET IsActive = 0 WHERE MusteriID = @id";
                    SqlCommand deactivateCommand = new SqlCommand(deactivateQuery, connection, transaction);
                    deactivateCommand.Parameters.AddWithValue("@id", Convert.ToInt32(gizliId.Text));
                    deactivateCommand.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Müşteri başarıyla pasifleştirildi.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    DataGridTemizlemeVeMusteriGetir();
                }
            }
        }

        #endregion


        private void DataGridTemizlemeVeMusteriGetir()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            MusteriGetir();
        }
        private void TumSeansTextleriniBosalt()
        {
            //Epilasyon seansları boşalt
            txtSeansCene.Text = "";
            txtSeansBiyik.Text = "";
            txtSeansCeneBiyik.Text = "";
            txtSeansYuz.Text = "";
            txtSeansFaul.Text = "";
            txtSeansGenital.Text = "";
            txtSeansTumKol.Text = "";
            txtSeansTumBacak.Text = "";
            txtSeansGobek.Text = "";
            txtSeansSirtKomple.Text = "";
            txtSeansKolAlti.Text = "";
            txtSeansPopo.Text = "";
            txtSeansGobekCizgisi.Text = "";
            txtSeansYarimBacak.Text = "";
            txtSeansYarimKol.Text = "";
            txtSeansBel.Text = "";
            txtSeansGogusKomple.Text = "";
            txtSeansGogusArasi.Text = "";
            txtSeansGogusUcu.Text = "";
            txtSeansOzelBolge.Text = "";
            txtSeansEnse.Text = "";
            txtSeansYanak.Text = "";
            txtSeansBoyun.Text = "";
            txtSeansKasUstuOrtasi.Text = "";
            txtSeansKulak.Text = "";
            txtSeansOmuz.Text = "";

            //Zayıflama seansları boşalt
            txtSeansEMSBodyPro.Text = "";
            txtSeansLeafDrenaji.Text = "";
            txtSeansPasifJimnastik.Text = "";
            txtSeansG5Masajı.Text = "";
            txtSeansMagicSound.Text = "";

            //Bakim seanslarını boşalt
            txtSeansKlasikCiltBakimi.Text = "";
            txtSeansHydrafacialCiltBakimi.Text = "";
            txtSeansGoldRenovationARCSystemRepair.Text = "";
            txtSeansTirnakBakimi.Text = "";
            txtSeansCatlakBakimi.Text = "";
            txtSeansYosunPeeling.Text = "";
            txtSeansCollagenIpProtokolu.Text = "";
            txtSeansElUstuBakimi.Text = "";
        }
        private void MusteriSeansVerileriYukle(int musteriId)
        {
            TumSeansTextleriniBosalt();
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
                        // Hizmet adı ve seans sayısı için null kontrolü
                        string adi = reader.IsDBNull(reader.GetOrdinal(adiColumn)) ? "Bilinmiyor" : reader.GetString(reader.GetOrdinal(adiColumn));
                        // Eğer veritabanında seans bilgisi NULL ise veya sıfır değeri taşıyorsa, 0 olarak işle
                        int seans = (reader.IsDBNull(reader.GetOrdinal(seansColumn)) || reader.GetInt32(reader.GetOrdinal(seansColumn)) == 0) ? 0 : reader.GetInt32(reader.GetOrdinal(seansColumn));

                        SeanslariTextBoxlaraYaz(adi, seans);
                    }
                }
            }
        }
        private void SeanslariTextBoxlaraYaz(string hizmetAdi, int seans)
        {
            switch (hizmetAdi)
            {

                //Chechboxın text adı yazılır , textboxun adı yazılır.
                //EPİLASYON SEANSLARI
                case "Çene":
                    txtSeansCene.Text = seans.ToString();
                    break;
                case "Bıyık":
                    txtSeansBiyik.Text = seans.ToString();
                    break;
                case "Çene-Bıyık":
                    txtSeansCeneBiyik.Text = seans.ToString();
                    break;
                case "Yüz":
                    txtSeansYuz.Text = seans.ToString();
                    break;
                case "Faul":
                    txtSeansFaul.Text = seans.ToString();
                    break;
                case "Genital":
                    txtSeansGenital.Text = seans.ToString();
                    break;
                case "Tüm Kol":
                    txtSeansTumKol.Text = seans.ToString();
                    break;
                case "Tüm Bacak":
                    txtSeansTumBacak.Text = seans.ToString();
                    break;
                case "Göbek":
                    txtSeansGobek.Text = seans.ToString();
                    break;
                case "Sırt Komple":
                    txtSeansSirtKomple.Text = seans.ToString();
                    break;
                case "Kol Altı":
                    txtSeansKolAlti.Text = seans.ToString();
                    break;
                case "Popo":
                    txtSeansPopo.Text = seans.ToString();
                    break;
                case "Göbek Çizgisi":
                    txtSeansGobekCizgisi.Text = seans.ToString();
                    break;
                case "Yarım Bacak":
                    txtSeansYarimBacak.Text = seans.ToString();
                    break;
                case "Yarım Kol":
                    txtSeansYarimKol.Text = seans.ToString();
                    break;
                case "Bel":
                    txtSeansBel.Text = seans.ToString();
                    break;
                case "Göğüs Komple":
                    txtSeansGogusKomple.Text = seans.ToString();
                    break;
                case "Göğüs Arası":
                    txtSeansGogusArasi.Text = seans.ToString();
                    break;
                case "Göğüs Ucu":
                    txtSeansGogusUcu.Text = seans.ToString();
                    break;
                case "Özel Bölge":
                    txtSeansOzelBolge.Text = seans.ToString();
                    break;
                case "Ense":
                    txtSeansEnse.Text = seans.ToString();
                    break;
                case "Yanak":
                    txtSeansYanak.Text = seans.ToString();
                    break;
                case "Boyun":
                    txtSeansBoyun.Text = seans.ToString();
                    break;
                case "Kaş Üstü-Ortası":
                    txtSeansKasUstuOrtasi.Text = seans.ToString();
                    break;
                case "Kulak":
                    txtSeansKulak.Text = seans.ToString();
                    break;
                case "Omuz":
                    txtSeansOmuz.Text = seans.ToString();
                    break;

                // CİLT VE VÜCUT BAKIMI SEANSLARI
                case "Klasik Cilt Bakımı":
                    txtSeansKlasikCiltBakimi.Text = seans.ToString();
                    break;
                case "Hydrafacial Cilt Bakımı":
                    txtSeansHydrafacialCiltBakimi.Text = seans.ToString();
                    break;
                case "GRARCS Repair":
                    txtSeansGoldRenovationARCSystemRepair.Text = seans.ToString();
                    break;
                case "Tırnak Bakımı":
                    txtSeansTirnakBakimi.Text = seans.ToString();
                    break;
                case "Çatlak Bakımı":
                    txtSeansCatlakBakimi.Text = seans.ToString();
                    break;
                case "Yosun Peeling":
                    txtSeansYosunPeeling.Text = seans.ToString();
                    break;
                case "Collagen İp Protokolü":
                    txtSeansCollagenIpProtokolu.Text = seans.ToString();
                    break;
                case "El Üstü Bakımı":
                    txtSeansElUstuBakimi.Text = seans.ToString();
                    break;



                //ZAYIFLAMA SEANSLARI
                case "EMS Body Pro":
                    txtSeansEMSBodyPro.Text = seans.ToString();
                    break;
                case "Leaf Drenaji":
                    txtSeansLeafDrenaji.Text = seans.ToString();
                    break;
                case "Pasif Jimnastik":
                    txtSeansPasifJimnastik.Text = seans.ToString();
                    break;
                case "G5 Masajı":
                    txtSeansG5Masajı.Text = seans.ToString();
                    break;
                case "Magic Sound":
                    txtSeansMagicSound.Text = seans.ToString();
                    break;

            }
        }


        #region CheckBoxları Boşalt
        private void TumSeansCheckBoxlariniBosalt()
        {
            // Burada her bir GroupBox için ResetCheckBoxesInContainer fonksiyonunu çağırıyoruz.
            ResetCheckBoxesInSpecificGroupBox(groupBox1);
            ResetCheckBoxesInSpecificGroupBox(groupBox2);
            ResetCheckBoxesInSpecificGroupBox(groupBox3);
        }
        private void ResetCheckBoxesInSpecificGroupBox(Control container)
        {
            // Her bir control için CheckBox mı diye kontrol ediyoruz.
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is System.Windows.Forms.CheckBox checkBox)
                {
                    checkBox.Checked = false;  // CheckBox ise, Checked özelliğini false yapıyoruz.
                }
                // Eğer control'ün çocukları varsa, yani başka kontroller içeriyorsa,
                // bu fonksiyonu yinelemeli olarak çağırıyoruz.
                else if (ctrl.HasChildren)
                {
                    ResetCheckBoxesInSpecificGroupBox(ctrl);
                }
            }
        }
        #endregion

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            TumSeansTextleriniBosalt();
            if (e.RowIndex >= 0) // Geçerli bir satır seçimi olduğundan emin ol
            {
                int musteriId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["MusteriID"].Value);
                MusteriSeansVerileriYukle(musteriId);
            }

            #region EPILSAYON CheckBox
            //Epilasyon CheckBox'larını temizle
            foreach (System.Windows.Forms.CheckBox checkBox in groupBox1.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                checkBox.Checked = false;
            }

            // Seçilen satırın epilasyon isimlerini al
            string bolgeler = dataGridView1.CurrentRow.Cells["BolgeAdiColumn"].Value.ToString();
            string[] bolgeListesi = bolgeler.Split(',');

            //Epilasyon CheckBox'larını işaretle
            foreach (string bolge in bolgeListesi)
            {
                string bolgeAdi = bolge.Trim();
                System.Windows.Forms.CheckBox checkBox = groupBox1.Controls.OfType<System.Windows.Forms.CheckBox>().FirstOrDefault(c => c.Text == bolgeAdi);
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }
            }
            #endregion

            #region CILT BAKIMI CheckBox
            // Cilt Bakımı CheckBox'larını temizle
            foreach (System.Windows.Forms.CheckBox checkBox in groupBox2.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                checkBox.Checked = false;
            }

            // Seçilen satırın cilt bakımı isimlerini al
            string ciltBakimlari = dataGridView1.CurrentRow.Cells["CiltBakimiAdiColumn"].Value.ToString();
            string[] ciltBakimiListesi = ciltBakimlari.Split(',');

            // Cilt Bakımı CheckBox'larını işaretle
            foreach (string ciltBakimi in ciltBakimiListesi)
            {
                string ciltBakimiAdi = ciltBakimi.Trim();
                System.Windows.Forms.CheckBox checkBox = groupBox2.Controls.OfType<System.Windows.Forms.CheckBox>().FirstOrDefault(c => c.Text == ciltBakimiAdi);
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }
            }
            #endregion

            #region ZAYIFLAMA CheckBox
            // Zayıflama CheckBox'larını temizle
            foreach (System.Windows.Forms.CheckBox checkBox in groupBox3.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                checkBox.Checked = false;
            }

            // Seçilen satırın zayıflama isimlerini al
            string zayiflamalar = dataGridView1.CurrentRow.Cells["ZayiflamaAdiColumn"].Value.ToString();
            string[] zayiflamaListesi = zayiflamalar.Split(',');

            // Zayıflama CheckBox'larını işaretle
            foreach (string zayiflama in zayiflamaListesi)
            {
                string zayiflamaAdi = zayiflama.Trim();
                System.Windows.Forms.CheckBox checkBox = groupBox3.Controls.OfType<System.Windows.Forms.CheckBox>().FirstOrDefault(c => c.Text == zayiflamaAdi);
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }
            }
            #endregion

            //Seçilen satırın diğer verilerini TextBox'lara aktar
            gizliId.Text = dataGridView1.CurrentRow.Cells["MusteriID"].Value.ToString();
            txtMusteriAdi.Text = dataGridView1.CurrentRow.Cells["AdColumn"].Value.ToString();
            txtMusteriSoyadi.Text = dataGridView1.CurrentRow.Cells["SoyadColumn"].Value.ToString();
            txtTelefonNumarasi.Text = dataGridView1.CurrentRow.Cells["TelefonColumn"].Value.ToString();
            txtMusteriOdenecekTutari.Text = dataGridView1.CurrentRow.Cells["Odenen"].Value.ToString();
            txtMusteriOdenecekTutari.Text = dataGridView1.CurrentRow.Cells["Odenecek"].Value.ToString();

            // DataGridView yüklediğinizde veya verileri güncellediğinizde
            // varsayılan olarak seçili olan satırın seçimini temizleyin

        }

        private void BtnAra_Click_Click(object sender, EventArgs e)
        {
            // Filtre için ad, soyad ve telefon numarası değerlerini al
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

            //Eğer aranan isimde bir kullanıcı bulunamadıysa uyarı mesajı göster
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
            txtMusteriOdenecekTutari.Text = "";

            // Tüm satırları görünür yap
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }
        }

        private void KayitTextBosalt_Click(object sender, EventArgs e)
        {
            txtMusteriAdi.Text = "";
            txtMusteriSoyadi.Text = "";
            txtTelefonNumarasi.Text = "";
            txtMusteriOdenecekTutari.Text = "";
            TumSeansTextleriniBosalt();
            TumSeansCheckBoxlariniBosalt();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }
        }

        #region FORM DUGMELERİ
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

        #region Check Box Renk Ayarı

        //CheckBox renklendirme
        private void CheckboxRenklendir()
        {
            CheckBoxlariGroupOlarakRenklendir(groupBox1, Color.PaleVioletRed);
            CheckBoxlariGroupOlarakRenklendir(groupBox2, Color.LightSeaGreen);
            CheckBoxlariGroupOlarakRenklendir(groupBox3, Color.Tan);
        }
        //Renklendirme Fonksiyonu
        private void CheckBoxlariGroupOlarakRenklendir(System.Windows.Forms.GroupBox groupBox, Color checkedColor)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is System.Windows.Forms.CheckBox)
                {
                    System.Windows.Forms.CheckBox checkBox = control as System.Windows.Forms.CheckBox;
                    checkBox.CheckedChanged += (sender, args) =>
                    {
                        // CheckBox seçili ise checkedColor, değilse siyah renk kullan
                        checkBox.BackColor = checkBox.Checked ? checkedColor : Color.Black;
                    };
                }
            }
        }
        #endregion

        #region DATAGRID RENK AYARI
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Satır 1
            if (e.RowIndex % 2 == 0)
                e.CellStyle.BackColor = Color.White;  // 1.satır Arkaplan
            //Satır 2
            else
                e.CellStyle.BackColor = Color.MediumPurple;  // 2.satır Arkaplan
            
            e.CellStyle.ForeColor = Color.Black; //Satırların Metin rengi
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Performansı artırmak için işlem öncesi ve sonrası güncellemeleri askıya al
            dataGridView1.SuspendLayout();

            try
            {
                // Güncellenmesi gereken satırları sıfırla
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.Selected && row.DefaultCellStyle.SelectionBackColor == Color.Pink)
                    {
                        // Sadece renkleri değiştirilmiş olanları varsayılan hale getir
                        row.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                        row.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                    }
                }

                // Yeni seçili olan satırların renklerini ayarla
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    row.DefaultCellStyle.SelectionBackColor = Color.Pink;
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
                }
            }
            finally
            {
                // İşlemler tamamlandıktan sonra güncellemeleri devam ettir
                dataGridView1.ResumeLayout();
            }
        }





        #endregion


    }
}



