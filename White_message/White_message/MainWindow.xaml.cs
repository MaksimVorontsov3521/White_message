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
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            string serverIP = "127.0.0.1";

            // Укажите порт сервера
            int port = 8000;

            // Создайте сокет TCP
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Подключитесь к серверу
            clientSocket.Connect(IPAddress.Parse(serverIP), port);

            // Отправьте сообщение серверу
            string message = Name.Text;
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(buffer);
            listen(clientSocket);

            // Закройте сокет
            //clientSocket.Close();
        }
        async void listen(Socket clientSocket)
        {
            // Получите ответ от сервера
            byte[] buffer = new byte[1024];
            int bytesReceived = clientSocket.Receive(buffer);
            string replyMessage = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            // Выведите ответ сервера
            Chat.Text += replyMessage+"\n";
            await Task.Delay(500);
            listen(clientSocket);
        }
    }
}
