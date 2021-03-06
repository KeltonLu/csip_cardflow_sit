//******************************************************************
//*  作    者：chaoma(Wilson)
//*  功能說明：屬性標識
//*  創建日期：2009/07/20
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* linwei(Terry)      2010-05-17        T-DW-201005-002         參數增加停用欄位  
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;

namespace BusinessRules
{
    public class BRM_PROPERTY_KEY : BRBase<EntityM_PROPERTY_KEY>
    {
        #region sql語句
        public const string SEL_FKEY_BY_FKEYandFNAME = @"SELECT PROPERTY_KEY , DESCRIPTION, OFF_FLAG OFF_FLAG FROM  M_PROPERTY_KEY WHERE   FUNCTION_KEY= @FUNCTION_KEY";

        /// <summary>
        /// 查詢公共屬性
        /// </summary>
        private const string SEL_MAILTYPE = @"SELECT PROPERTY_CODE,PROPERTY_NAME ,OFF_FLAG OFF_FLAG FROM [M_PROPERTY_CODE] WHERE 
                                                FUNCTION_KEY = @FUNCKEY AND PROPERTY_KEY = @PROPERTY_KEY
                                                ORDER BY SEQUENCE ";

        //REQ-T20090065 參數增加停用欄位 Added by weilin 2010/05/17 Start
        /// <summary>
        /// 僅僅查詢啟用的屬性
        /// </summary>
        private const string SEL_ENABLEPROPROPERTY = @"SELECT PROPERTY_CODE PROPERTY_CODE,PROPERTY_NAME PROPERTY_NAME FROM [M_PROPERTY_CODE] MC,[M_PROPERTY_KEY] MK 
                                               WHERE MC.FUNCTION_KEY=MK.FUNCTION_KEY 
                                               AND   MC.PROPERTY_KEY=MK.PROPERTY_KEY 
                                               AND   MC.FUNCTION_KEY = @FUNCKEY
                                               AND   MC.PROPERTY_KEY = @PROPERTY_KEY 
                                               AND   MC.OFF_FLAG=@OFF_FLAG 
                                               AND   MK.OFF_FLAG=@OFF_FLAG ORDER BY SEQUENCE";
        //REQ-T20090065 參數增加停用欄位 Added by weilin 2010/05/17 End


        //REQ-T20090065 參數增加停用欄位 Added by weilin 2010/05/17 Start
        /// <summary>
        /// 查詢單個啟用的屬性的PROPERTY_NAME
        /// </summary>
        private const string SEL_ENABLEPROPROPERTYNAME = @"SELECT PROPERTY_CODE PROPERTY_CODE,PROPERTY_NAME PROPERTY_NAME FROM [M_PROPERTY_CODE] MC,[M_PROPERTY_KEY] MK 
                                               WHERE MC.FUNCTION_KEY=MK.FUNCTION_KEY 
                                               AND   MC.PROPERTY_KEY=MK.PROPERTY_KEY 
                                               AND   MC.FUNCTION_KEY = @FUNCKEY
                                               AND   MC.PROPERTY_KEY = @PROPERTY_KEY 
                                               AND   MC.PROPERTY_CODE = @PROPERTY_CODE
                                               AND   MC.OFF_FLAG=@OFF_FLAG 
                                               AND   MK.OFF_FLAG=@OFF_FLAG ORDER BY SEQUENCE";
        //REQ-T20090065 參數增加停用欄位 Added by weilin 2010/05/17 End


        #endregion



        /// <summary>
        /// 取得FunctionKey列表
        /// </summary>
        /// <param name="eProperty">屬性標識列表</param>
        /// <returns>DataTable</returns>
        public static DataTable GetPropertyKeyList(string strFunctionKey, int intPageIndex, int intPageSize, ref int intTotolCount, ref string strMsgID)
        {
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandText = SEL_FKEY_BY_FKEYandFNAME;

            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmProperty = new SqlParameter("@" + EntityM_PROPERTY_KEY.M_FUNCTION_KEY, strFunctionKey);

            sqlcmd.Parameters.Add(parmProperty);

            DataTable dtblProperty = null;
            try
            {
                dtblProperty = BRM_PROPERTY_KEY.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount).Tables[0];
            }
            catch (Exception exp)
            {
                strMsgID = "00_00000000_000";
                throw exp;
            }

            return dtblProperty;
        }


        /// <summary>
        /// 判断是否重復的屬性標識
        /// </summary>
        /// <param name="eProperty">要做判斷的屬性標識信息</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_PROPERTY_KEY eProperty)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityM_PROPERTY_KEY.M_PROPERTY, Operator.Equal, DataTypeUtils.String, eProperty.PROPERTY_KEY);
            sql.AddCondition(EntityM_PROPERTY_KEY.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eProperty.FUNCTION_KEY);

            if (BRM_PROPERTY_KEY.Search(sql.GetFilterCondition()).Count > 0)
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
        public static bool Add(EntityM_PROPERTY_KEY eProperty, ref string strMsgID)
        {
            if (!IsRepeat(eProperty))
            {

                if (BRM_PROPERTY_KEY.AddNewEntity(eProperty))
                {
                    strMsgID = "00_01040000_004";
                    return true;
                }
                else
                {
                    strMsgID = "00_01040000_005";
                    return false;
                }
            }
            strMsgID = "00_01040000_009";
            return false;
        }


        /// <summary>
        /// 修改屬性標識信息
        /// </summary>
        /// <param name="eProperty">屬性標識信息</param>
        /// <param name="strMsgID">消息ID</param>
        /// <returns></returns>
        public static bool Update(EntityM_PROPERTY_KEY eProperty, ref string strMsgID)
        {

           
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PROPERTY_KEY.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eProperty.FUNCTION_KEY);

