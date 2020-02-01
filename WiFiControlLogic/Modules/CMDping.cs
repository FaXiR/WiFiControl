using System;

namespace WiFiControlLogic.Modules
{
    /// <summary>
    /// Проверка потерь и средней задержки к адресу
    /// </summary>
    class CMDping : CMDbase
    {
        public string URL { get; }
        public string Losses { private set; get; } = null;
        public string AvgPing { private set; get; } = null;

        /// <summary>
        /// Проверка доступа к сайту (Потери и средняя задержка доступа)
        /// </summary>
        /// <param name="Address">Адрес сайта или ip</param>
        public CMDping(string Address)
        {
            URL = Address;
        }

        /// <summary>
        /// Обновление данных в AvgPing и Losses
        /// </summary>
        public void UpdateInfo()
        {
            string result = CMDexecute("ping " + URL);

            //Очитска данных
            AvgPing = null;
            Losses = null;

            //Поиск потерь и проверка корректного ответа
            int index = result.IndexOf("потерь)");
            if (index == -1)
            {
                throw new NullReferenceException(result);
            }
            while (index > 0)
            {
                index--;
                if (result[index] == '(')
                {
                    break;
                }
            }
            while (index < result.Length - 1)
            {
                index++;
                Losses += result[index];
                if (result[index] == '%')
                {
                    break;
                }
            }
            //Поиск пинга
            if (Losses != "100%")
            {
                index = result.IndexOf("Среднее = ") + 9;
                while (index <= result.Length)
                {
                    index++;
                    if (result[index] == '\r')
                    {
                        break;
                    }
                    AvgPing += result[index];
                }
            }
        }

        /// <summary>
        /// Вывод данных из URL, Losses и AvgPing в консоль
        /// </summary>
        public void InfoToConsole()
        {
            Console.WriteLine(URL);
            Console.WriteLine($"Потери: {Losses}");
            Console.WriteLine($"Задержка: {AvgPing}");
        }
    }
}
