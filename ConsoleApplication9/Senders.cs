using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Air;
using Devices;

// commands: 0-attach

namespace Senders
{
    public class Sender: Device
    {
        public Channel channel;
        private Dictionary<int, String> receiversNames; // id, name(type)
        private Dictionary<int, LinkedList<Data>> messages; // id and Data
        private List<int> temp; // temp list for Listen
        private List<int> receivers;
        private int timeSpace; // to catch a breath before all server task
        private int timeSingle; // time to signle transmission to or from receiver, 2 for user
        public Sender(int _timeSingle, int _timeSpace)
        {
            Name = "Sender" + id;
            receivers = new List<int>();
            messages = new Dictionary<int, LinkedList<Data>>();
            receiversNames = new Dictionary<int, string>();
            timeSpace = _timeSpace;
            timeSingle = _timeSingle;
        }
        ~Sender()
        {
            TurnOff();
        }
        public void StartTransmission(float freq, int bandwidthIndex)
        {
            if (bandwidthIndex >= Bandwidth.Length)
                bandwidthIndex = Bandwidth.Length - 1;
            float start = freq - Bandwidth[bandwidthIndex] / 2;
            if (start < 0)
                start = 0.0f;
            float end = freq + Bandwidth[bandwidthIndex] / 2;
            if (end > maxFreq)
                end = maxFreq;
            channel = AirInterface.NewTransmission(start, end, this);
            makeLogs("Transmission started on " + start + "-" + end+"MHz");
            changeState(State.CONNECTED);
            listenFlag = true;
            listen =new Thread(Listen);
            listen.Start();
        }
        public void StartTransmission()
        {
            StartTransmission(defalutFreq, defalutBandwidthIndex);
        }
        public void Disconnect()
        {
            listenFlag = false;
        }
        private void Listen()
        {
            Data newReceiver = new Data();
            temp=new List<int>(receivers);
            newReceiver.state = State.CONNECTED;
            newReceiver.id = id;
            newReceiver.direction=Direction.DL;
            long milliseconds;
            int timeSpaceToSend;
            while (listenFlag)
            {

                foreach (var receiver in temp)
                {
                    milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                    if (GlobalVarDevice.DetailedLogs) makeLogs("traing to send message to " + receiver);
                    SendMessage(receiver,milliseconds.ToString()); 
                    
                    try { 
                        Thread.Sleep(timeSingle // time to recive message
                        - (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)-milliseconds)); // difference from now and start time
                        }
                        catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait

                    milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (GlobalVarDevice.DetailedLogs) makeLogs("traing to get message from " + receiver);
                    GetMessage(receiver,timeSingle);
                    
                    try { 
                        Thread.Sleep(timeSingle // time to recive message
                        - (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)-milliseconds));
                        }
                        catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait
                }

                milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                //WATEK ATTACH
                timeSpaceToSend = timeSpace + (receivers.Count * 2 * timeSingle)
                    /*- (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds)*/;
                newReceiver.description = timeSingle + "#" //single time for 1 way transmission
                    + timeSpaceToSend // time between all transmission
                     + "#" + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); //actual time

                channel.ReceiveAttach(ref newReceiver); // if Data is returned new receiver has attached
                if (newReceiver.id != id) // new reciver
                {
                    if (!receivers.Contains(newReceiver.id))
                    {
                        makeLogs("New Reciever " + newReceiver.id + " " + newReceiver.description);
                        ReconfigureALL(timeSingle, timeSpace + (receivers.Count*2*timeSingle));
                        receivers.Add(newReceiver.id);
                        receiversNames.Add(newReceiver.id, newReceiver.description);
                        messages.Add(newReceiver.id, new LinkedList<Data>());
                    }
                    newReceiver.state = State.DISCONNECTED;
                    newReceiver.id = id;
                    newReceiver.direction = Direction.DL;
                }
                //wait timeSpace

