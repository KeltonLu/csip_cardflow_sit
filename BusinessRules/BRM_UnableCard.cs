//******************************************************************
//*  功能說明：無法制卡資料業務邏輯層
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
    public class BRM_UnableCard : BRBase<Entity_UnableCard>
    {


        /// <summary>
        /// 功能說明:更新一筆無法制卡資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Back"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(Entity_UnableCard UnableCard, string strCondition, ref string strMsgID, params string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_UnableCard.UpdateEntityByCondition(UnableCard, strCondition, FiledSpit))
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
                BRM_UnableCard.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:尚未退回轉出的無法制卡資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/19
        /// 修改記錄:
        /// 2021/01/21 陳永銘 增加羅馬拼音
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchUnableCard(ref DataTable dtUnableCard)
        {
            try
            {
                string sql = @"SELECT distinct T1.indate1, T2.id, T2.CustName,T2.photo,T1.Action,T1.CardNo, T2.Kind,
                                T2.maildate, T2.mailno, T1.blockcode, T1.ImportDate,T2.Merch_Code, T2.CustName_Roma,
                                T1.ImportFileName,T1.SNO,T1.ImportDate,T2.trandate 
                                FROM  tbl_UnableCard T1,tbl_Card_BaseInfo T2
                                Where T1.Action=T2.action and T1.indate1=T2.indate1 and T1.CardNo=T2.CardNo 
                                and T1.OutputFlg ='N' and T2.is_LackCard = '1' and T2.CardNo is not null 
                                order by T1.ImportDate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_UnableCard.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtUnableCard = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_UnableCard.SaveLog(exp.Message);
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
                string strSql = @"select isnull(BlockCode,'') From tbl_UnableCard where sNo=@sNo";
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = strSql;
                sqlCmd.Parameters.Add(new SqlParameter("@sNo", strSno));
                DataSet ds = BRM_UnableCard.SearchOnDataSet(sqlCmd);
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
                BRM_UnableCard.SaveLog(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 功能說明:新增多筆無法制卡資料，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="UnableCard"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsertFor0106(EntitySet<Entity_UnableCard> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_UnableCard.BatInsert(Set))
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
                BRM_UnableCard.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，製卡日判断是否有重復的卡片資料
        /// 作    者:CHEN HAO
        /// 創建時間:2010/08/17
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(Entity_CardBaseInfo TCardBaseInfo)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();

                Sql.AddCondition(Entity_UnableCard.M_Action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);
                Sql.AddCondition(Entity_UnableCard.M_CardNo, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                Sql.AddCondition(Entity_UnableCard.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);

                string strSql = @"SELECT COUNT(*) FROM [tbl_UnableCard]";
                strSql = strSql + " WHERE " + Sql.GetFilterCondition().Remove(0, 4);

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSql;
                DataSet ds = BRM_UnableCard.SearchOnDataSet(sqlcmd);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (null != ds.Tables[0].Rows[0][0])
                    {
                        if (Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString()) > 0)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return false;
            }
            catch (Exception exp)
            {
                BRM_Post.SaveLog(exp.Message);
                return false;
            }

        }
    }
}
