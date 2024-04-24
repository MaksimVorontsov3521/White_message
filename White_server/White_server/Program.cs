using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace White_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.listen();          
        }
        List<Socket> Users = new List<Socket>();
        public void listen()
        {
            // Укажите порт для прослушивания
            int port = 8000;
            // Создайте сокет TCP
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Привяжите сокет к порту
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            while (true)
            {
                // Прослушивайте входящие соединения
                serverSocket.Listen(10);

                Console.WriteLine($"Сервер запущен на порту {port}. Ожидаем подключения...");

                // Примите подключение от клиента
                Socket clientSocket = serverSocket.Accept();

                // Получите данные от клиента
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);

                // Преобразуйте данные в строку
                string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                Users.Add(clientSocket);
                // Выведите сообщение от клиента
                for (int i = 0; i < Users.Count; i++)
                {
                    // Отправьте сообщение серверу клиенту
                    string replyMessage = ($"new user {message} joined");
                    byte[] replyBuffer = Encoding.ASCII.GetBytes(replyMessage);
                    Users[i].Send(replyBuffer);
                }
                Console.WriteLine($"new user {message}");
                // Закройте сокеты
                //clientSocket.Close();
                //serverSocket.Close();
            }

        }
    }
}
