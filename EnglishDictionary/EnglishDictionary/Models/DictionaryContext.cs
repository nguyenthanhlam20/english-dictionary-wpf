using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class DictionaryContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }

        public DbSet<WordType> WordTypes { get; set; }
        public DbSet<WordExample> WordExamples { get; set; }
        public DbSet<WordMeaning> WordMeanings { get; set; }

        public DbSet<Word> Words { get; set; }


        private static string GetDatabasePath()
        {
            string databaseName = "dictionary.db";
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderName = "EnglishDictionary";
            string databasePath = Path.Combine(appDataPath, folderName, databaseName);

            if (!Directory.Exists(Path.GetDirectoryName(databasePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            }

            return databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = GetDatabasePath();
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

    }
}
