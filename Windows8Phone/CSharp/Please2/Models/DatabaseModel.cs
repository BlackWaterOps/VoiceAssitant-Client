using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Phone.Scheduler;

using Please2.Util;

namespace Please2.Models
{
    class DatabaseModel : DataContext
    {
        public DatabaseModel(string connectionString) : base(connectionString)
        {
        }
        public Table<MenuItem> Menu;

        public Table<NoteItem> Notes;
        public Table<NoteItemBody> NoteBody;

        public Table<AlarmItem> Alarms;
        public Table<AlarmNameItem> AlarmNames;
        public Table<AlarmDayItem> AlarmDays;

        public Table<PreferenceItem> Preferences;
    }
    #region Menu Table
    [Table]
    public class MenuItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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


        private int _orderID;
        
        [Column]
        public int OrderID
        {
            get
            {
                return _orderID;
            }
            set
            {
                if (_orderID != value)
                {
                    NotifyPropertyChanging("OrderID");
                    _orderID = value;
                    NotifyPropertyChanged("OrderID");
                }
            }
        }

        private string _background;

        [Column]
        public string Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (_background != value)
                {
                    NotifyPropertyChanging("Background");
                    _background = value;
                    NotifyPropertyChanged("Background");
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

        private bool _enabled;

        [Column]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    NotifyPropertyChanging("Enabled");
                    _enabled = value;
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        private string _page;

        [Column]
        public string Page
        {
            get
            {
                return _page;
            }
            set
            {
                if (_page != value)
                {
                    NotifyPropertyChanging("Page");
                    _page = value;
                    NotifyPropertyChanged("Page");
                }
            }
        }

        private string _icon;

        [Column]
        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (_icon != value)
                {
                    NotifyPropertyChanging("Icon");
                    _icon = value;
                    NotifyPropertyChanged("Icon");
                }
            }
        }


        private string _viewModel;

        [Column]
        public string ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if (_viewModel != value)
                {
                    NotifyPropertyChanging("ViewModel");
                    _viewModel = value;
                    NotifyPropertyChanged("ViewModel");
                }
            }
        }

        private string _details;
        
        [Column]
        public string Details
        {
            get
            {
                return _details;
            }
            set
            {
                if (_details != value)
                {
                    NotifyPropertyChanging("Details");
                    _details = value;
                    NotifyPropertyChanged("Details");
                }
            }
        }
    }
    #endregion

    #region Notes Table
    [Table]
    public class NoteItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        private byte[] _thumbnail;

        [Column(DbType="image", UpdateCheck=UpdateCheck.Never)]
        public byte[] Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                if (_thumbnail != value)
                {
                    NotifyPropertyChanging("Thumbnail");
                    _thumbnail = value;
                    NotifyPropertyChanged("Thumbnail");
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

        private DateTime _creationDate;

        [Column]
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                if (_creationDate != value)
                {
                    NotifyPropertyChanging("CreationDate");
                    _creationDate = value;
                    NotifyPropertyChanged("CreationDate");
                }
            }
        }

        private DateTime _modifiedDate;

        [Column]
        public DateTime ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
            set
            {
                if (_modifiedDate != value)
                {
                    NotifyPropertyChanging("ModifiedDate");
                    _modifiedDate = value;
                    NotifyPropertyChanged("ModifiedDate");
                }
            }
        }

        // Define the entity set for the collection side of the relationship.
        private EntitySet<NoteItemBody> _noteBody;

        [Association(Storage = "_noteBody", OtherKey = "_noteID", ThisKey = "ID")]
        public EntitySet<NoteItemBody> NoteBody
        {
            get { return this._noteBody; }
            set { this._noteBody.Assign(value); }
        }

        // Assign handlers for the add and remove operations, respectively. 
        public NoteItem()
        {
            _noteBody = new EntitySet<NoteItemBody>(
                new Action<NoteItemBody>(this.attach_noteBody),
                new Action<NoteItemBody>(this.detach_noteBody)
                );
        }

        // Called during an add operation 
        private void attach_noteBody(NoteItemBody noteBody)
        {
            NotifyPropertyChanging("NoteBody");
            noteBody.Note = this;
        }

        // Called during a remove operation
        private void detach_noteBody(NoteItemBody noteBody)
        {
            NotifyPropertyChanging("NoteBody");
            noteBody.Note = null; 
        }
    }

    [Table]
    public class NoteItemBody : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        private int _style;

        [Column]
        public int Style
        {
            get
            {
                return _style;
            }
            set
            {
                if (_style != value)
                {
                    NotifyPropertyChanging("Style");
                    _style = value;
                    NotifyPropertyChanged("Style");
                }
            }
        }

        /* currently not is use. flag to determine if a checkbox should be shown with this entry
        private bool _hasCheckbox;

        [Column]
        public bool HasCheckbox
        {
            get
            {
                return _hasCheckbox;
            }
            set
            {
                if (_hasCheckbox != value)
                {
                    NotifyPropertyChanging("HasCheckbox");
                    _hasCheckbox = value;
                    NotifyPropertyChanged("HasCheckbox");
                }
            }
        }
        */

        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 

        // Internal column for the associated ToDoCategory ID value 
        [Column] 
        internal int _noteID; 
 
        // Entity reference, to identify the ToDoCategory "storage" table 
        private EntityRef<NoteItem> _note; 
 
        // Association, to describe the relationship between this key and that "storage" table 
        [Association(Storage = "_note", ThisKey = "_noteID", OtherKey = "ID", IsForeignKey = true)] 
        public NoteItem Note 
        { 
            get { return _note.Entity; } 
            set 
            { 
                NotifyPropertyChanging("Note"); 
                _note.Entity = value; 
 
                if (value != null) 
                { 
                    _noteID = value.ID; 
                } 
 
                NotifyPropertyChanging("Note"); 
            } 
        } 
    }
    #endregion

    #region Alarms Table
    [Table]
    public class AlarmItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        private string _displayName;

        [Column]
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (_displayName != value)
                {
                    NotifyPropertyChanging("DisplayName");
                    _displayName = value;
                    NotifyPropertyChanged("DisplayName");
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
        
        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 

        // Define the entity set for the collection side of the relationship.
        // _names is a collection of scheduled alarm GUID's 
        private EntitySet<AlarmNameItem> _names;

        // _days is a collection of DayOfWeek enums 
        private EntitySet<AlarmDayItem> _days;

        [Association(Storage = "_names", OtherKey = "_alarmID", ThisKey = "ID")]
        public EntitySet<AlarmNameItem> Names
        {
            get { return this._names; }
            set { this._names.Assign(value); }
        }

        [Association(Storage = "_days", OtherKey = "_alarmID", ThisKey = "ID")]
        public EntitySet<AlarmDayItem> Days
        {
            get { return this._days; }
            set { this._days.Assign(value); }
        }

        // Assign handlers for the add and remove operations, respectively. 
        public AlarmItem()
        {
            _names = new EntitySet<AlarmNameItem>(
                new Action<AlarmNameItem>(this.attach_names),
                new Action<AlarmNameItem>(this.detach_names)
                );

            _days = new EntitySet<AlarmDayItem>(
                new Action<AlarmDayItem>(this.attach_days),
                new Action<AlarmDayItem>(this.detach_days)
                );
        }

        // Called during an add operation 
        private void attach_names(AlarmNameItem alarmName)
        {
            NotifyPropertyChanging("Names");
            alarmName.Alarm = this;
        }

        private void attach_days(AlarmDayItem alarmDay)
        {
            NotifyPropertyChanging("Days");
            alarmDay.Alarm = this;
        }

        // Called during a remove operation
        private void detach_names(AlarmNameItem alarmName)
        {
            NotifyPropertyChanging("Names");
            alarmName.Alarm = null; 
        }

        private void detach_days(AlarmDayItem alarmDay)
        {
            NotifyPropertyChanging("Days");
            alarmDay.Alarm = null;
        }
    }

    [Table]
    public class AlarmDayItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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
   
        private DayOfWeek _day;

        [Column]
        public DayOfWeek Day
        {
            get
            {
                return _day;
            }
            set
            {
                if (_day != value)
                {
                    NotifyPropertyChanging("Day");
                    _day = value;
                    NotifyPropertyChanged("Day");
                }
            }
        }

        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 

        // Internal column for the associated ToDoCategory ID value 
        [Column] 
        internal int _alarmID; 
 
        // Entity reference, to identify the ToDoCategory "storage" table 
        private EntityRef<AlarmItem> _alarm; 
 
        // Association, to describe the relationship between this key and that "storage" table 
        [Association(Storage = "_alarm", ThisKey = "_alarmID", OtherKey = "ID", IsForeignKey = true)] 
        public AlarmItem Alarm 
        { 
            get { return _alarm.Entity; } 
            set 
            { 
                NotifyPropertyChanging("Alarm"); 
                _alarm.Entity = value; 
 
                if (value != null) 
                { 
                    _alarmID = value.ID; 
                } 
 
                NotifyPropertyChanging("Alarm"); 
            } 
        } 
    }

    [Table]
    public class AlarmNameItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 

        // Internal column for the associated ToDoCategory ID value 
        [Column] 
        internal int _alarmID; 
 
        // Entity reference, to identify the ToDoCategory "storage" table 
        private EntityRef<AlarmItem> _alarm; 
 
        // Association, to describe the relationship between this key and that "storage" table 
        [Association(Storage = "_alarm", ThisKey = "_alarmID", OtherKey = "ID", IsForeignKey = true)] 
        public AlarmItem Alarm 
        { 
            get { return _alarm.Entity; } 
            set 
            { 
                NotifyPropertyChanging("Alarm"); 
                _alarm.Entity = value; 
 
                if (value != null) 
                { 
                    _alarmID = value.ID; 
                } 
 
                NotifyPropertyChanging("Alarm"); 
            } 
        } 
    }
    #endregion

    #region Preferences Table
    [Table]
    public class PreferenceItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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

        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 
    }
    #endregion
}
