using System;
using System.Collections.Generic;

namespace TimerBot.Bot.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitByMessageLength(this string str)
        {
            var MessageLength = 2000;

            for (var index = 0; index < str.Length; index += MessageLength)
                yield return str.Substring(index, Math.Min(MessageLength, str.Length - index));
        }
    }
}
