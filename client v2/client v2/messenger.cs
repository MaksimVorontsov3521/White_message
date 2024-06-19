using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System;
using static client_v2.api;
using static client_v2.api.MessengerClient;
using System.ComponentModel;
using System.Collections.Immutable;

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
        bool stopreading = true;
        public int myId;
        private List<UserOnline> users;
        private List<ContactsOnline> contacts;
        private Keys keys;

        public messenger()
        {
            InitializeComponent();


            messengerclient = new MessengerClient();
            clientmenu.Opening += ContextMenuStrip_Opening;
            online.SelectedIndexChanged += new EventHandler(online_SelectedIndexChanged);
            online.DrawMode = DrawMode.OwnerDrawFixed;
            online.SelectionMode = SelectionMode.One;
            online.DrawItem += new DrawItemEventHandler(online_DrawItem);

            yourcontacts.DrawMode = DrawMode.OwnerDrawFixed;
            yourcontacts.SelectionMode = SelectionMode.One;
            yourcontacts.DrawItem += new DrawItemEventHandler(yourcontacts_DrawItem);

            //buttonpress();
            main();
        }
        private void messenger_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
        //private void buttonpress()
        //{
        //    if (tabControl1.SelectedTab == groupchat) this.AcceptButton = this.send;
        //    else if (tabControl1.SelectedTab == personmessages) this.AcceptButton = this.personalsend;
        //}
        private void online_SelectedIndexChanged(object sender, EventArgs e)
        {
            online.Refresh();
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
            keys = new Keys();
            keyExchange();
        }
        private async void SetGroupMessages()
        {
            var messages = await messengerclient.GetGroupMessages();
            if (messages == null) return;
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
                while (stopreading)
                {
                    string encryptedMessage = reader.ReadLine();
                    if (encryptedMessage != null)
                    {
                        string message = keys.Decrypt(Convert.FromBase64String(encryptedMessage));
                        if (message == "RefreshOnline")
                        {
                            ClearOnline();
                            refreshonline();
                        }
                        else if (message == "privatemessagetoyou")
                        {
                            message = keys.Decrypt(Convert.FromBase64String(reader.ReadLine()));
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
            await messengerclient.SendGroupMessage(message, myId);
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
        private async void refreshonline()
        {
            users = await messengerclient.GetAllUsersOnline();
            contacts = await messengerclient.GetContactsStatus(myId);
            UpdateListBox();
            online.Invoke((MethodInvoker)delegate
            {
                online.Refresh();
            });
        }
        private void UpdateListBox()
        {
            foreach (var user in users)
            {
                online.Invoke((MethodInvoker)delegate
                {
                    if (user.UserNick == yournick.Text) online.Items.Add(user.UserNick);
                    else online.Items.Add(user.UserNick);
                });
            }
        }
        private async void online_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox == null) return;
            string userNick = listBox.Items[e.Index].ToString();
            UserOnline user = users.FirstOrDefault(u => u.UserNick == userNick);
            Color circleColor = GetUserOnlineStatusColor(user.IsOnline);

            int circleSize = 8;
            int circleOffset = 4;
            int circleX = e.Bounds.Left + circleOffset;
            int circleY = e.Bounds.Top + circleOffset;

            if (user.UserNick == yournick.Text)
            {
                circleColor = Color.Black;
            }
            e.Graphics.FillEllipse(new SolidBrush(circleColor), circleX, circleY, circleSize, circleSize);
            e.Graphics.DrawString(userNick, e.Font, Brushes.Black, new PointF(circleX + circleSize + 2, e.Bounds.Top));
            e.DrawFocusRectangle();
        }

        public static Color GetUserOnlineStatusColor(bool isOnline)
        {
            if (isOnline) return Color.Green;
            else return Color.Red;
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
                byte[] responseBuffer = Encoding.UTF8.GetBytes("checkaccountreg\n");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
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
        public void sendmessage(string message)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(Convert.ToBase64String(keys.Encrypt(message)) + "\n");
            stream.Write(responseBuffer, 0, responseBuffer.Length);
        }
        public async void log_in_successfully(string usernick)
        {
            try
            {
                sendmessage("addaccountonclients");
                sendmessage(usernick);
                string answer = keys.Decrypt(Convert.FromBase64String(reader.ReadLine()));
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
                        yournick.Text = usernick;
                    });
                    IsYouOnline.Invoke((MethodInvoker)delegate
                    {
                        IsYouOnline.Text = "Вы в сети под никнеймом:";
                    });
                    SetGroupMessages();
                    string[] contacts = await messengerclient.getcontacts(myId);
                    if (contacts != null && contacts.Length != 0)
                    {
                        foreach (string contact in contacts)
                        {
                            yourcontacts.Items.Add(contact);
                        }
                        yourcontacts.SelectedIndex = 0;
                    }
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
        private async void Refreshcontacts()
        {
            if (online.SelectedItem != null)
            {
                string selectedUser = online.SelectedItem.ToString();
                if (!yourcontacts.Items.Contains(selectedUser))
                {
                    if (selectedUser != (yournick.Text))
                    {
                        yourcontacts.Items.Add(selectedUser);
                        tabControl1.SelectedTab = personmessages;
                        int contactid = await messengerclient.GetUserIdByNick(selectedUser);
                        await messengerclient.AddContact(myId, contactid);
                    }
                    else MessageBox.Show("Вы не можете добавить себя в список контактов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (yourcontacts.Items.Contains(selectedUser))
                {
                    tabControl1.SelectedTab = personmessages;
                    yourcontacts.SelectedItem = selectedUser;
                    update_contacts();
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
                sendmessage("privatemessage");
                string selectedUser = yourcontacts.SelectedItem.ToString();
                sendmessage(selectedUser);
                sendmessage(personalmess.Text);
                int receiverid = await messengerclient.GetUserIdByNick(selectedUser);
                await messengerclient.SendPrivateMessage(myId, receiverid, personalmess.Text);
                personalmess.Clear();
            }
            else MessageBox.Show("Выберите пользователя, которому хотите отправить сообщение");
        }
        private void yourcontacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            yourcontacts.Refresh();
            personalchat.Clear();
            update_contacts();
            contact.Text = yourcontacts.SelectedItem.ToString();
        }
        private async void update_contacts()
        {
            if (yourcontacts.SelectedItem != null)
            {
                int receiverid = await messengerclient.GetUserIdByNick(yourcontacts.SelectedItem.ToString());
                var messages = await messengerclient.GetPrivateMessages(myId, receiverid);
                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        if (message.Sendernick == yournick.Text) AddMessageToPersonalTextBox(message.Sendernick + " (Вы): " + message.Content);
                        else AddMessageToPersonalTextBox(message.Sendernick + ": " + message.Content);
                    }
                }
            }
        }
        private void yourcontacts_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox == null || e.Index < 0) return;
            string usernick = yourcontacts.Items[e.Index].ToString();
            ContactsOnline contact = contacts.FirstOrDefault(c => c.UserNick == usernick);
            if (contact == null) return;

            Color circleColor = contact.IsOnline ? Color.Green : Color.Red;
            int circleSize = 10;
            int circleOffset = 2;
            int circleX = e.Bounds.Left + circleOffset;
            int circleY = e.Bounds.Top + circleOffset + (e.Bounds.Height - circleSize) / 2;
            using (Brush circleBrush = new SolidBrush(circleColor))
            {
                e.Graphics.FillEllipse(circleBrush, circleX, circleY, circleSize, circleSize);
            }
            string userNick = contact.UserNick;
            using (Font font = new Font(FontFamily.GenericSansSerif, 10))
            {
                e.Graphics.DrawString(userNick, font, Brushes.Black, new PointF(circleX + circleSize + 5, e.Bounds.Top));
            }
            e.DrawFocusRectangle();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonpress();
            if (tabControl1.SelectedTab == personmessages && yourcontacts.Items.Count > 0)
            {
                update_contacts();
                if (online.SelectedItem != null)
                {
                    string selectedUser = online.SelectedItem.ToString();
                    if (yourcontacts.Items.Contains(selectedUser))
                    {
                        yourcontacts.SelectedItem = selectedUser;
                    }
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
            if (tabControl1.SelectedTab != groupchat)
            {
                e.Cancel = true;
            }
        }
        private void reconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            ClearOnline();
            this.Hide();
            closeform = true;
            stopreading = false;
            main();
        }
        private void keyExchange()
        {
            byte[] buffer = new byte[1024];
            int receivedBytes = stream.Read(buffer, 0, buffer.Length);
            if (receivedBytes > 0)
            {

                keys.NPublicKey = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            }
            else
            {
                client.Close();
            }
            buffer = Encoding.UTF8.GetBytes(keys.myPublicKey);
            stream.Write(buffer, 0, buffer.Length);
        }
        private void send_group_Click(object sender, EventArgs e)
        {
            if (writer != null)
            {
                if (mess.Text == null) mess.Text = "";
                sendmessage(mess.Text);
                AddMessegeoToApi(mess.Text);
                mess.Clear();
            }
        }

        private void personmessages_Click(object sender, EventArgs e)
        {

        }
    }
}