using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        protected Receiver()
        {
            Name = "Receiver" + id;
            listenFlag = true;
            attachTry = 0;
            messages = new LinkedList<Data>();
            tasks = new List<Thread>();
            waitingForRespond = new LinkedList<string>();
        }
        ~Receiver()
        {
            TurnOff();
        }
        public AirInterface.Station SearchTransmission(float start, float end)
        {
            return AirInterface.SearchTransmission(start, end);
        }
        public void ConnectToStation(AirInterface.Station found)
        {
            try
            {
                SearchTransmission(found.bandwidthStart, found.bandwidthEnd);
                if (found.quality > 0)
                {
                    station = found;
                    makeLogs("Connected on " + station.bandwidthStart + "-" + station.bandwidthEnd + "MHz quality: " +
                             station.quality);
                    changeState(State.CONNECTED);
                }
            }
            catch (Exception ex)
            {
                makeLogs("Incorrect Station");
            }
        }
        public void BlindDecoding()
        {
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
            }
        }
        public void Attach()
        {
            if (state!=State.CONNECTED) // if it's unconnected
            {
                makeLogs("Unable to attach: state: "+state+". Station not declared");
                return;
            }
            Data info;
            info.id = id;
            info.state = 0;
            info.description = Name;
            info.direction = Direction.UL;
            station.channel.SendAttach(ref info);
            if (info.id==id)
            {
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
        }
        public override void TurnOff()
        {
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
        public void Disconnect()
        {
            Disconnect("Unresolved");
        }
        public void Disconnect(String descryption)
        {
            listenFlag = false;
            changeState(State.DISCONNECTED);
            makeLogs("Disconnecting cause: " + descryption);
            Data temp = new Data();
            temp.id = id;
            temp.state = State.DISCONNECTED;
            temp.description = descryption; // 0 - means reconfiguration
            messages.AddFirst(temp);
        }
        protected abstract String HandleFollowSpecial(int order, String argv);
        protected abstract void HandleResultSpecial(int orderNumer, String argv, String name, String answer);
        private void Listen()
        {
            Data DL = new Data();
            Data UL = new Data();
            while (listenFlag)
            {
                try {
                    Thread.Sleep(timeWait - (int) ((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait
                milliseconds =(DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond); // set next synchronization point

                if (GlobalVarDevice.DetailedLogs) makeLogs("traing to get message");
                GetMessage(timeSingle);

                try { 
                    Thread.Sleep(timeSingle - (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait
                milliseconds = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); // set next synchronization point

                if (GlobalVarDevice.DetailedLogs) makeLogs("traing to send message");
                SendMessage();

                try {
                    Thread.Sleep(timeSingle - (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds));
                    }
                    catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait
                milliseconds = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); // set next synchronization point
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
                return;
            }

            if (message.id != id || message.direction != Direction.DL) // odebrano wiadomosc nie dla mnie
            {
                makeLogs("Received message for "+ message.id+" in "+message.direction+". Disconnecting from station");
                Disconnect("Received message for " + message.id + " in " + message.direction + ".");
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
            if (GlobalVarDevice.DetailedLogs) makeLogs("Received results: " + command+" ans: "+ parameters[3]);
            HandleResultSpecial(Int32.Parse(parameters[0]), parameters[1], parameters[2], parameters[3]);
        }
        private void HandleFollow(String input)
        {
            // otrzymany rozkaz to NUMER ROZKAZU.argumenty.NazwaOtzyujacego.idWysylajacego.czasSynchronizacji
            // lub 0.NewTimeSingle.NewTimeWait
            String[] parameters=input.Split('#');
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
                makeLogs("Command: " + parametersC[0]+ parametersC[1]);
                AddMessageResults(input, HandleFollowSpecial(Int32.Parse(parametersC[0]), parametersC[1]) +"#"+ parameters[3]);
            }
        }
        protected bool AddMessageFollow(int orderNumer, String argv, String name)
        {
            //wysylana odp to NUMER ROZKAZU.argumenty.NazwaOtzyujacego.odp.master
            String order = orderNumer + "#" + argv + "#" + name;
            if (waitingForRespond.Contains(order))
            {
                if (GlobalVarDevice.DetailedLogs) makeLogs("Still havent got respond for: "+order);
                return false; // if order hasnt get respond
            }
            Data temp = new Data();
            temp.id = id;
            temp.state = State.FOLLOW;
            temp.description = order;
            temp.direction = Direction.UL;
            messages.AddLast(temp);
            waitingForRespond.AddLast(temp.description);
            makeLogs("New message added: " + order);
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
    }
}