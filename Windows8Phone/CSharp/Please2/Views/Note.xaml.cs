using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using LinqToVisualTree;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Note : PhoneApplicationPage
    {
        private ListStyle listStyle = ListStyle.None;

        private const string unorderedBullet = "\u2022";

        public enum ListStyle
        {
            None = 0, Ordered = 1, Unordered = 2
        }

        NotesViewModel vm;

        public Note()
        {
            InitializeComponent();

            this.vm = new Please2.ViewModels.NotesViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = vm;

            if (NoteBody.Blocks.Count == 0)
            {
                Debug.WriteLine("create first textbox");
                Paragraph paragraph = CreateNewParagraph();

                NoteBody.Blocks.Add(paragraph);

                TextBox box = GetTextBox(paragraph);

                box.Focus();
            }
        }

        private void RichTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Debug.WriteLine("richtextbox tap. create new element");

            // set focus on last element
        }
        
        // TODO check sender for list style
        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Debug.WriteLine("textbox tap");

            e.Handled = true;

            TextBox box = (sender as TextBox);

            this.listStyle = (ListStyle)box.Tag;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("textbox keyup");

            TextBox box = sender as TextBox;

            Paragraph paragraph = GetParagraph(box);

            int idx = NoteBody.Blocks.IndexOf(paragraph);

            if (box.Text.Length == 0 && e.Key.Equals(Key.Back))
            {
                if (this.listStyle != ListStyle.None)
                {
                    // remove list style for this textbox
                    TextBlock block = box.Descendants<TextBlock>().Cast<TextBlock>().FirstOrDefault();

                    if (block != null)
                    {
                        block.Visibility = Visibility.Collapsed;
                        block.Text = String.Empty;
                    }

                }
                else if (idx > 0)
                {
                    Paragraph previous = (Paragraph)NoteBody.Blocks.ElementAt(idx - 1);

                    NoteBody.Blocks.Remove(paragraph);

                    TextBox last = GetTextBox(previous);

                    last.Focus();
                }
            }

            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;

                TextBox newBox;

                if (this.listStyle == ListStyle.None)
                {
                    Debug.WriteLine("create new paragraph");
                    Paragraph newParagraph = CreateNewParagraph();

                    NoteBody.Blocks.Insert(idx + 1, newParagraph);

                    newBox = GetTextBox(newParagraph);

                    newBox.Focus();
                }
                else
                {
                    Debug.WriteLine("create new textbox in same paragraph");
                    InlineUIContainer container = (InlineUIContainer)paragraph.Inlines.First();

                    StackPanel panel = (StackPanel)container.Child;
                    
                    int boxIdx = panel.Children.IndexOf(box);

                    int newIdx = boxIdx + 1;

                    newBox = CreateTextBox();

                    panel.Children.Insert(newIdx, newBox);

                    panel.UpdateLayout();

                    newBox.Focus();

                    ReorderParagraph(panel);
                }
            }
        }

        //TODO check currently selected textbox for style type
        private void UnorderedMenuItem_Click(object sender, EventArgs e)
        {
            TextBox box = (TextBox)FocusManager.GetFocusedElement();

            TextBlock block = box.Descendants<TextBlock>().Cast<TextBlock>().FirstOrDefault();

            if (this.listStyle.Equals(ListStyle.Unordered))
            {
                this.listStyle = ListStyle.None;

                //UpdateListPrefix(false);

                if (block != null)
                {
                    block.Visibility = Visibility.Collapsed;
                    block.Text = String.Empty;
                }
            }
            else
            {
                this.listStyle = ListStyle.Unordered;

                ReorderParagraph();

                if (block != null)
                {
                    block.Text = unorderedBullet;
                    block.Visibility = Visibility.Visible;
                }

                //UpdateListPrefix(true);
            }
        }

        //TODO check currently selected textbox for style type
        private void OrderedMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listStyle.Equals(ListStyle.Ordered))
            {
                this.listStyle = ListStyle.None;
               
                UpdateListPrefix(false);
            }
            else
            {
                listStyle = ListStyle.Ordered;

                ReorderParagraph();

                UpdateListPrefix(true);
            }
        }

        #region helpers
        private TextBox CreateTextBox()
        {
            TextBox box = new TextBox();
            box.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 200, 200, 200) };
            box.Tap += TextBox_Tap;
            box.KeyDown += TextBox_KeyDown;
            box.TextWrapping = TextWrapping.Wrap;
            box.Style = Resources["NoteText"] as Style;
            box.Tag = this.listStyle;
            
            return box;
        }

        private Paragraph CreateNewParagraph()
        {
            Paragraph paragraph = new Paragraph();

            InlineUIContainer container = new InlineUIContainer();

            StackPanel panel = new StackPanel();

            TextBox box = CreateTextBox();

            panel.Children.Add(box);
            container.Child = panel;
            paragraph.Inlines.Add(container);

            return paragraph;
        }

        private void ReorderParagraph()
        {
            TextBox box = (TextBox)FocusManager.GetFocusedElement();

            StackPanel panel = (StackPanel)box.Parent;

            ReorderParagraph(panel);
        }

        private void ReorderParagraph(StackPanel panel)
        {
            var list = panel.Children.OfType<TextBox>();

            //foreach (TextBox box in list)
            for (var i = 0; i < list.Count(); i++)
            {
                TextBox box = list.ElementAt(i);

                TextBlock block = box.Descendants<TextBlock>().Cast<TextBlock>().Where(x => x.Name == "ListPrefix").FirstOrDefault();

                if (block != null)
                {
                    switch (this.listStyle)
                    {
                        case ListStyle.Ordered:
                            block.Text = String.Format("{0}.", (i + 1));
                            break;

                        case ListStyle.Unordered:
                            block.Text = unorderedBullet;
                            break;
                    }

                    if (block.Visibility.Equals(Visibility.Collapsed))
                    {
                        block.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private Paragraph GetParagraph(TextBox box)
        {
            StackPanel panel = box.Parent as StackPanel;

            InlineUIContainer container = panel.Parent as InlineUIContainer;

            TextPointer pointer = container.ElementStart;

            return pointer.Parent as Paragraph;
        }

        private TextBox GetTextBox(Paragraph paragraph)
        {
            InlineUIContainer container = (InlineUIContainer)paragraph.Inlines.First();

            StackPanel panel = (StackPanel)container.Child;

            return (TextBox)panel.Children.Last();
        }

        //TODO find out why panel reports two textboxes instead on one
        private void UpdateListPrefix(bool isVisible)
        {
            Debug.WriteLine("update list prefix");

            TextBox box = (TextBox)FocusManager.GetFocusedElement();

            StackPanel panel = (StackPanel)box.Parent;

            IEnumerable<TextBox> boxes = panel.Descendants<TextBox>().Cast<TextBox>();

            Debug.WriteLine(boxes.Count());

            foreach (TextBox item in boxes)
            {
                Debug.WriteLine(item.GetType());

                IEnumerable<TextBlock> blocks = item.Descendants<TextBlock>().Cast<TextBlock>();

                Debug.WriteLine(blocks.Count());

                TextBlock block = item.Descendants<TextBlock>().Cast<TextBlock>().Where(x => x.Name == "ListPrefix").FirstOrDefault();

                if (block != null)
                {
                    block.Visibility = (isVisible) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        #endregion
    }
}