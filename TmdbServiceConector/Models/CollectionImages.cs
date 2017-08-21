using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmdbServiceConnector.Models
{
    public class CollectionImages
    {
        public int Id { get; set; }
        public List<Image> Backdrops { get; set; }
        public List<Image> Posters { get; set; }
    }
}
