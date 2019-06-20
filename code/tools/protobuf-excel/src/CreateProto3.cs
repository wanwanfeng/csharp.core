using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Extensions;
using protobuf_excel;

public class CreateProto3 : CreateProto
{
    private static Dictionary<string, string> cacheProto = new Dictionary<string, string>()
    {
        {"bool", "	bool	"},
        {"int", "	int32	"},
        {"int[]", "	repeated	int32	"},
        {"long", "	int64	"},
        {"long[]", "	repeated	int64	"},
        {"float", "	float	"},
        {"float[]", "	repeated	float	"},
        {"double", "	double	"},
        {"double[]", "	repeated	double	"},
        {"string", "	string	"},
        {"string[]", "	repeated	string	"}
    };

    public string _Entity = "Info";
    public string _Master = "Table";
    public string _NameSpace = "Table";

    public CreateProto3()
    {
        var relultTableList = new List<string>()
        {
            "namespace " + _NameSpace,
            "{",
        };
        var relultInfosList = new List<string>()
        {
            "namespace " + _NameSpace,
            "{",
        };
        var tables = new List<string>
        {
            "message tables",
            "{",
        };
        var infos = new List<string>()
        {
            "syntax = \"proto3\";",
            "",
            "package " + _NameSpace + ";",
        };

        var fileList =  CheckPath(".xls|.xlsx")
            .Select((p, index) =>
            {
                return new
                {
                    file = p,
                    dts = ExcelByBase.Data.ImportToDataTable(p, table.lines.Count(c => c >= 0)).Where(q => q != null)
                        .ToList()
                };
            }).Select((p, index) =>
            {
                var fileInfo = Path.GetFileNameWithoutExtension(p.file).Replace("Table", "Info");
                var fileTable = Path.GetFileNameWithoutExtension(p.file).Replace("Table", "Table");
                Console.WriteLine(p.file);
                var list = new List<string> {string.Format("\nmessage {0}", fileInfo), "{"};

                var keyNames = new List<string>();
                var keyTypes = new List<string>();
                var Names = new List<string>();

                var dt = p.dts.First();

                try
                {
                    if (dt.Rows.Count == 1)
                    {
                        //字段名
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var name = dt.Rows[table.nameline][i].ToString();
                            if (string.IsNullOrEmpty(name))
                                throw new Exception("字段名称未设置！");
                            if (name.StartsWith("#")) continue;

                            list.Add(string.Format("{0}	{1} = {2};", "	string", name, i + 1));
                        }
                    }
                    else if (dt.Rows.Count == 2)
                    {
                        //字段名
                        //类型
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var name = dt.Rows[table.nameline][i].ToString();
                            if (string.IsNullOrEmpty(name))
                                throw new Exception("字段名称未设置！");
                            if (name.StartsWith("#")) continue;

                            var type = dt.Rows[table.typeline][i].ToString();
                            if (string.IsNullOrEmpty(type))
                                throw new Exception("字段类型未设置！");
                            if (type.StartsWith("#")) continue;

                            if (cacheProto.ContainsKey(type))
                                list.Add(string.Format("{0} {1} = {2};", cacheProto[type], name, i + 1));
                            else
                                list.Add(string.Format("{0} {1} = {2};", "	string", name, i + 1));
                        }
                    }
                    else if (dt.Rows.Count == 3)
                    {
                        //描述
                        //字段名
                        //类型
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var remark = dt.Rows[table.descline][i].ToString().Replace("\r", ";").Replace("\n", ";");
                            if (remark.StartsWith("#")) continue;

                            var name = dt.Rows[table.nameline][i].ToString();
                            if (string.IsNullOrEmpty(name))
                                throw new Exception("字段名称未设置！");
                            if (name.StartsWith("#")) continue;

                            var type = dt.Rows[table.typeline][i].ToString();
                            if (string.IsNullOrEmpty(type))
                                throw new Exception("字段类型未设置！");
                            if (type.StartsWith("#")) continue;


                            if (cacheProto.ContainsKey(type))
                                list.Add(string.Format("{0} {1} = {2}; // {3}", cacheProto[type], name, i + 1, remark));
                            else
                                list.Add(string.Format("{0} {1} = {2}; // {3}", "	string", name, i + 1, remark));
                        }
                    }
                    else if (dt.Rows.Count == 4)
                    {
                        //键标注
                        //描述
                        //字段名
                        //类型
                        int line = 1;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var remark = dt.Rows[table.descline][i].ToString().Replace("\r", ";").Replace("\n", ";");
                            if (remark.StartsWith("#")) continue;

                            var name = dt.Rows[table.nameline][i].ToString();
                            if (string.IsNullOrEmpty(name))
                                throw new Exception("字段名称未设置！");
                            if (name.StartsWith("#")) continue;

                            var type = dt.Rows[table.typeline][i].ToString();
                            if (string.IsNullOrEmpty(type))
                                throw new Exception("字段类型未设置！");
                            if (type.StartsWith("#")) continue;

                            if (cacheProto.ContainsKey(type))
                                list.Add(string.Format("{0} {1} = {2}; // {3}", cacheProto[type], name, line++,
                                    remark));
                            else
                                list.Add(string.Format("{0} {1} = {2}; // {3}", "	string", name, line++, remark));

                            var key = dt.Rows[table.keysline][i].ToString();
                            if (string.IsNullOrEmpty(key)) continue;
                            if (key.StartsWith("#")) continue;

                            keyNames.Add(name);
                            keyTypes.Add(type);
                        }
                    }
                    else
                    {
                        throw new Exception("没有的配置类型！");
                    }
                }
                catch (Exception e)
                {
                    ErrorOut(e, dt);
                    throw;
                }

                list.Add("}");

                //WriteAllText(string.Format("tables/Info/{0}.cs", fileInfo),
                //    Replace(str_entity_na)
                //        .Replace("$EntityName$", fileInfo)
                //        .Replace("$Id$", names[0])
                //        .Replace("$IdType$", types[0])
                //);

                string keyString = keyTypes.Count == 0
                    ? ""
                    : string.Format(" DataInfoBase<{0}>", string.Join(",", keyTypes.ToArray()));

                relultInfosList.Add(
                    Replace(str_entity)
                        .Replace("$EntityName$", fileInfo)
                        .Replace("$Partent$", keyString)
                        .Replace("$Id$", keyNames[0]+"_")
                        .Replace("$IdType$", keyTypes[0])
                        );

                return new { list = list, fileInfo = fileInfo, table = fileTable, index = index, keyTypes = keyTypes };
            })
            .Select(p =>
            {
                infos.AddRange(p.list);
                tables.Add(string.Format("   repeated  {0} {1} = {2};", p.fileInfo, p.table, p.index + 1));

                //WriteAllLines(string.Format("tables/Table/{0}.cs", p.table), new string[]
                //{
                //    "namespace " + _NameSpace,
                //    "{",
                //    string.Format("\tpublic partial class {0} {{}}", p.table),
                //    "}"
                //});

                string keys = p.keyTypes.Count == 0
                    ? ""
                    : string.Format(", {0}", string.Join(",", p.keyTypes.ToArray()));

                keys = string.Format(" DataTableBase<{0}{1}>", p.fileInfo, keys);
                relultTableList.Add(
                    Replace(str_master)
                        .Replace("$Partent$", keys)
                        .Replace("$MasterName$", p.table)
                        .Replace(" $FileName$", "\"" + p.table + "\"")

                );
                return p.table;
            }).ToList();

        tables.Add("}");
        infos.AddRange(tables);
        string tables_path = WriteAllLines("tables.proto", infos.ToArray());
		
        File.Copy(tables_path, "tables.proto");
        string cmd = "protoc.exe --proto_path=./ --csharp_out=./{0} tables.proto";
        CmdReadAll(string.Format(cmd, GetType().Name)).ForEach(p => Console.WriteLine(p));

        CreateCS(relultTableList, relultInfosList, fileList);
    }

    public override string str_data_master
    {
        get
        {
            return @"using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace $namespace$ {
    public partial class Data$Master$Manager {

        public static tables tables { get; set; }

        public List<IData$Master$Base> dataObjects { get; private set; }

$ListField$

        public Data$Master$Manager () {
            dataObjects = new List<IData$Master$Base> ();
$List$
        }

        public void Init () {
            tables = Table.tables.Parser.ParseFrom (System.Convert.FromBase64String (System.IO.File.ReadAllText (""masters.data"")));
            dataObjects.ForEach (p => p.Init ());
        }
    }
}";
        }
    }
}