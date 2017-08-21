using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector.Models;

namespace TmdbServiceConnector
{
    interface IDataServiceConnector
    {
        Response<Configuration> GetConfiguration();

        Task<Response<Configuration>> GetConfigurationAsync();

        Response<Collection> GetCollection(int id);

        Task<Response<Collection>> GetCollectionAsync(int id);

        Response<CollectionImages> GetCollectionImages(int id);

        Task<Response<CollectionImages>> GetCollectionImagesAsync(int id);

        Response<Movie> GetMovie(int id);

        Task<Response<Movie>> GetMovieAsync(int id);

        Response<MoviesResult> GetSimilarMovies(int id, int page = 1);

        Task<Response<MoviesResult>> GetSimilarMoviesAsync(int id, int page = 1);

        Response<MoviesResult> GetUpcomingMovies(int page = 1);

        Task<Response<MoviesResult>> GetUpcomingMoviesAsync(int page = 1);

        Response<MoviesResult> GetNowPlayingMovies(int page = 1);

        Task<Response<MoviesResult>> GetNowPlayingMoviesAsync(int page = 1);

        Response<MoviesResult> GetPopularMovies(int page = 1);

        Task<Response<MoviesResult>> GetPopularMoviesAsync(int page = 1);

        Response<MoviesResult> GetTopRatedMovies(int page = 1);

        Task<Response<MoviesResult>> GetTopRatedMoviesAsync(int page = 1);

        Response<MoviesResult> SearchMovies(string query);

        Task<Response<MoviesResult>> SearchMoviesAsync(string query);

        Response<CollectionResult> SearchCollections(string query);

        Task<Response<CollectionResult>> SearchCollectionsAsync(string query);
    }
}
