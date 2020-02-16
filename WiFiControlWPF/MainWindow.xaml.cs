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
using WiFiControlWPF.DialogBox;

namespace WiFiControlWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Возможность исполнения
        static bool WiFiExecute = false;
        static bool PingExecute = false;
        static bool MACexecute = false;
        static bool WiFiRestart = false;
        static bool WiFiButtonBlock = false;

        //Объявление классов
        readonly CMDping YandP = new CMDping("yandex.ru");
        readonly CMDping GoogdP = new CMDping("google.com");
        readonly CMDping ip2P = new CMDping("2ip.ru");
        readonly CMDping UfanP = new CMDping("My.ufanet.ru");
        readonly CMDWiFi WiFi = new CMDWiFi();

        /// <summary>
        /// Список известных MAC адресов
        /// </summary>
        static List<string> MAC = new List<String>();

        /// <summary>
        /// Пароль от WiFi сети
        /// </summary>
        static string WiFiPassword;

        public MainWindow()
        {
            InitializeComponent();
            //Проверка на работоспособность
            Test();
            //Запуск основной части приложения
            Start();
        }

        /// <summary>
        /// Запускает тестирование разных CMD комманд в разных потоках
        /// </summary>
        void Test()
        {
            new Thread(TestPing).Start();
            new Thread(TestWiFi).Start();
        }

        /// <summary>
        /// Проверка работоспособности CMD комманд ping
        /// </summary>
        void TestPing()
        {
            try
            {
                new CMDping("-n 1 localhost").UpdateInfo();
                PingExecute = true;
            }
            catch
            {
                PingExecute = false;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    W_PingStatus.Content = "Получить невозможно";
                }));

            }
        }

        /// <summary>
        /// Проверка работоспособности CMD комманд netsh wlan, задает известные MAC адреса и пароль из текстового файла
        /// </summary>
        void TestWiFi()
        {
            Visibility Hidding;

            try
            {
                new CMDWiFi().UpdateInfo();

                WiFiExecute = true;
                Hidding = Visibility.Hidden;
            }
            catch
            {
                WiFiExecute = false;
                Hidding = Visibility.Visible;
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                WiFiClose0.Visibility = Hidding;
                WiFiClose1.Visibility = Hidding;
                WiFiClose2.Visibility = Hidding;
            }));

            //Преждевременное завершение, если WiFi не прошел тест
            if (!WiFiExecute)
            {
                MACexecute = false;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CheckUserForDisablePC.IsEnabled = false;
                }));
                return;
            }

            try
            {
                MAC = File.ReadAllLines(@"MACList.txt").ToList();
                MACexecute = true;
                WiFiPassword = null;
            }
            catch
            {
                MACexecute = false;
            }

            try
            {
                WiFiPassword = File.ReadLines(@"Password.txt").First();
            }
            catch
            {
                WiFiPassword = null;
            }
        }

        /// <summary>
        /// Создание и запуск основных потоков
        /// </summary>
        void Start()
        {
            var Threads = new List<Thread>();

            Threads.Add(new Thread(() => CheckPing(YandP)));
            Threads.Add(new Thread(() => CheckPing(GoogdP)));
            Threads.Add(new Thread(() => CheckPing(ip2P)));
            Threads.Add(new Thread(() => CheckPing(UfanP)));
            Threads.Add(new Thread(CheckWiFi));
            Threads.Add(new Thread(ResultOfUI));

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
            int attemp = 100;
            while (true)
            {
                if (WiFiExecute)
                {
                    while (true)
                    {
                        cls.UpdateInfo();
                        WiFiRestart = false;
                        Thread.Sleep(2000);
                    }
                }
                attemp--;
                if (attemp == 0)
                {
                    return;
                }
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Обновляет данные в CMDWiFi классе
        /// </summary>
        private void CheckWiFi()
        {
            int attemp = 100;
            while (true)
            {
                if (WiFiExecute)
                {
                    while (true)
                    {
                        WiFi.UpdateInfo();

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if ((bool)CheckUserForDisablePC.IsChecked)
                            {
                                if (WiFi.ListMAC.Count == 0)
                                {
                                    new CMDCommand().ExecuteForceShutdown();
                                }
                            }
                        }));
                        Thread.Sleep(2000);
                    }
                }
                attemp--;
                if (attemp == 0)
                {
                    return;
                }
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
                        W_Yand.Content = ($"{YandP.Losses} / {YandP.AvgPing}");
                        W_Goog.Content = ($"{GoogdP.Losses} / {GoogdP.AvgPing}");
                        W_2ip.Content = ($"{ip2P.Losses} / {ip2P.AvgPing}");
                        W_Ufan.Content = ($"{UfanP.Losses} / {UfanP.AvgPing}");
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
                            W_PingStatus.Header = "Проблемы с кабелем";
                        }));
                    }
                    else if (!Y && !G && !I && !U)
                    {
                        //Если доступно все
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            W_PingStatus.Header = "Полный интернет доступ";
                        }));
                    }
                    else if (!U && (!Y || !G || !I))
                    {
                        //Если доступен только UfanP и что нибудь другое
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            W_PingStatus.Header = "Требуется подключение";
                        }));

                    }
                    else if (Y && G && I && !U)
                    {
                        //Если доступен только UfanP
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            W_PingStatus.Header = "Требуется оплата";
                        }));
                    }
                    else
                    {
                        //Неизвестно...
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            W_PingStatus.Header = "Неизвестно";
                        }));
                    };
                }

                if (WiFiExecute)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        W_WiFiMacList.Children.Clear();

                        if (WiFiRestart)
                        {
                            W_WiFiStatus.Value = 3;
                            W_WiFiUserCount.Header = "Подключения к сети";
                        }
                        else
                        {
                            switch (WiFi.WiFiStatus)
                            {
                                case "Запущено":
                                    W_WiFiStatus.Value = 4;
                                    break;
                                case "Не запущено":
                                    W_WiFiStatus.Value = 2;
                                    break;
                                default: //Неизвестно
                            W_WiFiStatus.Value = 1;
                                    break;
                            }

                            W_WiFiName.Text = $"Название: {WiFi.ReceivedWiFiName}";
                            W_WiFiPassword.Text = $"Пароль: {WiFiPassword}";

                            if (WiFi.ListMAC.Count == 0)
                            {
                                W_WiFiUserCount.Header = "Подключения к сети";
                            }
                            else
                            {
                                W_WiFiUserCount.Header = $"Подключения к сети ({WiFi.ListMAC.Count})";
                            }

                            List<string> OutMACList = new List<string>();
                            if (MACexecute)
                            {
                                OutMACList = new MACcomparsion().MatchingMAC(WiFi.ListMAC, MAC);
                            }
                            else
                            {
                                OutMACList = WiFi.ListMAC;
                            }

                            foreach (string mac in OutMACList)
                            {
                        // Create a button.
                        TextBlock ad = new TextBlock()
                                {
                                    Text = mac,
                                    Margin = new Thickness(4)
                                };
                        // Add created button to a previously created container.
                        W_WiFiMacList.Children.Add(ad);
                            }
                        }
                    }));
                }
                Thread.Sleep(1000);
            }
        }

        private void Expander_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((Expander)e.OriginalSource).Header.ToString() == "Подключения к сети")
            {
                return;
            }
            ((Expander)e.OriginalSource).IsExpanded = true;
        }

        private void Expander_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Expander)e.OriginalSource).IsExpanded = false;
        }

        private void WiFi_Enable(object sender, RoutedEventArgs e)
        {
            if (WiFiButtonBlock || WiFiRestart)
            {
                return;
            }

            Thread thread = new Thread(() => WiFi_act(1));
            thread.IsBackground = true;
            thread.Start();
        }

        private void WiFI_Disable(object sender, RoutedEventArgs e)
        {
            if (WiFiButtonBlock || WiFiRestart)
            {
                return;
            }

            Thread thread = new Thread(() => WiFi_act(3));
            thread.IsBackground = true;
            thread.Start();
        }

        private void WiFi_Restart(object sender, RoutedEventArgs e)
        {
            if (WiFiButtonBlock || WiFiRestart)
            {
                return;
            }

            Thread thread = new Thread(() => WiFi_act(2));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Включает, перезагружает или выключает WiFi в зависимости от числа
        /// </summary>
        /// <param name="act">1 - Вкл. 2 - Перезагрузка. 3 - Выкл.</param>
        void WiFi_act(int act)
        {
            WiFiButtonBlock = true;

            if (WiFiExecute)
            {
                switch (act)
                {
                    case 1:
                        WiFi.EnabledWiFi();
                        break;
                    case 2:
                        WiFi.RestartWiFi();
                        WiFiRestart = true;
                        break;
                    case 3:
                        WiFi.DisabledWiFi();
                        break;
                }
            }
            WiFiButtonBlock = false;
        }

        private void WiFi_Change(object sender, RoutedEventArgs e)
        {
            string TempLogin = null;
            string TempPassword = null;

            if (WiFiButtonBlock || WiFiRestart)
            {
                return;
            }

            SetLoginPassword Dialog = new SetLoginPassword();

            Dialog.Login = WiFi.ReceivedWiFiName;
            Dialog.Password = WiFiPassword;

            if (Dialog.ShowDialog() == true)
            {
                TempLogin = Dialog.Login;
                TempPassword = Dialog.Password;
            }
            else
            {
                MessageBox.Show("Запись была отменена");
                return;
            }

            try
            {
                WiFi.CMDSetNamePassword(TempLogin, TempPassword);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            //Перезапуск WiFi
            Thread thread = new Thread(() => WiFi_act(2));
            thread.IsBackground = true;
            thread.Start();

            //Сохранение нового пароля
            File.WriteAllText(@"Password.txt", TempPassword);
            WiFiPassword = TempPassword;

        }

        private void Add15Minutes(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
