/* 
   ======================================================================== 
    文 件 名：     
	功能描述：                
    作    者:	Cumin           
    创建时间： 	2016/11/2 15:23:34
	版    本:	V1.0.0
   ------------------------------------------------------------------------
	历史更新纪录
   ------------------------------------------------------------------------
	版    本：           修改时间：           修改人：          
	修改内容：
   ------------------------------------------------------------------------
	Copyright (C) 2016   北京荣大科技有限公司
   ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyPay
{
    public class Crypt3Des
    {
        private string key = string.Empty;
        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }
        public Crypt3Des(string key) {
            this.key = key;
        }
        private SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
        public string Encrypt(string input) {
            mCSP.Padding = PaddingMode.None;
            mCSP.Mode = CipherMode.ECB;
            int size = mCSP.FeedbackSize;
            string input1 = Pkcs5_pad(input, size);
            string k = key.PadRight(24, '0');
            //mCSP.KeySize = 24;
            //mCSP.Key = Convert.FromBase64String(k);
            mCSP.Key =  System.Text.Encoding.Default.GetBytes(k.Substring(0,8));
            mCSP.GenerateIV();
            using (ICryptoTransform ct = mCSP.CreateEncryptor())
            {
               // byte[] byt = Convert.FromBase64String(input1);
                byte[] byt =  Encoding.UTF8.GetBytes(input1);
                byte[] results = ct.TransformFinalBlock(byt, 0, 8);
                string a = Convert.ToBase64String(results);
            }
            //mCSP.IV=Convert.FromBase64String()
            return "";
        }




        //// <SUMMARY>
            /// 加密
            /// </SUMMARY>
            /// <PARAM name="strString"></PARAM>
            /// <PARAM name="strKey"></PARAM>
            /// <PARAM name="encoding"></PARAM>
            /// <RETURNS></RETURNS>
        public string Encrypt3DES(string strString)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            DES.Key = System.Text.Encoding.Default.GetBytes(this.Key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.Zeros;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = System.Text.Encoding.Default.GetBytes(strString);

            string a= Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            return a;
        }



        /// <summary>
        /// 一种补充算法
        /// </summary>
        /// <param name="text"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public string Pkcs5_pad(string text, int blockSize) {
            int pad = blockSize - (text.Length % blockSize);
            byte[] array = new byte[1];
            array[0] = (byte)(pad);
            string ch = Convert.ToString(System.Text.Encoding.ASCII.GetString(array));
            return text;
            // return text + Repeat(ch, pad);
        }

        /// <summary>
        /// 字符串重复次数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public string Repeat(string str, int multiplier) {
            string result=string.Empty;
            for (int i = 0; i < multiplier; i++) {
                result = str;
            }
            return result;
        }


        private void Test() {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            byte[] buffer = Encoding.Default.GetBytes("明文");
            MemoryStream stream = new MemoryStream();
            byte[] key = Convert.FromBase64String("AQjP4U1aCnnybWsmHUQ7BVIxHyrnQ2AP");
            CryptoStream encStream = new CryptoStream(stream, des.CreateEncryptor(key, null), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            byte[] res = stream.ToArray();
            Console.WriteLine("result:" + Convert.ToBase64String(res));
        }


        public  string Encrypt3DES(string a_strString, string a_strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(a_strString);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Value">明文</param>
        /// <returns>密文 base64转码</returns>
        public string EncryptString(string Value)
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(GetString(this.Key));
            // mCSP.IV = Convert.FromBase64String(sIV);
            mCSP.GenerateIV();
            Console.WriteLine("Key:" + mCSP.Key.ToString() + ",IV:" + mCSP.IV.ToString());
            //指定加密的运算模式
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            //获取或设置加密算法的填充模式
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        private string GetString(string imgData) {
            string dummyData = imgData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+").Replace(".", "").Replace("@", "+");
            if (dummyData.Length % 4 > 0)
            {
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            }

            return dummyData;
        }
    }
}
