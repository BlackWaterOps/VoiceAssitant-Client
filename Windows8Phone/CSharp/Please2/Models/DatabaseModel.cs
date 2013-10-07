using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Windows;

using Microsoft.Phone.Scheduler;

namespace Please2.Models
{
    class DatabaseModel : DataContext
    {
        public DatabaseModel(string connectionString) : base(connectionString)
        {
        }

        public Table<Alarm> Alarms;
        public Table<Note> Notes;
        // public Table<Template> Templates;
        public Table<PreferenceItem> Preferences;

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

    /*
    [Table]
    public class Template : ModelBase
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

        private string _title;

        [Column]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    NotifyPropertyChanging("Title");
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _subTitle;

        [Column]
        public string SubTitle
        {
            get
            {
                return _subTitle;
            }
            set
            {
                if (_subTitle != value)
                {
                    NotifyPropertyChanging("SubTitle");
                    _subTitle = value;
                    NotifyPropertyChanged("SubTitle");
                }
            }
        }

        private DataTemplate _dataTemplate;

        [Column]
        public DataTemplate DataTemplate
        {
            get
            {
                return _dataTemplate;
            }
            set
            {
                if (_dataTemplate != value)
                {
                    NotifyPropertyChanging("DataTemplate");
                    _dataTemplate = value;
                    NotifyPropertyChanged("DataTemplate");
                }
            }
        }
    }
    */
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
}
