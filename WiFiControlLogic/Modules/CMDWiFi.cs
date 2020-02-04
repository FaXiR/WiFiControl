using System;
using System.Collections.Generic;

namespace WiFiControlLogic.Modules
{
    /// <summary>
    /// Получает ифнормацю о состоянии WiFi и подключениях. А также управляет состоянием WiFi
    /// </summary>
    public class CMDWiFi : CMDbase
    {
        public string WiFiStatus { private set; get; }
        public string ReceivedWiFiName { private set; get; }
        public List<string> ListMAC { private set; get; } = new List<string>();

        /// <summary>
        /// Задает название сети и пароль
        /// </summary>
        /// <param name="Login">Имя сети</param>
        /// <param name="Password">Пароль сети</param>
        public void CMDSetNamePassword(string Login, string Password)
        {
            if (Password.Length < 8 || Password.Length > 0)
            {
                throw new ArgumentException("Пароль не может быть короче 8 символов");
            }

            string result = CMDexecute($"netsh wlan set hostednetwork mode = allow ssid = {Login} key = {Password}");

            if (result.IndexOf("Один или несколько параметров команды отсутсвуют") != -1)
            {
                throw new NullReferenceException("Ошибка установки логина/пароля, подробнее: " + result);
            }
        }

        /// <summary>
        /// Запускает видимое CMD окно с командой netsh wlan show hostednetwork && Timeout /t 
        /// </summary>
        public void CMDShowHostedNetwork()
        {
            CMDexecuteShow("netsh wlan show hostednetwork && Timeout /t 3");
        }

        /// <summary>
        /// Запускает WiFi
        /// </summary>
        public void EnabledWiFi()
        {
            CMDexecute("netsh wlan start hostednetwork");
        }

        /// <summary>
        /// Перезапускает WiFi
        /// </summary>
        public void RestartWiFi()
        {
            CMDexecute("netsh wlan stop hostednetwork && netsh wlan start hostednetwork");
        }

        /// <summary>
        /// Выключает WiFi
        /// </summary>
        public void DisabledWiFi()
        {
            CMDexecute("netsh wlan stop hostednetwork");
        }

        /// <summary>
        /// Обновление данных в WiFiStatus, ReceivedWiFiName и ListMAC.
        /// </summary>
        public void UpdateInfo()
        {
            string result = CMDexecute("netsh wlan show hostednetwork");

            if (result.IndexOf("Служба") != -1 && result.IndexOf("не запущена") != -1 || result == "")
            {
                throw new NullReferenceException("Пришел необрабатываемый результат: " + result);
            }

            ReceivedWiFiName = null;
            int index = result.IndexOf(@"Имя идентификатора SSID : """) + 27;
            while (true)
            {
                if (result[index] == '"')
                {
                    break;
                }
                ReceivedWiFiName += result[index];
                index++;
            }

            WiFiStatus = null;
            index = result.LastIndexOf("Состояние");
            while (true)
            {
                index++;
                if (result[index] == ':')
                {
                    while (true)
                    {
                        index++;
                        if (result[index] != ' ')
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            while (true)
            {
                if (result[index] == '\r')
                {
                    break;
                }
                WiFiStatus += result[index];
                index++;
            }

            ListMAC.Clear();
            index = result.IndexOf("Проверка подлинности выполнена");
            if (index == -1)
                return;
            result = result.Substring(0, result.Length - 4).Substring(index - 33);
            result = result.Replace("Проверка подлинности выполнена", "");
            result = result.Replace("\n", "");
            result = result.Replace(" ", "");
            ListMAC.AddRange(result.Split(new char[] { '\r' }));
        }

        /// <summary>
        /// Вывод данных из ReceivedWiFiName, WiFiStatus и ListMAC в консоль
        /// </summary>
        public void InfoToConsole()
        {
            Console.WriteLine(ReceivedWiFiName);
            Console.WriteLine(WiFiStatus);
            foreach (string MAC in ListMAC)
            {
                Console.WriteLine(MAC);
            }
        }
    }
}
