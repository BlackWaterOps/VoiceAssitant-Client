using System;
using System.Text.RegularExpressions;

namespace Please2.Util
{
    public static class Extensions
    {
        public static string CamelCase(this string name)
        {
            if (name.IndexOf("_") != -1)
            {
                name = Regex.Replace(name, @"[_-][a-z]{1}", (match) => match.ToString().ToUpper());
                name = Regex.Replace(name, @"[_-]", "");
            }

            return Char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}
