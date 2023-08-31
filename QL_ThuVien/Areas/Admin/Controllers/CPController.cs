using PagedList;
using QL_ThuVien.App_Start.FilterAtributes;
using QL_ThuVien.Containers;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_ThuVien.Areas.Admin.Controllers
{
    [Authorize]
    public class CPController : Controller
    {
        ServicesContainer _services;
        public CPController()
        {
            _services = ServicesContainer.Container;
        }

        // GET: Admin/CP
        [AuthorizeRole(Roles = "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            IEnumerable<TaiKhoanV2> users = _services.DbContext.Get<TaiKhoanV2>("TaiKhoan");
            ViewBag.NVList = _services.DbContext.Get<NhanVien>("NhanVien");
            return View(users.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            List<string> cvList = new List<string>() { "Nhân Viên", "Quản trị viên" };
            Session["CVList"] = new SelectList(cvList);
            Session["NVList"] = new SelectList(_services.Db.NHANVIENs, "MANHANVIEN", "HOTEN");
            //Session["NDList"] = new SelectList(_services.Db.NGUOISUDUNGs, "MANGUOISUDUNG", "HOTEN");
            return View();
        }
        [HttpPost]
        public ActionResult Create(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.MANHANVIEN == null)
                    {
                        throw new Exception("Vui lòng nhập mã nhân viên");
                    }
                    if (model.NHANVIEN != null)
                    {
                        throw new Exception("Nhân viên đã được cấp tài khoản");
                    }
                    if (_services.Db.TAIKHOANs.Any(x => x.TENDN.Equals(model.TENDN)))
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã trùng!");
                    }
                    else
                    {
                        _services.Db.TAIKHOANs.InsertOnSubmit(model);
                        _services.Db.SubmitChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }
        public ActionResult Details(int id)
        {
            IEnumerable<TaiKhoanV2> users = _services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan");
            ViewBag.NVList = _services.DbContext.Get<NhanVien>("Select * from NhanVien");
            return View(users.SingleOrDefault(x => x.MaTaiKhoan == id));
        }
        public ActionResult Edit(int id)
        {
            List<string> cvList = new List<string>() { "Nhân Viên", "Quản trị viên" };
            Session["CVList"] = new SelectList(cvList);
            return View(_services.Db.TAIKHOANs.ToList().SingleOrDefault(x => x.MATAIKHOAN == id));
        }
        [HttpPost]
        public ActionResult Edit(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                var account = _services.Db.TAIKHOANs.SingleOrDefault(x => x.MATAIKHOAN == model.MATAIKHOAN);
                //Update
                account.MANHANVIEN = model.MANHANVIEN;
                account.MATKHAU = model.MATKHAU;
                account.TENDN = model.TENDN;
                account.CHUCVU = model.CHUCVU;
                _services.Db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public string Delete(int id)
        {
            var account = _services.Db.TAIKHOANs.SingleOrDefault(x => x.MATAIKHOAN == id);
            if (account == null)
            {
                return "Không tìm thấy tài khoản!";
            }
            else
            {
                _services.Db.TAIKHOANs.DeleteOnSubmit(account);
                _services.Db.SubmitChanges();
                return "ok";
            }       
        }

        #region Thêm nhân viên
        public ActionResult CreateNV()
        {
            List<string> cvList = new List<string>() { "Nhân Viên", "Quản trị viên" };
            Session["CVList"] = new SelectList(cvList);
            Session["NVList"] = new SelectList(_services.Db.NHANVIENs, "MANHANVIEN", "HOTEN");
            return View();
        }
        [HttpPost]
        public ActionResult CreateNV(NHANVIEN model, string province, string district, string ward, string address)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.DIACHI = address + ", " + ward + ", " + district + ", " + province;
                    _services.Db.NHANVIENs.InsertOnSubmit(model);
                    _services.Db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi đã xảy ra!");
                }
            }
            return View();
        }
        #endregion

        public ActionResult EmployeeDetailsPartial()
        {
            return PartialView();
        }
        public ActionResult ReaderDetailsPartial()
        {
            return PartialView();
        }
    }
}