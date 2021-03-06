﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBlog.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public string Review { get; set; }
        public bool MovieHasPic { get; set; }

        //If the movie has an image, record the extension of the image (.png, .gif, .jpg, etc.)
        public string PicExtension { get; set; }

        [ForeignKey("Director")]
        public int DirectorID { get; set; }
        public virtual Director Director { get; set; }
        public ICollection<Actor> Actors { get; set; }
        public ICollection<Genre> Genres { get; set; }
    }

    public class MovieDto
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public string Review { get; set; }
        public int DirectorID { get; set; }
        public bool MovieHasPic { get; set; }
        public string PicExtension { get; set; }

    }
}