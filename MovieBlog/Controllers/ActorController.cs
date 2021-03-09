using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;   //HttpResponseMessage
using MovieBlog.Models;
using MovieBlog.Models.ViewModels;
using System.Net.Http.Headers; //MediaTypeWithQualityHeaderValue
using System.Web.Script.Serialization; //JavaScriptSerializer

namespace MovieBlog.Controllers
{
    public class ActorController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static ActorController()
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
        /// Returns a List of all the actors in the database to the view
        /// </summary>
        /// <returns>
        ///     If successfully retrieved data from the tables , then returns the data to the view
        ///     If not -> Returns an error message to the Error View
        /// </returns>
        /// <example>
        /// GET: Actor/ListActors
        /// </example>

        public ActionResult ListActors()
        {
            string url = "ActorData/ListActors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ActorDto> ListOfActors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;
                return View(ListOfActors);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// Gets the details of the Actor whose Id is passed
        /// </summary>
        /// <param name="id">Id of the Actor</param>
        /// <returns>
        ///     If successfull , Returns the details of the actor to the view
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///     GET: Actor/ActorDetails/5
        /// </example>

        [HttpGet]
        public ActionResult ActorDetails(int id)
        {
            //ShowActor ViewModel = new ShowActor();
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                ActorDto Actor = response.Content.ReadAsAsync<ActorDto>().Result;
                return View(Actor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Adds a movie to the list of movies acted by the Actor
        /// </summary>
        /// <param name="ActorId">Id of the Actor</param>
        /// <param name="MovieId">Id of the Movie which is to be added to the list</param>
        /// <returns>
        ///     If successfully added , then the control is passed to the Action "UpdateActor" to update the details of the Actor
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpPost]
        [Route("Actor/AddMovieforActor/{ActorId}/{MovieId}")]
        public ActionResult AddMovieforActor(int ActorId, int MovieId)
        {
            string url = "ActorData/AddMovieforActor/" + ActorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UpdateActor", new { id = ActorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///   Removes a movie from  the list of movies acted by the Actor
        /// </summary>
        /// <param name="ActorId">Id of the Actor</param>
        /// <param name="MovieId">Id of the Movie which is to be added to the list</param>
        /// <returns>
        ///     If successfully removed , then the control is passed to the Action "UpdateActor" to update the details of the Actor
        ///     If not successfull , then returns an error
        /// </returns>

        [HttpGet]
        [Route("Actor/RemoveMovieforActor/{ActorId}/{MovieId}")]
        public ActionResult RemoveMovieforActor(int ActorId, int MovieId)
        {
            string url = "ActorData/RemoveMovieforActor/" + ActorId + "/" + MovieId;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UpdateActor", new { id = ActorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///  Renders the view for creating a new Actor
        /// </summary>
        /// <returns>Returns a view for creating a new Actor</returns>
        /// <example>
        /// GET : Actor/Create
        /// </example>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new Actor in the database
        /// </summary>
        /// <param name="ActorInfo">Form data which has the details for the new Actor to be entered into the database</param>
        /// <returns>
        ///     If successfull , A new Actor would be entered into the database.
        ///     If not , then an error would be thrown 
        /// </returns>
        /// <example>
        /// POST: Actor/Create
        /// </example>
        
        [HttpPost]
        public ActionResult Create(Actor ActorInfo)
        {
            string url = "ActorData/AddActor";
            HttpContent content = new StringContent(jss.Serialize(ActorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                int ActorId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("ActorDetails", new { id = ActorId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Updates the movies of the Actor
        /// </summary>
        /// <param name="id">Id of the Actor to be updated</param>
        /// <returns>
        ///     If successfull , returns a ViewModel which contains details about the Actor and the movies he acted and the potential movies
        ///     he could be a part of.
        ///     If not successfull , returns an error.
        /// </returns>
        /// <example>
        ///      GET: Actor/UpdateActor/15
        /// </example>

        [HttpGet]
        public ActionResult UpdateActor(int id)
        {
            ShowActor ViewModel = new ShowActor();
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                ActorDto Actor = response.Content.ReadAsAsync<ActorDto>().Result;
                ViewModel.Actor = Actor;

                url = "ActorData/GetMoviesofActor/" + id;
                response = client.GetAsync(url).Result;
                IEnumerable<MovieDto> MoviesActed = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
                ViewModel.MoviesActed = MoviesActed;


                url = "ActorData/GetPotentialMovies/" + id;
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
        /// Updates the basic information of the Actor in the database ( Without the movies he acted )
        /// </summary>
        /// <param name="id">Id of the Actor to be updated</param>
        /// <param name="ActorInfo">Form data which contains the new information of the Actor to be updated</param>
        /// <returns>
        ///     If successfully updated the information in the database , then the action is redirected to the Action "UpdateActor"
        ///     If not successfull, then an error message is thrown. 
        /// </returns>
        /// <example>
        /// POST: Actor/UpdateActor/5
        /// </example>

        [HttpPost]
        public ActionResult UpdateActor(int id, Actor ActorInfo, HttpPostedFileBase ActorPic)
        {
            string url = "ActorData/UpdateActor/" + id;
            HttpContent content = new StringContent(jss.Serialize(ActorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                //only attempt to send player picture data if we have it
                if (ActorPic != null)
                {
                    //Send over image data for player
                    url = "ActorData/UpdateActorPic/" + id;
                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(ActorPic.InputStream);
                    requestcontent.Add(imagecontent, "ActorPic", ActorPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }
                return RedirectToAction("UpdateActor", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Asks the user to confirm before deleting the Actor from the database permanently. Shows the information about the actor   
        /// </summary>
        /// <param name="id">Id of the actor to be deleted.</param>
        /// <returns>
        ///     If successfull, renders the details of the Actor to be deleted to the corresponding View.
        ///     If not successfull, then returns an error.
        /// </returns>
        /// <example>
        ///     GET: Actor/DeleteConfirm/5
        /// </example>
        
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                ActorDto SelectedActor = response.Content.ReadAsAsync<ActorDto>().Result;
                return View(SelectedActor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        ///     Deletes the actor from the Database.
        /// </summary>
        /// <param name="id">Id of the Actor to be deleted.</param>
        /// <returns>
        ///     If successfully deleted the user, then redirects the control to the action "ListActors" to list all the actors
        ///     If not , returns an error.
        /// </returns>
        /// <example>
        ///     POST: Actor/Delete/5
        /// </example>
       
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "ActorData/DeleteActor/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListActors");
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
