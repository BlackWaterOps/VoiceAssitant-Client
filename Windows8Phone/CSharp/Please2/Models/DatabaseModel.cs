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

        public Table<NoteItem> Notes;
        public Table<NoteItemBody> NoteBody;

        public Table<Alarm> Alarms;
        
        public Table<PreferenceItem> Preferences;
    }

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
    public class Alarm : ModelBase
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

        // Version column aids update performance. 
        [Column(IsVersion = true)]
        private Binary _version; 
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
