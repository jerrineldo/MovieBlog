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
    public class MovieDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Get a list of all the movies in the database
        /// </summary>
        /// <param name=""></param>
        /// <returns>A list of movies including the MovieId, Title, YearReleased, Review and the DirectorId.</returns>
        /// <example>
        /// GET : api/MovieData/ListMovies
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult ListMovies()
        {
            List<Movie> Movies = db.Movies.ToList();
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
        /// Finds a movie in the database with a 200 status code.If the actor is not found return 404.
        /// </summary>
        /// <param name="id">Id of the Movie to be searched</param>
        /// <returns>Information about the Movie just searched including the MovieId, Title, YearReleased, Review and the DirectorId.</returns>
        /// <example>
        /// GET: api/MovieData/FindMovie/5
        /// </example>

        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult FindMovie(int id)
        {
            Movie Movie = db.Movies.Find(id);
            if (Movie == null)
            {
                return NotFound();
            }

            MovieDto NewMovie = new MovieDto
            {
                MovieID = Movie.MovieID,
                Title = Movie.Title,
                YearReleased = Movie.YearReleased,
                Review = Movie.Review,
                DirectorID = Movie.DirectorID
            };

            return Ok(NewMovie);
        }

        /// <summary>
        /// Gets the details of the director of a movie
        /// </summary>
        /// <param name="id">Id of the movie who's director is to be found out.</param>
        /// <returns>Details of the director including his FirstName, LastName, Country and Bio of the Director.</returns>
        /// <example>
        /// GET: api/MovieData/GetDirectorOfMovie/5
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(DirectorDto))]
        public IHttpActionResult GetDirectorOfMovie(int id)
        {
            Director Director = db.Directors
                .Where(d => d.Movies.Any(m => m.MovieID == id))
                .FirstOrDefault();

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
        /// Gets all the directors in the database 
        /// </summary>
        /// <returns>Details of all the directors including FirstName, LastName, Bio</returns>
        /// <example>
        ///     GET : api/MovieData/GetDirectors
        /// </example>
        
        [HttpGet]
        [ResponseType(typeof(DirectorDto))]
        public IHttpActionResult GetDirectors()
        {
            List<Director> Directors = db.Directors.ToList();

            List<DirectorDto> DirectorDtos = new List<DirectorDto> { };

            foreach (var Director in Directors)
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
        /// Gets the list of all the actors in the movie
        /// </summary>
        /// <param name="id">Id of the movie</param>
        /// <returns>The details of all the actors in the movie including their FirstName, LastName, Bio, ActorId and Country </returns>
        /// <example>
        /// GET: api/MovieData/FindActorsinMovie/1
        /// </example>

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ActorDto>))]
        public IHttpActionResult FindActorsinMovie(int id)
        {
            List<Actor> Actors = db.Actors
                .Where(a => a.Movies.Any(m => m.MovieID == id))
                .ToList();

            List<ActorDto> ActorDtos = new List<ActorDto> { };

            foreach (var Actor in Actors)
            {
                ActorDto NewActor = new ActorDto
                {
                    ActorID = Actor.ActorID,
                    Country = Actor.Country,
                    FirstName = Actor.FirstName,
                    LastName = Actor.LastName,
                    Bio = Actor.Bio
                };
                ActorDtos.Add(NewActor);
            }
            return Ok(ActorDtos);
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ActorDto>))]
        public IHttpActionResult GetPotentialActors(int id)
        {
            List<Actor> Actors = db.Actors
                .Where(a => !a.Movies.Any(m => m.MovieID == id))
                .ToList();

            List<ActorDto> ActorDtos = new List<ActorDto> { };

            foreach (var Actor in Actors)
            {
                ActorDto NewActor = new ActorDto
                {
                    ActorID = Actor.ActorID,
                    Country = Actor.Country,
                    FirstName = Actor.FirstName,
                    LastName = Actor.LastName,
                    Bio = Actor.Bio
                };
                ActorDtos.Add(NewActor);
            }
            return Ok(ActorDtos);
        }

        /// <summary>
        /// Returns the List of Genres of the movie
        /// </summary>
        /// <param name="id">Id of the movie</param>
        /// <returns>List of Genres,Each Genre contains details like GenreId, GenreName</returns>
        /// <example>
        /// GET: api/MovieData/FindGenresofMovie/1
        /// </example>

        [HttpGet]
        [ResponseType(typeof(IEnumerable<GenreDto>))]
        public IHttpActionResult FindGenresofMovie(int id)
        {
            List<Genre> Genres = db.Genres
                .Where(g => g.Movies.Any(m => m.MovieID == id))
                .ToList();

            List<GenreDto> GenreDtos = new List<GenreDto> { };

            foreach(var Genre in Genres)
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

        /// curl -H "Content-Type:application/json"
        /// -d @UpdatedMovie.json "https://localhost:44301/api/MovieData/UpdateMovie/10"
        /// <summary>
        /// Updates a Movie in the database , when the new information is provided. 
        /// </summary>
        /// <param name="id">The movie Id</param>
        /// <param name="Movie">A Movie object.Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/MovieData/UpdateMovie/5
        /// </example>

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateMovie(int id, [FromBody] Movie Movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Movie.MovieID)
            {
                return BadRequest();
            }

            db.Entry(Movie).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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

        /// curl -H "Content-Type:application/json" 
        /// -d @NewMovie.json "https://localhost:44301/api/MovieData/AddMovie"
        /// <summary>
        /// Adds a Movie to the database
        /// </summary>
        /// <param name="Movie">A Movie object.Sent as POST form data</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/MovieData/AddMovie
        /// FORM DATA: Movie JSON Object
        /// </example>

        [ResponseType(typeof(Movie))]
        [HttpPost]
        public IHttpActionResult AddMovie([FromBody] Movie Movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Movies.Add(Movie);
            db.SaveChanges();

            return Ok(Movie.MovieID);
        }

        /// Checked with Http Request (curl -d "" "https://localhost:44301/api/MovieData/DeleteMovie/9")
        /// <summary>
        /// Deletes a movie from the database
        /// </summary>
        /// <param name="id">Id of the movie to be deleted</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/MovieData/DeleteMovie/5
        /// </example>

        [HttpPost]
        public IHttpActionResult DeleteMovie(int id)
        {
            Movie Movie = db.Movies.Find(id);
            if (Movie == null)
            {
                return NotFound();
            }

            db.Movies.Remove(Movie);
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

        private bool MovieExists(int id)
        {
            return db.Movies.Count(e => e.MovieID == id) > 0;
        }
    }
}