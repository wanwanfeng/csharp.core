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
        [TypeValue(typeof(CompareExcel))] CompareExcel,

        [Description("Excel->ToXml")] [TypeValue(typeof(ActionExcel.ToXml))] ExcelToXml,
        [Description("Excel->ToCsv")] [TypeValue(typeof(ActionExcel.ToCsv))] ExcelToCsv,
        [Description("Excel->ToJson")] [TypeValue(typeof(ActionExcel.ToJson))] ExcelToJson,
        [Description("Excel->ToExcel")] [TypeValue(typeof(ActionExcel.ToExcel))] ExcelToExcel,
        [Description("Excel->ToOneExcel")] [TypeValue(typeof(ActionExcel.ToOneExcel))] ExcelToOneExcel,
        [Description("Excel->ToKvExcel")] [TypeValue(typeof(ActionExcel.ToKvExcel))] ExcelToKvExcel,
        [Description("Excel->KvExcelTo")] [TypeValue(typeof(ActionExcel.KvExcelTo))] ExcelKvExcelTo,

        [Description("Json->ToXml")] [TypeValue(typeof(ActionJson.ToXml))] JsonToXml,
        [Description("Json->ToCsv")] [TypeValue(typeof(ActionJson.ToCsv))] JsonToCsv,
        [Description("Json->ToExcel")] [TypeValue(typeof(ActionJson.ToExcel))] JsonToExcel,
        [Description("Json->ToOneExcel")] [TypeValue(typeof(ActionJson.ToOneExcel))] JsonToOneExcel,
        [Description("JsonArray->ToKvExcel")] [TypeValue(typeof(ActionJson.ArrayToKvExcel))] JsonArrayToKvExcel,
        [Description("JsonArray->KvExcelTo")] [TypeValue(typeof(ActionJson.ArrayKvExcelTo))] JsonArrayKvExcelTo,
        [Description("JsonObject->ToKvExcel")] [TypeValue(typeof(ActionJson.ObjectToKvExcel))] JsonObjectToKvExcel,

        [Description("Xml->ToCsv")] [TypeValue(typeof(ActionXml.ToCsv))] XmlToCsv,
        [Description("Xml->ToJson")] [TypeValue(typeof(ActionXml.ToJson))] XmlToJson,
        [Description("Xml->ToExcel")] [TypeValue(typeof(ActionXml.ToExcel))] XmlToExcel,
        [Description("Xml->ToOneExcel")] [TypeValue(typeof(ActionXml.ToOneExcel))] XmlToOneExcel,
        [Description("Xml->ToKvExcel")] [TypeValue(typeof(ActionXml.ToKvExcel))] XmlToKvExcel,
        [Description("Xml->KvExcelTo")] [TypeValue(typeof(ActionXml.KvExcelTo))] XmlKvExcelTo,

        [Description("CSV->ToXml")] [TypeValue(typeof(ActionCSV.ToXml))] CsvToXml,
        [Description("CSV->ToJson")] [TypeValue(typeof(ActionCSV.ToJson))] CsvToJson,
        [Description("CSV->ToExcel")] [TypeValue(typeof(ActionCSV.ToExcel))] CsvToExcel,
        [Description("CSV->ToOneExcel")] [TypeValue(typeof(ActionCSV.ToOneExcel))] CsvToOneExcel,
        [Description("CSV->ToKvExcel")] [TypeValue(typeof(ActionCSV.ToKvExcel))] CsvToKvExcel,
        [Description("CSV->KvExcelTo")] [TypeValue(typeof(ActionCSV.KvExcelTo))] CsvKvExcelTo,
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
            //SystemConsole.Run<CaoType>(null, 3);
            //return;


            Dictionary<string, string> caDictionary = new Dictionary<string, string>()
            {
                {"\\image_native\\scene\\quest\\prologueBattle2.json/scenario/missionList/1/description","ミッション２"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/scenario/missionList/2/description","ミッション３"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList/0/name","巴 マミ"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList/0/endMessage","一仕事、終わったわぁ"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList/1/name","鹿目 まどか"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/playerList/1/endMessage","勝利ですね♡"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList/0/enemyList/0/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList/0/enemyList/1/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList/0/enemyList/2/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList/0/enemyList/3/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/waveList/0/enemyList/4/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList/0/name","回復"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList/0/description","コネクト対象を回復。"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList/1/name","ティロ・デュエット"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/connectList/1/description","コネクト対象の防御力アップ。"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/memoriaList/0/name","キュアヒール"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/memoriaList/0/description","HPを回復する"},
                {"\\image_native\\scene\\quest\\prologueBattle2.json/navi/talk/1/message","ふぅ…惜しかったわね"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/title","プロローグ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/titleExtend","第３話"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList/0/description","ミッション１"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList/1/description","ミッション２"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/scenario/missionList/2/description","ミッション３"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList/0/name","巴 マミ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList/0/endMessage","一仕事、終わったわぁ"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList/1/name","鹿目 まどか"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/playerList/2/name","暁美 ほむら"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/0/enemyList/0/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/0/enemyList/1/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/0/enemyList/2/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/0/enemyList/3/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/0/enemyList/4/name","魔女の手下"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/1/enemyList/0/name","委員長の魔女"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/1/enemyList/1/name","部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/1/enemyList/2/name","部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/1/enemyList/3/name","部位"},
                {"\\image_native\\scene\\quest\\prologueBattle3.json/waveList/1/enemyList/4/name","部位"},
            };

            JsonData json =
                LitJsonHelper.ConvertDictionaryToJson(
                    caDictionary.ToDictionary(p => p.Key, q => new JsonData(q.Value)));
            File.WriteAllText("ss.txt", LitJsonHelper.ToJson(json));
        }
    }
}
