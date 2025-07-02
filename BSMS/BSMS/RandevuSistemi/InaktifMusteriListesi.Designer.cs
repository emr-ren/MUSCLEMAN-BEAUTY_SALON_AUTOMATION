namespace RandevuSistemi
{
    partial class InaktifMusteriListesi
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InaktifMusteriListesi));
            this.dataGridViewInactiveCustomers = new System.Windows.Forms.DataGridView();
            this.backButton = new System.Windows.Forms.Button();
            this.BackIcon = new System.Windows.Forms.ImageList(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtTefonlaArama = new System.Windows.Forms.TextBox();
            this.AramaSifirla = new System.Windows.Forms.Button();
            this.FindAndCleanIcons = new System.Windows.Forms.ImageList(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSoyadArama = new System.Windows.Forms.TextBox();
            this.txtAdArama = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAra = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInactiveCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewInactiveCustomers
            // 
            this.dataGridViewInactiveCustomers.AllowUserToAddRows = false;
            this.dataGridViewInactiveCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewInactiveCustomers.BackgroundColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewInactiveCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewInactiveCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Tai Le", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Fuchsia;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewInactiveCustomers.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewInactiveCustomers.GridColor = System.Drawing.Color.Black;
            this.dataGridViewInactiveCustomers.Location = new System.Drawing.Point(509, 525);
            this.dataGridViewInactiveCustomers.Name = "dataGridViewInactiveCustomers";
            this.dataGridViewInactiveCustomers.ReadOnly = true;
            this.dataGridViewInactiveCustomers.RowHeadersWidth = 20;
            this.dataGridViewInactiveCustomers.RowTemplate.Height = 80;
            this.dataGridViewInactiveCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewInactiveCustomers.Size = new System.Drawing.Size(1004, 408);
            this.dataGridViewInactiveCustomers.TabIndex = 147;
            this.dataGridViewInactiveCustomers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewInactiveCustomers_CellDoubleClick);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.ForeColor = System.Drawing.Color.Black;
            this.backButton.ImageIndex = 0;
            this.backButton.ImageList = this.BackIcon;
            this.backButton.Location = new System.Drawing.Point(25, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(112, 96);
            this.backButton.TabIndex = 149;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // BackIcon
            // 
            this.BackIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("BackIcon.ImageStream")));
            this.BackIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.BackIcon.Images.SetKeyName(0, "login.png");
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(1074, 206);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(245, 30);
            this.label7.TabIndex = 163;
            this.label7.Text = "Telefon Numarası";
            // 
            // txtTefonlaArama
            // 
            this.txtTefonlaArama.BackColor = System.Drawing.Color.DarkOrchid;
            this.txtTefonlaArama.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTefonlaArama.Font = new System.Drawing.Font("Microsoft Yi Baiti", 18F);
            this.txtTefonlaArama.ForeColor = System.Drawing.Color.Black;
            this.txtTefonlaArama.Location = new System.Drawing.Point(1080, 244);
            this.txtTefonlaArama.Multiline = true;
            this.txtTefonlaArama.Name = "txtTefonlaArama";
            this.txtTefonlaArama.Size = new System.Drawing.Size(144, 39);
            this.txtTefonlaArama.TabIndex = 2;
            this.txtTefonlaArama.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // AramaSifirla
            // 
            this.AramaSifirla.BackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatAppearance.BorderSize = 0;
            this.AramaSifirla.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AramaSifirla.ForeColor = System.Drawing.Color.Black;
            this.AramaSifirla.ImageIndex = 1;
            this.AramaSifirla.ImageList = this.FindAndCleanIcons;
            this.AramaSifirla.Location = new System.Drawing.Point(1080, 340);
            this.AramaSifirla.Name = "AramaSifirla";
            this.AramaSifirla.Size = new System.Drawing.Size(94, 88);
            this.AramaSifirla.TabIndex = 4;
            this.AramaSifirla.UseVisualStyleBackColor = false;
            this.AramaSifirla.Click += new System.EventHandler(this.BtnAramaSifirla_Click);
            // 
            // FindAndCleanIcons
            // 
            this.FindAndCleanIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FindAndCleanIcons.ImageStream")));
            this.FindAndCleanIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.FindAndCleanIcons.Images.SetKeyName(0, "market-research (1).png");
            this.FindAndCleanIcons.Images.SetKeyName(1, "remove (2).png");
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(939, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 30);
            this.label5.TabIndex = 159;
            this.label5.Text = "Soyad";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(802, 206);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 30);
            this.label4.TabIndex = 158;
            this.label4.Text = "Ad";
            // 
            // txtSoyadArama
            // 
            this.txtSoyadArama.BackColor = System.Drawing.Color.MediumOrchid;
            this.txtSoyadArama.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSoyadArama.Font = new System.Drawing.Font("Microsoft Yi Baiti", 18F);
            this.txtSoyadArama.ForeColor = System.Drawing.Color.Black;
            this.txtSoyadArama.Location = new System.Drawing.Point(911, 244);
            this.txtSoyadArama.Multiline = true;
            this.txtSoyadArama.Name = "txtSoyadArama";
            this.txtSoyadArama.Size = new System.Drawing.Size(144, 39);
            this.txtSoyadArama.TabIndex = 1;
            this.txtSoyadArama.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAdArama
            // 
            this.txtAdArama.BackColor = System.Drawing.Color.Orchid;
            this.txtAdArama.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAdArama.Font = new System.Drawing.Font("Microsoft Yi Baiti", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdArama.ForeColor = System.Drawing.Color.Black;
            this.txtAdArama.Location = new System.Drawing.Point(748, 244);
            this.txtAdArama.Multiline = true;
            this.txtAdArama.Name = "txtAdArama";
            this.txtAdArama.Size = new System.Drawing.Size(144, 39);
            this.txtAdArama.TabIndex = 0;
            this.txtAdArama.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAra
            // 
            this.btnAra.BackColor = System.Drawing.Color.Transparent;
            this.btnAra.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAra.FlatAppearance.BorderSize = 0;
            this.btnAra.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnAra.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAra.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAra.ForeColor = System.Drawing.Color.Black;
            this.btnAra.ImageKey = "market-research (1).png";
            this.btnAra.ImageList = this.FindAndCleanIcons;
            this.btnAra.Location = new System.Drawing.Point(798, 340);
            this.btnAra.Name = "btnAra";
            this.btnAra.Size = new System.Drawing.Size(94, 88);
            this.btnAra.TabIndex = 3;
            this.btnAra.UseVisualStyleBackColor = false;
            this.btnAra.Click += new System.EventHandler(this.BtnAra_Click_1);
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(816, 431);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 25);
            this.label11.TabIndex = 170;
            this.label11.Text = "Müşteri Ara";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1077, 431);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 25);
            this.label1.TabIndex = 171;
            this.label1.Text = "Aramayı Temizle";
            // 
            // InaktifMusteriListesi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.dataGridViewInactiveCustomers);
            this.Controls.Add(this.btnAra);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtTefonlaArama);
            this.Controls.Add(this.AramaSifirla);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSoyadArama);
            this.Controls.Add(this.txtAdArama);
            this.Controls.Add(this.backButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InaktifMusteriListesi";
            this.Text = "PasiflesmisMusteriler";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.InaktifMusteriListesi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInactiveCustomers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewInactiveCustomers;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.ImageList BackIcon;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtTefonlaArama;
        private System.Windows.Forms.Button AramaSifirla;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSoyadArama;
        private System.Windows.Forms.TextBox txtAdArama;
        private System.Windows.Forms.ImageList FindAndCleanIcons;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnAra;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
    }
}