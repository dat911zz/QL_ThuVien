using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TaiKhoanNguoiDung
    {
        private int maNguoiSuDung;
        private string hoTen;

        public TaiKhoanNguoiDung()
        {
        }

        public TaiKhoanNguoiDung(int maNguoiSuDung, string tenNguoiDung)
        {
            MaNguoiSuDung = maNguoiSuDung;
            HoTen = tenNguoiDung;
        }

        public int MaNguoiSuDung { get => maNguoiSuDung; set => maNguoiSuDung = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
    }
}