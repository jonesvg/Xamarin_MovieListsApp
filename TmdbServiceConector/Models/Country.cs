using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class Country
    {
        public string Name { get; set; }
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }        
    }
}
