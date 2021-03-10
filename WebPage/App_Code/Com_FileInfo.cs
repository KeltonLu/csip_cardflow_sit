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
using Framework.Common.Utility;
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
                                  FROM [dbo].[tbl_FileInfo]
                                where Status ='U'and Job_ID ='" + strJobId + "'";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            DataSet ds = BRM_FileInfo.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                dtFileInfo = ds.Tables[0];


                // 專案代號:20200031-CSIP EOS 功能說明:FTP PWD加解密 - CSIP EOS 作者:Ares Luke 創建時間:2021/01/13
                foreach (DataRow rowFileInfo in dtFileInfo.Rows)
                {
                    if (rowFileInfo["FtpPwd"] != null)
                    {
                        rowFileInfo["FtpPwd"] = RedirectHelper.GetDecryptString(rowFileInfo["FtpPwd"].ToString());
                    }
                }
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
}
