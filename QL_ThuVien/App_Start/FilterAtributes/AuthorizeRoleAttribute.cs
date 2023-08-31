using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QL_ThuVien.App_Start.FilterAtributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public AuthorizeRoleAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("roles");

            this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            logger.Info($"IP {filterContext.HttpContext.Request.UserHostAddress} access denined!. ({filterContext.HttpContext.Request.Path})");
            filterContext.Result = new RedirectResult("~/Auth");
        }
    }
}