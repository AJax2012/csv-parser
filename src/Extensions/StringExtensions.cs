using System.Collections.Generic;
using System.Linq;

namespace GardnerCsvParser.Extensions
{
    public static class StringExtensions
    {
        private static int AMOUNT_OF_LETTERS_TO_GET = 4;

        public static string GetFirstLetters(this string str)
        {
            return str
                .Substring(0, AMOUNT_OF_LETTERS_TO_GET)
                .ToLower();
        }

        public static List<string> SplitCsvRow(this string row)
        {
            return row
                .Split(',')
                .Select(item => 
                    item.Trim())
                .ToList();
        }
    }
}
