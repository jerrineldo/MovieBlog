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
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Gets a list of all the Genre's present in the database
        /// </summary>
        /// <returns>The details of all the Genres present in the database including their GenreName and their GenreId.</returns>
        /// <example>
        /// GET: api/GenreData/ListGenres
        /// </example>

        [HttpGet]
        [ResponseType(typeof(IEnumerable<GenreDto>))]
        public IHttpActionResult ListGenres()
        {
            List<GenreDto> GenreDtos = new List<GenreDto>();
            List<Genre> Genres = db.Genres.ToList();

            foreach (var Genre in Genres)
            {
                GenreDto NewGenre = new GenreDto
                {
                    GenreID = Genre.GenreID,
                    GenreName = Genre.GenreName
                };
                GenreDtos.Add(NewGenre);
            }

            return Ok(GenreDtos);
        }


        /// <summary>
        /// Finds a Genre from the list of Genres based on the Id passed
        /// </summary>
        /// <param name="id">Id of the Genre to be searched</param>
        /// <returns>The details of the Genre searched . Details include name and Id of the Genre searched</returns>
        /// <example>
        /// GET: api/GenreData/FindGenre/5
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult FindGenre(int id)
        {
            Genre Genre = db.Genres.Find(id);
            if (Genre == null)
            {
                return NotFound();
            }
            GenreDto NewGenre = new GenreDto
            {
                GenreID = Genre.GenreID,
                GenreName = Genre.GenreName
            };

            return Ok(NewGenre);
        }

        /// <summary>
        /// Gets a list of movies in a particular Genre
        /// </summary>
        /// <param name="id">Genre id </param>
        /// <returns>A list of Movies which are in the Genre</returns>
        /// <example>
        /// GET: api/GenreData/MoviesofGenre/1
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult MoviesofGenre(int id)
        {
            List<Movie> Movies = db.Movies
                .Where(m => m.Genres.Any(g => g.GenreID == id))
                .ToList();

            List<MovieDto> MovieDtos = new List<MovieDto> { };

            foreach(var Movie in Movies)
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
        /// Updates a Genre in the database , when the new information is provided. 
        /// </summary>
        /// <param name="id">The Genre Id</param>
        /// <param name="Genre">A Genre object.Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/GenreData/UpdateGenre/5
        /// </example>

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateGenre(int id, [FromBody] Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Genre.GenreID)
            {
                return BadRequest();
            }

            db.Entry(Genre).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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
        /// Adds a Genre to the database
        /// </summary>
        /// <param name="Genre">A Genre object.Sent as POST form data</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/GenreData/AddGenre
        /// FORM DATA: Genre JSON Object
        /// </example>
        
        [HttpPost]
        [ResponseType(typeof(Genre))]
        public IHttpActionResult AddGenre(Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(Genre);
            db.SaveChanges();

            return Ok(Genre.GenreID);
        }

        /// <summary>
        /// Deletes a Genre from the database
        /// </summary>
        /// <param name="id">Id of the Genre to be deleted</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/GenreData/DeleteGenre/5
        /// </example>
        
        [HttpPost]
        public IHttpActionResult DeleteGenre(int id)
        {
            Genre Genre = db.Genres.Find(id);
            if (Genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(Genre);
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

        private bool GenreExists(int id)
        {
            return db.Genres.Count(e => e.GenreID == id) > 0;
        }
    }
}