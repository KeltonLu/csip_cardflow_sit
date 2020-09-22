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
    public class BRM_CancelOASASource : BRBase<Entity_CancelOASA_Source> 
    {
        /// <summary>
        /// 功能說明:新增多筆，事務專用
        /// 作    者:Linda
        /// 創建時間:2010/07/06
        /// 修改記錄:
        /// </summary>
        /// <param name="TCancelOASASource"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<Entity_CancelOASA_Source> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CancelOASASource.BatInsert(Set))
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
                BRM_CancelOASASource.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:獲取注銷卡片資料
        /// 作    者:linda
        /// 創建時間:2010/07/07
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchOASASourceFileInfo(string strFtpFileName, string strCancelTime, ref  DataTable dtOASASourceFileInfo)
        {
            try
            {
                string sql = @"select distinct ImportFile,ImportDate from dbo.tbl_CancelOASA_Source"; 
                       sql += " where ImportDate<=";
                switch(strCancelTime)
                {
                    case "0":
                        sql += "convert(varchar(10),dateadd(dd,-1,getdate()),111)";
                        break;
                    case "1":
                        sql += "convert(varchar(10),dateadd(mm,-1,getdate()),111)";
                        break;
                    //不確定為什麼有0與1 ，直接增加2 抓取今天匯入的檔案來註銷
                    case "2":
                        sql += "convert(varchar(10),dateadd(dd,-0,getdate()),111)";
                        break;
                }
                       sql += " and left(ltrim(rtrim(ImportFile)),4)=@ImportFile";
                       sql += " and Stauts='0'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@ImportFile", strFtpFileName));
                DataSet ds = BRM_CancelOASASource.SearchOnDataSet(sqlcmd);
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
                BRM_CancelOASASource.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:獲取注銷卡片資料
        /// 作    者:linda
        /// 創建時間:2010/07/07
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchOASACardInfo(string strFtpFileName, string strCancelTime, ref  DataTable dtOASACardInfo)
        {
            try
            {
                string sql = @"select * from dbo.tbl_CancelOASA_Source";
                sql += " where ImportDate<=";
                switch (strCancelTime)
                {
                    case "0":
                        sql += "convert(varchar(10),dateadd(dd,-1,getdate()),111)";
                        break;
                    case "1":
                        sql += "convert(varchar(10),dateadd(mm,-1,getdate()),111)";
                        break;
                    //不確定為什麼有0與1 ，直接增加2 抓取今天匯入的檔案來註銷
                    case "2":
                        sql += "convert(varchar(10),dateadd(dd,-0,getdate()),111)";
                        break;

                }
                sql += " and left(ltrim(rtrim(ImportFile)),4)=@ImportFile";
                sql += " and Stauts='0'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@ImportFile", strFtpFileName));
                DataSet ds = BRM_CancelOASASource.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtOASACardInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASASource.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:更新卡片注銷狀態
        /// 作    者:linda
        /// 創建時間:2010/07/07
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool UpdateStauts(string strImportFile, string strImportDate)
        {
            try
            {

                string sql = @"update dbo.tbl_CancelOASA_Source";
                sql += " set Stauts='1'";
                sql += " where ImportFile=@ImportFile and ImportDate=@ImportDate";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@ImportFile", strImportFile));
                sqlcmd.Parameters.Add(new SqlParameter("@ImportDate", strImportDate));
                if (BRM_CancelOASASource.Update(sqlcmd))
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
                BRM_CancelOASASource.SaveLog(exp.Message);
                return false;
            }

        }


    }
}
