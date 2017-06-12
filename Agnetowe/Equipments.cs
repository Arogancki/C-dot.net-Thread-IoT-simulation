using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Agnetowe;
using Receivers;

namespace Equipments
{ 
    internal static class GlobalVarEquipments
    {
        public static String Done = "DONE";
    }
    class Display: Receiver
    {
        // ORDERS:
        //1- get number of wachers
        //other - get list of wachers
        private List<String[]> watchList;
        private Form3 monitor;
        static private int defalutRefreshTime= 1000;
        static private String defalutName="Display";
        private Semaphore watchListSemaphore;
        public int refreshTime;
        public Display(int _refreshTime) : this(defalutName, _refreshTime)
        {
            Name += id;
            monitor.Text += id;
        }
        public Display(String name) : this(name, defalutRefreshTime) {}
        public Display() : this(defalutName) {}
        public Display(String name, int _refreshTime):base (name)
        {
            refreshTime = _refreshTime;
            watchList = new List<String[]>();
            tasks=new List<Thread>();
            watchListSemaphore= new Semaphore(1, 1);
            monitor=new Form3();
            monitor.StartPosition = FormStartPosition.CenterParent;
            monitor.Text = Name;
            Task.Run(() => monitor.ShowDialog());
        }
        public override void TurnOff()
        {
            base.TurnOff();
            if (monitor != null) monitor.Close();
        }
        public String printDisplay()
        {
            String output = Name +" shows:\n";
            foreach (var i in watchList)
            {
                output += "# "+i[0] + ": " + i[1] + "\n";
            }
            return output;
        }
        public void AddNewPreview(String name)
        {
            if (name == "")
                return;
            if (AddMessageFollow(1, "", name))
            {
                String[] temp = {name, "NA"};
                watchListSemaphore.WaitOne();
                watchList.Add(temp);
                watchListSemaphore.Release();
                makeLogs("Added to watch list: " + name);
            }
            else
                makeLogs("Alreay waiting for respond: " + name);
        }
        public void RemovePreview(String name)
        {
            if (name == "")
                return;
            for (int i = 0; i < watchList.Count; i++)
                if (watchList[i][0] == name)
                {
                    makeLogs("removed from watch list: " + name);
                    watchListSemaphore.WaitOne();
                    watchList.RemoveAt(i);
                    watchListSemaphore.Release();
                    return;
                }
            makeLogs("Could not find: " + name);
        }
        public void Action()
        {
            Task.Run(() => RefreshWatchListStatus());
        }
        private void RefreshWatchListStatus()
        {
            watchListSemaphore.WaitOne();
            foreach (var i in watchList)
            {
                AddMessageFollow(1, "", i[0]);
            }
            watchListSemaphore.Release();
        }
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
            {
                return Name+" watches "+watchList.Count+" devices.";
            }
            //order other
            String output = "My devices:";
            foreach (var device in watchList)
            {
                output += device[0]+": "+device[1]+", ";
            }
            return output;
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (answer == "ERROR")
            {
                answer = "NA";
            }
            foreach (String[] device in watchList)
            {
                if (device[0] == name)
                {
                    if (GlobalVarDevice.DetailedLogs) makeLogs("received "+name+" status: "+answer);
                    device[1] = answer;
                    if (monitor != null) monitor.label1.Text = printDisplay();
                    return;
                }
            }
        }
        // tasks running on timer
        private void TimeTask1()
        {
            IMGstate = true;
            try
            {
                while (listenFlag)
                {
                    RefreshWatchListStatus();
                    Thread.Sleep(refreshTime);
                }
                if (monitor != null) monitor.label1.Text = "";
            }
            catch (Exception) { if (monitor != null) monitor.label1.Text = ""; }
        }
        public override void HandleClick(string button)
        {
            base.HandleClick(button);
            if (button == "Add new preview")
            {
                    AddNewPreview(IoT.GetInputString("Name"));
            }
            else if (button == "Remove preview")
            {
                RemovePreview(IoT.GetInputString("Name"));
            }
            else if (button == "Refresh")
            {
                Action();
                IoT.ShowMessage("Refresh sent");
            }
        }
        protected override void StartTasks()
        {
            tasks.Add(new Thread(TimeTask1));
            tasks[tasks.Count - 1].Start();
        }
    }
    class TempMeter : Receiver
    {
        // ORDERS:
        //1- get tempperature
        //other themp up()heating
        static private int defalutRefreshTime = 200;
        static private String defalutName = "TempMeter";
        private float temp = 18.0f;
        public int refreshTime;
        public TempMeter(int _refreshTime) : this(defalutName, _refreshTime)
        {
            Name += id;
        }
        public TempMeter(String name) : this(name, defalutRefreshTime)
        {
            Name += id.ToString();
        }
        public TempMeter() : this(defalutName) { }
        public TempMeter(String name, int _refreshTime) : base(name)
        {
            refreshTime = _refreshTime;
            tasks = new List<Thread>();
        }
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
                return Math.Round(temp,2).ToString();
            else
            {
                while (temp < float.Parse(argv) && listenFlag)
                {
                    temp += 0.1f;
                    Thread.Sleep(refreshTime/4);
                }
                return GlobalVarEquipments.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            makeLogs("Recived strange results...");
        }
        // tasks running on timer
        private void TimeTask1()
        {
            IMGstate = true;
            try
            {
                while (listenFlag)
                {
                    if (temp > 0.0f)
                        temp -= 0.1f;
                    Thread.Sleep(refreshTime);
                }
            }
            catch (Exception){}
            finally{IMGstate = false;}
        }
        public override void HandleClick(string button)
        {
            base.HandleClick(button);
        }

        protected override void StartTasks()
        {
            tasks.Add(new Thread(TimeTask1));
            tasks[tasks.Count-1].Start();
        }
    }
    class Radiator : Receiver
    {
        // ORDERS:
        //1- is my temp ok
        //2-start heating
        static private int defalutRefreshTime = 500;
        static private String defalutName = "Radiator";
        static private float defalutTempTarget = 20.0f;
        private float tempTarget= defalutTempTarget;
        static private String defalutMyTempMeter = "TempMeter";
        private String myTempMeter = defalutMyTempMeter;
        private bool iSmyTempMeterOk=true;
        public Radiator() : this(defalutName){Name += id;}
        public Radiator(String name) : base(name)
        {
            Name = name;
        }
        public void setTempTarget(float temp)
        {
            tempTarget = temp;
            makeLogs("Target temp set to "+tempTarget);
        }
        public void setTempTarget()
        {
            setTempTarget(defalutTempTarget);
        }
        public void setTempMeter(String name)
        {
            myTempMeter = name;
            makeLogs("TempMeter set to "+name);
        }
        public void setTempMeter()
        {
            setTempMeter(defalutMyTempMeter);
        }
        public void Action()
        {
            Task.Run(() => HandleFollowSpecial(2, ""));
        }
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
            {
                if (iSmyTempMeterOk)
                    return "My temperature is ok";
                else
                    return "My temperature is not ok";
            }
            else
            {
                try
                {
                    if (iSmyTempMeterOk != false)
                    {
                        iSmyTempMeterOk = false;
                        IMGstate = true;
                        makeLogs("Heating started");
                        AddMessageFollow(2, tempTarget.ToString(), myTempMeter); // send message to warm
                    }
                    while (!iSmyTempMeterOk && listenFlag) // check if temperature is stil NOK
                        Thread.Sleep(100);
                }
                catch (Exception){}
                finally {IMGstate = false;}
                return GlobalVarEquipments.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (answer == "ERROR")
            {
                makeLogs("Cooperating devices "+name+" not connected"); // incorrect message or user is not connected
                makeLogs("Heating ended");
                iSmyTempMeterOk = true;
                return;
            }
            if (name == myTempMeter && answer == GlobalVarEquipments.Done)
            {
                iSmyTempMeterOk = true; // if get answer that temp is now ok
                makeLogs("Heating ended");
            }
        }
        public override void HandleClick(string button)
        {
            base.HandleClick(button);
            if (button == "Set temp sensor")
            {
                String _name = IoT.GetInputString("Name");
                if (_name == "")
                    setTempMeter();
                else
                    setTempMeter(_name);
                IoT.ShowMessage("Temp sensor name is " + myTempMeter);
            }
            else if (button == "Set expected temp")
            {
                float teMp= IoT.GetInputFloat("Expected temp", defalutTempTarget);
                    setTempTarget(teMp);
                IoT.ShowMessage("Expected temp is " + tempTarget);
            }
            else if (button == "Heat")
            {
                if (state == Air.State.CONNECTED)
                {
                    if (iSmyTempMeterOk)
                    {
                        Action();
                    }
                    else
                        IoT.ShowMessage("Already heating");
                }
                else
                    IoT.ShowMessage("Radiator not connected");
            }
        }
        protected override void StartTasks()
        {
        }
    }
    class TempRegulator : Receiver
    {
        // ORDERS:
        //1- give isRadiatorWorking or not
        //other check temp
        static private int defalutRefreshTime = 5000;
        static private String defalutName = "TempRegulator";
        static private String defalutMyTempMeter = "TempMeter";
        private String myTempMeter = defalutMyTempMeter;
        static private String defalutMyRadiator = "Radiator";
        private String myRadiator = defalutMyRadiator;
        private bool isRadiatorWorking=false;
        public int refreshTime;
        public TempRegulator(int _refreshTime) : this(defalutName, _refreshTime)
        {
            Name += id;
        }
        public TempRegulator(String name) : this(name, defalutRefreshTime) { }
        public TempRegulator() : this(defalutName) { }
        public TempRegulator(String name, int _refreshTime) : base(name)
        {
            Name = name;
            refreshTime = _refreshTime;
            tasks = new List<Thread>();
        }
        public void setTempMeter(String name)
        {
            myTempMeter = name;
            makeLogs("My temp Meter set to "+name);
        }
        public void setTempMeter()
        {
            setTempMeter(defalutMyTempMeter);
        }
        public void setMyRadiator(String name)
        {
            myRadiator = name;
            makeLogs("My Radiator set to "+name);
        }
        public void setMyRadiator()
        {
            setMyRadiator(defalutMyRadiator);
        }
        public void Action()
        {
            Task.Run(() => HandleFollowSpecial(2, ""));
        }
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
                if (isRadiatorWorking)
                    return myRadiator + " is warming for " + myTempMeter;
                else
                    return myRadiator + " is not warming for " + myTempMeter;
            else
            {
                isRadiatorWorking = true;
                AddMessageFollow(1, "", myTempMeter);
                while (isRadiatorWorking && listenFlag)
                {
                    Thread.Sleep(refreshTime/4);
                }
                return GlobalVarEquipments.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (answer == "ERROR")
            {
                makeLogs("Cooperating devices not connected"); // incorrect message or user is not connected
                isRadiatorWorking = false;
                return;
            }
            if (name==myTempMeter)
                AddMessageFollow(2, answer, myRadiator);
            else if (name == myRadiator && answer == GlobalVarEquipments.Done)
                isRadiatorWorking = false;
        }
        // tasks running on timer
        private void TimeTask1()
        {
            try
            {
                while (listenFlag)
                {
                    AddMessageFollow(1, "", myTempMeter);
                    Thread.Sleep(refreshTime);
                }
            }
            catch (Exception e) { }
        }
        public override void HandleClick(string button)
        {
            base.HandleClick(button);
            if (button == "Attach" && IMGstate==false)
                IMGstate = true;
            else if (button == "Set temp sensor")
            {
                String _name = IoT.GetInputString("Name");
                if (_name == "")
                    setTempMeter();
                else
                    setTempMeter(_name);
                IoT.ShowMessage("Temp sensor name is " + myTempMeter);
            }
            else if (button == "Set radiator")
            {
                String _name = IoT.GetInputString("Name");
                if (_name == "")
                    setMyRadiator();
                else
                    setMyRadiator(_name);
                IoT.ShowMessage("Radiator name is " + myRadiator);
            }
            else if (button == "Refresh")
            {
                if(state==Air.State.CONNECTED)
                { 
                    Action();
                    IoT.ShowMessage("Temperatur will be check");
                }
                else
                    IoT.ShowMessage("Regulator not connected");
        }

        }
        protected override void StartTasks()
        {
            tasks.Add(new Thread(TimeTask1));
            tasks[tasks.Count - 1].Start();
        }
    }
}
