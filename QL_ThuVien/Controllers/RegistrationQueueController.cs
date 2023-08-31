using QL_ThuVien.Containers;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QL_ThuVien.Controllers
{
    public class RegistrationQueueController : Controller
    {
        ServicesContainer _services;
        public RegistrationQueueController()
        {
            _services = ServicesContainer.Container;
        }
        // GET: RegistrationQueue
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BorrowBook()
        {
            return View(_services.DbContext.Get<PhieuMuon>("select * from dbo.FN_MuonSachXN()"));
        }
        public string ApproveBorrowBook(int id)
        {
            try
            {
                var NVList = _services.DbContext.QueryTable<NhanVien>("nhanvien");
                var ticket = FormsAuthentication.Decrypt(Request.Cookies[".ASPXAUTH"].Value) ?? null;
                var phieuMuon = _services.Db.PHIEUMUONs.SingleOrDefault(x => x.MAPHIEUMUON == id);
                if (phieuMuon != null)
                {
                    phieuMuon.MANHANVIEN = _services.Db.TAIKHOANs.ToList().SingleOrDefault(x => x.TENDN.Equals(ticket.Name)).MANHANVIEN;
                    _services.Db.SubmitChanges();
                }
                else
                {
                    return "Không tìm thấy phiếu mượn!";
                }
            }
            catch (Exception ex)
            {
                return "Chi tiết: " + ex.Message;
            }           
            return "ok";
        }
        public ActionResult BorrowRoom()
        {
            return View(_services.DbContext.Get<ChiTietMuonPhong>("select * from dbo.FN_PhongChoXN()"));
        }
        public ActionResult ApproveBorrowRoom(int maNSD, int maPhong, DateTime thoiGianMuon, DateTime thoiGianTra)
        {
            var NVList = _services.DbContext.QueryTable<NhanVien>("nhanvien");
            var ticket = FormsAuthentication.Decrypt(Request.Cookies[".ASPXAUTH"].Value) ?? null;
            var user = NVList.SingleOrDefault(x => x.HoTen.Equals(ticket.Name.Trim()));
            var result = _services.DbContext.Exceute("exec UpdateChiTietMuonPhong @mansd, @maphong, @manv, @thoigianmuon, @thoigiantra",
                new { 
                    mansd = maNSD,
                    maphong = maPhong,
                    manv = user != null ? user.MaNhanVien : 1,
                    thoigianmuon = thoiGianMuon,
                    thoigiantra = thoiGianTra
                });
            if (result == 0)
            {
                throw new Exception("Thao tác thất bại, vui lòng kiểm tra lại!");
            }
            return RedirectToAction("BorrowRoom");
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
    }
}