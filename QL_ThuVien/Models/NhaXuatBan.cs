using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class NhaXuatBan
    {
        private int maNXB;
        private string tenNXB;

        public NhaXuatBan()
        {
        }

        public int MaNXB { get => maNXB; set => maNXB = value; }
        public string TenNXB { get => tenNXB; set => tenNXB = value; }
    }
}