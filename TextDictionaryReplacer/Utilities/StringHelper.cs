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

        private static bool IsWordChar(char character)
        {
            return char.IsLetterOrDigit(character) || character == '_';
        }

        public static string ReplaceFullWords(this string text, string oldWord, string newWord)
        {
            if (text.IsEmpty())
            {
                return text;
            }

            int startIndex = 0;
            while (true)
            {
                int position = text.IndexOf(oldWord, startIndex);
                if (position == -1)
                {
                    return text;
                }
                int indexAfter = position + oldWord.Length;
                if ((position == 0 || !IsWordChar(text[position - 1])) && (indexAfter == text.Length || !IsWordChar(text[indexAfter])))
                {
                    text = text.Substring(0, position) + newWord + text.Substring(indexAfter);
                    startIndex = position + newWord.Length;
                }
                else
                {
                    startIndex = position + oldWord.Length;
                }
            }
        }
    }
}
