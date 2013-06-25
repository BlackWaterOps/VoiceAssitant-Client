using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Please.Models
{
    class PleaseContext : DataContext
    {
        public PleaseContext(string connectionString)
            : base(connectionString)
        {
        }

        // define our Contacts table
        public Table<ContactItem> Contacts;

        // define our Appointments table
        public Table<AppointmentItem> Appointments;

        public Table<OutputItem> Outputs;
    }

    [Table]
    public class ContactItem : ModelBase
    {
        private int _id;
       
        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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
                    NotifyPropertyChanging("ContactId");
                    _id = value;
                    NotifyPropertyChanged("ContactId");
                }
            }
        }

        private int _hash;

        [Column]
        public int Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (_hash != value)
                {
                    NotifyPropertyChanging("ContactHash");
                    _hash = value;
                    NotifyPropertyChanged("ContactHash");
                }
            }
        }
    }

    [Table]
    public class AppointmentItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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
                    NotifyPropertyChanging("AppointmentId");
                    _id = value;
                    NotifyPropertyChanged("AppointmentId");
                }
            }
        }

        private int _hash;

        [Column]
        public int Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (_hash != value)
                {
                    NotifyPropertyChanging("AppointmentHash");
                    _hash = value;
                    NotifyPropertyChanged("AppointmentHash");
                }
            }
        }
    }

    // dummy table to hold show output so I don't have to keep requesting data while testing 
    [Table]
    public class OutputItem : ModelBase
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
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
                    NotifyPropertyChanging("OutputId");
                    _id = value;
                    NotifyPropertyChanged("OutputId");
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
                    NotifyPropertyChanging("OutputValue");
                    _value = value;
                    NotifyPropertyChanged("OutputValue");
                }
            }
        }
    }
}
