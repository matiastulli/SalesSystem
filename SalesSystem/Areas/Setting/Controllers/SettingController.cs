using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesSystem.Areas.Setting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Setting.Controllers
{
    [Authorize]
    [Area("Setting")]
    public class SettingController : Controller
    {
        public IActionResult Setting()
        {
            var model = new InputModelSetting();
            return View(model);
        }
    }
}
