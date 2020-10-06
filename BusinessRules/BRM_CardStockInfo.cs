using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace BusinessRules
{
    public class BRM_CardStockInfo : BRBase<Entity_CardStockInfo>
    {
        /// <summary>
        /// 功能說明:查詢自取卡片資料
        /// 作    者:linda
        /// 創建時間:2010/06/22
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetSelfPickInfoMerch(ref  DataTable dtSelfPickInfoMerch, int iPageIndex, int iPageSize, ref int iTotalCount, string strMerchDate,string strFactory)
        {
            try
            {
                string strMerchDateSQL = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strMerchDate.Replace("/", ""), -1);
                strMerchDateSQL = strMerchDateSQL.Substring(0, 4) + "/" + strMerchDateSQL.Substring(4, 2) + "/" + strMerchDateSQL.Substring(6, 2);

                string sql = @"SELECT * FROM(";
                //自取
                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'')  as OutStore_Date from dbo.tbl_Card_BaseInfo where kind='1' and isnull(Urgency_Flg,'')<>'1' and (indate1=@strMerchDateSQL or (indate1<@strMerchDateSQL and isnull(IntoStore_Status,'0')='0'))";
                sql += " and cardtype<>'900'";//此種卡片為特殊處理 Bug234
                if (strFactory != "0")
                {
                    sql += " and Merch_Code=@Merch_Code";
                }
                sql += " union";
                //自取+緊急製卡
                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo where kind='1' and Urgency_Flg='1' and (indate1='" + strMerchDate + "' or (indate1<'" + strMerchDate + "'and isnull(IntoStore_Status,'0')='0'))";
                sql += " and cardtype<>'900'";//此種卡片為特殊處理 Bug234
                if (strFactory !="0")
                {
                    sql += " and Merch_Code=@Merch_Code";
                }
                sql += " union";
                //其他取卡方式+退件改自取
                sql += " select base.custname,base.id,base.cardno,base.indate1,base.IntoStore_Date,base.action,base.trandate,isnull(base.OutStore_Date,'') as OutStore_Date "; 
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";  
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and ((InformMerchDate<@InformMerchDate and isnull(IntoStore_Status,'0')<>'1') or ";
                sql += " (InformMerchDate=@InformMerchDate and (isnull(IntoStore_Status,'0')<>'1' or (IntoStore_Status='1' and IntoStore_Date>=back.ImportDate))))";
                if (strFactory != "0")
                {
                    sql += " and base.Merch_Code=@Merch_Code";
                }
                sql += " union";
                //其他取卡方式+退件改自取 且為 自取->郵寄出庫->退件->自取 的情況
                sql += " select base.custname,base.id,base.cardno,base.indate1,'' as IntoStore_Date,base.action,base.trandate,'' as OutStore_Date "; 
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base ";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate ";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0' and (InformMerchDate=@InformMerchDate or InformMerchDate<@InformMerchDate)  and IntoStore_Status='1' and IntoStore_Date<back.ImportDate";
                if (strFactory != "0")
                {
                    sql += " and base.Merch_Code=@Merch_Code";
                }

                sql += ")U ";
                sql += "ORDER BY U.IntoStore_Date DESC,U.id";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strMerchDateSQL", strMerchDateSQL));
                if (strFactory != "0")
                    sqlcmd.Parameters.Add(new SqlParameter("@Merch_Code", strFactory));
                sqlcmd.Parameters.Add(new SqlParameter("@InformMerchDate", strMerchDate));
                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtSelfPickInfoMerch = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:查詢自取卡片資料
        /// 作    者:linda
        /// 創建時間:2010/06/22
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetSelfPickInfoFetch(ref  DataTable dtSelfPickInfoFetch, int iPageIndex, int iPageSize, ref int iTotalCount, string strFetchDate)
        {
            try
            {
                //string strFetchDateSQL1 = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strFetchDate.Replace("/", ""), -15);
                //strFetchDateSQL1 = strFetchDateSQL1.Substring(0, 4) + "/" + strFetchDateSQL1.Substring(4, 2) + "/" + strFetchDateSQL1.Substring(6, 2);

                //string strFetchDateSQL2 = CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", strFetchDate.Replace("/", ""), -14);
                //strFetchDateSQL2 = strFetchDateSQL2.Substring(0, 4) + "/" + strFetchDateSQL2.Substring(4, 2) + "/" + strFetchDateSQL2.Substring(6, 2);

                string strFetchDateSQL1 = DateTime.Parse(strFetchDate).AddDays(-15).ToString("yyyy/MM/dd");
                string strFetchDateSQL2 = DateTime.Parse(strFetchDate).AddDays(-14).ToString("yyyy/MM/dd");

                string sql = @"SELECT * FROM(";
                //自取
                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo where kind='1' and isnull(Urgency_Flg,'')<>'1' and indate1<= @strFetchDateSQL1 and IntoStore_Status='1'";
                sql += " and(isnull(OutStore_Status,'0')='0' or isnull(OutStore_Date,'') >(select top 1 DailyCloseDate from dbo.tbl_Card_DailyClose order  by DailyCloseDate desc))";//排除「已出庫、且出庫日已日結」的資料
                sql += " and cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " union";
                //自取+緊急製卡
                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo where kind='1' and Urgency_Flg='1' and indate1<= @strFetchDateSQL2 and IntoStore_Status='1'";
                sql += " and(isnull(OutStore_Status,'0')='0' or isnull(OutStore_Date,'') >(select top 1 DailyCloseDate from dbo.tbl_Card_DailyClose order  by DailyCloseDate desc))";//排除「已出庫、且出庫日已日結」的資料
                sql += " and cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " union";
                //其他取卡方式+退件改自取
                sql += " select base.custname,base.id,base.cardno,base.indate1,IntoStore_Date,base.action,base.trandate,isnull(base.OutStore_Date,'') as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base ";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate ";
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and InformMerchDate <= @strFetchDateSQL2 and IntoStore_Status='1' and IntoStore_Date>=back.ImportDate";
                sql += " and(isnull(OutStore_Status,'0')='0' or isnull(OutStore_Date,'') >(select top 1 DailyCloseDate from dbo.tbl_Card_DailyClose order  by DailyCloseDate desc))";//排除「已出庫、且出庫日已日結」的資料
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234

                sql += ")U ";
                sql += "ORDER BY U.IntoStore_Date DESC,U.id";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strFetchDateSQL1", strFetchDateSQL1));
                sqlcmd.Parameters.Add(new SqlParameter("@strFetchDateSQL2", strFetchDateSQL2));

                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtSelfPickInfoFetch = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢自取卡片資料
        /// 作    者:linda
        /// 創建時間:2010/06/22
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetSelfPickInfoOther(ref  DataTable dtSelfPickInfoOther,int iPageIndex,int iPageSize,ref int iTotalCount,string strId, string strCardNo, string strFromDate, string strToDate)
        {
            if (string.IsNullOrEmpty(strFromDate))
            {
                strFromDate = "1911/01/01";
            }
            if (string.IsNullOrEmpty(strToDate))
            {
                strToDate = "9999/12/31";
            }

            try
            {
                string sql = @"SELECT * FROM(";

                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo base where (kind='1' or isnull(IntoStore_Date,'')<>'')";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and IntoStore_Date between @strFromDate and @strToDate";
                
                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id=@id"; 
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno=@cardno";
                }
                //其他取卡方式+退件改自取
                sql += " union";
                sql += " select base.custname,base.id,base.cardno,base.indate1,base.IntoStore_Date,base.action,base.trandate,isnull(base.OutStore_Date,'') as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date>=back.ImportDate";
                sql += " and IntoStore_Date between @strFromDate and @strToDate";

                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id=@id";
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno=@cardno";
                }
                //其他取卡方式+退件改自取 且為 自取->郵寄出庫->退件->自取 的情況
                sql += " union";
                sql += " select base.custname,base.id,base.cardno,base.indate1,'' as IntoStore_Date,base.action,base.trandate,'' as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date<back.ImportDate";
                sql += " and IntoStore_Date between @strFromDate and @strToDate";
                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id=@id";
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno=@cardno";
                }

                sql += ")U ";
                sql += "ORDER BY U.IntoStore_Date DESC,U.id";             

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strFromDate", strFromDate));
                sqlcmd.Parameters.Add(new SqlParameter("@strToDate", strToDate));
                if (!strId.Equals(string.Empty))
                    sqlcmd.Parameters.Add(new SqlParameter("@id", strId));
                if (!strCardNo.Equals(string.Empty))
                    sqlcmd.Parameters.Add(new SqlParameter("@cardno", strCardNo));
                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtSelfPickInfoOther = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:入庫操作-更新卡片基本信息、新增庫存信息
        /// 作    者:linda
        /// 創建時間:2010/06/23
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool IntoStore(string strAction, string strId, string strCardNo, string strTrandate, string strCustname)
        {
            try
            {

                // 專案代號:20200031-CSIP EOS 功能說明:處理資安-CSIP EOS 作者:Ares Luke 創建時間:2020/08/19
                string sql = @"Update dbo.tbl_Card_BaseInfo";
                sql += " Set IntoStore_Status='1',IntoStore_Date=convert(nvarchar(10),getdate(),111),OutStore_Status='0',OutStore_Date=''";
                sql += " Where action= @strAction and id = @strId and cardno= @strCardNo and trandate= @strTrandate";

                sql += " Insert into dbo.tbl_Card_StockInfo(IntoStore_Date,cardno,OutStore_Status,OutStore_Date,custname,id,action,trandate)";
                sql += " Values(convert(nvarchar(10),getdate(),111), @strCardNo ,'0','', @strCustname , @strId , @strAction , @strTrandate )";

                SqlCommand sqlcmd = new SqlCommand {CommandType = CommandType.Text, CommandText = sql};
                sqlcmd.Parameters.Add(new SqlParameter("@strAction", strAction));
                sqlcmd.Parameters.Add(new SqlParameter("@strId", strId));
                sqlcmd.Parameters.Add(new SqlParameter("@strCardNo", strCardNo));
                sqlcmd.Parameters.Add(new SqlParameter("@strTrandate", strTrandate));
                sqlcmd.Parameters.Add(new SqlParameter("@strCustname", strCustname));

                if (BRM_CardStockInfo.Add(sqlcmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:出庫操作-更新卡片基本信息、刪除庫存信息
        /// 作    者:linda
        /// 創建時間:2010/06/23
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool OutStore(string strAction, string strId, string strCardNo, string strTrandate, string strCustname,string strIntoStoreDate)
        {
            try
            {
                // 專案代號:20200031-CSIP EOS 功能說明:處理資安-CSIP EOS 作者:Ares Luke 創建時間:2020/08/19
                string sql = @"Update dbo.tbl_Card_BaseInfo";
                sql += " Set IntoStore_Status='0',IntoStore_Date='',OutStore_Status='0',OutStore_Date=''";
                sql += " Where action= @strAction  and id= @strId and cardno= @strCardNo and trandate= @strTrandate";

                sql += " Delete from dbo.tbl_Card_StockInfo";
                sql += " where IntoStore_Date= @strIntoStoreDate and cardno= @strCardNo ";

                SqlCommand sqlcmd = new SqlCommand {CommandType = CommandType.Text, CommandText = sql};
                sqlcmd.Parameters.Add(new SqlParameter("@strAction", strAction));
                sqlcmd.Parameters.Add(new SqlParameter("@strId", strId));
                sqlcmd.Parameters.Add(new SqlParameter("@strCardNo", strCardNo));
                sqlcmd.Parameters.Add(new SqlParameter("@strTrandate", strTrandate));
                sqlcmd.Parameters.Add(new SqlParameter("@strIntoStoreDate", strIntoStoreDate));

                if (BRM_CardStockInfo.Delete(sqlcmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆庫存資料
        /// 作    者:Linda
        /// 創建時間:2010/06/25
        /// 修改記錄:
        /// </summary>
        /// <param name="Back"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(Entity_CardStockInfo CardStockInfo, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CardStockInfo.UpdateEntityByCondition(CardStockInfo, strCondition, FiledSpit))
                    {
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:卡片庫存信息查詢卡片掛號號碼By CardNo,IntoStore_Date
        /// 作    者:Linda
        /// 創建時間:2010/12/30
        /// 修改記錄:
        /// </summary>
        /// <param name="strCardNo"></param>
        /// <param name="strIntoStoreDate"></param>
        /// <returns></returns>
        public static bool SearchMailNo(ref  DataTable dtMailNo, string strCardNo,string strIntoStoreDate)
        {
            try
            {
                // 專案代號:20200031-CSIP EOS 功能說明:處理資安-CSIP EOS 作者:Ares Luke 創建時間:2020/08/19
                string sql = @"select mailno from tbl_Card_StockInfo where cardno = @strCardNo and IntoStore_Date = @strIntoStoreDate ";
                SqlCommand sqlcmd = new SqlCommand {CommandType = CommandType.Text, CommandText = sql};
                sqlcmd.Parameters.Add(new SqlParameter("@strCardNo", strCardNo));
                sqlcmd.Parameters.Add(new SqlParameter("@strIntoStoreDate", strIntoStoreDate));

                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtMailNo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }

    }
}
