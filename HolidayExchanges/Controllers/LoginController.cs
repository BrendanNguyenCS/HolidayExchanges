using HolidayExchanges.Models;
using HolidayExchanges.Services;
using HolidayExchanges.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class LoginController : BaseController
    {
        public HashManager hasher = new HashManager();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region Replaced by remote validation logic

                /*
                    var user = db.Users.SingleOrDefault(u => u.UserName == model.UserName);

                    if (user == null) // Username doesn't exist in the db
                    {
                        ModelState.AddModelError("UserName", "Incorrect username");
                        return View(model);
                    }
                    else
                    {
                        // Check if entered password matches user's password in db
                        var IsAUser = hasher.VerifyPassword(model.Password, user.Password, user.Salt);

                        if (!IsAUser) // Incorrect password
                        {
                            // Replaced using remote validation? https://docs.microsoft.com/en-us/previous-versions/aspnet/gg508808(v=vs.98)?redirectedfrom=MSDN#adding-custom-remote-validation
                            //ModelState.AddModelError("Password", "The password you entered is incorrect.");

                            return View(model);
                        }
                    }
                */

                #endregion Replaced by remote validation logic

                Session["UserName"] = model.UserName;

                if (Session["RedirectLink"] != null)
                {
                    return Redirect(Session["RedirectLink"].ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region Has been replaced by data annotations compare and remote validation attributes

                /*
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

                    var confirmPassword = model.ConfirmPassword;
                    if (!model.ValidatePassword())
                    {
                        ModelState.AddModelError("ConfirmPassword", "The ConfirmPassword field must match the Password field.");
                        return View(model);
                    }
                */

                #endregion Has been replaced by data annotations compare and remote validation attributes

                if (model.Birthday == null)
                {
                    model.Birthday = DateTime.UtcNow.Date; // set to default value of date of registration
                }

                var user = new User
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    State = model.State,
                    Zip = model.Zip,
                    Country = model.Country,
                    Email = model.Email,
                    Birthday = model.Birthday,
                    PhoneNumber = model.PhoneNumber
                };

                // only executes if username AND email doesn't exist in the db and password has been verified
                var userSalt = hasher.GenerateSalt();
                var userHash = hasher.ComputeHash(model.Password, userSalt);
                user.Password = Convert.ToBase64String(userHash);
                user.Salt = Convert.ToBase64String(userSalt);

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

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordVM model = new ForgotPasswordVM();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordVM model)
        {
            var user = db.Users.SingleOrDefault(u => u.UserName == model.UserName);
            // no need for null check since validation took care of it
            db.Entry(user).Property(u => u.Password).IsModified = true;
            db.Entry(user).Property(u => u.Salt).IsModified = true;

            var salt = hasher.GenerateSalt();
            var hash = hasher.ComputeHash(model.NewPassword, salt);
            user.Password = Convert.ToBase64String(hash);
            user.Salt = Convert.ToBase64String(salt);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message + e.StackTrace + e.InnerException;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!IsLoggedIn("ChangePassword", "User", id))
                return RedirectToAction("Login", "Login");

            if (!IsOwnerOfPage(id))
            {
                var currentUser = GetCurrentUser();
                return RedirectToAction("Details", "User", new { id = currentUser.UserID });
            }

            var model = new ChangePasswordVM();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            var user = GetCurrentUser();
            db.Entry(user).Property(u => u.Password).IsModified = true;
            db.Entry(user).Property(u => u.Salt).IsModified = true;
            var salt = hasher.GenerateSalt();
            var hash = hasher.ComputeHash(model.NewPassword, salt);
            user.Password = Convert.ToBase64String(hash);
            user.Salt = Convert.ToBase64String(salt);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message + e.StackTrace + e.InnerException;
                return View("Error");
            }
        }

        #region Remote Validation Methods

        [HttpGet]
        public JsonResult IsAValidUser(string UserName)
        {
            return Json(db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsPasswordCorrect(string Password, string UserName)
        {
            var user = db.Users.SingleOrDefault(u => u.UserName == UserName);
            return Json(hasher.VerifyPassword(Password, user.Password, user.Salt), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsUsernameAvailable(string UserName)
        {
            return Json(!db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsEmailAvailable(string Email)
        {
            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsEmailAvailableOnEdit(string Email, string OriginalEmail)
        {
            if (Email.Equals(OriginalEmail))
            {
                // validates field if unchanged
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsUsernameAvailableOnEdit(string UserName, string OriginalUserName)
        {
            if (UserName.Equals(OriginalUserName))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(!db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        #endregion Remote Validation Methods

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}