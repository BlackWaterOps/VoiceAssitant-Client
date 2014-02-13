using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

using Windows.Networking.Proximity;

using GalaSoft.MvvmLight;

namespace VoiceAssistant.ViewModels
{
    public class PeerFinderViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public bool started = false;

        Windows.Networking.Proximity.PeerInformation requestingPeer;

        private List<PeerInformation> peerList;
        public List<PeerInformation> PeerList
        {
            get { return peerList; }
            set
            {
                peerList = value;
                RaisePropertyChanged("PeerList");
            }
        }

        public PeerFinderViewModel()
        {
            FindPeers();
        }

        public async void FindPeers()
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";

            try
            {
                var peers = await PeerFinder.FindAllPeersAsync();

                PeerList = new List<PeerInformation>(peers);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public void BroadcastForPeer()
        {
            if (started)
            {
                MessageBox.Show("You are already advertising for a connection.\n");
                return;
            }

            Windows.Networking.Proximity.PeerFinder.DisplayName = "Please Demo";

            if ((Windows.Networking.Proximity.PeerFinder.SupportedDiscoveryTypes &
                 Windows.Networking.Proximity.PeerDiscoveryTypes.Triggered) ==
                 Windows.Networking.Proximity.PeerDiscoveryTypes.Triggered)
            {
                Windows.Networking.Proximity.PeerFinder.TriggeredConnectionStateChanged +=
                    TriggeredConnectionStateChanged;

                MessageBox.Show("You can tap to connect a peer device that is " +
                                 "also advertising for a connection.\n");
            }
            else
            {
                MessageBox.Show("Tap to connect is not supported.\n");
            }

            if ((Windows.Networking.Proximity.PeerFinder.SupportedDiscoveryTypes &
                 Windows.Networking.Proximity.PeerDiscoveryTypes.Browse) !=
                 Windows.Networking.Proximity.PeerDiscoveryTypes.Browse)
            {
                MessageBox.Show("Peer discovery using Wifi-Direct is not supported.\n");
            }

            Windows.Networking.Proximity.PeerFinder.Start();
            started = true;
        }

        public void CancelBroadcast()
        {

        }

        public async Task ConnectToPeer(PeerInformation peer)
        {
            var socket = await Windows.Networking.Proximity.PeerFinder.ConnectAsync(peer);

            await socket.ConnectAsync(peer.HostName, "0");
        }

        public void TriggeredConnectionStateChanged(object sender, Windows.Networking.Proximity.TriggeredConnectionStateChangedEventArgs e)
        {
            if (e.State == Windows.Networking.Proximity.TriggeredConnectState.PeerFound)
            {
                MessageBox.Show("Peer found. You may now pull your devices out of proximity.\n");
            }
            if (e.State == Windows.Networking.Proximity.TriggeredConnectState.Completed)
            {
                MessageBox.Show("Connected. You may now send a message.\n");
                
                //SendMessage(e.Socket);
            }
        }
    }
}
