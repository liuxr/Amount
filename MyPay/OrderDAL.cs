/* 
   ======================================================================== 
    文 件 名：     
	功能描述：                
    作    者:	Cumin           
    创建时间： 	2016/10/24 17:53:36
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
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace MyPay
{

    public class OrderDAL
    {
        private static string DataSource = @AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "DB.db";
        public OrderDAL()
        {

        }

        /// <summary>
        /// 获取所有的资源信息
        /// </summary>
        /// <returns></returns>
        public List<Order> GetList()
        {
            List<Order> list = new List<Order>();
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = DataSource;
            using (IDbConnection con = new SQLiteConnection(sb.ToString()))
            {
                con.Open();
                string sql = "select * from 'order'";
                list = Dapper.SqlMapper.Query<Order>(con, sql).ToList();
                con.Close();
            }

            return list;

        }

        public void Insert(List<Order> list)
        {
            //开始事务
            IDbTransaction transaction = null;
            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = DataSource;


                using (IDbConnection con = new SQLiteConnection(sb.ToString()))
                {
                    con.Open();
                    transaction = con.BeginTransaction();
                    foreach (Order model in list)
                    {
                        string sql = "insert into 'order' (createDate,name,orderNo,tradeNo,batchNo,payee,amount,state,UpLoadState,UpLoadCount,UpLoadError,UpLoadDate) values (@createDate,@name,@orderNo,@tradeNo,@batchNo,@payee,@amount,@state,@UpLoadState,@UpLoadCount,@UpLoadError,@UpLoadDate)";
                        int i = Dapper.SqlMapper.Execute(con, sql, new
                        {
                            createDate = model.CreateDate,
                            name = model.Name,
                            orderNo = model.OrderNo,
                            tradeNo = model.TradeNo,
                            batchNo = model.BatchNo,
                            payee = model.Payee,
                            amount = model.Amount,
                            state = model.State,
                            UpLoadState = model.UpLoadState,
                            UpLoadCount = model.UpLoadCount,
                            UpLoadError = model.UpLoadError,
                            UpLoadDate = model.UpLoadDate
                        }, transaction, null, null);
                    }
                    //提交事务
                    transaction.Commit();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    //出现异常，事务Rollback
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
            }
        }

        public void Insert(Order model)
        {

            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = DataSource;
                using (IDbConnection con = new SQLiteConnection(sb.ToString()))
                {
                    con.Open();
                    string sql = "insert into 'order' (createDate,name,orderNo,tradeNo,batchNo,payee,amount,state,UpLoadState,UpLoadCount,UpLoadError,UpLoadDate) values (@createDate,@name,@orderNo,@tradeNo,@batchNo,@payee,@amount,@state,@UpLoadState,@UpLoadCount,@UpLoadError,@UpLoadDate)";
                    int i = Dapper.SqlMapper.Execute(con, sql, new
                    {
                        createDate = model.CreateDate,
                        name = model.Name,
                        orderNo = model.OrderNo,
                        tradeNo = model.TradeNo,
                        batchNo = model.BatchNo,
                        payee = model.Payee,
                        amount = model.Amount,
                        state = model.State,
                        UpLoadState = model.UpLoadState,
                        UpLoadCount = model.UpLoadCount,
                        UpLoadError = model.UpLoadError,
                        UpLoadDate = model.UpLoadDate
                    });

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        }
    }
}
