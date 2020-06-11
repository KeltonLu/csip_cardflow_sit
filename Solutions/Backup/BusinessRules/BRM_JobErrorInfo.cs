//******************************************************************
//*  功能說明：Job錯誤資料檔
//*  作    者：HAO CHEN 
//*  創建日期：2010/06/08
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
    public class BRM_JobErrorInfo: BRBase<Entity_JobErrorInfo>
    {
        /// <summary>
        /// 功能說明:新增一筆錯誤資料
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="JobErrorInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_JobErrorInfo jobErrs, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_JobErrorInfo.AddNewEntity(jobErrs))
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
                BRM_JobErrorInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆Job錯誤資料
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(Entity_JobErrorInfo jobErrs, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_JobErrorInfo.DeleteEntityByCondition(jobErrs, strCondition))
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
                BRM_JobErrorInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆Job錯誤資料
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(Entity_JobErrorInfo jobErrs, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_JobErrorInfo.UpdateEntityByCondition(jobErrs, strCondition, FiledSpit))
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
                BRM_JobErrorInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }
    }
}
