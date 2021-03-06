﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlexiSDK.Util
{
    public static class Extensions
    {
        public static string CamelCase(this string name)
        {
            if (Regex.IsMatch(name, @"[-_:]"))
            {
                name = Regex.Replace(name, @"[-_:][a-z]{1}", (match) => match.ToString().ToUpper());
                name = Regex.Replace(name, @"[-_:]", "");
            }

            return Char.ToUpper(name[0]) + name.Substring(1);
        }

        internal static T DeepCopy<T>(this object model) where T : class
        {
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var serial = JsonConvert.SerializeObject(model, jsonSettings);
           
            var clone = JsonConvert.DeserializeObject<T>(serial, jsonSettings);

            //Debug.WriteLine("Deep Copy");
            //Debug.WriteLine(JsonConvert.SerializeObject(clone, jsonSettings));

            return clone;
        }
    }
}
