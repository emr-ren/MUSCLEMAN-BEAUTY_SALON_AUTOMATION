namespace RandevuSistemi
{
    partial class FaturaGecmisi
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FaturaGecmisi));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.backButton = new System.Windows.Forms.Button();
            this.BackIcon = new System.Windows.Forms.ImageList(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSoyadArama = new System.Windows.Forms.TextBox();
            this.txtAdArama = new System.Windows.Forms.TextBox();
            this.btnAra_Click = new System.Windows.Forms.Button();
            this.FindAndCleanIcons = new System.Windows.Forms.ImageList(this.components);
            this.AramaSifirla = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.ColseAndMinimalizeIcon = new System.Windows.Forms.ImageList(this.components);
            this.MinimizeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Black;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeight = 40;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Pink;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Purple;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dataGridView1.Location = new System.Drawing.Point(229, 40);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 40;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(772, 974);
            this.dataGridView1.TabIndex = 137;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // backButton
            // 
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.ForeColor = System.Drawing.Color.Black;
            this.backButton.ImageIndex = 0;
            this.backButton.ImageList = this.BackIcon;
            this.backButton.Location = new System.Drawing.Point(25, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(112, 96);
            this.backButton.TabIndex = 148;
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // BackIcon
            // 
            this.BackIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("BackIcon.ImageStream")));
            this.BackIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.BackIcon.Images.SetKeyName(0, "login.png");
            // 
            // label6
            // 
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1223, 293);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 47);
            this.label6.TabIndex = 157;
            this.label6.Text = "Müşteri Arama";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1352, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 47);
            this.label5.TabIndex = 156;
            this.label5.Text = "Soyisim";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(1156, 369);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 47);
            this.label4.TabIndex = 155;
            this.label4.Text = "İsim";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSoyadArama
            // 
            this.txtSoyadArama.BackColor = System.Drawing.Color.MediumOrchid;
            this.txtSoyadArama.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSoyadArama.Font = new System.Drawing.Font("Yu Gothic Light", 18F);
            this.txtSoyadArama.Location = new System.Drawing.Point(1332, 419);
            this.txtSoyadArama.Multiline = true;
            this.txtSoyadArama.Name = "txtSoyadArama";
            this.txtSoyadArama.Size = new System.Drawing.Size(155, 36);
            this.txtSoyadArama.TabIndex = 2;
            this.txtSoyadArama.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAdArama
            // 
            this.txtAdArama.BackColor = System.Drawing.Color.Orchid;
            this.txtAdArama.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAdArama.Font = new System.Drawing.Font("Yu Gothic Light", 18F);
            this.txtAdArama.Location = new System.Drawing.Point(1115, 419);
            this.txtAdArama.Multiline = true;
            this.txtAdArama.Name = "txtAdArama";
            this.txtAdArama.Size = new System.Drawing.Size(155, 36);
            this.txtAdArama.TabIndex = 1;
            this.txtAdArama.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAra_Click
            // 
            this.btnAra_Click.BackColor = System.Drawing.Color.Black;
            this.btnAra_Click.FlatAppearance.BorderSize = 0;
            this.btnAra_Click.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAra_Click.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAra_Click.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAra_Click.ForeColor = System.Drawing.Color.Black;
            this.btnAra_Click.ImageIndex = 0;
            this.btnAra_Click.ImageList = this.FindAndCleanIcons;
            this.btnAra_Click.Location = new System.Drawing.Point(1238, 495);
            this.btnAra_Click.Name = "btnAra_Click";
            this.btnAra_Click.Size = new System.Drawing.Size(120, 101);
            this.btnAra_Click.TabIndex = 3;
            this.btnAra_Click.UseVisualStyleBackColor = false;
            this.btnAra_Click.Click += new System.EventHandler(this.BtnAra_Click_Click);
            // 
            // FindAndCleanIcons
            // 
            this.FindAndCleanIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FindAndCleanIcons.ImageStream")));
            this.FindAndCleanIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.FindAndCleanIcons.Images.SetKeyName(0, "market-research (1).png");
            this.FindAndCleanIcons.Images.SetKeyName(1, "remove (2).png");
            this.FindAndCleanIcons.Images.SetKeyName(2, "coin.png");
            this.FindAndCleanIcons.Images.SetKeyName(3, "money.png");
            // 
            // AramaSifirla
            // 
            this.AramaSifirla.BackColor = System.Drawing.Color.Black;
            this.AramaSifirla.FlatAppearance.BorderSize = 0;
            this.AramaSifirla.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.AramaSifirla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AramaSifirla.ForeColor = System.Drawing.Color.Black;
            this.AramaSifirla.ImageIndex = 1;
            this.AramaSifirla.ImageList = this.FindAndCleanIcons;
            this.AramaSifirla.Location = new System.Drawing.Point(1548, 382);
            this.AramaSifirla.Name = "AramaSifirla";
            this.AramaSifirla.Size = new System.Drawing.Size(108, 94);
            this.AramaSifirla.TabIndex = 4;
            this.AramaSifirla.UseVisualStyleBackColor = false;
            this.AramaSifirla.Click += new System.EventHandler(this.AramaSifirla_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.Color.Black;
            this.ExitButton.FlatAppearance.BorderSize = 0;
            this.ExitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.ExitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.ForeColor = System.Drawing.Color.Black;
            this.ExitButton.ImageIndex = 0;
            this.ExitButton.ImageList = this.ColseAndMinimalizeIcon;
            this.ExitButton.Location = new System.Drawing.Point(1820, 29);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 70);
            this.ExitButton.TabIndex = 160;
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // ColseAndMinimalizeIcon
            // 
            this.ColseAndMinimalizeIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ColseAndMinimalizeIcon.ImageStream")));
            this.ColseAndMinimalizeIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.ColseAndMinimalizeIcon.Images.SetKeyName(0, "remove.png");
            this.ColseAndMinimalizeIcon.Images.SetKeyName(1, "minimize.png");
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.FlatAppearance.BorderSize = 0;
            this.MinimizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.MinimizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinimizeButton.ImageKey = "minimize.png";
            this.MinimizeButton.ImageList = this.ColseAndMinimalizeIcon;
            this.MinimizeButton.Location = new System.Drawing.Point(1718, 29);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(75, 70);
            this.MinimizeButton.TabIndex = 161;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1174, 581);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(263, 26);
            this.label1.TabIndex = 162;
            this.label1.Text = "Müşteri Ara";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1492, 468);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 30);
            this.label2.TabIndex = 163;
            this.label2.Text = "Aramayı Sıfırla";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FaturaGecmisi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MinimizeButton);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.AramaSifirla);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSoyadArama);
            this.Controls.Add(this.txtAdArama);
            this.Controls.Add(this.btnAra_Click);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.dataGridView1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FaturaGecmisi";
            this.Text = "Fatura Geçmişi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FaturaGecmisi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSoyadArama;
        private System.Windows.Forms.TextBox txtAdArama;
        private System.Windows.Forms.Button btnAra_Click;
        private System.Windows.Forms.Button AramaSifirla;
        private System.Windows.Forms.ImageList BackIcon;
        private System.Windows.Forms.ImageList FindAndCleanIcons;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.ImageList ColseAndMinimalizeIcon;
        private System.Windows.Forms.Button MinimizeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}