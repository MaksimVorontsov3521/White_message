﻿using System;
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
using System.Data;
using System.Data.OleDb;

namespace White_message
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // глобальные переменные
        string serverIP = "127.0.0.1";
        int port = 8000;
        string UserChat = null;
        Keys Keys = null;

        public MainWindow()
        {
            InitializeComponent();
            Databaseconfig();
            Keys = new Keys();
            connection();
        }
        
        // подключения
        private OleDbConnection sqlConnection = null;
        Socket clientSocket = null;
        string UserName = null;
        // обработка сообщений
        async void work()
        {
            bool t = true;
            string message = null;
            while (t)
            {
                await Task.Run(() => message = listen());
                if (message != null)
                {
                    if (message.StartsWith("\t"))
                    {
                        char three = message[1];
                        switch (three)
                        {
                            // разорванное соединение
                            case '0':
                                t = false;
                                Chat.Text += "\nCоединение разорванно\n";
                                break;
                            // кто онлайн?
                            case '1':
                                OnLine.Items.Clear();
                                string[] messages = message.Split('\n');
                                Chat.Text += messages[0].Remove(0, 2) + "\n";
                                for (int i = 1; i < messages.Length; i++)
                                {
                                    OnLine.Items.Add(messages[i]);
                                }
                                break;
                            case '2':

                                break;
                            // регистрация и аккаунты
                            case '3':
                                Chat.Text += "Неверное имя или пароль\n";
                                SettingsMessage.Content = "Неверное имя или пароль";
                                regtangle.Visibility = Visibility.Visible;
                                break;
                            case '4':
                                Chat.Text += $"Создан новый аккаунт {Name.Text}\n";
                                SettingsMessage.Content = $"Создан новый аккаунт {Name.Text}";
                                regtangle.Visibility = Visibility.Visible;
                                break;
                            case '5':
                                Chat.Text += "Такой login уже существует\n";
                                SettingsMessage.Content = "Такой login уже существует";
                                regtangle.Visibility = Visibility.Visible;
                                break;
                            case '6':
                                Chat.Text += "Этот пользователь уже в сети\n";
                                SettingsMessage.Content = "Этот пользователь уже в сети";
                                regtangle.Visibility = Visibility.Visible;
                                break;
                            // Получение прошлых сообщений
                            case '7':
                                message = message.Remove(0, 3);
                                string[] parts = message.Split('\t');
                                for (int i = 0; i < parts.Length; i++)
                                {
                                    Chat.Text += parts[i] + "\n";
                                }
                                break;
                            case '8':
                                continue;
                                break;
                        }
                    }
                    else
                    {
                        Chat.Text += message + "\n";
                    }
                }
                else { message = "\t8"; }
            }
            clientSocket.Close();
        }

        public string listen()
        {
            try
            {
                // Получите ответ от сервера
                byte[] buffer = new byte[128];
                int bytesReceived = clientSocket.Receive(buffer);              
                if (bytesReceived == 0)
                {
                    clientSocket.Close();
                    Chat.Text+= "\nCоединение разорванно\n";
                }
                if (bytesReceived > 0)
                {
                    string replyMessage = Keys.Decrypt(buffer);                

                    return replyMessage;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void send_all()
        {
            string message = Message.Text;
            for (int i = 0; i < message.Length; i += 250)
            {
                string block = UserChat+message.Substring(i, Math.Min(250, message.Length - i));
                if (UserChat != null)
                { Chat.Text += "\n"+message; }
                clientSocket.Send(Keys.Encrypt(block));
            }
            Message.Clear();
        }


        // Соединение
        public void connection()
        {
            // Создайте сокет TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Подключитесь к серверу
                clientSocket.Connect(IPAddress.Parse(serverIP), port);
                keyExchange();

                // Отправьте сообщение серверу
                string message = Name.Text + "\n" + Password.Text + "\n" + 10;
                UserName = Name.Text;
                clientSocket.Send(Keys.Encrypt(message));
                SettingsMessage.Content = "Вы подключились";
                regtangle.Visibility = Visibility.Hidden;
                work();                
            }
            catch
            {
                //Сервер не доступен
                Chat.Text += "Сервер не доступен\n";
                SettingsMessage.Content = "Сервер не доступен";
                clientSocket.Close();
            }
        }
        private void keyExchange()
        {
            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            if (receivedBytes > 0)
            {
                Keys.NPublicKey = Encoding.UTF8.GetString(buffer);
            }
            else { clientSocket.Close(); }
            buffer = Encoding.UTF8.GetBytes(Keys.myPublicKey);
            clientSocket.Send(buffer);

        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            connection();
        }
        private void disConnect_Click(object sender, RoutedEventArgs e)
        {
            clientSocket.Close();
            OnLine.Items.Clear();
            //clientSocket = null;
            regtangle.Visibility = Visibility.Visible;          
        }

        // работа offline
        private void Save()
        {
            DateTime now = DateTime.UtcNow;
            string query = $"Insert Into Offline (Message,TimeSended,Chat_name,UserName) VALUES ('{Message.Text}','{now}','MainChat','{Name.Text}')";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            com.ExecuteNonQuery();
            Chat.Text += $"Offline {Name.Text}: {Message.Text}";
        }
        public void offlinebuf()
        {
            string[] offmesages = new string[3];
            string query = $"Select Message,TimeSended,Chat_name From Offline";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            using (OleDbDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    offmesages[0] = (reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null);
                    offmesages[1] = (reader["TimeSended"] != DBNull.Value ? reader["TimeSended"].ToString() : null);
                    offmesages[2] = (reader["Chat_name"] != DBNull.Value ? reader["Chat_name"].ToString() : null);
                    Thread.Sleep(10);
                    byte[] buffer = Encoding.UTF8.GetBytes(offmesages[0]);
                    clientSocket.Send(buffer);
                }
            }
        }

        // база данных
        private void Databaseconfig()
        {
            // подключение к базе данных
            string relativePath = "Data.accdb";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fullPath};";
            sqlConnection = new OleDbConnection(connectionString);
            sqlConnection.Open();

            // Обращение к базе данных для данных пользователя
            string query = $"Select PasswordUser,UserName,LengthP From Account";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            using (OleDbDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    Name.Text = (reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : null);
                    Password.Text = (reader["PasswordUser"] != DBNull.Value ? reader["PasswordUser"].ToString() : null);
                    PrevTbox.Text = (reader["LengthP"] != DBNull.Value ? reader["LengthP"].ToString() : null);
                }
            }
        }

        //кнопка для отправки сообщений и enter
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                send_all();
            }
            catch
            {
                Save();
                Message.Clear();
            }
        }
        private void Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { send_all(); }
        }

        // изменение вида окна
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
        private void ChangeViewDark_Click(object sender, RoutedEventArgs e)
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

        // регистрация 
        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            // Создайте сокет TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Подключитесь к серверу
                clientSocket.Connect(IPAddress.Parse(serverIP), port);

                // Отправьте сообщение серверу
                string message = "\t" + Name.Text + "\n" + Password.Text+ "\n" + " ";
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

            if (Name.Text.Contains('\t'))
            {
                Name.Text = Name.Text.Replace('\t', ' ');
                SettingsMessage.Content = "Недопустимый символ";
            }
            if (Name.Text.Contains('\n'))
            {
                Name.Text = Name.Text.Replace('\n', ' ');
                SettingsMessage.Content = "Недопустимый символ";
            }
        }
        // ограничения для password
        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Password.Text.Contains('\t'))
            {
                Password.Text = Password.Text.Replace('\t', ' ');
                SettingsMessage.Content = "Недопустимый символ";
            }
            if (Name.Text.Contains('\n'))
            {
                Name.Text = Name.Text.Replace('\n', ' ');
                SettingsMessage.Content = "Недопустимый символ";
            }
        }
        
        // не помню что это 
        private void PrivAply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"Update Account Set LengthP = {Convert.ToInt32(PrevTbox.Text)},UserName = '{Name.Text}',PasswordUser = '{Password.Text}'";
                OleDbCommand com = new OleDbCommand(query, sqlConnection);
                com.ExecuteNonQuery();
                SettingsMessage.Content = "Изменения приняты";
            }
            catch
            {
                SettingsMessage.Content = "Неверный формат ввода ";
            }


        }
        
        // ограничения для чата
        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Message.Text.Contains('\t'))
            {
                Message.Text = Message.Text.Replace('\t', ' ');
                SettingsMessage.Content = "Недопустимый символ";
            }
        }

        // изменение чатов
        private void OnLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Convert.ToString(OnLine.SelectedItem) == UserName)
            {
                WhatChat.Content = "It is you";
            }
            else
            {
                WhatChat.Content = OnLine.SelectedItem;
            }
            
            UserChat = "\t" + OnLine.SelectedItem + "\t" + $"From {UserName} \t";
           
        }
        private void BackMainChat_Click(object sender, RoutedEventArgs e)
        {
            WhatChat.Content = "Main";
            UserChat = null;
        }
    }
}