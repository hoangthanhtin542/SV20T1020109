using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SV20T1020109.Web.WebSecurityModels;

namespace SV20T1020109.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id = 0)
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            return View();
        }
        public IActionResult Shipping(int id = 0)
        {
            return View();
        }

    }
}
