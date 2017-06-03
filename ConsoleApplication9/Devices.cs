using System;
using System.Collections.Generic;
using System.Threading;
using Air;

namespace Devices
{
    public abstract class Device
    {
        public String Name;
        public State state
        {
            get { return _state; }
            private set { _state = value; }
        }
        private State _state;
        public int id
        {
            get { return _id; }
            private set { _id = value; }
        }
        private int _id;
        protected static float maxFreq = 300.0f;
        protected static float[] Bandwidth = {300.0f, 150.0f, 100.0f, 50.0f, 30.0f, 15.0f};
        protected static float defalutFreq = 150.0f;
        protected static int defalutBandwidthIndex = 4;
        private int maxLogsSize = 50;
        private List<String> logs;
        private static int amount;
        public override string ToString()
        {
            return getLogs();
        }
        public void SaveLogsToFile(String filePath)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".txt");
            file.WriteLine(this);
            file.Close();
        }
        public virtual void TurnOff()
        {
            listenFlag = false;
            makeLogs("Turned down");
            try
            {
                listen.Join();
            }
            catch (Exception) { }
        }
        protected Thread listen;
        protected bool listenFlag;
        protected Device()
        {
            id = amount++;
            logs = new List<String>();
            logs.Capacity = maxLogsSize;
            state = State.DISCONNECTED;
            setHeader();
            Name = "Device";
        }
        private void setHeader()
        {
            logs.Add(getName());
        }
        protected void makeLogs(String log)
        {
            if (logs.Count > maxLogsSize)
                logs.RemoveAt(1);
            //logs.Add(DateTime.Now.ToString("yy-MM-dd hh:mm:ss:ffff ")+log);
            logs.Add(DateTime.Now.ToString("ss:ffff ") + log);
        }
        private String getLogs()
        {
            String output = "";
            foreach (var line in logs)
                output += line + "\n";
            return output;
        }
        private string getName()
        {
            return this.GetType().Name + " - id:" + id;
        }
        protected void changeState(State inState)
        {
            makeLogs("Change state form " + state + " to " + inState);
            state = inState;
        }
    }
}
