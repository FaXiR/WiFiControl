namespace WiFiControlLogic.Modules
{
    public class CMDCommand : CMDbase
    {
        /// <summary>
        /// Выполнение CMD комманды 
        /// </summary>
        /// <param name="CMDcommand">Команда для выполнения</param>
        public string ExecuteCommand(string CMDcommand)
        {
            return CMDexecute(CMDcommand);
        }

        /// <summary>
        /// Принудительное выключение ПК с игнорированием о сохранении данных и без задержки
        /// </summary>
        public void ExecuteForceShutdown()
        {
            CMDexecute("shutdown -f -s -t 0");
        }
    }
}
