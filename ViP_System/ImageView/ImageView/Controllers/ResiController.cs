using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageView.Models;
using Microsoft.AspNetCore.Authorization;

namespace ImageView.Controllers
{
    public class ResiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public class ImagesController : Controller
        {
            [HttpGet]
            public ActionResult Show(long id)
            {
                var image = new ImageModel(id).Show();
                return new FileContentResult(image, "image/jpeg");
            }

            [HttpPost]
            public string Create()
            {
                var id = new ImageModel().Create();
                return id.ToString();
            }
        }
    }
}
