using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Server
{

    /**
     *Does what is says on the tin, All things Encrytipn. All functions are static and can be called without initialisation 
     */
    class Encryption
    {
        //Used as the IV for the symetrical algorithm encryption
        public static byte[] BPASSWORD = new byte[] { 0xf1, 0xa8, 0xc2, 0xe5, 0xa2, 0xd6, 0xe5, 0x12, 0xb3, 0x01, 0xaa, 0xdb, 0xf1, 0x7f, 0x9f, 0x01 };
        public static int BITSINSALT = 256; 

        static RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        /**
         * Returns a sudo random salt, constructuted to work with a bas64 conversion.
         * */
        public static string GetSalt()
        {

            byte[] saltBytes = new byte[BITSINSALT / 8];

            new RNGCryptoServiceProvider().GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        /**
         *Not currently used as there is one that generates with a hash.
         */
        public static String GenerateHash(byte[] plainText)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            String str = Convert.ToBase64String((algorithm.ComputeHash(plainText)));
            return str;
        }

        /**
         * This function used RijndaelManaged class to encrypt a byte array, using the SALT and this classes BPASSWORD as the
         * Key and IV respectively.
         * @param plain the buffer to be encrypted.
         * @param SALT the salt to be used as the key to the symetrical encryption
         */
        public static byte[] Encrypt(byte[] plain, string SALT )
        {
            
            byte[] salt = Convert.FromBase64String(SALT);

            var algorithm = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

            //set algorithm key and IV. Also set padding mode. The algorithm needs padding or else it wont work
            algorithm.Key = salt;
            algorithm.IV = BPASSWORD;
            algorithm.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = algorithm.CreateEncryptor();

            //actually do the encryption
            byte[] resultArray = cTransform.TransformFinalBlock(plain, 0, plain.Length);

            return resultArray;
        }

        /**
         *Decrypts an incoming cipher using the salt and the hardcoded byte password
         * @param ciper the bytearray that is to be decoded
         * @param SALT the salt to act as the key to decrypt the message
         */
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
