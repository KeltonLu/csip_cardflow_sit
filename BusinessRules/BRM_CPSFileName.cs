//******************************************************************
//*  功能說明：郵局交寄匯入檔業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/06/09
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
    public class BRM_CPSFileName : BRBase<Entity_CPSFileName>
    {

        /// <summary>
        /// 功能說明:新增一筆郵局交寄匯入檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_CPSFileName CPSFileName, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CPSFileName.AddNewEntity(CPSFileName))
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
                BRM_CPSFileName.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆郵局交寄匯入檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(Entity_CPSFileName CPSFileName, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CPSFileName.DeleteEntityByCondition(CPSFileName, strCondition))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_005";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_006";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_CPSFileName.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆郵局交寄匯入檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(Entity_CPSFileName CPSFileName, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CPSFileName.UpdateEntityByCondition(CPSFileName, strCondition, FiledSpit))
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
                BRM_CPSFileName.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(Entity_CPSFileName CPSFileName)
        {
            string sql = @"SELECT count(ImportFileName) FROM [dbo].[tbl_CPS_FileName] ";

            SqlHelper SqlHp = new SqlHelper();
            SqlHp.AddCondition(Entity_CPSFileName.M_ImportFileName, Operator.Equal, DataTypeUtils.String, CPSFileName.ImportFileName);
            string strCondition = SqlHp.GetFilterCondition();
            if (strCondition != "")
            {
                strCondition = strCondition.Remove(0, 4);
                sql += " where " + strCondition;
            }
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CPSFileName.SearchOnDataSet(sqlcmd);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    int iCount = Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString());
                    if (iCount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CPSFileName.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
