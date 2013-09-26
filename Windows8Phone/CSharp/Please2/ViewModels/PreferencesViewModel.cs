using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;
using Please2.Resources;

namespace Please2.ViewModels
{
    class PreferencesViewModel : NotificationBase
    {
        DatabaseModel db;

        private ObservableCollection<PreferenceItem> _preferenceList;
        public ObservableCollection<PreferenceItem> PreferenceList
        {
            get
            {
                return _preferenceList;
            }
            set
            {
                _preferenceList = value;
                NotifyPropertyChanged("PreferenceList");
            }
        }

        public PreferencesViewModel()
        {
            LoadPreferences();
        }

        public void LoadPreferences()
        {
            List<PreferenceItem> preferences = GetPreferences();

            PreferenceList = new ObservableCollection<PreferenceItem>(preferences);
        }

        public List<PreferenceItem> GetPreferences()
        {
            using (db = new DatabaseModel(AppResources.DataStore))
            {
                if (db.DatabaseExists().Equals(false))
                {
                    db.CreateDatabase();
                }
                
                IQueryable<PreferenceItem> query = from PreferenceItem preferences in db.Preferences select preferences;

                return new List<PreferenceItem>(query);
            }
        }

        public PreferenceItem GetPreference(int id)
        {
            using (db = new DatabaseModel(AppResources.DataStore))
            {
                IQueryable<PreferenceItem> query = from PreferenceItem preference in db.Preferences where preference.ID == id select preference;

                return query.First();
            }
        }

        public PreferenceItem GetPreference(string name)
        {
            using (db = new DatabaseModel(AppResources.DataStore))
            {
                IQueryable<PreferenceItem> query = from PreferenceItem preference in db.Preferences where preference.Name == name select preference;

                return query.First();
            }
        }

        public void AddPreference(string name, string value)
        {
            using (db = new DatabaseModel(AppResources.DataStore))
            {
                if (db.DatabaseExists().Equals(false))
                {
                    db.CreateDatabase();
                }

                db.Preferences.InsertOnSubmit(
                    new PreferenceItem
                    {
                        Name = name,
                        Value = value
                    }
                );

                db.SubmitChanges();
            }
        }
    }
}
