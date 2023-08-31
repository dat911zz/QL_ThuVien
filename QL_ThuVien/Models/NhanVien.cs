using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class NhanVien
    {
        private int maNhanVien;
        private int? maTaiKhoan;
        private string hoTen;
        private DateTime ngaySinh;
        private string soDienThoai;
        private string diaChi;
        string email;
        public NhanVien()
        {

        }
        public NhanVien(int maNhanVien, int? maTaiKhoan, string hoTen, DateTime ngaySinh, string soDienThoai, string diaChi, string email)
        {
            MaNhanVien = maNhanVien;
            MaTaiKhoan = maTaiKhoan;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            SoDienThoai = soDienThoai;
            DiaChi = diaChi;
            Email = email;
        }

        public int MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        public int? MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string Email { get => email; set => email = value; }
    }
}