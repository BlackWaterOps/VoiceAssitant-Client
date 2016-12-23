using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using Microsoft.Phone.Info;

namespace PlexiSDK.Util
{
    public static class Security
    {
        private static string key = "HMACKey";

        internal static byte[] salt = null;

        public static byte[] Encrypt(string data)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                if (salt == null)
                {
                    GenerateSalt();
                }

                return ProtectedData.Protect(bytes, null);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Encrypt Error:{0}", err.Message));
                return null;
            }
        }

        public static string Decrypt(byte[] data)
        {
            try
            {
                if (salt == null)
                {
                    GenerateSalt();
                }

                byte[] bytes = ProtectedData.Unprotect(data, null);

                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Decrypt Error:{0}", err.Message));
                return null;
            }
        }

        internal static void GenerateSalt()
        {
            try
            {
                byte[] duid = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

                HMACSHA256 hmac;

                try
                {
                    byte[] key = GetKey();

                    hmac = new HMACSHA256(key);
                }
                catch (KeyNotFoundException)
                {
                    hmac = new HMACSHA256();

                    StoreKey(hmac.Key);
                }

                salt = hmac.ComputeHash(duid);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("GenerateSalt Error:{0}", err.Message));
            }
        }

        private static void StoreKey(byte[] key)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                settings[Security.key] = key;
            }
            catch (KeyNotFoundException)
            {
                settings.Add(Security.key, key);
            }
            catch (ArgumentException)
            {
                settings.Add(Security.key, key);
            }

            settings.Save();
        }

        private static byte[] GetKey()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(Security.key))
            {
                throw new KeyNotFoundException();
            }

            return (byte[])settings[Security.key];
        }
    }
}
