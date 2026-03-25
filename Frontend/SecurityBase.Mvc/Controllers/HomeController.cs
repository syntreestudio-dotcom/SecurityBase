using Microsoft.AspNetCore.Mvc;

namespace SecurityBase.Mvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        ViewData["ActivePage"] = "Dashboard";
        return View();
    }
}
