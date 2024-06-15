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
        TcpListener serverSocket = new TcpListener(IPAddress.Any, 6666);
        serverSocket.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключения...\n");
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
    static async void readmessage_after_login(TcpClient clientSocket, NetworkStream stream, StreamReader reader, StreamWriter writer, string usernick, MessengerClient messengerClient)
    {
        refreshOnline();
        int client_id = await messengerClient.GetUserIdByNick(usernick);
        ClientInfo client = Clients.FirstOrDefault(c => c.id == client_id);
        try
        {
            while (true)
            {
                string message = readmessage(reader);
                if (message == null)
                {
                    // Клиент отключился
                    await messengerClient.SetUserOffline(client_id);
                    Console.WriteLine(client.usernick + " отключился");                   
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
            await messengerClient.SetUserOffline(client_id);            
            Clients.Remove(client);
            refreshOnline();
        }
    }
    static void sendprivatemessage(ClientInfo c, StreamWriter writer, string receivernick, string content)
    {
        foreach (var client in Clients)
        {
            if (client.usernick == receivernick)
            {
                client.Writer.WriteLine("privatemessagetoyou");
                client.Writer.WriteLine(c.usernick + ": " + content);
            }
            else if (client.usernick == c.usernick)
            {
                client.Writer.WriteLine("privatemessagetoyou");
                client.Writer.WriteLine(c.usernick + " (Вы): " + content);
            }

        }
    }
    static async void client_log_in(TcpClient clientSocket, StreamReader reader, StreamWriter writer, NetworkStream stream, string usernick)
    {
        var messengerClient = new api.MessengerClient();
        int id = await messengerClient.GetUserIdByNick(usernick);
        var user = await messengerClient.GetUserById(id);
        await messengerClient.SetUserOnline(id);
        Console.WriteLine($"ID: {user.id}, nickname: {user.usernick}, login: {user.login}, password: {user.password}, fio: {user.fio}, post: {user.post}");
        if (user != null)
        {
            ClientInfo client = new ClientInfo(user.id, writer, user.usernick, user.login, user.password, user.fio, user.post);
            Clients.Add(client);
            client.connect(Clients);
            Thread readThread = new Thread(() =>
            {
                readmessage_after_login(clientSocket, stream, reader, writer, user.usernick, messengerClient);
            });
            readThread.Start();
        }
        else
        {
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
                    case "addaccountonclients":
                        string usernick = readmessage(reader);
                        if (checkmultyacc(usernick))
                        {
                            writesystemmessage("Вы успешно вошли в аккаунт", writer);
                            client_log_in(clientSocket, reader, writer, stream, usernick);
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
        finally
        {

        }
    }
    static bool checkmultyacc(string nick)
    {
        foreach (var client in Clients)
        {
            if (client.usernick == nick) return false;
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
            if (client.id != c.id) client.Writer.WriteLine(c.usernick + " - отключился ");
            else client.Writer.WriteLine("Вы отключились");
        }
    }
    static void ClientWrite(string message, ClientInfo c)
    {
        foreach (var client in Clients)
        {
            if (client.id == c.id) client.Writer.WriteLine(c.usernick + " (Вы): " + message);
            else client.Writer.WriteLine(c.usernick + ": " + message);
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
            client.Writer.WriteLine("RefreshOnline");
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