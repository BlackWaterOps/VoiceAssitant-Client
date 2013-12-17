using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Image : ViewBase
    {
        public Image()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // prevent media viewer from resetting the layout
            this.ApplicationBar.Opacity = 0.9;
        }


        private void MediaViewer_ItemUnzoomed(object sender, EventArgs e)
        {
            if (this.ApplicationBar.IsVisible == false)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.ApplicationBar.IsVisible = true;
                });
            }
        }

        private void MediaViewer_ItemZoomed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.ApplicationBar.IsVisible = false;
            }); 
        }

        /* while this method could work to pin an image, when a user clicks on it,
         * the rest of the images would need to be retrieved again which could get complicated.
         * Especially if search returns different images than before 
        protected void PinToStartButton_Click(object sender, EventArgs e)
        {
            int idx = MediaViewer.DisplayedItemIndex;

            Uri currentImage = (DataContext as ImageViewModel).GetImageAtIndex(idx);

            var tile = new FlipTileData();

            tile.BackgroundImage = currentImage;
            //tile.BackContent = "";
            //tile.Title = tile.BackTitle = "";
            tile.Count = 0;

            Uri uri = new Uri(String.Format(ViewModelLocator.FullImageUri, idx));

            ShellTile.Create(uri, tile);
        }
        */

        protected void ShareButton_Click(object sender, EventArgs e)
        {
            int idx = MediaViewer.DisplayedItemIndex;

            Uri currentImage = (DataContext as ImageViewModel).GetImageAtIndex(idx);

            ShareMediaTask task = new ShareMediaTask();

            task.FilePath = currentImage.OriginalString;
            
            task.Show();
        }
    }
}