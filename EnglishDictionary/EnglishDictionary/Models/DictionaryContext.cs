using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnglishDictionary.Models
{
    public class DictionaryContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<WordType> WordTypes { get; set; }
        public DbSet<WordExample> WordExamples { get; set; }
        public DbSet<WordMeaning> WordMeanings { get; set; }
        public DbSet<Word> Words { get; set; }

        private static bool isInitialized = false;

        private static string GetDatabasePath()
        {
            string databaseName = "dictionary.db";
            string currentDirectory = Directory.GetCurrentDirectory();
            string databasePath = Path.Combine(currentDirectory, databaseName);
            if (!Directory.Exists(Path.GetDirectoryName(databasePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            }
            else
            {
                UpdateDatabase();
            }

            return databasePath;
        }

        private static void InitializeDatabase()
        {
            if (isInitialized) return;

            try
            {
                using (var context = new DictionaryContext())
                {
                    if (!context.Database.EnsureCreated())
                    {
                        // database already exists, do nothing
                        return;
                    }

                    Account ac = new Account()
                    {
                        Username = "admin",
                        Password = "admin",
                        Role = "admin",
                    };
                    context.Accounts.Add(ac);

                    List<WordType> wordTypes = new List<WordType>()
            {
                 new WordType() { WordTypeName = "noun" },
                 new WordType() { WordTypeName = "verb" },
                 new WordType() { WordTypeName = "adverb" },
                 new WordType() { WordTypeName = "pronoun" },
                 new WordType() { WordTypeName = "adjective" },
                 new WordType() { WordTypeName = "determiner" },
                 new WordType() { WordTypeName = "preposition" },
            };

                    context.WordTypes.AddRange(wordTypes);

                    int changes = context.SaveChanges();

                    if (changes > 0)
                    {
                        //MessageBox.Show($"Database initialization successful. {changes} records added.");
                    }
                }

                isInitialized = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error initializing database: {ex.Message}");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = GetDatabasePath();
            optionsBuilder.UseSqlite($"Data Source={databasePath}");

            // Automatically apply any pending migrations
            optionsBuilder.EnableServiceProviderCaching(false);

        }

        public static void UpdateDatabase()
        {
            if (isInitialized == false)
            {
                isInitialized = true;

                try
                {
                    using (var context = new DictionaryContext())
                    {
                        context.Database.Migrate();

                        Account ac = new Account()
                        {
                            Username = "admin",
                            Password = "admin",
                            Role = "admin",
                        };
                        context.Accounts.Add(ac);

                        List<WordType> wordTypes = new List<WordType>()
            {
                 new WordType() { WordTypeName = "noun" },
                 new WordType() { WordTypeName = "verb" },
                 new WordType() { WordTypeName = "adverb" },
                 new WordType() { WordTypeName = "pronoun" },
                 new WordType() { WordTypeName = "adjective" },
                 new WordType() { WordTypeName = "determiner" },
                 new WordType() { WordTypeName = "preposition" },
            };

                        context.WordTypes.AddRange(wordTypes);

                        int changes = context.SaveChanges();

                        if (changes > 0)
                        {
                            //MessageBox.Show($"Database initialization successful. {changes} records added.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error initializing database: {ex.Message}");
                }

            }

          


        }

    }
}
