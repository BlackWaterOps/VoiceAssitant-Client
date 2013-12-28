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

                return (model.sender == "user") ? UserDialog : PlexiDialog;
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
                WeatherViewModel vm = ViewModelLocator.GetServiceInstance<WeatherViewModel>();

                int index = vm.MultiForecast.IndexOf((WeatherDay)item);

                return (index == 0) ?  WeatherFull : WeatherShort;                
            }

            return null;
        }
    }

    public class EventsTemplateSelector : TemplateSelector
    {
        public DataTemplate NoImage { get; set; }

        public DataTemplate Image { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                EventModel result = item as EventModel;

                return (result.image == null) ? NoImage : Image;
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
