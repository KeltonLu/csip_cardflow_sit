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
    public class BRM_CancelOASAUd : BRBase<Entity_CancelOASAUd>
    {
        /// <summary>
        /// 功能說明:新增多筆，事務專用
        /// 作    者:Linda
        /// 創建時間:2010/07/13
        /// 修改記錄:
        /// </summary>
        /// <param name="EntitySetCancelOASAUd"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<Entity_CancelOASAUd> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CancelOASAUd.BatInsert(Set))
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
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:判斷管制解管/監控補掛注銷卡片資料是否已上主機
        /// 作    者:Carolyn
        /// success_flag = 0 : 表示未上主機
        /// 回傳 : true : 該批檔案已上主機(該批資料,只要有一筆曾上送過主機,則視為整批皆已上送)
        ///        false : 該批檔案未上主機
        /// 修改記錄:
        public static bool IS_OASAUDCardInfo_UpToMF(string strFileName, string strFileDate)
        {
            try
            {
                string strCount = "";
                string sql = @"select count(*) from dbo.tbl_CancelOASA_UD";
                sql += " where FileName_Real='"+strFileName+"' and File_Date='"+strFileDate+"'";
                sql += " and success_flag <> '0'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASAUd.SearchOnDataSet(sqlcmd);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    strCount = ds.Tables[0].Rows[0][0].ToString();
                    if (int.Parse(strCount) > 0) //有上送過主機
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
                    return true;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return true;
            }

        }

        /// <summary>
        /// 功能說明:獲取管制解管/監控補掛注銷卡片資料
        /// 作    者:linda
        /// 創建時間:2010/07/14
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchOASAUDCardInfo(string strFileName, string strFileDate, ref DataTable dtOASAUDCardInfo)
        {
            try
            {
                string sql = @"select * from dbo.tbl_CancelOASA_UD";
                sql += " where FileName_Real='" + strFileName + "' and File_Date='" + strFileDate + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASAUd.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtOASAUDCardInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:更新一筆OASAUD資料
        /// 作    者:linda
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool Update(Entity_CancelOASAUd CardBackInfo, string strCondition, params  string[] FiledSpit)
        {
            try
            {

                if (BRM_CancelOASAUd.UpdateEntityByCondition(CardBackInfo, strCondition, FiledSpit))
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
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
