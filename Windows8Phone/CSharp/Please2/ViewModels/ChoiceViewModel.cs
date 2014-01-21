using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
using PlexiSDK.Models;

namespace Please2.ViewModels
{
    class ChoiceViewModel : ListViewModel
    {
        public RelayCommand<ChoiceModel> ChoiceItemSelection { get; set; }

        public ChoiceViewModel()
        {
            ChoiceItemSelection = new RelayCommand<ChoiceModel>(ChoiceItemSelected);

        }
        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["list"] as JArray).ToObject<List<ChoiceModel>>();

            this.Scheme = ColorScheme.Default;
            this.Title = (string)structured["text"];

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void ChoiceItemSelected(ChoiceModel choice)
        {
            // add choice to conversation page
           
            ConversationViewModel vm = ViewModelLocator.GetServiceInstance<ConversationViewModel>();

            vm.AddDialog(DialogOwner.User, choice.text);
            
            // pass selection to please service to process and send to auditor
            plexiService.Choice(choice);
        }
    }
}
