namespace findText
{
    partial class FormMain
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
            this.button_cs = new System.Windows.Forms.Button();
            this.button_cj = new System.Windows.Forms.Button();
            this.button_php = new System.Windows.Forms.Button();
            this.button_html = new System.Windows.Forms.Button();
            this.button_js = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_cs
            // 
            this.button_cs.Location = new System.Drawing.Point(12, 52);
            this.button_cs.Name = "button_cs";
            this.button_cs.Size = new System.Drawing.Size(291, 23);
            this.button_cs.TabIndex = 0;
            this.button_cs.Text = "Open Cs Text";
            this.button_cs.UseVisualStyleBackColor = true;
            this.button_cs.Click += new System.EventHandler(this.button_cs_Click);
            // 
            // button_cj
            // 
            this.button_cj.Location = new System.Drawing.Point(12, 81);
            this.button_cj.Name = "button_cj";
            this.button_cj.Size = new System.Drawing.Size(291, 23);
            this.button_cj.TabIndex = 1;
            this.button_cj.Text = "Open C++ Text";
            this.button_cj.UseVisualStyleBackColor = true;
            // 
            // button_php
            // 
            this.button_php.Location = new System.Drawing.Point(12, 110);
            this.button_php.Name = "button_php";
            this.button_php.Size = new System.Drawing.Size(291, 23);
            this.button_php.TabIndex = 2;
            this.button_php.Text = "Open Php Text";
            this.button_php.UseVisualStyleBackColor = true;
            // 
            // button_html
            // 
            this.button_html.Location = new System.Drawing.Point(12, 139);
            this.button_html.Name = "button_html";
            this.button_html.Size = new System.Drawing.Size(291, 23);
            this.button_html.TabIndex = 3;
            this.button_html.Text = "Open Html Text";
            this.button_html.UseVisualStyleBackColor = true;
            this.button_html.Click += new System.EventHandler(this.button_html_Click);
            // 
            // button_js
            // 
            this.button_js.Location = new System.Drawing.Point(12, 168);
            this.button_js.Name = "button_js";
            this.button_js.Size = new System.Drawing.Size(291, 23);
            this.button_js.TabIndex = 4;
            this.button_js.Text = "Open Js Text";
            this.button_js.UseVisualStyleBackColor = true;
            this.button_js.Click += new System.EventHandler(this.button_js_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 232);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 18);
            this.progressBar1.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.AllowDrop = true;
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(290, 21);
            this.textBox1.TabIndex = 6;
            this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button_js);
            this.Controls.Add(this.button_html);
            this.Controls.Add(this.button_php);
            this.Controls.Add(this.button_cj);
            this.Controls.Add(this.button_cs);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_cs;
        private System.Windows.Forms.Button button_cj;
        private System.Windows.Forms.Button button_php;
        private System.Windows.Forms.Button button_html;
        private System.Windows.Forms.Button button_js;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}

