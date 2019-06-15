using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Extensions;
using protobuf_excel;

public class CreateProto2 : CreateProto
{
    private static Dictionary<string, string> cacheProto = new Dictionary<string, string>()
    {
        {"bool", "	optional	bool	"},
        {"int", "	optional	int32	"},
        {"int[]", "	repeated	int32	"},
        {"long", "	optional	int64	"},
        {"long[]", "	repeated	int64	"},
        {"float", "	optional	float	"},
        {"float[]", "	repeated	float	"},
        {"double", "	optional	double	"},
        {"double[]", "	repeated	double	"},
        {"string", "	optional	string	"},
        {"string[]", "	repeated	string	"}
    };

    public CreateProto2()
    {
        WriteAllLines("Data/Base/IDataEntityBase.cs",
            new string[]
            {
                "namespace table",
                "{",
                "\tpublic partial interface IDataEntityBase {}",
                "\tpublic partial interface IDataEntityBase<T> : IDataEntityBase {}",
                "}"
            });

        WriteAllLines("Data/Base/IDataMasterBase.cs",
            new string[]
            {
                "namespace table",
                "{",
                "\tpublic partial interface IDataMasterBase{}",
                "\tpublic partial interface IDataMasterBase<T> : IDataMasterBase where T : IDataEntityBase {}",
                "}"
            });


        var relultMasterList = new List<string>()
        {
            "namespace table",
            "{",
        };
        var relultEntitysList = new List<string>()
        {
            "namespace table",
            "{",
        };
        var masters = new List<string>
        {
            "syntax = \"proto2\";",
            "",
            "import \"entitys.proto\";",
            "",
            "package table;",
            "",
            "message masters",
            "{",
        };
        var entitys = new List<string>()
        {
            "syntax = \"proto2\";",
            "",
            "package table;"
        };

        CheckPath(".xls|.xlsx")
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
                var fileEntity = Path.GetFileNameWithoutExtension(p.file).Replace("Table", "Entity");
                var fileMaster = Path.GetFileNameWithoutExtension(p.file).Replace("Table", "Master");
                Console.WriteLine(p.file);
                var list = new List<string> {string.Format("\nmessage {0}", fileEntity), "{"};
                var types = new List<string>();
                var dt = p.dts.First();

                if (dt.Rows.Count == 1)
                {
                    //字段名
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var name = dt.Rows[table.nameline][i].ToString();
                        if (name.StartsWith("#")) continue;
                        list.Add(string.Format("{0}	{1} = {2};", "	optional string", name, i + 1));
                    }
                }
                else if (dt.Rows.Count == 2)
                {
                    //字段名
                    //类型
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var name = dt.Rows[table.nameline][i].ToString();
                        if (name.StartsWith("#")) continue;
                        var type = dt.Rows[table.typeline][i].ToString();
                        if (type.StartsWith("#")) continue;
                        if (cacheProto.ContainsKey(type))
                            list.Add(string.Format("{0} {1} = {2};", cacheProto[type], name, i + 1));
                        else
                            list.Add(string.Format("{0} {1} = {2};", "	optional string", name, i + 1));
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
                        if (name.StartsWith("#")) continue;
                        var type = dt.Rows[table.typeline][i].ToString();
                        if (type.StartsWith("#")) continue;
                        if (cacheProto.ContainsKey(type))
                            list.Add(string.Format("{0} {1} = {2}; // {3}", cacheProto[type], name, i + 1, remark));
                        else
                            list.Add(string.Format("{0} {1} = {2}; // {3}", "	optional string", name, i + 1, remark));
                    }
                }
                else if (dt.Rows.Count == 4)
                {   
                    //键标注
                    //描述
                    //字段名
                    //类型
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var remark = dt.Rows[table.descline][i].ToString().Replace("\r", ";").Replace("\n", ";");
                        if (remark.StartsWith("#")) continue;
                        var name = dt.Rows[table.nameline][i].ToString();
                        if (name.StartsWith("#")) continue;
                        var type = dt.Rows[table.typeline][i].ToString();
                        if (type.StartsWith("#")) continue;
                        if (cacheProto.ContainsKey(type))
                            list.Add(string.Format("{0} {1} = {2}; // {3}", cacheProto[type], name, i + 1, remark));
                        else
                            list.Add(string.Format("{0} {1} = {2}; // {3}", "	optional string", name, i + 1, remark));


                        var key = dt.Rows[table.keysline][i].ToString();
                        if (string.IsNullOrEmpty(key)) continue;
                        if (key.StartsWith("#")) continue;
                        types.Add(type);
                    }
                }
                else
                {
                    throw new Exception("没有的配置类型！");
                }
                list.Add("}");

                WriteAllLines(string.Format("Data/Info/{0}.cs", fileEntity), new string[]
                {
                    "namespace table",
                    "{",
                    string.Format("\tpublic partial class {0} {{}}", fileEntity),
                    "}"
                });

                string keys = types.Count == 0
                    ? ""
                    : string.Format("DataEntityBase<{0}>,", string.Join(",", types.ToArray()));
                relultEntitysList.Add(string.Format("\tpublic partial class {0} :{1} IDataEntityBase {{}}", fileEntity,
                    keys));
                return new { list = list, fileEntity = fileEntity, master = fileMaster, index = index, types = types };
            })
            .ToList()
            .ForEach(p =>
                {
                    entitys.AddRange(p.list);
                    masters.Add(string.Format("   repeated  {0} {1} = {2};", p.fileEntity, p.master, p.index + 1));

                    WriteAllLines(string.Format("Data/Master/{0}.cs", p.master), new string[]
                    {
                        "namespace table",
                        "{",
                        string.Format("\tpublic partial class {0} {{}}", p.master),
                        "}"
                    });

                    string keys = p.types.Count == 0
                        ? ""
                        : string.Format("DataMasterBase<{0}>,", string.Join(",", p.types.ToArray()));
                    relultMasterList.Add(string.Format("\tpublic partial class {0} :{1} IDataMasterBase {{}}", p.master,
                        p.fileEntity));
                }
            );

        masters.Add("}");
        entitys.AddRange(masters);
        string masters_path = WriteAllLines("masters.proto", entitys.ToArray());

        string cmd = "protogen.exe -i:{0} -o:{1} -p:partialMethods";
        CmdReadAll(string.Format(cmd, masters_path, Path.ChangeExtension(masters_path, ".cs"))).ForEach(p => Console.WriteLine(p));

        relultMasterList.Add("}");
        WriteAllLines("Data/Base/DataMasterBase.cs", relultMasterList.ToArray());
        relultEntitysList.Add("}");
        WriteAllLines("Data/Base/DataEntityBase.cs", relultEntitysList.ToArray());
    }
}