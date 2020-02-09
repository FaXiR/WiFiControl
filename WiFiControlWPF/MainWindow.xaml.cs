using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WiFiControlLogic.Modules;
using System.Threading;
using System.IO;

namespace WiFiControlWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Возможность исполнения
        static bool WiFiExecute;
        static bool PingExecute;
        static bool MACexecute;

        //Запись классов
        readonly CMDping YandP = new CMDping("yandex.ru");
        readonly CMDping GoogdP = new CMDping("google.com");
        readonly CMDping ip2P = new CMDping("2ip.ru");
        readonly CMDping UfanP = new CMDping("My.ufanet.ru");

        readonly CMDWiFi WiFi = new CMDWiFi();

        /// <summary>
        /// Список известных MAC адресов
        /// </summary>
        static List<string> MAC = new List<String>();

        public MainWindow()
        {
            InitializeComponent();

            //Проверка на работоспособность
            Test();

            //Запуск основной части приложения
            Start();
        }

        /// <summary>
        /// Проверка работоспособности CMD комманд ping, netsh и задает известные MAC адреса
        /// </summary>
        void Test()
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
        /// Создание и запуск основных потоков
        /// </summary>
        void Start()
        {        
            var Threads = new List<Thread>();

            if (PingExecute)
            {
                Threads.Add(new Thread(() => CheckPing(YandP)));
                Threads.Add(new Thread(() => CheckPing(GoogdP)));
                Threads.Add(new Thread(() => CheckPing(ip2P)));
                Threads.Add(new Thread(() => CheckPing(UfanP)));
            }
            else
            {
                //Перекритие пинг части
            }

            if (WiFiExecute)
            {
                Threads.Add(new Thread(CheckWiFi));
            }
            else
            {
                //Перекрытие WiFi части
            }

            if (PingExecute || WiFiExecute)
            {
                Threads.Add(new Thread(ResultOfUI));
            }
            else
            {
                //Ничего?
            }

            foreach (Thread th in Threads)
            {
                th.IsBackground = true;
                th.Start();
            }
        }

        /// <summary>
        /// Обновляет данные в CMDping классе
        /// </summary>
        /// <param name="Ping">Класс CMDPing</param>
        private void CheckPing(CMDping cls)
        {
            while (true)
            {
                cls.UpdateInfo();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Обновляет данные в CMDWiFi классе
        /// </summary>
        private void CheckWiFi()
        {
            while (true)
            {
                WiFi.UpdateInfo();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Выводит общий результат CMDPing
        /// </summary>
        private void ResultOfUI()
        {
            Thread.Sleep(1000);
            while (true)
            {
                if (PingExecute)
                {
                    //Вывод состояния каждого адреса
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        YandexPing.Text = ($"{YandP.Losses} / {YandP.AvgPing}");
                        GooglePing.Text = ($"{GoogdP.Losses} / {GoogdP.AvgPing}");
                        Ip2Ping.Text = ($"{ip2P.Losses} / {ip2P.AvgPing}");
                        UfanetPing.Text = ($"{UfanP.Losses} / {UfanP.AvgPing}");
                    }));

                    //Задание данных для статуса
                    bool Y = YandP.Losses == "100%";
                    bool G = GoogdP.Losses == "100%";
                    bool I = ip2P.Losses == "100%";
                    bool U = UfanP.Losses == "100%";

                    //Вывод статуса
                    if (Y && G && I && U)
                    {
                        //Если все недоступно
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            PingColor.Fill = Brushes.Red;
                            PingStatus.Foreground = Brushes.Black;
                            PingStatus.Text = "Без доступа";
                        }));
                    }
                    else if (!Y && !G && !I && !U)
                    {
                        //Если доступно все
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            PingColor.Fill = Brushes.LightGreen;
                            PingStatus.Foreground = Brushes.Black;
                            PingStatus.Text = "Полный доступ";
                        }));
                    }
                    else if (!U && (!Y || !G || !I))
                    {
                        //Если доступен только UfanP и что нибудь другое
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            PingColor.Fill = Brushes.Yellow;
                            PingStatus.Foreground = Brushes.Black;
                            PingStatus.Text = "Доступ с потерями";
                        }));
                    }
                    else if (Y && G && I && !U)
                    {
                        //Если доступен только UfanP
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            PingColor.Fill = Brushes.Orange;
                            PingStatus.Foreground = Brushes.Black;
                            PingStatus.Text = "Ограниченный доступ";
                        }));
                    }
                    else
                    {
                        //Неизвестно...
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            PingColor.Fill = Brushes.Black;
                            PingStatus.Foreground = Brushes.White;
                            PingStatus.Text = "Неизвестно...";
                        }));
                    };
                }

                if (false) //(WiFiExecute)
                {
                    //TODO: доделать

                    Console.Write($"Название сети: {WiFi.ReceivedWiFiName}   ");
                    Console.Write($"Статус сети: {WiFi.WiFiStatus}   ");

                    List<string> OutMACList = new List<string>();
                    if (MACexecute)
                    {
                        OutMACList = new MACcomparsion().MatchingMAC(WiFi.ListMAC, MAC);
                    }
                    else
                    {
                        OutMACList = WiFi.ListMAC;
                    }

                    Console.Write($"Список подключеных пользователей:");
                    for (int i = 4; i < OutMACList.Count + 4; i++)
                    {
                        Console.Write(OutMACList[i - 4] + "   ");
                    }
                }
         
                Thread.Sleep(1000);
            }
        }
    }
}
