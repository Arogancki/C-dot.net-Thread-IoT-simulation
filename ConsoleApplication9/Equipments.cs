using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Receivers;


namespace Equipments
{ 
    public static class GlobalVar
    {
        public static String Done = "DONE";
    }
    class Display: Receiver
    {
        // ORDERS:
        //1- get number of wachers
        //other - get list of wachers
        private List<String[]> watchList;
        static private int defalutRefreshTime= 10000;
        static private String defalutName="Display";
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
        public void addNewPreview(String name)
        {
            if (AddMessageFollow(1, "", name))
            {
                String[] temp = {name, "NA"};
                watchList.Add(temp);
                makeLogs("Added to watch list: " + name);
            }
            else
                makeLogs("Alreay waiting for respond: " + name);
        }
        public void removePreview(String name)
        {
            for (int i = 0; i < watchList.Count; i++)
                if (watchList[i][0] == name)
                {
                    makeLogs("removed from watch list: " + name);
                    watchList.RemoveAt(i);
                }
        }   
        protected override String HandleFollowSpecial(int order, String argv)
        {
            if (order == 1)
            {
                return Name+" watchs "+watchList.Count+" devices.";
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
                foreach (var i in watchList)
                {
                    AddMessageFollow(1, "", i[0]);
                }
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
                return GlobalVar.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            makeLogs("I haven't asked for anything");
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
        static private int defalutRefreshTime = 1000;
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
        }

        public void setTempTarget()
        {
            tempTarget = defalutTempTarget;
        }

        public void setTempMeter(String name)
        {
            myTempMeter = name;
        }
        public void setTempMeter()
        {
            myTempMeter = defalutMyTempMeter;
        }

        protected override String HandleFollowSpecial(int order, String argv)
        {
            //TODO send message to radiator to warm up
            if (order == 1)
            {
                if (iSmyTempMeterOk)
                    return "My temperature is ok";
                else
                    return "My temperature is not ok";
            }
            else
            {
                iSmyTempMeterOk = false;
                AddMessageFollow(2, tempTarget.ToString(), myTempMeter); // send message to warm
                while (!iSmyTempMeterOk && listenFlag) // check if temperature is stil NOK
                    Thread.Sleep(refreshTime);
                return GlobalVar.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (name == myTempMeter && answer == GlobalVar.Done)
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
        }
        public void setTempMeter()
        {
            myTempMeter = defalutMyTempMeter;
        }

        public void setMyRadiator(String name)
        {
            myRadiator = name;
        }
        public void setMyRadiator()
        {
            myRadiator = defalutMyRadiator;
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
                return GlobalVar.Done;
            }
        }
        protected override void HandleResultSpecial(int orderNumer, String argv, String name, String answer)
        {
            if (name==myTempMeter)
                AddMessageFollow(1, answer, myRadiator);
            else if (name == myRadiator && answer == GlobalVar.Done)
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
