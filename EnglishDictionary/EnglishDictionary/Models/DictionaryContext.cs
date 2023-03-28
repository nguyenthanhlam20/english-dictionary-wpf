using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class DictionaryContext : DbContext
    {

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Type> Types { get; set; }

        public DbSet<Word> Words { get; set; }


        public string path = @"C:\Temp\dictionary.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlite($"Data Source={path}");

    }
}
