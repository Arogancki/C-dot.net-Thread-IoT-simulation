using System;
using System.Windows.Forms;

namespace Agnetowe
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.GetText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // GetText
            // 
            resources.ApplyResources(this.GetText, "GetText");
            this.GetText.Name = "GetText";
            this.GetText.TextChanged += new System.EventHandler(this.GetText_TextChanged);
            GetText.KeyDown += (sender, args) => {
                if (args.KeyCode == Keys.Return)
                {
                    button1.PerformClick();
                }
            };
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // title
            // 
            this.title.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.title, "title");
            this.title.ForeColor = System.Drawing.SystemColors.InfoText;
            this.title.Name = "title";
            this.title.ReadOnly = true;
            // 
            // Form2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.title);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.GetText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        #endregion

        public System.Windows.Forms.TextBox GetText;
        private System.Windows.Forms.Button button1;
        public TextBox title;
    }
}