using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using MauiApp1.BuildingNETMauiAppDirectSQLServerAccess.Models;

namespace MauiApp1
{
namespace BuildingNETMauiAppDirectSQLServerAccess.Models
    {
        public class DatabaseConnection
        {
            private string _connectionString;

            public DatabaseConnection(string connectionString)
            {
                _connectionString = connectionString;
            }
        }
    }
}