using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class U
    {
        //list of all chars that are bad
        private static char[] BadChars = {'&', ' ', ',', ')', '(' };

        /**
         *Add a new line to the end of String
         * @param s the strign to new line
         */
        public static String NL(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }
        
        //dont think I use this
        public static bool GetRandomChance(Random r, int probability)
        {
            return r.Next(0, 100) < probability;
        }

        /**
         * Check to see if a string has any characters on the no go list
         * @param s string to check
         */
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
