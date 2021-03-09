using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBlog.Models.ViewModels
{
    public class UpdateMovie
    {
        public MovieDto Movie { get; set; }

        public IEnumerable<DirectorDto> Directors { get; set; }

        public IEnumerable<ActorDto> ActorsinMovie { get; set; }

        public IEnumerable<ActorDto> PotentialActors { get; set; }

        public IEnumerable<GenreDto> GenresOfMovie { get; set; }

        public IEnumerable<GenreDto> PotentialGenresOfMovie { get; set; }
    }
}