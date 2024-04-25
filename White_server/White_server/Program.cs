using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace White_server
{

    class Server
    {
        List<Socket> Sockets = new List<Socket>();
        List<string> names = new List<string>();
            
        static void Main(string[] args)
        {
            Server server = new Server();
            server.work();
        }
        private void send(byte[] message)
        {
            for (int i = 0; i < Sockets.Count; i++)
            {
                Sockets[i].Send(message);
            }
        }
        private void work()
        {
            // Server settings
            const int port = 8000; // Replace with your desired port
            string ipAddress = "127.0.0.1"; // Replace with your IP address or "*" for all interfaces

            // Create server socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind and start listening
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            serverSocket.Listen(10); // Backlog (max queued connections)

            Console.WriteLine($"Server listening on port {port}...");

            // Accept and handle clients
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                byte[] buffer = new byte[1024];
                // Добавляем клиента в list<>
                int receivedBytes = clientSocket.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                names.Add(message);
                Sockets.Add(clientSocket);
                // Говорим всем, что подключился клиент
                Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint}\nName: {message}");               
                buffer = Encoding.UTF8.GetBytes($"connected: {message}");
                send(buffer); buffer = null;
                Thread.Sleep(10);
                buffer = Encoding.UTF8.GetBytes($"//2");
                send(buffer);buffer = null;
                Thread.Sleep(10);
                for (int i = 0; i < names.Count; i++)
                {
                    buffer = Encoding.UTF8.GetBytes($"//1 {names[i]}");
                    send(buffer);buffer = null;
                    Thread.Sleep(10);
                }
                // Handle client communication in a separate thread
                Thread clientThread = new Thread(() => HandleClient(clientSocket, names.Count - 1));
                clientThread.Start();
            }
        }

        private void HandleClient(Socket clientSocket,int name)
        {
            try
            {
                while (true)
                {
                    // Receive data from the client
                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);

                    if (receivedBytes == 0)
                    {
                        Console.WriteLine($"Client disconnected: {clientSocket.RemoteEndPoint}");
                        string mess = $"{names[name]} - disconnected";
                        string Response = $"{names[name]}: {mess}";
                        byte[] ResponseBuffer = Encoding.UTF8.GetBytes(Response);
                        Sockets.RemoveAt(name);
                        names.RemoveAt(name);     
                        send(ResponseBuffer);
                        break;
                    }
                    // Process received data (e.g., convert to string, handle message)
                    string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine($"Received from {names[name]}: {message}");
                    // Send a message back to the client
                    string response = $"{names[name]}: {message}";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    send(responseBuffer);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                // Close the client socket
                clientSocket.Close();
            }
        }
    }

}
