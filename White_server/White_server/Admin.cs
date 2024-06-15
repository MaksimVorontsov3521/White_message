using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace White_server
{
    class Admin
    {
        public void Adminwork()
        {
            DataBase dataBase = new DataBase();
            write();
            while (true) 
            {
                string comand = Console.ReadLine();
                switch (comand)
                {
                    case "0":
                        Console.Clear();
                        break;
                    case "1":
                        Console.WriteLine("Введите имя");
                        string name = Console.ReadLine();
                        Console.WriteLine("Введите пароль");
                        string password = Console.ReadLine();
                        Console.WriteLine("ФИО");
                        string[] FIO =Console.ReadLine().Split(' ');
                        Console.WriteLine();
                        string post = Console.ReadLine();
                        try
                        {
                            Console.WriteLine(dataBase.new_user(name, password, FIO[0], FIO[1], FIO[2],post));
                        }
                        catch { Console.WriteLine("Ф И О"); }
                        break;
                    case "2":
                        Console.WriteLine("Введите имя");
                        string delname = Console.ReadLine();
                        Console.WriteLine(dataBase.deleteUser(delname));
                        break;
                    case "3":
                        dataBase.allUsers();
                        break;
                    default:
                        Console.WriteLine("Неверная команда");
                        break;
                }
                write();
            }
        }
        private void write()
        {
            Console.WriteLine("0 - очистить консоль\n" +
                "1 - добавить пользователя\n" +
                "2 - удалить пользователя\n" +
                "3 - все пользователи");
        }
    }
}
