using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Please2.ViewModels;

namespace Please2.Pages
{
    public partial class MoviesPage : PhoneApplicationPage
    {
        private MoviesViewModel viewModel;
        
        public MoviesPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = new MoviesViewModel();

            await viewModel.GetMovies();

            DataContext = viewModel;

            //string id = null;

            //NavigationContext.QueryString.TryGetValue("id", out id);
        }
    }
}