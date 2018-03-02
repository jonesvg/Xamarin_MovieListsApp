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
    public partial class MoviesListPage : ContentPage
    {
        private MoviesListViewModel viewModel;

        public MoviesListPage(DataServiceConnector _service, int _listType)
        {
            InitializeComponent();
            BindingContext = viewModel = new MoviesListViewModel(_service, this.Navigation, _listType);
        }
        
    }
}