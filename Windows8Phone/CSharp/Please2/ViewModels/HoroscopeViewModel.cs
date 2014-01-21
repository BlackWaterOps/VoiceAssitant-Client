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

using Please2.Models;
using Please2.Util;
namespace Please2.ViewModels
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

        public HoroscopeViewModel()
        {
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            HoroscopeModel horoscopeResults = ((JObject)structured["item"]).ToObject<HoroscopeModel>();

            ZodiacSign = horoscopeResults.zodiac_sign;
            Horoscope = horoscopeResults.horoscope;

            string date = DateTime.Now.ToString("dddd, MMMM d, yyyy");

            var data = new Dictionary<string, object>();

            data.Add("title", "horoscope");
            data.Add("subtitle", ZodiacSign + " for " + date);
            data.Add("scheme", ColorScheme.Default);
            data.Add("margin", new Thickness(12, 24, 12, 24));

            return data;
        }
    }
}
