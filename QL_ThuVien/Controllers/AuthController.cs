using QL_ThuVien.Containers;
using QL_ThuVien.Intergrate.Models;
using QL_ThuVien.Intergrate.Services.Data.ORM;
using QL_ThuVien.Intergrate.Services.Helper;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QL_ThuVien.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private ServicesContainer _services;
        public AuthController()
        {
            _services = ServicesContainer.Container;
        }

        /// <summary>
        /// Trang tích hợp
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            CaptchaHelper.captchaText = CaptchaHelper.GenerateCaptchaCode(4);
            Session["captchaCode"] = CaptchaHelper.captchaText;
            Session["NXBList"] = _services.Db.NHAXUATBANs.ToList();
            Session["CDList"] = _services.Db.CHUDEs.ToList();
            Session["NXBSelectList"] = new SelectList(Session["NXBList"] as List<NhaXuatBan>, "MaNXB", "TenNXB");
            Session["CDSelectList"] = new SelectList(Session["CDList"] as List<ChuDe>, "MaChuDe", "TenChuDe");
            Session["SachList"] = _services.Db.SACHes.ToList();
            //Kiểm tra đã có ticket chưa?
            try
            {
                if (HttpContext.Request.Cookies.Count == 0)
                {
                    return View();
                }
                else
                {
                    var cookie = HttpContext.Request.Cookies[".ASPXAUTH"];
                    if (cookie == null)
                    {
                        return View();
                    }
                    var authTicket = FormsAuthentication.Decrypt(cookie.Value);
                    if (authTicket == null || authTicket.Expired)
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");

        }
        [AllowAnonymous]
        public ActionResult Image()
        {
            byte[] imageData = CaptchaHelper.ImageToByte(CaptchaHelper.GenetareCaptchaImage());
            return File(imageData, "image/jpeg");
        }
        [AllowAnonymous]
        public ActionResult CaptchaPartial()
        {
            return PartialView();
        }
        
        [HttpPost]
        [AllowAnonymous]
        //public string SignIn(string name, string pass)
        //{
        //    string connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};",
        //                "DESKTOP-GUE0JS7",
        //                "QLTHUVIEN",
        //                name,
        //                pass
        //            );
        //    try
        //    {
        //        using (var conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            HttpContext.Session.Add("cn", SecurityHelper.Encrypt(connectionString, "QLTHUVIEN"));
        //            _services = ServicesContainer.Container;
        //            logger.Info($"IP {HttpContext.Request.UserHostAddress} has been connected.");
        //            if (name.Equals("sa"))
        //            {
        //                FormsAuthentication.SetAuthCookie(name, false);
        //                var authTicket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddHours(3), false, "Quản trị viên");
        //                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        //                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //                HttpContext.Response.Cookies.Add(authCookie);
        //                HttpContext.Session.Add("username", "System Administrator");
        //            }
        //            else
        //            {
        //                Dapper appDBContext = new Dapper(ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString);

        //                var user = appDBContext.QueryTable<TaiKhoanV2>("TaiKhoan").SingleOrDefault(x => x.TenDN.Equals(name));
        //                FormsAuthentication.SetAuthCookie(user.TenDN, false);
        //                var authTicket = new FormsAuthenticationTicket(1, user.TenDN, DateTime.Now, DateTime.Now.AddHours(3), false, user.ChucVu);
        //                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        //                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //                HttpContext.Response.Cookies.Add(authCookie);
        //                string userName = appDBContext.QueryTable<NhanVien>("NhanVien").SingleOrDefault(x => x.MaNhanVien == user.MaNhanVien).HoTen;
        //                HttpContext.Session.Add("username", userName);
        //            }                   
        //            return "";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return "Tên tài khoản hoặc mật khẩu không đúng!";
        //    }
        //}
        public string SignIn(string name, string pass)
        {
            //FormsAuthentication.SetAuthCookie(name, false);
            //var authTicket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddHours(3), false, "Quản trị viên");
            //string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            //var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            //HttpContext.Response.Cookies.Add(authCookie);
            //HttpContext.Session.Add("username", "System Administrator");
            //return "";
            ////gk9yyKQ@P39gfsW
            string connectionString = ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    HttpContext.Session.Add("cn", SecurityHelper.Encrypt(connectionString, "QLTHUVIEN"));
                    _services = ServicesContainer.Container;
                    logger.Info($"IP {HttpContext.Request.UserHostAddress} has been connected.");
                    Dapper appDBContext = new Dapper(ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString);

                    var user = appDBContext.QueryTable<TaiKhoanV2>("TaiKhoan").SingleOrDefault(x => x.TenDN.Equals(name));
                    if (user == null)
                    {
                        return "Tên tài khoản hoặc mật khẩu không đúng!";
                    }
                    FormsAuthentication.SetAuthCookie(user.TenDN, false);
                    var authTicket = new FormsAuthenticationTicket(1, user.TenDN, DateTime.Now, DateTime.Now.AddHours(3), false, user.ChucVu);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Response.Cookies.Add(authCookie);
                    string userName = appDBContext.QueryTable<NhanVien>("NhanVien").SingleOrDefault(x => x.MaNhanVien == user.MaNhanVien).HoTen;
                    HttpContext.Session.Add("username", userName);
                    return "";
                }

            }
            catch (Exception ex)
            {
                return "Tên tài khoản hoặc mật khẩu không đúng!";
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public string SignInForUser(string name)
        {
            if (CaptchaHelper.captchaText.Equals(name))
            {
                return "";
            }
            return "Captcha không đúng, vui lòng thử lại!";
        }
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}