using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ModernApp.MVC.Model
{
    internal class DBconnection
    {
        private readonly string _connectionString = "Server=localhost; Database=POSDBTEST; Uid=root; Pwd=QWE12as@;";

        public MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            //MessageBox.Show("Connected successfull");
            return connection;
        }
    }
}
