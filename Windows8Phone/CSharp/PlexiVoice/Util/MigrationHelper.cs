using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Linq;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Data.Linq;

using PlexiVoice.Models;
using PlexiVoice.Resources;

namespace PlexiVoice.Util
{
    class MigrationHelper
    {
        private DatabaseModel db = new DatabaseModel(AppResources.DataStore);
        
        public static readonly MigrationHelper Default = new MigrationHelper();

        private MigrationHelper()
        {
        }

        public void MigrateToVersion2()
        {
            try
            {
                PlexiVoice.ModelsV1.DatabaseModel oldDb = new PlexiVoice.ModelsV1.DatabaseModel("Data Source=isostore:/Plexi.sdf");

                Debug.WriteLine("migrate versions");

                if (oldDb.DatabaseExists() == true)
                {
                    Debug.WriteLine("old db exists");

                    if (db.DatabaseExists() == false)
                    {
                        db.CreateDatabase();
                    }

                    // migrate notes
                    /*
                    foreach (var note in oldDb.Notes)
                    {
                        EntitySet<NoteItemBody> noteBody = new EntitySet<NoteItemBody>();

                        foreach (var body in note.NoteBody)
                        {
                            noteBody.Add(new NoteItemBody() { Style = body.Style, Text = body.Text });
                        }

                        NoteItem newNote = new NoteItem
                        {
                            Thumbnail = note.Thumbnail,
                            Title = note.Title,
                            ModifiedDate = note.ModifiedDate,
                            CreationDate = note.CreationDate,
                            NoteBody = noteBody
                        };

                        db.Notes.InsertOnSubmit(newNote);
                    }
                    */

                    //migrate alarms
                    foreach (var alarm in oldDb.Alarms)
                    {
                        List<DayOfWeek> days = (from alarmDays in oldDb.AlarmDays where alarmDays.Alarm == alarm select alarmDays.Day).ToList();

                        EntitySet<AlarmNameItem> names = new EntitySet<AlarmNameItem>();

                        foreach (var name in alarm.Names)
                        {
                            var day = (from alarmDay in oldDb.AlarmDays where alarmDay._alarmID == alarm.ID select alarmDay).FirstOrDefault();

                            if (day != null)
                            {
                                names.Add(new AlarmNameItem() { Name = name.Name, Day = day.Day });
                            }
                            else
                            {
                                names.Add(new AlarmNameItem() { Name = name.Name });
                            }
                        }

                        AlarmItem newAlarm = new AlarmItem
                        {
                            DisplayName = alarm.DisplayName,
                            IsEnabled = alarm.IsEnabled,
                            Names = names,
                            Time = alarm.Time,
                            Interval = alarm.Interval,
                        };

                        db.Alarms.InsertOnSubmit(newAlarm);
                    }

                    // db.SubmitChanges();

                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = App.APP_VERSION;
                    //dbUpdater.Execute();

                    //var storage = IsolatedStorageFile.GetUserStoreForApplication();

                    //storage.DeleteFile("Plexi.sdf");
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
