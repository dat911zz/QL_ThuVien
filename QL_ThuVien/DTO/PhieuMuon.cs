using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.DTO
{
    public class PhieuMuon
    {
        public int MAPHIEUMUON { get; set; }
        public int? MANHANVIEN { get; set; }
        public string HOTEN_NV { get; set; }
        public string SODIENTHOAI_NV { get; set; }
        public int MANSD { get; set; }
        public string HOTEN_ND { get; set; }
        public string SODIENTHOAI_ND { get; set; }
        public DateTime NGAYMUON { get; set; }
        public DateTime? NGAYTRA {  get; set; }
    }
}