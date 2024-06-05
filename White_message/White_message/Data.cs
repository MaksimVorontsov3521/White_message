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
        // UserName = Sender +-
        // Chat_name = Receiver +-
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
            if (history == null) { return; }
            string[] parts = history.Split('\t'); int m, t, r;
            for (int i = 0; i < parts.Length - 4; i += 4)
            {
                m = i + 1;
                t = i + 2;
                r = i + 3;
                string query = $"INSERT INTO MessagesTab (UserName,Message,TimeSended,Chat_name) VALUES (N'{parts[i]}',N'{parts[m]}','{parts[t]}',N'{parts[r]}')";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                com.ExecuteNonQuery();
            }
        }

        public string Showhistory(string Chat_name,string Username)
        {
            string message = null;
            string query = $"Select * From MessagesTab where (Chat_name=N'{Chat_name}' and UserName=N'{Username}') or" +
                $" (Chat_name=N'{Username}' and UserName=N'{Chat_name}') ORDER BY TimeSended,Id";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    message += "from ";
                    message += (reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : null);                  
                    message += "To ";
                    message += (reader["Chat_name"] != DBNull.Value ? reader["Chat_name"].ToString() : null);
                    message += ": ";
                    message += (reader["Message"] != DBNull.Value ? reader["Message"].ToString() : null);
                    message += "\n";                    
                }
            }           
            return message;
        }

        public int anyMessagesFromUser(string User)
        {
            if (User == "") { return 10; }
            string query = $"SELECT COUNT(*) From MessagesTab Where Chat_name ='{User}'";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            int a = (int)com.ExecuteScalar();
            return a;
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
        public void newMessage(string message)
        {
            string[] parts = message.Split('\t');
            DateTime now = DateTime.UtcNow;
            string query = $"INSERT INTO MessagesTab (UserName,Message,TimeSended,Chat_name) VALUES (N'{parts[0]}',N'{parts[1]}',N'{now}',N'{parts[2]}')";
            SqlCommand com = new SqlCommand(query, sqlConnection);
            com.ExecuteNonQuery();
        }

    }
}
