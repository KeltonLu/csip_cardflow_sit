//******************************************************************
//*  功能說明：每日卡片資料獲取
//*  作    者：xiongxiaofeng
//*  創建日期：2013/06/15
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Quartz;
using Quartz.Impl;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using System.Resources.Tools;
using System.Text;
using System.Text.RegularExpressions;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;

public class AutoCardBaseInfo : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCount;
    protected string sFileName;
    protected string strLocalPath;
    protected string strFolderName;
    protected string strLocalFilePath;
    protected string strFtpFilePath;
    protected string strFileName;
    protected string strFtpIp;
    protected string strFtpUserName;
    protected string strFtpPwd;        
          
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;

    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:xiongxiaofeng
    /// 創建時間:2013/06/15
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        try
        {
            #region 获取jobID
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString().Trim();
            JobHelper.strJobId = strJobId;
            //strJobId = "0121";
            #endregion

            #region 获取本地路徑
            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["UpLoadFilePath"] + "\\" + strJobId;
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            #endregion

            #region 計數器歸零
            SCount = 0;
            FCount = 0;
            #endregion

            #region 匯出資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");   //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");     //Ftp全路徑
            dtLocalFile.Columns.Add("FtpIp");           //FtpIP
            dtLocalFile.Columns.Add("FtpUserName");     //Ftp用戶名
            dtLocalFile.Columns.Add("FtpPwd");          //Ftp密碼
            dtLocalFile.Columns.Add("FileName");        //資料檔名
            dtLocalFile.Columns.Add("txtStates");       //产出txt档状态         
            dtLocalFile.Columns.Add("UploadStates");    //上传Ftp状态        
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId) == "" || JobHelper.SerchJobStatus(strJobId) == "0")
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
                return;
            }
            #endregion

            #region 檢測JOB是否在執行中
            if (BRM_LBatchLog.JobStatusChk(strFunctionKey, strJobId, DateTime.Now))
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不在執行           
                return;
            }
            else
            {
                BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, "R", "開始執行");
            }
            #endregion

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000115, "OU");

            #region 獲取卡片基本檔資料
            DataTable dtExportCardBaseInfo = new DataTable();
            String MailDate = StartTime.ToString("yyyy/MM/dd").ToString();
            BusinessRules.BRM_CardBaseInfo.SearchExportCardBaseInfo(ref dtExportCardBaseInfo, MailDate );
            //if (!(dtExportCardBaseInfo.Rows.Count > 0))
            //{
            //    JobHelper.SaveLog(Resources.JobResource.Job0113004.ToString());
            //    BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            //    BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "沒有卡片資料");
            //    return;
            //}
            #endregion

            #region 依據卡片基本資料檔Layout產出TXT檔
            ArrayList List = new ArrayList();//*記錄產生的文件明細

            string strFileName1 = "cardmaildate";     //*文件名稱        
            string strFileContent1 = string.Empty;  //*文件內容 

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))  //表格交換檔設定檔資料
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                for (int f = 0; f < dtFileInfo.Rows.Count; f++)
                //if (dtFileInfo.Rows.Count > 0)
               {
                  strFolderName = strJobId + StartTime.ToString("yyyyMMddHHmmss");

                  #region　新制卡郵寄檔
                   // if (rowFileInfo1.Length > 0)
                    strFtpIp = dtFileInfo.Rows[f]["FtpIP"].ToString();
                    strFtpUserName = dtFileInfo.Rows[f]["FtpUserName"].ToString();
                    strFtpPwd = dtFileInfo.Rows[f]["FtpPwd"].ToString();
                    strFtpFilePath = dtFileInfo.Rows[f]["FtpPath"].ToString();
                    strLocalFilePath = strLocalPath + "\\" + strFolderName;
                    if (dtExportCardBaseInfo.Rows.Count > 0)
                    {
                        foreach (DataRow rowExport in dtExportCardBaseInfo.Rows)
                        {
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["id"].ToString().Trim(), 11);//*身份證字號

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["cardno"].ToString().Trim(), 16);//*卡號1

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["CardType"].ToString().Trim(), 3);//*卡別
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["Photo"].ToString().Trim(), 2);//*相片卡別

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["Affinity"].ToString().Trim(), 4);//* 認同代碼

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["MailDate"].ToString().Trim(), 10);//*郵寄日期

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["Kind"].ToString().Trim(), 2);//*  取件方式

                            strFileContent1 += JobHelper.SetStrngValue("", 30);//*  預設空白
                            
                            strFileContent1 += "\r\n";


                        }
                        //max 修正收尾的多行
                        if (strFileContent1.Length > 2)
                        {
                            strFileContent1 = strFileContent1.ToString().Substring(0, strFileContent1.ToString().Length - 2);
                        }
                    }
                    FileTools.Create(strLocalFilePath + "\\" + strFileName1 + ".txt", strFileContent1);  //創建文件路徑

                    if (File.Exists(strLocalFilePath + "\\" + strFileName1 + ".txt"))
                    {
                        DataRow row = dtLocalFile.NewRow();
                        row["txtStates"] = "S";
                        row["LocalFilePath"] = strLocalFilePath + "\\";
                        row["FtpFilePath"] = strFtpFilePath + "//";
                        row["FtpIP"] = strFtpIp;
                        row["FtpUserName"] = strFtpUserName;
                        row["FtpPwd"] = strFtpPwd;
                        row["FileName"] = strFileName1;
                        dtLocalFile.Rows.Add(row);

                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000029, strFileName1), LogState.Info);
                    }
                    else
                    {
                        DataRow row = dtLocalFile.NewRow();
                        row["txtStates"] = "F";
                        row["FileName"] = strFileName1;
                        row["UploadStates"] = "F";
                        dtLocalFile.Rows.Add(row);

                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000030, strFileName1));
                    }
                  #endregion
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 登陸ftp上載文件
            DataRow[] rows = dtLocalFile.Select("txtStates='S'");
            for (int j = 0; j < rows.Length; j++)
            {
                FTPFactory objFtp = new FTPFactory(dtLocalFile.Rows[j]["FtpIp"].ToString().Trim(), ".", dtLocalFile.Rows[j]["FtpUserName"].ToString().Trim(), dtLocalFile.Rows[j]["FtpPwd"].ToString().Trim(), "21", @"C:\CS09", "Y");
                if (objFtp.Upload(dtLocalFile.Rows[j]["FtpFilePath"].ToString().Trim(), dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".txt", dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".txt"))
                {
                    //*更新上載狀態為S
                    dtLocalFile.Rows[j]["UploadStates"] = "S";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000033, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".txt"), LogState.Info);
                }
                else
                {
                    //*更新上載狀態為F
                    dtLocalFile.Rows[j]["UploadStates"] = "F";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000034, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".txt"));
                }
            }
            #endregion

            #region 刪除本地上載成功文件
            DataRow[] rowDels = dtLocalFile.Select("UploadStates='S'");
            for (int k = 0; k < rowDels.Length; k++)
            {
                FileTools.DeleteFile(rowDels[k]["LocalFilePath"].ToString().Trim() + rowDels[k]["FileName"].ToString().Trim() + ".txt");
            }
            #endregion

            #region 記錄job結束時間
            EndTime = DateTime.Now;
            #endregion

            #region job執行結果寫入日誌
            BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
            WriteLogToDB();
            #endregion
            JobHelper.SaveLog("JOB結束！", LogState.Info);
        }
        catch (Exception ex)
        {
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤_" + ex.ToString().Substring(0, 280));
            BRM_LBatchLog.SaveLog(ex);
        }
    }
    #endregion

    #region 記錄JOB成功或失敗資料至質料庫表
    /// <summary>
    /// 功能說明:記錄JOB成功和失敗資料至質料庫表
    /// 作    者:xiongxiaofeng
    /// 創建時間:2013/06/18
    /// 修改記錄:
    /// </summary>
    /// <param name="strJobId"></param>
    /// <returns></returns>
    public void WriteLogToDB()
    {
        DataRow[] RowS = dtLocalFile.Select("UploadStates='S'");
        DataRow[] RowF = dtLocalFile.Select("UploadStates='F'");
        if (RowS != null && RowS.Length > 0)
        {
            SCount = RowS.Length;
        }
        if (RowF != null && RowF.Length > 0)
        {
            FCount = RowF.Length;
        }
        //*判斷job完成狀態
        string strJobStatus = JobHelper.GetJobStatus(SCount, FCount);
        string strReturnMsg = string.Empty;
        strReturnMsg += Resources.JobResource.Job0000024 + SCount;
        strReturnMsg += Resources.JobResource.Job0000025 + FCount + "!";
        if (RowF != null && RowF.Length > 0)
        {
            string strTemps = string.Empty;
            for (int k = 0; k < RowF.Length; k++)
            {
                strTemps += RowF[k]["FileName"].ToString() + "  ";
            }
            strReturnMsg += Resources.JobResource.Job0000026 + strTemps;
        }
        JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);

    }
    #endregion

}
