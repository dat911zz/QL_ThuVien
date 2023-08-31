using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class GioSach
    {
        Models.QLTVDataContext db = new QLTVDataContext();
        public SACH ThongTinSach { get; set; }
        public int SL { get; set; }
        public GioSach(int idSach)
        {
            ThongTinSach = db.SACHes.Single(s => s.MASACH == idSach);
            SL = 1;
        }
    }
}