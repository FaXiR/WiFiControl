using System;
using System.Collections.Generic;
using WiFiControlLogic.Modules;
using System.IO;

namespace WiFiControlConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool WiFiExecute = true;
            bool EthernetExecute = true;
            bool PingExecute = true;
            bool MACexecute = true;

            var MACcomp = new MACcomparsion();
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

                if (MACexecute)
                {
                    try
                    {
                        List<string> MAC = new List<String>();
                        string[] MACUserList = File.ReadAllLines(@"C:\Users\FaXiR\Desktop\MACList.txt");
                        for (int i =0; i < MACUserList.Length; i++)
                        {
                            MAC.Add(MACUserList[i]);
                        }
                        var OutMACList = MACcomp.matchingMAC(WiFi.ListMAC, MAC);
                        foreach (string OML in OutMACList)
                        {
                            Console.WriteLine(OML);
                        }
                    }
                    catch (Exception ex)
                    {
                        MACexecute = false;
                        Console.WriteLine("Ошибка MACexecute модуля " + ex);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("---------------------------");
                Console.WriteLine();
            }
        }
    }
}
