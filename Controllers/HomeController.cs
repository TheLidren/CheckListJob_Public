using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckListJob.Controllers;
[Authorize]
public class HomeController : Controller
{

    public IActionResult WelcomeIndex() => View();

    public IActionResult AccessDenied() => View();
    public IActionResult ErrorIndex() => View();

}
