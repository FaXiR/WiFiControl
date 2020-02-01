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
        public List<string> ListMAC { private set; get; }

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

            if (result.IndexOf("Параметры размещенной сети") == -1)
            {
                throw new NullReferenceException(result);
            }

            WiFiStatus = null;
            ReceivedWiFiName = null;
            ListMAC.Clear();

            // TODO: допилить обновление данных
        }
    }
}
