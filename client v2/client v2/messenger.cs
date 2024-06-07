using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System;
using static client_v2.api;
using static client_v2.api.MessengerClient;

namespace client_v2
{
    public partial class messenger : Form
    {
        string serverIP = "127.0.0.1"; // Указываем IP адрес сервера
        int serverPort = 6666;
        int ListIndex = 0;
        private log_in_account loginForm;
        private NetworkStream stream;
        private StreamWriter writer;
        private TcpClient client;
        private StreamReader reader;
        bool closeform = true;
        private readonly MessengerClient messengerclient;
        public messenger()
        {
            InitializeComponent();           
            messengerclient = new MessengerClient();
            clientmenu.Opening += ContextMenuStrip_Opening;
            buttonpress();
            // Привязываем контекстное меню только к tabPage1
            main();

        }
        private void messenger_Shown(object sender, EventArgs e)
        {
            this.Hide(); // Скрыть основную форму при загрузке
        }
        private void buttonpress()
        {
            if (tabControl1.SelectedTab == groupchat) this.AcceptButton = this.send;
            else if (tabControl1.SelectedTab == personmessages) this.AcceptButton = this.personalsend;
        }

        private void main()
        {
            loginForm = new log_in_account(this);
            loginForm.FormClosing += log_in_account_FormClosing;
            this.Shown += new EventHandler(messenger_Shown);
            loginForm.Show();


            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }
        private async void SetGroupMessages()
        {
            var messages = await messengerclient.GetGroupMessages();
            foreach (var m in messages)
            {
                if (m.Sendernick == yournick.Text) AddMessageToGroupTextBox(m.Sendernick + " (Вы): " + m.Content);
                else AddMessageToGroupTextBox(m.Sendernick + ": " + m.Content);
            }
        }
        private void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
        private void readmessage(NetworkStream stream)
        {
            StreamReader reader = new StreamReader(stream);
            try
            {
                while (true)
                {
                    string message = reader.ReadLine();
                    if (message != null)
                    {
                        if (message == "RefreshOnline")
                        {
                            ClearOnline();
                            readonline(reader);
                        }
                        else if (message == "privatemessagetoyou")
                        {
                            message = reader.ReadLine();
                            AddMessageToPersonalTextBox(message);
                        }
                        else
                        {
                            AddMessageToGroupTextBox(message);
                        }
                    }
                }
            }
            catch { }
        }
        private async void AddMessegeoToApi(string message)
        {
            await messengerclient.SendGroupMessage(message, yournick.Text);
        }
        private void AddMessageToGroupTextBox(string message)
        {
            if (chat.InvokeRequired)
            {
                chat.Invoke((MethodInvoker)delegate
                {
                    AddMessageToGroupTextBox(message);
                });
            }
            else
            {
                chat.AppendText(message + Environment.NewLine);
            }
        }
        private void AddMessageToPersonalTextBox(string message)
        {
            if (personalchat.InvokeRequired)
            {
                personalchat.Invoke((MethodInvoker)delegate
                {
                    AddMessageToPersonalTextBox(message);
                });
            }
            else
            {
                if (tabControl1.SelectedTab == personmessages) personalchat.AppendText(message + Environment.NewLine);
            }
        }
        private void readonline(StreamReader reader)
        {
            int index = 0;
            int count = getCount(reader);
            try
            {
                while (index < count)
                {
                    string message = reader.ReadLine();
                    online.Invoke((MethodInvoker)delegate
                    {
                        online.Items.Add(message);
                    });
                    ListIndex = online.Items.Count - 1;
                    index++;
                }
            }
            catch { }
        }
        private int getCount(StreamReader reader)
        {
            string message = reader.ReadLine();
            return int.Parse(message);
        }
        private void ClearOnline()
        {
            online.Invoke((MethodInvoker)delegate
            {
                online.Items.Clear();
            });
        }

