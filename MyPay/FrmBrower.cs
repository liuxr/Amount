/*
 *1.SQLlite 存储问题
 *2.
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace MyPay
{
    public delegate void GetContext(string userName, string html,List<Order> newOrders);
    public partial class FrmBrower : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private List<Order> OrderList = new List<Order>();
        /// <summary>
        /// 
        /// </summary>
        private OrderDAL orderDal = new OrderDAL();
        /// <summary>
        /// 
        /// </summary>
        public GetContext onGetContext;

        private Element element = new Element();
        private string url = "https://consumeprod.alipay.com/record/advanced.htm";

        private Timer timer = null;

        private bool isWatch = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsWatch
        {
            get
            {
                return isWatch;
            }

            set
            {
                isWatch = value;
            }
        }

        public FrmBrower()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 20 * 1000;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
            //webBrowser1.Refresh();
            //LoadInfo();
        }

        public void Start()
        {
            timer.Start();
            isWatch = true;
        }

        public void Stop()
        {
            timer.Stop();
            isWatch = false;
        }

        public void SetInterval(int time) {
            timer.Interval = time*1000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!url.Equals(e.Url.AbsoluteUri.ToLower()))
                return;

            LoadInfo();
        }

        private void LoadInfo()
        {
            if (onGetContext == null)
                return;
            string html = GetHtmlString();

            HtmlElement htmlElement = element.GetElement_Id(webBrowser1, "global-username");
            string loginName = htmlElement.InnerText;

            List<Order> newOrders = new List<MyPay.Order>();
            if (isWatch)
            {
                List<Order> list = GetTableContent();
                if (list.Count <= 0) return;

                foreach (Order model in list)
                {
                    if (OrderList.Exists(m => m.OrderNo == model.OrderNo))
                        continue;
                    newOrders.Add(model);
                    orderDal.Insert(model);
                }
                OrderList.AddRange(newOrders);
                int count = OrderList.Count;
                if (count >= 100) {
                    OrderList.RemoveRange(count - 11, 10);
                }
            }

            onGetContext(loginName, html,newOrders);

        }

        /// <summary>
        /// 文档流转文本
        /// </summary>
        /// <returns></returns>
        public string GetHtmlString()
        {
            string html = string.Empty;
            using (Stream st = webBrowser1.DocumentStream)
            {
                StreamReader sr = new StreamReader(st, Encoding.GetEncoding("gbk"));
                html = sr.ReadToEnd();
            }
            return html;
        }

        /// <summary>
        /// 获取Table中内容，组织成集合
        /// </summary>
        /// <returns>返回订单集合</returns>
        private List<Order> GetTableContent()
        {
            //定义订单集合
            List<Order> list = new List<Order>();
            //获取Html中的Table
            HtmlElement table = element.GetElement_Id(webBrowser1, "tradeRecordsIndex");
            //html中没有table或者子元素小于3就直接返回
            if (table == null || table.Children.Count < 3)
                return list;
            //获取Table中第三个元素tr
            HtmlElement child = table.Children[2];
            //遍历所有的tr元素将td中的元素记录到集合中
            foreach (HtmlElement tr in child.Children)
            {
                Order order = new Order();
                HtmlElementCollection tds = tr.Children;
                //创建时间
                order.CreateDate = tds[0].InnerText.Replace("\r\n", " ");
                //名称
                order.Name = tds[2].InnerText;
                //分割获取商户订单号和交易号
                string no = tds[3].InnerText;
                string[] strs = no.Split(new char[] { '|', ':' });
                if (strs.Length == 2)
                {
                    //流水号
                    order.BatchNo = strs[1];
                }
                else if (strs.Length == 4)
                {
                    //商户订单号
                    order.OrderNo = strs[1];
                    //交易号
                    order.TradeNo = strs[3];
                }

                //对方
                order.Payee = tds[4].InnerText;
                //金额
                order.Amount = tds[5].InnerText;
                //状态
                order.State = tds[7].InnerText;
                if (string.IsNullOrEmpty(order.OrderNo))
                    continue;
                if (order.Amount.Contains("-"))
                {
                    continue;
                }
                list.Add(order);
            }
            return list;
        }

        private void FrmBrower_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }

}
