using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Caf.Core
{
    public static class UtilsExtension
    {

        /// <summary>
        ///   start&lt;= num && &gt;=end
        /// </summary>
        /// <param name="num"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between(this int num, int start, int end)
        {
            return num >= start && num < end;
        }

        public static bool Between(this Enum value, int start, int end)
        {
            int num = value.ToInt();
            return num >= start && num < end;
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static string ToIntString(this Enum value)
        {
            return value.ToString("d");
        }

        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
        }

        #region 验证
        //public static bool IsEmail(this string value)
        //{
        //    return Regex.IsMatch(value, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);
        //}

        //public static bool IsTelephone(this string value)
        //{
        //    return Regex.IsMatch(value, @"^(\d{3,4}-)?\d{6,8}$");
        //}

        //public static bool IsMobile(this string source)
        //{
        //    return Regex.IsMatch(source, @"^1[0-9]\d{9}$", RegexOptions.IgnoreCase);
        //}

        //public static bool IsIDcard(this string value)
        //{
        //    return Regex.IsMatch(value, @"(^\d{18}$)|(^\d{15}$)");
        //}

        //public static bool IsNumber(this string value)
        //{
        //    return Regex.IsMatch(value, @"^[0-9]*$");
        //}

        //public static bool IsPostalcode(this string value)
        //{
        //    return Regex.IsMatch(value, @"^\d{6}$");
        //}
        #endregion

        #region 时间
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalMilliseconds);
        }


        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime ConvertUnixTimeToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 某月的开始时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return date.AddDays(1 - date.Day).StartOfDay();
        }

        /// <summary>
        /// 某月的结束时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return date.AddMonths(1).AddDays(0 - date.Day).EndOfDay();
        }

        /// <summary>
        /// 某天开始时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        /// <summary>
        /// 某天结束时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }
        #endregion


        #region DataTable

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">data</param>
        /// <param name="columns">列顺序</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list, string[] columns, string tableName)
        {
            var dt = new DataTable(tableName);
            var props = new List<PropertyInfo>(typeof(T).GetProperties());

            foreach (var item in columns)
            {
                dt.Columns.Add(item);
            }

            foreach (var item in list)
            {
                var values = new object[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    var prop = props.Find(p => p.Name.Equals(columns[i], StringComparison.OrdinalIgnoreCase));
                    values[i] = prop?.GetValue(item);
                }

                dt.Rows.Add(values);
            }

            return dt;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="columnsMap">T的属性，table的列名</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list, Dictionary<string, string> columnsMap, string tableName)
        {
            var dt = new DataTable(tableName);
            var props = new List<PropertyInfo>(typeof(T).GetProperties());

            foreach (var item in columnsMap.Values)
            {
                dt.Columns.Add(item);
            }

            foreach (var item in list)
            {
                var values = new object[columnsMap.Count];
                var index = 0;
                foreach (var key in columnsMap.Keys)
                {
                    var prop = props.Find(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                    values[index] = prop?.GetValue(item);
                    index++;
                }
                dt.Rows.Add(values);
            }

            return dt;
        }

        /// <summary>
        /// 将DataTable导出到CSV中，防止乱码
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">propName-columnName</param>
        /// <returns></returns>
        public static byte[] ToCsvStream(this DataTable data, Dictionary<string, string> columns = null,bool isContainHeader=true)
        {
            StringBuilder sb = new StringBuilder();
            if (columns?.Count > 0)
            {
                if (isContainHeader)
                {
                    sb.Append("\"").AppendJoin("\",\"", columns.Values).Append("\"").AppendLine();
                }
                foreach (DataRow dr in data.Rows)
                {
                    int i = 0;
                    foreach (var c in columns)
                    {
                        if (!Convert.IsDBNull(dr[c.Key]))
                        {
                            sb.AppendFormat("\"{0}\"", dr[c.Key].ToString().Replace("\"", "\"\"").RemoveEnterChar());
                        }

                        if (i < columns.Count - 1)
                        {
                            sb.Append(",");
                        }

                        i++;
                    }
                    sb.AppendLine();
                }

            }
            else
            {
                if (isContainHeader)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        sb.AppendFormat("\"{0}\"", data.Columns[i]);
                        if (i < data.Columns.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine();
                }
                foreach (DataRow dr in data.Rows)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            var value = String.Format("\"{0}\"", dr[i].ToString().Replace("\"", "\"\"").RemoveEnterChar());
                            sb.Append(value);
                        }
                        if (i < data.Columns.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sb.AppendLine();
                }
            }

            var str = sb.ToString();
            var ms = new MemoryStream();
            {
                if (isContainHeader)
                {
                    //添加BOM文件头，防止csv乱码问题 Write BOM Header-- - \uFEFF
                    ms.Write(new byte[] { 239, 187, 191 }, 0, 3);
                }
                var temp = Encoding.UTF8.GetBytes(str);
                ms.Write(temp, 0, temp.Length);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// dataTabla转换成CSV文件
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <param name="strFilePath"></param>
        public static string ToCSV(this DataTable dtDataTable)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sb.AppendFormat("\"{0}\"", dtDataTable.Columns[i].ColumnName.Replace("\"", "\"\"").RemoveEnterChar());
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.AppendLine();
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        var value = String.Format("\"{0}\"", dr[i].ToString().Replace("\"", "\"\"").RemoveEnterChar());
                        sb.Append(value);
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static string ToTxtBySeparator(this DataTable dtDataTable, string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sb.AppendFormat("{0}", dtDataTable.Columns[i].ColumnName.RemoveEnterChar());
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sb.Append(separator);
                }
            }
            //sb.AppendLine();
            sb.Append("\r\n");
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        var value = String.Format("{0}", dr[i].ToString().RemoveEnterChar());
                        sb.Append(value);
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(separator);
                    }
                }
                //sb.AppendLine();
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// dataTabla转换成CSV文件
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <param name="strFilePath"></param>
        public static string ToCSVForODS(this DataTable dtDataTable)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sb.AppendFormat("\"{0}\"", dtDataTable.Columns[i].ColumnName.Replace("\"", "\"\"").RemoveEnterChar());
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("\r\n");

            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        var value = String.Format("\"{0}\"", dr[i].ToString().Replace("\"", "\"\"").RemoveEnterChar());
                        sb.Append(value);
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("\r\n");
            }
            return sb.ToString();
        }


        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>DataTable</returns>
        public static void OpenCSV(Stream stream, Action<DataTable> fun)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8"); //Encoding.ASCII;
            //FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            StreamReader sr = new StreamReader(stream);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            if ((strLine = sr.ReadLine()) != null)
            {
                tableHead = strLine.Split(',');
                columnCount = tableHead.Length;

                int batchSize = 4000;
                do
                {
                    var dt = GetOneBatch(sr, ref strLine, tableHead, columnCount, batchSize);
                    fun(dt);
                } while (strLine != null);
            }

            sr.Close();
        }

        private static DataTable GetOneBatch(StreamReader sr, ref string strLine, string[] tableHead, int columnCount, int batchSize)
        {
            DataTable dt = new DataTable();
            //创建列
            for (int i = 0; i < columnCount; i++)
            {
                DataColumn dc = new DataColumn(tableHead[i]);
                dt.Columns.Add(dc);
            }
            string[] aryLine = null;
            for (int i = 0; i < batchSize && (strLine = sr.ReadLine()) != null; i++)
            {
                if (!String.IsNullOrEmpty(strLine))
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            return dt;
        }

        public static string RemoveEnterChar(this string str)
        {
            str = str.Replace((char)13, (char)0);
            str = str.Replace((char)13, (char)0);
            return str;
        }
        #endregion

        /// <summary>
        /// 获取字符串UTF-8编码
        /// </summary>
        /// <param name="unicodeString">需要编码的字符串</param>
        /// <returns></returns>
        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        /// <summary>
        /// 判断数组中值是否在存在于字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool contains(this string str, string[] arr)
        {
            if (str == null || arr == null)
                throw  new  ArgumentNullException();
            foreach (var item in arr)
            {
                if (str.IndexOf(item) > 0) return true;
            }
            return false;
        }
        public static string GetEnumDesc(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
    }



    #region
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                func(left, right), parameter);
        }

        private class ReplaceExpressionVisitor
            : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

    }
    #endregion
}
