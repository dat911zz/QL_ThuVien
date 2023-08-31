using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class ChuDe
    {
        private int maChuDe;
        private string tenChuDe;

        public ChuDe()
        {
        }

        public int MaChuDe { get => maChuDe; set => maChuDe = value; }
        public string TenChuDe { get => tenChuDe; set => tenChuDe = value; }
    }
}