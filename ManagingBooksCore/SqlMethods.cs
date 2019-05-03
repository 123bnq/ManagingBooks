using Microsoft.Data.Sqlite;

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
