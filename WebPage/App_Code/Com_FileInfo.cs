//******************************************************************
//*  功能說明：由ACQ>BusinessRules Copy BRM_FileInfo.cs做修改
//*  作    者：Tank
//*  創建日期：2016/06/20
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//* Annie           2018/09/14     新增UpdateParameter方法  
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Message;
using Framework.Common.Logging;
// using CSIPACQ.EntityLayer;
// using CSIPACQ.BusinessRules;
using System.Data.SqlClient;
using System.Data;
using BusinessRules;
using Framework.Data;

/// <summary>
/// Com_FileInfo 的摘要描述
/// </summary>
public class Com_FileInfo
{
    /// <summary>
    /// 功能說明:根據JOB ID和FUNCTION_KEY查詢Job狀態
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/26
    /// 修改記錄:
    /// </summary>
    /// <param name="dtFileInfo"></param>
    /// <param name="strJobId"></param>
    /// <returns></returns>
    public static bool selectFileInfo(ref  DataTable dtFileInfo, string strJobId)
    {
        try
        {
            string sql = @"SELECT [Job_ID]
                                      ,[FtpFileName]
                                      ,[FtpPath]
                                      ,[ZipPwd]
                                      ,[FtpIP]
                                      ,[FtpUserName]
                                      ,[FtpPwd]
                                      ,[Status]
                                      ,[LoopMinutes]
                                      ,[Parameter]
                                      ,[ForceImp]
                                  FROM [dbo].[tbl_FileInfo]
                                where Status ='U'and Job_ID ='" + strJobId + "'";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            DataSet ds = BRM_FileInfo.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                dtFileInfo = ds.Tables[0];
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception exp)
        {
            BRM_FileInfo.SaveLog(exp.Message);
            return false;
        }

    }

    /// <summary>
    /// 功能說明:依據jobID更新檔案交換設定中的參數為空白
    /// 作    者:Annie Chou
    /// 創建時間:2018/09/14
    /// 修改記錄:
    /// </summary>
    /// <param name="jobID">要更新的jobID</param>
    public static bool UpdateParameter(string jobID)
    {
        DataHelper dh = new DataHelper("Connection_System");

        string strSql = @"update dbo.tbl_FileInfo set Parameter = '',ForceImp = ''  where Job_ID = @jobID ";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        SqlParameter spjb = new SqlParameter("jobID", jobID);
        sqlComm.Parameters.Add(spjb);

        try
        {
            dh.ExecuteNonQuery(sqlComm);
            return true;
        }
        catch (Exception exp)
        {
            BRM_FileInfo.SaveLog(exp.Message);
            return false;
        }

    }

}