                temp = new List<int>(receivers);
                try
                {
                    Thread.Sleep(timeSpace - (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds));
                }
                catch (ArgumentOutOfRangeException e) { } // no need to handle -time exception, just do NOT wait
            }
        }
        private void DeleteReceiver(int receiver)
        {
            receivers.Remove(receiver);
            messages.Remove(receiver);
            receiversNames.Remove(receiver);
            ReconfigureALL(timeSingle, timeSpace + ((receivers.Count-1) * 2 * timeSingle));
        }
        private void GetMessage(int receiver,int timeOut)
        {
            Data message;
            try
            {
                message = channel.getChanel(timeOut);
            }
            catch (SemaphoreFullException e)
            {
                makeLogs("Reciver didn't answer. Disconnecting from station");
                DeleteReceiver(receiver);
                return;
            }
            catch (Exception e)
            {
                makeLogs("Error\n" + e);
                return;
            }

            if (message.id != receiver || message.direction!=Direction.UL) // odebrano wiadomosc od zlego receivera
            {
                if (message.id== receiver && message.direction==Direction.DL)
                    makeLogs("Reciver didn't answered. Disconnecting from station");
                else
                    makeLogs("Received message from " + message.id+" in "+message.direction + ". Disconnecting from station");
                DeleteReceiver(receiver);
            }
            else
            {
                switch (message.state)
                {
                    case State.CONNECTED:
                        if (GlobalVarDevice.DetailedLogs) makeLogs("Received: Nothing to do");
                        break;
                    case State.DISCONNECTED:
                        makeLogs("Receiver shuts down. Cause: "+message.description);
                        DeleteReceiver(receiver);
                        break;
                    case State.FOLLOW:
                        makeLogs("Received command: " + message.description);
                        HandleFollow(receiver, message.description);
                        break;
                    case State.RESULTS:
                        makeLogs("Received results for previously command");
                        HandleResults(message.description);
                        break;
                }
            }
        }
        private void PrepareMessage(int _id, State _state, String _descryption)
        {
            Data temp = new Data();
            temp.id = _id;
            temp.description = _descryption;
            temp.state = _state;
            temp.direction = Direction.DL;
            messages[_id].AddLast(temp);
        }
        private void SendMessage(int receiver, string time)
        {
            LinkedList<Data> temp = messages[receiver];
            if (temp.Count > 0) // if there is message for receiver
            {
                channel.setChannel(temp.First(), "#" + time);
                temp.RemoveFirst();
                if (GlobalVarDevice.DetailedLogs) makeLogs("Message sent to " + receiver);
            }
            else
            {
                channel.setChannel(receiver, State.CONNECTED, "#" + time, Direction.DL); // send defalut message
                //makeLogs("Message sent to " + receiver);
            }
        }
        private void HandleResults(String input)
        {
            String[] parameters = input.Split('#');
            int receiver = Int32.Parse(parameters[4]);
            if (receiversNames.ContainsKey(receiver))
            {
                PrepareMessage(receiver, State.RESULTS, input);
                makeLogs("result message prapared to send");
            }
            else
             // if there isn't reciver with this name send that hi dont exist
            makeLogs("result message master is no longer connected");
        }
        private void HandleFollow(int master,String input)
        {
            Data temp = new Data();
            temp.direction = Direction.DL;
            temp.state = State.FOLLOW;
            temp.description = input + "#" + master;/// skladnia wysylanego rozkazu to NUMER ROZKAZU.argumenty.NazwaOtzyujacego.idWysylajacego.czasSynchronizacji
            String[] parameters = input.Split('#');
            foreach (var receiver in receiversNames)
            {
                if (receiver.Value == parameters[2])
                {
                    temp.id = receiver.Key;
                    messages[receiver.Key].AddLast(temp);
                    makeLogs("Message prepared for: "+receiver.Key);
                    return;
                }
            }
             // if there isn't reciver with this name send that hi dont exist
            temp.id = master;
            temp.state = State.RESULTS;
            temp.description = input+"#ERROR#" +master;// SKLADNIA Dodawnaych resultatow string_rozkazu.odpowiedz
            messages[master].AddLast(temp);
            makeLogs("Prepared ERROR message for: " + master);
        }
        private void ReconfigureALL(int single, int space)
        {
            foreach (var receiver in receivers)
            {
                Reconfigure(receiver,single,space);
            }
            makeLogs("Reconfiguration Prepered for all");
        }
        private void Reconfigure(int receiver, int single, int space)
        {
            Data temp=new Data();
            temp.id = receiver;
            temp.state=State.FOLLOW;
            temp.description = "0#" + single + "#" + space; // 0 - means reconfiguration
            temp.direction=Direction.DL;
            if (messages[receiver].Count > 0 && messages[receiver].First().description.Split('#').First() == "0")
                messages[receiver].RemoveFirst(); // remove previously reconfiguration if exist
            messages[receiver].AddFirst(temp); 
        }
    }
}
