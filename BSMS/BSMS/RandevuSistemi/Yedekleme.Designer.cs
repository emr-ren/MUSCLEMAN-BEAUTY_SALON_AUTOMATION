namespace RandevuSistemi
{
    partial class Yedekleme
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Yedekleme));
            this.backButton = new System.Windows.Forms.Button();
            this.HomeIcon = new System.Windows.Forms.ImageList(this.components);
            this.SelectionIcons = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.CiktiAl = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PasiflesmisMusteriler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.backButton.ImageList = this.HomeIcon;
            this.backButton.Location = new System.Drawing.Point(25, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(112, 96);
            this.backButton.TabIndex = 148;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // HomeIcon
            // 
            this.HomeIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("HomeIcon.ImageStream")));
            this.HomeIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.HomeIcon.Images.SetKeyName(0, "home.png");
            // 
            // SelectionIcons
            // 
            this.SelectionIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SelectionIcons.ImageStream")));
            this.SelectionIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.SelectionIcons.Images.SetKeyName(0, "checklist.png");
            this.SelectionIcons.Images.SetKeyName(1, "download.png");
            this.SelectionIcons.Images.SetKeyName(2, "remove (1).png");
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.ImageIndex = 1;
            this.button2.ImageList = this.SelectionIcons;
            this.button2.Location = new System.Drawing.Point(1271, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 108);
            this.button2.TabIndex = 149;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.VeriTabaniYedegi_Click);
            // 
            // CiktiAl
            // 
            this.CiktiAl.BackColor = System.Drawing.Color.Transparent;
            this.CiktiAl.FlatAppearance.BorderSize = 0;
            this.CiktiAl.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.CiktiAl.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CiktiAl.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CiktiAl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CiktiAl.Font = new System.Drawing.Font("Agency FB", 18F, System.Drawing.FontStyle.Bold);
            this.CiktiAl.ForeColor = System.Drawing.Color.Black;
            this.CiktiAl.ImageIndex = 0;
            this.CiktiAl.ImageList = this.SelectionIcons;
            this.CiktiAl.Location = new System.Drawing.Point(591, 149);
            this.CiktiAl.Name = "CiktiAl";
            this.CiktiAl.Size = new System.Drawing.Size(121, 108);
            this.CiktiAl.TabIndex = 161;
            this.CiktiAl.UseVisualStyleBackColor = false;
            this.CiktiAl.Click += new System.EventHandler(this.CSVkaydet);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bauhaus 93", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.MediumPurple;
            this.label2.Location = new System.Drawing.Point(1191, 272);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 33);
            this.label2.TabIndex = 162;
            this.label2.Text = "VERI TABANI YEDEKLE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bauhaus 93", 21.75F);
            this.label3.ForeColor = System.Drawing.Color.MediumPurple;
            this.label3.Location = new System.Drawing.Point(496, 272);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(328, 33);
            this.label3.TabIndex = 163;
            this.label3.Text = "MÜSTERI LISTESI YEDEKLE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PasiflesmisMusteriler
            // 
            this.PasiflesmisMusteriler.BackColor = System.Drawing.Color.Transparent;
            this.PasiflesmisMusteriler.FlatAppearance.BorderSize = 0;
            this.PasiflesmisMusteriler.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.PasiflesmisMusteriler.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PasiflesmisMusteriler.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PasiflesmisMusteriler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PasiflesmisMusteriler.ForeColor = System.Drawing.Color.Black;
            this.PasiflesmisMusteriler.ImageIndex = 2;
            this.PasiflesmisMusteriler.ImageList = this.SelectionIcons;
            this.PasiflesmisMusteriler.Location = new System.Drawing.Point(591, 649);
            this.PasiflesmisMusteriler.Name = "PasiflesmisMusteriler";
            this.PasiflesmisMusteriler.Size = new System.Drawing.Size(121, 108);
            this.PasiflesmisMusteriler.TabIndex = 165;
            this.PasiflesmisMusteriler.UseVisualStyleBackColor = false;
            this.PasiflesmisMusteriler.Click += new System.EventHandler(this.PasiflesmisMusteriler_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bauhaus 93", 21.75F);
            this.label1.ForeColor = System.Drawing.Color.MediumPurple;
            this.label1.Location = new System.Drawing.Point(496, 787);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 33);
            this.label1.TabIndex = 166;
            this.label1.Text = "INAKTIF MÜSTERI LISTESI";
            // 
            // Yedekleme
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PasiflesmisMusteriler);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CiktiAl);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.backButton);
            this.Font = new System.Drawing.Font("Papyrus", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Red;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Yedekleme";
            this.Text = "Yedekleme";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Yedekleme_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.ImageList SelectionIcons;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList HomeIcon;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button CiktiAl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button PasiflesmisMusteriler;
        private System.Windows.Forms.Label label1;
    }
}