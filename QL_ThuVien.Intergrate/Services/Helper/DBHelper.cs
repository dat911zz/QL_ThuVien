using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QL_ThuVien.Intergrate.Services.Helper
{
    public class DBHelper
    {
        private string connStr;

        public string ConnStr { get => connStr; set => connStr = value; }
        public DBHelper()
        {
            connStr = ConfigurationManager.ConnectionStrings["DoAnLienMonConnectionString"].ConnectionString;
        }
        public DBHelper(string connStr)
        {
            ConnStr = connStr;
        }

        #region Utilities
        /// <summary>
        /// Add params with prefix: @p_{0} ({0} is integer start from 0)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        public void AddParameters(ref SqlCommand cmd, object[] obj)
        {
            int paramsLenth = obj.Length;
            for (int i = 0; i < paramsLenth; i++)
            {
                cmd.Parameters.AddWithValue("@p_" + i.ToString(), obj[i]);
            }
        }
        public void AddParameters(ref SqlCommand cmd, Dictionary<string, object> mapParams)
        {
            foreach (var param in mapParams)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
        }
        public void BeginTransact(Action<SqlCommand> action)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            action(cmd);
            conn.Close();
        }
        public int ExceuteNonQuery(string query, params object[] obj)
        {
            int result = 0;
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (obj != null)
                {
                    AddParameters(ref cmd, obj);
                }
                result = cmd.ExecuteNonQuery();
            });
            return result;
        }
        public int ExceuteNonQuery(string query, Dictionary<string, object> mapParams)
        {
            int result = 0;
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (mapParams != null)
                {
                    AddParameters(ref cmd, mapParams);
                }
                result = cmd.ExecuteNonQuery();
            });
            return result;
        }
        public int ExceuteScalar(string query, params object[] obj)
        {
            int result = 0;
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (obj != null)
                {
                    AddParameters(ref cmd, obj);
                }
                result = (int)cmd.ExecuteScalar();
            });
            return result;
        }
        public string ExceuteScalarString(string query, params object[] obj)
        {
            string result = "";
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (obj != null)
                {
                    AddParameters(ref cmd, obj);
                }
                result = (string)cmd.ExecuteScalar();
            });
            return result;
        }
        public double ExceuteScalarDouble(string query, params object[] obj)
        {
            double result = 0;
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (obj != null)
                {
                    AddParameters(ref cmd, obj);
                }
                result = (double)cmd.ExecuteScalar();
            });
            return result;
        }
        public int ExceuteScalar(string query, Dictionary<string, object> mapParams)
        {
            int result = 0;
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (mapParams != null)
                {
                    AddParameters(ref cmd, mapParams);
                }
                result = (int)cmd.ExecuteScalar();
            });
            return result;
        }
        /// <summary>
        /// Lấy data lên 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<T> ExecuteReader<T>(string query, params object[] obj) where T : class, new()//Attribute for avoid normal data type
        {
            List<T> list = new List<T>();
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (obj != null)
                {
                    AddParameters(ref cmd, obj);
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    T item = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(item, reader.GetValue(i));
                        }
                    }
                    list.Add(item);
                }
            });
            return list;
        }
        public List<T> ExecuteReader<T>(string query, Dictionary<string, object> mapParams) where T : class, new()
        {
            List<T> list = new List<T>();
            BeginTransact(cmd =>
            {
                cmd.CommandText = query;
                if (mapParams != null)
                {
                    AddParameters(ref cmd, mapParams);
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    T item = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(item, reader.GetValue(i));
                        }
                    }
                    list.Add(item);
                }
            });
            return list;
        }
        public void FillData(DataSet dataSet, string tableName)
        {
            string selectCmd = "Select * from " + tableName;
            SqlDataAdapter adapter = new SqlDataAdapter(selectCmd, connStr);
            adapter.Fill(dataSet, tableName);
        }      
        public int Update(DataSet ds, string tableName)
        {
            string selectCmd = "Select * from " + tableName;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(selectCmd, connStr);
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(adapter);
                var rowAffect = adapter.Update(ds, tableName);
                return rowAffect;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public void SetPrimaryKey(ref DataTable dt, params object[] mapKeys)
        {
            DataColumn[] keys = new DataColumn[mapKeys.Length];
            for (int i = 0; i < mapKeys.Length; i++)
            {
                keys[i] = dt.Columns[mapKeys[i].ToString()];
            }
            dt.PrimaryKey = keys;
        }
        public List<string> GetColNameList(string tableName)
        {
            List<string> colNameList = new List<string>();
            BeginTransact(cmd => {
                cmd.CommandText = "Select * from " + tableName;
                var reader = cmd.ExecuteReader();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    colNameList.Add(reader.GetName(i));
                }
            });
            return colNameList;
        }
        public List<string> GetListString(string tableName, string colName)
        {
            List<string> list = new List<string>();
            BeginTransact(cmd => {
                cmd.CommandText = "Select " + colName + " from " + tableName;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                }
            });
            return list;
        }
        #endregion
    }
}
