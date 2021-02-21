using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MovieBlog.Models;

namespace MovieBlog.Controllers
{
    public class DirectorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Gets the details of a director in the database based on the Id passed as argument.
        /// </summary>
        /// <param name="id">Id of the Director to be searched</param>
        /// <returns>Details of the director whos Id was passsed.The details include the FirstName, LastName, Country, Bio and DirectorId</returns>
        /// <example>
        /// GET: api/DirectorData/FindDirector/5
        /// </example>

        [HttpGet]
        [ResponseType(typeof(DirectorDto))]
        public IHttpActionResult FindDirector(int id)
        {
            Director Director = db.Directors.Find(id);
            if (Director == null)
            {
                return NotFound();
            }

            DirectorDto NewDirector = new DirectorDto
            {
                DirectorID = Director.DirectorID,
                FirstName = Director.FirstName,
                LastName = Director.LastName,
                Country = Director.Country,
                Bio = Director.Bio
            };

            return Ok(NewDirector);
        }

        /// <summary>
        /// List all the directors in the database
        /// </summary>
        /// <returns>List of all directors including their DirectorId,FirstName, LastName, Country, and Bio.</returns>
        /// <example>
        /// GET : api/DirectorData/ListDirectors
        /// </example>
       
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DirectorDto>))]
        public IHttpActionResult ListDirectors()
        {
            List<Director> Directors = db.Directors.ToList();
            List<DirectorDto> DirectorDtos = new List<DirectorDto>();
            
            foreach(var Director in Directors)
            {
                DirectorDto NewDirector = new DirectorDto
                {
                    DirectorID = Director.DirectorID,
                    FirstName = Director.FirstName,
                    LastName = Director.LastName,
                    Country = Director.Country,
                    Bio = Director.Bio
                };
                DirectorDtos.Add(NewDirector);
            }
            return Ok(DirectorDtos);
        }

        /// <summary>
        /// Returns the Movies done by the director
        /// </summary>
        /// <param name="id">Id of the Director</param>
        /// <returns>The details of all the Movies done by the director including their MovieId,Title,YearReleased,Review</returns>
        /// <example>
        /// GET: api/DirectorData/MoviesofDirector/1
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult MoviesofDirector(int id)
        {
            List<Movie> Movies = db.Movies
                .Where(m => m.DirectorID == id)
                .ToList();

            List<MovieDto> MovieDtos = new List<MovieDto> { };

            foreach (var Movie in Movies)
            {
                MovieDto NewMovie = new MovieDto
                {
                    MovieID = Movie.MovieID,
                    Title = Movie.Title,
                    YearReleased = Movie.YearReleased,
                    Review = Movie.Review,
                    DirectorID = Movie.DirectorID
                };
                MovieDtos.Add(NewMovie);
            }
            return Ok(MovieDtos);
        }

        /// <summary>
        /// Updates the details of the director
        /// </summary>
        /// <param name="id">Id of the director to be updated.</param>
        /// <param name="Director">A Director Object.Received as POST Form Data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/DirectorData/UpdateDirector/5
        /// </example>
        
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateDirector(int id, [FromBody]Director Director)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Director.DirectorID)
            {
                return BadRequest();
            }

            db.Entry(Director).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirectorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds a Director to the database
        /// </summary>
        /// <param name="Director">A Director object.Sent as POST form data</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/DirectorData/AddDirector
        /// FORM DATA: Director JSON Object
        /// </example>

        [HttpPost]
        [ResponseType(typeof(Director))]
        public IHttpActionResult AddDirector(Director Director)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Directors.Add(Director);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Director.DirectorID }, Director);
        }

        /// <summary>
        /// Deletes a Director from the database
        /// </summary>
        /// <param name="id">Id of the Director to be deleted</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/DirectorData/DeleteDirector/5
        /// </example>
        
        [HttpPost]
        public IHttpActionResult DeleteDirector(int id)
        {
            Director Director = db.Directors.Find(id);
            if (Director == null)
            {
                return NotFound();
            }

            db.Directors.Remove(Director);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DirectorExists(int id)
        {
            return db.Directors.Count(e => e.DirectorID == id) > 0;
        }
    }
}