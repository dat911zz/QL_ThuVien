using System;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Configuration;

namespace QL_ThuVien.Intergrate.Services.Data.ORM
{
    public class Dapper
    {
        private readonly string conStr;

        public Dapper()
        {
            this.conStr = ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString;
        }

        public Dapper(string conStr)
        {
            this.conStr = conStr;
        }

        public List<T> Get<T>(string sql)
        {
            List<T> listModel = new List<T>();
            using (var conn = new SqlConnection(conStr))
            {
                listModel = conn.Query<T>(sql).AsList();
            }
            return listModel;
        }

        public List<T> QueryTable<T>(string tbname)
        {
            List<T> listModel = new List<T>();
            using (var conn = new SqlConnection(conStr))
            {
                listModel = conn.Query<T>("Select * from " + tbname).AsList();
            }
            return listModel;
        }
        public void ExecuteProc(string procname, params object[] objs)
        {
            using (var conn = new SqlConnection(conStr))
            {
                conn.Execute(procname, objs, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        /// <summary>
        /// Excecute query into table with Insert, Update, Delete
        /// </summary>
        /// <param name="sql">Sql command</param>
        /// <param name="objs">Parameters</param>
        /// <returns>Number of row affected</returns>
        public int Exceute(string sql, params object[] objs)
        {
            int result = 0;
            using (var conn = new SqlConnection(conStr))
            {
                if (objs.Length != 0)
                {
                    result = conn.Execute(sql, objs);
                }
                else
                {
                    result = conn.Execute(sql);
                }
            }
            return result;
        }
        /// <summary>
        /// Execute Procedure, Function that return single value
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objs"></param>
        /// <returns>Return single cell with object type</returns>
        public string ExecuteScarlar(string sql, params object[] objs)
        {
            string result;
            using (var conn = new SqlConnection(conStr))
            {
                result = conn.ExecuteScalar(sql, objs).ToString();
            }
            return result;
        }
        public string QuerySingle(string sql)
        {
            using (var conn = new SqlConnection(conStr))
            {
                return conn.Query<string>(sql).First();
            }
        }
        public int Add<T>(T model)
        {
            int result = 0;
            string cmd = "";
            //Build Insert command
            var propertiesWithAttribute = typeof(T).GetProperties().ToList();
            cmd += "Set dateformat DMY ";
            cmd += "Insert into " + model.GetType().Name;
            cmd += "(";
            for (int i = 1; i < propertiesWithAttribute.Count; i++)
            {
                cmd += "" + propertiesWithAttribute[i].Name + ",";
            }
            cmd = cmd.Substring(0, cmd.Length - 1);
            cmd += ")";

            cmd += " values(";
            for (int i = 1; i < propertiesWithAttribute.Count; i++)
            {
                if (propertiesWithAttribute[i].PropertyType.Name == typeof(DateTime).Name)
                {
                    cmd += "'" + ((DateTime)propertiesWithAttribute[i].GetValue(model)).ToShortDateString() + "',";
                }
                else
                {
                    cmd += "'" + propertiesWithAttribute[i].GetValue(model) + "',";
                }
            }
            cmd = cmd.Substring(0, cmd.Length - 1);
            cmd += ");";

            result = Exceute(cmd);
            return result;
        }
    }
}
