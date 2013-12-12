using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

using Microsoft.Phone.Data.Linq;

using Please2.Models;
using Please2.Resources;

namespace Please2.ViewModels
{
    public class NotesViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private List<Note> notes;
        public List<Note> Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                RaisePropertyChanged("Notes");
            }
        }

        public void LoadNotes()
        {
            try
            {
                using (DatabaseModel db = new DatabaseModel(AppResources.DataStore))
                {
                    IQueryable<Note> query = from Note notes in db.Notes select notes;

                    IOrderedQueryable<Note> orderedQuery = query.OrderBy(note => (DateTime)note.UpdateDate);

                    Notes = new List<Note>(orderedQuery);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("LoadNotes Error: {0}", e.Message));
                return;
            }
        }

        public void SaveNote(byte[] thumbnail, string title, UIElementCollection body)
        {
            using (DatabaseModel db = new DatabaseModel(AppResources.DataStore))
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }
                else
                {
                    // check if db update is needed
                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

                    /*
                    if (dbUpdater.DatabaseSchemaVersion < App.APP_VERSION)
                    {
                        if (dbUpdater.DatabaseSchemaVersion < 2)
                        {
                            dbUpdater.AddColumn<Note>("OrderID");
                        }

                        dbUpdater.DatabaseSchemaVersion = 2;

                        dbUpdater.Execute();
                    }
                    */
                }

                db.Notes.InsertOnSubmit(
                    new Note
                    {
                        Thumbnail = thumbnail,
                        Title = title,
                        Body = body,
                        CreationDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    }
                );
            }
        }

        public void GetNotes()
        {

        }
    }
}
