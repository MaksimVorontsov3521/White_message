using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace White_server
{
    class DataBase
    {
        private OleDbConnection sqlConnection = null;
        public DataBase()
        {
            string relativePath = "Data\\SrverDB.accdb";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fullPath};";
            sqlConnection = new OleDbConnection(connectionString);
            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Open)
            {
                Console.WriteLine("Подключение к БД - открыто");
            }
            else
            {
                Console.WriteLine("Подключение к БД - не открыто");
            }
        }
        public void new_message(string user,string message,string chat)
        {
            DateTime now = DateTime.UtcNow;
            string query = $"INSERT INTO {chat} (NickName,Message,ResivedTime) VALUES ('{user}','{message}','{now}')";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            com.ExecuteNonQuery();                        
        }
        public int Entrance(string name,string password)
        {
            string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}' AND AccountPassword='{password}'";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            int a =(int)com.ExecuteScalar();
            return a;
        }
        public bool new_user(string name, string password)
        {
            string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}'";
            OleDbCommand com = new OleDbCommand(query, sqlConnection);
            int a = (int)com.ExecuteScalar();
            if (a >= 1)
            {
                return false;
            }
            else
            {
                query = $"INSERT INTO Accounts (AccountName, AccountPassword) VALUES (' {name} ', ' {password} ')";
                com = new OleDbCommand(query, sqlConnection);
                com.ExecuteScalar();
                return true;
            }
        }

    }
}
