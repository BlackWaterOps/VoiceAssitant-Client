using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;
using Please2.Util;

using Plexi;

namespace Please2.ViewModels
{
    public class SettingsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private IsolatedStorageSettings settings;

        private const string ProvidersSettingKeyName = "Providers";

        private const List<ProviderModel> ProvidersSettingsDefault = null;

        public ColorScheme Scheme { get { return ColorScheme.Settings; } }

        private List<ProviderModel> providers;

        public List<ProviderModel> Providers
        {
            get { return providers; }
            set 
            { 
                providers = value;
                RaisePropertyChanged("Providers");
            }
        }

        public SettingsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.settings = IsolatedStorageSettings.ApplicationSettings;

            InitializeSettings();
        }

        public void InitializeSettings()
        {
            InitializeAccounts(); 
        }

        #region accounts
        //TODO: need to handle to ability to remove/disable an account
        private void InitializeAccounts()
        {
            Array provs = Enum.GetValues(typeof(AccountType));

            foreach (AccountType prov in provs)
            {
                if (prov != AccountType.None)
                {
                    string providerString = prov.ToString();

                    if (!settings.Contains(providerString))
                    {
                        Debug.WriteLine("provider init");

                        if (AddOrUpdateValue(providerString, AccountStatus.NotConnected))
                        {
                            Save();
                        }
                    }
                }
            }

            LoadProviders();
        }

        private void LoadProviders()
        {
            Array provs = Enum.GetValues(typeof(AccountType));

            List<ProviderModel> list = new List<ProviderModel>();

            foreach (AccountType prov in provs)
            {
                if (prov != AccountType.None)
                {
                    string providerString = prov.ToString();

                    if (settings.Contains(providerString))
                    {
                        AccountStatus status = (AccountStatus)settings[providerString];

                        list.Add(new ProviderModel(prov, status));
                    }
                }
            }

            Providers = list;
        }

        public void UpdateProvider(AccountType type, AccountStatus status)
        {
            // update provider model
            ProviderModel provider = Providers.Where(x => x.name == type).FirstOrDefault();

            provider.status = status;

            // update settings value
            if (AddOrUpdateValue(type.ToString(), status))
            {
                Save();
            }
        }

        #endregion

        #region helpers
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            settings.Save();
        }
        #endregion
    }
}
