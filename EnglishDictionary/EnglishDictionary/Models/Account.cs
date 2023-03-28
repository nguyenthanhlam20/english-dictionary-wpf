using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
   public class Account
    {
        [Key]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
