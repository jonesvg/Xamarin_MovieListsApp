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
            try
            {
                InitializeComponent();
                BindingContext = viewModel = new SearchViewModel(_service);
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Houston, we have a problem!", "OK");
            }
        }

        private void srcBar_SearchButtonPressed(object sender, EventArgs e)
        {
            viewModel.SearchMovie(srcBar.Text);
        }

        async private Task SearchListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var movie = args.SelectedItem as Movie;
            if (movie == null)
                return;
            try
            {
                movie.Genres = movie.Genres ?? new List<Genre>();

                foreach (var gn in movie.GenreId)
                {
                    var genre = viewModel.service.Genres.FirstOrDefault(x => x.Id == gn);
                    if (genre != null)
                        movie.Genres.Add(genre);
                }

                if (string.IsNullOrEmpty(movie.MoviePosterPath))
                {
                    var im = viewModel.service.config.Images;
                    movie.MoviePosterPath = im.BaseUrl + im.PosterSizes[5] + movie.PosterPath;
                }
                await Navigation.PushAsync(new MovieDetailsPage(movie));

                // Manually deselect item
                SearchListView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Houston, we have a problem!", "OK");
                //Here we can use a Xamarin insights to report the exception, or one other type of report to catch the exception!
            }
        }

        private void Button_Next(object sender, EventArgs e)
        {
            viewModel.ExecuteLoadNewMoviesCommand(+1);
        }

        private void Button_Back(object sender, EventArgs e)
        {
            viewModel.ExecuteLoadNewMoviesCommand(-1);
        }
    }
}