using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Please
{
    public partial class Gallery : PhoneApplicationPage
    {
        public Gallery()
        {
            InitializeComponent();

            DataContext = App.GalleryViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var listener = GestureService.GetGestureListener(this);
            
            listener.DragCompleted += DragHandler;
            listener.PinchCompleted += PinchHandler;

            base.OnNavigatedTo(e);
        }

        protected void ImageTapped(object sender, EventArgs e)
        {

        }

        // TODO HANDLE IMAGE FLICKS AND PINCH/ZOOM
        protected void DragHandler(object sender, DragCompletedGestureEventArgs e)
        {

        }

        protected void PinchHandler(object sender, PinchGestureEventArgs e)
        {

        }
    }
}