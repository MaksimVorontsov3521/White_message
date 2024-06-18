﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static client_v2.api;

namespace client_v2
{
    public partial class log_in_account : Form
    {
        private readonly MessengerClient messengerclient;
        private messenger mess;
        public log_in_account(messenger messangerForm)
        {
            InitializeComponent();

            this.mess = messangerForm;
            messengerclient = new MessengerClient();
            

        }
        private async void log_in_button_Click(object sender, EventArgs e)
        {
            if (enter_login.Text != "" && enter_password.Text != "")
            {
                if (!await messengerclient.cheacaccounttologin(enter_login.Text, enter_password.Text))
                {
                    MessageBox.Show("Неправильный логин или пароль");
                    return;
                }
                mess.myId = await messengerclient.GetUserIdByLogin(enter_login.Text);
                string usernick = await messengerclient.GetUsernickById(mess.myId);
                if (usernick == "Invalid username or password.")
                {
                    MessageBox.Show("Invalid username or password.");
                }
                else if (usernick == "Request error" || usernick == "Unexpected error")
                {
                    MessageBox.Show(usernick);
                }
                else
                {
                    mess.log_in_successfully(usernick);
                }
            }
            else MessageBox.Show("Заполните логин и пароль");
        }

        private void log_in_account_Load(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath myPath =
            new System.Drawing.Drawing2D.GraphicsPath();
            myPath.AddEllipse(0, 0, log_in_button.Width, log_in_button.Height);

            Region myRegion = new Region(myPath);
            log_in_button.Region = myRegion;
        }
    }
}
