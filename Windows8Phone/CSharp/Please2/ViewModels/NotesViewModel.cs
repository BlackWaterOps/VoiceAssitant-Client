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

        private DatabaseModel db = new DatabaseModel(AppResources.DataStore);

        public void LoadNotes()
        {
            try
            {
                if (db.DatabaseExists() == true)
                {
                    IQueryable<NoteItem> query = from NoteItem notes in db.Notes select notes;

                    IOrderedQueryable<NoteItem> orderedQuery = query.OrderBy(note => (DateTime)note.ModifiedDate);

                    Notes = new List<NoteItem>(orderedQuery);
                }
                else
                {
                    Notes = new List<NoteItem>();
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

        public int SaveNote(byte[] thumbnail, string title, UIElementCollection body)
        {
            try
            {
                DatabaseSchemaUpdater dbUpdater;

                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();

                    // set the new database version
                    dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                    dbUpdater.Execute();
                }
                else
                {
                    // check if db update is needed
                    /*
                    dbUpdater = db.CreateDatabaseSchemaUpdater();

                    if (dbUpdater.DatabaseSchemaVersion < App.APP_VERSION)
                    {
                        if (dbUpdater.DatabaseSchemaVersion < 2)
                        {
                            dbUpdater.AddColumn<NoteItem>("OrderID");
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
                        Style = (int)box.Tag,
                        Note = newNote
                    };

                    db.NoteBody.InsertOnSubmit(newNoteBody);
                }

                db.SubmitChanges();

                return newNote.ID;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("SaveNote Error: {0}", err.Message));
            }

            return default(int);
        }

        public void UpdateNote(int noteID, byte[] thumbnail, string title, UIElementCollection body)
        {
            try
            {
                // update note query
                NoteItem currentNote = GetNoteItem(noteID);

                currentNote.Title = title;
                currentNote.Thumbnail = thumbnail;
                currentNote.ModifiedDate = DateTime.Now;

                // remove current note body and replace with new body
                //IQueryable<NoteItemBody> noteBodyQuery = from NoteItemBody noteBody in db.NoteBody where noteBody._noteID == noteID select noteBody;

                List<NoteItemBody> currentNoteBody = GetNoteItemBody(noteID);

                foreach (NoteItemBody line in currentNoteBody)
                {
                    db.NoteBody.DeleteOnSubmit(line);
                }

                // add new notebody
                foreach (TextBox box in body)
                {
                    NoteItemBody newNoteBody = new NoteItemBody
                    {
                        Text = box.Text,
                        Style = (int)box.Tag,
                        Note = currentNote
                    };

                    db.NoteBody.InsertOnSubmit(newNoteBody);
                }

                db.SubmitChanges();

                Debug.WriteLine(String.Format("updated note id {0}", currentNote.ID));
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("UpdateNote Error: {0}", err.Message));
            }
        }

        //TODO: find a way to query both tables at same time
        public Dictionary<string, object> GetNote(int noteID)
        {
            try
            {
                NoteItem currentNote = GetNoteItem(noteID);

                List<NoteItemBody> currentNoteBody = GetNoteItemBody(noteID);

                Dictionary<string, object> results = new Dictionary<string, object>();

                results.Add("note", currentNote);
                results.Add("body", currentNoteBody);

                Debug.WriteLine("return results");
                
                return results;
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

            return default(Dictionary<string, object>);
        }

        public void DeleteNote(int noteID)
        {
            NoteItem currentNote = GetNoteItem(noteID);
 
            db.Notes.DeleteOnSubmit(currentNote);

            List<NoteItemBody> currentNoteBody = GetNoteItemBody(noteID);

            foreach (NoteItemBody line in currentNoteBody)
            {
                db.NoteBody.DeleteOnSubmit(line);
            }

            db.SubmitChanges();
        }

        private NoteItem GetNoteItem(int noteID)
        {
            IQueryable<NoteItem> noteQuery = from NoteItem note in db.Notes where note.ID == noteID select note;

            return noteQuery.First();
        }

        private List<NoteItemBody> GetNoteItemBody(int noteID)
        {
            IQueryable<NoteItemBody> noteBodyQuery = from NoteItemBody noteBody in db.NoteBody where noteBody._noteID == noteID select noteBody;

            return noteBodyQuery.ToList();
        }
    }
}
