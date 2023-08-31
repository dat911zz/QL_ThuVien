using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL_ThuVien.Containers;
using System.Web.Mvc;
using QL_ThuVien.Models;

namespace QL_ThuVien.Areas.UserPage.Controllers
{
    [AllowAnonymous]
    public class GioSachController : Controller
    {
        ServicesContainer _services;
        QLTVDataContext db = new QLTVDataContext();

        public GioSachController()
        {
            _services = ServicesContainer.Container;
        }

        // GET: UserPage/GioSach
        public List<GioSach> LayGioSach()
        {
            List<GioSach> lstGH = Session["GioSach"] as List<GioSach>;
            if (lstGH == null)
            {
                lstGH = new List<GioSach>();
                Session["GioSach"] = lstGH;
            }
            return lstGH;
        }
        public ActionResult Index()
        {
            var lst = Session["GioSach"] as List<GioSach>;
            ViewBag.TongCong = lst.Sum(x => x.SL);

            return View(LayGioSach());
        }
        public ActionResult ThemGioSach(int idSach, string strURL)
        {
            List<GioSach> lstGH = LayGioSach();
            GioSach sp = lstGH.Find(s => s.ThongTinSach.MASACH == idSach);
            if (sp == null)
            {
                sp = new GioSach(idSach);
                lstGH.Add(sp);
                return Redirect(strURL);
            }
            else
            {
                //sp.SL++;
                return Redirect(strURL);
            }
        }
        public ActionResult GioSachPartial()
        {
            Session["SoLuong"] = LayGioSach().Sum(x => x.SL);
            return PartialView();
        }
        public string RemoveBanSaoSach(int? id)
        {
            try
            {
                List<GioSach> lstGH = LayGioSach();
                GioSach sp = lstGH.Find(s => s.ThongTinSach.MASACH == id);
                lstGH.Remove(sp);
                Session["GioSach"] = lstGH;
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }         
        }
        public ActionResult SendForm(FormCollection frm)
        {
            try
            {
                if (frm["ttv"] == null || frm["nm"] == null)
                {
                    throw new Exception("Vui lòng nhập đầy đủ thông tin!");
                }
                PHIEUMUON pm = new PHIEUMUON();
                pm.MANSD = int.Parse(frm["ttv"]);
                pm.NGAYMUON = DateTime.Parse(frm["nm"]);            
                List<GioSach> lstGH = Session["GioSach"] as List<GioSach>;
                //Check Người này đã có trả sách hết chưa
                //if (_services.Db.PHIEUTRAs.Any(x => x.PHIEUMUON.MANSD == pm.MANSD && x.PHIEUMUON.) 
                //    )
                //{

                //}
                foreach (var item in lstGH)
                {
                    var banSaoSach = _services.Db.BANSAOSACHes.FirstOrDefault(bs => bs.MASACH == item.ThongTinSach.MASACH && bs.TINHTRANG.Equals("true"));
                    if (banSaoSach == null)
                    {
                        throw new Exception("Tất cả bản sao của sách " + item.ThongTinSach.TENSACH + " đang được mượn, vui lòng chọn sách khác!");
                    }
                    pm.CHITIETMUONSACHes.Add(new CHITIETMUONSACH() { MABANSAO = banSaoSach.MABANSAO, PHIEUMUON = pm, MASACH = banSaoSach.SACH.MASACH });
                    banSaoSach.TINHTRANG = false;
                }
                pm.NGAYTRA = pm.NGAYMUON.AddDays(7);
                _services.Db.PHIEUMUONs.InsertOnSubmit(pm);
                _services.Db.CHITIETMUONSACHes.InsertAllOnSubmit(pm.CHITIETMUONSACHes);
                _services.Db.SubmitChanges();
                lstGH.Clear();
                return Json("ok");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }         
        }
    }
}