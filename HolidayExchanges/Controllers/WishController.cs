using HolidayExchanges.Models;
using HolidayExchanges.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class WishController : BaseController
    {
        // GET: Wish/Details/1
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Wish wish = db.Wishes.Find(id);
            if (wish == null)
                return HttpNotFound();

            var model = new WishDetailsVM
            {
                UserName = wish.User.UserName,
                WishID = id.Value,
                UserID = wish.UserID,
                ItemName = wish.ItemName,
                Description = wish.Description,
                Quantity = wish.Quantity,
                ItemLink = wish.ItemLink,
                PurchasingInstructions = wish.PurchasingInstructions,
                HasBeenBought = wish.HasBeenBought
            };
            model.PageOwner = IsOwnerOfPage(id);

            return View(model);
        }

        // GET: Wish/Create
        [HttpGet]
        public ActionResult Create()
        {
            // check for log in status
            if (!IsLoggedIn("Create", "Wish"))
                return RedirectToAction("Login", "Login");
            var username = Session["UserName"].ToString();
            var user = db.Users.Single(u => u.UserName == username);
            var model = new WishViewModel(user.UserID);
            return View(model);
        }

        // POST: Wish/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(WishViewModel model)
        {
            if (ModelState.IsValid)
            {
                ResetRedirectLink();

                var wish = new Wish
                {
                    UserID = model.UserId,
                    ItemName = model.ItemName,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    ItemLink = model.ItemLink,
                    PurchasingInstructions = model.PurchasingInstructions,
                    HasBeenBought = model.HasBeenBought
                };

                db.Wishes.Add(wish);
                db.SaveChanges();
                return RedirectToAction("Details", "User", new { id = model.UserId });
            }

            return View(model);
        }

        // GET: Wish/Edit/1
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // check for log in status
            if (!IsLoggedIn("Edit", "Wish", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);

            var username = Session["UserName"].ToString();
            var selectedWish = db.Wishes
                .Single(w => w.WishID == id && w.User.UserName == username);

            return View(selectedWish);
        }

        // POST: Wish/Edit/1
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WishID,UserID,ItemName,Description,Quantity,ItemLink,PurchasingInstructions,HasBeenBought")] Wish model)
        {
            if (ModelState.IsValid)
            {
                ResetRedirectLink();

                db.Entry(model).State = EntityState.Modified;
                db.Entry(model).Property(m => m.UserID).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.WishID });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult NewEdit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!IsLoggedIn("Edit", "Wish", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);

            var selectedWish = db.Wishes
                .Single(w => w.WishID == id && w.User.UserName == GetCurrentUsername());

            var model = new WishEditVM
            {
                WishID = id.Value,
                ItemName = selectedWish.ItemName,
                Description = selectedWish.Description,
                Quantity = selectedWish.Quantity,
                ItemLink = selectedWish.ItemLink,
                PurchasingInstructions = selectedWish.PurchasingInstructions,
                HasBeenBought = selectedWish.HasBeenBought
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewEdit(WishEditVM model)
        {
            if (ModelState.IsValid)
            {
                var selectedWish = db.Wishes
            .Single(w => w.WishID == model.WishID && w.User.UserName == GetCurrentUsername());
                db.Entry(selectedWish).State = EntityState.Modified;
                db.Entry(selectedWish).Property(w => w.UserID).IsModified = false;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = model.WishID });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e.Message + e.StackTrace + e.InnerException;
                    return View("Error");
                }
            }

            return View(model);
        }

        // GET: Wish/Delete/1
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //checks current login status
            if (!IsLoggedIn("Delete", "Wish", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);

            Wish wish = db.Wishes.Find(id);
            if (wish == null)
                return HttpNotFound();
            return View(wish);
        }

        // POST: User/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResetRedirectLink();

            Wish wish = db.Wishes.Find(id);
            db.Wishes.Remove(wish);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // AJAX call
        public JsonResult MarkAsPurchased(int? id)
        {
            var wish = db.Wishes.Find(id);
            if (wish == null)
                return Json(new { success = false, ex = "This item cannot be marked as purchased." }, JsonRequestBehavior.AllowGet);

            if (wish.HasBeenBought)
                return Json(new { success = true, reload = false, reloadMsg = "This item has already been purchased" }, JsonRequestBehavior.AllowGet);

            db.Entry(wish).State = EntityState.Modified;
            wish.HasBeenBought = true;
            db.SaveChanges();
            return Json(new { success = true, reload = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks the current session username to the username attached the wish being accessed
        /// with <see cref="Wish.WishID"/> of <paramref name="id"/>
        /// </summary>
        /// <param name="id">The wish identifier.</param>
        /// <returns></returns>
        /// <remarks>Overload for the <see cref="BaseController.IsOwnerOfPage(int?)"/></remarks>
        protected override bool IsOwnerOfPage(int? id)
        {
            var username = GetCurrentUsername();
            if (id == null)
                return false;
            var pageOwner = db.Wishes.Find(id).User;

            return pageOwner.UserName == username;
        }

        /// <summary>
        /// A combiner helper function for <see cref="BaseController.IsLoggedIn(string, string,
        /// int?)"/> and <see cref="IsOwnerOfPage(int?)"/>
        /// </summary>
        /// <param name="id">The wish identifier.</param>
        /// <param name="currentController">The current controller ("Wish").</param>
        /// <param name="currentAction">The current action.</param>
        /// <returns>
        /// <see langword="true"/> if the current user session is logged in AND the wish being
        /// accessed belong to them, <see langword="false"/> otherwise.
        /// </returns>
        protected ActionResult IsAuthorized(int? id, string currentController, string currentAction)
        {
            if (!IsLoggedIn("Edit", "Wish", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);

            return new EmptyResult();
        }
    }
}