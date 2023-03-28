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

        public WordType Type { get; set; }


        public int WordTypeId { get; set; }

        public string IPA { get; set; }

        public List<WordMeaning> WordMeanings { get; set; }
        public List<WordExample> WordExamples { get; set; }

    }
}
