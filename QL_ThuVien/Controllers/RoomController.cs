using QL_ThuVien.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_ThuVien.Models;
using PagedList;
using Newtonsoft.Json;
using System.Globalization;

namespace QL_ThuVien.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private ServicesContainer _services;

        public RoomController()
        {
            _services = ServicesContainer.Container;
        }

        // GET: Room
        public ActionResult Index(int? page)
        {
            ViewBag.userList = _services.Db.THETHUVIENs.ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            IEnumerable<PHONG> rooms = _services.Db.PHONGs.ToList();
            return View(rooms.ToPagedList(pageNumber, pageSize));
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

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(PHONG phong)
        {
            if (ModelState.IsValid)
            {
                _services.Db.PHONGs.InsertOnSubmit(phong);
                _services.Db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(phong);
        }
        public ActionResult Edit(int id)
        {
            var phong = _services.Db.PHONGs.FirstOrDefault(p => p.MAPHONG == id);

            if (phong == null)
            {
                return HttpNotFound();
            }

            return View(phong);
        }

        [HttpPost]
        public ActionResult Edit(PHONG phong)
        {
            if (ModelState.IsValid)
            {
                var existingPhong = _services.Db.PHONGs.FirstOrDefault(p => p.MAPHONG == phong.MAPHONG);

                if (existingPhong != null)
                {
                    // Update existingPhong with values from phong
                    existingPhong.TENPHONG = phong.TENPHONG;
                    existingPhong.VITRI = phong.VITRI;
                    // Update other properties as needed

                    _services.Db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(phong);
        }

        public ActionResult Delete(int id)
        {
            var phong = _services.Db.PHONGs.FirstOrDefault(p => p.MAPHONG == id);

            if (phong == null)
            {
                return HttpNotFound();
            }

            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var phong = _services.Db.PHONGs.FirstOrDefault(p => p.MAPHONG == id);

            if (phong != null)
            {
                _services.Db.PHONGs.DeleteOnSubmit(phong);
                _services.Db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
        
        public ActionResult Detail(int id)
        {
            var phong = _services.Db.PHONGs.FirstOrDefault(p => p.MAPHONG == id);

            if (phong == null)
            {
                return HttpNotFound();
            }

            return View(phong);
        }

        public ActionResult AllRoom()
        {
            return View();
        }
        public ActionResult EmptyRoom()
        {
            return View();
        }
        [HttpPost]
        public string LendRoom(int maNSD, int maPhong, string tgMuon)
        {
            DateTime tStart = DateTime.Parse(tgMuon, CultureInfo.InvariantCulture);
            double tLenght = 2;
            DateTime tEnd = tStart.AddHours(tLenght);

            int result = _services.DbContext.Exceute(string.Format("exec ADD_ChiTietMuonPhong '{0}', '{1}', '{2}', '{3}'", maNSD, maPhong, tStart, tEnd));

            if (result == 0)
            {
                return "Thao tác thất bại, vui lòng kiểm tra lại!";
            }
            return "ok";
        }
    }
}