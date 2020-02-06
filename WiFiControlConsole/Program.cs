using System;
using System.Collections.Generic;
using WiFiControlLogic.Modules;
using System.IO;
using System.Linq;
using System.Threading;

namespace WiFiControlConsole
{
    class Program
    {
        //Возможность исполнения
        static bool WiFiExecute;
        static bool EthernetExecute;
        static bool PingExecute;
        static bool MACexecute;

        //Список известных MAC адресов
        static List<string> MAC = new List<String>();

        static void Main(string[] args)
        {
            //Задание размеров и названия
            Console.Title = "WiFi Control";
            Console.SetWindowSize(55, 16);
            Console.SetBufferSize(55, 16);

            //Проверка на работоспособность
            Console.WriteLine("Тест нужных CMD команд для работы");
            CMDTest();
            while ((WiFiExecute || EthernetExecute || PingExecute) == false)
            {
                Console.WriteLine();
                Console.WriteLine("Не удалось воспользоваться ни одной CMD командой...");
                Console.WriteLine("Вы можете повторить тест, нажав любую кнопку на клавиатуре либо закрыть программу");
                Console.ReadKey();
                CMDTest();
            }
            Console.Clear();

            //Создание потоков            
            var Threads = new List<Thread>();

            if (PingExecute)
            {
                Threads.Add(new Thread(() => CheckPing("Yandex.ru", 0, 0)));
                Threads.Add(new Thread(() => CheckPing("Google.com", 0, 3)));
                Threads.Add(new Thread(() => CheckPing("Yahoo.com", 0, 6)));
                Threads.Add(new Thread(() => CheckPing("Mail.ru", 0, 9)));
            }

            if (EthernetExecute)
            {
                Threads.Add(new Thread(() => CheckEthernet(0, 13)));
            }

            if (WiFiExecute)
            {
                Threads.Add(new Thread(() => CheckWiFi(20, 1)));
            }

            for (int i = 0; i < Threads.Count; i++)
            {
                Threads[i].IsBackground = true;
                Threads[i].Start();
            }

            //Защита от преждевременного закрытия консоли
            while (true)
            {
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Проверка работоспособности CMD комманд ping, netsh, netstat и задает известные MAC адреса
        /// </summary>
        static void CMDTest()
        {
            try
            {
                new CMDWiFi().UpdateInfo();
                WiFiExecute = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                WiFiExecute = false;
            }

            try
            {
                new CMDEthernet().UpdateInfo();
                EthernetExecute = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                EthernetExecute = false;
            }

            try
            {
                new CMDping("-n 1 localhost").UpdateInfo();
                PingExecute = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                PingExecute = false;
            }

            try 
            {
                MAC = File.ReadAllLines(@"C:\Users\FaXiR\Desktop\MACList.txt").ToList();
                MACexecute = true;                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MACexecute = false;
            }
        }

        /// <summary>
        /// Проверяет доступ к сайту и выводит информацию с начала соотвествующей позиции на консоле
        /// </summary>
        /// <param name="URL">Адрес проверяемого сайта</param>
        /// <param name="X">Позиция X для курсора консоли</param>
        /// <param name="Y">Позиция Y для курсора консоли</param>
        static void CheckPing(string URL, int X, int Y)
        {
            while (true)
            {
                var check = new CMDping(URL);
                check.UpdateInfo();

                if (check.Losses != "0%")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.SetCursorPosition(X, Y);
                Console.Write($"Адрес {check.URL}   ");
                Console.SetCursorPosition(X, Y + 1);
                Console.Write($"{check.Losses} / {check.AvgPing}   ");

                Console.ResetColor();

                Console.SetCursorPosition(0, 15);
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Проверяет статус сети и выводит информацию о ней
        /// </summary>
        /// <param name="X">Позиция X для курсора консоли</param>
        /// <param name="Y">Позиция Y для курсора консоли</param>
        static void CheckWiFi(int X, int Y)
        {
            while (true)
            {
                var check = new CMDWiFi();
                check.UpdateInfo();

                Console.SetCursorPosition(X, Y);
                Console.Write($"Название сети: {check.ReceivedWiFiName}   ");
                Console.SetCursorPosition(X, Y + 1);
                Console.Write($"Статус сети: {check.WiFiStatus}   ");

                List<string> OutMACList = new List<String>();
                if (MACexecute)
                {
                    OutMACList = new MACcomparsion().MatchingMAC(check.ListMAC, MAC);
                }
                else
                {
                    OutMACList = check.ListMAC;
                }
                Console.SetCursorPosition(X, Y + 2);
                Console.Write($"Список подключеных пользователей:");
                for (int i = 4; i < OutMACList.Count + 4; i++)
                {
                    Console.SetCursorPosition(X, Y + i);
                    Console.Write(OutMACList[i - 4] + "   ");
                }

                Console.SetCursorPosition(0, 15);
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Получает ифнормацию о переданных и принятых данных по Ethernet и выводит информацию об этом
        /// </summary>
        /// <param name="X">Позиция X для курсора консоли</param>
        /// <param name="Y">Позиция Y для курсора консоли</param>
        static void CheckEthernet(int X, int Y)
        {
            while (true)
            {
                var check = new CMDEthernet();
                check.UpdateInfo();

                Int64 Mb = check.ByteInp / 1024 / 1024;
                Int64 Gb = Mb / 1024;

                Console.SetCursorPosition(X, Y);
                Console.WriteLine($"Принято: {check.ByteInp} байт | {Mb} Мб | {Gb} Гб   ");

                Mb = check.ByteOut / 1024 / 1024;
                Gb = Mb / 1024;

                Console.SetCursorPosition(X, Y + 1);
                Console.WriteLine($"Отправлено: {check.ByteOut} байт | {Mb} Мб | {Gb} Гб   ");

                Console.SetCursorPosition(0, 15);
                Thread.Sleep(4000);
            }
        }
    }
}

