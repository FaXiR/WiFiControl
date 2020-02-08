﻿using System;
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

        //Список известных MAC адресов
        static List<string> MAC = new List<String>();

        public MainWindow()
        {
            InitializeComponent();

            //Проверка на работоспособность
            Test();

            //Запуск работы
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

        void Start()
        {
            var YandP = new CMDping("yandex.ru");
            var GoogdP = new CMDping("google.com");
            var ip2P = new CMDping("2ip.ru");
            var UfanP = new CMDping("My.ufanet.ru");

            //Создание потоков            
            var Threads = new List<Thread>();

            if (PingExecute)
            {
                Threads.Add(new Thread(() => CheckPing(YandexPing, YandP)));
                Threads.Add(new Thread(() => CheckPing(GooglePing, GoogdP)));
                Threads.Add(new Thread(() => CheckPing(Ip2Ping, ip2P)));
                Threads.Add(new Thread(() => CheckPing(UfanetPing, UfanP)));
                Threads.Add(new Thread(() => ResultOfCheckPing(YandP, GoogdP, ip2P, UfanP)));
            }
            else
            {
                //Перекритие пинг части
            }

            if (WiFiExecute)
            {
                //Threads.Add(new Thread(() => CheckWiFi(20, 1)));
            }
            else
            {
                //Перекрытие WiFi части
            }

            foreach (Thread th in Threads)
            {
                th.IsBackground = true;
                th.Start();
            }
        }

        /// <summary>
        /// Выводит состоние сети в WPF
        /// </summary>
        /// <param name="title">ТекстБлок из WPF</param>
        /// <param name="check">Класс CMDPing</param>
        private void CheckPing(TextBlock title, CMDping check)
        {
            while (true)
            {
                check.UpdateInfo();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    title.Text = ($"{check.Losses} / {check.AvgPing}");
                }));
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Выводит общий результат CMDPing
        /// </summary>
        /// <param name="checks">Классы CMDPing (4 штуки)</param>
        private void ResultOfCheckPing(params CMDping[] checks)
        {
            while (true)
            {
                //Класс CMDPing объявленный как UfanP имеет наивысшее значение. т.к. он доступен даже если интерента нет (В моем случае это так) 
                bool Y = checks[0].Losses == "100%";
                bool G = checks[1].Losses == "100%";
                bool I = checks[2].Losses == "100%";
                bool U = checks[3].Losses == "100%";

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

                Thread.Sleep(1500);
            }
        }
    }
}
