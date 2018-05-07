using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class U
    {
        private static char[] BadChars = {'&', ' ', ',', ')', '(' };

        public static String NL(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }
        
        public static bool GetRandomChance(Random r, int probability)
        {
            return r.Next(0, 100) < probability;
        }

        public static bool HasBadChars(String s)
        {
            if (s.Count() < 1) return true;

            foreach (char c in BadChars)
            {
                if (s.Contains(c)) return true;
            }
            return false;
        }

    }
}
