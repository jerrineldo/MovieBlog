using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBlog.Models.ViewModels
{
    public class ShowMovie
    {
        public MovieDto Movie { get; set; }

        public IEnumerable<DirectorDto> Directors { get; set; }
    }
}