using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
   public static class U
    {
        public static String newLineS(String s)
        {
            String newline = "\r\n";
            String finalString = s + newline;
            return finalString;
        }

    }
}
