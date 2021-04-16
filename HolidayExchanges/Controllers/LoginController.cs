using HolidayExchanges.Models;
using HolidayExchanges.Services;
using HolidayExchanges.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class LoginController : BaseController
    {
        public HashManager hasher = new HashManager();

        [HttpGet]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.UserName == model.UserName);

                if (user == null) // Username doesn't exist in the db
                {
                    ModelState.AddModelError("UserName", "This username doesn't exist in our system. Try again, and contact the system administrator if the problem persists.");
                    return View(model);
                }
                else
                {
                    // Check if entered password matches user's password in db
                    var IsAUser = hasher.VerifyPassword(model.Password, user.Password, user.Salt);

                    if (!IsAUser) // Incorrect password
                    {
                        ModelState.AddModelError("Password", "The password you entered is incorrect.");
                        return View(model);
                    }
                }

                Session["UserName"] = model.UserName;

                if (Session["RedirectLink"] != null)
                {
                    return Redirect(Session["RedirectLink"].ToString());
                }

                return RedirectToAction("Index", "Home"); // redirect to main page
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ValidateLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.UserName == model.UserName);

                if (user == null)
                {
                    ModelState.AddModelError("UserName", "This username doesn't exist.");
                    return Json(new { success = false, ex = "This username doesn't exist." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var isAUser = hasher.VerifyPassword(model.Password, user.Password, user.Salt);

                    if (!isAUser)
                    {
                        ModelState.AddModelError("Password", "This password is incorrect.");
                        return Json(new { success = false, ex = "This password is incorrect." }, JsonRequestBehavior.AllowGet);
                    }
                }

                Session["UserName"] = model.UserName;
                if (Session["RedirectLink"] != null)
                    return Json(new { success = true, redirect = true, redirectUrl = Session["RedirectLink"].ToString() });

                return Json(new { success = true, redirect = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Register()
        {
            User user = new User();
            RegisterViewModel model = new RegisterViewModel
            {
                User = user
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.User;
                var existingUserName = db.Users.SingleOrDefault(l => l.UserName == user.UserName);
                if (existingUserName != null) //an account already exists with username
                {
                    ModelState.AddModelError("User.UserName", "This username already exists. Please try another username.");
                    return View(model);
                }

                var existingEmail = db.Users.SingleOrDefault(l => l.Email == user.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("User.Email", "Sorry, there's already an account associated with this email. Please try again.");
                    return View(model);
                }

                // retrieve password fields and compares them
                var confirmPassword = model.ConfirmPassword;
                if (!model.ValidatePassword())
                {
                    ModelState.AddModelError("ConfirmPassword", "The ConfirmPassword field must match the Password field.");
                    return View(model);
                }

                // check if birthday is blank
                if (user.Birthday == null)
                {
                    user.Birthday = DateTime.UtcNow.Date; // set to default value of date of registration
                }

                // only executes if username AND email doesn't exist in the db and password has been verified
                var userSalt = hasher.GenerateSalt(); // creates salt
                var userHash = hasher.ComputeHash(user.Password, userSalt); // creates hashed password
                user.Password = Convert.ToBase64String(userHash); // save above to appropriate login acct
                user.Salt = Convert.ToBase64String(userSalt); // save salt to same acct

                // adding both objects to db
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return RedirectToAction("Error", "Home");
                }

                Session["UserName"] = user.UserName;
                if (Session["RedirectLink"] != null)
                {
                    return Redirect(Session["RedirectLink"].ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult AjaxRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.User;
                var existingUser = db.Users.SingleOrDefault(l => l.UserName == model.User.UserName);
                if (existingUser != null)
                    return Json(new { success = false, ex = "This username already exists." }, JsonRequestBehavior.AllowGet);

                var existingEmail = db.Users.SingleOrDefault(l => l.UserName == model.User.Email);
                if (existingEmail != null)
                    return Json(new { success = false, ex = "This email already exists." }, JsonRequestBehavior.AllowGet);

                if (!model.ValidatePassword())
                    return Json(new { success = false, ex = "Your password fields are different." }, JsonRequestBehavior.AllowGet);

                if (user.Birthday == null)
                    user.Birthday = DateTime.UtcNow.Date;

                var userSalt = hasher.GenerateSalt();
                var userHash = hasher.ComputeHash(user.Password, userSalt);
                user.Password = Convert.ToBase64String(userHash);
                user.Salt = Convert.ToBase64String(userSalt);

                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, ex = "Sorry but something went wrong and your registration failed. Please try again." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("Success", "Home");
        }

        #region Method to satisfy abstraction (this doesn't do anything)

        protected override ActionResult IsAuthorized(int? id, string currentController, string currentAction)
        {
            return new EmptyResult();
        }

        #endregion Method to satisfy abstraction (this doesn't do anything)
    }
}