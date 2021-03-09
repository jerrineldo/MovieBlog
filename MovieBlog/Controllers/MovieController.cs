using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using MovieBlog.Models;
using MovieBlog.Models.ViewModels;

namespace MovieBlog.Controllers
{
    public class MovieController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static MovieController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44301//api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        }

        /// <summary>
        /// Returns a List of all the movies in the database to the view
        /// </summary>
        /// <returns>
        ///     If successfully retrieved data from the tables , then returns the data to the view
        ///     If not -> Returns an error message to the Error View
        /// </returns>
        /// <example>
        /// GET: Movie/ListMovies
        /// </example>
        public ActionResult ListMovies()
        {
            string url = "MovieData/ListMovies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<MovieDto> ListOfMovies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
                return View(ListOfMovies);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        ///  Renders the view for creating a new Movie with the options to select a director
        /// </summary>
        /// <returns>Returns a view for creating a new Movie</returns>
        /// <example>
        /// GET : Movie/Create
        /// </example>
        
        [HttpGet]
        public ActionResult Create()
        {
            ShowMovie ViewModel = new ShowMovie();
            string url = "MovieData/GetDirectors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DirectorDto> Directors = response.Content.ReadAsAsync<IEnumerable<DirectorDto>>().Result;
                ViewModel.Directors = Directors;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Creates a new Movie in the database
        /// </summary>
        /// <param name="MovieInfo">Form data which has the details for the new Movie to be entered into the database</param>
        /// <returns>
        ///     If successfull , A new Movie would be entered into the database.
        ///     If not , then an error would be thrown 
        /// </returns>
        /// <example>
        /// POST: Movie/Create
        /// </example>

        [HttpPost]
        public ActionResult Create(Movie MovieInfo)
        {
            string url = "MovieData/AddMovie";
            HttpContent content = new StringContent(jss.Serialize(MovieInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                int MovieId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("MovieDetails", new { id = MovieId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Gets the details of the movie including the director, Actors in the movie.
        /// </summary>
        /// <param name="MovieId">Id of the movie</param>
        /// <returns>
        ///     If successfull , returns a ViewModel which contains details of the Movie and the Actors in the movie
        ///     If not , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Movie/MovieDetails/15
        /// </example>
        [HttpGet]
        [Route("Movie/MovieDetails/{MovieId}")]
        public ActionResult MovieDetails(int Id)
        {
            UpdateMovie ViewModel = new UpdateMovie();

            string url = "MovieData/FindMovie/" + Id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                MovieDto Movie = response.Content.ReadAsAsync<MovieDto>().Result;
                ViewModel.Movie = Movie;

                url = "MovieData/GetDirectors";
                response = client.GetAsync(url).Result;
                IEnumerable<DirectorDto> Directors = response.Content.ReadAsAsync<IEnumerable<DirectorDto>>().Result;
                ViewModel.Directors = Directors;

                url = "MovieData/FindActorsinMovie/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<ActorDto> ActorsinMovie = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;
                ViewModel.ActorsinMovie = ActorsinMovie;

                url = "MovieData/GetPotentialActors/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<ActorDto> PotentialActors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;
                ViewModel.PotentialActors = PotentialActors;
               
                url = "MovieData/FindGenresofMovie/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<GenreDto> GenresofMovie = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
                ViewModel.GenresOfMovie = GenresofMovie;

                url = "MovieData/PotentialGenresofMovie/" + Id;
                response = client.GetAsync(url).Result;
                IEnumerable<GenreDto> PotentialGenres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
                ViewModel.PotentialGenresOfMovie = PotentialGenres;

                return View(ViewModel);

            }
            else
            {
                return RedirectToAction("Error");
            }

        }


        /// <summary>
        ///   Adds an actor to the list of actors for the movie
        /// </summary>
        /// <param name="ActorId">Id of the Actor which is to be added to the list</param>
        /// <param name="MovieId">Id of the Movie to be edited</param>
        /// <returns>
        ///     If successfully added , then the control is passed to the Action "MovieDetails" to update the details of the movie
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpPost]
        [Route("Movie/AddActorforMovie/{ActorId}/{MovieId}")]
        public ActionResult AddActorforMovie(int ActorId, int MovieId)
        {
            string url = "MovieData/AddActorforMovie/" + ActorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MovieDetails", new { id = MovieId});
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Removes an actor from  the list of actors in the Movie
        /// </summary>
        /// <param name="ActorId">Id of the Actor</param>
        /// <param name="MovieId">Id of the Movie</param>
        /// <returns>
        ///     If successfully removed , then the control is passed to the Action "MovieDetails" to update the details of the Movie
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpGet]
        [Route("Movie/RemoveActorforMovie/{ActorId}/{MovieId}")]
        public ActionResult RemoveActorforMovie(int ActorId, int MovieId)
        {
            string url = "MovieData/RemoveActorforMovie/" + ActorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MovieDetails", new { id = MovieId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Adds a genre to the list of genres for the movie
        /// </summary>
        /// <param name="GenreId">Id of the Genre which is to be added to the list</param>
        /// <param name="MovieId">Id of the Movie to be edited</param>
        /// <returns>
        ///     If successfully added , then the control is passed to the Action "MovieDetails" to update the details of the movie
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpPost]
        [Route("Movie/AddGenreforMovie/{GenreId}/{MovieId}")]
        public ActionResult AddGenreforMovie(int GenreId, int MovieId)
        {
            string url = "MovieData/AddGenreforMovie/" + GenreId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MovieDetails", new { id = MovieId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Removes a genre from the list of genres for the movie
        /// </summary>
        /// <param name="GenreId">Id of the Genre which is to be removed from the list</param>
        /// <param name="MovieId">Id of the Movie to be edited</param>
        /// <returns>
        ///     If successfully added , then the control is passed to the Action "MovieDetails" to update the details of the movie
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpGet]
        [Route("Movie/RemoveGenreforMovie/{GenreId}/{MovieId}")]
        public ActionResult RemoveGenreforMovie(int GenreId, int MovieId)
        {
            string url = "MovieData/RemoveGenreforMovie/" + GenreId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MovieDetails", new { id = MovieId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Gets the details of the Movie to be updated
        /// </summary>
        /// <param name="id">Id of the Movie</param>
        /// <returns>
        ///     If successfull , Returns the details of the movie to the updated to the view
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Movie/UpdateMovie/5
        /// </example>

        [HttpGet]
        public ActionResult UpdateMovie(int id)
        {
            ShowMovie ViewModel = new ShowMovie();
            string url = "MovieData/FindMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                MovieDto Movie = response.Content.ReadAsAsync<MovieDto>().Result;
                ViewModel.Movie = Movie;

                
                url = "MovieData/GetDirectors";
                response = client.GetAsync(url).Result;
                IEnumerable<DirectorDto> Directors = response.Content.ReadAsAsync<IEnumerable<DirectorDto>>().Result;
                ViewModel.Directors = Directors;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the basic information of the Movie in the database ( Without the actors )
        /// </summary>
        /// <param name="id">Id of the Movie to be updated</param>
        /// <param name="MovieInfo">Form data which contains the new information of the Movie to be updated</param>
        /// <returns>
        ///     If successfully updated the information in the database , then the action is redirected to the Action "MovieDetails"
        ///     If not successfull, then an error message is thrown. 
        /// </returns>
        /// <example>
        /// POST: Movie/UpdateMovie/5
        /// </example>

        [HttpPost]
        public ActionResult UpdateMovie(int id, Movie MovieInfo, HttpPostedFileBase MoviePic)
        {
            string url = "MovieData/UpdateMovie/" + id;
            HttpContent content = new StringContent(jss.Serialize(MovieInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                //only attempt to send Movie picture data if we have it
                if (MoviePic != null)
                {
                    //Send over image data for movie
                    url = "MovieData/UpdateMoviePic/" + id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(MoviePic.InputStream);
                    requestcontent.Add(imagecontent, "MoviePic", MoviePic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                return RedirectToAction("MovieDetails", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Asks the user to confirm before deleting the Movie from the database permanently. Shows the information about the movie   
        /// </summary>
        /// <param name="id">Id of the movie to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the movie to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Movie/DeleteConfirm/5
        /// </example>

        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "MovieData/FindMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                MovieDto SelectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;
                return View(SelectedMovie);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///     Deletes the movie from the Database.
        /// </summary>
        /// <param name="id">Id of the movie to be deleted.</param>
        /// <returns>
        ///     If successfully deleted the user, then redirects the control to the action "ListMovies" to list all the movies
        ///     If not , returns an error.
        /// </returns>
        /// <example>
        ///     POST: Movie/DeleteMovie/5
        /// </example>

        [HttpPost]
        public ActionResult DeleteMovie(int id)
        {
            string url = "MovieData/DeleteMovie/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListMovies");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        // GET: Movie
        public ActionResult Index()
        {
            return View();
        }

        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        


        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Movie/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Movie/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Movie/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
