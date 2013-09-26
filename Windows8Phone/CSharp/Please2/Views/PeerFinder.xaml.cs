using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Networking.Proximity;

using Please2.ViewModels;

namespace Please2.Views
{
    public partial class PeerFinder : PhoneApplicationPage
    {
        PeerFinderViewModel vm;

        public PeerFinder()
        {
            InitializeComponent();

            vm = new PeerFinderViewModel();

            DataContext = vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShowPeerList();
        }

        protected void ShowPeerList()
        {
            if (vm.PeerList.Count > 0)
            {
                PeerLongListSelector.Visibility = Visibility.Visible;
                PeerTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                PeerLongListSelector.Visibility = Visibility.Collapsed;
                PeerTextBlock.Visibility = Visibility.Visible;
            }
        }

        protected async void PeerItem_Tap(object sender, EventArgs e)
        {
            try
            {
                await vm.ConnectToPeer(sender as PeerInformation);

                MessageBox.Show("Connection successful.");
            }
            catch (Exception err)
            {
                MessageBox.Show("Connection failed: " + err.Message);
            }
        }

        protected void PairButton_Click(object sender, EventArgs e)
        {
            ProgBar.Visibility = Visibility.Visible;

            vm.FindPeers();

            ProgBar.Visibility = Visibility.Collapsed;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            vm.CancelBroadcast();
        }
    }
}