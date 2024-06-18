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
            log_in_button = new RoundedButton();
            enter_password = new TextBox();
            enter_login = new TextBox();
            label5 = new Label();
            label6 = new Label();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // log_in_button
            // 
            log_in_button.BackColor = Color.FromArgb(255, 128, 0);
            log_in_button.Cursor = Cursors.Hand;
            log_in_button.ForeColor = SystemColors.ControlText;
            log_in_button.Location = new Point(345, 353);
            log_in_button.Margin = new Padding(4, 3, 4, 3);
            log_in_button.Name = "log_in_button";
            log_in_button.Size = new Size(155, 51);
            log_in_button.TabIndex = 18;
            log_in_button.Text = "Войти";
            log_in_button.UseVisualStyleBackColor = false;
            log_in_button.Click += log_in_button_Click;
            // 
            // enter_password
            // 
            enter_password.BackColor = Color.FromArgb(255, 128, 0);
            enter_password.Location = new Point(345, 301);
            enter_password.Margin = new Padding(4, 3, 4, 3);
            enter_password.Name = "enter_password";
            enter_password.Size = new Size(155, 23);
            enter_password.TabIndex = 17;
            // 
            // enter_login
            // 
            enter_login.BackColor = Color.FromArgb(255, 128, 0);
            enter_login.Location = new Point(345, 257);
            enter_login.Margin = new Padding(4, 3, 4, 3);
            enter_login.Name = "enter_login";
            enter_login.Size = new Size(155, 23);
            enter_login.TabIndex = 16;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(255, 128, 0);
            label5.Location = new Point(363, 283);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(122, 15);
            label5.TabIndex = 15;
            label5.Text = "Введите свой пароль";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.FromArgb(255, 128, 0);
            label6.Location = new Point(363, 239);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(115, 15);
            label6.TabIndex = 14;
            label6.Text = "Введите свой логин";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImage = Properties.Resources.WhiteMessageLogoV2__1_;
            panel1.Location = new Point(131, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(627, 236);
            panel1.TabIndex = 19;
            // 
            // log_in_account
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.FonWM;
            ClientSize = new Size(842, 505);
            Controls.Add(panel1);
            Controls.Add(log_in_button);
            Controls.Add(enter_password);
            Controls.Add(enter_login);
            Controls.Add(label5);
            Controls.Add(label6);
            Margin = new Padding(4, 3, 4, 3);
            Name = "log_in_account";
            Text = "Авторизация";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RoundedButton log_in_button;
        private TextBox enter_password;
        private TextBox enter_login;
        private Label label5;
        private Label label6;
        private Panel panel1;
    }
}