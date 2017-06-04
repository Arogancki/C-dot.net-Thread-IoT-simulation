﻿using System;
using System.Threading;
using Devices;
using Equipments;
using Senders;

namespace ConsoleApplication9
{
    class Program
    {
        private static String logFile = Environment.CurrentDirectory+"\\..\\..\\..\\logi"; 
        static void Main(string[] args)
        {
            Sender station = new Sender(20,30);
            station.StartTransmission(150, 5);
            
            TempRegulator u1=new TempRegulator(100);
            u1.BlindDecoding();
            u1.Attach();

            TempMeter u2 = new TempMeter();
            u2.BlindDecoding();
            //u2.Attach();

            Radiator u3 = new Radiator();
            u3.BlindDecoding();
            u3.Attach();

            Thread.Sleep(1500);
            Device.DeviceContainer.RemoveAll();
        }
    }
}