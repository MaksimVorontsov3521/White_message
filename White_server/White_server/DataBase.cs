using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using System.IO;

namespace White_server
{
    class DataBase
    {
        private string connectionString;
        public DataBase()
        {
            Thread save = new Thread(SaveDataBase);
            DateTime now = DateTime.UtcNow;
            save.Start();
            string relativePath = "Database1.mdf";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    Console.WriteLine("Подключение к БД - открыто");
                }
                else
                {
                    Console.WriteLine("Подключение к БД - не открыто");
                }
                sqlConnection.Close();

            }
        }

        private void SaveDataBase()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            DateTime LastSave = Convert.ToDateTime(config.AppSettings.Settings["LastSave"].Value);
            Console.WriteLine(LastSave.AddDays(6));
            if (LastSave.AddDays(6) > DateTime.Now)
            {
                string relativePath = "Database1.mdf"; string relativePath2 = $"Database1Copy{DateTime.UtcNow.Day}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Year}.mdf";
                string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                string destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath2);
                try
                {
                    File.Copy(source, destination);
                    DateTime now = DateTime.UtcNow;
                    config.AppSettings.Settings["LastSave"].Value = Convert.ToString(now);
                    Console.WriteLine("Успех?");
                }
                catch (IOException e)
                {
                    Console.WriteLine("Ошибка копирования Базы Данных: " + e.Message);
                }
                Thread.Sleep(3600000);
            }
        }
        public void new_message(string user, string message, string chat)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                DateTime now = DateTime.UtcNow;
                string query = $"INSERT INTO Message (Message,Receiver,Sender,ResivedTime) VALUES ('{message}','{chat}','{user}','{now}')";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                com.ExecuteNonQuery();
                sqlConnection.Close();

            }
        
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
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
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
                sqlConnection.Close();
                return message;
            }
        }
        public int Entrance(string name,string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}' AND AccountPassword='{password}'";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                int a = (int)com.ExecuteScalar();
                sqlConnection.Close();
                return a;
            }

        }
        public bool new_user(string name, string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT COUNT(*) From Accounts Where AccountName='{name}'";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                int a = (int)com.ExecuteScalar();
                if (a >= 1)
                {
                    sqlConnection.Close();
                    return false;
                }
                else
                {
                    query = $"INSERT INTO Accounts (AccountName, AccountPassword) VALUES ('{name}', '{password}')";
                    com = new SqlCommand(query, sqlConnection);
                    com.ExecuteScalar();
                    sqlConnection.Close();
                    return true;
                }                
            }
        }

    }
}
