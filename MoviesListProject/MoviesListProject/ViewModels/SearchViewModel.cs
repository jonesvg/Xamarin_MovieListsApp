using MoviesListProject.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector;
using TmdbServiceConnector.Models;

namespace MoviesListProject.ViewModels
{
    public class SearchViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public DataServiceConnector service;
        public ObservableRangeCollection<Movie> Movies { get; set; }
        public string Query;

        public SearchViewModel(DataServiceConnector _service)
        {
            service = _service;
            Movies = new ObservableRangeCollection<Movie>();
        }

        public void SearchMovie(string query)
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();
            Query = query;
            movies = service.SearchMovies(query, page);

            Movies?.Clear();
            Movies.AddRange(movies.Body.Results.ToList<Movie>());
        }
        private int page = 1;

        public void SelectListTypePage()
        {
            Response<MoviesResult> movies = new Response<MoviesResult>();
           
            movies = service.SearchMovies(Query, page);
                    
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
