using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonNamespace;

namespace Utilities
{
    public static class U
    {
        public static String NewLineS(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }

        public static bool GetRandomChance(Random r, int probability)
        {
            return r.Next(0, 100) < probability;
        }
    }
}
