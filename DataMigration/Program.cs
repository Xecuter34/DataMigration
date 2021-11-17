using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataMigration
{
    class Program
    {
        public static string FilePath = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to enter a location for the .json files? y/n");
            IDictionary<string, string> files;
            if (Console.ReadLine() == "y")
            {
                Console.Write("Enter path for the file/s to import: ");
                files = HandlePath(Console.ReadLine());
            }
            else
            {
                files = HandlePath("D:\\Repo\\Edge");
            }

            if (files.Count == 0)
            {
                Console.WriteLine("No Files to convert.");
                return;
            }

            Migrate(files).Wait();
        }

        private async static Task Migrate(IDictionary<string, string> files)
        {
            await Migrations.MigrateUsersAsync(files["users"]);
            await Migrations.MigrateOrganisationAsync(files["organizations"]);
            Console.WriteLine("\nMigration Completed successfully.");
        }
        private static IDictionary<string, string> HandlePath(string path)
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                // Directory
                if (Directory.Exists(path))
                {
                    string[] dir = Directory.GetFiles(path);
                    foreach (string filePath in dir)
                    {
                        if (Path.GetExtension(filePath) == ".json")
                        {
                            string fileData = File.ReadAllText(path);
                            string fileName = Path.GetFileName(filePath).Split(".")[0];
                            files.Add(fileName, fileData);
                        }
                        else
                        {
                            Console.WriteLine($"File type is {Path.GetExtension(filePath)}, skipping...");
                        }
                    }
                }
            }
            else
            {
                // File
                if (File.Exists(path))
                {
                    if (Path.GetExtension(path) == ".json")
                    {
                        string fileData = File.ReadAllText(path);
                        string fileName = Path.GetFileName(path).Split(".")[0];
                        files.Add(fileName, fileData);
                    }
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }

            return files;
        }
    }
}
