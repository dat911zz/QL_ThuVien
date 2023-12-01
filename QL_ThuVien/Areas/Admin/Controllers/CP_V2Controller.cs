using Newtonsoft.Json;
using QL_ThuVien.App_Start.FilterAtributes;
using QL_ThuVien.Containers;
using QL_ThuVien.Intergrate.Services.Helper;
using QL_ThuVien.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QL_ThuVien.Areas.Admin.Controllers
{
    [Authorize]
    public class CP_V2Controller : Controller
    {
        // GET: Admin/CP_V2
        ServicesContainer _services;
        public CP_V2Controller()
        {
            _services = ServicesContainer.Container;
        }

        // GET: Admin/CP
        [AuthorizeRole(Roles = "Quản trị viên")]
        public ActionResult Index()
        {
            IEnumerable<TaiKhoanV2> users = _services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan");
            ViewBag.NVList = _services.DbContext.QueryTable<NhanVien>("NhanVien");
            return View(users);
        }
        public ActionResult Create()
        {
            List<string> cvList = new List<string>() { "Nhân viên", "Quản trị viên" };
            Session["CVList"] = new SelectList(cvList);
            Session["NVList"] = new SelectList(_services.Db.NHANVIENs, "MANHANVIEN", "HOTEN");

            IEnumerable<TaiKhoanV2> users = _services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan");
            ViewBag.NVList = _services.DbContext.QueryTable<NhanVien>("NhanVien");
            return View();
        }
        [HttpPost]
        public ActionResult Create(TaiKhoanV2 model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_services.Db.TAIKHOANs.Any(x => x.TENDN.Equals(model.TenDN)))
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã trùng!");
                    }
                    else
                    {
                        //_services.Db.TAIKHOANs.InsertOnSubmit(model);
                        //_services.Db.SubmitChanges();
                        string address = "";
                        address += collection["address"].ToString() + " ,";
                        address += collection["ward"].ToString() + " ,";
                        address += collection["district"].ToString() + " ,";
                        address += collection["province"].ToString() + ".";

                        var result = _services.DbContext.Exceute(string.Format("exec TaoTaiKhoanNhanVien N'{0}', '{1}', '{2}', N'{3}', '{4}', '{5}', '{6}', N'{7}'",
                                collection["hoten"].ToString(),
                                collection["ns"].ToString(),
                                collection["sdt"].ToString(),
                                address,
                                collection["email"].ToString(),
                                model.TenDN,
                                model.MatKhau,
                                model.ChucVu
                            ));
                        if (result == 0)
                        {
                            throw new Exception("Thao tác thất bại!");
                        }
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
            ViewBag.NVList = _services.DbContext.QueryTable<NhanVien>("NhanVien");
            return View(users.SingleOrDefault(x => x.MaTaiKhoan == id));
        }
        public ActionResult Edit(int id)
        {
            List<string> cvList = new List<string>() { "Nhân Viên", "Quản trị viên" };
            Session["CVList"] = new SelectList(cvList);
            return View(_services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan").SingleOrDefault(x => x.MaTaiKhoan == id));
        }
        [HttpPost]
        public ActionResult Edit(TaiKhoanV2 model)
        {
            if (ModelState.IsValid)
            {
                var account = _services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan").SingleOrDefault(x => x.MaTaiKhoan == model.MaTaiKhoan);
                //Update
                var result = _services.DbContext.Exceute(string.Format("exec SP_CapNhatTaiKhoan {0}, N'{1}', N'{2}', N'{3}'",

                       model.MaTaiKhoan,
                       model.TenDN,
                       model.MatKhau,
                       model.ChucVu
                    ));
                if (result == 0)
                {
                    throw new Exception("Thao tác thất bại, vui lòng kiểm tra lại!");
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public string Delete(int id)
        {
            try
            {
                var account = _services.DbContext.QueryTable<TaiKhoanV2>("TaiKhoan").SingleOrDefault(x => x.MaTaiKhoan == id);

                if (account == null)
                {
                    return "Không tìm thấy tài khoản!";
                }
                var result = _services.DbContext.Exceute("" +
                    "exec SP_XoaTaiKhoan @maTK",
                        new
                        {
                            matk = account.MaTaiKhoan,
                        }
                );
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
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
        [AuthorizeRole(Roles = "Quản trị viên")]
        public ActionResult ManagePermissions()
        {
            List<string> listTarg = new List<string>();
            listTarg.AddRange(new List<string>(_services.DbHelper.GetListString("DanhSachDoiTuong", "*")));
            Session["listTarg"] = new SelectList(listTarg);

            List<string> listObj = new List<string>();
            listObj.AddRange(new List<string>(_services.DbHelper.GetListString("Information_Schema.Tables", "TABLE_NAME")));
            listObj.AddRange(new List<string>(_services.DbHelper.GetListString("Information_Schema.Routines", "SPECIFIC_NAME")));
            Session["listObj"] = new SelectList(listObj);

            return View();
        }
        [HttpPost]

        public string ManagePermissions(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["Target"]) && !string.IsNullOrEmpty(collection["DbObject"]))
            {
                int result = 0;
                int objType = _services.DbHelper.ExceuteScalar(string.Format("select dbo.F_KiemTraBangThuTucHoacHam('{0}')", collection["DbObject"]));
                if (objType == 0 || objType == 2)
                {
                    result = _services.DbContext.Exceute(string.Format("exec CapHuyQuyenBang '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'",
                           collection["Target"].ToString(),
                           collection["DbObject"].ToString(),
                           collection["SELECT"] ?? "false",
                           collection["DELETE"] ?? "false",
                           collection["INSERT"] ?? "false",
                           collection["UPDATE"] ?? "false"
                       ));
                }
                else
                {
                    result = _services.DbContext.Exceute(string.Format("exec CapHuyQuyenHamThuTuc '{0}', '{1}', '{2}'",
                           collection["Target"].ToString(),
                           collection["DbObject"].ToString(),
                           collection["EXECUTE"] ?? "false"
                       ));
                }
                //Update

                if (result == 0)
                {
                    return "Thao tác thất bại, vui lòng kiểm tra lại!";
                }
                return "ok";
            }
            return "Vui lòng chọn đầy đủ thông tin!";
        }
        public ActionResult GetPermsOfObject(string target, string dbObject)
        {
            return Content(JsonConvert.SerializeObject(_services.DbHelper.GetListString(string.Format("dbo.FN_LayDSQuyenTrenObject('{0}', '{1}')",
                    target,
                    dbObject),
            "*")));

        }
        public ActionResult GetPermStatesOfObject(string target, string dbObject)
        {
            return Content(JsonConvert.SerializeObject(_services.DbHelper.GetListString(string.Format("dbo.FN_LayDSTTQuyenTrenObject('{0}', '{1}')",
                    target,
                    dbObject),
            "*")));
        }
        public ActionResult GetTypeOfObject(string dbObject)
        {
            return Content(_services.DbHelper.ExceuteScalar(string.Format("select dbo.F_KiemTraBangThuTucHoacHam('{0}')", dbObject)).ToString());
        }
        public string FullBackUp()
        {
            try
            {
                _services.DbContext.Exceute("exec FullBackUp");
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DiffBackUp()
        {
            try
            {
                _services.DbContext.Exceute("exec DiffBackUp");
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string LogBackUp()
        {
            try
            {
                _services.DbContext.Exceute("exec LogBackUp");
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult RestoreDataBase()
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};",
             "NGOC",
             "master",
             "sa",
             "123456");
            try
            {
                //if (name.Equals("sa"))
                //{
                //    connectionString = @"Data Source=DESKTOP-GUE0JS7;Initial Catalog=QLTHUVIEN;Integrated Security=True";
                //}
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    HttpContext.Session.Add("cn", SecurityHelper.Encrypt(connectionString, "QLTHUVIEN"));
                    _services = ServicesContainer.Container;
                    _services.DbContext.Exceute("execute RestoreDatabase");
                    conn.Close();
                    return RedirectToAction("Index", "Auth");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return RedirectToAction("BackUpDataBase", "LibCard");
            }
        }
    }
}