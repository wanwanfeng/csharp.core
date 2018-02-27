using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using excel;
using findText.Script;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace findText
{
    public enum ConvertType
    {
        [TypeValue(typeof (ActionForCpp))] cpp,
        [TypeValue(typeof (ActionForCSharp))] csharp,
        [TypeValue(typeof (ActionForPhp))] php,
        [TypeValue(typeof (ActionForJava))] java,
        [TypeValue(typeof (ActionForJava))] javascript,
        [TypeValue(typeof (ActionForHtml))] html,
    }

    public abstract class BaseActionFor
    {

        private TextBox textBox1;
        private Label label1;
        private ProgressBar progressBar1;


        public void Init(TextBox textBox, ProgressBar progressBar, Label label)
        {
            label1 = label;
            textBox1 = textBox;
            progressBar1 = progressBar;

            //label1.Text = "";
            //textBox1.Text = "";
            progressBar1.Value = 0;
        }

        protected static Regex regex = new Regex(
            //"\"([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]+.*\")|(\".*[\u30A0-\u30FF]+)|([\u30A0-\u30FF])\""
            "([\u4E00-\u9FA5]+)|([\u4E00-\u9FA5]')|([\u30A0-\u30FF])|([\u30A0-\u30FF])"
            //regexStr = "/u0800-/u4e00"
            );

        private string[] GetFiles(out string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
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

        private JsonData SetJsonDataArray(List<List<object>> content)
        {
            JsonData jsonDatas = new JsonData();
            jsonDatas.SetJsonType(JsonType.Array);
            List<object> first = content.First();
            content.Remove(first);
            foreach (List<object> objects in content)
            {
                JsonData data = new JsonData();
                for (int j = 0; j < first.Count; j++)
                {
                    string val = objects[j].ToString();
                    val = val.Replace("::", ":").Replace("\\n", "\n");
                    data[first[j].ToString()] = val;
                }
                jsonDatas.Add(data);
            }
            return jsonDatas;
        }

        private List<List<object>> GetJsonDataArray(string content)
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
            vals.Insert(0, keys.Select(p => (object) p).ToList());
            return vals;
        }

        private void WriteExcel(string fileName, JsonData resJsonData)
        {
            label1.Text = "正在写入Excel...";
            progressBar1.Value = 0;

            string outpath = textBox1.Text + ".xlsx";
            List<List<object>> vals = GetJsonDataArray(JsonMapper.ToJson(resJsonData));
            OfficeWorkbooks.WriteToExcel(outpath, vals);

            label1.Text = "";
            if (MessageBox.Show("是否打开文件查看信息？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(outpath);
            }
        }

        private void WriteResult(string fileName, string res, string ex = ".txt")
        {
            return;
            string outpath = Environment.CurrentDirectory + "/log/" + fileName + ex;
            FileHelper.CreateDirectory(outpath);
            File.WriteAllText(outpath, res);
            if (MessageBox.Show("是否打开文件查看信息？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(outpath);
            }

            return;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "/log/";
            openFileDialog.Filter = string.Format("文本文件(*{0})|*{0}|所有文件(*.*)|*.*", ex);
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

            }
        }

        protected string path = "";
        protected List<string> all = new List<string>();
        private JsonData resJsonData;

        protected virtual string textName
        {
            get { return "Find_Text"; }
        }

        protected virtual string[] exName
        {
            get { return new string[0]; }
        }

        public void Open()
        {
            var res = GetFiles(out path, SearchOption.AllDirectories).ToList();
            foreach (var s in exName)
            {
                all.AddRange(res.Where(p =>
                {
                    var xx = Path.GetExtension(p);
                    return !string.IsNullOrEmpty(xx) && xx == s;
                }));
            }
            if (all.Count == 0) return;
            all.Sort();

            resJsonData = new JsonData();
            resJsonData.SetJsonType(JsonType.Array);
            OpenRun();
            WriteResult(textName, JsonMapper.ToJson(resJsonData), ".json");
            WriteExcel(textName, resJsonData);
        }

        protected abstract void OpenRun();

        protected string[] GetShowInfo(int i)
        {
            label1.Text = "搜索中...请稍后" + (float) i/all.Count;
            progressBar1.Value = (int) ((float) i/all.Count*100);

            return File.ReadAllLines(all[i]);
        }

        protected void GetJsonValue(string val, int i, int k, string[] input)
        {
            if (string.IsNullOrEmpty(val.Trim())) return;
            JsonData jsonData = new JsonData();
            jsonData["文件名"] = all[i];
            jsonData["行号"] = k + 1;
            jsonData["原文"] = input[k].Trim();
            jsonData["译文"] = val.Trim();
            resJsonData.Add(jsonData);
        }

        public virtual void Revert()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "/log/";
            openFileDialog.Filter = string.Format("文本文件(*{0})|*{0}|所有文件(*.*)|*.*", ".xlsx");
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Dictionary<string, List<List<object>>> dic = OfficeWorkbooks.ReadFromExcel(openFileDialog.FileName);

                foreach (KeyValuePair<string, List<List<object>>> pair in dic)
                {
                    JsonData jsonData = SetJsonDataArray(pair.Value);


                    foreach (JsonData data in jsonData)
                    {
                        string temp = data["文件名"].ToString();
                        string[] content = File.ReadAllLines(temp);
                        int line = data["行号"].ToString().AsInt();
                        string oldStr = data["原文"].ToString();
                        string oldStr2 = data["需翻译"].ToString();
                        string newStr = data["译文"].ToString();
                        //if (content[line] == oldStr)
                            content[line] = content[line].Replace(oldStr2, newStr);
                            File.WriteAllLines(temp, content);
                    }

                    //foreach (KeyValuePair<string, JsonData> data in jsonData.Inst_Object)
                    //{
                    //    string temp = data.Value["文件名"].ToString();
                    //    string[] content = File.ReadAllLines(temp);
                    //    int line = data.Value["行号"].ToString().AsInt() + 1;
                    //    string oldStr = data.Value["原文"].ToString();
                    //    string oldStr2 = data.Value["需翻译"].ToString();
                    //    string newStr = data.Value["译文"].ToString();
                    //    if (content[line] == oldStr)
                    //        content[line] = content[line].Replace(oldStr2, newStr);
                    //    File.WriteAllLines(temp, content);
                    //}
                }
            }
        }
    }
}
