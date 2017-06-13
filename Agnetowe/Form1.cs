using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Senders;
using Devices;
using Equipments;
using Label = System.Reflection.Emit.Label;

namespace Agnetowe
{
    public partial class IoT : Form
    {
        private bool on = true;
        static private Device device = null;
        static private List<String> logs=new List<string>();
        private Bitmap ImgState0 = null;
        private Bitmap  ImgState1 = null;

        static public void updateLogs(String line,Device updater)
        {
            if (device !=null && updater==device)
                logs.Add(line);
        }

        private void DetailedLogs(object s,EventArgs e)
        {
            Device.GlobalVarDevice.DetailedLogs = !Device.GlobalVarDevice.DetailedLogs;
            panel1.BackgroundImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            panel1.Refresh();
        }

        private void showCredits(object o, EventArgs e)
        {
            ShowMessage("Creator Artur Ziemba");
        }

        public static String GetInputString(String Print)
        {
            Form2 messboxForm2 = new Form2();
            messboxForm2.StartPosition = FormStartPosition.CenterParent;
            messboxForm2.title.Text = Print;
            messboxForm2.GetText.Text = "";
            DialogResult result = messboxForm2.ShowDialog();
            if (result == DialogResult.OK)
            {
                return(messboxForm2.GetText.Text);
            }
            return "";
        }
        public static int GetInputInt(String Print, int defalutInt)
        {
            try
            {
                defalutInt = Int32.Parse(GetInputString(Print));
            }
            catch (Exception)
            {}
            return defalutInt;
        }
        public static float GetInputFloat(String Print, float defalutFloat)
        {
            try
            {
                defalutFloat = float.Parse(GetInputString(Print));
            }
            catch (Exception)
            {}
            return defalutFloat;
        }

        public static void ShowMessage(String msg)
        {
            Form2 messboxForm2 = new Form2();
            messboxForm2.StartPosition = FormStartPosition.CenterParent;
            messboxForm2.title.Text = msg;
            messboxForm2.GetText.Visible = false;
            DialogResult result = messboxForm2.ShowDialog();
            if (result == DialogResult.OK)
            {
                return;
            }
        }

        private bool forceSelect = false;

        private void LV1MouseClick(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (forceSelect || !e.IsSelected)
                return;
            e.Item.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                ((int) (((byte) (32)))));
            try
            {
                string name = GetInputString("Name");
                if (name!=""&&listView2.FindItemWithText(name) != null)
                    throw new Exception("Name already taken");
                if (e.Item.Text == "Router")
                {
                    device = new Sender(GetInputInt("Short transmission time", 250),
                        GetInputInt("Long transmission time", 500), name);
                    ShowMessage(device.Name + " created");
                    NewButtonList2(device.Name);
                }
                else if (e.Item.Text == "Monitor")
                {
                    if (name == "")
                        device = new Display(GetInputInt("Refresh Time", 5000));
                    else
                        device = new Display(name, GetInputInt("Refresh Time", 5000));
                    ShowMessage(device.Name + " created");
                    NewButtonList2(device.Name);
                }
                else if (e.Item.Text == "Temp meter")
                {
                    if (name == "")
                        device = new TempMeter(GetInputInt("Refresh Time", 200));
                    else
                        device = new TempMeter(name, GetInputInt("Refresh Time", 200));
                    ShowMessage(device.Name + " created");
                    NewButtonList2(device.Name);
                }
                else if (e.Item.Text == "Radiator")
                {
                    if (name == "")
                        device = new Radiator();
                    else
                        device = new Radiator(name);
                    ShowMessage(device.Name + " created");
                    NewButtonList2(device.Name);
                }
                else if (e.Item.Text == "Temp regulator")
                {
                    if (name == "")
                        device = new TempRegulator(GetInputInt("Refresh Time", 200));
                    else
                        device = new TempRegulator(name, GetInputInt("Refresh Time", 200));
                    ShowMessage(device.Name + " created");
                    NewButtonList2(device.Name);
                }
                ;
            }
            catch (NullReferenceException)
            {}
            catch (Exception se)
            {
                ShowMessage(se.Message);
                return;
            }
        }

