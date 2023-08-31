using QL_ThuVien.Containers;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using System.Globalization;
using PagedList;

namespace QL_ThuVien.Areas.UserPage.Controllers
{
    [AllowAnonymous]
    public class FormRegistrationController : Controller
    {
        ServicesContainer _services;
        // GET: UserPage/FormRegistration
        public FormRegistrationController()
        {
            _services = ServicesContainer.Container;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LibCard()
        {
            return View();
        }
        public ActionResult BorrowBook()
        {
            Session["NXBList"] = _services.DbContext.Get<NhaXuatBan>("Select * from NhaXuatBan");
            Session["CDList"] = _services.DbContext.Get<ChuDe>("Select * from ChuDe");
            IEnumerable<Sach> books = _services.DbContext.Get<Sach>("SELECT * FROM SACH");
            return View(books);
        }
        [HttpGet]
        public ActionResult BorrowRoom()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BorrowRoom(ChiTietMuonPhong model, DateTime gioBD, double thoiLuong)
        {
            if (ModelState.IsValid)
            {

                DateTime tgm = new DateTime(
                    model.ThoiGianMuon.Year,
                    model.ThoiGianMuon.Month,
                    model.ThoiGianMuon.Day,
                    gioBD.Hour,
                    gioBD.Minute,
                    gioBD.Second
                    );
                model.ThoiGianMuon = tgm;
                model.ThoiGianTra = tgm.AddHours(thoiLuong);

                var NVList = _services.DbContext.Get<NhanVien>("select * from nhanvien");
                var result = _services.DbContext.Exceute("exec ADD_ChiTietMuonPhong @mansd, @maphong, @thoigianmuon, @thoigiantra",
                    new
                    {
                        mansd = model.MaNSD,
                        maphong = model.MaPhong,
                        thoigianmuon = model.ThoiGianMuon.GetDateTimeFormats()[42],
                        thoigiantra = model.ThoiGianTra.GetDateTimeFormats()[42]
                    });
                if (result == 0)
                {
                    throw new Exception("Thao tác thất bại, vui lòng kiểm tra lại!");
                }
                return RedirectToAction("Index", "V1");

            }        
            return View();
        }
        public ActionResult GetEmployeeName(int id)
        {
            return Content(_services.DbContext.QuerySingle(String.Format("select dbo.LAY_TEN_NV({0})", id)));
        }
        public ActionResult GetUserName(int id)
        {
            return Content(_services.DbContext.QuerySingle(String.Format("select dbo.LAY_TEN_ND({0})", id)));
        }
        public ActionResult GetRoomName(int id)
        {
            return Content(_services.DbContext.QuerySingle(String.Format("select dbo.LAY_TEN_PHONG({0})", id)));
        }
        public ActionResult GetRoomId(int id)
        {
            return Content(_services.DbContext.QuerySingle(String.Format("select dbo.LAY_MA_PHONG({0})", id)));
        }
        public ActionResult GetEmptyRooms(string dateTimeSend, double hourUse)
        {
            DateTime tStart = DateTime.Parse(dateTimeSend, CultureInfo.InvariantCulture);
            double tLenght = hourUse;
            DateTime tEnd = tStart.AddHours(tLenght);
            return Content(JsonConvert.SerializeObject(_services.DbContext.Get<string>(String.Format("select * from F_TimPhongTrongTrongKhoangThoiGianAB ('{0}','{1}')",
                tStart.GetDateTimeFormats()[42],
                tEnd.GetDateTimeFormats()[42]
                ))
            ));
        }
    }
}