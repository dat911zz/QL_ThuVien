using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TaiKhoanV2
    {
        private int maTaiKhoan;
        private int? maNhanVien;
        private string tenDN;
        private string matKhau;
        private string chucVu;

        public TaiKhoanV2()
        {

        }

        public int MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
        public int? MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        [Required(ErrorMessage = "Vui lòng nhập tên DN!")]
        [RegularExpression(@"^([^\d].*)$", ErrorMessage = "Tên tài khoản không đúng định dạng!")]
        public string TenDN { get => tenDN; set => tenDN = value; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string MatKhau { get => matKhau; set => matKhau = value; }
        public string ChucVu { get => chucVu; set => chucVu = value; }
    }
}