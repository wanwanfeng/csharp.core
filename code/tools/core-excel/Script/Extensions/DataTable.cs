//using Library.Extensions;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;

//namespace Library.Excel
//{
//    public class DataTable : System.Data.DataTable
//    {
//        public DataTable(string table)
//            : base(table)
//        {
//        }

//        public DataTable()
//        {

//        }

//        #region ListTable DataTable 隐式转换


//        public static implicit operator ListTable(DataTable dt)
//        {
//            var vals = new List<List<object>>();
//            foreach (DataRow dr in dt.Rows)
//            {
//                vals.Add(dr.ItemArray.Select(p => (p is System.DBNull) ? "" : p).ToList());
//            }
//            return new ListTable()
//            {
//                TableName = dt.TableName,
//                Columns = dt.GetHeaderList(),
//                Rows = vals,
//            };
//        }

//        /// <summary>
//        /// 行集合转换为DataTable
//        /// </summary>
//        /// <param name="lt"></param>
//        /// <returns></returns>
//        public static implicit operator DataTable(ListTable lt)
//        {
//            var dt = new DataTable()
//            {
//                TableName = string.IsNullOrEmpty(lt.TableName) ? "Sheet1" : lt.TableName,
//            };

//            foreach (object o in lt.Columns)
//                dt.Columns.Add(o.ToString(), typeof(object));

//            foreach (List<object> objects in lt.Rows)
//                dt.Rows.Add(objects.ToArray());

//            return dt;
//        }

//        #endregion
//    }
//}