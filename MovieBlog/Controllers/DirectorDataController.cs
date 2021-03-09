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
using System.Web;
using System.IO;
using System.Diagnostics;

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
                DirectorHasPic = Director.DirectorHasPic,
                PicExtension = Director.PicExtension,
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
                    DirectorHasPic = Director.DirectorHasPic,
                    PicExtension = Director.PicExtension,
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
                    MovieHasPic = Movie.MovieHasPic,
                    PicExtension = Movie.PicExtension,
                    DirectorID = Movie.DirectorID
                };
                MovieDtos.Add(NewMovie);
            }
            return Ok(MovieDtos);
        }

        /// <summary>
        /// Gets a list of potential movies in the database for a specific Director ( Movies which the director is not a part of)
        /// </summary>
        /// <param name="id">Id of the Director</param>
        /// <returns>A list of potential movies which the drector is not a part of in the database</returns>
        /// <example>
        /// GET: api/DirectorData/PotentialMoviesofDirector/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<MovieDto>))]
        public IHttpActionResult PotentialMoviesofDirector(int id)
        {
            List<Movie> Movies = db.Movies
                .Where(m => m.DirectorID != id)
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
                    MovieHasPic = Movie.MovieHasPic,
                    PicExtension = Movie.PicExtension,
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
            // Picture update is handled by another method
            db.Entry(Director).Property(p => p.DirectorHasPic).IsModified = false;
            db.Entry(Director).Property(p => p.PicExtension).IsModified = false;

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
        /// Receives Director picture data, uploads it to the webserver and updates the director's HasPic option
        /// </summary>
        /// <param name="id">the director id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F directorpic=@file.jpg "https://localhost:xx/api/DirectorData/UpdateDirectorPic/2"
        /// POST: api/DirectorData/UpdateDirectorPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UpdateDirectorPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                int numfiles = HttpContext.Current.Request.Files.Count;

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var DirectorPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (DirectorPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(DirectorPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Actors/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Directors/"), fn);

                                //save the file
                                DirectorPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the Actor haspic and picextension fields in the database
                                Director SelectedDirector = db.Directors.Find(id);
                                SelectedDirector.DirectorHasPic = haspic;
                                SelectedDirector.PicExtension = extension;
                                db.Entry(SelectedDirector).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Director Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }

        /// <summary>
        /// Adds a movie for a director or establishes a connection between a movie and a director
        /// </summary>
        /// <param name="DirectorId">Id of the director associated with the movie</param>
        /// <param name="MovieId">Id of the movie which the director is a part of</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DirectorData/AddMovieForDirector/{DirectorId}/{MovieId}")]
        public IHttpActionResult AddMovieForDirector(int DirectorId, int MovieId)
        {
            Movie SelectedMovie = db.Movies
                .Where(m => m.MovieID == MovieId)
                .FirstOrDefault();

            Director DirectorSelected = db.Directors.Find(DirectorId);
            DirectorSelected.Movies = new List<Movie>();

            if (DirectorSelected.Movies.Count == 0)
            {
                DirectorSelected.Movies = new List<Movie>();
            }
            

            if (SelectedMovie == null || DirectorSelected == null || DirectorSelected.Movies.Contains(SelectedMovie))
            {
                return NotFound();
            }
            else
            {
               
                DirectorSelected.Movies.Add(SelectedMovie);
                db.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        ///     Removes a movie for a Director
        /// </summary>
        /// <param name="DirectorId">Id of the Director </param>
        /// <param name="MovieId">Id of the movie</param>
        /// <returns>
        ///     Returns not found if the SelectedMovie or DirectorSelected is null or if the they are not linked
        ///     If successfull , removes the movie from the movie list of the particular Director
        /// </returns>

        [HttpGet]
        [Route("api/DirectorData/RemoveMovieforDirector/{DirectorId}/{MovieId}")]
        public IHttpActionResult RemoveMovieforDirector(int DirectorId, int MovieId)
        {
            Movie SelectedMovie = db.Movies
               .Where(m => m.MovieID != MovieId)
               .FirstOrDefault();

            Director DirectorSelected = db.Directors.Find(DirectorId);

            if (SelectedMovie == null || DirectorSelected == null || !DirectorSelected.Movies.Contains(SelectedMovie))
            {
                return NotFound();
            }
            else
            {
                //Remove the sponsor from the team
                DirectorSelected.Movies.Remove(SelectedMovie);
                db.SaveChanges();
                return Ok();
            }

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

            return Ok(Director.DirectorID);
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

            if (Director.DirectorHasPic && Director.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Directors/" + id + "." + Director.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
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