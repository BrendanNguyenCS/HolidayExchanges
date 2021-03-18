using HolidayExchanges.DAL;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HolidayExchanges.Components
{
    /// <summary>
    /// A helper class that deals with login status and page access authorization
    /// </summary>
    public static class CheckLoginStatus
    {
        private static SecretSantaDbContext db = new SecretSantaDbContext();

        #region Attempt as checking login status

        /// <summary>
        /// Determines whether [is logged in] [the specified current action method].
        /// </summary>
        /// <param name="currentActionMethod">The current action method.</param>
        /// <param name="currentController">The current controller.</param>
        /// <param name="routeValue">The route value.</param>
        /// <returns>A <see cref="bool"/>: <c>true</c> if [is logged in]; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Relies on the <see cref="HttpContext"/> to get the session variable; the <see
        /// cref="UrlHelper"/> class and its <see cref="UrlHelper.Action(string, string)"/>, <see
        /// cref="UrlHelper.RouteUrl(object)"/> methods to set the redirect link session variable
        /// </remarks>
        public static bool IsLoggedIn(string currentActionMethod, string currentController, int routeValue = 0)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            var username = HttpContext.Current.Session["UserName"] != null ? HttpContext.Current.Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                if (routeValue == 0)
                {
                    HttpContext.Current.Session["RedirectLink"] = url.Action(currentActionMethod, currentController);
                }
                else
                {
                    HttpContext.Current.Session["RedirectLink"] = url.RouteUrl(new { controller = currentController, action = currentActionMethod, id = routeValue });
                }
                return false;
            }
            return true;
        }

        #endregion Attempt as checking login status

        #region Checking login status using a custom Controller extension method

        /// <summary>
        /// Determines whether [is logged in] [the specified current action method].
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="currentActionMethod">The current action method.</param>
        /// <param name="currentController">The current controller.</param>
        /// <param name="routeValue">The route value.</param>
        /// <returns></returns>
        /// <remarks>Is a custom <see cref="Controller"/> extension method</remarks>
        public static ActionResult IsLoggedIn(this Controller controller,
            string currentActionMethod,
            string currentController,
            int routeValue = 0)
        {
            var username = controller.Session["UserName"] != null ? controller.Session["UserName"].ToString() : "";
            if (string.IsNullOrEmpty(username))
            {
                if (routeValue == 0)
                {
                    controller.Session["RedirectLink"] = controller.Url.Action(currentActionMethod, currentController);
                }
                else
                {
                    controller.Session["RedirectLink"] = controller.Url.Action(currentActionMethod, currentController, routeValue);
                }
                RouteValueDictionary route = new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Login"
                });
                return new RedirectToRouteResult(route);
            }

            // returns nothing (like void)
            return new EmptyResult();
        }

        #endregion Checking login status using a custom Controller extension method

        #region Reset RedirectLink session variable

        /// <summary>
        /// Resets the redirect link.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <remarks>Is a custom <see cref="Controller"/> extension method.</remarks>
        public static void ResetRedirectLink(this Controller controller)
        {
            controller.Session["RedirectLink"] = null;
        }

        #endregion Reset RedirectLink session variable

        #region Check current user's page access authorization

        /// <summary>
        /// Determines whether [is current user owner of page] [the specified identifier].
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <example>
        /// If user <c>bnuge</c> wants to access <c>User/Edit/2</c> which is "owned" by goodvnguy,
        /// the method will check if they have the same user id. The method will find that they are
        /// not the same user and thus will redirect them to <c>bnuge</c>'s details page with the
        /// error message shown.
        /// </example>
        public static ActionResult IsCurrentUserOwnerOfPage(this Controller controller, int? id)
        {
            var username = controller.Session["UserName"] != null ? controller.Session["UserName"].ToString() : "";
            // going to assume that the current user is already logged in since that is checked
            // before this method is called
            var pageOwner = db.Users.Find(id);
            if (pageOwner == null)
            {
                RouteValueDictionary route = new RouteValueDictionary(new
                {
                    action = "Error",
                    controller = "Home"
                });
                return new RedirectToRouteResult(route);
            }
            var currentUser = db.Users.Where(u => u.UserName == username).Single();

            if (pageOwner.UserID != currentUser.UserID)
            {
                // redirect to current user's details page with "not authorized to access" message
                // on top
                controller.ViewBag.NotAuthorizedMessage = "You aren't authorized to enter the previous page. You have been redirected to this page. If this is in error, please contact the developer.";
                RouteValueDictionary route = new RouteValueDictionary(new
                {
                    action = "Details",
                    controller = "User",
                    id = currentUser.UserID
                });
                return new RedirectToRouteResult(route);
            }

            return new EmptyResult();
        }

        #endregion Check current user's page access authorization
    }
}