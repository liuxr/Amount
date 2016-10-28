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
            SetWidth(lvNear);
            SetWidth(lvWorking, false);

            LoadData();
            //更新用户名
            frmBrower.onUpdateName += (name) => {
                lblName.Text = name;
            };

            frmBrower.onGetContext += (( html, newOrders,canWatch) => {
                btnWatch.Enabled = canWatch;
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
                    model.CreateDate,
                    model.Name,
                    model.OrderNo,
                    model.TradeNo,
                    model.BatchNo,
                    model.Payee,
                    model.Amount,
                    model.State
                });

                lvNear.Items.Insert(0,item);
            }
        }

        private void AppendData(List<Order> list) {
            foreach (Order model in list)
            {
                ListViewItem item = new ListViewItem(new string[] {
                      model.ID.ToString(),
                    model.CreateDate,
                    model.Name,
                    model.OrderNo,
                    model.TradeNo,
                    model.BatchNo,
                    model.Payee,
                    model.Amount,
                    model.State
                });

                lvWorking.Items.Insert(0,item);
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


        private void SetWidth(ListView lv,bool showId=true) {
            int width = lv.Width;
            int w1 = lv.Columns[0].Width = showId ? 60 : 0;
            int w2 = lv.Columns[1].Width = 120;
            int w6 = lv.Columns[7].Width = 80;
            int w7 = lv.Columns[8].Width = 80;
            int w = width - (w1+ w2 + w6 + w7+16);
            
            lv.Columns[2].Width = w / 5;
            lv.Columns[3].Width = w / 5;
            lv.Columns[4].Width = w / 5;
            lv.Columns[5].Width = w / 5;
            lv.Columns[6].Width = w / 5;
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            SetWidth(lvNear);
            SetWidth(lvWorking, false);
        }
    }
}
