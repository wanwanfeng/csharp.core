namespace Encrypt
{
    partial class EncryptFile
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Key = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Out = new System.Windows.Forms.TextBox();
            this.button_clear_exclude = new System.Windows.Forms.Button();
            this.button_md5 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox_In = new System.Windows.Forms.TextBox();
            this.button_clear_in = new System.Windows.Forms.Button();
            this.textBox_Exclude = new System.Windows.Forms.TextBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.textBox_Key);
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBox_Out);
            this.panel2.Controls.Add(this.button_clear_exclude);
            this.panel2.Controls.Add(this.button_md5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.richTextBox1);
            this.panel2.Controls.Add(this.textBox_In);
            this.panel2.Controls.Add(this.button_clear_in);
            this.panel2.Controls.Add(this.textBox_Exclude);
            this.panel2.Location = new System.Drawing.Point(15, 22);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(660, 366);
            this.panel2.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "密钥:";
            // 
            // textBox_Key
            // 
            this.textBox_Key.AllowDrop = true;
            this.textBox_Key.Location = new System.Drawing.Point(54, 276);
            this.textBox_Key.Name = "textBox_Key";
            this.textBox_Key.Size = new System.Drawing.Size(480, 21);
            this.textBox_Key.TabIndex = 23;
            this.textBox_Key.TextChanged += new System.EventHandler(this.TextChanged_In);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(21, 324);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(615, 10);
            this.progressBar1.TabIndex = 22;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(560, 213);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 19;
            this.button1.Text = "清除";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "输出目录:";
            // 
            // textBox_Out
            // 
            this.textBox_Out.AllowDrop = true;
            this.textBox_Out.Location = new System.Drawing.Point(20, 238);
            this.textBox_Out.Name = "textBox_Out";
            this.textBox_Out.Size = new System.Drawing.Size(615, 21);
            this.textBox_Out.TabIndex = 17;
            this.textBox_Out.TextChanged += new System.EventHandler(this.TextChanged_In);
            this.textBox_Out.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.textBox_Out.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            // 
            // button_clear_exclude
            // 
            this.button_clear_exclude.Location = new System.Drawing.Point(560, 176);
            this.button_clear_exclude.Name = "button_clear_exclude";
            this.button_clear_exclude.Size = new System.Drawing.Size(75, 23);
            this.button_clear_exclude.TabIndex = 16;
            this.button_clear_exclude.Text = "清除";
            this.button_clear_exclude.UseVisualStyleBackColor = true;
            this.button_clear_exclude.Click += new System.EventHandler(this.button_clear_exclude_Click);
            // 
            // button_md5
            // 
            this.button_md5.Location = new System.Drawing.Point(553, 274);
            this.button_md5.Name = "button_md5";
            this.button_md5.Size = new System.Drawing.Size(83, 23);
            this.button_md5.TabIndex = 15;
            this.button_md5.Text = "MD5加密";
            this.button_md5.UseVisualStyleBackColor = true;
            this.button_md5.Click += new System.EventHandler(this.button_md5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "排除的文件后缀(逗号分隔):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "加密根目录(多个换行分隔):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "说明:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(20, 35);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(615, 59);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // textBox_In
            // 
            this.textBox_In.AllowDrop = true;
            this.textBox_In.Location = new System.Drawing.Point(20, 145);
            this.textBox_In.Name = "textBox_In";
            this.textBox_In.Size = new System.Drawing.Size(615, 21);
            this.textBox_In.TabIndex = 7;
            this.textBox_In.TextChanged += new System.EventHandler(this.TextChanged_In);
            this.textBox_In.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.textBox_In.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            // 
            // button_clear_in
            // 
            this.button_clear_in.Location = new System.Drawing.Point(560, 118);
            this.button_clear_in.Name = "button_clear_in";
            this.button_clear_in.Size = new System.Drawing.Size(75, 23);
            this.button_clear_in.TabIndex = 5;
            this.button_clear_in.Text = "清除";
            this.button_clear_in.UseVisualStyleBackColor = true;
            this.button_clear_in.Click += new System.EventHandler(this.button_clear_in_Click);
            // 
            // textBox_Exclude
            // 
            this.textBox_Exclude.AllowDrop = true;
            this.textBox_Exclude.Location = new System.Drawing.Point(179, 178);
            this.textBox_Exclude.Name = "textBox_Exclude";
            this.textBox_Exclude.Size = new System.Drawing.Size(341, 21);
            this.textBox_Exclude.TabIndex = 4;
            this.textBox_Exclude.TextChanged += new System.EventHandler(this.TextChanged_In);
            // 
            // EncryptFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(691, 407);
            this.Controls.Add(this.panel2);
            this.Name = "EncryptFile";
            this.Text = "Excel Export";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_md5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox_In;
        private System.Windows.Forms.Button button_clear_in;
        private System.Windows.Forms.TextBox textBox_Exclude;
        private System.Windows.Forms.Button button_clear_exclude;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Out;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Key;



    }
}

