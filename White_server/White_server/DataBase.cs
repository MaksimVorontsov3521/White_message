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
            sqlConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings["Sqlcon"].ConnectionString);
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
        public void new_message(string message)
        { 
        
        }

    }
}
