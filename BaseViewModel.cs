using MoviesListProject.Helpers;
using MoviesListProject.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TmdbServiceConnector;
using TmdbServiceConnector.Models;
using Xamarin.Forms;

namespace MoviesListProject.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Get the azure service instance
        /// </summary>
                
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        /// <summary>
        /// Private backing field to hold the title
        /// </summary>
        private string title = string.Empty;
        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private int page = 1;
        public int Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }        

        public void MovePage(int iterator)
        {
            if (Page + iterator != 0)
            {
                Page += iterator;
            }
        }
        

        public void SelectedMovie(DataServiceConnector service, INavigation Navigator, Movie SelectedItem)
        {           
            if (SelectedItem == null)
                return;
            SelectedItem.Genres = SelectedItem.Genres ?? new List<Genre>();

            foreach (var gn in SelectedItem.GenreId)
            {
                var genre = service.Genres.FirstOrDefault(x => x.Id == gn);
                if (genre != null)
                    SelectedItem.Genres.Add(genre);
            }

            if (string.IsNullOrEmpty(SelectedItem.MoviePosterPath))
            {
                var im = service.config.Images;
                SelectedItem.MoviePosterPath = im.BaseUrl + im.PosterSizes[5] + SelectedItem.PosterPath;
            }
            Navigator.PushAsync(new MovieDetailsPage(SelectedItem));

            // Manually deselect item
            SelectedItem = null;
        }
    }
}

