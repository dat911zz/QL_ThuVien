using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class NguoiSuDung
    {
        private int maNguoiSuDung;
        private string hoTen;
        private DateTime ngaySinh;
        private string soDienThoai;
        private bool nguoiNgoai;
        private string email;
        private string diaChi;
        public NguoiSuDung()
        {

        }
        public NguoiSuDung(int maNguoiSuDung, string hoTen, DateTime ngaySinh, string soDienThoai, bool nguoiNgoai, string email, string diaChi)
        {
            MaNguoiSuDung = maNguoiSuDung;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            SoDienThoai = soDienThoai;
            NguoiNgoai = nguoiNgoai;
            Email = email;
            DiaChi = diaChi;
        }

        public int MaNguoiSuDung { get => maNguoiSuDung; set => maNguoiSuDung = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public bool NguoiNgoai { get => nguoiNgoai; set => nguoiNgoai = value; }
        public string Email { get => email; set => email = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
    }
}