//******************************************************************
//*  功能說明：郵局交寄資訊檔業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/06/04
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
    public class BRM_PostSend : BRBase<EntityM_PostSend>
    {

        /// <summary>
        /// 功能說明:新增一筆郵局交寄資訊檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(EntityM_PostSend PostSend, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSend.AddNewEntity(PostSend))
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
                BRM_PostSend.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆郵局交寄資訊檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(EntityM_PostSend PostSend, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSend.DeleteEntityByCondition(PostSend, strCondition))
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
                BRM_PostSend.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆郵局交寄資訊檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(EntityM_PostSend PostSend, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_PostSend.UpdateEntityByCondition(PostSend, strCondition, FiledSpit))
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
                BRM_PostSend.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }
       
        /// <summary>
        /// 功能說明:查詢郵局交寄資訊檔by cardno
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <returns></returns>
        public static bool SearchByMailNo(string strCondition, ref  DataTable dtPostSend, ref string strMsgID)
        {
            try
            {
                string sql = @"SELECT [maildate]
                                      ,[mailno]
                                      ,[Info1]
                                      ,[Info2]
                                      ,[Send_status_Code]
                                      ,[Send_status_Name]
                                      ,[M_date]
                                      ,[Post_Code]
                                      ,[Post_Name]
                                      ,[Post_TEL]
                                      ,[Post_ADDR]
                                      ,[M_Code]
                                      ,[M_Name]
                                      ,[Non_Send_Code]
                                      ,[Non_Send_Name]
                                      ,[Exp_Count]
                                      ,[Exp_Date]
                                      ,[Imp_date]
                                      ,[Imp_Time]
                                      ,[Imp_file]
                                      ,[Imp_code]
                                      ,[Imp_name]
                                  FROM [tbl_Post_Send]";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_PostSend.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtPostSend = ds.Tables[0];
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
                BRM_PostSend.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }


        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_PostSend PostSend)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_PostSend.M_Exp_Date, Operator.Equal, DataTypeUtils.String, PostSend.Exp_Date);

            Sql.AddCondition(EntityM_PostSend.M_Imp_name, Operator.Equal, DataTypeUtils.String, PostSend.Imp_name);

            if (BRM_PostSend.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }
     /// <summary>
        /// 功能說明:根據交寄日期郵件號碼處理日期時間判断是否有重復的郵局交寄資訊檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatFor0111(EntityM_PostSend PostSend)
        {
            string sql = @"SELECT count(maildate) FROM [dbo].[tbl_Post_Send] (nolock) ";
            
            SqlHelper SqlHp = new SqlHelper();
            SqlHp.AddCondition(EntityM_PostSend.M_maildate, Operator.Equal, DataTypeUtils.String, PostSend.maildate);
            SqlHp.AddCondition(EntityM_PostSend.M_mailno, Operator.Equal, DataTypeUtils.String, PostSend.mailno);

            string strCondition = SqlHp.GetFilterCondition();
            if (strCondition != "")
            {
                strCondition = strCondition.Remove(0, 4);
                sql += " where " + strCondition;
            }
            try
            {
                //SqlCommand sqlcmd = new SqlCommand();
                //sqlcmd.CommandType = CommandType.Text;
                //sqlcmd.CommandText = sql;
                DataSet ds = BRM_PostSend.SearchOnDataSet(sql);
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
                BRM_PostSend.SaveLog(exp.Message);
                return false;
            }
        }


        /// <summary>
        /// 功能說明:根據交寄日期郵件號碼處理日期時間判断是否有重復的郵局交寄資訊檔
        /// 作    者:Wallace Liu
        /// 創建時間:2013/10/22
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatFor0111_1(string strCondition)
        {
            string sql = @"SELECT count(maildate) FROM [dbo].[tbl_Post_Send] (nolock) ";

            if (strCondition != "")
            {
//                strCondition = strCondition.Remove(0, 4);
                sql += " where " + strCondition;
            }
            try
            {
                //SqlCommand sqlcmd = new SqlCommand();
                //sqlcmd.CommandType = CommandType.Text;
                //sqlcmd.CommandText = sql;
                DataSet ds = BRM_PostSend.SearchOnDataSet(sql);
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
                BRM_PostSend.SaveLog(exp.Message);
                return false;
            }
        }

    }

}


