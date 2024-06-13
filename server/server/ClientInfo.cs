using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class ClientInfo
    {
        public StreamWriter Writer { get; set; }
        public int id { get; set; }
        public string fio { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string post { get; set; }
        public string usernick { get; set; }

        public ClientInfo(int id, StreamWriter writer, string usernick, string login, string password,string fio, string post)
        {
            Writer = writer;
            this.id = id;
            this.fio = fio;
            this.login = login;
            this.password = password;           
            this.post = post;
            this.usernick = usernick;
        }
        public void connect(List<ClientInfo> clients)
        {
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Клиент подключился:");
            Console.WriteLine("id Клиента: " + id + "\nКлиент установил себе ник - '" + usernick + "'");
            Console.WriteLine("Клиентов на сервере: " + (clients.Count) + "");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++\n");
        }
        public void disconnect(List<ClientInfo> clients)
        {
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Клиент отключился:");
            Console.WriteLine("id Клиента: " + id + "\nНикнейм - '" + usernick + "' отключился");
            Console.WriteLine("Клиентов на сервере: " + (clients.Count-1) + "");
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++\n");
        }
    }
}
