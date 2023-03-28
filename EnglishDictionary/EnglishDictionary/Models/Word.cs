using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class Word
    {
        [Key]
        public int WordId { get; set; }
        public string WordName { get; set; }

        public Type WordType { get; set; }


        public int WordTypeId { get; set; }
        public string Meanings { get; set; }

        public string Examples { get; set; }
        public string IPA { get; set; }
    }
}
