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
    public partial class Notlar : Form
    {
        public Notlar()
        {

            InitializeComponent();
            this.TopMost = true;
            btnNotEkle.Click += new EventHandler(BtnNotEkle_Click); // Not ekleme event'i
            notListesi.KeyDown += new KeyEventHandler(NotListesi_KeyDown); // Not silme için klavye event'i
            notListesi.DoubleClick += new EventHandler(NotListesi_DoubleClick); // Not silme için çift tıklama event'i
            this.StartPosition = FormStartPosition.Manual; // Başlangıç pozisyonunu manuel olarak ayarla
            this.Location = new Point(5, 0); // Formun açılacağı konumu belirle
        }
        public class NoteItem
        {
            public int NoteID { get; set; }
            public string NoteText { get; set; }
            public DateTime CreatedDate { get; set; }

            public override string ToString()
            {
                return $"{NoteText} - {CreatedDate:yyyy-MM-dd HH:mm:ss}";
            }
        }

        private void BtnNotEkle_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(notMetni.Text))
            {
                AddTextToListBox(notMetni.Text); // Metni işleyip ListBox'a ekle
                SaveNoteToDatabase(notMetni.Text); // Notu veritabanına kaydet
                notMetni.Clear(); // TextBox'ı temizle
            }
        }
        private void SaveNoteToDatabase(string noteText)
        {
            string connectionString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
            // Updated query to include CreatedDate
            string query = "INSERT INTO Notes (NoteText, CreatedDate) VALUES (@NoteText, @CreatedDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NoteText", noteText);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now); // Set CreatedDate to current time

                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        MessageBox.Show("Not kaydedilemedi.", "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void NotListesi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && notListesi.SelectedItem != null)
            {
                int selectedIndex = notListesi.SelectedIndex;
                notListesi.Items.RemoveAt(selectedIndex); // Seçili notu siler

                if (selectedIndex < notListesi.Items.Count && string.IsNullOrEmpty(notListesi.Items[selectedIndex].ToString()))
                {
                    notListesi.Items.RemoveAt(selectedIndex); // Ardından gelen boş satırı siler
                }
                UpdateNoteNumbers(); // Not numaralarını güncelle
            }
        }

        private void UpdateNoteNumbers()
        {
            int count = 1;
            for (int i = 0; i < notListesi.Items.Count; i += 2)
            {
                if (!string.IsNullOrEmpty(notListesi.Items[i].ToString()))
                {
                    string noteText = notListesi.Items[i].ToString().Split(new char[] { '.' }, 2).Last().Trim();
                    notListesi.Items[i] = $"{count}. {noteText}";
                    count++;
                }
            }
        }

        private void NotListesi_DoubleClick(object sender, EventArgs e)
        {
            if (notListesi.SelectedItem != null && notListesi.SelectedItem is NoteItem selectedNote)
            {
                DeleteNoteFromDatabase(selectedNote.NoteID);
                notListesi.Items.Remove(selectedNote);  // Seçili notu ListBox'dan sil
            }
        }


        private void DeleteNoteFromDatabase(int noteID)
        {
            string connectionString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
            string query = "DELETE FROM Notes WHERE NoteID = @NoteID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NoteID", noteID);
                connection.Open();
                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    MessageBox.Show("Not veritabanından silinemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AddTextToListBox(string text)
        {
            const int maxLineLength = 95;
            while (text.Length > maxLineLength)
            {
                int lastSpace = text.LastIndexOf(' ', maxLineLength);
                if (lastSpace == -1) lastSpace = maxLineLength; // Boşluk yoksa, doğrudan kes

                string line = text.Substring(0, lastSpace).Trim();
                notListesi.Items.Add(line);
                text = text.Substring(lastSpace).Trim();
            }

            if (!string.IsNullOrEmpty(text))
            {
                notListesi.Items.Add(text);
            }
        }

        private void BtnNotuKapa(object sender, EventArgs e)
        {
            
                this.Close(); // Formu kapat
            
        }
        private void LoadNotesFromDatabase()
        {
            string connectionString = "Data Source=INCUBUS\\SQLEXPRESS01;Initial Catalog=GuzellikDB;Integrated Security=True";
            string query = "SELECT NoteID, NoteText, CreatedDate FROM Notes ORDER BY CreatedDate DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                notListesi.Items.Clear(); // ListBox'u temizle

                while (reader.Read())
                {
                    NoteItem note = new NoteItem()
                    {
                        NoteID = Convert.ToInt32(reader["NoteID"]),
                        NoteText = reader["NoteText"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                    notListesi.Items.Add(note);
                }
                reader.Close();
            }
        }



        private void Notlar_Load(object sender, EventArgs e)
        {
            LoadNotesFromDatabase();
        }

        private void notListesi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
