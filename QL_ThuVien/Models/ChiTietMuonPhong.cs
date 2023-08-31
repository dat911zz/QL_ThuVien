using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class ChiTietMuonPhong
    {
        private int maNSD, maPhong, maNhanVien;
        private DateTime thoiGianMuon, thoiGianTra;

        public ChiTietMuonPhong()
        {
        }

        public int MaNSD { get => maNSD; set => maNSD = value; }
        public int MaPhong { get => maPhong; set => maPhong = value; }
        public int MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        public DateTime ThoiGianMuon { get => thoiGianMuon; set => thoiGianMuon = value; }
        public DateTime ThoiGianTra { get => thoiGianTra; set => thoiGianTra = value; }
    }
}