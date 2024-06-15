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
using System.Timers;

namespace White_server
{
    class DataBase
    {
        private string connectionString;
        public DataBase()
        {
            Thread save = new Thread(SaveDataBase);
            //save.Start();
            string relativePath = "Database1.mdf";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    //Console.WriteLine("Подключение к БД - открыто");
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
            if (LastSave.AddDays(6) < DateTime.Now)
            {
                string relativePath = "Database1.mdf"; string relativePath2 = $"Database1Copy{DateTime.UtcNow.Day}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Year}.mdf";
                string source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                string destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath2);
                try
                {
                    File.Copy(source, destination);
                    DateTime now = DateTime.UtcNow;
                    config.AppSettings.Settings["LastSave"].Value = Convert.ToString(now);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Ошибка копирования Базы Данных: " + e.Message);
                }
            }
        }
        public void new_message(string Receiver,string Sender, string message)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                DateTime wrongnow = DateTime.UtcNow;
                string now = wrongnow.ToString("yyyy.MM.dd hh:mm:ss");
                string query = $"INSERT INTO Message (MessageText,Receiver,Sender,ResivedTime) VALUES (N'{message}',N'{Receiver}',N'{Sender}',N'{now}')";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                
                com.ExecuteNonQuery();
                sqlConnection.Close();

            }
        
        }
        public string previous(int p)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string message = null;
                string query = $"Select Top {p} * From Message where Receiver = 'MainChat' ORDER BY ResivedTime Desc";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    DateTime time;
                    while (reader.Read())
                    {
                        message += (reader["Sender"] != DBNull.Value ? reader["Sender"].ToString() : null);
                        message += "\t";
                        message += (reader["MessageText"] != DBNull.Value ? reader["MessageText"].ToString() : null);
                        message += "\t";
                        time = Convert.ToDateTime((reader["ResivedTime"] != DBNull.Value ? reader["ResivedTime"].ToString() : null));                       
                        message += time.ToString("yyyy.MM.dd hh:mm:ss");
                        message += "\t";
                        message += (reader["Receiver"] != DBNull.Value ? reader["Receiver"].ToString() : null);
                        message += "\t";
                    }
                }
                sqlConnection.Close();
                return message;
            }
        }

        public string previousUser(int p,string Sender,string Receiver)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                List<string> more = new List<string>();
                string message = null;
                string query = $"Select Top {p} * From Message where Receiver = N'{Receiver}' And Sender = N'{Sender}' ORDER BY ResivedTime Desc";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    DateTime time;
                    while (reader.Read())
                    {                     
                        message += (reader["Sender"] != DBNull.Value ? reader["Sender"].ToString() : null);
                        message += "\t";
                        message += (reader["MessageText"] != DBNull.Value ? reader["MessageText"].ToString() : null);
                        message += "\t";
                        time = Convert.ToDateTime((reader["ResivedTime"] != DBNull.Value ? reader["ResivedTime"].ToString() : null));
                        message += time.ToString("yyyy.MM.dd hh:mm:ss");
                        message += "\t";
                        message += (reader["Receiver"] != DBNull.Value ? reader["Receiver"].ToString() : null);
                        message += "\t";
                        more.Add(message);
                        message = null;
                    }
                }
                query = $"Select Top {p} * From Message where Receiver = N'{Sender}' And Sender = N'{Receiver}' ORDER BY ResivedTime  Desc";
                com = new SqlCommand(query, sqlConnection);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    DateTime time;
                    while (reader.Read())
                    {                   
                        message += (reader["Sender"] != DBNull.Value ? reader["Sender"].ToString() : null);
                        message += "\t";
                        message += (reader["MessageText"] != DBNull.Value ? reader["MessageText"].ToString() : null);
                        message += "\t";
                        time = Convert.ToDateTime((reader["ResivedTime"] != DBNull.Value ? reader["ResivedTime"].ToString() : null));
                        message += time.ToString("yyyy.MM.dd hh:mm:ss");
                        message += "\t";
                        message += (reader["Receiver"] != DBNull.Value ? reader["Receiver"].ToString() : null);
                        message += "\t";
                        more.Add(message);
                        message = null;
                    }
                }               
                sqlConnection.Close();
                List<string> one = more.Distinct().ToList();
                for (int i = 0; i < one.Count; i++)
                {
                    message += one[i];
                }
                return message;
            }
        }

        public int Entrance(string name,string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT COUNT(*) From Accounts Where AccountName=N'{name}' AND AccountPassword=N'{password}'";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                int a = (int)com.ExecuteScalar();
                sqlConnection.Close();
                return a;
            }

        }
        public bool new_user(string name, string password,string F,string I,string O,string post)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"SELECT COUNT(*) From Accounts Where AccountName=N'{name}'";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                int a = (int)com.ExecuteScalar();
                if (a >= 1)
                {
                    sqlConnection.Close();
                    return false;
                }
                else
                {
                    query = $"INSERT INTO Accounts (AccountName, AccountPassword,FirstName,SecondName,ThirdName,Post) VALUES (N'{name}', N'{password}',(N'{F}',(N'{I}',(N'{O}',(N'{post}')";
                    com = new SqlCommand(query, sqlConnection);
                    com.ExecuteScalar();
                    sqlConnection.Close();
                    return true;
                }                
            }
        }
        public int deleteUser(string name)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"Delete From Accounts where AccountName = 'N{name}'";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                return Convert.ToInt32(com.ExecuteScalar());

            }
        }
        public void allUsers()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = $"Select * From Accounts";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                      Console.Write("Username - "+(reader["AccountName"] != DBNull.Value ? reader["AccountName"].ToString() : null)+" ");
                      Console.Write("Password - "+(reader["AccountPassword"] != DBNull.Value ? reader["AccountPassword"].ToString() : null)+"\n");
                      Console.Write("ФИО - "+(reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : null) + " "+ (reader["SecondName"] != DBNull.Value ? reader["SecondName"].ToString() : null)+ " " + (reader["ThirdName"] != DBNull.Value ? reader["ThirdName"].ToString() : null)+"\n");
                      Console.Write("Post - " + (reader["Post"] != DBNull.Value ? reader["Post"].ToString() : null) + "\n");
                      Console.WriteLine("/////////////////////////////");
                    }
                }
            }
        }

    }
}
