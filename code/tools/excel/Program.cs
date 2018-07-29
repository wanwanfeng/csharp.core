using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.LitJson;
using LitJson;
using Script;

namespace Library.Excel
{
    public enum CaoType
    {
        [Category("文件比较")]
        [TypeValue(typeof (CompareExcel))] CompareExcel,

        [Category("文件转换")] [Description("Excel->ToXml")] [TypeValue(typeof (ActionExcel.ToXml))] ExcelToXml,
        [Category("文件转换")] [Description("Excel->ToCsv")] [TypeValue(typeof (ActionExcel.ToCsv))] ExcelToCsv,
        [Category("文件转换")] [Description("Excel->ToJson")] [TypeValue(typeof (ActionExcel.ToJson))] ExcelToJson,
        [Category("文件转换")] [Description("Excel->ToExcel")] [TypeValue(typeof (ActionExcel.ToExcel))] ExcelToExcel,
        [Category("文件转换")] [Description("Excel->ToOneExcel")] [TypeValue(typeof (ActionExcel.ToOneExcel))] ExcelToOneExcel,


        [Category("文件转换")] [Description("Json->ToXml")] [TypeValue(typeof (ActionJson.ToXml))] JsonToXml,
        [Category("文件转换")] [Description("Json->ToCsv")] [TypeValue(typeof (ActionJson.ToCsv))] JsonToCsv,
        [Category("文件转换")] [Description("Json->ToExcel")] [TypeValue(typeof (ActionJson.ToExcel))] JsonToExcel,
        [Category("文件转换")] [Description("Json->ToOneExcel")] [TypeValue(typeof (ActionJson.ToOneExcel))] JsonToOneExcel,

        [Category("文件转换")] [Description("Xml->ToCsv")] [TypeValue(typeof (ActionXml.ToCsv))] XmlToCsv,
        [Category("文件转换")] [Description("Xml->ToJson")] [TypeValue(typeof (ActionXml.ToJson))] XmlToJson,
        [Category("文件转换")] [Description("Xml->ToExcel")] [TypeValue(typeof (ActionXml.ToExcel))] XmlToExcel,
        [Category("文件转换")] [Description("Xml->ToOneExcel")] [TypeValue(typeof (ActionXml.ToOneExcel))] XmlToOneExcel,


        [Category("文件内容替换")] [Description("CSV->ToXml")] [TypeValue(typeof (ActionCSV.ToXml))] CsvToXml,
        [Category("文件内容替换")] [Description("CSV->ToJson")] [TypeValue(typeof (ActionCSV.ToJson))] CsvToJson,
        [Category("文件内容替换")] [Description("CSV->ToExcel")] [TypeValue(typeof (ActionCSV.ToExcel))] CsvToExcel,
        [Category("文件内容替换")] [Description("CSV->ToOneExcel")] [TypeValue(typeof (ActionCSV.ToOneExcel))] CsvToOneExcel,
        [Category("文件内容替换")] [Description("Excel->ToKvExcel")] [TypeValue(typeof (ActionExcel.ToKvExcel))] ExcelToKvExcel,
        [Category("文件内容替换")] [Description("Excel->KvExcelTo")] [TypeValue(typeof (ActionExcel.KvExcelTo))] ExcelKvExcelTo,
        [Category("文件内容替换")] [Description("Json->ToKvExcel")] [TypeValue(typeof (ActionJson.ToKvExcel))] JsonArrayToKvExcel,
        [Category("文件内容替换")] [Description("Json->KvExcelTo")] [TypeValue(typeof (ActionJson.KvExcelTo))] JsonArrayKvExcelTo,
        [Category("文件内容替换")] [Description("Xml->ToKvExcel")] [TypeValue(typeof (ActionXml.ToKvExcel))] XmlToKvExcel,
        [Category("文件内容替换")] [Description("Xml->KvExcelTo")] [TypeValue(typeof (ActionXml.KvExcelTo))] XmlKvExcelTo,
        [Category("文件内容替换")] [Description("CSV->ToKvExcel")] [TypeValue(typeof (ActionCSV.ToKvExcel))] CsvToKvExcel,
        [Category("文件内容替换")] [Description("CSV->KvExcelTo")] [TypeValue(typeof (ActionCSV.KvExcelTo))] CsvKvExcelTo,
    }


    internal class Program
    {
        static Program()
        {
            Ldebug.OnActionLog += Console.WriteLine;
            Ldebug.OnActionLogError += Console.WriteLine;
        }

        private static void Main(string[] args)
        {
            SystemConsole.Run<CaoType>(null, 3);
            return;


            Dictionary<string, string> caDictionary = new Dictionary<string, string>()
            {
                {"\\image_native\\scene\\quest\\prologueBattle2.json/scenario/missionList@0/description", "ミッション１"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/scenario/missionList@1/description", "ミッション２"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/scenario/missionList@2/description", "ミッション３"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList@0/name", "巴 マミ"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList@0/endMessage", "一仕事、終わったわぁ"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList@1/name", "鹿目 まどか"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList@1/endMessage", "勝利ですね♡"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList@0/enemyList@0/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList@0/enemyList@1/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList@0/enemyList@2/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList@0/enemyList@3/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList@0/enemyList@4/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList@0/name", "回復"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList@0/description", "コネクト対象を回復。"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList@1/name", "ティロ・デュエット"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList@1/description", "コネクト対象の防御力アップ。"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/memoriaList@0/name", "キュアヒール"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/memoriaList@0/description", "HPを回復する"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/navi/talk@1/message", "ふぅ…惜しかったわね"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/title", "プロローグ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/titleExtend", "第３話"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList@0/description", "ミッション１"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList@1/description", "ミッション２"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList@2/description", "ミッション３"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList@0/name", "巴 マミ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList@0/endMessage", "一仕事、終わったわぁ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList@1/name", "鹿目 まどか"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList@2/name", "暁美 ほむら"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@0/enemyList@0/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@0/enemyList@1/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@0/enemyList@2/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@0/enemyList@3/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@0/enemyList@4/name", "魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@1/enemyList@0/name", "委員長の魔女"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@1/enemyList@1/name", "部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@1/enemyList@2/name", "部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@1/enemyList@3/name", "部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList@1/enemyList@4/name", "部位"},
            };



            ////var cache = LitJsonHelper.ConvertDictionaryToDictionary(caDictionary.ToDictionary(p => p.Key, q => q.Value));
            //var cache = LitJsonHelper.ConvertDictionaryToDictionary(caDictionary.ToDictionary(p => p.Key, q =>new JsonData(q.Value)));
            //File.WriteAllText("ss.txt", LitJsonHelper.ToJson(cache));

            JsonData json =
                LitJsonHelper.ConvertDictionaryToJson(caDictionary.ToDictionary(p => p.Key, q => new JsonData(q.Value)));
            File.WriteAllText("ss.txt", LitJsonHelper.ToJson(json));
        }
    }
}
