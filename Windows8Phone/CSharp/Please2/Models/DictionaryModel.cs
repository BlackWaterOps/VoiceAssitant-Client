using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class DictionarySense
    {
        public string definition { get; set; }
        public string kind { get; set; }
        public List<string> examples { get; set; }
        public List<string> lemmas { get; set; }
    }

    public class DictionaryModel
    {
        public string headword { get; set; }
        public Dictionary<string, List<DictionarySense>> senses { get; set; }
        public List<string> pos { get; set; }
    }
}
