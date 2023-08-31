using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using QL_ThuVien.Intergrate.Services.Data.ORM;
using QL_ThuVien.Intergrate.Services.Helper;
using QL_ThuVien.Models;

namespace QL_ThuVien.Containers
{
    public class ServicesContainer
    {
        private QLTVDataContext db;
        private Dapper dbContext;
        private DBHelper dbHelper;
        private static ServicesContainer container;

        public ServicesContainer()
        {
            string connStr = HttpContext.Current.Session["cn"]?.ToString();
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString;
            }
            else
            {
                connStr = SecurityHelper.Decrypt(connStr, "QLTHUVIEN");

            }
            this.dbContext = new Dapper(connStr);
            this.db = new QLTVDataContext(connStr);
            this.dbHelper = new DBHelper(connStr);
        }
        public static ServicesContainer Container { 
            get => container ?? new ServicesContainer(); 
            private set => container = value; 
        }
        public QLTVDataContext Db { get => db;}
        public Dapper DbContext { get => dbContext;}
        public DBHelper DbHelper { get => dbHelper;}
    }
}