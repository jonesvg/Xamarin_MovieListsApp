using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class Language
    {
        public string Name { get; set; }
        [JsonProperty("iso_639_1")]
        public string Code { get; set; }
    }
}
