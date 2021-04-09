using HolidayExchanges.DAL;
using HolidayExchanges.Models;
using System.Linq;
using System.Web.Mvc;

namespace HolidayExchanges.Controllers
{
    /// <summary>
    /// The base controller class with several authentication non-action helper methods
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly SecretSantaDbContext db = new SecretSantaDbContext();

        #region Checking the current session username

        /// <summary>
        /// Retrieves the current session username
        /// </summary>
        /// <returns></returns>
        /// <remarks>Not an action method</remarks>
        protected string GetCurrentUsername()
        {
            return Session["UserName"] != null ? Session["UserName"].ToString() : "";
        }

        #endregion Checking the current session username

        #region Retrieve the user of the current session from db

        /// <summary>
        /// Retrieves the user that corresponds to the current session username
        /// </summary>
        /// <returns></returns>
        /// <remarks>Not an action method</remarks>
        protected User GetCurrentUser()
        {
            var username = GetCurrentUsername();
            return db.Users.Single(u => u.UserName == username);
        }

        #endregion Retrieve the user of the current session from db

        #region Checking login status using a custom Controller extension method

        /// <summary>
        /// Determines whether the current user is logged based on the session variable
        /// </summary>
        /// <param name="currentActionMethod">The current action method.</param>
        /// <param name="currentController">The current controller.</param>
        /// <param name="routeValue">The route value.</param>
        /// <returns></returns>
        /// <remarks>Not an action method</remarks>
        protected bool IsLoggedIn(string currentActionMethod,
            string currentController,
            int? routeValue = 0)
        {
            var username = GetCurrentUsername();
            if (string.IsNullOrEmpty(username))
            {
                if (routeValue == 0)
                {
                    Session["RedirectLink"] = Url.Action(currentActionMethod, currentController);
                }
                else
                {
                    Session["RedirectLink"] = Url.Action(currentActionMethod, currentController, routeValue);
                }
                return false;
            }

            return true;
        }

        #endregion Checking login status using a custom Controller extension method

        #region Check current user's page access authorization as a boolean

        /// <summary>
        /// Determines whether the current user is authorized to access the page
        /// </summary>
        /// <param name="id">The identifier of the page's owner.</param>
        /// <returns></returns>
        /// <remarks>
        /// Usually will involve a user trying to access another user's edit details pages.
        /// </remarks>
        /// <example>
        /// If user <c>bnuge</c> wants to access <c>User/Edit/2</c> which is "owned" by goodvnguy,
        /// the method will check if they have the same user id. The method will find that they are
        /// not the same user and thus return false.
        /// </example>
        protected virtual bool IsOwnerOfPage(int? id)
        {
            var username = GetCurrentUsername();
            if (id == null)
                return false;
            var pageOwner = db.Users.Find(id);
            if (pageOwner == null)
                return false;
            var currentUser = db.Users.Single(u => u.UserName == username);
            //if (pageOwner.UserID != currentUser.UserID)
            //    return false;
            //return true;

            // same as above
            return pageOwner.UserID == currentUser.UserID;
        }

        #endregion Check current user's page access authorization as a boolean

        #region Reset RedirectLink session variable

        /// <summary>
        /// Resets the redirect link.
        /// </summary>
        protected void ResetRedirectLink()
        {
            Session["RedirectLink"] = null;
        }

        #endregion Reset RedirectLink session variable

        // TODO: Is there a way to add a method that automatically combines IsLoggedIn and IsOwnerOfPage in the child classes (consider the fact that the GroupController and WishController had overriding methods for IsOwnerOfPage)

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