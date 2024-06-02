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
            string relativePath = "Database1.mdf";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

        }
        public void History(string history)
        {
            string[] parts = history.Split('\t');
            for (int i = 0; i < parts.Length; i +=3)
            {
                string query = $"INSERT INTO Message (Sender,Message,ResivedTime) VALUES ('{parts[i]}','{parts[i++]}','{parts[i+=2]}')";
                SqlCommand com = new SqlCommand(query, sqlConnection);
                com.ExecuteNonQuery();
            }
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
