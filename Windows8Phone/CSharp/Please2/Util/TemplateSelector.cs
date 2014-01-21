using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Phone.Controls;

using GalaSoft.MvvmLight.Ioc;

using LinqToVisualTree;

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

    /// <summary>
    /// Main Template for the Voice Assistante results.
    /// <para>By default, If a template is not set, the speach text will be shownt</para>
    /// </summary>
    public class ResultsTemplateSelector : TemplateSelector
    {
        public DataTemplate AlarmTemplate { get; set; }

        public DataTemplate ChoiceTemplate { get; set; }

        public DataTemplate ClockTemplate { get; set; }

        public DataTemplate DialogTemplate { get; set; }

        public DataTemplate DictionaryTemplate { get; set; }

        public DataTemplate EventsTemplate { get; set; }

        public DataTemplate FitbitActivityTemplate { get; set; }

        public DataTemplate FitbitFoodTemplate { get; set; }

        public DataTemplate FitbitWeightTemplate { get; set; }

        public DataTemplate FlightsTemplate { get; set; }

        public DataTemplate FuelTemplate { get; set; }

        public DataTemplate GeopoliticsTemplate { get; set; }

        public DataTemplate HoroscopeTemplate { get; set; }

        public DataTemplate ImagesTemplate { get; set; }

        public DataTemplate ListTemplate { get; set; }

        public DataTemplate MoviesTemplate { get; set; }

        public DataTemplate NewsTemplate { get; set; }

        public DataTemplate RealEstateTemplate { get; set; }

        public DataTemplate SearchTemplate { get; set; }

        public DataTemplate ShoppingTemplate { get; set; }

        public DataTemplate StockTemplate { get; set; }

        public DataTemplate WeatherTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                try
                {   /*
                    if (item is DialogModel)
                    {
                        return DialogTemplate;
                    }
                    */

                    Type type = item.GetType();

                    string typeString = type.ToString();

                    string vmName = typeString.Split('.').Last();

                    string templateString = vmName.Replace("ViewModel", "Template");

                    Debug.WriteLine(templateString);

                    PropertyInfo templateProperty = this.GetType().GetProperty(templateString);

                    if (templateProperty != null)
                    {
                        Debug.WriteLine("attach " + templateString);

                        // we have a template attached that matches the viewmodel/item
                        return (DataTemplate)templateProperty.GetValue(this, null);
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }

            return null;
        }
    }

    public class DialogTemplateSelector : TemplateSelector
    {
        public DataTemplate PlexiDialog { get; set; }

        public DataTemplate UserDialog { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                ConversationViewModel vm = ViewModelLocator.GetServiceInstance<ConversationViewModel>();

                DialogModel model = item as DialogModel;

                return (model.sender == DialogOwner.User) ? UserDialog : PlexiDialog;
            }
            return null;
        }
    }

    public class WeatherTemplateSelector : TemplateSelector
    {
        public DataTemplate WeatherFull { get; set; }

        public DataTemplate WeatherShort { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            { 
                /*
                WeatherViewModel vm = ViewModelLocator.GetServiceInstance<WeatherViewModel>();

                int index = vm.MultiForecast.IndexOf((WeatherDay)item);

                return (index == 0) ?  WeatherFull : WeatherShort;
                */

                WeatherTemplateSelector selector = container as WeatherTemplateSelector;

                ListBox list = selector.Ancestors<ListBox>().Cast<ListBox>().Where(x => x.Name == "WeatherListBox").FirstOrDefault();

                if (list != null)
                {
                    int index = list.Items.IndexOf((WeatherDay)item);

                    return (index == 0) ? WeatherFull : WeatherShort;

                }

                return WeatherShort;
            }

            return null;
        }
    }

    public class SearchTemplateSelector : TemplateSelector
    {
        public DataTemplate NoImage { get; set; }

        public DataTemplate Image { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                SearchModel result = item as SearchModel;

                Debug.WriteLine(result.opengraph_image);

                return (result.opengraph_image == null) ? NoImage : Image;
            }

            return null;
        }
    }

    public class FuelTemplateSelector : TemplateSelector
    {
        public DataTemplate BioDiesel { get; set; }

        public DataTemplate Electric { get; set; }

        public DataTemplate Gasoline { get; set; }

        public DataTemplate Ethanol { get; set; }

        public DataTemplate Hydrogen { get; set; }

        public DataTemplate CompressedNaturalGas { get; set; }

        public DataTemplate LiquifiedNaturalGas { get; set; }

        public DataTemplate LiquifiedPetroleumGas { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var result = item as AltFuelModel;

            var fuelType = result.fuel_type_code.ToLower();

            DataTemplate template = new DataTemplate();

            switch (fuelType)
            {
                case "bd":
                    template = BioDiesel;
                    break;
                
                case "elec":
                    template = Electric;
                    break;

                case "gas":
                    template = Gasoline;
                    break;

                case "e85":
                    template = Ethanol;
                    break;

                case "hy":
                    template = Hydrogen;
                    break;

                case "lng":
                    template = LiquifiedNaturalGas;
                    break;

                case "lpg":
                    template = LiquifiedPetroleumGas;
                    break;

                case "cng":
                    template = CompressedNaturalGas;
                    break;
            }

            return template;
        }
    }

    public class DetailsTemplateSelector : TemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }
    }
}
