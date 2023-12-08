using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class SachDTO
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string AnhBia { get; set; }
        public DateTime NamXuatBan { get; set; }
        public string TenNXB { get; set; }
        public string TenChuDe { get; set; }
        public int SLBS { get; set; }
        public int SLDangMuon { get; set; }
        public string GetHtmlSLTon()
        {
            return SLBS == 0 ? "<span style=\"color:red\">Chưa nhập hàng</span>" : "<span style=\"color:green\">" + (SLBS - SLDangMuon).ToString() + " / " + SLBS.ToString() + "</span>";
        }
    }
}