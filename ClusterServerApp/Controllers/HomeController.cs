using GraphAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ClusterServerApp.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CalculateMatrix()
        {
            const int SIZE = 5000;

            var matrix = generateMatrix(SIZE);

            var algorithm = new HungarianAlgorithm(matrix);

            Thread.Sleep(1000);

            var result = algorithm.Run();

            return Json(result, JsonRequestBehavior.AllowGet);
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
        
        #endregion
    }
}
