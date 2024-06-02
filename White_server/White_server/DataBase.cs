using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace White_server
{
    class DataBase
    {
        private SqlConnection sqlConnection = null;
        public DataBase()
        {
            string relativePath = "Database1.mdf";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
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
            string query = $"INSERT INTO Message (Message,Receiver,Sender,ResivedTime) VALUES ('{message}','{chat}','{user}','{now}')";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            com.ExecuteNonQuery();               
        }
        public string previous(int p)
        {
            //string message = null;
            //string query = $"Select Top {p} * From Message where Receiver = '{chat}' ORDER BY ResivedTime";
            //SqlCommand com = new SqlCommand(query, sqlConnection);
            //using (SqlDataReader reader = com.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        message +=(reader["Sender"]!= DBNull.Value ? reader["Sender"].ToString() : null);
            //        message += "\t";
            //        message +=(reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null);
            //        message += "\t";
            //        message += (reader["ResivedTime"] != DBNull.Value ? reader["ResivedTime"].ToString() : null);
            //        message += "\t";
            //    }
            //}
            string message = null;
            string query = $"Select Top {p} * From Message where Receiver = 'MainChat' ORDER BY ResivedTime";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    message += (reader["Sender"] != DBNull.Value ? reader["Sender"].ToString() : null);
                    message += "\t";
                    message += (reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null);
                    message += "\t";
                    message += (reader["ResivedTime"] != DBNull.Value ? reader["ResivedTime"].ToString() : null);
                    message += "\t";
                }
            }
            return message;
        }
        public int Entrance(string name,string password)
        {
            string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}' AND AccountPassword='{password}'";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            int a =(int)com.ExecuteScalar();
            return a;
        }
        public bool new_user(string name, string password)
        {
            string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}'";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            int a = (int)com.ExecuteScalar();
            if (a >= 1)
            {
                return false;
            }
            else
            {
                query = $"INSERT INTO Accounts (AccountName, AccountPassword) VALUES ('{name}', '{password}')";
                com = new SqlCommand(query, sqlConnection);
                com.ExecuteScalar();
                return true;
            }
        }

    }
}
