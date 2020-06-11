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
    public class BRM_CancelOASADetail : BRBase<Entity_CancelOASA_Detail> 
    {
        /// <summary>
        /// 功能說明:新增多筆
        /// 作    者:Linda
        /// 創建時間:2010/07/06
        /// 修改記錄:
        /// </summary>
        /// <param name="TCancelOASASource"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<Entity_CancelOASA_Detail> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CancelOASADetail.BatInsert(Set))
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
                BRM_CancelOASADetail.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:返回注銷的資料
        /// 作    者:linda
        /// 創建時間:2010/07/19
        /// 修改記錄:
        /// </summary>
        /// <param name="dtEndCaseFlg"></param>
        /// <returns></returns>
        public static bool GetDetailInfo(ref  DataTable dtDetailInfo, int iPageIndex, int iPageSize, ref int iTotalCount, string strDate, string strFile, string strSFFlg)
        {

            try
            {
                string sql = @" select CardNo,BlockLog,MemoLog,case SFFlg when '0' then '未註銷' when '1' then '註銷成功' when '2' then '註銷失敗' when '3' then '人工註銷成功' end as SFFlgName,SFFlg";
                sql += " from dbo.tbl_CancelOASA_Detail";
                sql += " where CancelOASAFile='" + strFile + "' and CancelOASADate='" + strDate + "'";
                if (strSFFlg != "4")
                {
                    sql += " and SFFlg='" + strSFFlg + "'";
                }


                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASADetail.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtDetailInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASADetail.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:更新注銷狀態
        /// 作    者:linda
        /// 創建時間:2010/07/22
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <param name="strDate"></param>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public static bool BatDetailUpdate(DataTable dtUpdateDetail, string strDate, string strFile, string strUserId)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtUpdateDetail.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CancelOASA_Detail CancelOASADetail = new Entity_CancelOASA_Detail();

                    CancelOASADetail.CancelOASADate = strDate;
                    CancelOASADetail.CancelOASAFile = strFile;
                    CancelOASADetail.CardNo = dtUpdateDetail.Rows[i]["CardNo"].ToString();
                    CancelOASADetail.SFFlg = dtUpdateDetail.Rows[i]["SFFlg"].ToString();
                    CancelOASADetail.ModSFFlgDate = DateTime.Now.ToString("yyyy/MM/dd");
                    CancelOASADetail.ModSFFlgUser = strUserId;


                    sqlhelp.AddCondition(Entity_CancelOASA_Detail.M_CancelOASADate, Operator.Equal, DataTypeUtils.String, CancelOASADetail.CancelOASADate);
                    sqlhelp.AddCondition(Entity_CancelOASA_Detail.M_CancelOASAFile, Operator.Equal, DataTypeUtils.String, CancelOASADetail.CancelOASAFile);
                    sqlhelp.AddCondition(Entity_CancelOASA_Detail.M_CardNo, Operator.Equal, DataTypeUtils.String, CancelOASADetail.CardNo);

                    if (!Update(CancelOASADetail, sqlhelp.GetFilterCondition(), "SFFlg", "ModSFFlgDate", "ModSFFlgUser"))
                    {
                        blnResult = false;
                        break;
                    }
                }
                if (blnResult)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASADetail.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆卡片基本資料，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="CancelOASADetail"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool Update(Entity_CancelOASA_Detail CancelOASADetail, string strCondition, params  string[] FiledSpit)
        {
            try
            {
                if (BRM_CancelOASADetail.UpdateEntityByCondition(CancelOASADetail, strCondition, FiledSpit))
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
                BRM_CancelOASADetail.SaveLog(exp.Message);
                return false;
            }
        }

    }
}
