using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class Sach
    {
        private int maSach, maNXB, maChuDe, giaSach;
        private string tenSach, moTa, anhBia;
        private DateTime namXuatBan;

        public Sach()
        {
        }
        
        public int MaSach { get => maSach; set => maSach = value; }
        [Required(ErrorMessage = "Vui lòng chọn NXB!")]
        public int MaNXB { get => maNXB; set => maNXB = value; }
        [Required(ErrorMessage = "Vui lòng chọn Chủ đề!")]
        public int MaChuDe { get => maChuDe; set => maChuDe = value; }
        [Required(ErrorMessage = "Vui lòng chọn giá sách!")]
        public int GiaSach { get => giaSach; set => giaSach = value; }
        [Required(ErrorMessage = "Vui lòng chọn tên sách!")]
        public string TenSach { get => tenSach; set => tenSach = value; }
        [Required(ErrorMessage = "Vui lòng chọn mô tả!")]
        public string MoTa { get => moTa; set => moTa = value; }
        public string AnhBia { get => anhBia; set => anhBia = value; }
        [Required(ErrorMessage = "Vui lòng chọn năm xuất bản!")]
        public DateTime NamXuatBan { get => namXuatBan; set => namXuatBan = value; }
    }
}