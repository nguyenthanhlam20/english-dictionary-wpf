using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class WordMeaning
    {
        [Key]
        public int MeaningId { get; set; }
        public string MeaningContent { get; set; }

        public Word Word { get; set; }
        public int WordId { get; set; }
    }
}
