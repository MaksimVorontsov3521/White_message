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

namespace White_message
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Socket clientSocket = null;
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            //Thread con = new Thread(connection);
            //con.Start();
            connection();
        }
        public void connection()
        {
            string serverIP = "127.0.0.1";
            // Укажите порт сервера
            int port = 8000;

            // Создайте сокет TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Подключитесь к серверу
                clientSocket.Connect(IPAddress.Parse(serverIP), port);

                // Отправьте сообщение серверу
                string message = Name.Text;
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                clientSocket.Send(buffer);
                work();
            }
            catch { MessageBox.Show("Сервер не доступен"); }
            // Закройте сокет
            //clientSocket.Close();
        }
        async void work()
        {
            string message=null;
            while (true)
            {
                await Task.Run(()=> message=listen());
                if (message != null)
                {
                    bool DoubleSlash = message.StartsWith("//");
                    if (DoubleSlash)
                    {
                        char three = message[2];
                        switch (three)
                        {
                            case '1':
                                OnLine.Items.Add(message.Substring(3));
                                break;
                            case '2':
                                OnLine.Items.Clear();
                                break;
                        }
                    }
                    else
                    { Chat.Text += message + "\n"; }
                    
                }
                
            }
        }
        public string listen()
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

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = Message.Text;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(buffer);
            Message.Clear();
        }
    }
}
