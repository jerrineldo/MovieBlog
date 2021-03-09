using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MovieBlog.Models;
using MovieBlog.Models.ViewModels;
using System.Net.Http.Headers; 
using System.Web.Script.Serialization;

namespace MovieBlog.Controllers
{
    public class GenreController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static GenreController()
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
        /// Returns a List of all the genres in the database to the view
        /// </summary>
        /// <returns>
        ///     If successfully retrieved data from the tables , then returns the data to the view
        ///     If not -> Returns an error message to the Error View
        /// </returns>
        /// <example>
        /// GET: Genre/ListGenres
        /// </example>

        public ActionResult ListGenres()
        {
            string url = "GenreData/ListGenres";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<GenreDto> ListOfGenres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
                return View(ListOfGenres);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }


        /// <summary>
        ///  Renders the view for creating a new Genre
        /// </summary>
        /// <returns>Returns a view for creating a new Genre</returns>
        /// <example>
        /// GET : Genre/Create
        /// </example>
        
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new Genre in the database
        /// </summary>
        /// <param name="GenreInfo">Form data which has the details for the new Genre to be entered into the database</param>
        /// <returns>
        ///     If successfull , A new Genre would be entered into the database.
        ///     If not , then an error would be thrown 
        /// </returns>
        /// <example>
        /// POST: Genre/Create
        /// </example>

        [HttpPost]
        public ActionResult Create(Genre GenreInfo)
        {
            string url = "GenreData/AddGenre";
            HttpContent content = new StringContent(jss.Serialize(GenreInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGenres");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the details of the Genre whose Id is passed
        /// </summary>
        /// <param name="id">Id of the Genre</param>
        /// <returns>
        ///     If successfull , Returns the details of the Genre to the view
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Genre/UpdateGenre/5
        /// </example>

        [HttpGet]
        public ActionResult UpdateGenre(int id)
        {
            //ShowActor ViewModel = new ShowActor();
            string url = "GenreData/FindGenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                GenreDto Genre = response.Content.ReadAsAsync<GenreDto>().Result;
                return View(Genre);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the basic information of the Genre in the database ( Without the movies in the Genre )
        /// </summary>
        /// <param name="id">Id of the Genre to be updated</param>
        /// <param name="GenreInfo">Form data which contains the new information of the Genre to be updated</param>
        /// <returns>
        ///     If successfully updated the information in the database , then the action is redirected to the Action "ListGenre"
        ///     If not successfull, then an error message is thrown. 
        /// </returns>
        /// <example>
        /// POST: Genre/UpdateGenre/5
        /// </example>

        [HttpPost]
        public ActionResult UpdateGenre(int id, Genre GenreInfo)
        {
            string url = "GenreData/UpdateGenre/" + id;
            HttpContent content = new StringContent(jss.Serialize(GenreInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGenres", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        /// <summary>
        /// Asks the user to confirm before deleting the Genre from the database permanently. Shows the information about the Genre   
        /// </summary>
        /// <param name="id">Id of the genre to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the genre to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Genre/DeleteConfirm/5
        /// </example>

        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "GenreData/FindGenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                GenreDto SelectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
                return View(SelectedGenre);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///     Deletes the Genre from the Database.
        /// </summary>
        /// <param name="id">Id of the genre to be deleted.</param>
        /// <returns>
        ///     If successfully deleted the user, then redirects the control to the action "ListGenres" to list all the genres
        ///     If not , returns an error.
        /// </returns>
        /// <example>
        ///     POST: Genre/DeleteGenre/5
        /// </example>

        [HttpPost]
        public ActionResult DeleteGenre(int id)
        {
            string url = "GenreData/DeleteGenre/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGenres");
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