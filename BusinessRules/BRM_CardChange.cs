//******************************************************************
//*  功能說明：綜合資料處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：2021/01/21 陳永銘
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;

namespace BusinessRules
{
    public class BRM_CardChange : BRBase<Entity_CardChange>
    {



        /// <summary>
        /// 功能說明:更新一筆綜合資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Back"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(Entity_CardChange CardChange, string strCondition, ref string strMsgID, params string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CardChange.UpdateEntityByCondition(CardChange, strCondition, FiledSpit))
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
                BRM_CardChange.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:尚未退回轉出的年度換卡異動資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/19
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchCardChange(ref DataTable dtChangeCard)
        {
            try
            {   //2021/01/21 陳永銘 增加羅馬拼音
                string sql = @"SELECT   distinct T2.indate1,T2.ID,T2.custName,T2.custname_roma,T2.photo,T2.Action,T1.CardNo,T2.Kind,T2.mailno,
                                        T1.blockcode, T1.ImportDate, T1.ImportFileName,T1.Sno,T2.Merch_Code,T1.ImportDate,T1.trandate  
                                        FROM tbl_CardChange T1, tbl_Card_BaseInfo T2
                                        Where T2.Action='5' and T1.Trandate=T2.Trandate and T1.CardNo=T2.CardNo 
                                        and T1.OutputFlg ='N'and T2.is_LackCard = '1' and T2.CardNo is not null
                                        order by T1.ImportDate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtChangeCard = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:按流水號取BlockCode
        /// </summary>
        /// <param name="strSno">流水號</param>
        /// <param name="strBlockCode">返回值</param>
        /// <returns></returns>
        public static string GetBlockCode(string strSno)
        {
            try
            {
                string strSql = @"select isnull(BlockCode,'') From tbl_CardChange where sNo=@sNo";
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = strSql;
                sqlCmd.Parameters.Add(new SqlParameter("@sNo", strSno));
                DataSet ds = BRM_CardChange.SearchOnDataSet(sqlCmd);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return ds.Tables[0].Rows[0][0].ToString().Trim();
                    }
                }
                return "";
            }
            catch (System.Exception ex)
            {
                BRM_CardChange.SaveLog(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 功能說明:新增多筆資料，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsertFor0107(EntitySet<Entity_CardChange> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CardChange.BatInsert(Set))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_001";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_002";
                        return false;
                    }

                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

    }
}
