using MoviesListProject.Helpers;
using MoviesListProject.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector;
using TmdbServiceConnector.Models;
using Xamarin.Forms;

namespace MoviesListProject.ViewModels
{
    public class MoviesListViewModel : BaseViewModel
    {
        public ICommandEx NextCommand { get; }
        public ICommandEx BackCommand { get; }
       
        public ObservableRangeCollection<Movie> Movies { get; set; }

        INavigation Navigator { get; }        

        public DataServiceConnector service;
        private int page;
        private int listPage;

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

        public MoviesListViewModel(DataServiceConnector _service, INavigation _navigator, int _listType)
        {
            page = 1;
            service = _service;
            listPage = _listType;
            Movies = new ObservableRangeCollection<Movie>();
            Task.Run(async () => { await SelectListTypePage(); });
            Navigator = _navigator;
            NextCommand = AsyncCommand.Create("The Next Command", NextPageAsync);
            BackCommand = AsyncCommand.Create("The Back Command", BackPageAsync);
            
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

        private async Task SelectListTypePage()
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();
            switch(listPage)
            {
                case 0:
                    movies = await service.GetUpcomingMoviesAsync(page);
                    Title = "Upcoming Movies";
                    break;
                case 1:
                    movies = await service.GetTopRatedMoviesAsync(page);
                    Title = "Top Rated Movies";
                    break;
                default:
                    break;
            }
            Movies?.Clear();
            Movies.AddRange(movies.Body.Results.ToList<Movie>());
        }        
    }
}
