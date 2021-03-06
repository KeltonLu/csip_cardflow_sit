//******************************************************************
//*  功能說明：卡片基本資料處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Text;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using CSIPCommonModel.BusinessRules;

namespace BusinessRulesNew
{
    public class BRM_TCardBaseInfo : BRBase<Entity_CardBaseInfo>
    {
        /// <summary>
        /// 功能說明:查詢卡片基本資料帶分頁
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Search(string strCondition, ref  DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [indate1],[action],[kind],[cardtype],[photo],[affinity],[id],");
                sbSql.Append("[cardno],[cardno2],[zip],[add1],[add2],[add3],[mailno],[n_card],");
                sbSql.Append("[maildate],[expdate],[expdate2],[seq],[custname],[name1],[name2],");
                sbSql.Append("[trandate],[card_file],[disney_code],[branch_id],[Merch_Code],");
                sbSql.Append("[monlimit],[is_LackCard],[Urgency_Flg],[IntoStore_Status],[IntoStore_Date],");
                sbSql.Append("[OutStore_Status],[OutStore_Date],[SelfPick_Type],[SelfPick_Date] ");
                sbSql.Append("FROM [tbl_Card_BaseInfo] ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append(" WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append("order by Maildate desc");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

    }
}
