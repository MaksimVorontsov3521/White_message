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
        public Clients(Socket socket, string name)
        {
            Socket = socket;
            Name = name;
        }
    }

    class Server
    {
        List<Clients> Clients_list = new List<Clients>();
        DataBase dataBase = null;
        Keys Keys = null;       
        static void Main(string[] args)
        {
            Server server = new Server();
            // начало работы
            server.work();
        }
        private string account(string message, Socket clientSocket, int e, uint OpenKey)
        {
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
                { return "\t6"; }
            }

            // Регистрация

            if (message.StartsWith("\t"))
            {
                // 4- Создан новый аккаунт / 5- Такой login уже существует
                name = name.Substring(1);
                return (dataBase.new_user(name, password)) ? "\t4" : "\t5";
            }

            // отправка прошлых сообщений
            if (1 == dataBase.Entrance(name, password))
            {
                if (prev == "0")
                {
                    return name;
                }

                // отправка прошлых сообщений
                byte[] buffer = new byte[1024];
                string prevmessage = "\t7";

                List<List<string>> MainChat = new List<List<string>>();

                MainChat = dataBase.previous(Convert.ToInt32(prev), "MainChat");

                List<string> historymessage = MainChat[1];
                List<string> historyUser = MainChat[0];

                for (int i = 0; i < historyUser.Count; i++)
                {
                    prevmessage += $"{historyUser[i]}: {historymessage[i]} \t";
                }

                buffer = Encoding.UTF8.GetBytes(prevmessage);
                buffer = Keys.coder(buffer, e, OpenKey);
                clientSocket.Send(buffer);

                return name;
            }
            else
            {// Неверное имя или пароль
                return "\t3";
            }
        }

        private void workKeys(Socket clientSocket,ref int e, ref uint OpenKey)
        {
            try
            {
                string message; byte[] buffer = new byte[1024];
                // send
                message = Convert.ToString(Keys.giveOpen_e()) + "\t" + Convert.ToString(Keys.giveOpenkey());
                buffer = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(buffer);
                // Receive
                int receivedBytes = clientSocket.Receive(buffer);
                if (receivedBytes > 0)
                {                   
                    message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    string[] keys = message.Split('\t');
                    e = Convert.ToInt32(keys[0]); OpenKey = Convert.ToUInt32(keys[1]);
                }
            }
            catch { }
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
                // Handle client communication in a separate thread
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            int e = 0; uint OpenKey = 0;
            Keys = new Keys();
            workKeys(clientSocket, ref e, ref OpenKey);

            byte[] buffer = new byte[1024];

            int receivedBytes = clientSocket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            message = Keys.decoder(message, receivedBytes);

            // ищем клиента в базе
            try { message = account(message, clientSocket, e, OpenKey); } catch { return; }
            
            if (message.StartsWith("\t"))
            {
                buffer = Encoding.UTF8.GetBytes(message);
                buffer = Keys.coder(buffer, e, OpenKey);
                clientSocket.Send(buffer);
                return;
            }

            // Добавляем клиента в list<>
            Clients client = new Clients(clientSocket, message);
            Clients_list.Add(client);
            Thread.Sleep(100);
            // Говорим всем, что подключился клиент
            update_OnLine($"connected: {message}",e,OpenKey);
            Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint} - Name: {message}");

            try
            {
                while (true)
                {
                    // Получаем данные от клиента в битах
                    buffer = new byte[1024];
                    receivedBytes = client.Socket.Receive(buffer);
                    Console.WriteLine(receivedBytes);

                    // Если клиен отключился от сервера удаляем его из списков
                    if (receivedBytes == 0)
                    {
                        Console.WriteLine($"Client disconnected: {client.Socket.RemoteEndPoint}");
                        disconect(client,e,OpenKey); break;
                    }

                    // Переводим биты в строки
                    message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    message = Keys.decoder(message, receivedBytes);
                    if (message.StartsWith("\t"))
                    {
                        string[] parts = message.Split('\t');
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(parts[2] + parts[3]);
                        responseBuffer = Keys.coder(responseBuffer, e, OpenKey);
                        try
                        {
                            // записываем сообщение в базу данных
                            dataBase.new_message(parts[2], parts[3], client.Name);
                        }
                        catch
                        {
                            disconect(client,e,OpenKey); break;
                        }

                        for (int i = 0; i < Clients_list.Count; i++)
                        {
                            if (Clients_list[i].Name == parts[1])
                            {
                                Clients_list[i].Socket.Send(responseBuffer);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            string[] parts = message.Split('\0');
                            // записываем сообщение в базу данных
                            dataBase.new_message(client.Name, parts[0], "MainChat");

                        }
                        catch
                        {
                            disconect(client,e,OpenKey); break;
                        }
                        // Отправляем сообщение всем клиентам
                        string response = $"{client.Name}: {message}";
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                        responseBuffer = Keys.coder(responseBuffer, e, OpenKey);
                        send(responseBuffer);
                    }

                }
            }
            catch (Exception ex)
            {
                // Если клиен отключился от сервера удаляем его из списков
                Console.WriteLine($"Error handling client: {ex.Message}");
                disconect(client,e,OpenKey);
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
        private void update_OnLine(string message,int e, uint OpenKey)
        {
            byte[] buffer = null;
            if (Clients_list.Count == 1)
            {
                message += "\n" + message.Remove(0, 11);
            }
            else
            {
                for (int i = 0; i < Clients_list.Count; i++)
                {
                    message += "\n" + Clients_list[i].Name;
                }
            }
            buffer = Encoding.UTF8.GetBytes($"\t1{message}");
            buffer = Keys.coder(buffer,e,OpenKey);
            send(buffer);
        }

        private void disconect(Clients client, int e, uint OpenKey)
        {
            Console.WriteLine($"Client disconnected: {client.Socket.RemoteEndPoint}");
            string mess = $"{client.Name} - disconnected";
            byte[] ResponseBuffer = Encoding.UTF8.GetBytes(mess);
            int indexof = Clients_list.IndexOf(client);
            Clients_list.RemoveAt(indexof);
            update_OnLine($"disconnected: {client.Name}",e,OpenKey);
            client.Socket.Close();
        }

    }
}
