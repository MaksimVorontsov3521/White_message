namespace client_v2
{
    partial class log_in_account
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(log_in_account));
            tabControl1 = new TabControl();
            log_in_page = new TabPage();
            log_in_button = new Button();
            enter_password = new TextBox();
            enter_login = new TextBox();
            label5 = new Label();
            label6 = new Label();
            pictureBox1 = new PictureBox();
            tabControl1.SuspendLayout();
            log_in_page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(log_in_page);
            tabControl1.Location = new Point(28, 12);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(489, 492);
            tabControl1.TabIndex = 0;
            // 
            // log_in_page
            // 
            log_in_page.BackgroundImage = (Image)resources.GetObject("log_in_page.BackgroundImage");
            log_in_page.Controls.Add(pictureBox1);
            log_in_page.Controls.Add(log_in_button);
            log_in_page.Controls.Add(enter_password);
            log_in_page.Controls.Add(enter_login);
            log_in_page.Controls.Add(label5);
            log_in_page.Controls.Add(label6);
            log_in_page.ForeColor = SystemColors.ControlText;
            log_in_page.Location = new Point(4, 24);
            log_in_page.Margin = new Padding(4, 3, 4, 3);
            log_in_page.Name = "log_in_page";
            log_in_page.Padding = new Padding(4, 3, 4, 3);
            log_in_page.Size = new Size(481, 464);
            log_in_page.TabIndex = 1;
            log_in_page.Text = "Вход";
            log_in_page.UseVisualStyleBackColor = true;
            log_in_page.Click += log_in_page_Click;
            // 
            // log_in_button
            // 
            log_in_button.BackColor = Color.Black;
            log_in_button.ForeColor = SystemColors.ControlLightLight;
            log_in_button.Location = new Point(159, 362);
            log_in_button.Margin = new Padding(4, 3, 4, 3);
            log_in_button.Name = "log_in_button";
            log_in_button.Size = new Size(155, 51);
            log_in_button.TabIndex = 13;
            log_in_button.Text = "Войти";
            log_in_button.UseVisualStyleBackColor = false;
            log_in_button.Click += log_in_button_Click;
            // 
            // enter_password
            // 
            enter_password.BackColor = Color.FromArgb(255, 192, 128);
            enter_password.Location = new Point(163, 251);
            enter_password.Margin = new Padding(4, 3, 4, 3);
            enter_password.Name = "enter_password";
            enter_password.Size = new Size(151, 23);
            enter_password.TabIndex = 10;
            // 
            // enter_login
            // 
            enter_login.BackColor = Color.FromArgb(255, 192, 128);
            enter_login.Cursor = Cursors.SizeAll;
            enter_login.Location = new Point(158, 180);
            enter_login.Margin = new Padding(4, 3, 4, 5);
            enter_login.Name = "enter_login";
            enter_login.Size = new Size(156, 23);
            enter_login.TabIndex = 9;
            enter_login.TextAlign = HorizontalAlignment.Center;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(159, 233);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(122, 15);
            label5.TabIndex = 8;
            label5.Text = "Введите свой пароль";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(159, 161);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(115, 15);
            label6.TabIndex = 7;
            label6.Text = "Введите свой логин";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(70, 21);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(362, 137);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 14;
            pictureBox1.TabStop = false;
            // 
            // log_in_account
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(205, 128, 0);
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(642, 584);
            Controls.Add(tabControl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "log_in_account";
            Text = "Авторизация";
            tabControl1.ResumeLayout(false);
            log_in_page.ResumeLayout(false);
            log_in_page.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1 ;
        private System.Windows.Forms.TabPage log_in_page;
        private System.Windows.Forms.Button log_in_button ;
        private System.Windows.Forms.TextBox enter_password;
        private System.Windows.Forms.TextBox enter_login;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private PictureBox pictureBox1;
    }
}