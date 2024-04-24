using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace White_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.work();
            
        }
        List<Socket> Users = new List<Socket>();
        async void work()
        {
            Thread myThread = new Thread(connect);
            // запускаем поток myThread
            myThread.Start();
            while (true)
            {               
                listen_all();
            }
        }

        public void listen_all()
        {
            byte[] buffer = new byte[1024];
            for (int i = 0; i < Users.Count; i++)
            {             
                int bytesReceived = Users[i].Receive(buffer);
                if (bytesReceived > 0)
                {              
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine("new message - "+message);
                    for (int j = 0; j < Users.Count; j++)
                    {
                        // Отправьте сообщение серверу клиентам
                        string replyMessage = ($"{message}");
                        byte[] replyBuffer = Encoding.ASCII.GetBytes(replyMessage);
                        Users[j].Send(replyBuffer);
                    }
                }                   
            }
        }
        public void connect()
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
                    // Отправьте сообщение серверу клиентам
                    string replyMessage = ($"new user {message} joined");
                    byte[] replyBuffer = Encoding.ASCII.GetBytes(replyMessage);
                    Users[i].Send(replyBuffer);
                }
                Console.WriteLine($"new user {message}");
            }
        }
    }
}
