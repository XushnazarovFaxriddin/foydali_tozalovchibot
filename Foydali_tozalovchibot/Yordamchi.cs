using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foydali_tozalovchibot
{
    public static class Yordamchi
    {
        /// <summary>
        /// Reklama functioni yuborilgan xabarda link borligini
        /// validatsiya qiladi va Boolean qaytaradi.
        /// </summary>
        /// <param str="kelgan xabar matni"></param>
        /// <returns></returns>
        public static bool Reklama(string str)
        {
            if (str == null || str == "") return false;
            try
            {
                return (str.Contains(".io") || str.Contains("http://") || str.Contains("https://")
                    || str.Contains(".uz") || str.Contains("bot?start=") || str.Contains(".ru")
                    || str.Contains(".com") || str.Contains(".me"));
            }
            catch { return false; }
        }
    }
}
