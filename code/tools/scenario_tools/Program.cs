using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Library.Excel;
using LitJson;

namespace scenario_tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"D:\Work\yuege\www\assets\res\scenario", "*.txt").ToList();
            files.Sort();

            JsonData res = new JsonData();
            foreach (string file in files)
            {
                Console.WriteLine(file);

                var text = File.ReadAllText(file, Encoding.UTF8);
                JsonData jsonData = ConvertCSVToTreeData(text);
                foreach (JsonData data in jsonData)
                {
                    var xx = new JsonData();
                    xx["file"] = Path.GetFileNameWithoutExtension(file);
                    foreach (KeyValuePair<string, JsonData> keyValuePair in data.Inst_Object)
                    {
                        if (keyValuePair.Key != "")
                            xx[keyValuePair.Key] = keyValuePair.Value;
                    }
                    res.Add(xx);
                }
            }
            File.WriteAllText("scenario.txt", JsonMapper.ToJson(res));

            string outpath = Environment.CurrentDirectory + "/scenario.xlsx";
            List<List<object>> vals = ExcelByNpoi.ConvertJsonToList(JsonMapper.ToJson(res));
            new ExcelByNpoi().WriteToExcel(outpath, vals);
        }

        /// <summary>
        /// CSV形式のデータを変換
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isMap"></param>
        /// <returns></returns>
        public static JsonData ConvertCSVToTreeData(string text, bool isMap = false)
        {
            string[] header = null;

            List<string[]> csvData = new List<string[]>();

            System.IO.StringReader reader = new System.IO.StringReader(text);

            // 1行目は型名の情報が入っている
            string line = reader.ReadLine();
            if (line.Contains("\t"))
            {
                header = line.Split('\t');
            }
            else
            {
                header = line.Split(',');
            }
            // 必要なデータ数
            int need = header.Length;


            List<string> values = new List<string>();

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            //AppDebug.Log ("header" + line);
            //AppDebug.Log ("カラム数：" + header.Length);
            while (reader.Peek() > -1)
            {
                values.Clear();
                //builder.Append(reader.ReadLine());
                builder.AppendLine(reader.ReadLine());	// 改行を維持する

                line = builder.ToString();

                // Clearが無いので、Removeで全削除 
                builder.Remove(0, builder.Length);


                //csvでタブかカンマで区切られた文字を配列にして取得
                string[] addValues = GetCSVSplitText(line);

                //最初の1行は全件追加
                values.AddRange(addValues);
                // データ数をチェック
                int count = values.Count;

                //AppDebug.Log ("line:" + line);
                //AppDebug.Log ("カラム数：" + values.Count);

                //改行コードにより途中で文が途切れた場合の為に、データ区切りの数が満たない場合は次の行も見る
                while (count < need)
                {
                    //すでに可読の文字がない場合は終了
                    if (reader.Peek() == -1)
                    {
                        break;
                    }

                    builder.AppendLine(reader.ReadLine());	// 改行を維持する
                    line = builder.ToString();

                    // Clearが無いので、Removeで全削除 
                    builder.Remove(0, builder.Length);

                    //csvでタブかカンマで区切られた文字を配列にして取得
                    addValues = GetCSVSplitText(line);

                    if (addValues.Length > 0)
                    {
                        //addValues[0] については前の行の文字とくっつける
                        values[values.Count - 1] += addValues[0];

                        //それ以外については後ろに追加
                        for (int i = 1; i < addValues.Length; i++)
                        {
                            values.Add(addValues[i]);
                        }
                    }

                    count = values.Count;


                    //AppDebug.Log ("addNext:" + line);
                    //AppDebug.Log ("カラム数：" + values.Count);

                }

                if (values.Count >= need)
                {
                    csvData.Add(values.ToArray());
                }
            }

            //AppDebug.Log ("csvData.Count=" + csvData.Count);

            JsonData retTreeData;

            //データ件数が1でMap設定のデータであればMapとして返却する
            if (csvData.Count == 1 && isMap)
            {
                retTreeData =  new JsonData();
                string[] datas = csvData[0];

                //for (int i = 0; i < datas.Length; i++)
                for (int i = 0; i < need; i++)
                {
                    string data = datas[i].Trim();
                    string title = header[i].Trim();
                    retTreeData[title] = (JsonData)data;
                }
            }
            else
            {
                //データ件数が1より多い場合は問答無用でArrayになる
                retTreeData = new JsonData();

                foreach (string[] datas in csvData)
                {
                    JsonData tree = new JsonData();
                    for (int i = 0; i < need; i++)
                    {
                        string data = datas[i].Trim();
                        string title = header[i].Trim();
                        tree[title] = (JsonData)data;
                    }

                    retTreeData.Add(tree);
                }
            }
            return retTreeData;
        }

        static string[] GetCSVSplitText(string text)
        {
            string[] addValues = new string[0];
            if (text.Contains("\t"))
            {
                addValues = text.Split('\t');
            }
            else
            {
                addValues = text.Split(',');
            }

            return addValues;
        }

    }
}
