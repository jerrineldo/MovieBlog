using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; //Key attribute

namespace MovieBlog.Models
{
    public class Director
    {
        [Key]
        public int DirectorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public bool DirectorHasPic { get; set; }
        public string PicExtension { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }

    //DirectorDTO
    public class DirectorDto

    {
        public int DirectorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public bool DirectorHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}