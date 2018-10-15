using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmtionPlatzi.Web.Models;

namespace EmtionPlatzi.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.WelcomeMessage = "Hola Omar Cervantes";
            ViewBag.ValorEntero = 1;
            return View();
        }

        public ActionResult IndexAlt()
        {
            var modelo = new Home();
            modelo.WelcomeMessage = "Hola Omar Cervantes desde el modelo home";
            return View(modelo);
        }
    }
}