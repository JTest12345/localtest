using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMMS_FW.Controllers
{
    public class AddLotInputController : Controller
    {
        // GET: AddLotInput
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InputName()
        {
            return View();
        }
    }
}