using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDA.Dell.Contarct
{
    public class DecryptUtility
    {

        private static readonly byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        private static readonly byte[] iv = { 8, 7, 6, 5, 4, 3, 2, 1 };
        static string strEConString = "";
        static string strDConString = "";
        public static string[] strColorsArr = null;
        public static Hashtable htColors = new Hashtable();



        public static string GetValue(string type)
        {

            Crypto objcrypto = new Crypto(key, iv);
            strEConString = objcrypto.GetEConnectionString(type);
            strDConString = objcrypto.GetDConnectionString(strEConString);

            return strDConString;

        }

        public static string GetDecryptValue(string EncryptString)
        {
            Crypto objcrypto = new Crypto(key, iv);
            strDConString = objcrypto.GetDConnectionString(EncryptString);

            return strDConString;
        }
    }
}
