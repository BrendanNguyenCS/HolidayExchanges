using HolidayExchanges.DAL;
using HolidayExchanges.Models;
using HolidayExchanges.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class WishController : Controller
    {
        private readonly SecretSantaDbContext _context = new SecretSantaDbContext();

        // GET: Wish
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Login");
            var user = _context.Users.Where(u => u.UserName == username).Single();
            var model = new WishViewModel(user.UserID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WishViewModel model)
        {
            if (ModelState.IsValid)
            {
                var wish = new Wish
                {
                    ItemName = model.ItemName,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    ItemLink = model.ItemLink,
                    PurchasingInstructions = model.PurchasingInstructions,
                    HasBeenBought = model.HasBeenBought
                };

                _context.Wishes.Add(wish);
                _context.SaveChanges();
                return RedirectToAction("Details", "User");
            }

            return View(model);
        }
    }
}