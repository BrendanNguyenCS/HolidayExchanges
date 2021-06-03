using HolidayExchanges.Models;
using HolidayExchanges.Services;
using HolidayExchanges.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    public class LoginController : BaseController
    {
        public HashManager hasher = new HashManager();

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // username & password checking here has been replaced by data annotations in view model

                Session["UserName"] = model.UserName;

                if (Session["RedirectLink"] != null)
                    return Redirect(Session["RedirectLink"].ToString());

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // username & password checking here has been replaced by data annotations in view model

                if (model.Birthday == null)
                    model.Birthday = DateTime.UtcNow.Date; // set to default value of date of registration

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
                    return Redirect(Session["RedirectLink"].ToString());

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

        // just a discovery method for sending emails with Razor template
        public async Task<ActionResult> SendForgotPWEmail(User user)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("~/Views/EmailTemplates/ForgotPWEMail.cshtml"))
            {
                body = reader.ReadToEnd();
                body = body.Replace("{Username}", user.UserName);
            }

            var message = new MailMessage
            {
                From = new MailAddress("holidayexchanges.ma@gmail.com", "Holiday Exchanges"),
                Subject = "Forgot your password?",
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(user.Email);

            try
            {
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = $"Error. Please try again later.\n{e.Message}";
                return View("Error");
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!IsLoggedIn("ChangePassword", "User", id))
                return RedirectToAction("Login", "Login");

            var currentUser = GetCurrentUser();
            var model = new ChangePasswordVM()
            {
                Username = currentUser.UserName
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
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

            return View(model);
        }

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("Index", "Home");
        }

        #region Remote Validation Methods

        /// <summary>
        /// Remote validation method that checks the existence of the desired username in the database.
        /// </summary>
        /// <param name="UserName">The desired username.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult IsAValidUser(string UserName)
        {
            return Json(db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remote validation method that checks if the entered password is correct for the given username.
        /// </summary>
        /// <param name="Password">The entered password.</param>
        /// <param name="UserName">The entered username.</param>
        /// <remarks>Assumes that the IsAValidUser method has returned true.</remarks>
        [HttpGet]
        public JsonResult IsPasswordCorrect(string Password, string UserName)
        {
            var user = db.Users.SingleOrDefault(u => u.UserName == UserName);
            return Json(hasher.VerifyPassword(Password, user.Password, user.Salt), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remote validation method that checks if the username has not been taken by another user
        /// in the database.
        /// </summary>
        /// <param name="UserName">The desired username.</param>
        /// <returns>
        /// <see langword="true"/> if the username doesn't exist in the database. <see
        /// langword="false"/> otherwise.
        /// </returns>
        [HttpGet]
        public JsonResult IsUsernameAvailable(string UserName)
        {
            return Json(!db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remote validation method that checks if the desired email is already associated with
        /// another user in the database.
        /// </summary>
        /// <param name="Email">The desired email.</param>
        /// <returns>
        /// <see langword="true"/> if the email doesn't exist. <see langword="false"/> otherwise.
        /// </returns>
        [HttpGet]
        public JsonResult IsEmailAvailable(string Email)
        {
            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remote validation method that checks if the desired email is already associated with
        /// another user in the database when editing a user profile.
        /// </summary>
        /// <param name="Email">The desired email.</param>
        /// <param name="OriginalEmail">The user's original email.</param>
        /// <returns>
        /// <see langword="true"/> if the email doesn't exist OR the email hasn't been changed. <see
        /// langword="false"/> otherwise.
        /// </returns>
        [HttpGet]
        public JsonResult IsEmailAvailableOnEdit(string Email, string OriginalEmail)
        {
            if (Email.Equals(OriginalEmail))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Remote validation method that checks if the username has not been taken by another user
        /// in the database when editing a user profile.
        /// </summary>
        /// <param name="UserName">The desired username.</param>
        /// <param name="OriginalUserName">The user's original username.</param>
        /// <returns>
        /// <see langword="true"/> if the username doesn't exist in the database OR the username
        /// hasn't been changed. <see langword="false"/> otherwise.
        /// </returns>
        [HttpGet]
        public JsonResult IsUsernameAvailableOnEdit(string UserName, string OriginalUserName)
        {
            if (UserName.Equals(OriginalUserName))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json(!db.Users.Any(u => u.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        #endregion Remote Validation Methods
    }
}