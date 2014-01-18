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

using PlexiSDK;
using PlexiSDK.Models;
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

        IPlexiService plexiService;

        public SettingsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.settings = IsolatedStorageSettings.ApplicationSettings;

            this.plexiService = plexiService;
        }

        public async Task InitializeSettings()
        {
            await InitializeAccounts(); 
        }

        #region accounts
        //TODO: save settings with id so we can deauth later
        // need to be able to handle multiple accounts of the same type
        private async Task InitializeAccounts()
        {
            List<ProviderModel> providers = new List<ProviderModel>();

            List<AccountModel> authedAccounts = await plexiService.GetAccounts();

            Array accounts = Enum.GetValues(typeof(AccountType));

            foreach (AccountType account in accounts)
            {
                if (account != AccountType.None)
                {
                    IEnumerable<AccountModel> query = from authedAccount in authedAccounts
                                                      where authedAccount.service_name.ToLower() == account.ToString().ToLower()
                                                      select authedAccount;

                    AccountStatus status = AccountStatus.NotConnected;

                    ProviderModel provider;

                    if (query.Count() > 0)
                    {
                        status = AccountStatus.Connected;
                        provider = new ProviderModel(query.LastOrDefault().id, account, status);
                    }
                    else
                    {
                        provider = new ProviderModel(account, status);
                    }

                    if (AddOrUpdateValue(account.ToString(), status))
                    {
                        Save();
                    }

                    providers.Add(provider);
                }
            }

            Providers = providers;
            //LoadAccounts();
        }

        private void LoadAccounts()
        {
            Array accounts = Enum.GetValues(typeof(AccountType));

            List<ProviderModel> list = new List<ProviderModel>();

            foreach (AccountType account in accounts)
            {
                if (account != AccountType.None)
                {
                    string accountString = account.ToString();

                    if (settings.Contains(accountString))
                    {
                        AccountStatus status = (AccountStatus)settings[accountString];

                        list.Add(new ProviderModel(account, status));
                    }
                }
            }

            Providers = list;
        }

        public async Task<List<ProviderModel>> GetAccounts()
        {
            return await GetAccounts(AccountType.None);
        }

        public async Task<List<ProviderModel>> GetAccounts(AccountType type)
        {
            List<AccountModel> accounts = await plexiService.GetAccounts();

            List<ProviderModel> providers = new List<ProviderModel>();

            IEnumerable<AccountModel> query;

            if (type == AccountType.None)
            {
                query = accounts;
            }
            else
            {
                query = from account in accounts
                        where account.service_name.ToLower() == type.ToString().ToLower()
                        select account;
            }

            foreach (AccountModel account in query)
            {
                ProviderModel provider = new ProviderModel();

                provider.id = account.id;
                provider.name = (AccountType)Enum.Parse(typeof(AccountType), account.service_name, true);
                provider.status = AccountStatus.Connected;

                providers.Add(provider);
            }

            return providers;
        }

        public void UpdateAccount(AccountType type, AccountStatus status)
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

        public void RemoveAccount(ProviderModel account)
        {
            plexiService.RemoveAccount(account.id);

            UpdateAccount(account.name, AccountStatus.NotConnected);
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
