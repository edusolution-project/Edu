using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public static class QueryUtils
    {

        private static List<string> GetAllParameter(string query)
        {
            List<string> lstString = new List<string>();
            MatchCollection mc = Regex.Matches(query, @"\:[\w_]+");
            foreach (Match m in mc)
            {
                string key = m.Value.Substring(1);
                if (!lstString.Contains(key))
                {
                    lstString.Add(key);
                }
            }

            return lstString;
        }

        public static List<Dictionary<string, object>> LoadData(DbConnection connection,
            string sqlSelect, Dictionary<string, object> param)
        {
            List<object> lstParam;
            List<Dictionary<string, object>> table;
            sqlSelect = PreProcess(connection, sqlSelect.Replace(":", "@"), param, out lstParam, out table);
            using (var cmd = (MySqlCommand)connection.CreateCommand())
            {
                cmd.CommandText = sqlSelect;

                //cmd. BindByName = true;
                foreach (var paramz in lstParam)
                    cmd.Parameters.Add(paramz);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                            row[reader.GetName(i)] = reader[i];
                        table.Add(row);
                    }
                }
            }

            return table;
        }

        public static string PreProcessQuery(string sqlSelect)
        {
            sqlSelect = sqlSelect.Replace("\r", " ");
            sqlSelect = sqlSelect.Replace("\n", " ");
            sqlSelect = sqlSelect.Replace("\t", " ");
            sqlSelect = sqlSelect.Trim().TrimEnd(';');
            return sqlSelect;
        }

        private static string PreProcess(DbConnection connection, string sqlSelect, Dictionary<string, object> param, out List<object> lstParam, out List<Dictionary<string, object>> table)
        {
            sqlSelect = sqlSelect.Replace("\r", " ");
            sqlSelect = sqlSelect.Replace("\n", " ");
            sqlSelect = sqlSelect.Replace("\t", " ");
            sqlSelect = sqlSelect.Trim().TrimEnd(';');
            lstParam = new List<object>();
            List<string> lstParamHasValue = new List<string>();
            foreach (var item in param)
            {
                if (sqlSelect.Contains(":" + item.Key))
                {
                    lstParamHasValue.Add(item.Key);
                    lstParam.Add(new MySqlParameter(item.Key, item.Value));
                }
            }

            List<string> lst = GetAllParameter(sqlSelect);
            foreach (var x in lst)
            {
                if (!lstParamHasValue.Contains(x))
                {
                    lstParam.Add(new MySqlParameter(x, null));
                }
            }

            table = new List<Dictionary<string, object>>();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            return sqlSelect;
        }

        public static object RunQuery(DbConnection connection, string sqlSelect, Dictionary<string, object> param)
        {
            List<object> lstParam;
            List<Dictionary<string, object>> table;
            sqlSelect = PreProcess(connection, sqlSelect, param, out lstParam, out table);

            object obj = null;
            using (var cmd = (MySqlCommand)connection.CreateCommand())
            {
                cmd.CommandText = sqlSelect;

                //cmd.BindByName = true;
                foreach (var paramz in lstParam)
                    cmd.Parameters.Add(paramz);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount != 1)
                    {
                        obj = "script không đúng";
                        return obj;
                    }
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        if (count > 1)
                        {
                            obj = "script không đúng";
                            return obj;
                        }
                        if (reader[0] == null)
                        {
                            obj = 0;
                        }
                        else
                        {
                            obj = reader[0];
                        }
                    }
                }

            }

            return obj;
        }
        public static QueryScalarResult RunQueryCheckError(DbConnection connection, string sqlSelect, Dictionary<string, object> param)
        {
            QueryScalarResult result = new QueryScalarResult();
            List<object> lstParam;
            List<Dictionary<string, object>> table;
            sqlSelect = PreProcess(connection, sqlSelect, param, out lstParam, out table);
            using (var cmd = (MySqlCommand)connection.CreateCommand())
            {
                cmd.CommandText = sqlSelect;
                //cmd.BindByName = true;
                foreach (var paramz in lstParam)
                    cmd.Parameters.Add(paramz);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.FieldCount != 1)
                    {
                        result.Value = null;
                        result.Error = true;
                        return result;
                    }
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        if (count > 1)
                        {
                            result.Value = null;
                            result.Error = true;
                            return result;
                        }
                        result.Value = reader[0];
                        result.Error = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Phucpd11 - 25/12/2018 - insert nhiều dữ liệu
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlSelect"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void DirectPushToDatabase(DbConnection connection,
            string query, List<MySqlDbType> listType, List<List<object>> listData)
        {
            int Count = listData[0].Count();
            if (Count > 0)
            {
                using (var cmd = (MySqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.CommandTimeout = 0;
                    for (int i = 0; i < listType.Count; i++)
                    {
                        MySqlParameter param = new MySqlParameter
                        {
                            MySqlDbType = listType[i],
                            Value = listData[i].ToArray()
                        };
                        cmd.Parameters.Add(param);
                    }
                    //cmd.ArrayBindCount = Count;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        ///  Phucpd11 - 25/12/2018 - bulk insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="lstData"></param>
        /// <param name="primaryKey">Nếu khác null, sẽ không chèn trong câu lệnh insert
        public static void BulkInsert<T>(DbConnection connection, List<T> lstData, string primaryKey = null)
        {
            if (lstData == null || lstData.Count <= 0)
            {
                return;
            }
            List<List<object>> ListParam = new List<List<object>>();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());
            StringBuilder query = new StringBuilder();
            string ObjName = typeof(T).Name;
            query.Append("insert into " + ObjName + "(");
            List<string> lstColumn = new List<string>();
            var PropDict = new Dictionary<string, PropertyInfo>();
            PropDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);
            foreach (var prop in props)
            {
                var Info = PropDict[prop.Name];
                if (primaryKey == null || prop.Name != primaryKey && !Info.PropertyType.ToString().Contains("Data.Access.Object.Entities"))
                {
                    lstColumn.Add("`" + prop.Name + "`");
                }
            }

            query.Append(string.Join(",", lstColumn));
            query.Append(") values ");
            if (connection.State != ConnectionState.Open)
                connection.Open();
            using (var cmdMySql = connection.CreateCommand())
            {
                List<string> lstVal = new List<string>();
                int j = 0;
                foreach (var item in lstData)
                {
                    Type type = item.GetType();
                    IList<PropertyInfo> props1 = new List<PropertyInfo>(type.GetProperties());
                    List<string> lstValue = new List<string>();
                    foreach (PropertyInfo prop in props1)
                    {
                        var Info = PropDict[prop.Name];
                        if (primaryKey == null || prop.Name != primaryKey && !Info.PropertyType.ToString().Contains("Data.Access.Object.Entities"))
                        {
                            object propValue = prop.GetValue(item, null);
                            cmdMySql.Parameters.Add(new MySqlParameter("@" + prop.Name + j, propValue));// AddWithValue(prop.Name + j, prop.GetValue(i));
                            lstValue.Add("@" + prop.Name + j);
                        }
                    }
                    lstVal.Add("(" + string.Join(", ", lstValue.ToArray()) + ")");
                    j++;
                }
                query.Append(string.Join(",", lstVal.ToArray()));
                cmdMySql.CommandText = query.ToString();
                cmdMySql.CommandType = CommandType.Text;                
                cmdMySql.ExecuteNonQuery();
                cmdMySql.Dispose();
            }
        }

        /// <summary>
        ///  Phucpd11 - 25/12/2018 - bulk insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="lstData"></param>
        /// <param name="primaryKey">Nếu khác null, sẽ không chèn trong câu lệnh insert
        public static void BulkInsertWithTrans<T>(DbConnection connection, IDbContextTransaction trans, List<T> lstData, string primaryKey = null)
        {
            if (lstData == null || lstData.Count <= 0)
            {
                return;
            }
            List<List<object>> ListParam = new List<List<object>>();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());           
            StringBuilder query = new StringBuilder();
            string ObjName = typeof(T).Name;
            query.Append("insert into " + ObjName + "(");
            List<string> lstColumn = new List<string>();
            var PropDict = new Dictionary<string, PropertyInfo>();
            PropDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);
            foreach (var prop in props)
            {
                var Info = PropDict[prop.Name];
                if (primaryKey == null || prop.Name != primaryKey && !Info.PropertyType.ToString().Contains("Data.Access.Object.Entities"))
                {
                    lstColumn.Add("`" + prop.Name + "`");
                }
            }
        
            query.Append(string.Join(",", lstColumn));
            query.Append(") values ");
            if (connection.State != ConnectionState.Open)
                connection.Open();
            using (var cmdMySql = connection.CreateCommand())
            {
                List<string> lstVal = new List<string>();
                int j = 0;
                foreach (var item in lstData)
                {
                    Type type = item.GetType();
                    IList<PropertyInfo> props1 = new List<PropertyInfo>(type.GetProperties());
                    List<string> lstValue = new List<string>();
                    foreach (PropertyInfo prop in props1)
                    {
                        var Info = PropDict[prop.Name];
                        if (primaryKey == null || prop.Name != primaryKey && !Info.PropertyType.ToString().Contains("Data.Access.Object.Entities"))
                        {
                            object propValue = prop.GetValue(item, null);
                            cmdMySql.Parameters.Add(new MySqlParameter("@" + prop.Name + j, propValue));
                            lstValue.Add("@" + prop.Name + j);
                        }
                    }
                    lstVal.Add("(" + string.Join(", ", lstValue.ToArray()) + ")");
                    j++;
                }
                query.Append(string.Join(",", lstVal.ToArray()));
                cmdMySql.CommandText = query.ToString();
                cmdMySql.CommandType = CommandType.Text;
                cmdMySql.Transaction = (trans as IInfrastructure<DbTransaction>).Instance; 
                cmdMySql.ExecuteNonQuery();
                cmdMySql.Dispose();
            }

        }
    }
}
