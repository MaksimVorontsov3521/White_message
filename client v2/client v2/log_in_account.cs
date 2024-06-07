using System;
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
                string loginResult = await messengerclient.GetnickByLog_in(enter_login.Text, enter_password.Text);
                if (loginResult == "Invalid username or password.")
                {
                    MessageBox.Show("Invalid username or password.");
                }
                else if (loginResult == "Request error" || loginResult == "Unexpected error")
                {
                    MessageBox.Show(loginResult);
                }
                else
                {
                    mess.log_in_successfully(loginResult);
                }
            }
        }
        public bool getanswer(string message)
        {
            MessageBox.Show(message);
            switch (message)
            {
                case "Такой аккаунт уже существует":
                    {
                        break;
                    }
                case "Аккаунт успешно создан":
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.tabControl1.SelectedTab = log_in_page;
                        });
                        break;
                    }
                case "Вы успешно вошли в аккаунт":
                    {
                        return true;
                    }
                case "Такой аккаунт не был найден":
                    {
                        break;
                    }
            }
            return false;
        }
    }
}
