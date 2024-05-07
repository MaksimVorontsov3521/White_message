using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Reflection;

namespace White_message
{

    public partial class MainWindow : Window
    {    //сервер
        string serverIP = "192.168.88.18";
        int port = 8000;
        public MainWindow()
        {
            InitializeComponent();
        }
        Socket clientSocket = null;
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            connection();
        }
        public void connection()
        {

            // Создайте сокет TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Подключитесь к серверу
                clientSocket.Connect(IPAddress.Parse(serverIP), port);
                // Отправьте сообщение серверу
                string message = Name.Text + "\n" + Password.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(buffer);
                Connect.Visibility = Visibility.Hidden;
                disConnect.Visibility = Visibility.Visible;
                int p = 10;
                previousMainChat(p);
                work();
            }
            catch
            {
                Connect.Visibility = Visibility.Visible;
                disConnect.Visibility = Visibility.Hidden;
                Chat.Text += "Сервер не доступен\n";
            }

        }
        private void previousMainChat(int p)
        {
            string message = $"{p}";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(buffer);
        }

        async void work()
        {
            bool t = true;
            string message = null;
            while (t)
            {
                await Task.Run(() => message = listen());
                if (message != null)
                {
                    bool DoubleSlash = message.StartsWith("//");
                    if (DoubleSlash)
                    {
                        char three = message[2];
                        switch (three)
                        {
                            // разорванное соединение
                            case '0':
                                t = false;
                                Chat.Text += "Сервер разорвал соединение\n";
                                break;
                            // кто онлайн?
                            case '1':
                                OnLine.Items.Add(message.Substring(3));
                                break;
                            case '2':
                                OnLine.Items.Clear();
                                break;
                            // регистрация и аккаунты
                            case '3':
                                Chat.Text += "Неверное имя или пароль\n";
                                break;
                            case '4':
                                Chat.Text += $"Создан новый аккаунт {Name.Text}\n";
                                break;
                            case '5':
                                Chat.Text += "Такой login уже существует\n";
                                break;
                        }
                    }
                    else
                    { Chat.Text += message + "\n"; }

                }

            }
            clientSocket.Close();
        }
        public string listen()
        {
            try
            {
                // Получите ответ от сервера
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);
                string replyMessage = null;
                if (bytesReceived > 0)
                {
                    replyMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    // Выведите ответ сервера
                }
                return replyMessage;
            }
            catch
            {
                clientSocket.Close();
                //clientSocket = null;
                return "//0";
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                send_all();
            }
            catch
            {
                Chat.Text += "Сервер разорвал соединение\n";

                Message.Clear();
            }
        }

        private void send_all()
        {
            string message = Message.Text;
            for (int i = 0; i < message.Length; i += 250)
            {
                string block = message.Substring(i, Math.Min(250, message.Length - i));
                byte[] buffer = Encoding.UTF8.GetBytes(block);
                clientSocket.Send(buffer);
            }
            Message.Clear();
        }

        private void Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { send_all(); }
        }

        private void ChangeView_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("не сейчас");
            //string relativePath = "Data\\MainView.xaml";
            //string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            //string buff;
            //using (StreamReader reader = new StreamReader(fullPath))
            //{
            //    buff = reader.ReadToEnd();
            //}

            //string Toofar = AppDomain.CurrentDomain.BaseDirectory;
            //Toofar = Toofar.Substring(0, Toofar.Length - 11);
            //Toofar += "\\MainWindow.xaml";

            //File.WriteAllText(Toofar, buff);
            //this.Close();           
        }


        private void ChangeViewShit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("не сейчас");
            //string relativePath = "Data\\SecondView.xaml";
            //string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            //string buff;
            //using (StreamReader reader = new StreamReader(fullPath))
            //{
            //    buff = reader.ReadToEnd();
            //}

            //string Toofar = AppDomain.CurrentDomain.BaseDirectory;
            //Toofar = Toofar.Substring(0, Toofar.Length - 11);
            //Toofar += "\\MainWindow.xaml";
            ////MessageBox.Show(Toofar);

            //File.WriteAllText(Toofar, buff);
            //this.Close();
        }

        private void disConnect_Click(object sender, RoutedEventArgs e)
        {
            clientSocket.Close();
            OnLine.Items.Clear();
            //clientSocket = null;
            disConnect.Visibility = Visibility.Hidden;
            Connect.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// регистрация и сопутствующие методы
        /// </summary>

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            // Создайте сокет TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Подключитесь к серверу
                clientSocket.Connect(IPAddress.Parse(serverIP), port);

                // Отправьте сообщение серверу
                string message = "\t" + Name.Text + "\n" + Password.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(buffer);
                work();
            }
            catch
            {
                Chat.Text += "Сервер не доступен\n";
            }
        }
        // ограничения нужны потомучто я дебил и отправляю команды как сообщения с // в начале. Я знаю - это очень тупо и не безопастно
        // ограничения для username
        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Name.Text.Contains('/'))
            {
                Name.Text = Name.Text.Replace('/', ' ');
            }
            if (Name.Text.Contains('\t'))
            {
                Name.Text = Name.Text.Replace('\t', ' ');
            }
            if (Name.Text.Contains('\n'))
            {
                Name.Text = Name.Text.Replace('\n', ' ');
            }
        }
        // ограничения для password
        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Password.Text.Contains('\t'))
            {
                Password.Text = Password.Text.Replace('\t', ' ');
            }
            if (Name.Text.Contains('\n'))
            {
                Name.Text = Name.Text.Replace('\n', ' ');
            }
        }
    }
}
