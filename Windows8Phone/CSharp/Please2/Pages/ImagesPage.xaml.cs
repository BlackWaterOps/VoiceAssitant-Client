using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Coding4Fun.Toolkit.Controls;

using LinqToVisualTree;

namespace Please2.Pages
{
    public partial class ImagesPage : PhoneApplicationPage
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
        
        // put in viewmodel
        private List<string> images;
        public List<string> Images
        {
            get { return images; }
        }

        public string SingleImage
        {
            get { return "http://ts1.mm.bing.net/th?id=H.4795460935616648&pid=15.1"; }
        }

        // put in viewmodel
        public double ScreenWidth
        {
            get { return Application.Current.Host.Content.ActualWidth; }
        }

        public ImagesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                var test = "[\"http://ts1.mm.bing.net/th?id=H.4795460935616648&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4793064379780934&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4980758731491717&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4864451004989771&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4849118011917927&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4943263647861386&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4943263647861384&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4592927499748292&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4619526242503831&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.5035412129055574&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4691720324251649&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4791561134015025&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4943263647861381&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4629877112113745&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4546739420136011&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4629877112113723&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4802959957690024&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4671830331097938&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4927672929749997&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4932513320010806&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4864451004989773&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4727767988044319&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4967929650022805&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4690616505795653&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4674106662912066&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4561015892082722&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4596157280487002&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4957359755821592&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.5009084005550020&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4693983778114474&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4734098766299698&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.5016497109270893&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4527626807674937&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.5015526481790603&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4727767988044309&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4700275874203465&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4552657882055617&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4728395056875137&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.5003500523356747&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4981995645044708&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4600555334536151&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4954593738819121&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4602775852616049&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4782515924634050&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4720840201928898&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4898454267429165&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4778830827291695&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4594933236107443&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4634704654697699&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4971266803500414&pid=15.1\"]";

                images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(test);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            DataContext = this;
        }

        protected void Image_Tap(object sender, EventArgs e)
        {
            // animate to full screen view of image
            // swipe left and right to view other images in full screen mode
            ImagesThumbnail.Visibility = Visibility.Collapsed;
            ImagesFull.Visibility = Visibility.Visible;
        }

        /// <summary>  
        /// Either the user has manipulated the image or the size of the viewport has changed. We only  
        /// care about the size.  
        /// </summary>  
        void viewport_ViewportChanged(object sender, System.Windows.Controls.Primitives.ViewportChangedEventArgs e)
        {
            Size newSize = new Size(viewport.Viewport.Width, viewport.Viewport.Height);
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

                viewport.Bounds = new Rect(0, 0, newWidth, newHeight);
                if (center)
                {
                    viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - viewport.ActualWidth) / 2),
                            Math.Round((newHeight - viewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * relativeMidpoint.X, newHeight * relativeMidpoint.Y);
                    Point origin = new Point(newImgMid.X - screenMidpoint.X, newImgMid.Y - screenMidpoint.Y);
                    viewport.SetViewportOrigin(origin);
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
            if (recompute && bitmap != null && viewport != null) 
            { 
                // Calculate the minimum scale to fit the viewport  
                double minX = viewport.ActualWidth / bitmap.PixelWidth; 
                double minY = viewport.ActualHeight / bitmap.PixelHeight; 

                minScale = Math.Min(minX, minY); 
            } 
 
            coercedScale = Math.Min(MaxScale, Math.Max(scale, minScale)); 
        }

        private void ImagesFull_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var listBox = sender as ListBox;

            Debug.WriteLine(e.DeltaManipulation.Translation.X + " - " + e.DeltaManipulation.Translation.Y);
            Debug.WriteLine(e.DeltaManipulation.Scale.X + " - " + e.DeltaManipulation.Scale.Y);

            //ListTransform.TranslateX = e.DeltaManipulation.Translation.X;
            //ListTransform.TranslateY = e.DeltaManipulation.Translation.Y;

            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                ListTransform.ScaleX *= e.DeltaManipulation.Scale.X;
                ListTransform.ScaleY *= e.DeltaManipulation.Scale.Y;
            }
        }
    }    
}