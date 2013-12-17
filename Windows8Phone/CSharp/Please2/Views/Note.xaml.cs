﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using LinqToVisualTree;

using Please2.ViewModels;
using Please2.Models;
using Please2.Util;

namespace Please2.Views
{
    public partial class Note : PhoneApplicationPage
    {
        private ListStyle listStyle = ListStyle.None;

        private TextBox currentTextBox;

        private const string unorderedBullet = "\u2022";

        private int? currentNote;

        NotesViewModel vm;

        public Note()
        {
            InitializeComponent();

            this.vm = new Please2.ViewModels.NotesViewModel();

            //Loaded += Note_Loaded;
        }

        private void Note_Loaded(object sender, EventArgs e)
        {
            if (this.currentNote.HasValue)
            {
                DeleteButton.IsEnabled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string noteid;

            NavigationContext.QueryString.TryGetValue("noteid", out noteid);

            if (noteid != null && noteid != String.Empty)
            {
                this.currentNote = Convert.ToInt32(noteid);
                LoadNote();
            }
        }

        #region event handlers
        private void NoteBody_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // set focus on last element
            if (NoteBodyStackPanel.Children.Count == 0)
            {
                AddTextBox();
            }
            else
            {
                //TODO: should put focus on closest TextBox to touch area
                (NoteBodyStackPanel.Children.Last() as TextBox).Focus();
            }
        }
        
        // TODO check sender for list style
        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;

            TextBox box = sender as TextBox;

            this.currentTextBox = box;

            this.listStyle = (ListStyle)box.Tag;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;

            switch (e.Key)
            {
                case Key.Enter:
                    if (this.listStyle != ListStyle.None && box.Text.Length == 0)
                    {
                        DisableStyle();
                    }
                    else
                    {
                        AddTextBox();
                    } 
                    break;

                case Key.Back:
                    if (box.Text.Length == 0)
                    {
                        switch (this.listStyle)
                        {
                            case ListStyle.None:
                                RemoveTextBox();
                                break;

                            default:
                                DisableStyle();                                
                                break;
                        }
                    }
                    break;
            }
        }

        private void UnorderedMenuItem_Click(object sender, EventArgs e)
        {
            switch (this.listStyle)
            {
                case ListStyle.None:
                    UpdateStyle(ListStyle.Unordered);
                    break;

                case ListStyle.Unordered:
                    DisableStyle();
                    break;

                case ListStyle.Ordered:
                    UpdateStyle(ListStyle.Unordered);
                    break;
            }
        }

        private void OrderedMenuItem_Click(object sender, EventArgs e)
        {
            switch (this.listStyle)
            {
                case ListStyle.None:
                    UpdateStyle(ListStyle.Ordered);
                    break;

                case ListStyle.Unordered:
                    UpdateStyle(ListStyle.Ordered);
                    break;

                case ListStyle.Ordered:
                    DisableStyle();
                    break;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {       
            WriteableBitmap bitmap = new WriteableBitmap(ContentPanel, null);
 
            MemoryStream stream = new MemoryStream();
            
            bitmap.SaveJpeg(stream, 221, 221, 0, 90);

            byte[] bitmapAsBytes = stream.ToArray();
  
            var vm = new Please2.ViewModels.NotesViewModel();

            if (this.currentNote.HasValue)
            {
                vm.UpdateNote(this.currentNote.Value, bitmapAsBytes, NoteTitle.Text, NoteBodyStackPanel.Children);
            }
            else
            {
                int id = vm.SaveNote(bitmapAsBytes, NoteTitle.Text, NoteBodyStackPanel.Children);

                if (id > 0)
                {
                    this.currentNote = id;

                    DeleteButton.IsEnabled = true;
                }
            }
        }

        // TODO: what happens when we delete a note? do we refresh the page or go to the list of notes
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (this.currentNote.HasValue)
            {
                this.vm.DeleteNote(this.currentNote.Value);
            }

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(ViewModelLocator.NotesUri);
            }
        }
        #endregion

        #region helpers
        //TODO: when disabling a line's style, we need to check the list style after the currentTextBox
        // if it's an ordered list, reorder them. 
        private void DisableStyle()
        {
            TextBlock block = this.currentTextBox.Descendants<TextBlock>().Cast<TextBlock>().FirstOrDefault();

            block.Visibility = Visibility.Collapsed;
            block.Text = String.Empty;

            this.listStyle = ListStyle.None;
            this.currentTextBox.Tag = ListStyle.None;

            int idx = NoteBodyStackPanel.Children.IndexOf(this.currentTextBox);

            if (idx < NoteBodyStackPanel.Children.Count)
            {
                TextBox nextBox = (TextBox)NoteBodyStackPanel.Children.ElementAt(idx + 1);

                if ((ListStyle)nextBox.Tag == ListStyle.Ordered)
                {
                    //Update(ListStyle.Ordered, nextBox);
                }
            }
        }

