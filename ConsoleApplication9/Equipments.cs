using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        static private int defalutRefreshTime= 1000;
        static private String defalutName="Display";
        private Semaphore watchListSemaphore;
        public int refreshTime;
        public Display(int _refreshTime) : this(defalutName, _refreshTime) {}
        public Display(String name) : this(name, defalutRefreshTime) {}
        public Display() : this(defalutName) {}
        public Display(String name, int _refreshTime)
        {
            Name = name;
            refreshTime = _refreshTime;
            watchList = new List<String[]>();
            tasks=new List<Thread>();
            watchListSemaphore= new Semaphore(1, 1);
            tasks.Add(new Thread(TimeTask1));
            tasks[0].Start();
        }
        public String printDisplay()
        {
            String output = "";
            foreach (var i in watchList)
            {
                output += i[0] + ": " + i[1] + "\n";
            }
            return output;
        }
        public void AddNewPreview(String name)
        {
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
            for (int i = 0; i < watchList.Count; i++)
                if (watchList[i][0] == name)
                {
                    makeLogs("removed from watch list: " + name);
                    watchListSemaphore.WaitOne();
                    watchList.RemoveAt(i);
                    watchListSemaphore.Release();
                }
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
                    makeLogs("received "+name+" status: "+answer);
                    device[1] = answer;
                    return;
                }
            }
        }
        // tasks running on timer
        private void TimeTask1()
        {
            while (listenFlag)
            {
                RefreshWatchListStatus();
                Thread.Sleep(refreshTime);
            }
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
        public TempMeter(int _refreshTime) : this(defalutName, _refreshTime) { }
        public TempMeter(String name) : this(name, defalutRefreshTime) { }
        public TempMeter() : this(defalutName) { }
        public TempMeter(String name, int _refreshTime)
        {
            Name = name;
            refreshTime = _refreshTime;
            tasks = new List<Thread>();
            tasks.Add(new Thread(TimeTask1));
            tasks[0].Start();
        }
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
                return temp.ToString();
            else
            {
                while (temp > float.Parse(argv) && listenFlag)
                {
                    temp += 0.4f;
                    Thread.Sleep(refreshTime);
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
            while (listenFlag)
            {
                if (temp>0.0f)
                    temp -= 0.1f;
                Thread.Sleep(refreshTime);
            }
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
        public int refreshTime;
        public Radiator(int _refreshTime) : this(defalutName, _refreshTime) { }
        public Radiator(String name) : this(name, defalutRefreshTime) { }
        public Radiator() : this(defalutName) { }
        public Radiator(String name, int _refreshTime)
        {
            Name = name;
            refreshTime = _refreshTime;
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
                        AddMessageFollow(2, tempTarget.ToString(), myTempMeter); // send message to warm
                    }
                    while (!iSmyTempMeterOk && listenFlag) // check if temperature is stil NOK
                        Thread.Sleep(refreshTime);
                }
                catch (Exception){}
                return GlobalVarEquipments.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (answer == "ERROR")
            {
                makeLogs("Cooperating devices not connected"); // incorrect message or user is not connected
                iSmyTempMeterOk = true;
            }
            if (name == myTempMeter && answer == GlobalVarEquipments.Done)
                iSmyTempMeterOk = true; // if get answer that temp is now ok
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
        public TempRegulator(int _refreshTime) : this(defalutName, _refreshTime) { }
        public TempRegulator(String name) : this(name, defalutRefreshTime) { }
        public TempRegulator() : this(defalutName) { }
        public TempRegulator(String name, int _refreshTime)
        {
            Name = name;
            refreshTime = _refreshTime;
            tasks = new List<Thread>();
            tasks.Add(new Thread(TimeTask1));
            tasks[0].Start();
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
                    return myRadiator + " is warming " + myTempMeter;
                else
                    return myRadiator + " is not warming " + myTempMeter;
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
            }
            if (name==myTempMeter)
                AddMessageFollow(2, answer, myRadiator);
            else if (name == myRadiator && answer == GlobalVarEquipments.Done)
                isRadiatorWorking = false;
        }
        // tasks running on timer
        private void TimeTask1()
        {
            while (listenFlag)
            {
                AddMessageFollow(1, "", myTempMeter);
                Thread.Sleep(refreshTime);
            }
        }
    }
}
