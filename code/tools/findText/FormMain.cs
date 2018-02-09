using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using excel;
using Library.Helper;
using LitJson;

namespace findText
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            label1.Text = "";
            progressBar1.Value = 0;
        }

        #region 拖拽

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.Lines = files;
        }

        #endregion


        private static Regex regex = new Regex(
            //"\"([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]+.*\")|(\".*[\u30A0-\u30FF]+)|([\u30A0-\u30FF])\""
            "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]')|([\u30A0-\u30FF])|([\u30A0-\u30FF])"
            //regexStr = "/u0800-/u4e00"
            );

        private static void GetJsonValue(string val, List<string> all, int i, int k, string[] input, JsonData resJsonData)
        {
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = all[i];
            jsonData["行号"] = k + 1;
            jsonData["原文"] = input[k].Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        public string[] GetFiles(out string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            label1.Text = "";
            progressBar1.Value = 0;

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                var dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件夹路径";
                dialog.ShowNewFolderButton = true;
                path = dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
            }
            else
            {
                path = textBox1.Text;
            }

            if (string.IsNullOrEmpty(path))
                return new string[0];

            if (!Directory.Exists(path))
            {
                MessageBox.Show("文件夹路径不存在!");
                return new string[0];
            }

            return Directory.GetFiles(path, "*", searchOption).Select(p => p.Replace("\\", "/")).ToArray();
        }


        public static List<List<object>> GetJsonDataArray(string content)
        {
            JsonData[] jsonDatas = JsonMapper.ToObject<JsonData[]>(content.Trim().Trim('\0'));
            //获取key集合
            List<string> keys = new List<string>();
            foreach (JsonData jsonData in jsonDatas)
            {
                foreach (var pair in jsonData.Inst_Object)
                {
                    if (keys.Contains(pair.Key))
                        continue;
                    keys.Add(pair.Key);
                }
            }
            //获取key集合对应的值集合
            var vals = new List<List<object>>();
            foreach (JsonData jsonData in jsonDatas)
            {
                List<object> val = new List<object>();
                vals.Add(val);

                foreach (var key in keys)
                {
                    JsonData value;
                    var str = jsonData.Inst_Object.TryGetValue(key, out value) ? value.ToString() : "";
                    val.Add(str.Replace(":", "::").Replace("\n", "\\n"));
                }
            }
            vals.Insert(0, keys.Select(p => (object)p).ToList());
            return vals;
        }

        private void WriteExcel(string fileName, JsonData resJsonData)
        {
            label1.Text = "正在写入Excel...";
            progressBar1.Value = 0;

            string outpath = textBox1.Text + ".xlsx";
            List<List<object>> vals = GetJsonDataArray(LitJsonHelper.ToJson(resJsonData));
            OfficeWorkbooks.WriteToExcel(outpath, vals);
            if (MessageBox.Show("是否打开文件查看信息？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                label1.Text = "";
                System.Diagnostics.Process.Start(outpath);
            }
        }

        public static void WriteResult(string fileName, string res, string ex = ".txt")
        {
            return;
            string outpath = Environment.CurrentDirectory + "/log/" + fileName + ex;
            FileHelper.CreateDirectory(outpath);
            File.WriteAllText(outpath, res);
            if (MessageBox.Show("是否打开文件查看信息？","",MessageBoxButtons.OKCancel)==DialogResult.OK)
            {
                System.Diagnostics.Process.Start(outpath);
            }

            return;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "/log/";
            openFileDialog.Filter = string.Format("文本文件(*{0})|*{0}|所有文件(*.*)|*.*",ex);
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

            }

            //if (EditorUtility.DisplayDialog(outpath,))
            //{
            //    EditorUtility.OpenWithDefaultApp(outpath);
            //}
        }

        private void button_cs_Click(object sender, EventArgs e)
        {
            string path = "";
            var all = GetFiles(out path, SearchOption.AllDirectories).ToList();
            all.Sort();

            JsonData resJsonData = new JsonData();
            resJsonData.SetJsonType(JsonType.Array);
            for (int i = 0; i < all.Count; i++)
            {
                progressBar1.Value = (int)((float)i / all.Count * 100);

                string[] input = File.ReadAllLines(all[i]);
                bool isTrue = false;

                for (int k = 0; k < input.Length; k++)
                {
                    if (isTrue) continue;
                    var val = input[k];
                    if (val.TrimStart().StartsWith("#")) continue;
                    if (val.TrimStart().StartsWith("@brief ")) continue;
                    if (val.TrimStart().StartsWith("///")) continue;
                    if (val.TrimStart().StartsWith("//")) continue;
                    //跨行注释
                    if (val.TrimStart().StartsWith("/*"))
                    {
                        if (!val.Contains("*/"))
                            isTrue = true;
                        continue;
                    }
                    if (val.TrimStart().EndsWith("*/"))
                    {
                        if (!val.Contains("/*"))
                            isTrue = false;
                        continue;
                    }
                    if (val.TrimStart().StartsWith("*")) continue;

                    MatchCollection mc = regex.Matches(val);
                    if (mc.Count == 0) continue;
                    //去除中间有//
                    var index = val.IndexOf("//", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(0, index);
                        mc = regex.Matches(val);
                        if (mc.Count == 0) continue;
                    }
                    //去除中间有/**
                    index = val.IndexOf("/**", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(0, index);
                        mc = regex.Matches(val);
                        if (mc.Count == 0) continue;
                    }
                    //去除最后一个双引号后的
                    index = val.LastIndexOf("\"", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(0, index);
                    }
                    //去除第一个双引号前的
                    index = val.IndexOf("\"", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(index + 1);
                    }
                    GetJsonValue(val, all, i, k, input, resJsonData);
                }
            }

            WriteResult("Find_CS_Text", LitJsonHelper.ToJson(resJsonData), ".json");
            WriteExcel("Find_CS_Text", resJsonData);
        }

        private void button_html_Click(object sender, EventArgs e)
        {
            string path = "";
            var all = GetFiles(out path, SearchOption.AllDirectories).ToList();
            all.Sort();

            JsonData resJsonData = new JsonData();
            resJsonData.SetJsonType(JsonType.Array);
            for (int i = 0; i < all.Count; i++)
            {
                label1.Text = "搜索中...请稍后" + (float) i/all.Count;
                progressBar1.Value = (int)((float)i / all.Count * 100);

                string[] input = File.ReadAllLines(all[i]);
                bool isTrue = false;

                for (int k = 0; k < input.Length; k++)
                {
                    if (isTrue) continue;
                    var val = input[k];
                    //if (val.TrimStart().StartsWith("//")) continue;
                    ////跨行注释
                    //if (val.TrimStart().Contains("<!--"))
                    //{
                    //    if (!val.Contains("-->"))
                    //        isTrue = true;
                    //    continue;
                    //}
                    //if (val.TrimStart().Contains("-->"))
                    //{
                    //    if (!val.Contains("<!--"))
                    //        isTrue = false;
                    //    continue;
                    //}

                    MatchCollection mc = regex.Matches(val);
                    if (mc.Count == 0) continue;
                    ////去除中间有//
                    //var index = val.IndexOf("//", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(0, index);
                    //    mc = regex.Matches(val);
                    //    if (mc.Count == 0) continue;
                    //}
                    ////去除中间有/**
                    //index = val.IndexOf("/**", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(0, index);
                    //    mc = regex.Matches(val);
                    //    if (mc.Count == 0) continue;
                    //}
                    ////去除最后一个双引号后的
                    //index = val.LastIndexOf("\"", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(0, index);
                    //}
                    ////去除第一个双引号前的
                    //index = val.IndexOf("\"", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(index + 1);
                    //}
                    GetJsonValue(val, all, i, k, input, resJsonData);
                }
            }

            WriteResult("Find_Html_Text", LitJsonHelper.ToJson(resJsonData), ".json");
            WriteExcel("Find_Html_Text", resJsonData);
        }

        private void button_js_Click(object sender, EventArgs e)
        {
            string path = "";
            var all = GetFiles(out path, SearchOption.AllDirectories).ToList();
            all.Sort();

            //return;

            JsonData resJsonData = new JsonData();
            resJsonData.SetJsonType(JsonType.Array);
            for (int i = 0; i < all.Count; i++)
            {
                label1.Text = "搜索中...请稍后" + (float)i / all.Count;
                progressBar1.Value = (int)((float)i / all.Count * 100);

                string[] input = File.ReadAllLines(all[i]);
                bool isTrue = false;

                for (int k = 0; k < input.Length; k++)
                {
                    if (isTrue) continue;
                    var val = input[k];
                    //跨行注释
                    if (val.TrimStart().StartsWith("/*"))
                    {
                        if (!val.Contains("*/"))
                            isTrue = true;
                        continue;
                    }
                    if (val.Trim().EndsWith("*/"))
                    {
                        if (!val.Contains("/*"))
                            isTrue = false;
                        continue;
                    }
                    MatchCollection mc = regex.Matches(val);
                    if (mc.Count == 0) continue;

                    if (val.TrimStart().StartsWith("//")) continue;
                    //去除中间有//
                    var index = val.IndexOf("//", StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        val = val.Substring(0, index);
                        mc = regex.Matches(val);
                        if (mc.Count == 0) continue;
                    }
                    ////去除中间有/**
                    //index = val.IndexOf("/**", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(0, index);
                    //    mc = regex.Matches(val);
                    //    if (mc.Count == 0) continue;
                    //}
                    ////去除最后一个双引号后的
                    //index = val.LastIndexOf("\"", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(0, index);
                    //}
                    ////去除第一个双引号前的
                    //index = val.IndexOf("\"", StringComparison.Ordinal);
                    //if (index >= 0)
                    //{
                    //    val = val.Substring(index + 1);
                    //}
                    GetJsonValue(val, all, i, k, input, resJsonData);
                }
            }

            WriteResult("Find_Js_Text", LitJsonHelper.ToJson(resJsonData), ".json");
            WriteExcel("Find_Js_Text", resJsonData);
        }
    }
}
