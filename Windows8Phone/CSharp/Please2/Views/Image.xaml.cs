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

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class Image : ViewBase
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

        ImageViewModel vm;

        public Image()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string imageUri = null;

            NavigationContext.QueryString.TryGetValue("image", out imageUri);

            if (imageUri == null)
            {
                // redirect outa here
                Debug.WriteLine("no image uri could be found");
                return;
            }

            ImageSingle.Source = new BitmapImage(new Uri(imageUri, UriKind.Absolute));
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

                    GeneralTransform xform = ImageSingle.TransformToVisual(ImageViewport);
                    screenMidpoint = xform.Transform(center);
                }

                scale = originalScale * e.PinchManipulation.CumulativeScale;

                CoerceScale(false);
                ResizeImage(false);
            }
            else if (pinching)
            {
                Debug.WriteLine("is pinching");
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
                    //Debug.WriteLine(String.Format("viewport res: {0} x {1}", ImageViewport.ActualWidth, ImageViewport.ActualHeight));

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
    }
}