        public void checkaccountreg(string[] regmassive)
        {
            try
            {
                // Отправка команды "checkaccountreg"
                byte[] responseBuffer = Encoding.UTF8.GetBytes("checkaccountreg\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                // Отправка данных регистрации
                for (int i = 0; i < regmassive.Length; i++)
                {
                    responseBuffer = Encoding.UTF8.GetBytes(regmassive[i] + "\n");
                    stream.Write(responseBuffer, 0, responseBuffer.Length);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ошибка при отправке данных: " + ex.Message);
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (writer != null)
            {
                if (mess.Text == null) mess.Text = "";
                byte[] responseBuffer = Encoding.UTF8.GetBytes(mess.Text + "\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                AddMessegeoToApi(mess.Text);
                mess.Clear();
            }
        }

        private void disconn_Click(object sender, EventArgs e)
        {
            // Отключаем текущее соединение
            Disconnect();
            ClearOnline();

            // Скрываем текущее окно и показываем форму логина
            this.Hide();
            closeform = true;

            // Подключаемся заново
            main();
        }
        private void log_in_account_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeform == true)
            {
                loginForm.Invoke((MethodInvoker)delegate
                {
                    closeform = false;
                    loginForm.Close();
                    this.Close();
                });
            }
        }
        public void log_in_successfully(string nickname)
        {
            StreamReader reader = new StreamReader(stream);
            try
            {
                byte[] responseBuffer = Encoding.UTF8.GetBytes("addaccountonclients\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                responseBuffer = Encoding.UTF8.GetBytes(nickname + "\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                string answer = reader.ReadLine();
                if (answer == "Этот аккаунт уже используется")
                {
                    MessageBox.Show(answer);
                    return;
                }
                else if (answer == "Вы успешно вошли в аккаунт")
                {
                    Thread readThread = new Thread(() =>
                    {
                        readmessage(stream);
                    })
                    {
                        IsBackground = true
                    };
                    readThread.Start();
                    loginForm.Invoke((MethodInvoker)delegate
                    {
                        closeform = false;
                        loginForm.Close();
                    });
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Show();
                    });
                    yournick.Invoke((MethodInvoker)delegate
                    {
                        yournick.Text = nickname;
                    });
                    IsYouOnline.Invoke((MethodInvoker)delegate
                    {
                        IsYouOnline.Text = "Вы в сети под никнеймом:";
                    });
                    SetGroupMessages();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ошибка при отправке данных: " + ex.Message);
            }
        }
        private void ls_Click(object sender, EventArgs e)
        {
            Refreshcontacts();
        }
        private void Refreshcontacts()
        {
            if (online.SelectedItem != null)
            {
                string selectedUser = online.SelectedItem.ToString();
                if (!yourcontacts.Items.Contains(selectedUser)) // Проверяем, не содержится ли уже выбранный пользователь в списке
                {
                    if (selectedUser != (yournick.Text + " (Вы)")) // Проверяем, не содержится ли уже ваше имя в списке
                    {
                        yourcontacts.Items.Add(selectedUser);
                        tabControl1.SelectedTab = personmessages;
                    }
                    else MessageBox.Show("Вы не можете добавить себя в список контактов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (yourcontacts.Items.Contains(selectedUser))
                {
                    tabControl1.SelectedTab = personmessages;
                    yourcontacts.SelectedItem = selectedUser;
                    update_contacts();
                }
            }
        }

        private async void personalsend_Click(object sender, EventArgs e)
        {
            if (yourcontacts.Items.Count > 0)
            {
                if (mess.Text == null) mess.Text = "";
                string selectedUser = yourcontacts.SelectedItem.ToString();
                byte[] responseBuffer = Encoding.UTF8.GetBytes("privatemessage" + "\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                responseBuffer = Encoding.UTF8.GetBytes(selectedUser + "\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                responseBuffer = Encoding.UTF8.GetBytes(personalmess.Text + "\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);

                await messengerclient.SendPrivateMessage(yournick.Text, selectedUser, personalmess.Text);
                personalmess.Clear();

            }
            else MessageBox.Show("Выберите пользователя, которому хотите отправить сообщение");
        }

        private void yourcontacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            personalchat.Clear();
            update_contacts();
            contact.Text = yourcontacts.SelectedItem.ToString();

        }
        private async void update_contacts()
        {
            if (yourcontacts.SelectedItem != null)
            {
                var messages = await messengerclient.GetPrivateMessages(yournick.Text, yourcontacts.SelectedItem.ToString());
                foreach (var message in messages)
                {
                    if (message.Sendernick == yournick.Text) AddMessageToPersonalTextBox(message.Sendernick + " (Вы): " + message.Content);
                    else AddMessageToPersonalTextBox(message.Sendernick + ": " + message.Content);
                }
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonpress();
            if (tabControl1.SelectedTab == personmessages && yourcontacts.Items.Count > 0)
            {
                update_contacts();
                string selectedUser = online.SelectedItem.ToString();
                if (yourcontacts.Items.Contains(selectedUser))
                {
                    yourcontacts.SelectedItem = selectedUser;
                }
            }
            if (tabControl1.SelectedTab == groupchat)
            {
                personalchat.Clear();

            }
        }
        private void IsYouOnline_TextChanged(object sender, EventArgs e)
        {
            IsYouOnlinePers.Text = IsYouOnline.Text;
        }

        private void yournick_TextChanged(object sender, EventArgs e)
        {
            yournickpers.Text = yournick.Text;
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Проверяем, если активная вкладка не tabPage1
            if (tabControl1.SelectedTab != groupchat)
            {
                // Отменяем отображение контекстного меню
                e.Cancel = true;
            }
        }
    }
}