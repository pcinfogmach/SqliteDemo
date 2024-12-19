using System;
using System.Data.SQLite;

namespace SqliteDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new FtsSample();
            new Sample();

            Console.WriteLine("\nAll Tasks are Done.\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
