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
        public ActionResult AllRoom()
        {
            return View();
        }
        public ActionResult EmptyRoom()
        {
            return View();
        }
        public ActionResult LendRoom()
        {
            return View();
        }
    }
}