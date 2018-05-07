using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    class Encryption
    {
        public static String PASSWORD = "123456789";
        public static byte[] BPASSWORD = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        public static int BITSINSALT = 256; 

        static RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        public static string GetSalt()
        {

            byte[] ivBytes = new byte[BITSINSALT / 8]; // 8 bits per byte

            new RNGCryptoServiceProvider().GetBytes(ivBytes);

            return Convert.ToBase64String(ivBytes);
        }
        public static String GenerateHash(byte[] plainText)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            String str = Convert.ToBase64String((algorithm.ComputeHash(plainText)));
            return str;
        }

        public static byte[] Encrypt(byte[] plain, string SALT )
        {
            
            byte[] salt = Convert.FromBase64String(SALT);

            var algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

            algorithm.Key = salt;
            algorithm.IV = BPASSWORD;
            algorithm.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = algorithm.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(plain, 0, plain.Length);

            return resultArray;
        }

        public static byte[] Decrypt(byte[] cipher, string SALT)
        {
            byte[] salt = Convert.FromBase64String(SALT);
            var algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

            algorithm.Key = salt;
            algorithm.IV = BPASSWORD;
            algorithm.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = algorithm.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(cipher, 0, cipher.Length);

            return resultArray;
        }

        public static String GenerateSaltedHash(String plainText, String salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            String temp = plainText + salt;

            return Convert.ToBase64String(algorithm.ComputeHash(Encoding.Unicode.GetBytes(salt)));
        }

        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
