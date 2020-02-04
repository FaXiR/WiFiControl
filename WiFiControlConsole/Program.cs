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
            bool WiFiExecute = true;
            bool EthernetExecute = true;
            bool PingExecute = true;

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
                if (PingExecute)
                {
                    try
                    {
                        for (int i = 0; i < Ping.Length; i++)
                        {
                            Ping[i].UpdateInfo();
                            Ping[i].InfoToConsole();
                        }
                    }
                    catch (Exception ex)
                    {
                        PingExecute = false;
                        Console.WriteLine("Ошибка Ping модуля " + ex);
                    }
                    Console.WriteLine();
                }

                if (EthernetExecute)
                {
                    try
                    {
                        Ethernet.UpdateInfo();
                        Ethernet.InfoToConsole();
                    }
                    catch (Exception ex)
                    {
                        EthernetExecute = false;
                        Console.WriteLine("Ошибка Ethernet модуля " + ex);                        
                    }
                    Console.WriteLine();
                }

                if (WiFiExecute)
                {
                    try
                    {
                    //    WiFi.CMDShowHostedNetwork();
                        WiFi.UpdateInfo();
                        WiFi.InfoToConsole();
                    }
                    catch (Exception ex)
                    {
                        WiFiExecute = false;
                        Console.WriteLine("Ошибка WiFi модуля " + ex);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("---------------------------");
                Console.WriteLine();
            }
        }
    }
}
