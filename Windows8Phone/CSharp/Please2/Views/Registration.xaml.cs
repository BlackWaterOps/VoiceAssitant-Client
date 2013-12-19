using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;

using LinqToVisualTree;

using Please2.ViewModels;

using Plexi.Util;

namespace Please2.Views
{
    public partial class Registration : ViewBase
    {
        private static string accountNamePattern = @"[a-zA-Z0-9_.-]{4,32}";

        private static string passwordPattern = @"(.+){4,32}";

        private bool hasUnsavedChanges = false;

        public string Scheme { get { return "Settings"; } }

        public Registration() : base(false)
        {
            InitializeComponent();

            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AccountName.Text = String.Empty;
            AccountPassword.Password = String.Empty;

            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (!hasUnsavedChanges)
            {
                return;
            }

            var result = MessageBox.Show("You are about to discard your registration. Continue?", "Warning", MessageBoxButton.OKCancel);
        }

        private void Control_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if ((ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled == true)
                    {
                        CreateButton_Tap(ApplicationBar.Buttons[0], EventArgs.Empty);
                    }
                    break;

                default:
                    if (AccountName.Text != String.Empty && AccountPassword.Password != String.Empty)
                    {
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    }
                    break;
            }
        }

        private async void CreateButton_Tap(object sender, EventArgs e)
        {
            string errorMessage = String.Empty;

            if (!Regex.IsMatch(AccountName.Text, accountNamePattern))
            {
                errorMessage = "account name must be alphanumeric, at least 4 characters, and a maximum of 32 characters";
            }
            else if (!Regex.IsMatch(AccountPassword.Password, passwordPattern))
            {
                errorMessage = "password must be between 4 and 32 characters";
            }

            if (errorMessage != String.Empty)
            {
                MessageBox.Show(errorMessage, "Sign up failed", MessageBoxButton.OK);
                return;
            }

            Debug.WriteLine("register me");

            return;

            string accountName = AccountName.Text;
            string password = AccountPassword.Password;

            Plexi.IPlexiService plexiService = ViewModelLocator.GetServiceInstance<Plexi.IPlexiService>();

            Dictionary<string, object> response = await plexiService.RegisterUser(accountName, password);

            if (response.ContainsKey("status"))
            {
                string status = (string)response["status"];

                if (status == "success")
                {
                    Dictionary<string, object> login = await plexiService.LoginUser(accountName, password);

                    Util.AccountHelper.Default.CheckAccounts();
                }
                else
                {
                    MessageBox.Show((string)response["error"]);
                }
            }
            else
            {
                if (response.ContainsKey("msg"))
                {
                    Dictionary<string, object> login = await plexiService.LoginUser(accountName, password);

                    Util.AccountHelper.Default.CheckAccounts();
                }
                else
                {
                    MessageBox.Show((string)response["error"]);
                }
            }
        }
    }
}