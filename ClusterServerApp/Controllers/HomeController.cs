using ClusterServerApp.Models;
using GraphAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClusterServerApp.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult CalculateMatrix(string guid)
        {
            try
            {
                SetProcessGuid(guid);

                const int SIZE = 5000;

                var matrix = generateMatrix(SIZE);

                var algorithm = new HungarianAlgorithm(matrix);

                Thread.Sleep(1000);

                var result = algorithm.Run(guid, Request.Cookies["access_token"].Value, Request);

                if(result == null)
                {
                    throw new Exception("Request was canceled");
                }


                StringBuilder builder = new StringBuilder();

                DeleteProcessGuid(guid);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Register()
        {
            RegisterBindingModel model = new RegisterBindingModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterBindingModel model)
        {
            if (ModelState.IsValid)
            {

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/'));

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage msg = await client.PostAsJsonAsync("/api/Account/Register", model);


                    if (msg.IsSuccessStatusCode)
                    {
                        return RedirectToAction("SignIn", "Home");
                    }
                    else
                    {
                        List<string> res = msg.Content.ReadAsAsync<List<string>>().Result;
                        ViewData["errors"] = res;
                    }
                }
            }

            return View(model);
        }

        public ActionResult SignIn()
        {
           UserLoginViewModel model = new UserLoginViewModel();

            return View(model);
        }


        [Authorize]
        public ActionResult AdminPage()
        {
            return View();
        }

        #region Helper Methods

        static int[,] generateMatrix(int size)
        {
            var matrix = new int[size, size];

            var rnd = new Random();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = rnd.Next() % size;

            return matrix;
        }

        private void SetProcessGuid(string _guid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/'));
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["access_token"].Value);

                SetGuidParams param = new SetGuidParams()
                {
                    ServerUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/'),
                    ProcessGuid = _guid
                };

                HttpResponseMessage msg = client.PostAsJsonAsync("/api/Db/SetProcessGuid", param).Result;
            }
        }

        private void DeleteProcessGuid(string _guid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/'));
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["access_token"].Value);

                HttpResponseMessage msg = client.GetAsync("/api/Db/DeleteProcessGuid?guid=" + _guid).Result;
            }
        }

        #endregion
    }
}
