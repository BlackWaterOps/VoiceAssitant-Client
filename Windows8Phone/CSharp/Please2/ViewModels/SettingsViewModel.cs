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

        public List<ProviderModel> Providers
        {
            get 
            { 
                string providers = GetValueOrDefault<string>(ProvidersSettingKeyName, "");

                return (List<ProviderModel>)Deserialize(providers, typeof(List<ProviderModel>));
            }
            set
            {
                if (AddOrUpdateValue(ProvidersSettingKeyName, Serialize(value)))
                {
                    Save();
                }
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
            //if (!settings.Contains(ProvidersSettingKeyName))
            //{
                Debug.WriteLine("set providers");
                List<ProviderModel> providers = new List<ProviderModel>();

                providers.Add(new ProviderModel("Google", AccountStatus.NotConnected, "google", true));
                providers.Add(new ProviderModel("Facebook", AccountStatus.NotConnected, "facebook", true));
                providers.Add(new ProviderModel("Fitbit", AccountStatus.NotConnected, "fitbit", true));

                Providers = providers;
            //}
            /*
             * for future use. Handle the ability to add new accounts and merge them with the currently stored accounts
            if (settings.Contains(AccountsSettingKeyName))
            {
                //run through list and add any missing accounts
                List<ProviderModel> savedAccounts = (List<ProviderModel>)Deserialize((string)settings[AccountsSettingKeyName], typeof(List<ProviderModel>));

                IEnumerable<ProviderModel> unioned = savedAccounts.Union(accounts, new ProviderComparer());

                Accounts = new List<ProviderModel>(unioned);
            }
            else
            {
                Accounts = accounts;
            }
            */
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

        private string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        private object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }
        #endregion
    }

    public class ProviderComparer : IEqualityComparer<ProviderModel>
    {
        public bool Equals(ProviderModel x, ProviderModel y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.name == y.name;
        }

        public int GetHashCode(ProviderModel provider)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(provider, null))
            {
                return 0;
            }

            //Get hash code for the Name field if it is not null. 
            int hashProviderName = (provider.name == null) ? 0 : provider.name.GetHashCode();

            return hashProviderName;

            //Get hash code for the Code field. 
            //int hashProviderStatus = provider.status.GetHashCode();

            //Calculate the hash code for the product. 
            //return hashProviderName ^ hashProviderStatus;
        }
    }
}
