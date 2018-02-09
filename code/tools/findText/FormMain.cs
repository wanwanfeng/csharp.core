using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Library.Helper;

namespace findText
{
    public partial class FormMain : Form
    {

        public IDictionary<ConvertType, Type> cache;

        public FormMain()
        {
            InitializeComponent();
            label1.Text = "";
            progressBar1.Value = 0;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(Enum.GetNames(typeof (ConvertType)).Cast<object>().ToArray());
            cache = AttributeHelper.GetCacheTypeValue<ConvertType>();
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


        private void button_run_Click_1(object sender, EventArgs e)
        {
            ConvertType convertType = (ConvertType)Enum.Parse(typeof(ConvertType), comboBox1.SelectedText);
            BaseActionFor baseActionFor = (BaseActionFor)Activator.CreateInstance(cache[convertType]);
            baseActionFor.Init(this.textBox1, this.progressBar1, this.label1);
            if (this.checkBox1.Checked)
            {
                baseActionFor.Revert();
            }
            else
            {
                baseActionFor.Open();
            }
        }
    }
}
