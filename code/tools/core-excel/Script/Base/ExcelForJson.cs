﻿using System.IO;
using System.Text;
using Library.Helper;
using LitJson;

namespace Library.Excel
{
    /// <summary>
    /// DataTable与Json
    /// </summary>
    public abstract partial class ExcelUtils
    {
        #region  Convert Json and DataTable

        public static DataTable ImportFromJson(string path)
        {
            return (DataTable)JsonHelper.ImportJsonToListTable(path);
        }

        public static void ExportToJson(DataTable dt, string file, bool isIndent = true)
        {
            string newPath = CheckExport(dt, file, ".json");
            JsonData resJsonDatas = (JsonData)dt;
            File.WriteAllText(newPath,
                isIndent ? JsonHelper.ToJson(resJsonDatas, indentLevel: 2) : JsonHelper.ToJson(resJsonDatas),
                new UTF8Encoding(false));
        }

        #endregion
    }
}