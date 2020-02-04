using System;

namespace WiFiControlLogic.Modules
{
    /// <summary>
    /// Получает ифнормацию о переданных и принятых данных по Ethernet за время работы ПК
    /// </summary>
    public class CMDEthernet : CMDbase
    {
        public Int64 ByteInp { private set; get; }
        public Int64 ByteOut { private set; get; }

        /// <summary>
        /// Обновление данных в ByteInp и ByteOut
        /// </summary>
        public void UpdateInfo()
        {
            string result = CMDexecute(@"netstat /e | find ""Байт""");

            if (result.IndexOf("Служба") != -1 && result.IndexOf("не запущена") != -1 || result == "")
            {
                throw new NullReferenceException("Пришел необрабатываемый результат: " + result);
            }

            //Позиция в строке
            int index = 0;

            //Удаление лишнего
            result = result.Substring(4);
            result = result.Substring(0, result.Length - 2);

            //Удаление лишнего
            while (true)
            {
                if (result[index] == ' ')
                {
                    index++;
                    continue;
                }
                result = result.Substring(index);
                index = 0;
                break;
            }

            //Поиск данных
            string Inp = null, Out = null;

            //Входящие
            while (true)
            {
                if (result[index] == ' ')
                {
                    result = result.Substring(Inp.Length);
                    index = 0;
                    break;
                }
                Inp += result[index];
                index++;
            }

            //Исходящие
            while (index < result.Length - 1)
            {
                index++;
                if (result[index] == ' ')
                {
                    continue;
                }
                Out += result[index];
            }

            ByteOut = Convert.ToInt64(Out);
            ByteInp = Convert.ToInt64(Inp);
        }

        /// <summary>
        /// Вывод данных из ByteInp и ByteOut в консоль
        /// </summary>
        public void InfoToConsole()
        {
            Int64 Mb = ByteInp / 1024 / 1024;
            Int64 Gb = Mb / 1024;
            Console.WriteLine($"Принято: {ByteInp} байт | {Mb} Мб | {Gb} Гб");
            Mb = ByteOut / 1024 / 1024;
            Gb = Mb / 1024;
            Console.WriteLine($"Отправлено: {ByteOut} байт | {Mb} Мб | {Gb} Гб");
        }
    }
}
