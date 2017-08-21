using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public double Popularity { get; set; }
        public int Budget { get; set; }
        public int Revenue { get; set; }
        public int Runtime { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        [JsonProperty("genre_ids")]
        public int[] GenreId { get; set; }
        public List<Genre> Genres { get; set; }
        public string Homepage { get; set; }
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
        [JsonProperty("belongs_to_collection")]
        public Collection BelongsToCollection { get; set; }  
        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }
        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty("adult")]
        public bool IsAdult { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }        
        [JsonProperty("production_companies")]
        public List<Company> ProductionCompanies { get; set; }
        [JsonProperty("production_countries")]
        public List<Country> ProductionCountries { get; set; }
        [JsonProperty("spoken_languages")]
        public List<Language> SpokenLanguages { get; set; }
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }
        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        public string MoviePosterPath { get; set; }
    }
}
