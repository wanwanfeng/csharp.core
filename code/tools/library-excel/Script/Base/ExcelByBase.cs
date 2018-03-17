using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Library.Helper;
using Library.LitJson;
using LitJson;

namespace Library.Excel
{
    public abstract class ExcelByBase
    { 
        
        #region virtual

        public KeyValuePair<string, List<List<object>>> ReadFromExcel(string filename)
        {
            var dic = ReadFromExcels(filename);
            return dic == null
                ? new KeyValuePair<string, List<List<object>>>()
                : new KeyValuePair<string, List<List<object>>>(filename,
                    ReadFromExcels(filename).Values.FirstOrDefault());
        }



        public virtual Dictionary<string, List<List<object>>> ReadFromExcels(string filename)
        {
            return null;
        }

        public virtual void WriteToExcel(string filename, List<List<object>> vals)
        {

        }

        public virtual void WriteToOneExcel(string fileName, Dictionary<string, List<List<object>>> dic)
        {

        }

        #endregion

        #region  Convert Csv and DataTable

        public static DataTable ConvertCsvToDataTable(string path, string dtName = "")
        {
            path = Path.ChangeExtension(path, ".csv");
            if (!File.Exists(path))
            {
                Console.WriteLine("文件不存在!");
            }
            if (path == null) return null;
            string[] content = File.ReadAllLines(path);
            var list = content.Select(p => p.Split(',').Cast<object>().ToList()).ToList();

            dtName = string.IsNullOrEmpty(dtName) ? Path.GetFileNameWithoutExtension(path) : dtName;
            return ConvertListToDataTable(list, dtName);
        }

        public static void ConvertDataTableToCsv(DataTable dt, string file = null)
        {
            var list = ConvertDataTableToList(dt);
            if (string.IsNullOrEmpty(file))
                file = dt.TableName;
            var contents = list.Select(p => string.Join(",", p.Select(q => q.ToString()).ToArray())).ToArray();
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? dt.TableName : file, ".csv");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllLines(newPath, contents, new UTF8Encoding(false));
        }

        #endregion

        #region  Convert List<List<object>> and DataTable

        public static DataTable ConvertListToDataTable(List<List<object>> vals, string dtName = "")
        {
            var dt = new DataTable(string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName);

            foreach (object o in vals.First())
                dt.Columns.Add(o.ToString(), typeof (string));

            foreach (List<object> objects in vals.Skip(1))
                dt.Rows.Add(objects.ToArray());

            return dt;
        }

        public static List<List<object>> ConvertDataTableToList(DataTable dt)
        {
            var vals = new List<List<object>>();

            foreach (DataRow dr in dt.Rows)
            {
                vals.Add(dr.ItemArray.ToList());
            }
            return vals;
        }

