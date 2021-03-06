//******************************************************************
//*  作    者：chaoma(Wilson)
//*  功能說明：屬性
//*  創建日期：2009/07/21
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Framework.Common.Message;
using EntityLayer;

namespace BusinessRules
{
    public class BRM_PROPERTY_CODE : BRBase<EntityM_PROPERTY_CODE>
    {
        #region sql語句

        #endregion


        /// <summary>
        /// 查詢公共屬性
        /// </summary>
        /// <param name="strPropertyKey"></param>
        /// <param name="dtblResult"></param>
        ///
        public static bool GetCommonProperty(string strPropertyKey, ref DataTable dtblResult)
        {
            return BRM_PROPERTY_KEY.GetEnableProperty("04", strPropertyKey, ref dtblResult); 
        }
        /// <summary>
        /// 刪除PropertyCode列表
        /// </summary>
        /// <param name="ePropertyCode">屬性參數</param>
        /// <param name="strMsgID">訊息ID</param>
        /// <returns>DataTable</returns>
        public static bool Delete(EntityM_PROPERTY_CODE ePropertyCode, ref string strMsgID)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.FUNCTION_KEY);

            Sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_KEY);
           
            if( BRM_PROPERTY_CODE.DeleteEntityByCondition(ePropertyCode, Sql.GetFilterCondition()))
            {
                strMsgID = "00_01040000_010";
                return true;
            }
            else
            {
                strMsgID = "00_01040000_011";
                return false;
            }

        }


        /// <summary>
        /// 判断是否重復的屬性代碼
        /// </summary>
        /// <param name="ePropertycode">要做判斷的屬性標識信息</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_PROPERTY_CODE ePropertyCode)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_KEY);
            sql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.FUNCTION_KEY);
            sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_CODE);
            if (BRM_PROPERTY_CODE.Search(sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsRepeat(EntityM_PROPERTY_CODE ePropertyCode, string strView)
        {

            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_KEY);
            sql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.FUNCTION_KEY);
            sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY, Operator.NotEqual, DataTypeUtils.String, strView);
            sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_CODE);
            if (BRM_PROPERTY_CODE.Search(sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 添加屬性標識信息
        /// </summary>
        /// <param name="eProperty">屬性標識信息</param>
        /// <param name="strMsgID">消息ID</param>
        /// <returns></returns>
        public static bool Add(EntityM_PROPERTY_CODE ePropertyCode, ref string strMsgID)
        {
            if (!IsRepeat(ePropertyCode))
            {

                if (BRM_PROPERTY_CODE.AddNewEntity(ePropertyCode))
                {
                    strMsgID = "00_01040000_015";
                    return true;
                }
                else
                {
                    strMsgID = "00_01040000_016";
                    return false;
                }
            }
            strMsgID = "00_01040000_020";
            return false;
        }

        /// <summary>
        /// 修改屬性代碼信息
        /// </summary>
        /// <param name="eProperty">屬性代碼信息</param>
        /// <param name="strMsgID">消息ID</param>
        /// <returns></returns>
        public static bool Update(EntityM_PROPERTY_CODE ePropertyCODE,string strView, ref string strMsgID)
        {

            if (!IsRepeat(ePropertyCODE, strView))
            {
                SqlHelper Sql = new SqlHelper();

                Sql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCODE.FUNCTION_KEY);

                Sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCODE.PROPERTY_KEY);

                Sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY, Operator.Equal, DataTypeUtils.String, strView);

                try
                {

                    BRM_PROPERTY_CODE.UpdateEntity(ePropertyCODE, Sql.GetFilterCondition());

                    strMsgID = "00_01040000_018";
                    return true;
                }
                catch
                {
                    strMsgID = "00_01040000_019";
                    return false;
                }
            }
            strMsgID = "00_01040000_019";
            return false;
        }

        /// <summary>
        /// 刪除PropertyCode列表
        /// </summary>
        /// <param name="ePropertyCode">屬性參數</param>
        /// <param name="strMsgID">訊息ID</param>
        /// <returns>DataTable</returns>
        public static bool DeleteCode(EntityM_PROPERTY_CODE ePropertyCode, ref string strMsgID)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.FUNCTION_KEY);

            Sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_KEY);

            Sql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY, Operator.Equal, DataTypeUtils.String, ePropertyCode.PROPERTY_CODE);

            if (BRM_PROPERTY_CODE.DeleteEntityByCondition(ePropertyCode, Sql.GetFilterCondition()))
            {
                strMsgID = "00_01040000_010";
                return true;
            }
            else
            {
                strMsgID = "00_01040000_011";
                return false;
            }

        }


        /// <summary>
        /// 功能說明:查詢郵局查單基本資料by cardno
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <returns></returns>
        public static bool SearchCode(string strCondition, ref  DataTable dtCode, ref string strMsgID)
        {
            try
            {
                string sql = @"SELECT FUNCTION_KEY
                                      ,PROPERTY_KEY
                                      ,PROPERTY_CODE
                                      ,PROPERTY_NAME
                                      ,SEQUENCE
                                      ,CHANGED_USER
                                      ,CHANGED_TIME
                                      ,OFF_FLAG
                                  FROM M_PROPERTY_CODE";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                sql += "ORDER BY SEQUENCE";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd, "Connection_CSIP");
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
                BRM_PROPERTY_CODE.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

    }

       
        
}
