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
                return View(ViewModel);
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
    }
}
