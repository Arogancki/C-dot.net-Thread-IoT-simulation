using System;
using System.Collections.Generic;
using System.Threading;
using Air;

namespace Devices
{
    public abstract class Device
    {
        public static class GlobalVarDevice
        {
            public static bool DetailedLogs = false;
        }
        public static class DeviceContainer 
        {
            private static String logFile = Environment.CurrentDirectory + "\\..\\..\\..\\logi";

            private static List<Device> Container = new List<Device>();
            public static List<Device> getContainter()
            {
                return Container;
            }

            public static Device FindByName(String name)
            {
                 foreach (var device in Container)
                 {
                     if (name == device.Name)
                         return device;
                 }
                return null;
            }

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

        public bool IMGstate = false;
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
        protected Thread listen;
        protected bool listenFlag;
        private int maxLogsSize = 40;
        private List<String> logs;
        private static int amount=1;
        public override string ToString()
        {
            return getLogs();
        }
        public void SaveLogsToFile(String filePath)
        {
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".txt");
                file.WriteLine(this);
                file.Close();
            }
            catch (Exception)
            {
                try
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Name + ".txt");
                    file.WriteLine(this);
                    file.Close();
                }
                catch (Exception)
                {
                }
            }
        }
        public virtual void TurnOff()
        {
            if (DeviceContainer.Contains(this))
                DeviceContainer.Remove(this);
        }
        public abstract void Disconnect();
        internal void hardTurnOff()
        {
            Disconnect();
            makeLogs("Turned down");
            try
            {
                listen.Interrupt();
                listen.Join();
            }
            catch (Exception) { }
        }
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
            if (logs.Count > maxLogsSize && maxLogsSize!=-1)
                logs.RemoveAt(1);
            String temp = DateTime.Now.ToString("yy-MM-dd hh:mm:ss:ffff ") + log;
            //String temp = DateTime.Now.ToString("mm:ss:ffff ") + log;
            logs.Add(temp);
            Agnetowe.IoT.updateLogs(temp, this);
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
        public abstract void HandleClick(string button);
    }
}
