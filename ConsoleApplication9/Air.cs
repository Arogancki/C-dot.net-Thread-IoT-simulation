using System;
using System.Collections.Generic;
using System.Threading;
using Senders;

namespace Air
{
    //abstract, static
    static class AirInterface
    {
        public struct Station
        {
            internal float bandwidthStart;
            internal float bandwidthEnd;
            public Channel channel;
            public float quality;
        }
        private static List<Station> stations = new List<Station>();
        static private Station newSender(float start, float end, Sender sender)
        {
            Station temp =new Station();
            temp.bandwidthStart=start;
            temp.bandwidthEnd=end;
            temp.quality = 1.0f;
            temp.channel=new Channel();
            foreach (Station station in stations)
            {
                float freqMatchLenght = (end > station.bandwidthEnd ? station.bandwidthEnd : end) -
                                 (start < station.bandwidthStart ? station.bandwidthStart : start);
                if (freqMatchLenght > 0) // if bandwidth is overlaped
                    temp.channel = station.channel;
            }
            return temp;
        }
        public static Channel NewTransmission(float start, float end, Sender sender)
        {
            stations.Add(newSender(start,end, sender));
            return stations[stations.Count - 1].channel;
        }
        public static Station SearchTransmission(float start, float end)
        {
            Station best = new Station();
            best.quality = 0.0f;
            foreach (Station station in stations)
            {
                float freqMatchLenght = (end > station.bandwidthEnd ? station.bandwidthEnd : end) -
                                  (start < station.bandwidthStart ? station.bandwidthStart : start);
                if (freqMatchLenght > best.quality)
                {
                    //best.quality = freqMatchLenght / (station.bandwidthEnd - station.bandwidthStart);
                    best.quality = freqMatchLenght / (end - start);
                    best.bandwidthStart = start;
                    best.bandwidthEnd = end;
                    best.channel = station.channel;
                }
            }
            return best;
        }
    }
    public class Channel
    {
        private Semaphore attachReceiver;
        private Semaphore attachSender;
        private Semaphore channelSemaphore;
        private Data attachChannel; //common memmory for attach
        private Data channel;// common memmory for connection
        public Channel()
        {
            channel=new Data();
            attachChannel = new Data();
            attachReceiver=new Semaphore(1,1);
            channelSemaphore = new Semaphore(1,1);
            attachSender=new Semaphore(1,1);
            attachSender.WaitOne(); // fake attachSender lock
            channelSemaphore.WaitOne();
        }
        ~Channel()
        {
            // attachReceiver.Dispose();
            // attachSender.Dispose();
        }
        public Data getChanel(int timeOut)
        {
            if (channelSemaphore.WaitOne(timeOut)) return channel;
            throw new SemaphoreFullException();
        }
        public void setChannel(int id, State state, String descryption, Direction direction)
        {
            channel.description = descryption;
            channel.id = id;
            channel.state = state;
            channel.direction = direction;
            try
            {
                channelSemaphore.Release();
            }
            catch (Exception e)
            {

            }
        }
        public void setChannel(Data input)
        {
            channel = input;
            try
            {
                channelSemaphore.Release();
            }
            catch (Exception e)
            {

            }
        }
        public void setChannel(Data input, String additionalDescryption)
        {
            channel = input;
            channel.description += additionalDescryption;
            try
            {
                channelSemaphore.Release();
            }
            catch (Exception e)
            {

            }
        }
        public void SendAttach(ref Data input)
        {
            if(!attachReceiver.WaitOne(5000)) return;
            attachChannel = input; // save receiver input
            if (!attachSender.WaitOne(5000)) return;
            input = attachChannel; // get sender input 
            attachReceiver.Release();
        }
        public void ReceiveAttach(ref Data input) // return in info if new attached
        {
            if (attachChannel.id != input.id)
            {
                Data temp = attachChannel;
                attachChannel = input; // save sender input
                input = temp; // get receiver input
                attachSender.Release();
            }
        }
    }
    public enum State
    {
        DISCONNECTED, // unconnected or conection broken
        CONNECTED,    // connection ok, nothing to send
        FOLLOW,     // command to perform (number 0-reconfiguration or get specyfic id, n-depends of device)
        RESULTS,    // results of command
    };
    public enum Direction
    {
        UL,
        DL
    };
    public struct Data
    {
        public int id;   // source id
        public State state; // reason of message
        public String description; // results or comand specyfication
        public Direction direction; // direction of message
    }
}