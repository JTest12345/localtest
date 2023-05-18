using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pmms_test.Models;

namespace pmms_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100"))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = "SELECT TOP 1 empname FROM TmEmployee WHERE empcode = 1";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    return Content(Result);
                }
            }
            catch (Exception ex)
            {
                return Content("SQL Fail");
            }



            //return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
