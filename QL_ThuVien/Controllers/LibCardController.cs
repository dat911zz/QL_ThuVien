using QL_ThuVien.Containers;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QL_ThuVien.Intergrate.Services.Helper;
using System.Data.SqlClient;
using System.Globalization;

namespace QL_ThuVien.Controllers
{
    public class LibCardController : Controller
    {
        ServicesContainer _services;

        public LibCardController()
        {
            _services = ServicesContainer.Container;
        }
        // GET: LibCard
        public ActionResult Index()
        {
            return View(_services.DbContext.Get<TheThuVienV2>("select * from FN_DanhSachTTV ()"));
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(THETHUVIEN model)
        {
            var result = 1;
            _services.Db.THETHUVIENs.InsertOnSubmit(model);
            _services.Db.SubmitChanges();
            if (result == 0)
            {
                throw new Exception("Thao tác thất bại, vui lòng kiểm tra lại!");
            }
            return View();
        }
        public string Delete(int id)
        {
            try
            {
                var ttv = _services.Db.THETHUVIENs.FirstOrDefault(x => x.MATTV == id);
                _services.Db.THETHUVIENs.DeleteOnSubmit(ttv);
                _services.Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public ActionResult Edit(int id)
        {
            IEnumerable<THETHUVIEN> ttv = _services.DbContext.Get<THETHUVIEN>("Select * from THETHUVIEN");
            return View(ttv.SingleOrDefault(x => x.MATTV == id));
        }
        [HttpPost]
        public ActionResult Edit(THETHUVIEN model)
        {
            int nn = model.NGUOINGOAI == true ? 1 : 0;
            string nC = model.NGAYCAP.ToShortDateString();
            string nHH = model.NGAYHETHAN.ToShortDateString();
            if (ModelState.IsValid)
            {
                //var result = _services.DbContext.Exceute("exec CapNhat_TheThuVien @maTTV N'@hoTen', '@SDT', @nguoiNgoai, '@email', '@diaChi', '@ngayCap', '@ngayHetHan'",
                //    new
                //    {
                //        hoTen = model.HOTEN,
                //        SDT = model.SODIENTHOAI,
                //        nguoiNgoai = nn,
                //        email = model.EMAIL,
                //        diaChi = model.DIACHI,
                //        ngayCap = DateTime.Parse(nC, new CultureInfo("en-CA")),
                //        ngayHetHan = DateTime.Parse(nHH, new CultureInfo("en-CA"))
                //    });

                THETHUVIEN ttv = _services.Db.THETHUVIENs.FirstOrDefault(x => x.MATTV == model.MATTV);
                ttv.HOTEN = model.HOTEN;
                ttv.SODIENTHOAI = model.SODIENTHOAI;
                ttv.NGUOINGOAI = model.NGUOINGOAI;
                ttv.EMAIL = model.EMAIL;
                ttv.DIACHI = model.DIACHI;
                ttv.NGAYCAP = model.NGAYCAP;
                ttv.NGAYHETHAN = model.NGAYHETHAN;

                _services.Db.SubmitChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}