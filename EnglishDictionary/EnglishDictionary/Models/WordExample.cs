using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EnglishDictionary.Models
{
    public class WordExample

    {
        [Key]
        public int ExampleId { get; set; }
        public string ExampleContent { get; set; }

        public Word Word { get; set; }
        public int WordId { get; set; }
    }
}
