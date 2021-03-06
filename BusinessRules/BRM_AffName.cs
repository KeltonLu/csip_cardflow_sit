//******************************************************************
//*  功能說明：認同代碼檔處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/06/07
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
    public class BRM_AffName : BRBase<Entity_AffName>
    {

        /// <summary>
        /// 功能說明:新增一筆認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_AffName AffName, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_AffName.AddNewEntity(AffName))
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
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(Entity_AffName AffName, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_AffName.DeleteEntityByCondition(AffName, strCondition))
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
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(Entity_AffName AffName, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_AffName.UpdateEntityByCondition(AffName, strCondition, FiledSpit))
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
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtAffName"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchPost(string strCondition, ref  DataTable dtAffName, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
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
                DataSet ds = BRM_AffName.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtAffName = ds.Tables[0];
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
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }

        }

        /// <summary>
        /// 功能說明:查詢認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtAffName"></param>
        /// <returns></returns>
        public static bool SearchByNo(string strCondition, ref  DataTable dtAffName, ref string strMsgID)
        {
            try
            {
                string sql = @"SELECT AFFID, AFFName  FROM   tbl_AFFNAME ";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_AffName.SearchOnDataSet(sqlcmd);
                if ( null != ds && ds.Tables[0].Rows.Count >0)
                {
                    dtAffName = ds.Tables[0];
                    strMsgID = "06_01160000_000";
                    return true;
                }
                else
                {
                    strMsgID = "06_01160000_001";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

      
        /// <summary>
        /// 功能說明:查詢認同代碼檔
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtAffName"></param>
        /// <returns></returns>
        public static bool SearchPostByNo(string strCondition, ref  DataTable dtAffName, ref string strMsgID)
        {
            try
            {
                string sql = @"select p.Podate,p.Cardno,p.OutPutDate,
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
                DataSet ds = BRM_AffName.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtAffName = ds.Tables[0];
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
                BRM_AffName.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="Post">Post</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(Entity_AffName AffName)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_AffName.M_AFFID, Operator.Equal, DataTypeUtils.String, AffName.AFFID);

            string strCondition = Sql.GetFilterCondition().Remove(0, 4);

            string strSql = @"SELECT COUNT(AFFID) FROM [tbl_AFFNAME]";
            strSql += " WHERE " + strCondition;


            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;
            DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt16(ds.Tables[0].Rows[0][0]) > 0)
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
    }
}
