using System;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using BusinessRules;

namespace BusinessRulesNew
{
    public class BRM_CardStockInfo : BRBase<Entity_CardStockInfo>
    {
        /// <summary>
        /// 功能說明:查詢自取卡片資料(自取逾期改限掛)
        /// 作    者:
        /// 創建時間:2016/01/28
        /// 修改記錄:2020/12/14 陳永銘 新增custname_roma
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetSelfPickInfoPost(ref  DataTable dtSelfPickInfoPost, int iPageIndex, int iPageSize, ref int iTotalCount, string strId, string strCardNo)
        {            
            try
            {
                //20210120 (U)by Joe, 增加羅馬拼音
                string sql = @"SELECT * FROM(";

                sql += " select custname,custname_roma,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo base where (kind='1' or isnull(IntoStore_Date,'')<>'')";
                sql += " and base.cardtype<>'900'"; //此種卡片為特殊處理 Bug234
                sql += " and base.selfpick_type = '4'";  //自取逾期改限掛
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
                sql += " select base.custname,base.custname_roma,base.id,base.cardno,base.indate1,base.IntoStore_Date,base.action,base.trandate,isnull(base.OutStore_Date,'') as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date>=back.ImportDate";
                sql += " and base.selfpick_type = '4'";  //自取逾期改限掛
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
                sql += " select base.custname,base.custname_roma,base.id,base.cardno,base.indate1,'' as IntoStore_Date,base.action,base.trandate,'' as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//此種卡片為特殊處理 Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date<back.ImportDate";
                sql += " and base.selfpick_type = '4'";  //自取逾期改限掛
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
                if (!strId.Equals(string.Empty))
                    sqlcmd.Parameters.Add(new SqlParameter("@id", strId));
                if (!strCardNo.Equals(string.Empty))
                    sqlcmd.Parameters.Add(new SqlParameter("@cardno", strCardNo));
                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtSelfPickInfoPost = ds.Tables[0];
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
        /// 功能說明:出庫操作-更新卡片基本信息、刪除庫存信息(自取逾期改限掛)
        /// 作    者:
        /// 創建時間:2016/01/28
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool OutStore(string strAction, string strId, string strCardNo, string strTrandate, string strCustname, string strIntoStoreDate)
        {
            try
            {
                string sql = @"Update dbo.tbl_Card_BaseInfo";
                sql += " Set IntoStore_Status='0',IntoStore_Date='',OutStore_Status='0',OutStore_Date='',SelfPick_Type='',SelfPick_date = ''";
                sql += " Where action=@@action and id=@id and cardno=@cardno and trandate=@trandate";

                sql += " Delete from dbo.tbl_Card_StockInfo";
                sql += " where IntoStore_Date=@IntoStore_Date and cardno=@cardno";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@action", strAction));
                sqlcmd.Parameters.Add(new SqlParameter("@id", strId));
                sqlcmd.Parameters.Add(new SqlParameter("@cardno", strCardNo));
                sqlcmd.Parameters.Add(new SqlParameter("@trandate", strTrandate));
                sqlcmd.Parameters.Add(new SqlParameter("@IntoStore_Date", strIntoStoreDate));
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


    }
}
