//******************************************************************
//*  功能說明：郵局查單申請處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using Framework.Common.Utility;

namespace BusinessRules
{
    public class BRM_Post : BRBase<Entity_Post>
    {

        /// <summary>
        /// 功能說明:新增一筆郵局查單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_Post Post, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_Post.AddNewEntity(Post))
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:取得UserRoleId資料
        /// 作    者:HAO CHEN
        /// 創建時間:2010/08/01
        /// 修改記錄:
        /// </summary>
        /// <param name="eAutoJob">AUTOJOB</param>
        /// <returns>DataTable</returns>
        public static string SearchUserRoleId(string strUserID)
        {
            SqlConnection sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_CSIP"));
            
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlConn;
            sqlcmd.CommandText = "SELECT ROLE_ID FROM M_USER WHERE (USER_ID = @UserID)";
            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmKey = new SqlParameter("@UserID", strUserID);
            sqlcmd.Parameters.Add(parmKey);
            sqlConn.Open();
            object objUserRoleID = sqlcmd.ExecuteScalar();
            sqlConn.Close();
            sqlConn.Dispose();
            if (null == objUserRoleID)
            {
                
                return string.Empty;
            }
            else
            {
                
                return objUserRoleID.ToString();
            }
            
        }

        /// <summary>
        /// 功能說明:獲取當前用戶角色 看是否有權限刪除和更新
        /// 作    者:Linda
        /// 創建時間:2010/09/01
        /// 修改記錄:
        /// </summary>
        /// <param name="strUserID">strUserID</param>
        /// <param name="strUserID">strRoleIdList</param>
        /// <returns>DataTable</returns>
        public static bool CheckUserRoleId(string strUserID, string strRoleIdList, ref DataTable dtUserRoleId)
        {
            try
            {
                string strSql = "SELECT ROLE_ID FROM M_USER WHERE USER_ID =@USER_ID AND ROLE_ID IN (" + strRoleIdList + ")";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = strSql;
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", strUserID));
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd,"Connection_CSIP");
                if (ds != null)
                {
                    dtUserRoleId = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_Post.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:刪除一筆郵局查單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(Entity_Post Post, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_Post.DeleteEntityByCondition(Post, strCondition))
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆郵局查單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(Entity_Post Post, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_Post.UpdateEntityByCondition(Post, strCondition, FiledSpit))
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢郵局查單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchPost(string strCondition, ref  DataTable dtPost, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                string sql = @"select p.sno, p.Podate,p.Cardno,p.OutPutDate,
                                 case   p.EndCaseFlg
                                 when 'Y' then '結案' 
                                 else  '未結案' 
                                 end  EndCaseFlgs ,b.Action,b.Mailno,b.custname,b.add1,b.Maildate,b.id
                                 from  tbl_Post p left join tbl_Card_BaseInfo b on p.cardno=b.cardno";
              
                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }
                sql += " order by p.Podate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtPost = ds.Tables[0];
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
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
        public static bool SearchPostByNo(string strCondition, ref  DataTable dtPost, ref string strMsgID)
        {
            try
            {
                string sql = @"select p.Podate,p.Cardno,p.OutPutDate,Uid,
                                 case   p.EndCaseFlg
                                 when 'Y' then '結案' 
                                 else  '未結案' 
                                 end  EndCaseFlgs ,p.Backdate,p.EndCaseFlg,p.Note,p.Stateflg,p.Sno,b.Action,b.Mailno,b.id
                                 from  tbl_Post p left join tbl_Card_BaseInfo b on p.cardno=b.cardno";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtPost = ds.Tables[0];
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢片語
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtPhrase"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchPhrase(ref  DataTable dtPhrase,string strCardNo, ref string strMsgID)
        {
            try
            {
                string sql = @"";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parmKey = new SqlParameter("@" + Entity_CardBaseInfo.M_cardno, strCardNo);
                sqlcmd.Parameters.Add(parmKey);
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtPhrase = ds.Tables[0];
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(Entity_Post entityPost)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_Post.M_Cardno, Operator.Equal, DataTypeUtils.String, entityPost.Cardno);

            Sql.AddCondition(Entity_Post.M_Uid, Operator.Equal, DataTypeUtils.String, entityPost.Uid);

            if (BRM_Post.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
