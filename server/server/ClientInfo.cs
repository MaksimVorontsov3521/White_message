using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class ClientInfo
    {
        public StreamWriter Writer { get; set; }
        public int id { get; set; }
        public string Nickname { get; set; }
        private string login { get; set; }
        private string password { get; set; }

        public ClientInfo(int id, StreamWriter writer, string nickname, string login, string password)
        {
            Writer = writer;
            Nickname = nickname;
            this.id = id;
            this.login = login;
            this.password = password;
        }
        public void connect(List<ClientInfo> clients)
        {
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Клиент подключился:");
            Console.WriteLine("id Клиента: " + id + "\nКлиент установил себе ник - '" + Nickname + "'");
            Console.WriteLine("Клиентов на сервере: " + (clients.Count) + "");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++\n");
        }
        public void disconnect(List<ClientInfo> clients)
        {
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Клиент отключился:");
            Console.WriteLine("id Клиента: " + id + "\nНикнейм - '" + Nickname + "' отключился");
            Console.WriteLine("Клиентов на сервере: " + (clients.Count-1) + "");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++\n");
        }
    }
}
