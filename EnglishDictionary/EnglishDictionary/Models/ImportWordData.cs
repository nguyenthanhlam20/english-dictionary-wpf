using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class ImportWordData
    {
        [Name("Word")]
        public string Word { get; set; }
        [Name("Type")]
        public string Type { get; set; }

        [Name("IPA")]
        public string IPA { get; set; }
        [Name("Definition")]
        public string Definition { get; set; }
        [Name("Example")]
        public string Example { get; set; }
    }
}
