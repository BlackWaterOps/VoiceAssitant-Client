using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

namespace PlexiVoice.ViewModels
{
    public class DictionaryViewModel : ViewModelBase, IViewModel
    {
        private string headword;
        public string Headword
        {
            get { return headword; }
            set
            {
                headword = value;
                RaisePropertyChanged("Headword");
            }
        }

        private Dictionary<string, List<DictionarySense>> senses;
        public Dictionary<string, List<DictionarySense>> Senses
        {
            get { return senses; }
            set
            {
                senses = value;
                RaisePropertyChanged("Senses");
            }
        }

        private List<string> pos;
        public List<string> Pos
        {
            get { return pos; }
            set
            {
                pos = value;
                RaisePropertyChanged("Pos");
            }
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            var dictionaryResults = (structured["item"] as JObject).ToObject<DictionaryModel>();

            var senses = dictionaryResults.senses;

            Senses = dictionaryResults.senses;
            Pos = dictionaryResults.pos;
            Headword = dictionaryResults.headword;
        }
    }
}
