using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class PhieuMuon
    {
        private int maPhieuMuon, maNhanVien, maNSD;
        private DateTime ngayMuon, ngayTra;

        public int MaPhieuMuon { get => maPhieuMuon; set => maPhieuMuon = value; }
        public int MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        public int MaNSD { get => maNSD; set => maNSD = value; }
        public DateTime NgayMuon { get => ngayMuon; set => ngayMuon = value; }
        public DateTime NgayTra { get => ngayTra; set => ngayTra = value; }
    }
}