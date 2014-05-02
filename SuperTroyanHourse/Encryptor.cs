using System;
using System.IO;
using System.Security.Cryptography;

namespace SuperTroyanHourse
{
    static class Encryptor
    {
        /// <summary>
        /// encryptes the given data using MD5 hash function
        /// </summary>
        /// <param name="data">the data</param>
        /// <returns>MD5 hash function result</returns>
        public static byte[] MD5Encryptor(byte[] data)
        {
            MD5CryptoServiceProvider c = new MD5CryptoServiceProvider();
            byte[] result = c.ComputeHash(data);
            return result;
        }

        /// <summary>
        /// encryptes the given data using SHA-1 hash function
        /// </summary>
        /// <param name="data">the data</param>
        /// <returns>SHA-1 hash function result</returns>
        public static byte[] SHA1Encryptor(byte[] data)
        {
            SHA1CryptoServiceProvider c = new SHA1CryptoServiceProvider();
            byte[] result = c.ComputeHash(data);
            return result;
        }

        /// <summary>
        /// Visnear between data & key
        /// </summary>
        /// <param name="data">the result</param>
        /// <param name="key">the key</param>
        /// <returns>the visnear result</returns>
        public static byte[] Visnear(byte[] data, byte[] key)
        {
            int length = key.Length;
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)((data[i] + key[i % length]) & 0xff);
            }
            return result;
        }
    }
}
