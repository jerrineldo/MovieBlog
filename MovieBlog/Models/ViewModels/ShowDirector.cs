using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBlog.Models.ViewModels
{
    public class ShowDirector
    {
        public DirectorDto Director { get; set; }

        public IEnumerable<MovieDto> PotentialMovies { get; set; }

        public IEnumerable<MovieDto> MoviesDirected { get; set; }
    }
}