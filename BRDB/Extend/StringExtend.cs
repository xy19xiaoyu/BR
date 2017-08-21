using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRDB.Extend
{
    public static class StringExtend
    {
        public static string Left(this string str, int length)
        {
            if (str.Length >= length)
            {
                return str.Substring(0, length);
            }
            else
            {
                return str;
            }
        }

        public static int to_i(this string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
