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
using static server.api.MessengerClient;

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

            Keys keys = new Keys();
            keyExchange(clientSocket.Client, keys);

            Thread loginThread = new Thread(() =>
            {
                read_log_in_messages(clientSocket, stream, reader, writer, keys);
            });
            loginThread.Start();
        }
    }
    static async void readmessage_after_login(TcpClient clientSocket, NetworkStream stream, StreamReader reader, StreamWriter writer, string usernick, MessengerClient messengerClient, Keys keys)
    {
        refreshOnline(keys);
        int client_id = await messengerClient.GetUserIdByNick(usernick);
        ClientInfo client = Clients.FirstOrDefault(c => c.id == client_id);
        try
        {
            while (true)
            {
                string message = readmessage(reader, keys);
                if (message == null)
                {
                    await messengerClient.SetUserOffline(client_id);
                    Console.WriteLine(client.usernick + " отключился");
                    disconnect(writer, reader, clientSocket, client, stream, keys);
                    return;
                }
                else if (message == "privatemessage")
                {
                    string[] mess = new string[2];
                    for (int i = 0; i < mess.Length; i++)
                    {
                        mess[i] = readmessage(reader, keys);
                    }
                    Console.WriteLine("nick - " + mess[0]+" content - " + mess[1]);
                    sendprivatemessage(client, writer, mess[0], mess[1], keys);
                }
                else ClientWrite(message, client, keys);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка чтения данных от клиента: " + ex.Message);
        }
        finally
        {
            await messengerClient.SetUserOffline(client_id);
            Clients.Remove(client);
            refreshOnline(keys);
        }
    }
    static void sendprivatemessage(ClientInfo c, StreamWriter writer, string receivernick, string content, Keys keys)
    {
        foreach (var client in Clients)
        {
            if (client.usernick == receivernick)
            {
                byte[] encryptedMessage = keys.Encrypt("privatemessagetoyou");
                string base64Message = Convert.ToBase64String(encryptedMessage);
                client.Writer.WriteLine(base64Message);

                encryptedMessage = keys.Encrypt(c.usernick + ": " + content);
                base64Message = Convert.ToBase64String(encryptedMessage);               
                client.Writer.WriteLine(base64Message);
            }
            else if (client.usernick == c.usernick)
            {
                byte[] encryptedMessage = keys.Encrypt("privatemessagetoyou");
                string base64Message = Convert.ToBase64String(encryptedMessage);
                client.Writer.WriteLine(base64Message);

                encryptedMessage = keys.Encrypt(c.usernick + " (Вы): " + content);
                base64Message = Convert.ToBase64String(encryptedMessage);
                client.Writer.WriteLine(base64Message);
            }
        }
    }
    static async void client_log_in(TcpClient clientSocket, StreamReader reader, StreamWriter writer, NetworkStream stream, string usernick, Keys keys)
    {
        var messengerClient = new api.MessengerClient();
        int id = await messengerClient.GetUserIdByNick(usernick);
        var user = await messengerClient.GetUserById(id);
        await messengerClient.SetUserOnline(id);
        Console.WriteLine($"ID: {user.id}, nickname: {user.usernick}, login: {user.login}, password: {user.password}, fio: {user.fio}, post: {user.post}");
        if (user != null)
        {
            ClientInfo client = new ClientInfo(user.id, writer, user.usernick, user.login, user.password, user.fio, user.post, keys);
            Clients.Add(client);
            client.connect(Clients);
            Thread readThread = new Thread(() =>
            {
                readmessage_after_login(clientSocket, stream, reader, writer, user.usernick, messengerClient, keys);
            });
            readThread.Start();
        }
        else
        {
            writesystemmessage("Пользователь не найден.", writer, keys);
        }
    }

    static async void read_log_in_messages(TcpClient clientSocket, NetworkStream stream, StreamReader reader, StreamWriter writer, Keys keys)
    {
        bool stopentermessage = true;
        try
        {
            while (stopentermessage)
            {
                string message = readmessage(reader, keys);
                if (message == null) break;
                switch (message)
                {
                    case "addaccountonclients":
                        string usernick = readmessage(reader, keys);
                        if (checkmultyacc(usernick))
                        {
                            writesystemmessage("Вы успешно вошли в аккаунт", writer, keys);
                            client_log_in(clientSocket, reader, writer, stream, usernick, keys);
                            stopentermessage = false;
                        }
                        else writesystemmessage("Этот аккаунт уже используется", writer, keys);
                        break;
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка чтения данных от клиента: " + ex.Message);
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
    static string readmessage(StreamReader reader, Keys keys)
    {
        string encryptedMessage = reader.ReadLine();
        byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
        string decryptedMessage = keys.Decrypt(encryptedBytes);
        Console.WriteLine("Клиент написал (расшифровано): " + decryptedMessage + "\n");
        return decryptedMessage;
    }
    static void writesystemmessage(string message, StreamWriter writer, Keys keys)
    {
        Console.WriteLine("Клиенту отправлено: " + message + "\n");

        byte[] encryptedMessage = keys.Encrypt(message);
        string base64Message = Convert.ToBase64String(encryptedMessage);

        writer.WriteLine(base64Message);
    }
    static void clientDisconnectmessage(ClientInfo c)
    {
        foreach (var client in Clients)
        {
            if (client.id != c.id) client.Writer.WriteLine(c.usernick + " - отключился ");
            else client.Writer.WriteLine("Вы отключились");
        }
    }
    static void ClientWrite(string message, ClientInfo c, Keys keys)
    {
        foreach (var client in Clients)
        {
            byte[] encryptedMessage;
            if (client.id == c.id) encryptedMessage = keys.Encrypt(c.usernick + " (Вы): " + message);
            else encryptedMessage = keys.Encrypt(c.usernick + ": " + message);
            string base64Message = Convert.ToBase64String(encryptedMessage);
            client.Writer.WriteLine(base64Message);
            Console.WriteLine("Клиенту с ником - " + client.usernick+" отправлено: "+base64Message);
        }
    }
    static void refreshOnline(Keys keys)
    {
        foreach (var client in Clients)
        {
            Console.WriteLine("----------------------------------------------");
            byte[] encryptedMessage = keys.Encrypt("RefreshOnline");
            string base64Message = Convert.ToBase64String(encryptedMessage);
            client.Writer.WriteLine(base64Message);
        }
    }
    static void disconnect(StreamWriter writer, StreamReader reader, TcpClient clientSocket, ClientInfo client, NetworkStream stream, Keys keys)
    {
        clientDisconnectmessage(client);
        client.disconnect(Clients);
        Clients.Remove(client);
        writer.Close();
        reader.Close();
        stream.Close();
        clientSocket.Close();
        refreshOnline(keys);
    }
    private static void keyExchange(Socket clientSocket, Keys keys)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(keys.myPublicKey);
        clientSocket.Send(buffer);
        int receivedBytes = clientSocket.Receive(buffer);
        if (receivedBytes > 0) keys.NPublicKey = Encoding.UTF8.GetString(buffer);
        else clientSocket.Close();
    }
}