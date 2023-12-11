using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class ChiTietPhieuTra
    {
        public int MaBanSao { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public int MaViPham { get; set; } = -1;
    }
}