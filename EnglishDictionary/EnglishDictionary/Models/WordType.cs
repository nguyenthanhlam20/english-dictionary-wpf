using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class WordType
    {

        [Key]
        public int WordTypeId { get; set; }
        public string WordTypeName { get; set; }


        public List<Word> Words { get; set; }

    }

}
