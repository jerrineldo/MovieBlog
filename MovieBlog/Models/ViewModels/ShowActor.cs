using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBlog.Models.ViewModels
{
    public class ShowActor
    {
        public ActorDto Actor { get; set; }

        public IEnumerable<MovieDto> PotentialMovies { get; set; }

        public IEnumerable<MovieDto> MoviesActed { get; set; }
    }
}