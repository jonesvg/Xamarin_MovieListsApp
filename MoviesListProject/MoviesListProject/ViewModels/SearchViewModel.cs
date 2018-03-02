using MoviesListProject.Helpers;
using MoviesListProject.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector;
using TmdbServiceConnector.Models;
using Xamarin.Forms;

namespace MoviesListProject.ViewModels
{
    public class SearchViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public ICommandEx NextCommand { get; }
        public ICommandEx BackCommand { get; }
        public ICommandEx SearchCommand { get; }
        INavigation Navigator { get; }

        public DataServiceConnector service;
        public ObservableRangeCollection<Movie> Movies { get; set; }

        public string SearchText { get; set; }
        public string Query;

        private Movie selectedItem;
        public Movie SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value);
                if (selectedItem != null)
                    ItemSelectedAsync();
            }
        }

        public SearchViewModel(DataServiceConnector _service, INavigation _navigator)
        {
            Navigator = _navigator;
            service = _service;
            Movies = new ObservableRangeCollection<Movie>();
            NextCommand = AsyncCommand.Create("The Next Command", NextPageAsync);
            BackCommand = AsyncCommand.Create("The Back Command", BackPageAsync);
            SearchCommand = AsyncCommand.Create("The Item Selected Command", SearchAsync);
        }

        public async Task SearchAsync()
        {
            await SearchMovie(SearchText);
        }

        public async Task NextPageAsync()
        {
            MovePage(1);
            if (Page != 0)
                await SelectListTypePage();
        }

        public async Task BackPageAsync()
        {
            MovePage(-1);
            if (Page != 0)
                await SelectListTypePage();
        }

        public void ItemSelectedAsync()
        {
            try
            {
                SelectedMovie(service, Navigator, SelectedItem);
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
                //Here we can use a Xamarin insights to report the exception, or one other type of report to catch the exception!
            }
        }

        public async Task SearchMovie(string query)
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();
            Query = query;
            movies = await service.SearchMoviesAsync(query, Page);

            Movies?.Clear();
            Movies.AddRange(movies.Body.Results.ToList<Movie>());
        }

        public async Task SelectListTypePage()
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();

            movies = await service.SearchMoviesAsync(Query, Page);

            Movies?.Clear();
            Movies.AddRange(movies.Body.Results.ToList<Movie>());
        }
    }
}
