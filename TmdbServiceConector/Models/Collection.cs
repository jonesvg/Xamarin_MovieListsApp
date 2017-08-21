using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
    }
}
