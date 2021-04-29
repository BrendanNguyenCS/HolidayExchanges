using HolidayExchanges.Models;
using HolidayExchanges.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class UserController : BaseController
    {
        #region Details View Initializers

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!IsLoggedIn("Details", "User", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", new { id = currentUser.UserID });
            }

            ResetRedirectLink();

            User user = db.Users.Find(id);
            int num = id.Value;
            if (user == null)
                return HttpNotFound();
            UserViewModel model = new UserViewModel
            {
                User = user,
                Groups = _santaMgr.GetAllGroupsForUser(num)
            };
            model.Recipients = _santaMgr.GetRecipientsForAllGroupsForUser(num, model.Groups);
            return View(model);
        }

        // GET: User/Details/bnuge
        [ActionName("DetailsByUsername")]
        public ActionResult Details(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Login");
            int id = db.Users.Single(u => u.UserName == username).UserID;
            return RedirectToAction("Details", new { id });
        }

        #endregion Details View Initializers

        // GET: User/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!IsLoggedIn("Edit", "User", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            UserEditViewModel model = new UserEditViewModel
            {
                ID = user.UserID,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                State = user.State,
                Zip = user.Zip,
                Country = user.Country,
                Email = user.Email,
                // below will be a hidden field
                OriginalEmail = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                ResetRedirectLink();

                var user = db.Users.Find(model.ID);
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Property(u => u.UserName).IsModified = false;
                db.Entry(user).Property(u => u.Password).IsModified = false;
                db.Entry(user).Property(u => u.Salt).IsModified = false;

                #region VM to model assignments

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address1 = model.Address1;
                user.Address2 = model.Address2;
                user.City = model.City;
                user.State = model.State;
                user.Country = model.Country;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Birthday = model.Birthday;

                #endregion VM to model assignments

                db.SaveChanges();
                return RedirectToAction("Details", new { id = user.UserID });
            }

            return View(model);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //checks current login status
            if (!IsLoggedIn("Delete", "User", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            User user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResetRedirectLink();

            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        #region Wishlist View Initializers

        // GET: User/Wishlist/1
        [HttpGet]
        public ActionResult Wishlist(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User user = db.Users.Find(id);
            bool hasWishes = db.Wishes.Any(w => w.UserID == user.UserID);
            WishlistViewModel model;
            if (hasWishes)
            {
                model = new WishlistViewModel
                {
                    UserID = id.Value,
                    UserName = user.UserName,
                    Wishlist = user.Wishes.ToList()
                };
            }
            else
            {
                model = new WishlistViewModel
                {
                    UserID = id.Value,
                    UserName = user.UserName,
                    Wishlist = new List<Wish>()
                };
            }

            return View(model);
        }

        // GET: User/Wishlist/bnuge (this is only for the navbar wishlist button)
        [ActionName("WishlistByUsername")]
        public ActionResult Wishlist(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Login");
            int id = db.Users.Single(u => u.UserName == username).UserID;
            return RedirectToAction("Wishlist", new { id });
        }

        #endregion Wishlist View Initializers

        #region AJAX Call API Endpoints

        [HttpGet]
        public JsonResult WishlistSearch(string searchCriteria)
        {
            if (string.IsNullOrEmpty(searchCriteria))
                return Json(new { success = false, ex = "The search is empty." }, JsonRequestBehavior.AllowGet);

            var user = db.Users.SingleOrDefault(u => (u.UserName == searchCriteria) || (u.Email == searchCriteria));

            if (user != null)
                return Json(new { success = true, redirectUrl = Url.Action("GetUserWishlist", "User", searchCriteria) }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false, ex = "A user was not found" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUserWishlist(string searchCriteria)
        {
            var user = db.Users.SingleOrDefault(u => (u.UserName == searchCriteria) || (u.Email == searchCriteria));

            // no need to check if it is null since AJAX API endpoint already did the work
            return RedirectToAction("Wishlist", "User", new { id = user.UserID });
        }

        #endregion AJAX Call API Endpoints

        // GET: User/Grouplist/1
        public ActionResult Grouplist(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!IsLoggedIn("Details", "User", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            // check not needed because if already null, it won't change anything
            ResetRedirectLink();

            User user = db.Users.Find(id);
            GrouplistViewModel model = new GrouplistViewModel
            {
                UserID = id.Value,
                UserName = user.UserName,
                Groups = _santaMgr.GetAllGroupsForUser(id.Value)
            };
            return View(model);
        }

        /// <summary>
        /// A combiner helper function for <see cref="BaseController.IsLoggedIn(string, string,
        /// int?)"/> and <see cref="BaseController.IsOwnerOfPage(int?)"/>
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="currentController">The current controller ("User").</param>
        /// <param name="currentAction">The current action.</param>
        /// <returns></returns>
        protected ActionResult IsAuthorized(int? id, string currentController, string currentAction)
        {
            if (!IsLoggedIn(currentController, currentAction, id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            return new EmptyResult();
        }
    }
}