﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class DictionaryViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        /*
        private string definitions;
        public string Definitions
        {
            get { return definitions; }
            set
            {
                definitions = value;
                RaisePropertyChanged("Definitions");
            }
        }

        private List<string> examples;
        public List<string> Examples
        {
            get { return examples; }
            set
            {
                examples = value;
                RaisePropertyChanged("Examples");
            }
        }
        */
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
        /*
        private string part;
        public string Part
        {
            get { return part; }
            set
            {
                part = value;
                RaisePropertyChanged("Part");
            }
        }
        */
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

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var dictionaryResults = (structured["item"] as JObject).ToObject<DictionaryModel>();

            var senses = dictionaryResults.senses;

            Senses = dictionaryResults.senses;
            Pos = dictionaryResults.pos;
            Headword = dictionaryResults.headword;

            var data = new Dictionary<string, object>();

            data.Add("title", "dictionary");
            data.Add("scheme", ColorScheme.Information);
            data.Add("margin", new Thickness(12, 24, 12, 24));

            return data;
        }
    }
}
