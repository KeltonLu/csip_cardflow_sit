//******************************************************************
//*  功能說明：尚未退回轉出的異動作業單
//*  作    者：JUN HU
//*  創建日期：2010/06/23
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using EntityLayer;

namespace BusinessRules
{
    public class BRM_Report : BRBase<EntityM_CallMail>
    {
        /// <summary>
        /// 功能說明:查詢扣卡明细帶分頁
        /// 作    者:JUN HU
        /// 創建時間:2010/06/23
        /// 修改記錄:20210118陳永銘
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchHoldCard(string strCondition, ref DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT distinct a.id,a.custname,a.custname_roma,a.cardno,a.indate1,a.action,a.Trandate,c.UpdDate,c.CNote,datediff(d,c.UpdDate,getdate()) kktimeA ");
                sbSql.Append("FROM ");
                sbSql.Append("tbl_Card_BaseInfo a join ");
                sbSql.Append("(select b.id,b.CardNo,b.action,b.Trandate,b.UpdDate,b.CNote ");
                sbSql.Append("from tbl_Card_DataChange b ");
                sbSql.Append("where b.OutputFlg not in ('N','T') and b.NewWay = '6') c ");
                sbSql.Append("on a.id=c.id and a.CardNo=c.CardNo and a.action=c.action and a.Trandate = c.Trandate ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append("ORDER BY kktimeA DESC");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    strMsgID = "06_02020000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02020000_005";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢扣卡明细
        /// 作    者:Linda
        /// 創建時間:2010/09/14
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchHoldCard(string strCondition, ref DataTable dtCardBaseInfo)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT a.id,a.custname,a.cardno,a.indate1,a.action,a.Trandate,c.UpdDate,c.CNote,a.custname_roma,datediff(d,c.UpdDate,getdate()) kktimeA ");
                sbSql.Append("FROM ");
                sbSql.Append("tbl_Card_BaseInfo a join ");
                sbSql.Append("(select b.id,b.CardNo,b.action,b.Trandate,b.UpdDate,b.CNote ");
                sbSql.Append("from tbl_Card_DataChange b ");
                sbSql.Append("where b.OutputFlg not in ('N','T') and b.NewWay = '6') c ");
                sbSql.Append("on a.id=c.id and a.CardNo=c.CardNo and a.action=c.action and a.Trandate = c.Trandate ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append("ORDER BY kktimeA DESC");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_Report.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_Report.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:清空tbl_HoldCard表
        /// 作    者:linda
        /// 創建時間:2010/09/14
        /// 修改記錄:
        /// </summary>
        /// <returns></returns>
        public static bool ClearHoldCard()
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("truncate table dbo.tbl_HoldCard");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                if (BRM_Report.Delete(sqlcmd))
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
                BRM_Report.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:寫入tbl_HoldCard表
        /// 作    者:linda
        /// 創建時間:2010/09/14
        /// 修改記錄:
        /// </summary>
        /// <returns></returns>
        public static bool InsetHoldCard(string strId, string strCustname, string strCustname_Roma, string strCardno, string strIndate1, string strUpdDate, string strCNote, string strkktime)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(" Insert into dbo.tbl_HoldCard Values(@strId, @strCustname, @strCardno, @strIndate1, @strUpdDate, @strCNote, @strkktime,@strCustname_Roma)");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                sqlcmd.Parameters.Add(new SqlParameter("@strId", strId));
                sqlcmd.Parameters.Add(new SqlParameter("@strCustname", strCustname));
                sqlcmd.Parameters.Add(new SqlParameter("@strCardno", strCardno));
                sqlcmd.Parameters.Add(new SqlParameter("@strIndate1", strIndate1));
                sqlcmd.Parameters.Add(new SqlParameter("@strUpdDate", strUpdDate));
                sqlcmd.Parameters.Add(new SqlParameter("@strCNote", strCNote));
                sqlcmd.Parameters.Add(new SqlParameter("@strkktime", strkktime));
                sqlcmd.Parameters.Add(new SqlParameter("@strCustname_Roma", strCustname_Roma));
                if (BRM_Report.Update(sqlcmd))
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
                BRM_Report.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:郵件交寄狀況檢核帶分頁
        /// 作    者:JUN HU
        /// 創建時間:2010/06/28
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchSendStatus(string dateFrom, string dateTo, ref DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                /////sbSql.Append("select tb1.card_file,isnull(tb1.allnum,0) as allnum, isnull(tb2.snum,0) as snum, isnull(tb3.fnum,0) as fnum from ");
                //sbSql.Append("select tb1.card_file,isnull(tb1.allnum,0) as allnum, isnull(tb2.snum,0) as snum, isnull(tb1.allnum,0)-isnull(tb2.snum,0) as fnum from ");
                //sbSql.Append("(select a.card_file,count(*) as allnum from tbl_Card_BaseInfo a where a.kind not in('1','2','9','10','11') and ");
                //sbSql.Append("(@indatefrom is null or convert(datetime,a.indate1 ,120)>=convert(datetime,@indatefrom ,120) ) and ");
                //sbSql.Append("(@indateto is null or convert(datetime,a.indate1 ,120)<=convert(datetime,@indateto ,120) ) ");
                //sbSql.Append("group by a.card_file) tb1 left join ");
                //sbSql.Append("(select a.card_file,count(*) as snum from tbl_Card_BaseInfo a join tbl_Post_Send b ");
                //sbSql.Append("on a.maildate = b.maildate and a.mailno = b.mailno where a.kind not in('1','2','9','10','11') and ");
                //sbSql.Append("(@indatefrom is null or convert(datetime,a.indate1 ,120)>=convert(datetime,@indatefrom ,120) ) and ");
                //sbSql.Append("(@indateto is null or convert(datetime,a.indate1 ,120)<=convert(datetime,@indateto ,120) ) ");
                //sbSql.Append("group by a.card_file) tb2 on tb1.card_file = tb2.card_file left join ");
                //sbSql.Append("(select a.card_file,count(*) as fnum from tbl_Card_BaseInfo a join tbl_Post_Send_F b ");
                //sbSql.Append("on a.maildate = b.maildate and a.mailno = b.mailno where a.kind not in('1','2','9','10','11') and ");
                //sbSql.Append("(@indatefrom is null or convert(datetime,a.indate1 ,120)>=convert(datetime,@indatefrom ,120) ) and ");
                //sbSql.Append("(@indateto is null or convert(datetime,a.indate1 ,120)<=convert(datetime,@indateto ,120) ) ");
                //sbSql.Append("group by a.card_file) tb3 on tb1.card_file = tb3.card_file");

                sbSql.Append("select tb1.card_file,isnull(tb1.allnum,0) as allnum, isnull(tb2.snum,0) as snum, isnull(tb1.allnum,0)-isnull(tb2.snum,0) as fnum from ");
                sbSql.Append("(select a.card_file,count(*) as allnum from tbl_Card_BaseInfo a where a.kind not in('1','2','9','10','11') and ");
                sbSql.Append("(@indatefrom is null or convert(datetime,a.indate1 ,120)>=convert(datetime,@indatefrom ,120) ) and ");
                sbSql.Append("(@indateto is null or convert(datetime,a.indate1 ,120)<=convert(datetime,@indateto ,120) ) ");
                sbSql.Append("group by a.card_file) tb1 left join ");
                sbSql.Append("(select a.card_file,count(*) as snum from tbl_Card_BaseInfo a");
                sbSql.Append(" where a.kind not in('1','2','9','10','11') and ");
                sbSql.Append(" isnull(a.maildate,'')<>'' and isnull(a.mailno,'')<>'' and");
                sbSql.Append("(@indatefrom is null or convert(datetime,a.indate1 ,120)>=convert(datetime,@indatefrom ,120) ) and ");
                sbSql.Append("(@indateto is null or convert(datetime,a.indate1 ,120)<=convert(datetime,@indateto ,120) ) ");
                sbSql.Append("group by a.card_file) tb2 on tb1.card_file = tb2.card_file");


                SqlCommand sqlcmd = new SqlCommand(sbSql.ToString());
                sqlcmd.Parameters.Add("@indatefrom", SqlDbType.NVarChar);
                sqlcmd.Parameters.Add("@indateto", SqlDbType.NVarChar);
                if (dateFrom.Equals(""))
                {
                    dateFrom = null;
                }
                if (dateTo.Equals(""))
                {
                    dateTo = null;
                }
                sqlcmd.Parameters["@indatefrom"].Value = dateFrom;
                sqlcmd.Parameters["@indateto"].Value = dateTo;
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    strMsgID = "06_02020000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02020000_005";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:郵件交寄狀況檢核未交寄明细帶分頁
        /// 作    者:JUN HU
        /// 創建時間:2010/06/29
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchSendDetail(string strCondition, ref DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("select a.id,a.cardno,a.trandate,");
                sbSql.Append("(case when a.kind ='0' then '普掛' when a.kind='1' then '自取' when a.kind='2' then '卡交介' when a.kind='3' then '限掛' ");
                sbSql.Append("when a.kind ='4' then '快遞' when a.kind ='5' then '三天快速發卡' when a.kind ='6' then '保留' when a.kind ='7' then '其他' ");
                sbSql.Append("when a.kind ='8' then '包裹' when a.kind ='9' then '限掛' when a.kind='10' then '自取' else '' end) as kind,");
                sbSql.Append("(case when a.Merch_Code ='A' then '台銘' when a.Merch_Code ='B' then '宏通' else '' end) as merchcode ");
                sbSql.Append("from tbl_Card_BaseInfo a join tbl_Post_Send_F b on a.maildate = b.maildate and a.mailno = b.mailno ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append("ORDER BY a.trandate DESC");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    strMsgID = "06_02020000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02020000_005";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查询角色
        /// 作    者:JUN HU
        /// 創建時間:2010/07/09
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <returns></returns>
        public static bool SearchRole(string strCondition, ref DataTable dtCode, ref string strMsgID)
        {
            try
            {
                string sql = @"select distinct m.ROLE_ID from M_USER m,R_ROLE_FUNCTION_KEY r where m.ROLE_ID=r.ROLE_ID and r.FUNCTION_KEY='06'";

                if (strCondition != "")
                {
                    strCondition = strCondition.Replace("UPPER(", "UPPER(m.");
                    sql += strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (ds != null)
                {
                    dtCode = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:Mail警訊通知查询
        /// 作    者:JUN HU
        /// 創建時間:2010/07/09
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtLogMail"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchLogMail(string strCondition, ref DataTable dtLogMail, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("select MailID,JobName,ConditionName,MailTittle,MailContext,ToUsers,CcUsers,CcUserName,ToUserName from tbl_CallMail");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append("WHERE ");
                    sbSql.Append(strCondition);
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtLogMail = ds.Tables[0];
                    strMsgID = "06_02020000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02020000_005";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查询用户名
        /// 作    者:JUN HU
        /// 創建時間:2010/07/09
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <returns></returns>
        public static bool SearchUser(string strCondition, ref DataTable dtCode, ref string strMsgID)
        {
            try
            {
                string sql = @"select USER_NAME,USER_MAIL from M_USER m,R_ROLE_FUNCTION_KEY r where m.ROLE_ID=r.ROLE_ID and r.FUNCTION_KEY='06'";

                if (strCondition != "")
                {
                    strCondition = strCondition.Replace("UPPER(", "UPPER(m.");
                    sql += strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (ds != null)
                {
                    dtCode = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新警訊通知發送用戶郵箱
        /// 作    者:JUN HU
        /// 創建時間:2010/07/12
        /// 修改記錄:
        /// </summary>
        /// <param name="CardDataChange"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(EntityM_CallMail callMail, string strCondition, ref string strMsgID, params string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_Report.UpdateEntityByCondition(callMail, strCondition, FiledSpit))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_003";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_004";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_Report.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查询OASA管制解管
        /// 作    者:JUN HU
        /// 創建時間:2010/07/14
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCode"></param>
        /// <returns></returns>
        public static bool SearchOASAG(string strCondition, ref DataTable dtCode, ref string strMsgID)
        {
            try
            {
                string sql = @"select CancelOASA_id from tbl_CancelOASA_UD";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCode = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                CSIPCommonModel.BusinessRules.BRM_PROPERTY_CODE.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }
    }
}
