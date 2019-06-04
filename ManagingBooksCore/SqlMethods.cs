using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Data.SQLite;

namespace ManagingBooks
{
    static class SqlMethods
    {
        /// <summary>
        /// Create Sqlite database connection with pre-defined DataSource
        /// </summary>
        /// <param name="con">Sqlite connectiion</param>
        public static void SqlConnect(out SQLiteConnection con)
        {
            string databaseFolder = "Data";
            string databaseFile = "Database.db";
            string databasePath = Path.Combine(AppContext.BaseDirectory, databaseFolder, databaseFile);
            if (!File.Exists(databasePath))
            {
                string scriptDB = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, databaseFolder, "Database.sql"));
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, databaseFolder));
                SQLiteConnection.CreateFile(databasePath);
                SQLiteConnection connection = new SQLiteConnection(new SQLiteConnectionStringBuilder { DataSource = Path.Combine(databaseFolder, databaseFile) }.ToString());
                connection.Open();
                SQLiteCommand createDB = connection.CreateCommand();
                createDB.CommandText = scriptDB;
                createDB.ExecuteNonQuery();
                connection.Close();
            }
            con = new SQLiteConnection(new SQLiteConnectionStringBuilder { DataSource = Path.Combine(databaseFolder, databaseFile) }.ToString());
            //con = new SqliteConnection("" + new SqliteConnectionStringBuilder
            //{
            //    DataSource = "Data\\Database.db"
            //});
            con.Open();
        }
    }
}
