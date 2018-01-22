using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Library;

namespace Encrypt
{
    public partial class EncryptFile : Form
    {
        public EncryptFile()
        {
            InitializeComponent();

            this.textBox_In.Text = Define.DefineRoot;
            this.textBox_Out.Text = Define.DefineSave;
            this.textBox_Exclude.Lines = Define.DefineExclude;
            this.textBox_Key.Text = Define.DefineKey;

            LocalEvent.eventManager.addEvent(Debug.NewMessage, NewMessage);
        }

        private void NewMessage(object obj)
        {

        }

        #region 拖拽

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.Lines = files;
        }

        #endregion

        private void button_clear_in_Click(object sender, EventArgs e)
        {
            this.textBox_In.Text = "";
        }

        private void button_clear_exclude_Click(object sender, EventArgs e)
        {
            this.textBox_Exclude.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox_Out.Text = "";
        }

        private void TextChanged_In(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || string.IsNullOrEmpty(textBox.Text)) return;
            if (textBox == this.textBox_In)
                Define.DefineRoot = this.textBox_In.Lines.FirstOrDefault();
            if (textBox == this.textBox_Out)
                Define.DefineSave = this.textBox_Out.Lines.FirstOrDefault();
            if (textBox == this.textBox_Exclude)
                Define.DefineExclude = this.textBox_Exclude.Lines;
            if (textBox == this.textBox_Key)
                Define.DefineKey = this.textBox_Key.Lines.FirstOrDefault();
        }

        private void button_md5_Click(object sender, EventArgs e)
        {
            var list = FileUtils.GetFiles(Define.DefineRoot, SearchOption.AllDirectories, Define.DefineExclude);
            var dic = list.ToDictionary(p => p, q => q.Replace(Define.DefineRoot.Replace("\\", "/") + "/", ""));
            var res = new List<string>();
            int index = 0;
            foreach (KeyValuePair<string, string> pair in dic)
            {
                progressBar1.Value = index*100/dic.Count;
                res.Add(pair.Key);
                res.Add(MD5Utils.Encrypt(Path.GetDirectoryName(pair.Value), Define.DefineKey) + "/" +
                        MD5Utils.Encrypt(Path.GetFileName(pair.Value), Define.DefineKey));
            }
            File.WriteAllLines("sssss.txt", res.ToArray(), Encoding.UTF8);
            //if (this.button_md5.Focused)
            //{
            //    MessageBox.Show("路径设置成功！", "提示", MessageBoxButtons.OK);
            //}
        }
    }
}
