using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
namespace BusinessRules
{
    public class BRM_CancelOASAFileName: BRBase<Entity_CancelOASA_FileName> 
    {
        /// <summary>
        /// 功能說明:獲取當日序號最大注銷檔名
        /// 作    者:linda
        /// 創建時間:2010/07/07
        /// 修改記錄:
        /// </summary>
        /// <returns strOASAFileName></returns>
        public static bool SearchOASAFileName(ref DataTable dtOASASourceFileInfo)
        {
            try
            {
                string sql = @"select top 1 CancelOASAFile from dbo.tbl_CancelOASA_FileName";
                       sql +=" where CancelOASADate=convert(nvarchar(10),getdate(),111)";
                       sql += " order by CancelOASAFile desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASAFileName.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtOASASourceFileInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASAFileName.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:寫入新增注銷檔名
        /// 作    者:linda
        /// 創建時間:2010/07/09
        /// 修改記錄:
        /// </summary>
        /// <param name="strOASAFileName"></param>
        /// <returns></returns>
        public static bool InsertOASAFileName(string strOASAFileName)
        {
            try
            {
                string sql = @"insert into dbo.tbl_CancelOASA_FileName(CancelOASADate,CancelOASAFile)";
                sql += " values(convert(nvarchar(10),getdate(),111),'" + strOASAFileName + "')";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                if (BRM_CancelOASAFileName.Add(sqlcmd))
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
                BRM_CancelOASAFileName.SaveLog(exp.Message);
                return false;
            }

        }
    }
}
