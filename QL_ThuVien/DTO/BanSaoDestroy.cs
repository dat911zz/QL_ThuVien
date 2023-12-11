using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class BanSaoDestroy
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public int MaBanSao { get; set; }
        public int? MANHANVIEN { get; set; }
        public string HOTEN_NV { get; set; }
        public string SODIENTHOAI_NV { get; set; }
        public DateTime NgayTieuHuy { get; set; }
        public string LyDoTieuHuy { get; set; }
    }
}