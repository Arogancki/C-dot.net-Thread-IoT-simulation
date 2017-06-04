using System;
using System.Collections.Generic;
using System.Threading;
using Air;

namespace Devices
{
    public abstract class Device
    {
        internal static class GlobalVarDevice
        {
            internal static bool DetailedLogs = false;
        }
        public static class DeviceContainer 
        {
            private static String logFile = Environment.CurrentDirectory + "\\..\\..\\..\\logi";
            private static List<Device> Container=new List<Device>();
            public static void AddNew(Device device)
            {
                Container.Add(device);
            }
            internal static void Remove(Device device)
            {
                device.hardTurnOff();
                Container.Remove(device);
                device.SaveLogsToFile(logFile+"\\"+device.Name);
            }
            internal static bool Contains(Device device)
            {
                return Container.Contains(device);
            }
            public static void RemoveAll()
            {
                while (Container.Count>0)
                    DeviceContainer.Remove(Container[0]);
            }
        }
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
            if (DeviceContainer.Contains(this))
                DeviceContainer.Remove(this);
        }
        internal void hardTurnOff()
        {
            listenFlag = false;
            if (state!=State.DISCONNECTED)
                changeState(State.DISCONNECTED);
            makeLogs("Turned down");
            try
            {
                listen.Interrupt();
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
            DeviceContainer.AddNew(this);
        }
        private void setHeader()
        {
            logs.Add(getName());
        }
        protected void makeLogs(String log)
        {
            if (logs.Count > maxLogsSize)
                logs.RemoveAt(1);
            logs.Add(DateTime.Now.ToString("yy-MM-dd hh:mm:ss:ffff ")+log);
            //logs.Add(DateTime.Now.ToString("ss:ffff ") + log);
        }
        private String getLogs()
        {
            String output = "";
            for (int i=0;i<logs.Count;i++)
                output += logs[i] + "\n";
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
