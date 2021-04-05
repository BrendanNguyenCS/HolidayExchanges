using HolidayExchanges.Models;
using HolidayExchanges.ViewModels;
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
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wish wish = db.Wishes.Find(id);
            if (wish == null) return HttpNotFound();
            return View(wish);
        }

        // GET: Wish/Create
        [HttpGet]
        public ActionResult Create()
        {
            // check for log in status
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                Session["RedirectLink"] = Url.Action("Create", "Wish");
                return RedirectToAction("Login", "Login");
            }
            var user = db.Users.Where(u => u.UserName == username).Single();
            var model = new WishViewModel(user.UserID);
            return View(model);
        }

        // POST: Wish/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WishViewModel model)
        {
            if (ModelState.IsValid)
            {
                // reset for unused session variable
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
            // check for log in status
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                Session["RedirectLink"] = Url.Action("Edit", "Wish", id);
                return RedirectToAction("Login", "Login");
            }

            var selectedWish = db.Wishes
                .Single(w => w.WishID == id && w.User.UserName == username);

            return View(selectedWish);
        }

        // POST: Wish/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WishID,UserID,ItemName,Description,Quantity,ItemLink,PurchasingInstructions,HasBeenBought")] Wish model)
        {
            if (ModelState.IsValid)
            {
                this.ResetRedirectLink();

                db.Entry(model).State = EntityState.Modified;
                db.Entry(model).Property(m => m.UserID).IsModified = false;
                db.SaveChanges();
                // not sure whether to display wish details or entire user wishlist
                return RedirectToAction("Details", model.UserID);
                //return RedirectToAction("Wishlist", "User", model.UserID);
            }

            return View(model);
        }

        // GET: Wish/Delete/1
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            //checks current login status
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                // if not currently logged in, redirect to login page
                Session["RedirectLink"] = Url.Action("Delete", "Wish", id);
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Wish wish = db.Wishes.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            return View(wish);
        }

        // POST: User/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            this.ResetRedirectLink();

            Wish wish = db.Wishes.Find(id);
            db.Wishes.Remove(wish);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult MarkAsPurchased(int? id)
        {
            var wish = db.Wishes.Find(id);
            if (wish == null)
            {
                ViewBag.ErrorMessage = "This item cannot be marked as purchased.";
                return RedirectToAction("Error", "Home");
            }

            if (wish.HasBeenBought)
            {
                return RedirectToAction("Details", id);
            }

            db.Entry(wish).State = EntityState.Modified;
            wish.HasBeenBought = true;
            db.SaveChanges();
            return RedirectToAction("Details", id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}