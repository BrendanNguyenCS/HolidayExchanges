using HolidayExchanges.Models;
using HolidayExchanges.Services;
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
        private SecretSantaManager santaMgr;

        public UserController()
        {
            santaMgr = new SecretSantaManager(db);
        }

        // TODO: Ask jon about refactoring authorization code so that it can also do the redirecting to login page (by returning an ActionResult)

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
                Groups = santaMgr.GetAllGroupsForUser(num)
            };
            model.Recipients = santaMgr.GetRecipientsForAllGroupsForUser(num, model.Groups);
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

        // GET: User/Edit/5
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
            return View(user);
        }

        // POST: User/Edit/5 To protect from overposting attacks, enable the specific properties you
        // want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,Password,Salt,FirstName,LastName,Address1,Address2,City,State,Zip,Country,Email,Birthday,PhoneNumber")] User user)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                ResetRedirectLink();

                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Property(u => u.UserName).IsModified = false;
                db.Entry(user).Property(u => u.Password).IsModified = false;
                db.Entry(user).Property(u => u.Salt).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = user.UserID });
            }

            #region Error Checking Comments

            /*var errors = ModelState
             .Where(x => x.Value.Errors.Count > 0)
             .Select(x => new { x.Key, x.Value.Errors })
             .ToArray();
             System.Console.WriteLine(errors.ToString());
            */

            #endregion Error Checking Comments

            return View(user);
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

        // GET: User/Wishlist/bnuge
        [ActionName("WishlistByUsername")]
        public ActionResult Wishlist(string username)
        {
            // will not redirect back to wishlist page after completion of login
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Login");
            int id = db.Users.Single(u => u.UserName == username).UserID;
            return RedirectToAction("Wishlist", new { id });
        }

        [HttpGet, ChildActionOnly]
        public ActionResult WishlistSearch()
        {
            WishlistSearchVM model = new WishlistSearchVM();
            return PartialView("_WishlistSearch", model);
        }

        [HttpPost]
        public ActionResult WishlistSearch(WishlistSearchVM model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.SearchUsername) && string.IsNullOrEmpty(model.SearchEmail))
                    return PartialView(model);

                User userFound;

                if (!string.IsNullOrEmpty(model.SearchUsername))
                {
                    userFound = db.Users.SingleOrDefault(u => u.UserName == model.SearchUsername);

                    if (userFound == null) // if a user doesn't exist
                    {
                        ModelState.AddModelError("SearchUsername", "This username doesn't exist.");
                        return PartialView(model);
                    }

                    // a matching user was found
                    return RedirectToAction("Wishlist", "User", new { id = userFound.UserID });
                }

                // the email search field must be filled at this point
                userFound = db.Users.SingleOrDefault(u => u.Email == model.SearchEmail);

                if (userFound == null)
                {
                    ModelState.AddModelError("SearchEmail", "This email doesn't exist.");
                    return PartialView(model);
                }

                return RedirectToAction("Wishlist", "User", new { id = userFound.UserID });
            }

            return PartialView(model);
        }

        #region AJAX/JSON version

        /*[HttpPost]
        public ActionResult WishlistSearch(string searchTerm)
        {
        }*/

        #endregion AJAX/JSON version

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
                Groups = santaMgr.GetAllGroupsForUser(id.Value)
            };
            return View(model);
        }
    }
}