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
            int size = mCSP.BlockSize;
            string input1 = Pkcs5_pad(input, size);
            string k = key.PadRight(24, '0');
            mCSP.Mode = CipherMode.ECB;
            mCSP.Key = Convert.FromBase64String(key);
            //mCSP.IV=Convert.FromBase64String()
            return "";
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
            return text + Repeat(ch, pad);
        }

        /// <summary>
        /// 字符串重复次数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public string Repeat(string str, int multiplier) {
            string result = str;
            for (int i = 0; i < multiplier; i++) {
                result += str;
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
    }
}
