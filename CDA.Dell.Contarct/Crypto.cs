using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CDA.Dell.Contarct
{
    internal class Crypto
    {
        public string GetEConnectionString(string type)
        {
            return ConStr(type);
            //  return Encrypt(constr);
        }

        public string GetDConnectionString(string constr)
        {
            return Decrypt(constr);
        }

        public string ConStr(string type)
        {
            string value = null;

            value = ConfigurationManager.AppSettings[type];

            #region Not Required
            //switch (type)
            //{
            //    case "DEEConstr":
            //        value = ConfigurationManager.AppSettings["DEEConstr"];
            //        break;
            //    case "DFEConstr":
            //        value = ConfigurationManager.AppSettings["DFEConstr"];
            //        break;
            //    case "LKMConstr":
            //        value = ConfigurationManager.AppSettings["LKMConstr"];
            //        break;
            //    case "DEELOGConstr":
            //        value = ConfigurationManager.AppSettings["DEELOGConstr"];
            //        break;
            //    case "DFELOGConstr":
            //        value = ConfigurationManager.AppSettings["DFELOGConstr"];
            //        break;
            //    case "DPDConstr":
            //        value = ConfigurationManager.AppSettings["DPDConstr"];
            //        break;
            //    default:
            //        throw new ApplicationException("Unrecognized value type in GetConnectionString.");
            //} 
            #endregion
            return value;
        }

        private TripleDESCryptoServiceProvider m_des = new TripleDESCryptoServiceProvider();
        // define the string handler

        private UTF8Encoding m_utf8 = new UTF8Encoding();
        // define the local property arrays
        private byte[] m_key;

        private byte[] m_iv;

        public Crypto(byte[] key, byte[] iv)
        {
            this.m_key = key;
            this.m_iv = iv;
        }

        private byte[] Encrypt(byte[] input)
        {
            return Transform(input, m_des.CreateEncryptor(m_key, m_iv));
        }

        private byte[] Decrypt(byte[] input)
        {
            return Transform(input, m_des.CreateDecryptor(m_key, m_iv));
        }

        public string Encrypt(string text)
        {
            byte[] input = m_utf8.GetBytes(text);
            byte[] output = Transform(input, m_des.CreateEncryptor(m_key, m_iv));
            return Convert.ToBase64String(output);
        }

        public string Decrypt(string text)
        {
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Transform(input, m_des.CreateDecryptor(m_key, m_iv));
            return m_utf8.GetString(output);
        }

        private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            // create the necessary streams
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);
            // transform the bytes as requested
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();
            // Read the memory stream and convert it back into byte array
            memStream.Position = 0;
            byte[] result = new byte[Convert.ToInt32(memStream.Length - 1) + 1];
            memStream.Read(result, 0, Convert.ToInt32(result.Length));
            // close and release the streams
            memStream.Close();
            cryptStream.Close();
            // hand back the encrypted buffer
            return result;
        }
    }
}
