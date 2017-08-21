using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TmdbServiceConnector.Models;
using Xamarin.Forms;

namespace TmdbServiceConnector
{
    public class DataServiceConnector
    {
        protected const string DefaultBaseUrl = "http://api.themoviedb.org";

        public List<Genre> Genres;

        public Configuration config;

        protected const string ApiKey = "1f54bd990f1cdfb230adb312546d765d"; // Eu usaria esta chave em algum arquivo de configuração, mas para este projeto decidi colocar Hard Code.

        public DataServiceConnector()
        {
            config = GetConfiguration().Body;
            Genres = new List<Genre>(GetGenreList().Body.Genres.ToList<Genre>());
        }

        public Response<GenresResult> GetGenreList()
        {
            return GetGenreListAsync().Result;
        }

        async public Task<Response<GenresResult>> GetGenreListAsync()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = createRequestUri("/genre/movie/list"),
                Method = HttpMethod.Get
            };

            Response<GenresResult> response = new Response<GenresResult>();

            response = await ExecuteRequestAsync<GenresResult>(request);

            return response;
        }

        public Response<Configuration> GetConfiguration()
        {
            return GetConfigurationAsync().Result;
        }

        public async Task<Response<Configuration>> GetConfigurationAsync()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = createRequestUri("/configuration"),
                Method = HttpMethod.Get                
            };

            Response<Configuration> response = new Response<Configuration>();
            
            response = await ExecuteRequestAsync<Configuration>(request);

            return response;
        }
                                
        public Response<MoviesResult> GetUpcomingMovies(int page = 1)
        {
            return GetUpcomingMoviesAsync(page).Result;
        }

        public async Task<Response<MoviesResult>> GetUpcomingMoviesAsync(int page = 1)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = createRequestUri("/movie/upcoming?page={0}", page),
                Method = HttpMethod.Get
            };

            var response = await ExecuteRequestAsync<MoviesResult>(request);
            return response;
        }
        
        public Response<MoviesResult> GetTopRatedMovies(int page = 1)
        {
            return GetTopRatedMoviesAsync(page).Result;
        }

        public async Task<Response<MoviesResult>> GetTopRatedMoviesAsync(int page = 1)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = createRequestUri("/movie/top_rated?page={0}", page),
                Method = HttpMethod.Get
            };

            var response = await ExecuteRequestAsync<MoviesResult>(request);
            return response;
        }

        public Response<MoviesResult> SearchMovies(string query, int page)
        {
            if (!string.IsNullOrEmpty(query))
                return SearchMoviesAsync(query, page).Result;
            else
                throw new Exception("Query is null!");
        }

        public async Task<Response<MoviesResult>> SearchMoviesAsync(string query, int page)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = createRequestUri("/search/movie?query={0}&page={1}", query, page),
                    Method = HttpMethod.Get
                };
                var response = await ExecuteRequestAsync<MoviesResult>(request);
                return response;
            }
            else
                throw new Exception("Query is null!");
        }

        private Response<T> ExecuteRequest<T>(HttpRequestMessage request) where T : new()
        {
            return ExecuteRequestAsync<T>(request).Result;
        }

        private async Task<Response<T>> ExecuteRequestAsync<T>(HttpRequestMessage request) where T : new()
        {
            try
            {
                using (var httpClient = new HttpClient(new HttpClientHandler()))
                {
                    httpClient.MaxResponseContentBufferSize = 256000;
                    // Send the request.
                    var httpResponseMessage = httpClient.GetAsync(request.RequestUri).Result;

                    // Read the content as string.
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();

                    var response = new Response<T>
                    {
                        HttpStatusCode = httpResponseMessage.StatusCode,
                        IsOk = httpResponseMessage.IsSuccessStatusCode,
                        Phrase = httpResponseMessage.ReasonPhrase
                    };

                    // Parse the content.
                    if (content != null)
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            // Parse the successful the response.
                            response.Body = Deserialize<T>(content);
                        }
                        else
                        {
                            // Parse the error response.
                            var error = Deserialize<Response>(content);
                            if (error != null)
                            {
                                response.StatusCode = error.StatusCode;
                                response.Status = error.Status;
                            }
                        }
                    }
                    return response;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        private Uri createRequestUri(string resource, params Object[] args)
        {
            if (!string.IsNullOrEmpty(resource))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(DefaultBaseUrl);
                builder.Append("/3");

                if (args != null)
                {
                    builder.AppendFormat(resource, args);
                }
                else
                {
                    builder.Append(resource);
                }

                if (resource.Contains("?"))
                {
                    builder.AppendFormat("&api_key={0}", ApiKey);
                }
                else
                {
                    builder.AppendFormat("?api_key={0}", ApiKey);
                }

                string url = builder.ToString();
                return new Uri(url);
            }
            else
                throw new Exception("Resource is null!");
        }

        public T Deserialize<T>(string value) where T : new()
        {
            if (value == null)
                return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception e)
            {                
                return default(T);
            }
        }
    }
}
