using MoviesListProject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector.Models;

namespace MoviesListProject.ViewModels
{
    public class MovieDetailsViewModel : BaseViewModel
    {        
        public Movie movie { get; set; }
        public string MoviePrincipalGenre { get; set; }
        public string OriginalTitle { get; set; }

        
        public MovieDetailsViewModel(Movie _movie)
        {
            movie = _movie;
            Title = movie.Title;
            MoviePrincipalGenre = movie.Genres == null ? string.Empty : GetGenres();              
        }

        public string GetGenres()
        {
            string returnGenre = string.Empty;
            for (int i = 0; i < movie.Genres.Count(); i++)
            {
                if(i == movie.Genres.Count() -1)
                {
                    returnGenre += movie.Genres[i].Name + ".";
                }else
                {
                    returnGenre += movie.Genres[i].Name + ", ";
                }
            }
            return returnGenre;
        }
    }
}
