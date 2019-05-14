using Microsoft.Data.Sqlite;

namespace ManagingBooks
{
    static class SqlMethods
    {
        /// <summary>
        /// Create Sqlite database connection with pre-defined DataSource
        /// </summary>
        /// <param name="con">Sqlite connectiion</param>
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
