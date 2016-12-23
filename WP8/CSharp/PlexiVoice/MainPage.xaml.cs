using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using LinqToVisualTree;

using PlexiVoice.Models;
using PlexiVoice.Resources;
using PlexiVoice.Util;
using PlexiVoice.ViewModels;
using PlexiVoice.Views;

namespace PlexiVoice
{
    public partial class MainPage : ViewBase
    {
        private const int LIMIT = 30;
        
        MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();

            vm = DataContext as MainViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            vm = (MainViewModel)DataContext;

            vm.Items.CollectionChanged += ItemsCollectionChanged;

            if (NavigationContext.QueryString.ContainsKey("voiceCommandName"))
            {
                string voiceCommand = NavigationContext.QueryString["voiceCommandName"];

                Debug.WriteLine(NavigationContext.QueryString);
                Debug.WriteLine(NavigationContext.QueryString["reco"]);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            vm.Items.CollectionChanged -= ItemsCollectionChanged;

            base.OnNavigatingFrom(e);
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Debug.WriteLine(e.Action);
            //Debug.WriteLine(e.NewStartingIndex);
         
            IEnumerable<DialogModel> queries = vm.Items.Where(x => x is DialogModel && (x as DialogModel).type == DialogType.Query).Cast<DialogModel>();

            if (queries.Count() == LIMIT)
            {
                vm.RemoveOldestQuery();
            }
        }

        //TODO: check if context is not dialog. if not run animation
        private void Template_Loaded(object sender, EventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element.DataContext is DialogModel)
            {
                element.Opacity = 1;
                element.RenderTransform = new CompositeTransform();

                if ((element.DataContext as DialogModel).type == DialogType.Query)
                {
                    // TODO: get dialog to top of page every time
                }

                return;
            }
            /*
            try
            {
                CompositeTransform composite = element.FindName("StackPanel_CompositeTransform") as CompositeTransform;

                composite.TranslateY = App.Current.Host.Content.ActualHeight;

                Storyboard sb = element.Resources["Template_Loaded"] as Storyboard;

                Storyboard.SetTarget(sb, element);

                sb.Begin();

                element.Loaded -= Template_Loaded;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
            */

            ScrollTo();
        }

        private void ScrollTo()
        {
            ScrollViewer scrollViewer = DialogList.Descendants<ScrollViewer>().Cast<ScrollViewer>().FirstOrDefault();

            if (scrollViewer != null)
            {
                Debug.WriteLine(String.Format("{0} {1}", scrollViewer.ScrollableHeight, scrollViewer.ExtentHeight));
 
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);

                scrollViewer.UpdateLayout();
            }
        }

        private void Glyphs_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DialogModel dialog = (sender as FrameworkElement).DataContext as DialogModel;

            if (dialog.link == null)
            {
                return;
            }

            WebBrowserTask task = new WebBrowserTask();

            task.Uri = new Uri(dialog.link, UriKind.Absolute);

            task.Show();
        }

        private void DialogItemContext_Click(object sender, EventArgs e)
        {
            DialogModel dialog = (sender as MenuItem).DataContext as DialogModel;

            Debug.WriteLine(dialog.message);

            Clipboard.SetText((string)dialog.message);
        }

        private void TemplateAnimation_Completed(object sender, EventArgs e)
        {
            ScrollViewer scrollViewer = DialogList.Descendants<ScrollViewer>().Cast<ScrollViewer>().FirstOrDefault();

            if (scrollViewer != null)
            {
                Debug.WriteLine("completed: " + scrollViewer.ScrollableHeight);
            }
        }
    }
}