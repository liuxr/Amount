/*
 备注订单号
 支付宝交易号
 收款金额
 付款人姓名
 付款时间 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyPay
{
    public partial class FrmMain : Form
    {
        FrmBrower frmBrower = new MyPay.FrmBrower();
        public FrmMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnShowWebBrower_Click(object sender, EventArgs e)
        {
            frmBrower.StartPosition = FormStartPosition.CenterScreen;
            frmBrower.Show();
            frmBrower.BringToFront();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadData();
            frmBrower.onGetContext += ((name, html, newOrders) => {
                lblName.Text = name;
                rtbHtml.Clear();
                rtbHtml.AppendText(html);
                AppendData(newOrders);
            });
            frmBrower.StartPosition = FormStartPosition.CenterScreen;
            frmBrower.Show();
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            if (btnWatch.Tag.Equals("0"))
            {
                btnWatch.Text = "监控已开启";
                btnWatch.Tag = "1";
                frmBrower.Start();
            }
            else {
                btnWatch.Text = "监控已关闭";
                btnWatch.Tag = "0";
                frmBrower.Stop();
            }
        }

        private void LoadData() {

            OrderDAL orderDal = new OrderDAL();
            List<Order> list = orderDal.GetList();

            foreach (Order model in list) {
                ListViewItem item = new ListViewItem(new string[] {
                    model.ID.ToString(),
                    model.OrderNo,
                    model.TradeNo,
                    model.Amount,
                    model.Payee,
                    model.CreateDate,
                    model.UpLoadState
                });

                lvNear.Items.Add(item);
            }
        }

        private void AppendData(List<Order> list) {
            foreach (Order model in list)
            {
                ListViewItem item = new ListViewItem(new string[] {
                    model.OrderNo,
                    model.TradeNo,
                    model.Amount,
                    model.Payee,
                    model.CreateDate,
                    model.UpLoadState
                });

                lvWorking.Items.Add(item);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int result = 20;
            bool b = int.TryParse(txtTimeSpan.Text.Trim(), out result);
            if (!b|| result<=0) {
                MessageBox.Show("Time is Invaid");
                return;
            }

            frmBrower.SetInterval(result);
        }
    }
}
