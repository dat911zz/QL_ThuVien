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
    public class NotificationController : Controller
    {
        ServicesContainer _services;

        public NotificationController()
        {
            _services = ServicesContainer.Container;
        }

        // GET: Notification
        public ActionResult Index()
        {
            return View(_services.Db.THONGBAOs.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ThongBao model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //_services.Db.THONGBAOs.InsertOnSubmit(model);
                    //_services.Db.SubmitChanges();
                    var TKList = _services.DbContext.Get<TaiKhoanV2>("select * from taikhoan");

                    if (Request.Cookies[".ASPXAUTH"] != null)
                    {
                        var ticket = FormsAuthentication.Decrypt(Request.Cookies[".ASPXAUTH"].Value) ?? null;
                        if (ticket != null)
                        {
                            int mant = 0;
                            if (ticket.Name.Equals("sa"))
                            {
                                mant = 1;
                            }
                            else
                            {
                                mant = TKList.SingleOrDefault(x => x.TenDN.Equals(ticket.Name)).MaTaiKhoan;
                            }
                            var result = _services.DbContext.Exceute("Insert into ThongBao(manguoitao, tieude, noidung, thoigian) values(@mant, @tieude, @noidung, @thoigian)",
                                new
                                {
                                    mant = mant,
                                    tieude = model.TieuDe,
                                    noidung = model.NoiDung,
                                    thoigian = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
                                });
                            if (result == 0)
                            {
                                throw new Exception("Thực thi thất bại!");
                            }
                        }
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi đã xảy ra!");
                }
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            return View(_services.Db.THONGBAOs.SingleOrDefault(x => x.MATHONGBAO == id));
        }
        [HttpPost]
        public ActionResult Edit(ThongBao model)
        {
            if (ModelState.IsValid)
            {
                var notice = _services.Db.THONGBAOs.SingleOrDefault(x => x.MATHONGBAO == model.MaThongBao);
                //Update
                notice.NOIDUNG = model.NoiDung;
                notice.TIEUDE = model.TieuDe;
                _services.Db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public string Delete(int id)
        {
            try
            {
                var notice = _services.Db.THONGBAOs.SingleOrDefault(x => x.MATHONGBAO == id);
                _services.Db.THONGBAOs.DeleteOnSubmit(notice);
                _services.Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        public ActionResult Counter()
        {
            //Session["NCount"] = _services.Db.THONGBAOs.Count();
            Session["NCount"] = 150;
            return PartialView();
        }
    }
}