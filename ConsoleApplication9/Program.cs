using System;
using System.Threading;
using Equipments;
using Senders;

namespace ConsoleApplication9
{
    class Program
    {
        static String logFile= "C:\\Users\\Artur\\Documents\\Visual Studio 2015\\Projects\\MISS\\ConsoleApplication9\\logi";
        static void Main(string[] args)
        {
            Sender station = new Sender(10,10);
            station.StartTransmission(150, 5);

            Display z1 =new Display("Moj ekran",50);
            z1.addNewPreview(z1.Name+"ek");
            z1.BlindDecoding();
            z1.Attach();

            Display z2 = new Display("Moj ekran", 50);
            z1.addNewPreview(z1.Name + "ek");
            z1.BlindDecoding();
            z1.Attach();

            Thread.Sleep(200);

            z1.TurnOff();
            station.TurnOff();
            z1.SaveLogsToFile(logFile + "\\Moj ekran");
            station.SaveLogsToFile(logFile + "\\station");
        }
    }
}