        private void LV2MouseClick(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (forceSelect)
                return;
            logs.Clear();
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                forceSelect = true;
                listView2.Items[i].Selected = false;
                forceSelect = false;
                listView2.Items[i].Font = new System.Drawing.Font("Century", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            }
            listView2.Items.RemoveAt(listView2.Items.IndexOf(listView2.FindItemWithText(e.Item.Text)));
            listView2.Items.Insert(0, e.Item.Text);
            listView2.Items[0].Font =
                    new System.Drawing.Font("Century", 10.75F, System.Drawing.FontStyle.Bold,
                        System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            listView2.Items[0].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))),
                ((int)(((byte)(48)))));

            device= Device.DeviceContainer.FindByName(e.Item.Text);
            logs.Clear();
            label3.Text = device.ToString();
            listView3.Visible = true;
            listView3.Items.Clear();
            if (device.GetType().Name == ("Sender"))
            {
                String[] commands = { "Start transmission", "Disconnect", "Turn Off" };
                foreach (String command in commands)
                {
                    this.listView3.Items.AddRange(new System.Windows.Forms.ListViewItem[]
                    {
                        new System.Windows.Forms.ListViewItem(new string[] {command}, -1,
                            System.Drawing.SystemColors.Info,
                            System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                                ((int) (((byte) (32))))), null)
                    });
                }
                ImgState0 = Properties.Resources.sender1;
                ImgState1 = Properties.Resources.sender2;
            }
            else
            if (device.GetType().Name == ("Display"))
            {
                String[] commands = { "Search Station","Blind search","Attach","Add new preview", "Remove preview", "Refresh","Disconnect","Turn Off" };
                foreach (String command in commands)
                {
                    this.listView3.Items.AddRange(new System.Windows.Forms.ListViewItem[]
                    {
                        new System.Windows.Forms.ListViewItem(new string[] {command}, -1,
                            System.Drawing.SystemColors.Info,
                            System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                                ((int) (((byte) (32))))), null)
                    });
                }
                ImgState0 = Properties.Resources.monitor1;
                ImgState1 = Properties.Resources.monitor2;
            }
            else
            if (device.GetType().Name == ("TempMeter"))
            {
                String[] commands = { "Search Station", "Blind search", "Attach", "Disconnect", "Turn Off" };
                foreach (String command in commands)
                {
                    this.listView3.Items.AddRange(new System.Windows.Forms.ListViewItem[]
                    {
                        new System.Windows.Forms.ListViewItem(new string[] {command}, -1,
                            System.Drawing.SystemColors.Info,
                            System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                                ((int) (((byte) (32))))), null)
                    });
                }
                ImgState0 = Properties.Resources.temp1;
                ImgState1 = Properties.Resources.temp2;
            }
            else
            if (device.GetType().Name == ("Radiator"))
            {
                String[] commands = { "Search Station", "Blind search", "Attach", "Set temp sensor", "Set expected temp", "Heat", "Disconnect", "Turn Off" };
                foreach (String command in commands)
                {
                    this.listView3.Items.AddRange(new System.Windows.Forms.ListViewItem[]
                    {
                        new System.Windows.Forms.ListViewItem(new string[] {command}, -1,
                            System.Drawing.SystemColors.Info,
                            System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                                ((int) (((byte) (32))))), null)
                    });
                }
                ImgState0 = Properties.Resources.radiator1;
                ImgState1 = Properties.Resources.radiator2;
            }
            else
            if (device.GetType().Name == ("TempRegulator"))
            {
                String[] commands = { "Search Station", "Blind search", "Attach", "Set temp sensor", "Set radiator", "Refresh", "Disconnect", "Turn Off" };
                foreach (String command in commands)
                {
                    this.listView3.Items.AddRange(new System.Windows.Forms.ListViewItem[]
                    {
                        new System.Windows.Forms.ListViewItem(new string[] {command}, -1,
                            System.Drawing.SystemColors.Info,
                            System.Drawing.Color.FromArgb(((int) (((byte) (32)))), ((int) (((byte) (32)))),
                                ((int) (((byte) (32))))), null)
                    });
                }
                ImgState0 = Properties.Resources.tempregulator1;
                ImgState1 = Properties.Resources.tempregulator2;
            }
        }
        private void LV3MouseClick(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (forceSelect)
                return;
            e.Item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                 ((int)(((byte)(32)))));
            if (e.Item.Text == "Turn Off")
            {
                try
                {
                    TurnOffDevice();
                }catch (Exception){}
                return;
            }
            device.HandleClick(e.Item.Text);
        }

        private void TurnOffDevice()
        {
            device.TurnOff(); 

            listView2.Items.RemoveAt(0);
            if (listView2.Items.Count > 0)
            {
                listView2.Items[0].Selected = true;
            }
            else
            {
                ImgState0 = null;
                ImgState1 = null;
                device = null;
                logs.Clear();
                label3.Text = "";
                listView3.Items.Clear();
                panel6.BackgroundImage = null;
            }

        }
        private void NewButtonList1(String name)
        {
            listView1.Items.Add(name);
        }
        private void NewButtonList2(String Name)
        {
            listView2.Items.Add(Name).Selected=true;
        }

        private void showLV1(object sender, EventArgs e)
        {
            listView1.BringToFront();
            panel2.SendToBack();
        }

        private void showLV2(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 8)
            {
                panel6.SendToBack();
                label3.SendToBack();
                panel3.BringToFront();
            }
        }

        private void LV1MouseEnter(object sender, ListViewItemMouseHoverEventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
            e.Item.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))),
                ((int) (((byte) (64)))));
        }

        private void LV2MouseEnter(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (listView2.Items.Count > 8)
            {
                panel6.SendToBack();
                label3.SendToBack();
                panel3.BringToFront();
            }
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                listView2.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
            if (!e.Item.Selected)
               e.Item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))),
                 ((int)(((byte)(64)))));
        }

        private void LV3MouseEnter(object sender, ListViewItemMouseHoverEventArgs e)
        {
            for (int i = 0; i < listView3.Items.Count; i++)
            {
                listView3.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
            e.Item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))),
                ((int)(((byte)(64)))));
        }

        private void LV1MouseLeave(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
        }

        private void LV3MouseLeave(object sender, EventArgs e)
        {
            for (int i = 0; i < listView3.Items.Count; i++)
            {
                listView3.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
        }

        private void LV2MouseLeave(object sender, EventArgs e)
        {
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                listView2.Items[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))),
                ((int)(((byte)(32)))));
            }
        }

        private void hideLV(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 8)
            {
                panel6.BringToFront();
                label3.BringToFront();
                panel3.SendToBack();
                panel2.BringToFront();
                listView1.SendToBack();
            }
        }

        private void LV2Draw(object s, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
            e.Graphics.DrawRectangle(Pens.Black, e.Item.Bounds);
        }

        private void LV3Draw(object s, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
            e.Graphics.DrawRectangle(Pens.Black, e.Item.Bounds);
        }

        public IoT()
        {
            InitializeComponent();
            NewButtonList1("Router");
            NewButtonList1("Monitor");
            NewButtonList1("Radiator");
            NewButtonList1("Temp regulator");
            NewButtonList1("Temp meter");
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

        private void TurnOff_Click(object sender, EventArgs e)
        {
            on = false;
            Application.Exit();
        }

        private void LV1KEYUP(object sender, MouseEventArgs e)
        {
            forceSelect = true;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Selected = false;
            }
            forceSelect = false;
        }

        private void LV3KEYUP(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < listView3.Items.Count; i++)
            {
                forceSelect = true;
                listView3.Items[i].Selected = false;
                forceSelect = false;
            }
        }
        private void timers()
        {
            while (on)
            {
                try
                {
                    if (device != null)
                    {
                        if (ImgState0 != null && ImgState1 != null)
                            if (device.IMGstate)
                                panel6.BackgroundImage = ImgState1;
                            else
                                panel6.BackgroundImage = ImgState0;
                        else
                        {

                        }
                        while (logs.Count > 0)
                        {
                            label3.Text += logs.First() + "\n";
                            logs.RemoveAt(0);
                        }
                    }
                }
                catch (Exception e) { }
                Thread.Sleep(10);
            }
        }
    }
}

//     CheckForIllegalCrossThreadCalls = false;
//   Task.Run(() => timers());