/* 
   ======================================================================== 
    文 件 名：     
	功能描述：                
    作    者:	Cumin           
    创建时间： 	2016/11/2 15:03:52
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
using System.Net;
using System.Text;

namespace MyPay
{
    public class Request
    {
        public static string Key = "keyjingcai2l.8ke520lhJin.Cai@ss283.229808e";

        ///<summary>
        ///采用https协议访问网络
        ///</summary>
        ///<param name="URL">url地址</param>
        ///<param name="strPostdata">发送的数据</param>
        ///<returns></returns>
        public static string OpenReadWithHttps(string URL, string strPostdata, string strEncoding)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }

        public void Test() {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["title"] = "交易名称";
            param["orderno"] = "商户订单号";
            param["tradeno"] = "支付宝交易单号";
            param["serialno"] = "流水号";
            param["money"] = "0.01";
            param["tradetime"] = "2016-10012 11：33：22";
            param["tradestatus"] = "交易成功";
            param["nickname"] = "支付宝名称";
            param["account"] = "wangjingcan@126.com";


            //m=Api&c=Alipay&a=alipaylog&inputparam
            string para = string.Empty;
            para = "m=Api&c=Alipay&a=alipaylog&inputparam=";
           

        }
    }
}
