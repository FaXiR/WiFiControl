using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WiFiControlLogic.Modules
{
    /// <summary>
    /// Класс исполнений CMD комманд
    /// </summary>
    public class CMDbase
    {
        /// <summary>
        /// Скрытное исполнение CMD команды
        /// </summary>
        /// <param name="CMDcommand">Команда для CMD. Наример (@"netstat /e | find ""Байт""")</param>
        /// <returns>Ответ консоли</returns>
        protected string CMDexecute(string CMDcommand)
        {
            ProcessStartInfo psiOpt = new ProcessStartInfo(@"cmd.exe", "/C " + CMDcommand)
            {
                //Задаем параметры запуска как скрытые
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(866)
            };

            Process procCommand = Process.Start(psiOpt);
            StreamReader srIncoming = procCommand.StandardOutput;
            procCommand.WaitForExit();

            return srIncoming.ReadToEnd();
        }

        /// <summary>
        /// Исполнение CMD команды в отображаемом окне без возврата данных
        /// </summary>
        /// <param name="CMDcommand">Команда для CMD. Наример (@"netstat /e | find ""Байт""" && Timeout /t 3)</param>
        protected void CMDexecuteShow(string CMDcommand)
        {
            Process.Start(@"cmd.exe", "/C" + CMDcommand);
        }
    }
}