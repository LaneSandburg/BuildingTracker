using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer
{
    internal static class DBConn
    {
        private static string connectionString = @"Data Source = localhost; Initial Catalog = BuildingManager; Integrated Security = True";
        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            return conn;
        }
    }
}
