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

        public string DisplayMeaning
        {
            get
            {
                using (var context = new DictionaryContext())
                {
                    WordMeanings = context.WordMeanings.Where(w => w.WordId == WordId).ToList();


                    if (WordMeanings != null && WordMeanings.Count() > 0)
                    {
                        return WordMeanings.ElementAt(0).MeaningContent;
                    }
                    return "";
                }

            }
        }

        public string DisplayExample
        {
            get
            {

                using (var context = new DictionaryContext())
                {

                    WordExamples = context.WordExamples.Where(w => w.WordId!= WordId).ToList();

                    if (WordExamples != null && WordExamples.Count() > 0)
                    {
                        return WordExamples.ElementAt(0).ExampleContent;
                    }
                    return "";
                }
               
            }
        }

    }
}
