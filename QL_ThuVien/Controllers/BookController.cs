using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QL_ThuVien.App_Start.FilterAtributes;
using QL_ThuVien.Containers;
using QL_ThuVien.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using NPOI.OpenXmlFormats;
using NPOI.XWPF.UserModel;
using QL_ThuVien.DTO;
using System.Web.Security;
using NPOI.SS.Formula.Functions;

namespace QL_ThuVien.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private ServicesContainer _services;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString);
        OleDbConnection Econ;
        public BookController()
        {
            _services = ServicesContainer.Container;
        }

        // GET: Book
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult Index()
        {
            var saches = (from s in _services.Db.SACHes
                       join nxb in _services.Db.NHAXUATBANs on s.MANXB equals nxb.MANXB
                       join cd in _services.Db.CHUDEs on s.MACHUDE equals cd.MACHUDE
                       where (from tl in _services.Db.THANHLies where tl.MASACH == s.MASACH select tl).Count() <= 0
                       select new SachDTO
                       {
                           MaSach = s.MASACH,
                           TenSach = s.TENSACH,
                           AnhBia = s.ANHBIA,
                           NamXuatBan = (DateTime)s.NAMXUATBAN,
                           TenNXB = nxb.TENNXB,
                           TenChuDe = cd.TENCHUDE,
                           GiaSach = s.GIASACH,
                           SLBS = (from bs in _services.Db.BANSAOSACHes
                                   where bs.MASACH == s.MASACH && (from th in _services.Db.TIEUHUYs where th.MABANSAO == bs.MABANSAO select th).Count() <= 0
                                   select bs).Count(),
                           SLDangMuon = (from bs in _services.Db.BANSAOSACHes where bs.MASACH == s.MASACH && bs.TINHTRANG select bs).Count()
                       }).OrderByDescending(x => x.MaSach).ToList();
            return View(saches);
        }
        public ActionResult BoostrapTableTest()
        {      
            return View(_services.DbContext.QueryTable<Sach>("Sach").OrderByDescending(x => x.MaSach));
        }
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Sach model, int? slbansao, HttpPostedFileBase uploadImg)
        {
            if (ModelState.IsValid)
            {
                if(slbansao == null || slbansao <= 0 || slbansao > 1000000000)
                {
                    ModelState.AddModelError("", "Số lượng bản sao tối đa là 1000000000 và lớn hơn 0!");
                    return View();
                }
                if (model.NamXuatBan.CompareTo(DateTime.Now) > 0 || model.NamXuatBan.CompareTo(DateTime.Parse("01-01-1780")) < 0)
                {
                    ModelState.AddModelError("NamXuatBan", "Năm đã nhập không hợp lệ!");
                    return View();
                }
                else
                {
                    try
                    {
                        if (uploadImg != null && uploadImg.ContentLength > 0)
                        {
                            if (uploadImg.ContentType.ToLower().Contains("gif")
                                || uploadImg.ContentType.ToLower().Contains("jpg")
                                || uploadImg.ContentType.ToLower().Contains("jpeg")
                                || uploadImg.ContentType.ToLower().Contains("png"))
                            {
                                var filename = Path.GetFileName(uploadImg.FileName);
                                var path = Path.Combine(Server.MapPath("~/Assets/HinhAnhSP"), filename);
                                uploadImg.SaveAs(path);
                                model.AnhBia = filename;
                                SACH sInsert = new SACH
                                {
                                    TENSACH = model.TenSach,
                                    MOTA = model.MoTa,
                                    ANHBIA = model.AnhBia,
                                    MANXB = model.MaNXB,
                                    MACHUDE = model.MaChuDe,
                                    GIASACH = model.GiaSach,
                                    NAMXUATBAN = model.NamXuatBan
                                };
                                _services.Db.SACHes.InsertOnSubmit(sInsert);
                                _services.Db.SubmitChanges();
                                SACH s = _services.Db.SACHes.Where(t => t.MACHUDE == model.MaChuDe && t.MANXB == model.MaNXB &&
                                            t.TENSACH == model.TenSach && t.MOTA == model.MoTa && t.NAMXUATBAN == model.NamXuatBan &&
                                            t.GIASACH == model.GiaSach && t.ANHBIA == model.AnhBia).OrderByDescending(t => t.MASACH).FirstOrDefault();
                                if (s != null)
                                {
                                    for (int i = 0; i < slbansao; i++)
                                    {
                                        BANSAOSACH bss = new BANSAOSACH();
                                        bss.MASACH = s.MASACH;
                                        _services.Db.BANSAOSACHes.InsertOnSubmit(bss);
                                        _services.Db.SubmitChanges();
                                    }
                                }
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("", "File tải lên không hợp lệ, vui lòng thử lại!");
                                return View();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Vui lòng nhập file ảnh bìa!");
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Thêm thất bại!");
                        return View();
                    }
                }
            }
            ModelState.AddModelError("", "Vui lòng điền đẩy đủ thông tin!");
            return View();
        }
        public ActionResult CreateFromFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateFromFile(HttpPostedFileBase file)
        {
            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filepath = "/excelfolder/" + filename;
            file.SaveAs(Path.Combine(Server.MapPath("/excelfolder"), filename));
            InsertExceldata(filepath, filename);
            return View();
        }
        private void ExcelConn(string filepath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);
            Econ = new OleDbConnection(constr);
        }
        private void InsertExceldata(string filepath, string filename)
        {
            string fullpath = Server.MapPath("/excelfolder/") + filename;
            ExcelConn(fullpath);
            string query = string.Format("Select * from [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(query, Econ);
            Econ.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
            Econ.Close();
            oda.Fill(ds);
            DataTable dt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.DestinationTableName = "SACH";
            objbulk.ColumnMappings.Add("MANXB", "MANXB");
            objbulk.ColumnMappings.Add("MACHUDE", "MACHUDE");
            objbulk.ColumnMappings.Add("TENSACH", "TENSACH");
            objbulk.ColumnMappings.Add("MOTA", "MOTA");
            objbulk.ColumnMappings.Add("NAMXUATBAN", "NAMXUATBAN");
            objbulk.ColumnMappings.Add("GIASACH", "GIASACH");
            objbulk.ColumnMappings.Add("ANHBIA", "ANHBIA");
            con.Open();
            objbulk.WriteToServer(dt);
            con.Close();
        }
        public ActionResult ExportFromFile()
        {
            string query = "select * From SACH";
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();
            IList<Sach> model = new List<Sach>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                model.Add(new Sach()
                {
                    MaNXB = Convert.ToInt32(dt.Rows[i]["MANXB"]),
                    MaChuDe = Convert.ToInt32(dt.Rows[i]["MACHUDE"]),
                    TenSach = dt.Rows[i]["TENSACH"].ToString(),
                    MoTa = dt.Rows[i]["MOTA"].ToString(),
                    NamXuatBan = Convert.ToDateTime(dt.Rows[i]["NAMXUATBAN"]),
                    GiaSach = Convert.ToInt32(dt.Rows[i]["GIASACH"]),
                    AnhBia = dt.Rows[i]["ANHBIA"].ToString(),
                });
            }
            return View(model);
        }
        public ActionResult ResultExportToExcel()
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook xlWorkBook = excelApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
            excelApp.Visible = false;
            object misValue = System.Reflection.Missing.Value;
            excelApp = new Excel.Application();
            xlWorkBook = (Excel.Workbook)(excelApp.Workbooks.Add(System.Reflection.Missing.Value));
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.ActiveSheet;
            List<string> colNameList = _services.DbHelper.GetColNameList("Sach");
            for (int i = 0; i < colNameList.Count; i++)
            {
                xlWorkSheet.Cells[1, i + 1] = colNameList[i];
            }           
            char lastColumn = (char)(65 + colNameList.Count - 1);
            xlWorkSheet.get_Range("A1", lastColumn + "1").Font.Bold = true;
            xlWorkSheet.get_Range("A1", lastColumn + "1").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            DataSet ds = new DataSet();
            _services.DbHelper.FillData(ds, "Sach");
            for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                {
                    xlWorkSheet.Cells[i + 2, j + 1] = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                }
            }
            xlWorkBook.Close(true, misValue, misValue);
            excelApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(excelApp);
            return RedirectToAction("ExportFromFile", "Book");
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
                //MessageBox.Show("Exception Occured while releasing object " + ex.ToString());  
            }
            finally
            {
                GC.Collect();
            }
        }
        [HttpPost]
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]

        public string Delete(int id, string dongia)
        {
            try
            {
                double dg;
                if (string.IsNullOrEmpty(dongia) || !double.TryParse(dongia, out dg))
                    return "Hệ thống không nhận được đơn giá thanh lý";
                if (dg < 0 || dg > 1000000000)
                    return "Đơn giá thanh lý bất thường";
                var cookie = HttpContext.Request.Cookies[".ASPXAUTH"];
                if (cookie == null)
                {
                    Session.Clear();
                    return "Vui lòng xác thực đăng nhập lại";
                }
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                int manv = (int)_services.Db.TAIKHOANs.Where(x => x.TENDN == ticket.Name).First().MANHANVIEN;
                var sach = (from s in _services.Db.SACHes where s.MASACH == id && (from tl in _services.Db.THANHLies where tl.MASACH == s.MASACH select tl).Count() <= 0 select s).FirstOrDefault();
                if (sach == null)
                {
                    return "Không tìm thấy sách có mã " + id;
                }
                var bansaos = (from bs in _services.Db.BANSAOSACHes where bs.MASACH == sach.MASACH select bs).ToList();
                if(bansaos.Where(x => x.TINHTRANG).Count() > 0)
                {
                    return "Hiện tại sách đang có bản sao được mượn, không thể thanh lý toàn bộ";
                }
                try
                {
                    _services.Db.THANHLies.InsertOnSubmit(new THANHLY
                    {
                        MANHANVIEN = manv,
                        MASACH = sach.MASACH,
                        NGAY = DateTime.Now,
                        SOLUONG = bansaos.Count(),
                        DONGIA = dg
                    });
                    _services.Db.SubmitChanges();
                }
                catch (Exception e)
                {
                    return "Truy cập cơ sở dữ liệu thất bại";
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }          
        }
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult Edit(int id)
        {
            var book = _services.Db.SACHes.SingleOrDefault(x => x.MASACH == id);
            return View(book);
        }
        [HttpPost]
        [AuthorizeRole(Roles = "Quản trị viên, Nhân Viên")]
        public ActionResult Edit(SACH model, HttpPostedFileBase uploadImg)
        {
            if (model.NAMXUATBAN.CompareTo(DateTime.Now) > 0 || model.NAMXUATBAN.CompareTo(DateTime.Parse("01-01-1780")) < 0)
            {
                ModelState.AddModelError("NAMXUATBAN", "Năm đã nhập không hợp lệ!");
                return View(model);
            }
            if (model.GIASACH == null || model.GIASACH > 1000000000)
            {
                ModelState.AddModelError("GIASACH", "Giá sách không hợp lệ!");
                return View(model);
            }
            if (model.TENSACH == null)
            {
                ModelState.AddModelError("TENSACH", "Tên sách không được bỏ trống!");
                return View(model);
            }
            if (model.MOTA == null)
            {
                ModelState.AddModelError("MOTA", "Mô tả không được bỏ trống!");
                return View(model);
            }
            if (ModelState.IsValid)
            {              
                var book = _services.Db.SACHes.SingleOrDefault(x => x.MASACH == model.MASACH);
                if (uploadImg != null && uploadImg.ContentLength > 0)
                {
                    if (uploadImg.ContentType.ToLower().Contains("gif")
                        || uploadImg.ContentType.ToLower().Contains("jpg")
                        || uploadImg.ContentType.ToLower().Contains("jpeg")
                        || uploadImg.ContentType.ToLower().Contains("png"))
                    {
                        var filename = Path.GetFileName(uploadImg.FileName);
                        var newPath = Path.Combine(Server.MapPath("~/Assets/HinhAnhSP"), filename);
                        if (!filename.Equals(model.ANHBIA))
                        {               
                            var oldPath = Path.Combine(Server.MapPath("~/Assets/HinhAnhSP"), book.ANHBIA);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                            uploadImg.SaveAs(newPath);                            
                            book.ANHBIA = filename;
                        }
                        //Update model
                        book.MANXB = model.MANXB;
                        book.MACHUDE = model.MACHUDE;
                        book.TENSACH = model.TENSACH;
                        book.MOTA = model.MOTA;
                        book.NAMXUATBAN = model.NAMXUATBAN;
                        book.GIASACH = model.GIASACH;
                        _services.Db.SubmitChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "File tải lên không hợp lệ, vui lòng thử lại!");
                    }
                }
                else
                {
                    //Update model without image
                    book.MANXB = model.MANXB;
                    book.MACHUDE = model.MACHUDE;
                    book.TENSACH = model.TENSACH;
                    book.MOTA = model.MOTA;
                    book.NAMXUATBAN = model.NAMXUATBAN;
                    book.GIASACH = model.GIASACH;
                    _services.Db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var book = _services.Db.SACHes.SingleOrDefault(x => x.MASACH == id);
            return View(book);
        }

        public ActionResult BanSao(int id)
        {
            var listBS = (from bs in _services.Db.BANSAOSACHes
                          where bs.MASACH == id && (from th in _services.Db.TIEUHUYs where th.MABANSAO == bs.MABANSAO select th).Count() <= 0
                          select bs).ToList();
            return View(listBS);
        }

        [HttpGet]
        public string CountSLBS(int id)
        {
            int total = _services.Db.BANSAOSACHes.Where(bs => bs.MASACH == id).Count();
            int totalInUse = _services.Db.BANSAOSACHes.Where(bs => bs.MASACH == id && bs.TINHTRANG).Count();
            string rs = total == 0 ? "0" : (total - totalInUse).ToString() + " / " + total.ToString();
            return rs;

        }
        public ActionResult BorrowBook()
        {
            List<THETHUVIEN> thes = _services.Db.THETHUVIENs.ToList();
            ViewData["TheThuViens"] = thes;
            return View();
        }

        [HttpPost]
        public ActionResult BorrowBook(int the_thu_vien)
        {
            string res = "";
            List<BANSAOSACH> carts = Session["cartBooks"] as List<BANSAOSACH>;
            var cookie = HttpContext.Request.Cookies[".ASPXAUTH"];
            if (cookie == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Auth");
            }
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            if (carts.Count > 0)
            {
                int manv = (int)_services.Db.TAIKHOANs.Where(x => x.TENDN == ticket.Name).First().MANHANVIEN;
                DateTime ngaymuon = DateTime.Now;
                _services.Db.PHIEUMUONs.InsertOnSubmit(new PHIEUMUON() { MANHANVIEN = manv, MANSD = the_thu_vien, NGAYMUON = ngaymuon, NGAYTRA = null });
                _services.Db.SubmitChanges();
                PHIEUMUON pm = _services.Db.PHIEUMUONs.Where(x => x.MANHANVIEN == manv && x.MANSD == the_thu_vien && x.NGAYMUON == ngaymuon && x.NGAYTRA == null).OrderByDescending(x => x.MAPHIEUMUON).First();
                if(pm != null)
                {
                    int i = 0;
                    foreach (var item in carts)
                    {
                        BANSAOSACH bs = _services.Db.BANSAOSACHes.Where(b => b.MABANSAO == item.MABANSAO).FirstOrDefault();
                        if (bs != null && !bs.TINHTRANG)
                        {
                            try
                            {
                                bs.TINHTRANG = true;
                                _services.Db.CHITIETMUONSACHes.InsertOnSubmit(new CHITIETMUONSACH() { MAPHIEUMUON = pm.MAPHIEUMUON, MABANSAO = bs.MABANSAO, MASACH = bs.MASACH });
                                _services.Db.SubmitChanges();
                                i++;
                            }
                            catch { }
                        }
                    }
                    if(i == carts.Count)
                    {
                        res = "Lập phiếu mượn thành công";
                    }
                    else
                    {
                        res = "Lập phiếu mượn thành công, nhưng có sách lỗi";
                    }
                    Session["cartBooks"] = null;
                }
                else
                {
                    res = "Lập phiếu mượn thất bại";
                }
            }
            else
            {
                res = "Không có sách để lập phiếu mượn";
            }
            Session["borrowBooks"] = res;
            return Redirect("BorrowedBook");
        }

        public ActionResult BorrowedBook()
        {
            var list = (from pm in _services.Db.PHIEUMUONs 
                        join nv in _services.Db.NHANVIENs on pm.MANHANVIEN equals nv.MANHANVIEN 
                        join ttv in _services.Db.THETHUVIENs on pm.MANSD equals ttv.MATTV
                        select new DTO.PhieuMuon
                        {
                            MAPHIEUMUON = pm.MAPHIEUMUON,
                            MANHANVIEN = pm.MANHANVIEN,
                            MANSD = pm.MANSD,
                            NGAYMUON = pm.NGAYMUON,
                            NGAYTRA = pm.NGAYTRA,
                            HOTEN_NV = nv.HOTEN,
                            SODIENTHOAI_NV = nv.SODIENTHOAI,
                            HOTEN_ND = ttv.HOTEN,
                            SODIENTHOAI_ND = ttv.SODIENTHOAI,
                            ChiTietPhieuMuons = (from s in _services.Db.SACHes
                                                 join bs in _services.Db.BANSAOSACHes on s.MASACH equals bs.MABANSAO
                                                 join cttpm in _services.Db.CHITIETMUONSACHes on bs.MABANSAO equals cttpm.MABANSAO
                                                 where cttpm.MAPHIEUMUON == pm.MAPHIEUMUON
                                                 select new ChiTietPhieuMuon
                                                 {
                                                     MaBanSao = bs.MABANSAO,
                                                     TenSach = s.TENSACH
                                                 }).ToList()
                        }).ToList();
            return View(list);
        }

        public ActionResult DetailsBorrowedBook(int maphieumuon)
        {
            var phieumuon = (from pm in _services.Db.PHIEUMUONs where pm.MAPHIEUMUON == maphieumuon
                        join nv in _services.Db.NHANVIENs
                            on pm.MANHANVIEN equals nv.MANHANVIEN
                        join ttv in _services.Db.THETHUVIENs
                            on pm.MANSD equals ttv.MATTV
                        select new DTO.PhieuMuon
                        {
                            MAPHIEUMUON = pm.MAPHIEUMUON,
                            MANHANVIEN = pm.MANHANVIEN,
                            MANSD = pm.MANSD,
                            NGAYMUON = pm.NGAYMUON,
                            NGAYTRA = pm.NGAYTRA,
                            HOTEN_NV = nv.HOTEN,
                            SODIENTHOAI_NV = nv.SODIENTHOAI,
                            HOTEN_ND = ttv.HOTEN,
                            SODIENTHOAI_ND = ttv.SODIENTHOAI,
                            ChiTietPhieuMuons = (from s in _services.Db.SACHes
                                                 join bs in _services.Db.BANSAOSACHes on s.MASACH equals bs.MASACH
                                                 join cttpm in _services.Db.CHITIETMUONSACHes on bs.MABANSAO equals cttpm.MABANSAO
                                                 where cttpm.MAPHIEUMUON == pm.MAPHIEUMUON
                                                 select new ChiTietPhieuMuon
                                                 {
                                                     MaBanSao = bs.MABANSAO,
                                                     TenSach = s.TENSACH
                                                 }).ToList()
                        }).FirstOrDefault();
            if (phieumuon == null)
                throw new Exception("Không tìm thấy phiếu mượn");
            return View(phieumuon);
        }
        public ActionResult EditBorrowedBook(int maphieumuon)
        {
            var phieumuon = (from pm in _services.Db.PHIEUMUONs
                             where pm.MAPHIEUMUON == maphieumuon
                             join nv in _services.Db.NHANVIENs
                                 on pm.MANHANVIEN equals nv.MANHANVIEN
                             join ttv in _services.Db.THETHUVIENs
                                 on pm.MANSD equals ttv.MATTV
                             select new
                             {
                                 pm,
                                 nv,
                                 ttv
                             }).FirstOrDefault();
            if (phieumuon == null)
                throw new Exception("Không tìm thấy phiếu mượn");
            ViewData["NhanViens"] = _services.Db.NHANVIENs.ToList();
            ViewData["NguoiDungs"] = _services.Db.THETHUVIENs.ToList();
            return View(new DTO.PhieuMuon
            {
                MAPHIEUMUON = phieumuon.pm.MAPHIEUMUON,
                MANHANVIEN = phieumuon.pm.MANHANVIEN,
                MANSD = phieumuon.pm.MANSD,
                NGAYMUON = phieumuon.pm.NGAYMUON,
                NGAYTRA = phieumuon.pm.NGAYTRA,
                HOTEN_NV = phieumuon.nv.HOTEN,
                SODIENTHOAI_NV = phieumuon.nv.SODIENTHOAI,
                HOTEN_ND = phieumuon.ttv.HOTEN,
                SODIENTHOAI_ND = phieumuon.ttv.SODIENTHOAI
            });
        }
        [HttpPost]
        public ActionResult EditBorrowedBook(DTO.PhieuMuon model)
        {
            if (ModelState.IsValid)
            {
                if (model.NGAYMUON == null)
                {
                    ModelState.AddModelError("", "Ngày mượn không được bỏ trống");
                    return View();
                }
                else
                {
                    try
                    {
                        PHIEUMUON pm = _services.Db.PHIEUMUONs.Where(p => p.MAPHIEUMUON == model.MAPHIEUMUON).FirstOrDefault();
                        pm.MANSD = model.MANSD;
                        pm.NGAYMUON = model.NGAYMUON;
                        pm.NGAYTRA = model.NGAYTRA;
                        _services.Db.SubmitChanges();
                        Session["EditBorrowedBook"] = true;
                        return Redirect("BorrowedBook");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Cập nhật thất bại!");
                        return View();
                    }
                }
            }
            ModelState.AddModelError("", "Vui lòng điền đẩy đủ thông tin!");
            return View();
        }

        public ActionResult SendBackBook()
        {
            return View();
        }
        public ActionResult SearchBooks()
        {
            return View();
        }

        public PartialViewResult CartBooks()
        {
            List<BANSAOSACH> carts = Session["cartBooks"] as List<BANSAOSACH>;
            if (carts == null)
            {
                carts = new List<BANSAOSACH>();
            }
            Session["cartBooks"] = carts;
            return PartialView("CartBooks", carts);
        }

        [HttpPost]
        public bool AddCartBooks(string mabs, string mas) {
            List<BANSAOSACH> carts = Session["cartBooks"] as List<BANSAOSACH>;
            if(carts == null)
            {
                carts = new List<BANSAOSACH>();
            }
            if(carts.FirstOrDefault(x => x.MABANSAO == int.Parse(mabs)) != null)
                return false;
            carts.Add(new BANSAOSACH { MABANSAO = int.Parse(mabs), MASACH = int.Parse(mas) });
            Session["cartBooks"] = carts;
            return true;
        }
        [HttpPost]
        public bool RemoveCartBooks(string mabs, string mas)
        {
            List<BANSAOSACH> carts = Session["cartBooks"] as List<BANSAOSACH>;
            if (carts == null)
            {
                carts = new List<BANSAOSACH>();
            }
            var iFind = carts.FirstOrDefault(x => x.MABANSAO == int.Parse(mabs));
            if (iFind == null)
                return false;
            carts.Remove(iFind);
            Session["cartBooks"] = carts;
            return true;
        }
        
        public void ClearCartBooks()
        {
            Session["cartBooks"] = new List<BANSAOSACH>();
        }

        [HttpPost]
        public string DestroyBook(string mabs, string lydo)
        {
            int mabansao;
            if (string.IsNullOrEmpty(mabs) || !int.TryParse(mabs, out mabansao))
                return JsonConvert.SerializeObject(new { res = false, msg = "Hệ thống không nhận được mã bản sao" });
            var obj = _services.Db.TIEUHUYs.Where(x => x.MABANSAO == mabansao).FirstOrDefault();
            if(obj != null)
                return JsonConvert.SerializeObject(new { res = false, msg = string.Format("Bản sao này đã hủy trước đó\n- Thời gian: {0}\n- Lý do: {1}", obj.NGAY?.ToString("dd/MM/yyyy HH:mm:ss"), obj.LYDO) });
            else if(_services.Db.BANSAOSACHes.Where(x => x.MABANSAO == mabansao && x.TINHTRANG).FirstOrDefault() != null)
                return JsonConvert.SerializeObject(new { res = false, msg = "Sách đang được mượn. Không được hủy." });
            if (string.IsNullOrEmpty(lydo))
                return JsonConvert.SerializeObject(new { res = false, msg = "Lý do hủy bản sao đang trống" });
            var cookie = HttpContext.Request.Cookies[".ASPXAUTH"];
            if (cookie == null)
            {
                Session.Clear();
                return JsonConvert.SerializeObject(new { res = false, msg = "Vui lòng xác thực đăng nhập lại" });
            }
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            int manv = (int)_services.Db.TAIKHOANs.Where(x => x.TENDN == ticket.Name).First().MANHANVIEN;
            DateTime d = DateTime.Now;
            try
            {
                _services.Db.TIEUHUYs.InsertOnSubmit(new TIEUHUY
                {
                    MABANSAO = mabansao,
                    MANHANVIEN = manv,
                    NGAY = d,
                    LYDO = lydo
                });
                _services.Db.SubmitChanges();
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { res = false, msg = "Truy cập cơ sở dữ liệu thất bại" });
            }
            return JsonConvert.SerializeObject(new { res = true, msg = "Tiêu hủy bản sao thành công" });
        }

        public ActionResult DestroyedBook()
        {
            var list = (from th in _services.Db.TIEUHUYs
                        join nv in _services.Db.NHANVIENs on th.MANHANVIEN equals nv.MANHANVIEN
                        join bs in _services.Db.BANSAOSACHes on th.MABANSAO equals bs.MABANSAO
                        join s in _services.Db.SACHes on bs.MASACH equals s.MASACH
                        select new BanSaoDestroy
                        {
                            MaSach = s.MASACH,
                            TenSach = s.TENSACH,
                            MaBanSao = bs.MABANSAO,
                            MANHANVIEN = nv.MANHANVIEN,
                            HOTEN_NV = nv.HOTEN,
                            SODIENTHOAI_NV = nv.SODIENTHOAI,
                            NgayTieuHuy = (DateTime)th.NGAY,
                            LyDoTieuHuy = th.LYDO
                        }).ToList();
            return View(list);
        }
    }
}