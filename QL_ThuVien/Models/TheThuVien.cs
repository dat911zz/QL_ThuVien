using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TheThuVien
    {
        private int maNguoiSuDung;
        private int? maTaiKhoan;
        private DateTime ngayCap, ngayHetHan;

        public TheThuVien(int maNguoiSuDung, int? maTaiKhoan, DateTime ngayCap, DateTime ngayHetHan)
        {
            MaNguoiSuDung = maNguoiSuDung;
            MaTaiKhoan = maTaiKhoan;
            NgayCap = ngayCap;
            NgayHetHan = ngayHetHan;
        }

        public int MaNguoiSuDung { get => maNguoiSuDung; set => maNguoiSuDung = value; }
        public int? MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
        public DateTime NgayCap { get => ngayCap; set => ngayCap = value; }
        public DateTime NgayHetHan { get => ngayHetHan; set => ngayHetHan = value; }
    }
}