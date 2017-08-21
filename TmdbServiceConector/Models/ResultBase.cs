using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    
    public class ResultBase<T>
    {
        public int Page { get; set; }
        public List<T> Results { get; set; }
        public List<T> Genres { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }
}