        private void UpdateStyle(ListStyle style, TextBox currentTextBox = null)
        {
            if (style.Equals(ListStyle.None))
            {
                return;
            }

            if (currentTextBox == null)
            {
                currentTextBox = this.currentTextBox;
            }

            // get start index
            TextBox startTextBox = currentTextBox.ElementsBeforeSelf<TextBox>().Cast<TextBox>().Where(x => (ListStyle)x.Tag != this.listStyle || (ListStyle)x.Tag == ListStyle.None).LastOrDefault();

            int startIdx = (startTextBox == null) ? -1 : NoteBodyStackPanel.Children.IndexOf(startTextBox);

            // get end index
            TextBox endTextBox = currentTextBox.ElementsAfterSelf<TextBox>().Cast<TextBox>().Where(x => (ListStyle)x.Tag != this.listStyle || (ListStyle)x.Tag == ListStyle.None).FirstOrDefault();

            int endIdx = (endTextBox == null) ? NoteBodyStackPanel.Children.Count : NoteBodyStackPanel.Children.IndexOf(endTextBox);

            int listNumber = 1;

            //Debug.WriteLine(String.Format("{0}:{1}", startIdx, endIdx));

            for (var i = startIdx; i < endIdx; i++)
            {
                /*
                Debug.WriteLine(String.Format("for loop: {0}", i));

                if (i >= 0)
                {
                    TextBox test = (TextBox)NoteBodyStackPanel.Children.ElementAt(i);

                    Debug.WriteLine(String.Format("for loop: {0}", ((ListStyle)test.Tag).ToString()));
                }
                */
                if (i != startIdx)
                {
                    TextBox current = (TextBox)NoteBodyStackPanel.Children.ElementAt(i);

                    TextBlock block = current.Descendants<TextBlock>().Cast<TextBlock>().FirstOrDefault();

                    switch (style)
                    {
                        case ListStyle.Unordered:
                            block.Text = unorderedBullet;
                            current.Tag = style;
                            break;

                        case ListStyle.Ordered:
                            block.Text = String.Format("{0}.", listNumber);
                            current.Tag = style;
                            break;
                    }

                    block.Visibility = Visibility.Visible;

                    listNumber++;
                }
            }

            this.listStyle = style;
        }

        private TextBox BuildNewTextBox()
        {
            TextBox box = new TextBox();

            //box.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 200, 200, 200) };
            box.Tap += TextBox_Tap;
            box.KeyDown += TextBox_KeyDown;
            box.TextWrapping = TextWrapping.Wrap;
            box.Tag = this.listStyle;
            box.Style = Resources["NoteText"] as Style;

            return box;
        }

        private void AddTextBox(string line = null, ListStyle style = ListStyle.None)
        {
            TextBox box = BuildNewTextBox();

            if (line != null)
            {
                box.Text = line;
                this.listStyle = style;
            }
            
            int idx = (this.currentTextBox == null) ? -1 : NoteBodyStackPanel.Children.IndexOf(this.currentTextBox);

            NoteBodyStackPanel.Children.Insert((idx + 1), box);

            NoteBodyStackPanel.UpdateLayout();

            if (line == null)
            {
                GeneralTransform transform = box.TransformToVisual(NoteBodyScrollViewer);

                Point pos = transform.Transform(new Point(0, 0));

                NoteBodyScrollViewer.ScrollToVerticalOffset(pos.Y);

                box.Focus();
            }

            this.currentTextBox = box;

            UpdateStyle(this.listStyle);
        }

        private void RemoveTextBox()
        {
            int idx = NoteBodyStackPanel.Children.IndexOf(this.currentTextBox);

            if (idx > 0)
            {
                NoteBodyStackPanel.Children.Remove(this.currentTextBox);

                this.currentTextBox = (TextBox)NoteBodyStackPanel.Children.ElementAt(idx - 1);

                // ensure cursor is at the end of the TextBox
                this.currentTextBox.Select(this.currentTextBox.Text.Length, 0);

                this.currentTextBox.Focus();

                this.listStyle = (ListStyle)this.currentTextBox.Tag;

                UpdateStyle(this.listStyle);
            }
        }

        private void LoadNote()
        {
            if (!this.currentNote.HasValue)
            {
                return;
            }

            Dictionary<string, object> note = this.vm.GetNote(this.currentNote.Value);

            NoteTitle.Text = (note["note"] as NoteItem).Title;

            List<NoteItemBody> body = note["body"] as List<NoteItemBody>;

            Debug.WriteLine(body.Count);

            foreach (NoteItemBody line in body)
            {
                AddTextBox(line.Text, (ListStyle)line.Style);
            }

            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }
        #endregion
    }
}