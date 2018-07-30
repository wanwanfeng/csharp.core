﻿using System.Collections.Generic;
using System.Data;
using System.IO;
using Library.LitJson;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelByBase
    {
        #region  Convert Json and DataTable

        public class Json
        {
            public static ListTable ConvertToListTable(string content)
            {
                return LitJsonHelper.ConvertJsonToListTable(content);
            }

            public static DataTable ConvertToDataTable(string content)
            {
                return List.ConvertToDataTable(ConvertToListTable(content));
            }

            public static DataTable ImportToDataTable(string path)
            {
                return List.ConvertToDataTable(ImportToListTable(path));
            }

            public static ListTable ImportToListTable(string file)
            {
                return LitJsonHelper.ImportJsonToListTable(file);
            }
        }

        public partial class Data
        {
            public static JsonData ConvertToJson(DataTable dt)
            {
                return List.ConvertToJson(ConvertToListTable(dt));
            }

            public static void ExportToJson(DataTable dt, string file)
            {
                file = string.IsNullOrEmpty(file) ? dt.FullName : file;
                List.ExportToJson(ConvertToListTable(dt), Path.ChangeExtension(file, ".json"));
            }
        }

        #endregion
    }
}