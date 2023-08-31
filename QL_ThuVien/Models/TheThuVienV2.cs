using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TheThuVienV2
    {
        private int maTTV;
        private string hoTen, soDienThoai;
        private bool nguoiNgoai;
        private string email, diaChi;
        private DateTime ngayCap, ngayHetHan,ngaySinh;

        public TheThuVienV2()
        {

        }

        public int MaTTV { get => maTTV; set => maTTV = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public bool NguoiNgoai { get => nguoiNgoai; set => nguoiNgoai = value; }
        public string Email { get => email; set => email = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public DateTime NgayCap { get => ngayCap; set => ngayCap = value; }
        public DateTime NgayHetHan { get => ngayHetHan; set => ngayHetHan = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
    }
}