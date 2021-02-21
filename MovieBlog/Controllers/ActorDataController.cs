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
    public class ActorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Get a list of all the actors in the database
        /// </summary>
        /// <param name=""></param>
        /// <returns>A list of Actors including their ID, name and Bio</returns>
        /// <example>
        /// GET : api/ActorData/ListActors
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ActorDto>))]
        public IHttpActionResult ListActors()
        {
            List<Actor> Actors = db.Actors.ToList();
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
        /// Finds a particular actor in the database with a 200 status code.If the actor is not found return 404.
        /// </summary>
        /// <param name="id">The actor id </param>
        /// <returns>Information about the Actor, including the Actor id, Country, FirstName, LastName and Bio </returns>
        /// <example>
        /// GET: api/ActorData/FindActor/5
        /// </example>

        [HttpGet]
        [ResponseType(typeof(ActorDto))]
        public IHttpActionResult FindActor(int id)
        {
            Actor Actor = db.Actors.Find(id);
            if (Actor == null)
            {
                return NotFound();
            }
            ActorDto NewActor = new ActorDto
            {
                ActorID = Actor.ActorID,
                Country = Actor.Country,
                FirstName = Actor.FirstName,
                LastName = Actor.LastName,
                Bio = Actor.Bio
            };
            return Ok(NewActor);
        }
        
        /// <summary>
        /// Gets a list of movies in the database for a specific Actor
        /// </summary>
        /// <param name="id">Id of the Actor</param>
        /// <returns>A list of movies acted by the Actor</returns>
        /// <example>
        /// GET: api/ActorData/GetMoviesofActor/1
        /// </example>
        
        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult GetMoviesofActor(int id)
        {
            List<Movie> Movies = db.Movies
                .Where(m => m.Actors.Any(a => a.ActorID == id))
                .ToList();
            List<MovieDto> MovieDtos = new List<MovieDto> { };

            foreach(var Movie in Movies )
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
        /// Gets a list of potential movies in the database for a specific Actor ( Movies which the Actor is not a part of)
        /// </summary>
        /// <param name="id">Id of the Actor</param>
        /// <returns>A list of potential movies which the actor is not a part of in the database</returns>
        /// <example>
        /// GET: api/ActorData/GetPotentialMovies/1
        /// </example>

        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult GetPotentialMovies(int id)
        {
            List<Movie> Movies = db.Movies
                .Where(m => !m.Actors.Any(a => a.ActorID == id))
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
        /// Adds a movie for an actor or establishes a connection between a movie and an actor
        /// </summary>
        /// <param name="ActorId">Id of the actor associated with the movie</param>
        /// <param name="MovieId">Id of the movie which the actor is a part of</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ActorData/AddMovieforActor/{ActorId}/{MovieId}")]
        public IHttpActionResult AddMovieforActor(int ActorId,int MovieId)
        {
            Movie SelectedMovie = db.Movies.Include(m => m.Actors)
                .Where(m => m.MovieID == MovieId)
                .FirstOrDefault();

            Actor ActorSelected = db.Actors.Find(ActorId);


            if (SelectedMovie == null || ActorSelected == null || SelectedMovie.Actors.Contains(ActorSelected))
            {
                return NotFound();
            }
            else
            {
                ActorSelected.Movies = new List<Movie>();
                ActorSelected.Movies.Add(SelectedMovie);
                db.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        ///     Removes a movie for an actor
        /// </summary>
        /// <param name="ActorId">Id of the actor </param>
        /// <param name="MovieId">Id of the movie</param>
        /// <returns>
        ///     Returns not found if the SelectedMovie or ActorSelected is null or if the they are not linked
        ///     If successfull , removes the movie from the movie list of the particular actor
        /// </returns>
        
        [HttpGet]
        [Route("api/ActorData/RemoveMovieforActor/{ActorId}/{MovieId}")]
        public IHttpActionResult RemoveMovieforActor(int ActorId, int MovieId)
        {
            Movie SelectedMovie = db.Movies.Include(m => m.Actors)
               .Where(m => m.MovieID == MovieId)
               .FirstOrDefault();

            Actor ActorSelected = db.Actors.Find(ActorId);


            if (SelectedMovie == null || ActorSelected == null || !SelectedMovie.Actors.Contains(ActorSelected))
            {
                return NotFound();
            }
            else
            {
                //Remove the sponsor from the team
                ActorSelected.Movies.Remove(SelectedMovie);
                db.SaveChanges();
                return Ok();
            }
        }


        /// curl -H "Content-Type:application/json" 
        /// -d @UpdatedActor.json "https://localhost:44301/api/ActorData/UpdateActor/14"
        /// <summary>
        /// Updates an actor in the database with the new information provided.
        /// </summary>
        /// <param name="id">The id of the actor</param>
        /// <param name="actor">An Actor object,Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/ActorData/UpdateActor/5
        /// </example>

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateActor(int id, [FromBody]Actor Actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Actor.ActorID)
            {
                return BadRequest();
            }

            db.Entry(Actor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
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
        /// Adds an Actor to the database.
        /// </summary>
        /// <param name="Actor">An Actor object.Sent as POST form data</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/ActorData/AddActor
        /// FORM DATA: Actor JSON Object
        /// </example>
        
        [HttpPost]
        [ResponseType(typeof(Actor))]
        public IHttpActionResult AddActor([FromBody] Actor Actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Actors.Add(Actor);
            db.SaveChanges();

            return Ok(Actor.ActorID);
        }

        /// curl -d "" "https://localhost:44301/api/ActorData/DeleteActor/14"
        /// <summary>
        /// Deletes an Actor from the database.
        /// </summary>
        /// <param name="id">Id of the actor to be deleted</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/ActorData/DeleteActor/5
        /// </example>

        [HttpPost]
        [ResponseType(typeof(Actor))]
        public IHttpActionResult DeleteActor(int id)
        {
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return NotFound();
            }

            db.Actors.Remove(actor);
            db.SaveChanges();

            return Ok(actor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActorExists(int id)
        {
            return db.Actors.Count(e => e.ActorID == id) > 0;
        }
    }
}