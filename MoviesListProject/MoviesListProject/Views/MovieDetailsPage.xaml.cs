using MoviesListProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoviesListProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPage : ContentPage
    {
        MovieDetailsViewModel viewModel;
        public MovieDetailsPage()
        {
            InitializeComponent();
        }
        public MovieDetailsPage(Movie movie)
        {
            InitializeComponent();
            BindingContext = viewModel = new MovieDetailsViewModel(movie);
        }
    }
}