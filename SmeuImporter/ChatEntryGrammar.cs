using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sprache;

namespace SmeuImporter
{
    public static class ChatEntryGrammar
    {
        public static Parser<int> Number(this Parser<IEnumerable<char>> characters)
        {
            return characters.Select(chs => int.Parse( new string(chs.ToArray<char>())));
        }

        public static readonly Parser<string> Author = Parse.Letter.Until(Parse.Char(':')).Text();

        public static readonly Parser<string> Description = Parse.Letter.Until(Parse.LineEnd).Text();

        private static readonly Parser<string> DateTimeText = Parse.AnyChar.Until(Parse.Char('-')).Text();

        public static readonly Parser<DateTime> DateTime =
            from text in DateTimeText
            select System.DateTime
                .ParseExact(text, "M/d/yy, HH:mm ", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                                                                      
    }
}