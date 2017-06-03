using System;
using System.Collections.Generic;
using System.Threading;
using Receivers;

// ORDERS:
//1- get number of wachers
//other - get list of wachers

namespace Equipments
{ 
    class Display: Receiver
    {
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
            waitingForRespond = new LinkedList<string>();
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
}
