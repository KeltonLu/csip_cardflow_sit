using System;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using BusinessRules;

/// <summary>
/// BR_FileInfo 的摘要描述
/// </summary>
public class BR_FileInfo : BRBase<Entity_FileInfo>
{
    public BR_FileInfo()
    {
          
    }
    /// <summary>
    /// 依據jobID更新檔案交換設定中的參數為空白
    /// </summary>
    /// <param name="jobID">要更新的jobID</param>
    public static bool UpdateParameter(string jobID)
    {
        string strSql = @"update dbo.tbl_FileInfo set ImportDate = '' where Job_ID = @jobID ";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        SqlParameter spjb = new SqlParameter("jobID", jobID);
        sqlComm.Parameters.Add(spjb);   
        return Update(sqlComm);
    }
    /// <summary>
    /// 檢查是否當日已匯入
    /// </summary>
    /// <param name="fname"></param>
    /// <param name="importDate"></param>
    /// <param name="CardNo"></param>
    /// <returns></returns>
    public static bool isExist(string fname, string importDate, string CardNo)
    {
        try
        {
            string sql = @"select  *  from  dbo.tbl_CancelOASA_Source ";
            sql += " where ImportFile='" + fname + "'and ImportDate='" + importDate + "' and CardNo='" + CardNo + "'";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            DataSet ds = BRM_CancelOASASource.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                DataTable dtOASACardInfo = ds.Tables[0];
                if (dtOASACardInfo.Rows.Count > 0)
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
            BRM_CancelOASASource.SaveLog(exp.Message);
            return false;
        }
    }

}