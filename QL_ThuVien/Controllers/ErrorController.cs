using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_ThuVien.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.ErrorCode = 404;
            ViewBag.ErrorContent = "Không tìm thấy tài nguyên!";
            return View();
        }
    }
}