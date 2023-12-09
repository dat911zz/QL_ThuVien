using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class LiquidatedBookDTO
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string AnhBia { get; set; }
        public DateTime NamXuatBan { get; set; }
        public string TenNXB { get; set; }
        public string TenChuDe { get; set; }
        public DateTime NgayThanhLy { get; set; }
        public int GiaThanhLy { get; set; }
        public int SoLuongThanhLy { get; set; }
    }
}