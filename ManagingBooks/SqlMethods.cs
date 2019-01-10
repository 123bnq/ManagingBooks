using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingBooks
{
    static class SqlMethods
    {
        public static void SqlConnect(out SqliteConnection con)
        {
            con = new SqliteConnection("" + new SqliteConnectionStringBuilder
            {
                DataSource = "Data\\Database.db"
            });
            con.Open();
        }
    }
}
