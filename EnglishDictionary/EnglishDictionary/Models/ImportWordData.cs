using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishDictionary.Models
{
    public class ImportWordData
    {
        public string Word { get; set; }

        public string IPA { get; set; }
        public string Type { get; set; }

        public string Example { get; set; }
        public string Definition { get; set; }
    }
}
