using HolidayExchanges.DAL;
using HolidayExchanges.Models;
using HolidayExchanges.Services;
using HolidayExchanges.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class UserController : Controller
    {
        private SecretSantaDbContext db = new SecretSantaDbContext();
        private SecretSantaManager santaMgr;

        public UserController()
        {
            santaMgr = new SecretSantaManager(db);
        }

        // GET: User, Not ideal: the "Index" should be the user's main page with profile
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            //var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            //if (string.IsNullOrEmpty(username))
            //{
            //    Session["RedirectLink"] = Url.Action("Details", "User", id);
            //    return RedirectToAction("Login", "Login");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            int num = id.GetValueOrDefault();
            if (user == null) return HttpNotFound();
            UserViewModel model = new UserViewModel
            {
                User = user,
                Groups = santaMgr.GetAllGroupsForUser(num)
            };
            model.Recipients = santaMgr.GetRecipientsForAllGroupsForUser(num, model.Groups);
            return View(model);
        }

        /*Not needed since register acts as a user creation tool
        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create To protect from overposting attacks, enable the specific properties you
        // want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,UserName,Password,Salt,FirstName,LastName,Address1,Address2,City,State,Zip,Country,Email,Birthday,PhoneNumber")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }*/

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            //var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            //if (string.IsNullOrEmpty(username))
            //{
            //    Session["RedirectLink"] = Url.Action("Edit", "User", id);
            //    return RedirectToAction("Login", "Login");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
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
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Property(u => u.UserName).IsModified = false;
                db.Entry(user).Property(u => u.Password).IsModified = false;
                db.Entry(user).Property(u => u.Salt).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            // checks current login status
            //var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            //if (string.IsNullOrEmpty(username))
            //{
            //    // if not currently logged in, redirect to login page
            //    Session["RedirectLink"] = Url.Action("Delete", "User", id);
            //    return RedirectToAction("Login", "Login");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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