using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using GalaSoft.MvvmLight.Ioc;

using Please2.Models;
using Please2.ViewModels;

namespace Please2.Util
{
    public abstract class TemplateSelector : ContentControl
    {
        public abstract DataTemplate SelectTemplate(object item, DependencyObject container);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class WeatherTemplateSelector : TemplateSelector
    {
        public DataTemplate WeatherFull { get; set; }

        public DataTemplate WeatherShort { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {                 
            var wvm = SimpleIoc.Default.GetInstance<WeatherViewModel>();

            var index = wvm.MultiForecast.IndexOf((WeatherDay)item);

            if (item != null)
            {
                if (index == 0)
                {
                    return WeatherFull;
                }
                else
                {
                    return WeatherShort;
                }
            }

            return null;
        }
    }
}
