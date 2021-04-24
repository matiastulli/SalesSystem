using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesSystem.Data;
using SalesSystem.Library;

namespace SalesSystem.Areas.Principal.Controllers
{
    [Area("Principal")]
    public class PrincipalController : Controller
    {

        private LSetting _setting;

        public PrincipalController(
            ApplicationDbContext context)
        {
            _setting = new LSetting(context);
        }

        public IActionResult Principal()
        {
            return View();
        }
    }
}