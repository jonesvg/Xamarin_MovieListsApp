using MoviesListProject.Helpers;
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
        public ObservableRangeCollection<Movie> Movies { get; set; }
                
        public DataServiceConnector service;
        private int page;
        private int listPage;

        public MoviesListViewModel(DataServiceConnector _service, int _listType)
        {
            page = 1;
            service = _service;
            listPage = _listType;
            Movies = new ObservableRangeCollection<Movie>();
            SelectListTypePage();            
        }

        public void SelectListTypePage()
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();
            switch(listPage)
            {
                case 0:
                    movies = service.GetUpcomingMovies(page);
                    Title = "Upcoming Movies";
                    break;
                case 1:
                    movies = service.GetTopRatedMovies(page);
                    Title = "Top Rated Movies";
                    break;
                default:
                    break;
            }
            Movies?.Clear();
            Movies.AddRange(movies.Body.Results.ToList<Movie>());
        }


        public void ExecuteLoadNewMoviesCommand(int pageIterator)
        {
            if (page + pageIterator != 0)
            {
                page += pageIterator;
                SelectListTypePage();
            }
        }

    }
}
