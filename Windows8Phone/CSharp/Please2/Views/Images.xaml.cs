using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Coding4Fun.Toolkit.Controls;

using GalaSoft.MvvmLight.Ioc;

using LinqToVisualTree;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Images : PhoneApplicationPage
    {
        const double MaxScale = 10; 
 
        private double scale = 1.0; 
        private double minScale; 
        private double coercedScale; 
        private double originalScale; 
 
        private Size viewportSize; 
        private bool pinching; 
        private Point screenMidpoint; 
        private Point relativeMidpoint; 
 
        private BitmapImage bitmap;  
        
        public Images()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!SimpleIoc.Default.IsRegistered<ImagesViewModel>())
            {
                SimpleIoc.Default.Register<ImagesViewModel>();
            }

            DataContext = SimpleIoc.Default.GetInstance<ImagesViewModel>(); 
        }

        protected void Image_Tap(object sender, EventArgs e)
        {
            // animate to full screen view of image
            // swipe left and right to view other images in full screen mode

            (DataContext as ImagesViewModel).SingleImage = (string)(sender as Image).Tag;

            ImagesThumbnail.Visibility = Visibility.Collapsed;
            ImageViewport.Visibility = Visibility.Visible;
        }

        /// <summary>  
        /// Either the user has manipulated the image or the size of the viewport has changed. We only  
        /// care about the size.  
        /// </summary>  
        void viewport_ViewportChanged(object sender, System.Windows.Controls.Primitives.ViewportChangedEventArgs e)
        {
            Size newSize = new Size(ImageViewport.Viewport.Width, ImageViewport.Viewport.Height);
            if (newSize != viewportSize)
            {
                viewportSize = newSize;
                CoerceScale(true);
                ResizeImage(false);
            }
        } 
        
        /// <summary>  
        /// Handler for the ManipulationStarted event. Set initial state in case  
        /// it becomes a pinch later.  
        /// </summary>  
        protected void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            pinching = false;
            originalScale = scale;
        }

        /// <summary>  
        /// Handler for the ManipulationDelta event. It may or may not be a pinch. If it is not a   
        /// pinch, the ViewportControl will take care of it.  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param> 
        protected void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!pinching)
                {
                    pinching = true;
                    Point center = e.PinchManipulation.Original.Center;
                    relativeMidpoint = new Point(center.X / ImageSingle.ActualWidth, center.Y / ImageSingle.ActualHeight);

                    var xform = ImageSingle.TransformToVisual(ImagesFull);
                    screenMidpoint = xform.Transform(center);
                }

                scale = originalScale * e.PinchManipulation.CumulativeScale;

                CoerceScale(false);
                ResizeImage(false);
            }
            else if (pinching)
            {
                pinching = false;
                originalScale = scale = coercedScale;
            }
        }

        /// <summary>  
        /// The manipulation has completed (no touch points anymore) so reset state.  
        /// </summary> 
        protected void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            pinching = false;
            scale = coercedScale;
        }

        /// <summary>  
        /// When a new image is opened, set its initial scale.  
        /// </summary>  
        void OnImageOpened(object sender, RoutedEventArgs e)
        {
            bitmap = (BitmapImage)ImageSingle.Source;

            // Set scale to the minimum, and then save it.  
            scale = 0;
            CoerceScale(true);
            scale = coercedScale;

            ResizeImage(true);
        } 

        /// <summary>  
        /// Adjust the size of the image according to the coerced scale factor. Optionally  
        /// center the image, otherwise, try to keep the original midpoint of the pinch  
        /// in the same spot on the screen regardless of the scale.  
        /// </summary>  
        /// <param name="center"></param> 
        void ResizeImage(bool center)
        {
            if (coercedScale != 0 && bitmap != null)
            {
                double newWidth = canvas.Width = Math.Round(bitmap.PixelWidth * coercedScale);
                double newHeight = canvas.Height = Math.Round(bitmap.PixelHeight * coercedScale);

                //double newWidth = Math.Round(bitmap.PixelWidth * coercedScale);
                //double newHeight = Math.Round(bitmap.PixelHeight * coercedScale);

                xform.ScaleX = xform.ScaleY = coercedScale;

                ImageViewport.Bounds = new Rect(0, 0, newWidth, newHeight);
                if (center)
                {
                    ImageViewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - ImageViewport.ActualWidth) / 2),
                            Math.Round((newHeight - ImageViewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * relativeMidpoint.X, newHeight * relativeMidpoint.Y);
                    Point origin = new Point(newImgMid.X - screenMidpoint.X, newImgMid.Y - screenMidpoint.Y);
                    ImageViewport.SetViewportOrigin(origin);
                }
            }
        }

        /// <summary>  
        /// Coerce the scale into being within the proper range. Optionally compute the constraints   
        /// on the scale so that it will always fill the entire screen and will never get too big   
        /// to be contained in a hardware surface.  
        /// </summary>  
        /// <param name="recompute">Will recompute the min max scale if true.</param>  
        protected void CoerceScale(bool recompute)
        {
            if (recompute && bitmap != null && ImageViewport != null) 
            { 
                // Calculate the minimum scale to fit the viewport  
                double minX = ImageViewport.ActualWidth / bitmap.PixelWidth; 
                double minY = ImageViewport.ActualHeight / bitmap.PixelHeight; 

                minScale = Math.Min(minX, minY); 
            } 
 
            coercedScale = Math.Min(MaxScale, Math.Max(scale, minScale)); 
        }

        private void ImagesFull_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var listBox = sender as ListBox;

            //if ()

            //Debug.WriteLine(e.DeltaManipulation.Translation.X + " - " + e.DeltaManipulation.Translation.Y);
            //Debug.WriteLine(e.DeltaManipulation.Scale.X + " - " + e.DeltaManipulation.Scale.Y);

            //ListTransform.TranslateX = e.DeltaManipulation.Translation.X;
            //ListTransform.TranslateY = e.DeltaManipulation.Translation.Y;

            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                ListTransform.ScaleX *= e.DeltaManipulation.Scale.X;
                ListTransform.ScaleY *= e.DeltaManipulation.Scale.Y;
            }
            else if (Math.Abs(e.DeltaManipulation.Translation.X) > Math.Abs(e.DeltaManipulation.Translation.Y))
            {
                //horizontal slide
                Debug.WriteLine("horizontal slide");

                var viewer = (VisualTreeHelper.GetChild(ImagesFull, 0) as FrameworkElement).FindName("ScrollViewer") as ScrollViewer;

                try
                {
                    viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    viewer.ScrollToHorizontalOffset(400);
                    //viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Dis;

                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
                //ListTransform.TranslateX = e.DeltaManipulation.Translation.X;
                //ListTransform.TranslateY = e.DeltaManipulation.Translation.Y;
            }

        }
    }    
}