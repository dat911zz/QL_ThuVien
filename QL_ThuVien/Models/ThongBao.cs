using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class ThongBao
    {
        private int maThongBao, maNguoiTao;
        private string tieuDe, noiDung;
        private DateTime thoiGian;

        public ThongBao()
        {
        }

        public int MaThongBao { get => maThongBao; set => maThongBao = value; }
        public int MaNguoiTao { get => maNguoiTao; set => maNguoiTao = value; }
        public string TieuDe { get => tieuDe; set => tieuDe = value; }
        public string NoiDung { get => noiDung; set => noiDung = value; }
        public DateTime ThoiGian { get => thoiGian; set => thoiGian = value; }
    }
}