namespace RandevuSistemi
{
    partial class Notlar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.notListesi = new System.Windows.Forms.ListBox();
            this.notMetni = new System.Windows.Forms.TextBox();
            this.btnNotEkle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // notListesi
            // 
            this.notListesi.BackColor = System.Drawing.Color.Orchid;
            this.notListesi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notListesi.Font = new System.Drawing.Font("Franklin Gothic Book", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notListesi.ForeColor = System.Drawing.Color.Black;
            this.notListesi.FormattingEnabled = true;
            this.notListesi.ItemHeight = 17;
            this.notListesi.Location = new System.Drawing.Point(0, 0);
            this.notListesi.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.notListesi.Name = "notListesi";
            this.notListesi.Size = new System.Drawing.Size(495, 814);
            this.notListesi.TabIndex = 0;
            this.notListesi.SelectedIndexChanged += new System.EventHandler(this.notListesi_SelectedIndexChanged);
            // 
            // notMetni
            // 
            this.notMetni.BackColor = System.Drawing.Color.DarkViolet;
            this.notMetni.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.notMetni.Font = new System.Drawing.Font("Franklin Gothic Book", 9.75F);
            this.notMetni.ForeColor = System.Drawing.Color.White;
            this.notMetni.Location = new System.Drawing.Point(0, 734);
            this.notMetni.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.notMetni.Multiline = true;
            this.notMetni.Name = "notMetni";
            this.notMetni.Size = new System.Drawing.Size(495, 80);
            this.notMetni.TabIndex = 1;
            // 
            // btnNotEkle
            // 
            this.btnNotEkle.BackColor = System.Drawing.Color.Magenta;
            this.btnNotEkle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnNotEkle.Font = new System.Drawing.Font("Microsoft PhagsPa", 12F, System.Drawing.FontStyle.Bold);
            this.btnNotEkle.Location = new System.Drawing.Point(0, 682);
            this.btnNotEkle.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.btnNotEkle.Name = "btnNotEkle";
            this.btnNotEkle.Size = new System.Drawing.Size(495, 52);
            this.btnNotEkle.TabIndex = 2;
            this.btnNotEkle.Text = "Notu Ekle";
            this.btnNotEkle.UseVisualStyleBackColor = false;
            this.btnNotEkle.Click += new System.EventHandler(this.BtnNotEkle_Click);
            // 
            // Notlar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(495, 814);
            this.Controls.Add(this.btnNotEkle);
            this.Controls.Add(this.notMetni);
            this.Controls.Add(this.notListesi);
            this.Font = new System.Drawing.Font("Poor Richard", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.Name = "Notlar";
            this.Text = "Notlar";
            this.Load += new System.EventHandler(this.Notlar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox notListesi;
        private System.Windows.Forms.TextBox notMetni;
        private System.Windows.Forms.Button btnNotEkle;
    }
}