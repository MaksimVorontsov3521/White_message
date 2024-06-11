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
            tabControl1 = new TabControl();
            log_in_page = new TabPage();
            log_in_button = new Button();
            enter_password = new TextBox();
            enter_login = new TextBox();
            label5 = new Label();
            label6 = new Label();
            tabControl1.SuspendLayout();
            log_in_page.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(log_in_page);
            tabControl1.Location = new Point(28, 14);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(201, 333);
            tabControl1.TabIndex = 0;
            // 
            // log_in_page
            // 
            log_in_page.Controls.Add(log_in_button);
            log_in_page.Controls.Add(enter_password);
            log_in_page.Controls.Add(enter_login);
            log_in_page.Controls.Add(label5);
            log_in_page.Controls.Add(label6);
            log_in_page.Location = new Point(4, 24);
            log_in_page.Margin = new Padding(4, 3, 4, 3);
            log_in_page.Name = "log_in_page";
            log_in_page.Padding = new Padding(4, 3, 4, 3);
            log_in_page.Size = new Size(193, 305);
            log_in_page.TabIndex = 1;
            log_in_page.Text = "Вход";
            log_in_page.UseVisualStyleBackColor = true;
            // 
            // log_in_button
            // 
            log_in_button.Location = new Point(9, 218);
            log_in_button.Margin = new Padding(4, 3, 4, 3);
            log_in_button.Name = "log_in_button";
            log_in_button.Size = new Size(155, 51);
            log_in_button.TabIndex = 13;
            log_in_button.Text = "Войти";
            log_in_button.UseVisualStyleBackColor = true;
            log_in_button.Click += log_in_button_Click;
            // 
            // enter_password
            // 
            enter_password.Location = new Point(13, 107);
            enter_password.Margin = new Padding(4, 3, 4, 3);
            enter_password.Name = "enter_password";
            enter_password.Size = new Size(151, 23);
            enter_password.TabIndex = 10;
            // 
            // enter_login
            // 
            enter_login.Location = new Point(13, 36);
            enter_login.Margin = new Padding(4, 3, 4, 3);
            enter_login.Name = "enter_login";
            enter_login.Size = new Size(151, 23);
            enter_login.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 89);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(122, 15);
            label5.TabIndex = 8;
            label5.Text = "Введите свой пароль";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(9, 17);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(115, 15);
            label6.TabIndex = 7;
            label6.Text = "Введите свой логин";
            // 
            // log_in_account
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(257, 375);
            Controls.Add(tabControl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "log_in_account";
            Text = "Авторизация";
            tabControl1.ResumeLayout(false);
            log_in_page.ResumeLayout(false);
            log_in_page.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage log_in_page;
        private System.Windows.Forms.Button log_in_button;
        private System.Windows.Forms.TextBox enter_password;
        private System.Windows.Forms.TextBox enter_login;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}