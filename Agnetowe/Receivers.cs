using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agnetowe;
using Air;
using Devices;

namespace Receivers
{
    abstract class Receiver: Device
    {
        protected LinkedList<String> waitingForRespond; // sended request that are waiting for respond
        protected List<Thread> tasks; // equipment specyfic tasks
        private LinkedList<Data> messages; // messages to send
        private AirInterface.Station station;
        private int sender;
        private Thread listen;
        private long milliseconds;//actual time
        private int timeWait; // gap between meseagers
        private int timeSingle; // time for one way transmission
        private int attachTry;
        private static int maxAttachTry=3;
        protected Receiver(string _Name)
        {
            if (_Name == "")
                Name = "Receiver" + id;
            else
               Name = _Name;
            listenFlag = true;
            attachTry = 0;
            messages = new LinkedList<Data>();
            tasks = new List<Thread>();
            waitingForRespond = new LinkedList<string>();
            IMGstate = false;
        }
        ~Receiver()
        {
            TurnOff();
        }
        private AirInterface.Station SearchTransmission(float start, float end)
        {
            return AirInterface.SearchTransmission(start, end);
        }
        public void ConnectToStation(float start, float end)
        {
            if (state != State.DISCONNECTED)
            {
                IoT.ShowMessage("Already connected");
                makeLogs("Already connected");
                return;
            }
            try
            {
                AirInterface.Station found = SearchTransmission(start, end);
                if (found.quality > 0)
                {
                    station = found;
                    makeLogs("Connected on " + station.bandwidthStart + "-" + station.bandwidthEnd + "MHz quality: " +
                             station.quality);
                    changeState(State.CONNECTED);
                    IoT.ShowMessage("Station found");
                    listenFlag = true;
                    StartTasks();
                }
                else
                {
                    makeLogs("Station does not exist");
                    IoT.ShowMessage("Station does not exist");
                }
            }
            catch (Exception ex)
            {
                makeLogs("Incorrect Station");
                IoT.ShowMessage("Station does not exist");
            }
        }
        public void BlindDecoding()
        {
            if (state != State.DISCONNECTED)
            {
                IoT.ShowMessage("Already connected");
                makeLogs("Already connected");
                return;
            }
            float bestQuality = 0.0f;
            foreach (var bandwidthIndex in Bandwidth)
                for (float start = 0.0f; start < maxFreq; start += bandwidthIndex)
                {
                    AirInterface.Station found = SearchTransmission(start, start + bandwidthIndex);
                    if (found.quality > bestQuality)
                    {
                        station = found;
                        bestQuality = station.quality;
                    }
                }
            if (bestQuality > 0.0f)
            { 
                makeLogs("Connected on " + station.bandwidthStart + "-" + station.bandwidthEnd+"MHz quality: "+ station.quality);
                changeState(State.CONNECTED);
                IoT.ShowMessage("Station found");
                listenFlag = true;
                StartTasks();
            }
            else
            {
                makeLogs("Station not found");
                IoT.ShowMessage("Station not found");
            }
        }
        public void Attach()
        {
            attachTry = 0;
            messages.Clear();
            waitingForRespond.Clear();
            if (state!=State.CONNECTED) // if it's unconnected
            {
                makeLogs("Unable to attach: state: "+state+". Station not declared");
                IoT.ShowMessage("No station");
                return;
            }
            Data info;
            info.id = id;
            info.state = 0;
            info.description = Name;
            info.direction = Direction.UL;
            station.channel.SendAttach(ref info);
            if (info.id==id) // something is wrong
            {
                if (info.id < 0)
                {
                    makeLogs("Unable to attach. Disconnecting");
                    Disconnect(info.description);
                    return;
                }
                if (attachTry++ < maxAttachTry)
                {
                    makeLogs("Unable to attach. Disconnecting");
                    Disconnect("Unable to Attach");
                    return;
                }
                makeLogs("Attach unhandled, restarting attach " + state);
                Attach();
                return;
            }
            attachTry = 0;
            sender = info.id;
            String[] times = info.description.Split('#');
            timeSingle = Int32.Parse(times[0]);
            timeWait = Int32.Parse(times[1]);
            milliseconds= Int64.Parse(times[2]); // first time it have to be deducted from timeWait to synchronization with sender
                                                 //Console.WriteLine("powinien o:"+ ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds));
            listen = new Thread(Listen);
            listen.Start();
            makeLogs("Attach successful: SingleMessage time: " + timeSingle + "ms, gap time: " + timeWait + "ms");
            IoT.ShowMessage("Attach successful");
        }
        public override void TurnOff()
        {
            IMGstate = false;
            base.TurnOff();
            foreach (Thread task in tasks)
            {
                try
                {
                    task.Interrupt();
                    task.Join();
                }
                catch (Exception) { }
            }
        }
        public override void Disconnect()
        {
            Disconnect("Unresolved");
        }
        public void Disconnect(String descryption)
        {
            IMGstate = false;
            listenFlag = false;
            changeState(State.DISCONNECTED);
            makeLogs("Disconnecting cause: " + descryption);
        }
        protected abstract String HandleFollowSpecial(int order, String argv);
        protected abstract void HandleResultSpecial(int orderNumer, String argv, String name, String answer);
        private void Listen()
        {
            try
            {
                Data DL = new Data();
                Data UL = new Data();
                while (listenFlag)
                {
                    try
                    {
                        Thread.Sleep(timeWait - (int) ((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    } // no need to handle -time exception, just do NOT wait
                    milliseconds = (DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond); // set next synchronization point

                    if (GlobalVarDevice.DetailedLogs) makeLogs("traing to get message");
                    GetMessage(timeSingle);

                    try
                    {
                        Thread.Sleep(timeSingle -
                                     (int) ((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    } // no need to handle -time exception, just do NOT wait
                    milliseconds = (DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond); // set next synchronization point

                    if (GlobalVarDevice.DetailedLogs) makeLogs("traing to send message");
                    SendMessage();

                    try
                    {
                        Thread.Sleep(timeSingle -
                                     (int) ((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    } // no need to handle -time exception, just do NOT wait
                    milliseconds = (DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond); // set next synchronization point
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (retry)
                    ReAttach();
            }
        }
        private void SendMessage()
        {
            if (messages.Count > 0) // if there is message to send
            {
                station.channel.setChannel(messages.First());
                messages.RemoveFirst();
                if (GlobalVarDevice.DetailedLogs) makeLogs("Message sent to " + sender);
            }
            else
            {
                station.channel.setChannel(id, State.CONNECTED, "", Direction.UL); // send defalut message
                //makeLogs("Message sent to " + sender);
            }
        }
        private void GetMessage(int timeOut)
        {
            Data message;
            try
            {
                message = station.channel.getChanel(timeOut);
            }
            catch (SemaphoreFullException e)
            {
                makeLogs("Station didn't answer. Disconnecting from station");
                Disconnect("Station didn't answer.");
                retry = true;
                return;
            }

            if (message.id != id || message.direction != Direction.DL) // odebrano wiadomosc nie dla mnie
            {
                makeLogs("Received message for "+ message.id+" in "+message.direction+". Disconnecting from station");
                Disconnect("Received message for " + message.id + " in " + message.direction + ".");
                retry = true;
            }
            else
            {
                switch (message.state) 
                {
                    case State.CONNECTED:
                        if (GlobalVarDevice.DetailedLogs) makeLogs("Received: Nothing to do");
                        break;
                    case State.DISCONNECTED:
                        Disconnect("Station shut down. Disconnecting");
                        break;
                    case State.FOLLOW:
                        Task.Run(() => HandleFollow(message.description));
                        break;
                    case State.RESULTS:
                        Task.Run(() => HandleResults(message.description));
                        break;
                }
                // update time:
                milliseconds = Int64.Parse(message.description.Split('#').Last());
            }
        }
        private void HandleResults(String input)
        {
            try
            {
                // recived results is rozkaz.argv.nameslave.odp.master.synchtime
                String[] parameters = input.Split('#');
                String command = "";
                int i = 0;
                for (; i < parameters.Length - 3; i++)
                {
                    if (i != 0) command += "#";
                    command += parameters[i];
                }
                if (!waitingForRespond.Remove(command))
                {
                    makeLogs("Received missmatched results: " + input);
                    return;
                }
                makeLogs("Received results: " + command + " ans: " + parameters[3]);
                HandleResultSpecial(Int32.Parse(parameters[0]), parameters[1], parameters[2], parameters[3]);
            }
            catch (Exception e) { }
        }
        private void HandleFollow(String input)
        {
            try
            {
                // otrzymany rozkaz to NUMER ROZKAZU.argumenty.NazwaOtzyujacego.idWysylajacego.czasSynchronizacji
                // lub 0.NewTimeSingle.NewTimeWait
                String[] parameters = input.Split('#');
                if (parameters[0] == "0")
                {
                    makeLogs("Reconfiguration complete: Single:" + timeSingle + " to " + parameters[1] + " Gap:" +
                             timeWait + " to " + parameters[2]);
                    timeSingle = Int32.Parse(parameters[1]);
                    timeWait = Int32.Parse(parameters[2]);
                }
                else
                {
                    String[] parametersC = input.Split('#');
                    makeLogs("Command: " + parametersC[0] +" arg: " + parametersC[1]);
                    AddMessageResults(input,
                        HandleFollowSpecial(Int32.Parse(parametersC[0]), parametersC[1]) + "#" + parameters[3]);
                }
            }
            catch (Exception e) { }
        }
        protected bool AddMessageFollow(int orderNumer, String argv, String name)
        {
            //wysylana odp to NUMER ROZKAZU.argumenty.NazwaOtzyujacego.odp.master
            String order = orderNumer + "#" + argv + "#" + name;
            Data temp = new Data();
            temp.id = id;
            temp.state = State.FOLLOW;
            temp.description = order;
            temp.direction = Direction.UL;
            if (!waitingForRespond.Contains(order))
            {
                waitingForRespond.AddLast(temp.description);
            }
            else
                if (GlobalVarDevice.DetailedLogs) makeLogs("Still havent got respond for: " + order);
            messages.AddLast(temp);
            if (GlobalVarDevice.DetailedLogs) makeLogs("New message added: " + order);
            return true;
        }
        protected void AddMessageResults(String receiverOrder, String answer)
        {
            // SKLADNIA Dodawnaych resultatow string_rozkazu.odpowiedz
            String[] parameters = receiverOrder.Split('#');
            String command = "";
            int i = 0;
            for (; i < parameters.Length - 2; i++)
            {
                if (i != 0) command += "#";
                command += parameters[i];
            }
            String order = command + "#" + answer;
            Data temp = new Data();
            temp.id = id;
            temp.state = State.RESULTS;
            temp.description = order;
            temp.direction = Direction.UL;
            messages.AddLast(temp);
            makeLogs("Respond sent: "+answer+" for "+ command);
        }
        public override void HandleClick(string button)
        {
            if (button == "Search Station")
            {
                ConnectToStation(IoT.GetInputFloat("Bandwidth begining", 0.0f), IoT.GetInputFloat("Bandwidth end",0.0f));
            }
            else if (button == "Blind search")
            {
                BlindDecoding();
            }
            else if (button == "Attach")
            {
                Attach();
            }
            else if (button == "Disconnect")
            {
                if (state != State.DISCONNECTED)
                {
                    Disconnect();
                    IoT.ShowMessage("Disconnected");
                }
                else
                    IoT.ShowMessage("Already disconnected");
            }
        }
        protected abstract void StartTasks();
        private bool retry = false;
        private void ReAttach()
        {
            retry = false;
            Data info;
            info.id = id;
            info.state = 0;
            info.description = Name;
            info.direction = Direction.UL;
            station.channel.SendAttach(ref info);
            if (info.id==id) // something is wrong
            {
                if (info.id < 0)
                {
                    makeLogs("Unable to reattach. Lost Connection");
                    return;
                }
                if (attachTry++ < maxAttachTry)
                {
                    makeLogs("Unable to reattach. Lost Connection");
                    return;
                }
                ReAttach();
                return;
            }
            attachTry = 0;
            listenFlag = true;
            sender = info.id;
            String []
            times = info.description.Split('#');
            timeSingle = Int32.Parse(times [0]);
            timeWait = Int32.Parse(times [1]);
            milliseconds= Int64.Parse(times [2]); // first time it have to be deducted from timeWait to synchronization with sender
                                                  //Console.WriteLine("powinien o:"+ ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds));
            makeLogs("Reconnection successful: SingleMessage time: " + timeSingle + "ms, gap time: " + timeWait + "ms");
            StartTasks();
            Listen();
        }
    }
}