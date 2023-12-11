using PagedList;
using QL_ThuVien.App_Start.FilterAtributes;
using QL_ThuVien.Containers;
using QL_ThuVien.Intergrate.Models;
using QL_ThuVien.Intergrate.Services.Helper;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using QL_ThuVien.DTO;


namespace QL_ThuVien.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ServicesContainer _services;

        public HomeController()
        {
            _services = ServicesContainer.Container;
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Kiểm tra đã có ticket chưa?
            try
            {
                if (HttpContext.Request.Cookies.Count == 0)
                {
                    Session.Clear();
                    return RedirectToAction("Index", "Auth");
                }
                else
                {
                    var cookie = HttpContext.Request.Cookies[".ASPXAUTH"];
                    if (cookie == null)
                    {
                        Session.Clear();
                        return RedirectToAction("Index", "Auth");
                    }
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var role = ticket.UserData;
                    ViewBag.TotalReaders = _services.Db.THETHUVIENs.Count();
                    ViewBag.TotalBooks = _services.Db.SACHes.Count();
                    ViewBag.TotalViolations = _services.Db.BIVIPHAMs.Count();
                    ViewBag.TotalBorrowBooks = _services.Db.PHIEUMUONs.Count();
                    ViewBag.TotalBookBacks = _services.Db.PHIEUTRAs.Count();
                    //ViewBag.TotalReaders = 100;
                    //ViewBag.TotalBooks = 203;
                    //ViewBag.TotalViolations = 12;
                    //ViewBag.TotalBorrowBooks = 1000;
                    //ViewBag.TotalBookBacks = 510;
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Auth");
            }
        }
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult TestMail()
        {
            return View();
        }
        [HttpPost]
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult TestMail(SMail sMail)
        {
            var mailConf = Intergrate.Services.Mailing.Service.Instance;
            mailConf.MailSender(sMail);
            return View();
        }
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult MailSender()
        {
            SMail sMail = new SMail("", "tienyoyoyo@gmail.com", "Hệ thống thư viện", "");//tienyoyoyo@gmail.com
            string Body = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Views/Shared/EmailTemplates/MailTemplate1.htm"));
            string name = "Đỗ Thế Sang";
            string headermess = "trễ thời hạn mượn trả sách";
            string bodymess = "Bạn đã mượn sách từ thời ông nội tôi còn sống, bây giờ đã trễ hạn 50 năm.<br>" +
                "Thư viện chúng tôi ra quyết định thu hồi sách, đồng thời yêu cầu sinh viên nộp phạt với số tiền: 100.000.000 VNĐ<br>" +
                "Vui lòng nộp tiền lại cho thư viện trước ngày 1/1/2023<br>" +
                "Sau thời hạn mà chưa nộp phạt, chúng tôi sẽ cử đội thu hồi nợ đến tới quý gia đình để giải quyết<br>" +
                "Xin trân trọng cảm ơn!";

            Body = Body.Replace("[Name]", name);
            Body = Body.Replace("[HeaderMess]", headermess);
            Body = Body.Replace("[MessBody]", bodymess);

            sMail.Body = Body;
            var mailConf = Intergrate.Services.Mailing.Service.Instance;
            mailConf.MailSender(sMail);
            logger.Info($"IP {Session["UserId"]}: Mail sended");
            return RedirectToAction("Index");
        }
        public ActionResult LibaryStats()
        {
            DBHelper db = new DBHelper();
            string sql = "select dbo.fn_ThongKeSLPhong('2022')";
            Session["Tk"] = db.ExceuteScalarString(sql, new object[] { });
            return View();
        }    
        public string StrBuilder(Dictionary<string, double[]> dics)
        {
            string template = "";
            template += string.Format(@"{label: '[Name]',name: '[Name]',tension: 0.4,borderwidth: 0,pointradius: 0,bordercolor: '#440ccb',borderwidth: 3,backgroundcolor: gradientstroke1,fill: true,showinlegend: true,legendtext: '[Name]',data:[[Data]],maxbarthickness: 6},");
            template = template.Replace("[Name]", dics.Keys.ToString());
            string dataS = "";
            foreach (var item in dics.Values)
            {
                dataS += item + ",";
            }
            dataS = dataS.Substring(0, dataS.Length - 2);
            template = template.Replace("[Data]", dataS);
            return template;
        }
        public ActionResult InventoryLibary()
        {
            return View();
        }
        public ActionResult HandleViolation()
        {
            return View();
        }
        public ActionResult AddHandleViolation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult BookLookUp(string txtSearch)
        {
            Session["NXBList"] = _services.DbContext.QueryTable<NhaXuatBan>("NhaXuatBan");
            Session["CDList"] = _services.DbContext.QueryTable<ChuDe>("ChuDe");
            IEnumerable<Sach> books = _services.DbContext.Get<Sach>("SELECT * FROM F_TimSach(N'%" + txtSearch + "%')").Where(x => _services.Db.THANHLies.Where(tl => tl.MASACH == x.MaSach).Count() <= 0).ToList();
            Session["SearchCredential"] = txtSearch;
            return View(books.Select(s => new SachDTO
                                    {
                                        MaSach = s.MaSach,
                                        TenSach = s.TenSach,
                                        AnhBia = s.AnhBia,
                                        NamXuatBan = (DateTime)s.NamXuatBan,
                                        TenNXB = _services.Db.NHAXUATBANs.Where(x => x.MANXB == s.MaNXB).Select(x => x.TENNXB).FirstOrDefault() ?? "",
                                        TenChuDe = _services.Db.CHUDEs.Where(x => x.MACHUDE == s.MaChuDe).Select(x => x.TENCHUDE).FirstOrDefault() ?? "",
                                        GiaSach = s.GiaSach,
                                        SLBS = (_services.Db.BANSAOSACHes.Where(x => x.MASACH == s.MaSach && (_services.Db.TIEUHUYs.Where(th => th.MABANSAO == x.MABANSAO)).Count() <= 0)).Count(),
                                        SLDangMuon = (_services.Db.BANSAOSACHes.Where(x => x.MASACH == s.MaSach && x.TINHTRANG)).Count()
                                    })
            );
        }

        public string[] GetViPham()
        {
            string[] viPham = { };


            return viPham;
        }

        public string[] GetMuonSach()
        {
            string[] muonSach = { };

            return muonSach;
        }
        public string[] GetTraSach()
        {
            string[] traSach = { };

            return traSach;
        }
    }
}