            Sql.AddCondition(EntityM_PROPERTY_KEY.M_PROPERTY, Operator.Equal, DataTypeUtils.String, eProperty.PROPERTY_KEY);

            if (BRM_PROPERTY_KEY.UpdateEntity(eProperty, Sql.GetFilterCondition()))
            {
                strMsgID = "00_01040000_000";
                return true;
            }
            else
            {
                strMsgID = "00_01040000_001";
                return false;
            }
        
        }


        /// <summary>
        /// 刪除PropertyKey列表
        /// </summary>
        /// <param name="eProperty">屬性標識參數</param>
        /// <param name="strMsgID">訊息ID</param>
        /// <returns>DataTable</returns>
        public static bool Delete(EntityM_PROPERTY_KEY eProperty, ref string strMsgID)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PROPERTY_KEY.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, eProperty.FUNCTION_KEY);

            Sql.AddCondition(EntityM_PROPERTY_KEY.M_PROPERTY, Operator.Equal, DataTypeUtils.String, eProperty.PROPERTY_KEY);

            if(BRM_PROPERTY_KEY.DeleteEntityByCondition(eProperty, Sql.GetFilterCondition()))  
            {
                strMsgID = "00_01040000_002";
                return true;
            }
            else
            {
                strMsgID = "00_01040000_003";
                return false;
            }

        }

        /// <summary>
        /// 刪除屬性數據
        /// </summary>
        /// <param name="eProperty">屬性</param>
        /// <param name="ePropertyCode">屬性訊息</param>
        /// <param name="strMsgID">消息ID</param>
        /// <returns></returns>
        public static bool Delete(EntityM_PROPERTY_KEY eProperty, EntityM_PROPERTY_CODE ePropertyCode, ref string strMsgID)
        {
            strMsgID = "00_01040000_003";   //* 刪除不成功
            try
            {
                //* 事務處理。
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //* 先刪除該ROLE 底下所有的 FUNCTION功能
                    if (! BRM_PROPERTY_KEY.Delete(eProperty, ref strMsgID))
                    {
                        return false;
                    }

 
                    if (!BRM_PROPERTY_CODE.Delete(ePropertyCode, ref strMsgID))
                    {
                        return false;
                    }

                    //* 刪除事務提交
                    ts.Complete();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 通過子系統ID和屬性KEY取得某個屬性Key下所有的資料
        /// </summary>
        /// <param name="strFuncKey">子系統ID</param>
        /// <param name="strPropertyKey">屬性KEY</param>
        /// <param name="dtblResult">屬性列表</param>
        /// <returns>True - 成功    /   False - 失敗</returns>
        public static bool GetProperty(string strFuncKey, string strPropertyKey, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_MAILTYPE;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCKEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY", strPropertyKey));
            
            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception exp)
            {
                SaveLog(exp);
                return false;
            }
        }

        //REQ-T20090065 參數增加停用控制欄位 Added by weilin 2010/05/17 Start 
        /// <summary>
        /// 功能:通過子系統ID和屬性KEY取得某個屬性Key下所有處於啟用狀態的屬性資料
        /// 作者:Weilin(Terry)
        /// 創建時間:2010/05/17
        /// </summary>
        /// <param name="strFuncKey">子系統ID</param>
        /// <param name="strPropertyKey">屬性KEY</param>
        /// <param name="dtblResult">屬性列表</param>
        /// <returns>True - 成功    /   False - 失敗</returns>
        public static bool GetEnableProperty(string strFuncKey, string strPropertyKey, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_ENABLEPROPROPERTY;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCKEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY", strPropertyKey));
            // 此處暫時寫為固定值,具體參數是傳遞還是寫固定根據需要再改
            sqlcmd.Parameters.Add(new SqlParameter("@OFF_FLAG", "1"));

            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception exp)
            {
                SaveLog(exp);
                return false;
            }
        }
        //REQ-T20090065 參數增加停用控制欄位 Added by weilin 2010/05/17 End 

        //REQ-T20090065 參數增加停用控制欄位 Added by weilin 2010/05/17 Start 
        /// <summary>
        /// 功能:通過子系統ID和屬性KEY取得某個屬性Key下所有處於啟用狀態的屬性資料
        /// 作者:Weilin(Terry)
        /// 創建時間:2010/05/17
        /// </summary>
        /// <param name="strFuncKey">子系統ID</param>
        /// <param name="strPropertyKey">屬性KEY</param>
        /// <param name="strPropertyCode">屬性CODE</param>
        /// <returns>PropertyName - 成功    /   String.Empty - 失敗</returns>
        public static String GetEnablePropertyName(string strFuncKey, string strPropertyKey, string strPropertyCode)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_ENABLEPROPROPERTYNAME;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCKEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY", strPropertyKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_CODE", strPropertyCode));
            // 此處暫時寫為固定值,具體參數是傳遞還是寫固定根據需要再改
            sqlcmd.Parameters.Add(new SqlParameter("@OFF_FLAG", "1"));
            
            // 存放查詢的DataTable
            DataTable dtblResult = null;
            // 返回原查詢結果
            String strProperName = String.Empty;

            try
            {
                // 獲取資料集
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                
                // 判斷是否存在數據，如果存在就返回PROPERTY_NAME，否則返回String.Empty
                if (dtblResult.Rows.Count > 0)
                { 
                    // 返回查詢到的數據
                    strProperName = dtblResult.Rows[0]["PROPERTY_NAME"].ToString();                    
                }
                return strProperName;                
            }
            catch (Exception exp)
            {
                SaveLog(exp);
                return String.Empty ;
            }
            finally
            {
                dtblResult = null;   
            }
        }
        //REQ-T20090065 參數增加停用控制欄位 Added by weilin 2010/05/17 End 
    }
}
