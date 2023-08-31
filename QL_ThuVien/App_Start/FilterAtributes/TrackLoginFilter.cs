using QL_ThuVien.Intergrate.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_ThuVien.App_Start.FilterAtributes
{
    /// <summary>
    /// Filter for check list online user
    /// </summary>
    public class TrackLoginsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, DateTime> loggedInUsers = SecurityHelper.GetLoggedInUsers();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (loggedInUsers.ContainsKey(HttpContext.Current.User.Identity.Name))
                {
                    loggedInUsers[HttpContext.Current.User.Identity.Name] = System.DateTime.Now;
                }
                else
                {
                    loggedInUsers.Add(HttpContext.Current.User.Identity.Name, System.DateTime.Now);
                }
            }

            // remove users where time exceeds session timeout
            var keys = loggedInUsers.Where(u => DateTime.Now.Subtract(u.Value).Minutes >
                       HttpContext.Current.Session.Timeout).Select(u => u.Key);
            foreach (var key in keys.ToList())
            {
                loggedInUsers.Remove(key);
            }

        }
    }
}