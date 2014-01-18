using System;
using System.Diagnostics;

using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlexiSDK.Util
{
    internal interface IPushService
    {
        event EventHandler<NotificationEventArgs> NotificationReceived;
    }

    internal class PushService : IPushService
    {
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public PushService()
        {
            Debug.WriteLine("push service initialized");

            HttpNotificationChannel pushChannel;

            string channelName = "PlexiPushChannel";

            pushChannel = HttpNotificationChannel.Find(channelName);

            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();
            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Pass MPNS URI back to server
                SendPushInfo(pushChannel.ChannelUri);
            }
        }

        private async void SendPushInfo(Uri channel)
        {
            Request request = new Request();

            JObject payload = new JObject();

            payload.Add("uri", channel);
            payload.Add("deviceid", Convert.ToBase64String((byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId")));
            payload.Add("os", Environment.OSVersion.Version.ToString());

            await request.DoRequestJsonAsync<JObject>("push.plexi.appspot.com", payload);
        }

        private void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            SendPushInfo(e.ChannelUri);
        }

        private void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {

        }

        private void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            EventHandler<NotificationEventArgs> handler = NotificationReceived;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
