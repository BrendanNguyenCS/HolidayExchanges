using HolidayExchanges.DAL;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SecretSantaDbContext db = new SecretSantaDbContext();

        [NonAction]
        public void ResetRedirectLink()
        {
            Session["UserName"] = null;
        }

        [NonAction]
        public bool IsOwnerOfPage(int? id)
        {
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            var pageOwner = db.Users.Find(id);
            if (pageOwner == null) return false;
            var currentUser = db.Users.Where(u => u.UserName == username).Single();
            if (pageOwner.UserID != currentUser.UserID) return false;
            return true;
        }

        [NonAction]
        public string GetCurrentSessionUserName()
        {
            return Session["UserName"] != null ? Session["UserName"].ToString() : String.Empty;
        }

        public ActionResult IsLoggedIn(string currentActionMethod, string currentController, int? routeValue = 0)
        {
            var username = GetCurrentSessionUserName();

            // if user isn't logged in yet
            if (string.IsNullOrEmpty(username))
            {
                if (routeValue == 0)
                {
                    Session["RedirectLink"] = Url.Action(currentActionMethod, currentController, routeValue);
                }
                else
                {
                    Session["RedirectLink"] = Url.Action(currentActionMethod, currentController);
                }
                return RedirectToAction("Login", "Login");
            }

            return new EmptyResult();
        }

        public ActionResult IsCurrentUserOwnerOfPage(int? id)
        {
            var username = GetCurrentSessionUserName();
            var pageOwner = db.Users.Find(id);

            if (pageOwner == null)
            {
                ViewBag.ErrorMessage = "Something went wrong. Sorry!";
                return RedirectToAction("Error", "Home");
            }

            var currentUser = db.Users.Where(u => u.UserName == username).Single();

            if (!IsOwnerOfPage(id))
            {
                ViewBag.NotAuthorizedMessage = "You aren't authorized to enter the previous page. You have been redirected to this page. If this is in error, please contact the developer.";
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            return new EmptyResult();
        }
    }
}