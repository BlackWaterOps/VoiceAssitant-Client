using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Models;

namespace PlexiVoice.ViewModels
{
    class ChoiceViewModel : ListBase, IViewModel
    {
        public RelayCommand<ChoiceModel> ChoiceItemSelection { get; set; }

        public ChoiceViewModel()
        {
            ChoiceItemSelection = new RelayCommand<ChoiceModel>(ChoiceItemSelected);

        }
        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<ChoiceModel>>();
        }

        public void ChoiceItemSelected(ChoiceModel choice)
        {
            // add choice to conversation page
            MainViewModel vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            vm.AddDialog(DialogOwner.User, choice.text);

            // pass selection to please service to process and send to auditor
            plexiService.Choice(choice);
        }
    }
}
