using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckListJob.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public IActionResult WelcomeIndex()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult ErrorIndex()
        {
            return View();
        }

    }
}
