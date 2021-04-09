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
            //User model = new User();
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to find the username that is entered
                var user = db.Users.FirstOrDefault(u => u.UserName == model.UserName);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.User;
                // check if username exists already (can return null)
                var existingUserName = db.Users.FirstOrDefault(l => l.UserName == user.UserName);
                if (existingUserName != null) //an account already exists with username
                {
                    // Adds new validation message to form
                    ModelState.AddModelError("User.UserName", "This username already exists. Please try another username.");
                    return View(model);
                }

                // might not be filled in as it is not a required field
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    // check if email exists already (can return null)
                    var existingEmail = db.Users.FirstOrDefault(l => l.Email == user.Email);
                    if (existingEmail != null)
                    {
                        // Adds new validation message to form
                        ModelState.AddModelError("User.Email", "Sorry, there's already an account associated with this email. Please try again.");
                        return View(model);
                    }
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

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("Success", "Home");
        }
    }
}