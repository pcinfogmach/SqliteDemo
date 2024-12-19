using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteDemo
{
    internal class Sample
    {
        public Sample()
        {
            using (var db = new SqliteDataBase())
            {
                CreateTable(db);
                InsertOrUpdateSampleData(db);

                Console.WriteLine("\nInitial Data:");
                DisplayUserData(db);

                UpdateUserName(db, 1, "Johnathan Doe");
                GetUserById(db, 1);

                DeleteUserById(db, 2);
                Console.WriteLine("\nData After Deletion:");
                DisplayUserData(db);
            }

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
        }


        static void CreateTable(SqliteDataBase db)
        {
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Age INTEGER NOT NULL
            );";

            db.ExecuteQuery(createTableQuery);
            Console.WriteLine("Table Created or Verified.");
        }

        static void InsertSampleData(SqliteDataBase db)
        {
            string insertQuery = @"
            INSERT INTO Users (Name, Age)
            VALUES ('John Doe', 30), 
                   ('Jane Smith', 25),
                   ('Bob Johnson', 40);";

            db.ExecuteQuery(insertQuery);
            Console.WriteLine("Sample Data Inserted.");
        }

        static void InsertOrUpdateSampleData(SqliteDataBase db)
        {
            string insertOrUpdateQuery = @"
                INSERT INTO Users (Id, Name, Age)
                VALUES 
                    (1, 'John Doe', 30), 
                    (2, 'Jane Smith', 25), 
                    (3, 'Bob Johnson', 40)
                ON CONFLICT(Id) DO UPDATE SET
                    Name = excluded.Name,
                    Age = excluded.Age;";

            db.ExecuteQuery(insertOrUpdateQuery);
            Console.WriteLine("Sample Data Inserted or Updated.");
        }


        static void DisplayUserData(SqliteDataBase db)
        {
            string selectQuery = "SELECT * FROM Users;";
            using (var command = new SQLiteCommand(selectQuery, db.myConnection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("\nUser Data:");
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int age = reader.GetInt32(2);

                        Console.WriteLine($"ID: {id}, Name: {name}, Age: {age}");
                    }
                }
            }
        }

        static void UpdateUserName(SqliteDataBase db, int userId, string newName)
        {
            string updateQuery = "UPDATE Users SET Name = @Name WHERE Id = @Id;";
            using (var command = new SQLiteCommand(updateQuery, db.myConnection))
            {
                command.Parameters.AddWithValue("@Name", newName);
                command.Parameters.AddWithValue("@Id", userId);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"User with ID {userId} updated to Name: {newName}");
        }

        static void DeleteUserById(SqliteDataBase db, int userId)
        {
            string deleteQuery = "DELETE FROM Users WHERE Id = @Id;";
            using (var command = new SQLiteCommand(deleteQuery, db.myConnection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"User with ID {userId} deleted.");
        }

        static void GetUserById(SqliteDataBase db, int userId)
        {
            string selectQuery = "SELECT * FROM Users WHERE Id = @Id;";
            using (var command = new SQLiteCommand(selectQuery, db.myConnection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int age = reader.GetInt32(2);

                        Console.WriteLine($"\nRetrieved User -> ID: {id}, Name: {name}, Age: {age}");
                    }
                    else
                    {
                        Console.WriteLine($"\nNo user found with ID {userId}");
                    }
                }
            }
        }
    }
}
