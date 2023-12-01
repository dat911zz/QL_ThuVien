using QL_ThuVien.App_Start.FilterAtributes;
using QL_ThuVien.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_ThuVien.Areas.Admin.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        ServicesContainer _services;

        public EmployeesController() {
            _services = ServicesContainer.Container;
        }
        // GET: Admin/Employees
        [AuthorizeRole(Roles = "Quản trị viên")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Employees/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
