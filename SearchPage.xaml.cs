using MoviesListProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector;
using TmdbServiceConnector.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoviesListProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        SearchViewModel viewModel;

        public SearchPage()
        {
            InitializeComponent();
        }

        public SearchPage(DataServiceConnector _service)
        {
            InitializeComponent();
            BindingContext = viewModel = new SearchViewModel(_service, this.Navigation);
        }
        
    }
}