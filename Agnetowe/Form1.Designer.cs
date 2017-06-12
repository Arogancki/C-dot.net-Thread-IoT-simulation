using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Agnetowe
{
    partial class IoT
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IoT));
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TurnOff = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel5.Location = new System.Drawing.Point(0, 1);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(796, 28);
            this.panel5.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel3.Controls.Add(this.listView2);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Location = new System.Drawing.Point(198, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(797, 74);
            this.panel3.TabIndex = 6;
            // 
            // listView2
            // 
            this.listView2.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView2.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listView2.AllowColumnReorder = true;
            this.listView2.AutoArrange = false;
            this.listView2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listView2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.listView2.Font = new System.Drawing.Font("Century", 12.75F);
            this.listView2.ForeColor = System.Drawing.SystemColors.Info;
            this.listView2.FullRowSelect = true;
            this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView2.LabelWrap = false;
            this.listView2.Location = new System.Drawing.Point(-1, 29);
            this.listView2.Margin = new System.Windows.Forms.Padding(0);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.OwnerDraw = true;
            this.listView2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listView2.ShowGroups = false;
            this.listView2.Size = new System.Drawing.Size(797, 41);
            this.listView2.TabIndex = 11;
            this.listView2.TabStop = false;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.List;
            this.listView2.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.LV2Draw);
            this.listView2.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.LV2MouseEnter);
            this.listView2.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV2MouseClick);
            this.listView2.MouseLeave += new System.EventHandler(this.LV2MouseLeave);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 100;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.Font = new System.Drawing.Font("Century", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox1.Location = new System.Drawing.Point(85, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(47, 26);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "IoT";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox2.Font = new System.Drawing.Font("Century", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox2.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox2.Location = new System.Drawing.Point(83, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(115, 26);
            this.textBox2.TabIndex = 9;
            this.textBox2.Text = "Simulator";
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.AutoArrange = false;
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1});
            this.listView1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listView1.Font = new System.Drawing.Font("Century", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listView1.ForeColor = System.Drawing.SystemColors.Info;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(5, 129);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(193, 353);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 0;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.SmallIcon;
            this.listView1.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.LV1MouseEnter);
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV1MouseClick);
            this.listView1.MouseEnter += new System.EventHandler(this.showLV1);
            this.listView1.MouseLeave += new System.EventHandler(this.LV1MouseLeave);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LV1KEYUP);
            // 
            // col1
            // 
            this.col1.Width = 193;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.label1.Font = new System.Drawing.Font("Century", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(5, 81);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 48);
            this.label1.TabIndex = 10;
            this.label1.Text = "Turn on devices";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel2.Location = new System.Drawing.Point(178, 129);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(19, 353);
            this.panel2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label2.Font = new System.Drawing.Font("Century", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.ForeColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(920, 485);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Artur Ziemba";
            this.label2.Click += new System.EventHandler(this.showCredits);
            // 
            // listView3
            // 
            this.listView3.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView3.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listView3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listView3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.listView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listView3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listView3.Font = new System.Drawing.Font("Century", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listView3.ForeColor = System.Drawing.SystemColors.Info;
            this.listView3.FullRowSelect = true;
            this.listView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Margin = new System.Windows.Forms.Padding(0);
            this.listView3.MultiSelect = false;
            this.listView3.Name = "listView3";
            this.listView3.OwnerDraw = true;
            this.listView3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listView3.Scrollable = false;
            this.listView3.ShowGroups = false;
            this.listView3.Size = new System.Drawing.Size(300, 228);
            this.listView3.TabIndex = 17;
            this.listView3.TabStop = false;
            this.listView3.TileSize = new System.Drawing.Size(100, 50);
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            this.listView3.Visible = false;
            this.listView3.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.LV3Draw);
            this.listView3.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.LV3MouseEnter);
            this.listView3.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV3MouseClick);
            this.listView3.MouseLeave += new System.EventHandler(this.LV3MouseLeave);
            this.listView3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LV3KEYUP);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "";
            this.columnHeader3.Width = 299;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel4.Controls.Add(this.listView3);
            this.panel4.Location = new System.Drawing.Point(198, 254);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(300, 228);
            this.panel4.TabIndex = 16;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel6.Location = new System.Drawing.Point(198, 81);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(300, 173);
            this.panel6.TabIndex = 15;
            this.panel6.MouseEnter += new System.EventHandler(this.hideLV);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Location = new System.Drawing.Point(-6, -6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(102, 92);
            this.panel1.TabIndex = 1;
            this.panel1.Click += new System.EventHandler(this.DetailedLogs);
            // 
            // TurnOff
            // 
            this.TurnOff.AccessibleName = "TurnOff";
            this.TurnOff.BackColor = System.Drawing.Color.Crimson;
            this.TurnOff.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TurnOff.BackgroundImage")));
            this.TurnOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TurnOff.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TurnOff.Location = new System.Drawing.Point(969, -1);
            this.TurnOff.Name = "TurnOff";
            this.TurnOff.Size = new System.Drawing.Size(29, 28);
            this.TurnOff.TabIndex = 8;
            this.TurnOff.UseVisualStyleBackColor = false;
            this.TurnOff.Click += new System.EventHandler(this.TurnOff_Click);
            // 
            // panel7
            // 
            this.panel7.AutoScroll = true;
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.panel7.Controls.Add(this.label3);
            this.panel7.Location = new System.Drawing.Point(497, 81);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(497, 401);
            this.panel7.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Lime;
            this.label3.Location = new System.Drawing.Point(0, 385);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.MaximumSize = new System.Drawing.Size(497, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(8, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "\r\n";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label3.MouseEnter += new System.EventHandler(this.hideLV);
            // 
            // IoT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1000, 504);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TurnOff);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 504);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 504);
            this.Name = "IoT";
            this.Text = "Form1";
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);

            CheckForIllegalCrossThreadCalls = false;
            Task.Run(() => timers());
        }

        #endregion
        
        private System.Windows.Forms.Button TurnOff;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label textBox1;
        private System.Windows.Forms.Label textBox2;
        private System.Windows.Forms.Panel panel1;
        private ListView listView1;
        private Label label1;
        private ColumnHeader col1;
        private ColumnHeader columnHeader1;
        private ListView listView2;
        private ColumnHeader columnHeader2;
        private Panel panel2;
        private Panel panel5;
        private Label label2;
        private ListView listView3;
        private ColumnHeader columnHeader3;
        private Panel panel4;
        private Panel panel6;
        private Panel panel7;
        private Label label3;
    }
}