        public static List<string> ConvertDataTableHeaderToList(DataTable dt)
        {
            var vals = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                vals.Add(dc.ColumnName);
            }
            return vals;
        }

        #endregion

        #region  Convert Xml and DataTable

        public static DataTable ConvertXmlToDataTable(string path, string dtName = "")
        {
            path = Path.ChangeExtension(path, ".xml");
            if (!File.Exists(path))
            {
                Console.WriteLine("文件不存在!");
            }
            if (path == null) return null;
            string content = File.ReadAllText(path);

            StringReader dsr = new StringReader(content);
            XmlTextReader xr = new XmlTextReader(dsr);

            var dt = new DataTable(string.IsNullOrEmpty(dtName) ? "Sheet1" : dtName);
            dt.ReadXml(xr);
            dsr.Close();
            xr.Close();
            return dt;
        }

        public static void ConvertDataTableToXml(DataTable dt, string file = null)
        {
            StringWriter dsw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(dsw);
            dt.WriteXml(xw);
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? dt.TableName : file, ".xml");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, dsw.ToString(), new UTF8Encoding(false));
            xw.Close();
            dsw.Close();
        }

        #endregion

        #region  Convert Json and DataTable

        public static DataTable ConvertJsonToDataTableByPath(string path, string dtName = "Sheet1")
        {
            return ConvertListToDataTable(ConvertJsonToListByPath(path), dtName);
        }

        public static DataTable ConvertJsonToDataTable(string content, string dtName = "Sheet1")
        {
            return ConvertListToDataTable(ConvertJsonToList(content), dtName);
        }

        public static void ConvertDataTableToJson(DataTable dt)
        {
            var kv = new KeyValuePair<string, List<List<object>>>(dt.TableName, ConvertDataTableToList(dt));
            ConvertListToJson(kv);
        }

        public static void ConvertDataTableToJsonByPath(DataTable dt, string file)
        {
            var dtName = string.IsNullOrEmpty(dt.TableName) ? Path.GetFileNameWithoutExtension(file) : dt.TableName;
            var kv = new KeyValuePair<string, List<List<object>>>(dtName, ConvertDataTableToList(dt));
            ConvertListToJsonFile(kv, file);
        }
        #endregion

        #region  Convert List<List<object>> and Json

        public static List<List<object>> ConvertJsonToListByPath(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("文件不存在!");
            }
            string content = File.ReadAllText(file);
            return ConvertJsonToList(content);
        }

        public static List<List<object>> ConvertJsonToList(string content)
        {
            return LitJsonHelper.ConvertJsonToList(content);
        }

        public static JsonData ConvertListToJson(KeyValuePair<string, List<List<object>>> keyValuePair)
        {
            Console.WriteLine(" is now sheet: " + keyValuePair.Key);
            return LitJsonHelper.ConvertListToJson(keyValuePair.Value);
        }

        public static void ConvertListToJsonFile(KeyValuePair<string, List<List<object>>> keyValuePair, string file)
        {
            Console.WriteLine(" is now sheet: " + keyValuePair.Key);
            JsonData resJsonDatas = ConvertListToJson(keyValuePair);
            string newPath = Path.ChangeExtension(string.IsNullOrEmpty(file) ? keyValuePair.Key : file, ".json");
            FileHelper.CreateDirectory(newPath);
            File.WriteAllText(newPath, JsonMapper.ToJson(resJsonDatas), new UTF8Encoding(false));
        }

        #endregion
    
        void HaHa(DataTable dt)
        {
            var headers = ConvertDataTableHeaderToList(dt);

            DataTable dataTable = dt.Clone();
            //dataTable.DefaultView.ToTable()
            string regex = "";

            foreach (string header in headers)
            {
                //List<object> vals = new List<object>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    vals.Add(dr[header]);
                //}
                //var content = string.Join(",", vals.Select(p => p.ToString()).ToArray());
                //if (Regex.IsMatch(content, regex))
                //{

                //}

                foreach (DataRow dr in dt.Rows)
                {
                    var content = dr[header].ToString();
                    if (Regex.IsMatch(content, regex))
                    {
                        dr.Delete();
                        break;
                    }
                }
            }
            dataTable.AcceptChanges();
        }

        #region zimu

        public static List<string> zimu = new List<string>()
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU",
            "AV",
            "AW",
            "AX",
            "AY",
            "AZ",
            "BA",
            "BB",
            "BC",
            "BD",
            "BE",
            "BF",
            "BG",
            "BH",
            "BI",
            "BJ",
            "BK",
            "BL",
            "BM",
            "BN",
            "BO",
            "BP",
            "BQ",
            "BR",
            "BS",
            "BT",
            "BU",
            "BV",
            "BW",
            "BX",
            "BY",
            "BZ",
            "CA",
            "CB",
            "CC",
            "CD",
            "CE",
            "CF",
            "CG",
            "CH",
            "CI",
            "CJ",
            "CK",
            "CL",
            "CM",
            "CN",
            "CO",
            "CP",
            "CQ",
            "CR",
            "CS",
            "CT",
            "CU",
            "CV",
            "CW",
            "CX",
            "CY",
            "CZ",
            "DA",
            "DB",
            "DC",
            "DD",
            "DE",
            "DF",
            "DG",
            "DH",
            "DI",
            "DJ",
            "DK",
            "DL",
            "DM",
            "DN",
            "DO",
            "DP",
            "DQ",
            "DR",
            "DS",
            "DT",
            "DU",
            "DV",
            "DW",
            "DX",
            "DY",
            "DZ",
            "EA",
            "EB",
            "EC",
            "ED",
            "EE",
            "EF",
            "EG",
            "EH",
            "EI",
            "EJ",
            "EK",
            "EL",
            "EM",
            "EN",
            "EO",
            "EP",
            "EQ",
            "ER",
            "ES",
            "ET",
            "EU",
            "EV",
            "EW",
            "EX",
            "EY",
            "EZ",
            "FA",
            "FB",
            "FC",
            "FD",
            "FE",
            "FF",
            "FG",
            "FH",
            "FI",
            "FJ",
            "FK",
            "FL",
            "FM",
            "FN",
            "FO",
            "FP",
            "FQ",
            "FR",
            "FS",
            "FT",
            "FU",
            "FV",
            "FW",
            "FX",
            "FY",
            "FZ",
            "GA",
            "GB",
            "GC",
            "GD",
            "GE",
            "GF",
            "GG",
            "GH",
            "GI",
            "GJ",
            "GK",
            "GL",
            "GM",
            "GN",
            "GO",
            "GP",
            "GQ",
            "GR",
            "GS",
            "GT",
            "GU",
            "GV",
            "GW",
            "GX",
            "GY",
            "GZ",
            "HA",
            "HB",
            "HC",
            "HD",
            "HE",
            "HF",
            "HG",
            "HH",
            "HI",
            "HJ",
            "HK",
            "HL",
            "HM",
            "HN",
            "HO",
            "HP",
            "HQ",
            "HR",
            "HS",
            "HT",
            "HU",
            "HV",
            "HW",
            "HX",
            "HY",
            "HZ",
            "IA",
            "IB",
            "IC",
            "ID",
            "IE",
            "IF",
            "IG",
            "IH",
            "II",
            "IJ",
            "IK",
            "IL",
            "IM",
            "IN",
            "IO",
            "IP",
            "IQ",
            "IR",
            "IS",
            "IT",
            "IU",
            "IV",
            "IW",
            "IX",
            "IY",
            "IZ",
            "JA",
            "JB",
            "JC",
            "JD",
            "JE",
            "JF",
            "JG",
            "JH",
            "JI",
            "JJ",
            "JK",
            "JL",
            "JM",
            "JN",
            "JO",
            "JP",
            "JQ",
            "JR",
            "JS",
            "JT",
            "JU",
            "JV",
            "JW",
            "JX",
            "JY",
            "JZ",
            "KA",
            "KB",
            "KC",
            "KD",
            "KE",
            "KF",
            "KG",
            "KH",
            "KI",
            "KJ",
            "KK",
            "KL",
            "KM",
            "KN",
            "KO",
            "KP",
            "KQ",
            "KR",
            "KS",
            "KT",
            "KU",
            "KV",
            "KW",
            "KX",
            "KY",
            "KZ",
            "LA",
            "LB",
            "LC",
            "LD",
            "LE",
            "LF",
            "LG",
            "LH",
            "LI",
            "LJ",
            "LK",
            "LL",
            "LM",
            "LN",
            "LO",
            "LP",
            "LQ",
            "LR",
            "LS",
            "LT",
            "LU",
            "LV",
            "LW",
            "LX",
            "LY",
            "LZ",
            "MA",
            "MB",
            "MC",
            "MD",
            "ME",
            "MF",
            "MG",
            "MH",
            "MI",
            "MJ",
            "MK",
            "ML",
            "MM",
            "MN",
            "MO",
            "MP",
            "MQ",
            "MR",
            "MS",
            "MT",
            "MU",
            "MV",
            "MW",
            "MX",
            "MY",
            "MZ",
            "NA",
            "NB",
            "NC",
            "ND",
            "NE",
            "NF",
            "NG",
            "NH",
            "NI",
            "NJ",
            "NK",
            "NL",
            "NM",
            "NN",
            "NO",
            "NP",
            "NQ",
            "NR",
            "NS",
            "NT",
            "NU",
            "NV",
            "NW",
            "NX",
            "NY",
            "NZ",
            "OA",
            "OB",
            "OC",
            "OD",
            "OE",
            "OF",
            "OG",
            "OH",
            "OI",
            "OJ",
            "OK",
            "OL",
            "OM",
            "ON",
            "OO",
            "OP",
            "OQ",
            "OR",
            "OS",
            "OT",
            "OU",
            "OV",
            "OW",
            "OX",
            "OY",
            "OZ",
        };

        #endregion
    }
}