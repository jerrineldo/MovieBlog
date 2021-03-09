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
    public class DirectorController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static DirectorController()
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
        /// Returns a List of all the directors in the database to the view
        /// </summary>
        /// <returns>
        ///     If successfully retrieved data from the tables , then returns the data to the view
        ///     If not -> Returns an error message to the Error View
        /// </returns>
        /// <example>
        /// GET: Director/ListDirectors
        /// </example>
        public ActionResult ListDirectors()
        {
            string url = "DirectorData/ListDirectors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DirectorDto> ListOfDirectors = response.Content.ReadAsAsync<IEnumerable<DirectorDto>>().Result;
                return View(ListOfDirectors);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        /// <summary>
        ///  Renders the view for creating a new Director 
        /// </summary>
        /// <returns>Returns a view for creating a new Director</returns>
        /// <example>
        /// GET : Director/Create
        /// </example>

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new Director in the database
        /// </summary>
        /// <param name="DirectorInfo">Form data which has the details for the new Director to be entered into the database</param>
        /// <returns>
        ///     If successfull , A new Director would be entered into the database.
        ///     If not , then an error would be thrown 
        /// </returns>
        /// <example>
        /// POST: Director/Create
        /// </example>

        [HttpPost]
        public ActionResult Create(Director DirectorInfo)
        {
            string url = "DirectorData/AddDirector";
            HttpContent content = new StringContent(jss.Serialize(DirectorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                int DirectorId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("ListDirectors");
                
                //return RedirectToAction("DirectorDetails", new { id = DirectorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Gets the details of the Director to be updated
        /// </summary>
        /// <param name="id">Id of the Director</param>
        /// <returns>
        ///     If successfull , Returns the details of the director to the updated to the view
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Director/UpdateDirector/5
        /// </example>

        [HttpGet]
        public ActionResult UpdateDirector(int id)
        {
            string url = "DirectorData/FindDirector/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                Director SelectedDirector = response.Content.ReadAsAsync<Director>().Result;
                return View(SelectedDirector);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Gets the details of the Director to be updated
        /// </summary>
        /// <param name="id">Id of the Director</param>
        /// <returns>
        ///     If successfull , Returns the details of the director to the updated to the view
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Director/Update/5
        /// </example>

        [HttpPost]
        public ActionResult UpdateDirector(int id, Director DirectorInfo, HttpPostedFileBase DirectorPic)
        {
            string url = "DirectorData/UpdateDirector/" + id;
            HttpContent content = new StringContent(jss.Serialize(DirectorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                //only attempt to send director picture data if we have it
                if (DirectorPic != null)
                {
                    //Send over image data for player
                    url = "DirectorData/UpdateDirectorPic/" + id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(DirectorPic.InputStream);
                    requestcontent.Add(imagecontent, "ActorPic", DirectorPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                return RedirectToAction("ListDirectors");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Gets the details of the Director to be updated, including the movies of the director
        /// </summary>
        /// <param name="id">Id of the Director</param>
        /// <returns>
        ///     If successfull , returns a ViewModel which contains details of the Director and the movies of the director.
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Director/DirectorDetails/5
        /// </example>

        [HttpGet]
        public ActionResult DirectorDetails(int id)
        {
            ShowDirector ViewModel = new ShowDirector();
            string url = "DirectorData/FindDirector/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                DirectorDto Director = response.Content.ReadAsAsync<DirectorDto>().Result;
                ViewModel.Director = Director;


                url = "DirectorData/MoviesofDirector/"+ id;
                response = client.GetAsync(url).Result;
                IEnumerable<MovieDto> Movies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
                ViewModel.MoviesDirected = Movies;

                url = "DirectorData/PotentialMoviesofDirector/" + id;
                response = client.GetAsync(url).Result;
                IEnumerable<MovieDto> PotentialMovies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
                ViewModel.PotentialMovies = PotentialMovies;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        /// <summary>
        ///   Adds a movie to the list of movies directed by the Director
        /// </summary>
        /// <param name="DirectorId">Id of the Director</param>
        /// <param name="MovieId">Id of the Movie which is to be added to the list</param>
        /// <returns>
        ///     If successfully added , then the control is passed to the Action "DirectorDetails" to view the details of the Director
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpPost]
        [Route("api/DirectorData/AddMovieforDirector/{DirectorId}/{MovieId}")]
        public ActionResult AddMovieForDirector(int DirectorId, int MovieId)
        {
            string url = "DirectorData/AddMovieforDirector/" + DirectorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("DirectorDetails", new { id = DirectorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Removes a movie from  the list of movies directed by the director
        /// </summary>
        /// <param name="DirectorId">Id of the Director</param>
        /// <param name="MovieId">Id of the Movie which is to be added to the list</param>
        /// <returns>
        ///     If successfully removed , then the control is passed to the Action "DirectorDetails" to update the details of the Director
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpGet]
        [Route("Director/RemoveMovieforDirector/{DirectorId}/{MovieId}")]
        public ActionResult RemoveMovieforDirector(int DirectorId, int MovieId)
        {
            string url = "DirectorData/RemoveMovieforDirector/" + DirectorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("DirectorDetails", new { id = DirectorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Asks the user to confirm before deleting the Director from the database permanently. Shows the information about the Director   
        /// </summary>
        /// <param name="id">Id of the director to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the Director to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Director/DeleteConfirm/5
        /// </example>

        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "DirectorData/FindDirector/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                DirectorDto SelectedDirector = response.Content.ReadAsAsync<DirectorDto>().Result;
                return View(SelectedDirector);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        /// <summary>
        ///     Deletes the Director from the Database.
        /// </summary>
        /// <param name="id">Id of the Director to be deleted.</param>
        /// <returns>
        ///     If successfully deleted the user, then redirects the control to the action "ListDirectors" to list all the directors
        ///     If not , returns an error.
        /// </returns>
        /// <example>
        ///     POST: Director/Delete/5
        /// </example>

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DirectorData/DeleteDirector/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListDirectors");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}