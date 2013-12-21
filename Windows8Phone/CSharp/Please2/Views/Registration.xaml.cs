using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

using Plexi.Models;
using Plexi.Util;

namespace Please2.Views
{
    public partial class Registration : ViewBase
    {
        private static string emailPattern = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";

        private static string accountNamePattern = @"[a-zA-Z0-9_.-]{4,32}";

        private static string passwordPattern = @"(.+){4,32}";

        private Plexi.IPlexiService plexiService;

        public string Scheme { get { return "Settings"; } }

        public Registration() : base(false)
        {
            InitializeComponent();

            DataContext = this;

            plexiService = ViewModelLocator.GetServiceInstance<Plexi.IPlexiService>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
        }

        private void Pivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            int idx = pivot.Items.IndexOf(e.Item);

            switch (idx)
            {
                case 0:
                    AddSignInButton();
                    break;

                case 1:
                    AddRegisterButton();
                    break;
                   
            }
        }

        #region Register
        private bool isRegisterValid()
        {
            if (!Regex.IsMatch(RegisterEmail.Text, emailPattern))
            {
                return false;
            }

            if (!Regex.IsMatch(RegisterAccountName.Text, accountNamePattern))
            {
                return false;
            }

            if (!Regex.IsMatch(RegisterAccountPassword.Password, passwordPattern))
            {
                return false;
            }

            return true;
        }

        private void AddRegisterButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "register";
            button.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
            button.Click += RegisterButton_Click;
            button.IsEnabled = isRegisterValid();

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
            ApplicationBar.Buttons.Add(button);
        }

        private void RegisterControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (RegisterAccountName.Text != String.Empty && RegisterAccountPassword.Password != String.Empty)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            }
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            await RegisterUser(RegisterAccountName.Text, RegisterAccountPassword.Password);
        }

        private async Task RegisterUser(string accountName, string password)
        {
            Debug.WriteLine("register user");
            return;

            string errorMessage = String.Empty;

            if (!Regex.IsMatch(accountName, accountNamePattern))
            {
                errorMessage = "account name must be alphanumeric, at least 4 characters, and a maximum of 32 characters";
            }
            else if (!Regex.IsMatch(password, passwordPattern))
            {
                errorMessage = "password must be between 4 and 32 characters";
            }

            if (errorMessage != String.Empty)
            {
                MessageBox.Show(errorMessage, "Sign up failed", MessageBoxButton.OK);
                return;
            }

            RegisterModel response = await plexiService.RegisterUser(accountName, password);

            if (response.error != null)
            {
                MessageBox.Show(response.error.msg, "Sign up failed", MessageBoxButton.OK);
                return;
            }

            await LoginUser(accountName, password);
        }
        #endregion

        #region SignIn
        private bool isSignInValid()
        {
            if (!Regex.IsMatch(SignInAccountName.Text, accountNamePattern))
            {
                return false;
            }

            if (!Regex.IsMatch(SignInAccountPassword.Password, passwordPattern))
            {
                return false;
            }

            return true;
        }

        private void AddSignInButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "sign in";
            button.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
            button.Click += SignInButton_Click;
            button.IsEnabled = isSignInValid();

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
            ApplicationBar.Buttons.Add(button);
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            await LoginUser(SignInAccountName.Text, SignInAccountPassword.Password);
        }

        private async Task LoginUser(string accountName, string password)
        {
            Debug.WriteLine("login user");
            return;

            LoginModel response = await plexiService.LoginUser(accountName, password);

            if (response.error != null)
            {
                MessageBox.Show(response.error.msg, "Login failed", MessageBoxButton.OK);
                return;
            }

            NavigationService.Navigate(ViewModelLocator.MainMenuPageUri);
        }
        #endregion
    }
}