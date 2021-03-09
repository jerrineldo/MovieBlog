using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MovieBlog.Models
{
    //This class describes a Actor Entity
    public class Actor
    {
        [Key]
        public int ActorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public bool ActorHasPic { get; set; }
        public string PicExtension { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }

    //Actor DTO
    public class ActorDto
    {

        public int ActorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public bool ActorHasPic { get; set; }
        public string PicExtension { get; set; }

    }
}

