using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class Response
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }
        [JsonProperty("status_message")]
        public string Status { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsOk { get; set; }
        public string Phrase { get; set; }
    }

    public class Response<T> : Response
    {
        public T Body { get; set; }

        public Response() { }

        public Response(T body)
        {
            Body = body;
        }

        public string GetResponseFormated()
        {
            return string.Format("HttpStatus={0}; StatusCode={1}; StatusMessage={2}; Body={3}", HttpStatusCode, StatusCode, Status, Body);
        }
    }
}
