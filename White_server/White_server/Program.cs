using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace White_server
{
    // для хранения клиентов
    class Clients
    {
        public Socket Socket { get; set; }
        public string Name { get; set; }
        public Keys Keys  { get; set; }
        public Clients(Socket socket, string name,Keys keys)
        {
            Socket = socket;
            Name = name;
            Keys = keys;
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
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        private void keyExchange(Socket clientSocket,Keys keys)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(keys.myPublicKey);
            clientSocket.Send(buffer);
            int receivedBytes = clientSocket.Receive(buffer);
            if (receivedBytes > 0)
            {
                keys.NPublicKey = Encoding.UTF8.GetString(buffer);
            }
            else { clientSocket.Close(); }

        }

        private void HandleClient(Socket clientSocket)
        {
            Keys keys = new Keys();
            keyExchange(clientSocket, keys);

            byte[] buffer = new byte[128];
            int receivedBytes = clientSocket.Receive(buffer);
            string message = keys.Decrypt(buffer);

            // ищем клиента в базе
            try { message = account(message, clientSocket); } catch { return; }
            
            if (message.StartsWith("\t"))
            {               
                clientSocket.Send(keys.Encrypt(message));
                return;
            }

            // Добавляем клиента в list<>
            Clients client = new Clients(clientSocket, message,keys);
            Clients_list.Add(client);
            // Говорим всем, что подключился клиент
            update_OnLine($"connected: {message}");
            Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint} - Name: {message}");
            clientWork(client);

        }

        private void previous(Clients client)
        {
            client.Socket.Send(client.Keys.Encrypt("\t7"));
            string message = dataBase.previous(10);
            string[] parts = message.Split('\t');
            sendLong(parts, client);
            client.Socket.Send(client.Keys.Encrypt("\t"));
        }

        private void previous(Clients clients,string User)
        {
            clients.Socket.Send(clients.Keys.Encrypt("\t9"));
            string message = dataBase.previousUser(10, clients.Name, User);
            string[] parts = message.Split('\t');
            sendLong(parts, clients);
            clients.Socket.Send(clients.Keys.Encrypt("\t"));

        }

        private void sendLong(string[] parts,Clients client)
        {
            List<byte[]> EN = new List<byte[]>();
            for (int i = 0; i < parts.Length; i++)
            {
                byte[] Encrypt = client.Keys.Encrypt(parts[i]);
                EN.Add(Encrypt);
            }
            for (int j = 0; j < EN.Count; j++)
            {
                byte[] Encrypt = EN[j];
                for (int i = 0; i < Encrypt.Length; i += 128)
                {
                    byte[] packet = new byte[128];
                    Array.Copy(Encrypt, i, packet, 0, 128);
                    client.Socket.Send(packet);
                }
            }
        }

        private void clientWork(Clients client)
        {
            previous(client);
            try
            {
                while (true)
                {
                    int receivedBytes=0;
                    // прослушиваем
                    string message = listen(client, ref receivedBytes);
                    // Если клиен отключился от сервера удаляем его из списков
                    if (receivedBytes == 0)
                    {
                        Console.WriteLine($"Client disconnected: {client.Socket.RemoteEndPoint}");
                        disconect(client); break;
                    }

                    if (message.StartsWith("\t") && message[1] == 'A')
                    {
                        previous(client,message.Substring(2));
                    }

                    if (message.StartsWith("\t"))
                    {
                        privateChat(message, client);
                    }
                    else
                    {
                        publicChat(message, client);
                    }

                }
            }
            catch (Exception ex)
            {
                // Если клиен отключился от сервера удаляем его из списков
                Console.WriteLine($"Error handling client: {ex.Message}");
                disconect(client);
            }
        }


        // общение
        private string listen(Clients client, ref int receivedBytes)
        {
            // Получаем данные от клиента в битах и переводим биты в строки
            byte[] buffer = new byte[128];
            receivedBytes = client.Socket.Receive(buffer);
            return client.Keys.Decrypt(buffer);
        }

        private void privateChat(string message, Clients client)
        {
            string[] parts = message.Split('\t');
            string answer = parts[2] + parts[3];
            try
            {
                // записываем сообщение в базу данных
                dataBase.new_message(parts[2], parts[3], client.Name);
            }
            catch
            {
                // disconect(client, e, OpenKey); break;
            }

            for (int i = 0; i < Clients_list.Count; i++)
            {
                if (Clients_list[i].Name == parts[1])
                {
                    Clients_list[i].Socket.Send(client.Keys.Encrypt(answer));
                }
            }
        }

        private void publicChat(string message, Clients client)
        {
            try
            {
                string[] parts = message.Split('\0');
                // записываем сообщение в базу данных
                dataBase.new_message("MainChat", client.Name, parts[0]);
            }
            catch
            {
                //disconect(client, e, OpenKey); break;
            }
            // Отправляем сообщение всем клиентам
            string response = $"{client.Name}: {message}";
            send(response);

        }

        // обновление списка online у клиентов
        private void update_OnLine(string message)
        {
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
            send($"\t1{message}");
        }

        // Соединение
        private string account(string message, Socket clientSocket)
        {
            // message = User1\n123\n10 - разбиваеться по \n

            string[] parts = message.Split('\n');
            string name = parts[0]; string password = parts[1]; string prev = parts[2];
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
                return name;
            }
            // Неверное имя или пароль
            else
            {
                return "\t3";
            }
        }


        private void disconect(Clients client)
        {
            Console.WriteLine($"Client disconnected: {client.Socket.RemoteEndPoint}");
            string mess = $"{client.Name} - disconnected";
            byte[] ResponseBuffer = Encoding.UTF8.GetBytes(mess);
            int indexof = Clients_list.IndexOf(client);
            Clients_list.RemoveAt(indexof);
            update_OnLine($"disconnected: {client.Name}");
            client.Socket.Close();
        }

        // отправить сообщение всем пользователям
        private void send(string message)
        {
            for (int i = 0; i < Clients_list.Count; i++)
            {               
                Clients_list[i].Socket.Send(Clients_list[i].Keys.Encrypt(message));
            }
        }
    }
}
