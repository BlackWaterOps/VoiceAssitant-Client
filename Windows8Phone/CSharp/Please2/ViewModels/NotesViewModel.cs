using System;
using System.Collections.Generic;
using System.Data.Common;
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
        public string Scheme { get { return "Notes"; } }

        private List<NoteItem> notes;
        public List<NoteItem> Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                RaisePropertyChanged("Notes");
            }
        }

        private NoteItem currentNote;
        public NoteItem CurrentNote
        {
            get { return currentNote; }
            set
            {
                currentNote = value;
                RaisePropertyChanged("CurrentNote");
            }
        }

        private NoteItemBody currentNoteBody;
        public NoteItemBody CurrentNoteBody
        {
            get { return currentNoteBody; }
            set
            {
                currentNoteBody = value;
                RaisePropertyChanged("CurrentNoteBody");
            }
        }

        public void LoadNotes()
        {
            try
            {
                using (DatabaseModel db = new DatabaseModel(AppResources.DataStore))
                {                    
                    IQueryable<NoteItem> query = from NoteItem notes in db.Notes select notes;

                    IOrderedQueryable<NoteItem> orderedQuery = query.OrderBy(note => (DateTime)note.ModifiedDate);

                    Notes = new List<NoteItem>(orderedQuery);
                }
            }
            catch (DbException)
            {
                //database or table does not exist
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("LoadNotes Error: {0}", e.Message));
            }
        }

        public void SaveNote(byte[] thumbnail, string title, UIElementCollection body)
        {
            try
            {
                using (DatabaseModel db = new DatabaseModel(AppResources.DataStore))
                {
                    DatabaseSchemaUpdater dbUpdater;

                    var tab = db.GetTable<NoteItem>();



                    if (db.DatabaseExists() == false)
                    {
                        db.CreateDatabase();
                    }
                    else
                    {
                        // check if db update is needed
                        dbUpdater = db.CreateDatabaseSchemaUpdater();

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

                    NoteItem newNote = new NoteItem
                    {
                        Title = title,
                        Thumbnail = thumbnail,
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    foreach (TextBox box in body)
                    {
                        NoteItemBody newNoteBody = new NoteItemBody
                        {
                            Text = box.Text,
                            //Style = (int)box.Tag,
                            Note = newNote
                        };

                        db.NoteBody.InsertOnSubmit(newNoteBody);
                    }
                    
                    db.SubmitChanges();

                    // set the new database version
                    //dbUpdater = db.CreateDatabaseSchemaUpdater();
                    //dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                    //dbUpdater.Execute();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("SaveNote Error: {0}", err.Message));
            }
        }

        //TODO: find a way to query both tables at same time
        public void LoadNote(int noteID)
        {
            try
            {
                using (DatabaseModel db = new DatabaseModel(AppResources.DataStore))
                {
                    IQueryable<NoteItem> noteQuery = from NoteItem note in db.Notes where note.ID == noteID select note;

                    NoteItem currentNote = noteQuery.ToArray()[0];

                    IQueryable<NoteItemBody> noteBodyQuery = from NoteItemBody noteBody in db.NoteBody where noteBody._noteID == noteID select noteBody;

                    List<NoteItemBody> currentNoteBody = noteBodyQuery.ToList();
                }
            }
            catch (DbException dbErr)
            {
                //database or table does not exist
                Debug.WriteLine(String.Format("LoadNote Error: {0}", dbErr.Message));
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("LoadNote Error: {0}", err.Message));
            }
        }
    }
}
