using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SqliteDemo
{
    public class SqliteDataBase : IDisposable
    {
        public SQLiteConnection myConnection;

        public SqliteDataBase()
        {
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                Console.WriteLine("DataBase File Created");
            }

            OpenConnection();
        }

        public void Dispose() => CloseConnection();

        public void OpenConnection()
        {
            if (myConnection.State != ConnectionState.Open)
                myConnection.Open();
        }

        public void CloseConnection()
        {
            if (myConnection.State != ConnectionState.Closed)
                myConnection.Close();
        }

        public void ExecuteQuery(string query)
        {
            using (var command = new SQLiteCommand(query, myConnection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
