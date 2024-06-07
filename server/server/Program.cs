using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using server;
using System.Windows.Markup;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static server.api;
using System.Reflection.Metadata;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class Server
{
    static List<ClientInfo> Clients = new List<ClientInfo>();
    public readonly MessengerClient messengerclient;


    static void Main(string[] args)//начало 
    {
        //bd.deleteclients(bd.getcs());
        TcpListener serverSocket = new TcpListener(IPAddress.Any, 6666);
        serverSocket.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключения...\n");

        Thread writeThread = new Thread(() =>
        {
            writemessage();
        });
        writeThread.Start();
        while (true)
        {
            TcpClient clientSocket = serverSocket.AcceptTcpClient();
            NetworkStream stream = clientSocket.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            Console.WriteLine("Клиент подключён");
            Thread loginThread = new Thread(() =>
            {
                read_log_in_messages(clientSocket, stream, reader, writer);
            });
            loginThread.Start();
        }
    }
    static async void readmessage_after_login(TcpClient clientSocket, NetworkStream stream, StreamReader reader, StreamWriter writer, string nickname, MessengerClient messengerClient)
    {
        refreshOnline();
        var user = await messengerClient.GetUserByNick(nickname);
        int client_id = user.Id;
        ClientInfo client = Clients.FirstOrDefault(c => c.id == client_id);
        try
        {
            while (true)
            {
                string message = readmessage(reader);
                if (message == null)
                {
                    // Клиент отключился
                    Console.WriteLine(client.Nickname + " отключился");
                    disconnect(writer, reader, clientSocket, client, stream);
                    return;
                }
                else if (message == "privatemessage")
                {
                    string[] mess = new string[2]; // mess[0] - nick, mess[1] - content
                    for (int i = 0; i < mess.Length; i++)
                    {
                        mess[i] = readmessage(reader);
                    }
                    sendprivatemessage(client, writer, mess[0], mess[1]);
                }
                else ClientWrite(message, client);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка чтения данных от клиента: " + ex.Message);
        }
        finally //DISCONNECT
        {
            Clients.Remove(client);
        }
    }
    static void sendprivatemessage(ClientInfo c, StreamWriter writer, string receivernick, string content)
    {
        foreach (var client in Clients)
        {
            if (client.Nickname == receivernick)
            {
                client.Writer.WriteLine("privatemessagetoyou");
                client.Writer.WriteLine(c.Nickname + ": " + content);
            }
            else if (client.Nickname == c.Nickname)
            {
                client.Writer.WriteLine("privatemessagetoyou");
                client.Writer.WriteLine(c.Nickname + " (Вы): " + content);
            }

        }
    }
    //static bool client_reg_in(TcpClient clientSocket,StreamReader reader)
    //    {
    //        string[] client_data = new string[3];       
    //        Array.Clear(client_data, 0, client_data.Length);

    //            for (int i = 0; i < client_data.Length; i++)
    //            {               
    //                client_data[i] = readmessage(reader);
    //            }
    //        if (/*bd.checkipforreg(bd.getcs(),getIP(clientSocket))==true &&*/ bd.checkclientsreg(bd.getcs(), client_data[2], client_data[0])==true)
    //        {
    //            bd.insertclient(bd.getcs(), bd.getlastclient_id(bd.getcs()), client_data[2], client_data[0], client_data[1],getIP(clientSocket));            
    //            return true;
    //        }
    //        else return false;
    //    }
    static async void client_log_in(TcpClient clientSocket, StreamReader reader, StreamWriter writer, NetworkStream stream, string nickname)
    {
        var messengerClient = new api.MessengerClient();
        var user = await messengerClient.GetUserByNick(nickname);
        Console.WriteLine($"ID: {user.Id}, nickname: {user.nickname}, login: {user.login}, password: {user.password}");
        if (user != null)
        {

            ClientInfo client = new ClientInfo(user.Id, writer, user.nickname, user.login, user.password);
            Clients.Add(client);
            client.connect(Clients);
            Thread readThread = new Thread(() =>
            {
                readmessage_after_login(clientSocket, stream, reader, writer, user.nickname, messengerClient);
            });
            readThread.Start();
        }
        else
        {
            // Обработка случая, когда пользователь не найден
            writesystemmessage("Пользователь не найден.", writer);
        }
    }

    static async void read_log_in_messages(TcpClient clientSocket, NetworkStream stream, StreamReader reader, StreamWriter writer)
    {
        bool stopentermessage = true;
        try
        {
            while (stopentermessage)
            {
                string message = readmessage(reader);
                if (message == null) break;
                switch (message)
                {
                    //case "checkaccountreg":
                    //    if (client_reg_in(clientSocket,reader))
                    //    {
                    //        Console.WriteLine("Аккаунт успешно создан");
                    //        writesystemmessage("Аккаунт успешно создан", writer);
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Такой аккаунт уже существует");
                    //        writesystemmessage("Такой аккаунт уже существует", writer);
                    //    }
                    //    break;
                    case "addaccountonclients":
                        string nickname = readmessage(reader);
                        if (checkmultyacc(nickname))
                        {
                            writesystemmessage("Вы успешно вошли в аккаунт", writer);
                            client_log_in(clientSocket, reader, writer, stream, nickname);
                            stopentermessage = false;
                        }
                        else writesystemmessage("Этот аккаунт уже используется", writer);
                        break;
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка чтения данных от клиента: " + ex.Message);
        }
        finally //DISCONNECT
        {

        }
    }
    static bool checkmultyacc(string nick)
    {
        foreach (var client in Clients)
        {
            if (client.Nickname == nick) return false;
        }
        return true;
    }
    static string readmessage(StreamReader reader)
    {
        string message = reader.ReadLine();
        Console.WriteLine("Клиент написал: " + message + "\n");
        return message;
    }
    static void writesystemmessage(string message, StreamWriter writer)
    {
        Console.WriteLine("Клиенту отправлено: " + message + "\n");
        writer.WriteLine(message);

    }
    static void clientDisconnectmessage(ClientInfo c)
    {
        foreach (var client in Clients)
        {
            if (client.id != c.id) client.Writer.WriteLine(c.Nickname + " - отключился ");
            else client.Writer.WriteLine("Вы отключились");
        }
    }
    static void ClientWrite(string message, ClientInfo c)
    {
        foreach (var client in Clients)
        {
            if (client.id == c.id) client.Writer.WriteLine(c.Nickname + " (Вы): " + message);
            else client.Writer.WriteLine(c.Nickname + ": " + message);
        }
    }
    static void ServerWrite(string message)
    {
        foreach (var client in Clients)
        {
            client.Writer.WriteLine("Сервер: " + message);
        }
    }
    static void writemessage()
    {
        while (true)
        {
            string message = Console.ReadLine();
            ServerWrite(message);
        }
    }
    static void refreshOnline()
    {
        foreach (var client in Clients)
        {
            Console.WriteLine("----------------------------------------------");
            // Очищаем список клиентов перед обновлением
            client.Writer.WriteLine("RefreshOnline");
            client.Writer.WriteLine(Clients.Count);
            Console.WriteLine("Для клиента - '" + client.Nickname + "' - было отправлено:");
            Console.WriteLine("Количество клиентов на сервере: " + Clients.Count);
            foreach (var c in Clients)
            {
                if (c.id == client.id)
                {
                    client.Writer.WriteLine(c.Nickname + " (Вы)");
                    Console.WriteLine(c.Nickname + " (Вы)" + "    for " + client.Nickname);
                }
                else
                {
                    client.Writer.WriteLine(c.Nickname);
                    Console.WriteLine(c.Nickname + "    for " + client.Nickname);
                }
            }
            Console.WriteLine("----------------------------------------------");
        }
    }
    static string getIP(TcpClient clientSocket)
    {
        IPAddress clientIP = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address;
        return clientIP.ToString();
    }
    static void disconnect(StreamWriter writer, StreamReader reader, TcpClient clientSocket, ClientInfo client, NetworkStream stream)
    {
        clientDisconnectmessage(client);
        client.disconnect(Clients);
        Clients.Remove(client);
        writer.Close();
        reader.Close();
        stream.Close();
        clientSocket.Close();
        refreshOnline();
    }
}