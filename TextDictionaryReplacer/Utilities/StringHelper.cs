using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextDictionaryReplacer.Utilities
{
    public static class StringHelper
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string CollapseSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
    }
}
