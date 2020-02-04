using System.Collections.Generic;

namespace WiFiControlLogic.Modules
{
    /// <summary>
    /// Сравнение MAC адресов
    /// </summary>
    public class MACcomparsion
    {
        /// <summary>
        /// Сравнивает MAC адреса и выводит список MAC адресов, с юзерами, к которым они принадлежат
        /// </summary>
        /// <param name="MACList">Список подключенных MAC адресов</param>
        /// <param name="MACcompare">Список знакомых MAC адресов и их владельцы (encoding UTF-8)</param>
        /// <returns></returns>
        public List<string> matchingMAC(List<string> MACList, List<string> MACcompare)
        {
            // Входящий MACcompare должен иметь вид:
            // *MAC adress 1*
            // *User 1*
            // *MAC adress 2*
            // *User 2*
            List<string> result = new List<string>();

            if (MACcompare.Count % 2 == 1)
            {
                MACcompare.Add("без владельца");
            }

            foreach (string MACdevices in MACList)
            {
                bool verifMAC = false;
                for (int i = 0; i < MACcompare.Count; i += 2)
                {
                    if (MACdevices == MACcompare[i])
                    {
                        result.Add($"{MACdevices} ({MACcompare[i + 1]})");
                        verifMAC = true;
                        break;
                    }                    
                }
                if (!verifMAC)
                {
                    result.Add($"{MACdevices} (?!?!)");
                }
            }
            return result;
        }
    }
}
