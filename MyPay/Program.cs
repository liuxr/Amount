using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MyPay
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Crypt3Des c = new Crypt3Des(Request.Key);
            string input= "{\"account\":\"dejin6334@163.com\"}";
            c.Encrypt(input);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
