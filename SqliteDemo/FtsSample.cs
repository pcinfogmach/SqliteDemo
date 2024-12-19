using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteDemo
{
    public class FtsSample
    {
        public FtsSample()
        {
            using (var db = new SqliteDataBase())
            {
                CreateFTSTable(db);
                InsertFTSSampleData(db);

                Console.WriteLine("\nFTS Data:");
                DisplayFTSData(db);

                Console.WriteLine("\nSearch Results for 'search':");
                SearchFTSData(db, "search");

                Console.WriteLine("\nSearch Results for 'example':");
                SearchFTSData(db, "example");
            }

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
        }

        static void CreateFTSTable(SqliteDataBase db)
        {
            try
            {
                string createFTSTableQuery = @"
        CREATE VIRTUAL TABLE IF NOT EXISTS Documents USING fts5(
            Title,
            Content
        );";

                db.ExecuteQuery(createFTSTableQuery);
                Console.WriteLine("FTS5 Table Created.");
            }
            catch (SQLiteException ex) when (ex.Message.Contains("no such module: fts5"))
            {
                Console.WriteLine("FTS5 is not available. Falling back to FTS4.");

                string createFTSTableQueryFallback = @"
        CREATE VIRTUAL TABLE IF NOT EXISTS Documents USING fts4(
            Title,
            Content
        );";

                db.ExecuteQuery(createFTSTableQueryFallback);
                Console.WriteLine("FTS4 Table Created as a fallback.");
            }
        }

        static void InsertFTSSampleData(SqliteDataBase db)
        {
            string insertOrReplaceFTSQuery = @"
        INSERT OR REPLACE INTO Documents (rowid, Title, Content)
        VALUES 
            ((SELECT rowid FROM Documents WHERE Title = 'Document 1'), 'Document 1', 'This is a full-text search example.'),
            ((SELECT rowid FROM Documents WHERE Title = 'Document 2'), 'Document 2', 'SQLite supports FTS for efficient search.'),
            ((SELECT rowid FROM Documents WHERE Title = 'Document 3'), 'Document 3', 'Full-text search is great for searching text data.');";

            db.ExecuteQuery(insertOrReplaceFTSQuery);
            Console.WriteLine("FTS Sample Data Inserted or Updated.");
        }

        static void DisplayFTSData(SqliteDataBase db)
        {
            string selectQuery = "SELECT rowid, Title, Content FROM Documents;";
            using (var command = new SQLiteCommand(selectQuery, db.myConnection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long rowid = reader.GetInt64(0);
                        string title = reader.GetString(1);
                        string content = reader.GetString(2);

                        Console.WriteLine($"RowID: {rowid}, Title: {title}, Content: {content}");
                    }
                }
            }
        }

        static void SearchFTSData(SqliteDataBase db, string searchTerm)
        {
            string searchQuery = "SELECT rowid, Title, Content FROM Documents WHERE Documents MATCH @SearchTerm;";
            using (var command = new SQLiteCommand(searchQuery, db.myConnection))
            {
                command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long rowid = reader.GetInt64(0);
                        string title = reader.GetString(1);
                        string content = reader.GetString(2);

                        Console.WriteLine($"RowID: {rowid}, Title: {title}, Content: {content}");
                    }
                }
            }
        }
    }
}
