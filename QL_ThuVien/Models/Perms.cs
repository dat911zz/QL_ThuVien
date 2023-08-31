using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class Perms
    {
        private string target;
        private string dbObject;
        private int xem;
        private int xoa;
        private int them;
        private int sua;
        private int capThem;

        public string Target { get => target; set => target = value; }
        public string DbObject { get => dbObject; set => dbObject = value; }
        public int Xem { get => xem; set => xem = value; }
        public int Xoa { get => xoa; set => xoa = value; }
        public int Them { get => them; set => them = value; }
        public int Sua { get => sua; set => sua = value; }
        public int CapThem { get => capThem; set => capThem = value; }
    }
}