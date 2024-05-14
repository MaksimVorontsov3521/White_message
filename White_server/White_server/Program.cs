using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace White_server
{
    class Clients
    {
        public Socket Socket { get; set; }
        public string Name { get; set; }
        public Clients(Socket socket,string name)
        {
            Socket = socket;
            Name = name;
        }
    }

    class Server
    {
        List<Clients> Clients_list = new List<Clients>();
        DataBase dataBase = null;
        static void Main(string[] args)
        {
            Server server = new Server();
            // начало работы
            server.work();
        }
        private void work()
        {
            DataBase dataBase = new DataBase();
            this.dataBase = dataBase;
            // Настройки сервера
            const int port = 8000; // Replace with your desired port
            string ipAddress = "127.0.0.1"; // Replace with your IP address or "*" for all interfaces
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Прослушка
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            serverSocket.Listen(10); // Backlog
            Console.WriteLine($"Server listening on port {port}...");

            // Подключить клиента
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                byte[] buffer = new byte[1024];

                // получаем сообщение от клиента
                int receivedBytes = clientSocket.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

                // ищем клиента в базе
                message=account(message, clientSocket);
                if (message.StartsWith("//"))
                {
                    buffer = Encoding.UTF8.GetBytes(message);
                    clientSocket.Send(buffer);
                    continue;
                }

                // Добавляем клиента в list<>
                Clients client = new Clients(clientSocket,message);
                Clients_list.Add(client);
                Thread.Sleep(100);
                // Говорим всем, что подключился клиент
                update_OnLine($"connected: {message}");
                Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint} - Name: {message}");

                // Handle client communication in a separate thread
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        
        private string account(string message, Socket clientSocket)
        {
            // Console.WriteLine($"Entered {message}");
            // message = User1\n123\n10 - разбиваеться по \n
            string name, password, prev;
            string[] parts = message.Split('\n');
            name = parts[0];
            password = parts[1];
            prev = parts[2];
            // "Этот пользователь уже в сети"
            for (int i = 0; i < Clients_list.Count; i++)
            {
                if (Clients_list[i].Name == name)
                { return "//6"; }
            }
            // Регистрация
            if (message.StartsWith("\t"))
            {// 4- Создан новый аккаунт / 5- Такой login уже существует
                name =name.Substring(1);
                if (dataBase.new_user(name, password))
                { return "//4"; }
                else { return "//5"; }

            }
            // отправка прошлых сообщений
            if (1 == dataBase.Entrance(name, password))
            {   if (prev == "0")
                {
                    return name;
                }
                // отправка прошлых сообщений
                byte[] buffer = new byte[1024];
                List<List<string>> MainChat = new List<List<string>>();
                MainChat = dataBase.previous(Convert.ToInt32(prev), "MainChat");
                string prevmessage = "//7";
                List<string> historymessage = MainChat[1];
                List<string> historyUser = MainChat[0];
                for (int i = 0; i < historyUser.Count; i++)
                {
                    prevmessage += $"{historyUser[i]}: {historymessage[i]} \t";
                }
                buffer = Encoding.UTF8.GetBytes(prevmessage);
                clientSocket.Send(buffer);
                return name;               
            }
            else
            {// Неверное имя или пароль
                return "//3";
            }
        }

        private void HandleClient(Clients client)
        {

            try
            {
                while (true)
                {
                    // Получаем данные от клиента в битах
                    byte[] buffer = new byte[1024];
                    int receivedBytes = client.Socket.Receive(buffer);

                    // Если клиен отключился от сервера удаляем его из списков
                    if (receivedBytes == 0)
                    {
                        Console.WriteLine($"Client disconnected: {client.Socket.RemoteEndPoint}");
                        disconect(client); break;
                    }

                    // Переводим биты в строки
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                   // Console.WriteLine($"Received from {client.Name}: {message}");
                    try
                    {
                        // записываем сообщение в базу данных
                        dataBase.new_message(client.Name, message, "MainChat");
                    }
                    catch
                    {
                        client.Socket.Disconnect(false);
                        disconect(client); break;
                    }


                    // Отправляем сообщения клиентам
                    string response = $"{client.Name}: {message}";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    send(responseBuffer);

                }
            }
            catch (Exception ex)
            {
                // Если клиен отключился от сервера удаляем его из списков
                Console.WriteLine($"Error handling client: {ex.Message}");
                disconect(client);
            }
            finally
            {
                // Close the client socket
                client.Socket.Close();
            }
        }

        // отправить сообщение всем пользователям
        private void send(byte[] message)
        { 
            for (int i = 0; i < Clients_list.Count; i++)
            {
                Clients_list[i].Socket.Send(message);
            }
        }

        // обновление списка online у клиентов
        private void update_OnLine(string message)
        {
            byte[] buffer = null;
            if (Clients_list.Count == 1)
            {
                message += "\t" + message.Remove(0,11);
            }
            else
            {
                for (int i = 1; i < Clients_list.Count; i++)
                {
                    message += "\t" + Clients_list[i];
                }
            }
            buffer = Encoding.UTF8.GetBytes($"//1{message}");
            send(buffer);
        }

        private void disconect(Clients client)
        {
            string mess = $"{client.Name} - disconnected";
            byte[] ResponseBuffer = Encoding.UTF8.GetBytes(mess);
            int indexof = Clients_list.IndexOf(client);
            Clients_list.RemoveAt(indexof);
            update_OnLine($"disconnected: {client.Name}");
        }

    }
}
