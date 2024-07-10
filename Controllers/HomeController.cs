using CheckListJob.Models;
using CheckListJob.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace CheckListJob.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public IActionResult WelcomeIndex()
        {
            return View();
        }

        public IActionResult ErrorIndex()
        {
            return View();
        }

    }
}
