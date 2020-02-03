using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiControlLogic.Modules;

namespace WiFiControlConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var WiFi = new CMDWiFi();
            var Ethernet = new CMDEthernet();
            var Ping = new CMDping[]
            {
                new CMDping("yandex.ru"),
            //    new CMDping("google.com"),
            //    new CMDping("yahoo.com"),
            //    new CMDping("mail.ru"),
            };                    

            while (true)
            {
                for (int i = 0; i < Ping.Length; i++)
                {
                    Ping[i].UpdateInfo();
                    Ping[i].InfoToConsole();
                    Console.WriteLine();
                }

                Ethernet.UpdateInfo();
                Ethernet.InfoToConsole();
                Console.WriteLine();

                //WiFi.CMDShowHostedNetwork();
                WiFi.UpdateInfo();
                WiFi.InfoToConsole();
                Console.WriteLine();

                Console.WriteLine("---------------------------");
                Console.WriteLine();
            }
        }
    }
}
