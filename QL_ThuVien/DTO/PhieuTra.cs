using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class PhieuTra
    {
        public int MaPhieuTra { get; set; }
        public int MaPhieuMuon { get; set; }
        public int MANSD { get; set; }
        public string HOTEN_ND { get; set; }
        public string SODIENTHOAI_ND { get; set; }
        public DateTime NGAYMUON { get; set; }
        public DateTime NgayTra { get; set; }
        public int? MANHANVIEN { get; set; }
        public string HOTEN_NV { get; set; }
        public string SODIENTHOAI_NV { get; set; }
        public List<ChiTietPhieuTra> ChiTietPhieuTras { get; set; } = new List<ChiTietPhieuTra>();
    }
}