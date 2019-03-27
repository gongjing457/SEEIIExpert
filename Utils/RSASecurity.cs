using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace SEEIPro.Utils
{
    public class RSASecurity
    {
        private static ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }
        /// <summary>
        /// Create public/private key
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CreateRsaPair()
        {
            var keyPair = new Dictionary<string, string>();
            var rsaProvider = new RSACryptoServiceProvider(1024);
            RSAParameters para = rsaProvider.ExportParameters(true);
            keyPair.Add("PUBLIC", BytesToHexString(para.Exponent) + "," + BytesToHexString(para.Modulus));
            keyPair.Add("PRIVATE", rsaProvider.ToXmlString(true));
            try
            {
                StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["RSAPrivateKey"]);
                string privatekeyXml = rsaProvider.ToXmlString(true);
                writer.Write(privatekeyXml);
                writer.Close();
            }
            catch (Exception) { throw; }

            return keyPair;
        }



        /// <summary>
        /// DencryRSA
        /// </summary>
        /// <param name="encryptdata"></param>
        /// <param name="privatekey"></param>
        /// <returns></returns>
        public static string DecryptRSA(string encryptdata, string privatekey)
        {
            string decryptdata = "";
            try
            {
                var provider = new RSACryptoServiceProvider();
                provider.FromXmlString(privatekey);
                byte[] result = provider.Decrypt(HexstringToBytes(encryptdata), false);
                ASCIIEncoding enc = new ASCIIEncoding();
                decryptdata = enc.GetString(result);
            }
            catch (Exception e)
            {
                throw new Exception("RSA解密出错！", e);
            }
            return decryptdata;
        }


        /// <summary>
        /// Bytes[] to String
        /// </summary>
        /// <param name="bytestr">bytes[]</param>
        /// <returns></returns>
        private static string BytesToHexString(byte[] bytestr)
        {
            StringBuilder hexstring = new StringBuilder(64);
            for (int i = 0; i < bytestr.Length; i++)
            {
                hexstring.Append(String.Format("{0:x2}", bytestr[i]));
            }
            return hexstring.ToString();
        }

        /// <summary>
        /// Hex to Bytes[]
        /// </summary>
        /// <param name="hex">hex</param>
        /// <returns></returns>
        private static byte[] HexstringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return result;
        }

        /// <summary>
        /// GET Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object CacheGet(string key)
        {
            return Cache[key];
        }

        /// <summary>
        /// SET Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cachetime"></param>
        public static void CacheSet(string key, object data, int cachetime)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(cachetime);
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// Remove Cache
        /// </summary>
        /// <param name="key"></param>
        public static void CacheRemove(string key)
        {
            Cache.Remove(key);
        }

    }



}