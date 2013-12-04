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
        private static string accountNamePattern = @"[a-zA-Z0-9_.-]{5,32}";

        private static string passwordPattern = @"(.+){5,32}";

        private bool hasUnsavedChanges = false;

        private List<Control> fields = new List<Control>();

        public string Scheme { get { return "Settings"; } }

        public Registration() : base(false)
        {
            InitializeComponent();

            DataContext = this;

            this.fields.Add(AccountName);
            this.fields.Add(Password);
            this.fields.Add(ConfirmPassword);
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
            Password.Password = String.Empty;
            ConfirmPassword.Password = String.Empty;

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
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                if (CreateButton.IsEnabled == true)
                {
                    CreateButton_Tap(CreateButton, EventArgs.Empty);
                }
            }
            else
            {
                bool isError = false;

                foreach (Control element in this.fields)
                {
                    string val = String.Empty;

                    if (element.GetType() == typeof(TextBox))
                    {
                        val = (element as TextBox).Text;
                    }

                    if (element.GetType() == typeof(PasswordBox))
                    {
                        val = (element as PasswordBox).Password;
                    }

                    if (val == String.Empty)
                    {
                        isError = true;
                        break;
                    }

                    IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(element) as IList<VisualStateGroup>;

                    VisualStateGroup common = groups.Where(x => x.Name == "CommonStates").FirstOrDefault();

                    if (common != null && common.CurrentState.Name == "Error")
                    {
                        isError = true;
                    }
                }

                if (isError == false)
                {
                    CreateButton.IsEnabled = true;
                }
            }
        }

        private async void CreateButton_Tap(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(AccountName.Text, accountNamePattern))
            {
                // invalid username
                MessageBox.Show("account name must be alphanumeric, at least 4 characters, and a maximum of 32 characters");
                return;
            }

            if (!Regex.IsMatch(Password.Password, passwordPattern))
            {
                // invalid password
                MessageBox.Show("password must be alphanumeric, at least 4 characters, and a maximum of 32 characters");
                return;
            }

            // validate password fields
            if (Password.Password != ConfirmPassword.Password)
            {
                MessageBox.Show("Your passwords do not match");
                return;
            }

            string accountName = AccountName.Text;
            string password = Password.Password;

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

        private void AccountName_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountName.Text != String.Empty && !Regex.IsMatch(AccountName.Text, accountNamePattern))
                {
                    // run visual state for error
                    VisualStateManager.GoToState(AccountName, "Error", false);
                    TextBlock content = AccountName.Descendants<TextBlock>().Cast<TextBlock>().Where(x => x.Name == "validationContent").FirstOrDefault();

                    content.Text = "account name must be alphanumeric between 5 and 32 characters";
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (Password.Password != String.Empty && !Regex.IsMatch(Password.Password, passwordPattern))
                {
                    // run visual state for error
                    VisualStateManager.GoToState(Password, "Error", false);

                    TextBlock content = Password.Descendants<TextBlock>().Cast<TextBlock>().Where(x => x.Name == "validationContent").FirstOrDefault();

                    content.Text = "password must be between 5 and 32 characters";
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void ConfirmPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfirmPassword.Password != String.Empty && ConfirmPassword.Password != Password.Password)
                {
                    // run visual state for error
                    VisualStateManager.GoToState(ConfirmPassword, "Error", false);

                    TextBlock content = ConfirmPassword.Descendants<TextBlock>().Cast<TextBlock>().Where(x => x.Name == "validationContent").FirstOrDefault();

                    content.Text = "passwords do not match";
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}