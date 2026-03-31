using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.CodeDom.Compiler;

namespace MessagerLib
{
    public class serverTools
    {
        public static object converByteToObject(byte[] b)
        {
            MemoryStream memstream = new MemoryStream();
            BinaryFormatter binform = new BinaryFormatter();
            memstream.Write(b, 0, b.Length);
            memstream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binform.Deserialize(memstream);
            return obj;
        }

        public static byte[] convertObjToByte(object o)
        {
            MemoryStream memstream = new MemoryStream();
            BinaryFormatter binform = new BinaryFormatter();
            binform.Serialize(memstream, o);
            return memstream.ToArray();
        }

        public static byte[] wrapMessage(byte[] mes)
        {
            byte[] lengPre = BitConverter.GetBytes(mes.Length);
            byte[] r = new byte[lengPre.Length + mes.Length];
            lengPre.CopyTo(r, 0);
            mes.CopyTo(r, lengPre.Length);
            return r;
        }

        public static byte[] encrypted_creds(string u, string p)
        {
            byte[] tempuserhash = Hash(u, get_peper());
            byte[] temppasshash = Hash(p, get_peper());
            byte[] temphash = Hash(temppasshash, tempuserhash);
            return temphash;
        }

        public static string bytetostringwird(byte[] b)
        {
            return Encoding.UTF8.GetString(b, 0, b.Length);
        }

        public static byte[] stringtobyte(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static string bytetostringexa(byte[] b)
        {
            var sBuilder = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                sBuilder.Append(b[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static byte[] get_peper()
        {
            return Encoding.UTF8.GetBytes("ʥѾ۞");
        }

        public static byte[] shahash(byte[] s)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hashValue = mySHA256.ComputeHash(s);
                return hashValue;
            }
        }

        public static byte[] Hash(string value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        public static byte[] Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();

            return new SHA256Managed().ComputeHash(saltedValue);
        }

        public static bool HasUpperLowerDigit(string text)
        {
            bool hasUpper = false; bool hasLower = false; bool hasDigit = false;
            for (int i = 0; i < text.Length && !(hasUpper && hasLower && hasDigit); i++)
            {
                char c = text[i];
                if (!hasUpper) hasUpper = char.IsUpper(c);
                if (!hasLower) hasLower = char.IsLower(c);
                if (!hasDigit) hasDigit = char.IsDigit(c);

            }
            return (hasUpper && hasLower && hasDigit);
        }

        public static bool isIdGood(string text)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length >= 7)
                {
                    ret = true;
                }
            }
            return ret;
        }

        public static bool isPassGood(string text)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > 7)
                {
                    ret = HasUpperLowerDigit(text);
                }
            }
            return ret;
        }

        public static bool isEmailGood(string text)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > 7)
                {
                    // Simple regex to check for a basic email format
                    var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                    ret = System.Text.RegularExpressions.Regex.IsMatch(text, emailPattern);
                }
            }
            return ret;
        }

    }
}

