using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    public class ShoppingViewModel : ListBase, IViewModel
    {
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }

        public ShoppingViewModel()
        {
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Debug.WriteLine("load shopping data");

            Items = (structured["items"] as JArray).ToObject<List<ShoppingModel>>();
        }

        public void ShoppingItemSelected(ShoppingModel product)
        {
            // navigate to generic details page with movies id and template name
            //navigationService.NavigateTo(new Uri(product.url, UriKind.Absolute));
        }
    }
}
