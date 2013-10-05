using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.ViewModels
{
    class ImagesViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private List<object> imageList;
        public List<object> ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;
                RaisePropertyChanged("ImageList");
            }
        }

        private string prevImage;
        public string PrevImage
        {
            get { return prevImage; }
            set
            {
                prevImage = value;
                RaisePropertyChanged("PrevImage");
            }
        }

        private string nextImage;
        public string NextImage
        {
            get { return nextImage; }
            set
            {
                nextImage = value;
                RaisePropertyChanged("NextImage");
            }
        }

        private string singleImage;
        public string SingleImage
        {
            get { return singleImage; }
            set
            {
                singleImage = value;
                RaisePropertyChanged("SingleImage");
            }
        }

        public double ScreenWidth
        {
            get { return App.Current.Host.Content.ActualWidth; }
        }

        public ImagesViewModel()
        {
            ImageTest();
        }

        public void ImageTest()
        {
            try
            {
                var test = "[\"http://ts1.mm.bing.net/th?id=H.4849118068541064&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4795460982802565&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4793064428145847&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4864451053355390&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4864451053355393&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4943263696226690&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4592927548113968&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4802960006055248&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4791561181200916&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4927672967498706&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4619526294407513&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4943263696226692&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4943263696226689&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4629877148748260&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4967929707105072&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4759658158031041&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4546739467321920&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4690616562418559&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4971266860123574&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4954593776567680&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4932513357300863&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4629877148748280&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4692338838735459&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4600555381721877&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4594873166988109&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.5035412176241547&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4937590031976216&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.5052665067734615&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.5003500579979739&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4759658158031042&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4561015947919379&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4859473205463471&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4870897823974944&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4654023445973212&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4594933284472576&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4728395106420018&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4734098822922796&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4802960006055249&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4759658158031039&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4552657919804535&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4957359812444731&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4592927548113965&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4835601766876390&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4778830883915025&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4811184859644493&pid=15.1\",\"http://ts2.mm.bing.net/th?id=H.4795508253262441&pid=15.1\",\"http://ts4.mm.bing.net/th?id=H.4704708327770243&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4736740203891622&pid=15.1\",\"http://ts3.mm.bing.net/th?id=H.4999888541254982&pid=15.1\",\"http://ts1.mm.bing.net/th?id=H.4893841524393284&pid=15.1\"]";
                
                ImageList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(test);

                PrevImage = "http://ts1.mm.bing.net/th?id=H.4849118068541064&pid=15.1";
                SingleImage = "http://ts2.mm.bing.net/th?id=H.4795460982802565&pid=15.1";
                NextImage = "http://ts4.mm.bing.net/th?id=H.4793064428145847&pid=15.1";

            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
