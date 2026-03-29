using Microsoft.AspNetCore.Mvc;
using SecurityBase.Mvc.Models;
using System.Diagnostics;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        ViewData["ActivePage"] = "Dashboard";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
