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
    /// <summary>
    /// 更新登录名
    /// </summary>
    /// <param name="userName"></param>
    public delegate void UpdateName(string userName);
    public delegate void GetContext(string html,List<Order> newOrders,bool canWatch=false);
    public partial class FrmBrower : Form
    {
        /// <summary>
        /// 原来的订单列表
        /// </summary>
        private List<Order> OldList = new List<Order>();

        /// <summary>
        /// 新的订单列表
        /// </summary>
        private List<Order> NewList = new List<Order>();
       
        /// <summary>
        /// 订单操作类
        /// </summary>
        private OrderDAL orderDal = new OrderDAL();
       
        /// <summary>
        /// 获取内容
        /// </summary>
        public GetContext onGetContext;

        /// <summary>
        /// 更新用户名
        /// </summary>
        public UpdateName onUpdateName;

        /// <summary>
        /// Html元素操作类
        /// </summary>
        private Element element = new Element();

        /// <summary>
        /// 高级版的访问页面地址
        /// </summary>
        private string url = "https://consumeprod.alipay.com/record/advanced.htm";

        /// <summary>
        /// 登录界面Url
        /// </summary>
        private string loginUrl = "https://auth.alipay.com/login/index.htm?goto=https%3A%2F%2Fconsumeprod.alipay.com%2Frecord%2Fadvanced.htm";

        private string loginIndexUrl = "https://authzui.alipay.com/login/index.htm";

        private string NavigatingUrl = "";

        /// <summary>
        /// 登录名
        /// </summary>
        private string loginName = string.Empty;

        /// <summary>
        /// 定时器，进行定时采集
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// 是否监视
        /// </summary>
        private bool isWatch = false;
        
        /// <summary>
        /// 是否监视
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
        /// <summary>
        /// 是否可以监视
        /// </summary>
        public bool CanWatch
        {
            get
            {
                return canWatch;
            }

            set
            {
                canWatch = value;
            }
        }

        private bool canWatch = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmBrower()
        {
            InitializeComponent();
            webBrowser1.IsWebBrowserContextMenuEnabled = false;
            webBrowser1.ScriptErrorsSuppressed = false;
            webBrowser1.WebBrowserShortcutsEnabled = false;

            timer = new Timer();
            timer.Interval = 20 * 1000;
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 定时访问地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }

        /// <summary>
        /// 监视开始
        /// </summary>
        public void Start()
        {
            timer.Start();
            isWatch = true;
        }

        /// <summary>
        /// 监视结束
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            isWatch = false;
        }

        /// <summary>
        /// 设置采集时间间隔
        /// </summary>
        /// <param name="time"></param>
        public void SetInterval(int time) {
            timer.Interval = time*1000;
        }

        /// <summary>
        /// 进入页面后直接访问地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }

        /// <summary>
        /// 访问地址完成后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //请求的URL(全部转换为小写)
            string requestUrl = e.Url.AbsoluteUri.ToLower();
            AutoLogin();

            return;
            if (NavigatingUrl.Equals(loginIndexUrl) && !requestUrl.Equals(loginIndexUrl)) {
                if (onUpdateName != null) {
                    onUpdateName(loginName);
                }
            }
           

            //需要采集的页面
            if (!url.Equals(requestUrl))
                return;

            LoadInfo();
        }


        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
           
            string requestUrl = e.Url.AbsoluteUri.ToLower();
            NavigatingUrl = requestUrl;
            //获取登录名并通知主界面更新用户名
            UpdateName(requestUrl);
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string requestUrl = e.Url.AbsoluteUri.ToLower();
        }

        private void UpdateName(string requestUrl)
        {
            //如果是登录界面
            if (!requestUrl.Equals(loginIndexUrl.ToLower()))
                return;
            //获取登录界面的名称
            //  "J-input-user"
            HtmlElement htmlElement = element.GetElement_Id(webBrowser1, "J-input-user");
            if (htmlElement == null)
                return;
            loginName = htmlElement.GetAttribute("value");
        }

        private void LoadInfo()
        {
            canWatch = true;
            if (onGetContext == null)
                return;
            string html = GetHtmlString();

            //HtmlElement htmlElement = element.GetElement_Id(webBrowser1, "global-username");
            //string loginName = htmlElement.InnerText;

            List<Order> newOrders = new List<MyPay.Order>();
            List<Order> list = GetTableContent();
            if (list.Count > 0)
            {
                if (isWatch)//进入监视状态
                {
                    NewList = list;
                    //比较，前后两个集合的差集
                    newOrders = NewList.Except(OldList, new OrderCompare()).ToList();
                    //采集到数据了
                    if (newOrders.Count > 0)
                    {
                        orderDal.Insert(newOrders);
                        OldList = NewList;
                    }
                }
                else
                {
                    OldList = list;//未进入监视状态
                }
            }
            onGetContext(html,newOrders,canWatch);

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
                //if (string.IsNullOrEmpty(order.OrderNo))
                //    continue;
                //if (order.Amount.Contains("-"))
                //{
                //    continue;
                //}
                list.Add(order);
            }
            return list;
        }

        private void FrmBrower_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        //自动登录
        private void AutoLogin() {
            //获取登录界面的名称
            //  "J-input-user"
            HtmlElement htmlElement = element.GetElement_Id(webBrowser1, "J-input-user");
            if (htmlElement == null)
                return;
            htmlElement.SetAttribute("value","liu.xr90@163.com");
            
                htmlElement = element.GetElement_Id(webBrowser1, "password_rsainput");
            if (htmlElement == null)
                return;
            htmlElement.SetAttribute("value", "394523078lxr");

            //"J-login-btn" "login"
            htmlElement = element.GetElement_Id(webBrowser1, "login");
            if (htmlElement == null)
                return;
            element.Btn_click(htmlElement, "submit");


            //htmlElement = element.GetElement_Id(webBrowser1, "J-checkcodeIcon");
            //if (htmlElement == null)
            //    return;
            htmlElement.Click += HtmlElement_Click;

            
        }

        private void HtmlElement_Click(object sender, HtmlElementEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
