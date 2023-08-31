using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TaiKhoan
    {
        private int maTaiKhoan;
        private int? maNhanVien, maNguoiSuDung;
        private string tenDN;
        private string matKhau;
        private string chucVu;

        public TaiKhoan(int maTaiKhoan, int? maNhanVien, int? maNguoiSuDung, string tenDN, string matKhau, string chucVu)
        {
            MaTaiKhoan = maTaiKhoan;
            MaNhanVien = maNhanVien;
            MaNguoiSuDung = maNguoiSuDung;
            TenDN = tenDN;
            MatKhau = matKhau;
            ChucVu = chucVu;
        }

        public int MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
        public int? MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        public int? MaNguoiSuDung { get => maNguoiSuDung; set => maNguoiSuDung = value; }
        public string TenDN { get => tenDN; set => tenDN = value; }
        public string MatKhau { get => matKhau; set => matKhau = value; }
        public string ChucVu { get => chucVu; set => chucVu = value; }
    }
}