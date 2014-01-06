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
using Please2.Util;

using Plexi.Models;
using Plexi.Util;

namespace Please2.Views
{
    public partial class Registration : ViewBase
    {
        private Plexi.IPlexiService plexiService;

        public ColorScheme Scheme { get { return ColorScheme.Settings; } }

        ApplicationBarIconButton button;

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
        private bool IsRegisterValid()
        {
            if (RegisterEmail.IsValid && RegisterAccountName.IsValid && RegisterAccountPassword.IsValid)
            {
                return true;
            }
            
            return false;
        }

        private void AddRegisterButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "register";
            button.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
            button.Click += RegisterButton_Click;
            button.IsEnabled = IsRegisterValid();

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
            ApplicationBar.Buttons.Add(button);

            this.button = button;
        }

        private void RegisterControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {        
            if (IsRegisterValid())
            {
                this.button.IsEnabled = true;
            }
            else
            {
                this.button.IsEnabled = false;
            }
        
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(RegisterEmail.Text);
            Debug.WriteLine(RegisterAccountName.Text);
            Debug.WriteLine(RegisterAccountPassword.Password);

            this.button.IsEnabled = false;

            await RegisterUser(RegisterEmail.Text, RegisterAccountName.Text, RegisterAccountPassword.Password);
        }

        private async Task RegisterUser(string email, string accountName, string password)
        {
            RegisterModel response = await plexiService.RegisterUser(email, accountName, password);

            if (response.error != null)
            {
                this.button.IsEnabled = true;

                MessageBox.Show(response.error.msg, "Sign up failed", MessageBoxButton.OK);
                return;
            }

            await LoginUser(accountName, password);
        }
        #endregion

        #region SignIn
        private bool IsSignInValid()
        {
            if (SignInAccountName.Text != String.Empty && SignInAccountPassword.Password != String.Empty)
            {
                return true;
            }

            return false;
        }

        private void AddSignInButton()
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();

            button.Text = "sign in";
            button.IconUri = new Uri("/Assets/check.png", UriKind.Relative);
            button.Click += SignInButton_Click;
            button.IsEnabled = IsSignInValid();

            if (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
            ApplicationBar.Buttons.Add(button);

            this.button = button;
        }

        private void SignInControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {            
            if (IsSignInValid())
            {
                this.button.IsEnabled = true;
            }
            else
            {
                this.button.IsEnabled = false;
            }
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            this.button.IsEnabled = false;

            await LoginUser(SignInAccountName.Text, SignInAccountPassword.Password);
        }

        private async Task LoginUser(string accountName, string password)
        {
            LoginModel response = await plexiService.LoginUser(accountName, password);

            if (response.error != null)
            {
                this.button.IsEnabled = true;

                MessageBox.Show(response.error.msg, "Login failed", MessageBoxButton.OK);
                return;
            }

            NavigationService.Navigate(ViewModelLocator.MainMenuPageUri);
        }
        #endregion
    }
}