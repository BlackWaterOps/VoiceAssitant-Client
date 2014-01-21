using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;
namespace Please2.ViewModels
{
    public class ShoppingViewModel : ListViewModel, IViewModel
    {
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }

        public ShoppingViewModel()
        {
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Debug.WriteLine("load shopping data");

            Items = (structured["items"] as JArray).ToObject<List<ShoppingModel>>();

            this.Scheme = ColorScheme.Commerce;
            this.Title = "shopping";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void ShoppingItemSelected(ShoppingModel product)
        {
            // navigate to generic details page with movies id and template name
            //navigationService.NavigateTo(new Uri(product.url, UriKind.Absolute));
        }
    }
}
