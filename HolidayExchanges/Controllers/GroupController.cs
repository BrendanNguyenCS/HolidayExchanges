using HolidayExchanges.DAL;
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
    public class GroupController : Controller
    {
        private readonly SecretSantaDbContext _context = new SecretSantaDbContext();
        private SecretSantaManager _santaMgr;

        public GroupController()
        {
            _santaMgr = new SecretSantaManager(_context);
        }

        // Group/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Group/Details/1
        public ActionResult Details(int id)
        {
            if (id == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Group group = _context.Groups.Find(id);
            if (group == null) return HttpNotFound();
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
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Login");
            return View();
        }

        // POST: Group/Create/1 To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,Name,ExchangeDate")] Group group)
        {
            if (ModelState.IsValid)
            {
                var username = Session["UserName"].ToString();
                var currentUser = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (currentUser != null)
                {
                    _context.Groups.Add(group);
                    _context.UserGroups.Add(new UserGroup { UserID = currentUser.UserID, GroupID = group.GroupID });
                    _context.SaveChanges();
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
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Group group = _context.Groups.Find(id);
            if (group == null) return HttpNotFound();
            return View(group);
        }

        // POST: Group/Edit/5 To protect from overposting attacks, enable the specific properties
        // you want to bind to, for more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,Name,ExchangeDate")] Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(group).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Group/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Group group = _context.Groups.Find(id);
            if (group == null) return HttpNotFound();
            return View(group);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = _context.Groups.Find(id);
            _context.Groups.Remove(group);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Group/Join/1
        [HttpGet]
        public ActionResult Join(int id)
        {
            var username = Session["UserName"] != null ? Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                Session["RedirectLink"] = Url.Action("Join", "Group", id);
                return RedirectToAction("Login", "Login");
            }
            var group = _context.Groups.Find(id);
            if (group != null) return View(group);
            ViewBag.ErrorMessage = "Unable to locate event group";
            return RedirectToAction("Error", "Home");
        }

        // POST: Group/Join/1
        [HttpPost]
        public ActionResult Join(Group model)
        {
            var username = Session["UserName"].ToString();
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            var group = _santaMgr.GetGroupById(model.GroupID);

            // reset for unused session variable
            Session["RedirectLink"] = null;

            try
            {
                _context.UserGroups.Add(new UserGroup { UserID = user.UserID, GroupID = group.GroupID });
                _context.SaveChanges();
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
            if (id == 0) return HttpNotFound();
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
                    var user = ug.User;
                    var group = ug.Group;
                    var recipient = _context.Users.Find(ug.RecipientUserID);
                    var message = new MailMessage();
                    message.To.Add(user.Email);

                    message.Subject = "Your assignment for the group " + group.Name;
                    message.Body = "<h1>Secret Santa Pair Notification</h1>" + Environment.NewLine +
                        "<p>Hello " + user.UserName + ",</p>" + Environment.NewLine +
                        "<p>Your recipient for the " + group.Name + " group has been chosen.<p>" + Environment.NewLine +
                        "<p><strong>Your recipient</strong>: " + recipient.UserName + "</p>" + Environment.NewLine +
                        "<p>To view their wishlist, go to our website and view your profile.</p>" + Environment.NewLine +
                        "<p>Thank you,</p>" + Environment.NewLine +
                        "<p>From your friends at Holiday Exchanges MA</p>";
                    message.IsBodyHtml = true;

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
            return RedirectToAction("Success", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}