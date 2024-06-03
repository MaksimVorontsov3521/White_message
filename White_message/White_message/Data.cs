using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace White_message
{
    class Data
    {
        private SqlConnection sqlConnection = null;
        public Data()
        {
            // подключение к базе данных
            string relativePath = "Database1C.mdf";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            string query = "Delete From MessagesTab";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            com.ExecuteNonQuery();

        }
        public void History(string history)
        {
            string[] parts = history.Split('\t');int  m, r;
            for (int i = 0; i < parts.Length-3; i +=3)
            {
                m = i + 1;
                r = i + 2;
                string query = $"INSERT INTO MessagesTab (UserName,Message,TimeSended,Chat_name) VALUES (N'{parts[i]}',N'{parts[m]}','{parts[r]}',N'MainChat')";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                com.ExecuteNonQuery();
            }
        }

        public string Showhistory(string Chat_name)
        {
            string message = null;
            string query = $"Select Message,UserName From MessagesTab where Chat_name=N'{Chat_name}' ORDER BY TimeSended";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    message += (reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : null);
                    message += (reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null);
                    message += "\n";
                    
                }
            }           
            return message;
        }

        public string autoName()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings["Login"].Value;
        }
        public string autoPassword()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return  config.AppSettings.Settings["Password"].Value;
        }

    }
}
