using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

using Microsoft.Phone.Scheduler;

namespace Please2.Models
{
    class DatabaseModel : DataContext
    {
        public DatabaseModel(string connectionString) : base(connectionString)
        {
        }

        public Table<PreferenceItem> Preferences;
        public Table<Alarm> Alarms;
        public Table<Note> Notes;
    }

    [Table]
    public class PreferenceItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("ID");
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _name;

        [Column]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _value;

        [Column]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    NotifyPropertyChanging("Value");
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }

    [Table]
    public class Note : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("ID");
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _text;

        [Column]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    NotifyPropertyChanging("Text");
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        private DateTime _time;

        [Column]
        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    NotifyPropertyChanging("Time");
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }
    }

    [Table]
    public class Alarm : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("ID");
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private List<string> _names;

        [Column]
        public List<string> Names
        {
            get
            {
                return _names;
            }
            set
            {
                if (_names != value)
                {
                    NotifyPropertyChanging("Names");
                    _names = value;
                    NotifyPropertyChanged("Names");
                }
            }
        }

        private bool _isEnabled;

        [Column]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    NotifyPropertyChanging("IsEnabled");
                    _isEnabled = value;
                    NotifyPropertyChanged("IsEnabled");
                }
            }
        }

        private DateTime _time;

        [Column]
        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    NotifyPropertyChanging("Time");
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        private RecurrenceInterval _interval;

        [Column]
        public RecurrenceInterval Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                if (_interval != value)
                {
                    NotifyPropertyChanging("Interval");
                    _interval = value;
                    NotifyPropertyChanged("Interval");
                }
            }
        }

        private List<DayOfWeek> _daysOfWeek;

        [Column]
        public List<DayOfWeek> DaysOfWeek
        {
            get
            {
                return _daysOfWeek;
            }
            set
            {
                if (_daysOfWeek != value)
                {
                    NotifyPropertyChanging("DaysOfWeek");
                    _daysOfWeek = value;
                    NotifyPropertyChanged("DaysOfWeek");
                }
            }
        }
    }
}
