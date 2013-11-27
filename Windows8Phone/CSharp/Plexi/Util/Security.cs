using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using Microsoft.Phone.Info;

namespace Plexi.Util
{
    public static class Security
    {
        internal static byte[] salt = null;

        public static byte[] Encrypt(string data)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(data);

                if (salt == null)
                {
                    GenerateSalt();
                }

                return ProtectedData.Protect(bytes, salt);
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

                byte[] bytes = ProtectedData.Unprotect(data, salt);

                return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
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
#if DEBUG
                hmac = new HMACSHA256();
#else
                string anid = UserExtendedProperties.GetValue("ANID2") as string;

                Debug.WriteLine(anid);

                byte[] anidAsBytes = Encoding.Unicode.GetBytes(anid);

                hmac = new HMACSHA256(anidAsBytes);
#endif
                salt = hmac.ComputeHash(duid);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("GenerateSalt Error:{0}", err.Message));
            }
        }
    }
}
