//******************************************************************
//*  功能說明：用戶檔處理業務邏輯層
//*  作    者：HAO CHEN
//*  創建日期：2010/06/25
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
    public class BRM_User:BRBase<CSIPCommonModel.EntityLayer.EntityM_USER>
    {
        /// <summary>
        /// 功能說明:根據用戶ID 取得用戶姓名
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/25
        /// 修改記錄:
        /// </summary>
        /// <param name="strUserId">用戶ID</param>
        /// <param name="strUserName">傳出用戶姓名</param>
        /// <returns>bool</returns>
        public static bool GetUserName(string strUserId,ref string strUserName)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT Distinct USER_NAME ");
                sbSql.Append("FROM M_USER ");
                sbSql.Append(" WHERE USER_ID = '{0}'");

                string strSql = string.Format(sbSql.ToString(), strUserId);
                object UserName = BRM_User.SearchAValue(strSql, "Connection_CSIP");

                if (UserName != null)
                {
                    strUserName = UserName.ToString();
                    return true;
                }
                else
                {
                    strUserName = string.Empty;
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_User.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
