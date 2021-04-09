using HolidayExchanges.Models;
using HolidayExchanges.Services;
using HolidayExchanges.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class GroupController : BaseController
    {
        private SecretSantaManager _santaMgr;

        public GroupController()
        {
            _santaMgr = new SecretSantaManager(db);
        }

        // Group/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Group/Details/1
        public ActionResult Details(int id)
        {
            if (id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Group group = db.Groups.Find(id);
            if (group == null)
                return HttpNotFound();
            var users = _santaMgr.GetAllUsersInGroup(id);
            var GroupViewModel = new GroupViewModel
            {
                Group = group,
                Users = users
            };
            return View(GroupViewModel);
        }

        // GET: Group/Create/1
        [HttpGet]
        public ActionResult Create()
        {
            if (!IsLoggedIn("Create", "Group"))
                return RedirectToAction("Login", "Login");

            return View();
        }

        // POST: Group/Create/1 To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,Name,ExchangeDate,HasBeenPaired,Creator")] Group group)
        {
            if (ModelState.IsValid)
            {
                var username = Session["UserName"].ToString();
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == username);
                group.Creator = username;

                ResetRedirectLink();

                if (currentUser != null)
                {
                    db.Groups.Add(group);
                    db.UserGroups.Add(new UserGroup { UserID = currentUser.UserID, GroupID = group.GroupID });
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Register", "Login");
            }

            return View(group);
        }

        // GET: Group/Edit/1
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!IsLoggedIn("Edit", "Group", id))
                return RedirectToAction("Login", "Login");
            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);
            Group group = db.Groups.Find(id);
            db.Entry(group).Property(g => g.Creator).IsModified = false;
            if (group == null)
                return HttpNotFound();
            return View(group);
        }

        // POST: Group/Edit/5 To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,Name,ExchangeDate,HasBeenPaired,Creator")] Group group)
        {
            if (ModelState.IsValid)
            {
                ResetRedirectLink();

                db.Entry(group).State = EntityState.Modified;
                db.Entry(group).Property(g => g.HasBeenPaired).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = group.GroupID });
            }
            return View(group);
        }

        // GET: Group/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!IsLoggedIn("Delete", "Group", id))
                return RedirectToAction("Login", "Login");
            if (!IsOwnerOfPage(id))
                return RedirectToAction("Details", id);
            Group group = db.Groups.Find(id);
            if (group == null)
                return HttpNotFound();
            return View(group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResetRedirectLink();

            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Group/Join/1
        [HttpGet]
        public ActionResult Join(int id)
        {
            if (id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!IsLoggedIn("Join", "Group", id))
                return RedirectToAction("Login", "Login");
            var group = db.Groups.Find(id);
            if (group != null)
                return View(group);
            ViewBag.ErrorMessage = "Unable to locate event group";
            return RedirectToAction("Error", "Home");
        }

        // POST: Group/Join/1
        [HttpPost]
        public ActionResult Join(Group model)
        {
            var username = Session["UserName"].ToString();
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            var group = _santaMgr.GetGroupById(model.GroupID);

            ResetRedirectLink();

            try
            {
                db.UserGroups.Add(new UserGroup { UserID = user.UserID, GroupID = group.GroupID });
                db.SaveChanges();
                return RedirectToAction("Success", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message + ". " + ex.InnerException.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public ActionResult Pair(int id)
        {
            if (id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var group = _santaMgr.GetGroupById(id);
            if (group != null)
            {
                _santaMgr.GetRecipientAssignments(id);
                return RedirectToAction("Success", "Home");
            }
            ViewBag.ErrorMessage = "Pairing failed.";
            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> SendAssignments(int id)
        {
            var list = _santaMgr.GetUserGroupsForGroup(id);
            foreach (var ug in list)
            {
                try
                {
                    #region Email Contacts

                    var user = ug.User;
                    var group = ug.Group;
                    var recipient = db.Users.Find(ug.RecipientUserID);
                    var message = new MailMessage();
                    message.To.Add(user.Email);

                    #endregion Email Contacts

                    #region Email Content

                    message.Subject = "Your assignment for the group " + group.Name;
                    message.Body = "<h1>Secret Santa Pair Notification</h1>" + Environment.NewLine +
                        "<p>Hello " + user.UserName + ",</p>" + Environment.NewLine +
                        "<p>Your recipient for the " + group.Name + " group has been chosen.<p>" + Environment.NewLine +
                        "<p><strong>Your recipient</strong>: " + recipient.UserName + "</p>" + Environment.NewLine +
                        "<p>To view their wishlist, go to our website and view your profile.</p>" + Environment.NewLine +
                        "<p>Thank you,</p>" + Environment.NewLine +
                        "<p>From your friends at Holiday Exchanges MA</p>";
                    message.IsBodyHtml = true;

                    #endregion Email Content

                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
                catch (SmtpException ex)
                {
                    string msg = "Email cannot be sent:";
                    msg += ex.Message;
                    throw new Exception(msg);
                }
                catch (ArgumentNullException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return RedirectToAction("Details", id);
        }

        /// <summary>
        /// Checks the current session username to the Creator property of the group with <see
        /// cref="Group.GroupID"/> of <paramref name="id"/>
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <returns></returns>
        /// <remarks>Overload for the <see cref="BaseController.IsOwnerOfPage(int?)"/></remarks>
        protected override bool IsOwnerOfPage(int? id)
        {
            var username = GetCurrentUsername();
            var currentGroup = db.Groups.Find(id);
            return currentGroup.Creator == username;
        }
    }
}