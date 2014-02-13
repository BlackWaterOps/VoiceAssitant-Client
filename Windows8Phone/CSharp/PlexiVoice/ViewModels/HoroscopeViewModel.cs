using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

namespace PlexiVoice.ViewModels
{
    public class HoroscopeViewModel : ViewModelBase, IViewModel
    {
        private string zodiacSignCap;
        public string ZodiacSignCap { get { return zodiacSignCap; } }

        private string zodiacSign;
        public string ZodiacSign
        {
            get { return zodiacSign; }
            set
            {
                zodiacSignCap = (Char.ToUpper(value[0]) + value.Substring(1));
                zodiacSign = value;
                RaisePropertyChanged("ZodiacSign");
            }
        }

        private string horoscope;
        public string Horoscope
        {
            get { return horoscope; }
            set 
            {
                horoscope = value;
                RaisePropertyChanged("Horoscope");
            }
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            HoroscopeModel horoscopeResults = ((JObject)structured["item"]).ToObject<HoroscopeModel>();

            ZodiacSign = horoscopeResults.zodiac_sign;
            Horoscope = horoscopeResults.horoscope;
        }
    }
}
