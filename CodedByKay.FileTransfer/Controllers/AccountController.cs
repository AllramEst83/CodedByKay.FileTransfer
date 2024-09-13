using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CodedByKay.FileTransfer.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "password") // Simple authentication
            {
                HttpContext.Session.SetString("IsAuthenticated", "true");
                return RedirectToAction("Upload", "File");
            }
            else
            {
                ViewBag.Message = "Invalid credentials";
                return View();
            }
        }
    }
}
