//******************************************************************
//*  功能說明：OASA注銷Log檔處理業務邏輯層
//*  作    者：zhiyuan
//*  創建日期：2010/05/27
//*  修改記錄：
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
    public class BRM_CancelOASA : BRBase<Entity_CancelOASA>
    {

        /// <summary>
        /// 功能說明:新增多筆郵局查單，事務專用
        /// 作    者:zhiyuan
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<Entity_CancelOASA> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CancelOASA.BatInsert(Set))
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

        /// <summary>
        /// 功能說明:返回未注銷的資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtEndCaseFlg"></param>
        /// <returns></returns>
        public static bool SearchNotCancelData(ref  DataTable dtNotCancelData)
        {
            try
            {
                string NowDate = DateTime.Now.ToString("yyyy/MM/dd");
                string sql = "select * from  tbl_CancelOASA where ImportDate <= '" + NowDate + "' and Stauts='0'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASA.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtNotCancelData = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
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
        public static bool GetCancelOASAInfo(ref  DataTable dtCancelOASAInfo,int iPageIndex,int iPageSize,ref int iTotalCount, string strFromDate, string strToDate)
        {
            if (strFromDate.Equals(string.Empty))
            {
                strFromDate = "1911/01/01";
            }
            if (strToDate.Equals(string.Empty))
            {
                strToDate = "9999/12/31";
            }
            try
            {
                string sql = @" select CancelOASADate,";
                sql += " CancelOASASource,case CancelOASASource when '0' then 'OU註銷' when '1' then '人工註銷' when '4' then '寄出前註銷' else '退件註銷' end as CancelOASASourceName,";
                sql += " CancelOASAFile,TotalCount,SCount,FCount,ConfirmUser,ModStautsUser,ModStautsDate,ChangeNote,";
                sql += " isnull(Stauts,'0') as Stauts,case isnull(Stauts,'0') when '0' then '記錄未確認' when '1' then '記錄確認' when '2' then '覆核' when '3' then '取消覆核' when '4' then '放行' when '5' then '取消放行'end as StautsName";
                sql += " from dbo.tbl_CancelOASA";
                sql += " where CancelOASADate between '"+strFromDate+"' and '"+strToDate+"'";
                sql += " order by CancelOASADate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASA.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);               
                if (ds != null)
                {
                    dtCancelOASAInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASA.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新注銷檔狀態
        /// 作    者:linda
        /// 創建時間:2010/07/22
        /// 修改記錄:
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="strFile"></param>
        /// <param name="strLogUserId"></param>
        /// <param name="strScount"></param>
        /// <param name="strFcount"></param>
        /// <param name="strNote"></param>
        /// <returns></returns>
        public static bool UpdateFor020902(string strDate, string strFile, string strLogUserId, string strScount, string strFcount, string strNote)
        {
            try
            {
                string sql = @"update dbo.tbl_CancelOASA ";
                sql += " set Stauts='1',ConfirmUser='" + strLogUserId + "',ModStautsUser='" + strLogUserId + "',ModStautsDate=convert(varchar(10),getdate(),111),SCount=SCount+" + strScount + "-" + strFcount+",FCount=FCount+" + strFcount + "-" + strScount+",";
                sql += " ChangeNote=isnull(ChangeNote,'')+'" + strNote + "'";
                sql += " where CancelOASADate ='" + strDate + "'and CancelOASAFile='" + strFile + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CancelOASA.Update(sqlcmd))
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
                BRM_CancelOASA.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新注銷檔狀態
        /// 作    者:linda
        /// 創建時間:2010/07/23
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <param name="strDate"></param>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public static bool BatStautsUpdate(DataTable dtUpdateStauts,string strUserId)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtUpdateStauts.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CancelOASA CancelOASA = new Entity_CancelOASA();

                    CancelOASA.CancelOASADate = dtUpdateStauts.Rows[i]["CancelOASADate"].ToString();
                    CancelOASA.CancelOASAFile = dtUpdateStauts.Rows[i]["CancelOASAFile"].ToString();
                    CancelOASA.Stauts = dtUpdateStauts.Rows[i]["Stauts"].ToString();
                    CancelOASA.ChangeNote = dtUpdateStauts.Rows[i]["ChangeNote"].ToString();
                    CancelOASA.ModStautsDate= DateTime.Now.ToString("yyyy/MM/dd");
                    CancelOASA.ModStautsUser= strUserId;


                    sqlhelp.AddCondition(Entity_CancelOASA.M_CancelOASADate, Operator.Equal, DataTypeUtils.String, CancelOASA.CancelOASADate);
                    sqlhelp.AddCondition(Entity_CancelOASA.M_CancelOASAFile, Operator.Equal, DataTypeUtils.String, CancelOASA.CancelOASAFile);

                    if (!Update(CancelOASA, sqlhelp.GetFilterCondition(), "Stauts", "ChangeNote", "ModStautsDate", "ModStautsUser"))//*更新條件設置
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
        public static bool Update(Entity_CancelOASA CancelOASA, string strCondition, params  string[] FiledSpit)
        {
            try
            {
                if (BRM_CancelOASA.UpdateEntityByCondition(CancelOASA, strCondition, FiledSpit))
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
                BRM_CancelOASA.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
