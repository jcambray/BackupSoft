using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Text;

namespace clientbackup
{
    public class Security
    {

        private const int caesarConst = 15;
        
        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string toMd5(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool compareToMd5(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = toMd5(input);

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string toCaesar(string str, int nb)
        {
            char[] c = str.ToCharArray(0, str.Length);
            for (int i = 0; i < c.Length; i++)
            {
                int j = (int)c[i];
                char newCarac = (char)(j + nb);
                c[i] = newCarac;
            }
            string caesarStr = new string(c);
            return caesarStr;
        }

        public string caesarToString(string caesarStr, int nb)
        {
            char[] c = caesarStr.ToCharArray(0, caesarStr.Length);
            for (int i = 0; i < c.Length; i++)
            {
                int j = (int)c[i];
                char newCarac = (char)(j - nb);
                c[i] = newCarac;
            }
            string str = new string(c);
            return str;
        }

        public static String Encrypt(string password)
        {
            byte[] passBytes = System.Text.Encoding.Unicode.GetBytes(password);
             return Convert.ToBase64String(passBytes);
        }

        public static string Decrypt(string encryptedpassword)
        {
            byte[] passBytes = Convert.FromBase64String(encryptedpassword);
            return System.Text.Encoding.Unicode.GetString(passBytes);
        }

    }
}
