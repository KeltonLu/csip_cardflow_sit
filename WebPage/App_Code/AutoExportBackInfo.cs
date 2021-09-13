//******************************************************************
//*  功能說明：自動化退件處理通知
//*  作    者：linda
//*  創建日期：2010/06/04
//*  修改記錄：2021/01/20 陳永銘
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20161108 (U) by Tank, 調整取CardType中文方式

using BusinessRules;
using CSIPCommonModel.EntityLayer;
using EntityLayer;
using Framework.Common.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
//20161108 (U) by Tank
using Framework.Data;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Quartz;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class AutoExportBackInfo : Quartz.IJob
{
    #region job基本參數設置
    protected string strJobId;
    protected string strFunctionKey = "06";
    private string strSessionId = "";
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
    protected string strZipPwd;

    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DataTable dtCardBackInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;

    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:Linda
    /// 創建時間:2010/06/05
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
            //strJobId = "0113";

            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            #endregion

            #region 获取本地路徑
            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\" + strJobId;
            #endregion

            #region 記錄job啟動時間
            StartTime = DateTime.Now;
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
            dtLocalFile.Columns.Add("ZipPwd");          //压缩密码
            dtLocalFile.Columns.Add("txtStates");       //产出txt档状态
            dtLocalFile.Columns.Add("ZipStates");       //压缩狀態          
            dtLocalFile.Columns.Add("UploadStates");    //上传Ftp状态        
            #endregion

            #region 更新退件資料
            dtCardBackInfo = new DataTable();
            dtCardBackInfo.Columns.Add("SerialNo");   //序列號
            dtCardBackInfo.Columns.Add("BlockCode");     //BLKCode    
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000113, "OU");

            #region 獲取退件通知檔資料
            DataTable dtExportCardBackInfo = new DataTable();
            BRM_CardBackInfo.SearchExportCardBackInfo(ref dtExportCardBackInfo);
            if (!(dtExportCardBackInfo.Rows.Count > 0))
            {
                JobHelper.SaveLog(Resources.JobResource.Job0113001.ToString(), LogState.Info);
                BRM_LBatchLog.Delete(strFunctionKey, strJobId, StartTime, "R");
                BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "沒有要轉出的退件資料");
                return;
            }
            #endregion

            #region 加載属性名
            MergeTable(ref dtExportCardBackInfo);
            #endregion

            #region 依據退件通知檔Layout產出TXT檔
            ArrayList List = new ArrayList();//*記錄產生的文件明細

            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string strFileName1 = string.Empty;     //*文件名稱        
            string strFileContent1 = string.Empty;  //*文件內容 
            string strFileName2 = string.Empty;     //*文件名稱        
            string strFileContent2 = string.Empty;  //*文件內容 
            string strFileName3 = string.Empty;     //*文件名稱        
            string strFileContent3 = string.Empty;  //*文件內容

            dtFileInfo = new DataTable();
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    strFolderName = strJobId + StartTime.ToString("yyyyMMddHHmmss");

                    #region　退件連絡報表
                    DataRow[] rowFileInfo1 = dtFileInfo.Select("FunctionFlg='1'");//*退件連絡報表
                    if (rowFileInfo1.Length > 0)
                    {

                        strFtpIp = rowFileInfo1[0]["FtpIP"].ToString();
                        strFtpUserName = rowFileInfo1[0]["FtpUserName"].ToString();
                        strFtpPwd = rowFileInfo1[0]["FtpPwd"].ToString();
                        strFtpFilePath = rowFileInfo1[0]["FtpPath"].ToString();
                        strLocalFilePath = strLocalPath + "\\" + strFolderName;
                        strFileName1 = strDate + rowFileInfo1[0]["FtpFileName"].ToString();
                        strZipPwd = RedirectHelper.GetDecryptString(rowFileInfo1[0]["ZipPwd"].ToString());
                        //string serial_no_temp="";
                        // string serial_no_Newtemp="";
                        foreach (DataRow rowExport in dtExportCardBackInfo.Rows)
                        {
                            //serial_no_Newtemp=JobHelper.SetStrngValue(rowExport["serial_no"].ToString().Trim(), 12);
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["serial_no"].ToString().Trim(), 12);//*流水號
                            //* update in 2010/09/29 by SIT&UAT bug list.xls(149)
                            //strFileContent1 +=  JobHelper.SetStrngValue(rowExport["NewName"].ToString().Trim(),10);//*姓名
                            if (rowExport["NewName"].ToString().Trim() != "")
                            {
                                String str = rowExport["NewName"].ToString().Trim();
                                str = (str.Length > 0 ? str.Substring(1) : str);
                                strFileContent1 += JobHelper.SetStrngValue(str, 10);//*新姓名
                            }
                            else
                            {
                                String str = rowExport["CustName"].ToString().Trim();
                                str = (str.Length > 0 ? str.Substring(1) : str);
                                strFileContent1 += JobHelper.SetStrngValue(str, 10);//*姓名
                            }
                            //* end update

                            //20161108 (U) by Tank, 取CardType中文
                            //switch (rowExport["cardtype"].ToString().Trim())
                            //{
                            //    case "000":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113012.ToString(), 20);
                            //        break;
                            //    case "013":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113013.ToString(), 20);
                            //        break;
                            //    case "370":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113013.ToString(), 20);
                            //        break;
                            //    case "035":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113013.ToString(), 20);
                            //        break;
                            //    case "571":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113013.ToString(), 20);
                            //        break;
                            //    case "018":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    case "019":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    case "039":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    //201304 悠遊Debit 需求修改
                            //    case "040":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    case "038":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    case "037":
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113014.ToString(), 20);
                            //        break;
                            //    default:
                            //        strFileContent1 += JobHelper.SetStrngValue(Resources.JobResource.Job0113011.ToString(), 20);
                            //        break;
                            //}
                            strFileContent1 += JobHelper.SetStrngValue(GetCardTypeName(rowExport["cardtype"].ToString().Trim()), 20);

                            strFileContent1 += JobHelper.SetStrngValue(rowExport["ActionName"].ToString().Trim(), 12);//*卡別
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["CardNo"].ToString().Trim(), 19);//*卡號
                            //* update in 2010/09/01 by SIT&UAT bug list.xls(56)
                            //strFileContent1 += JobHelper.SetStrngValue(rowExport["Backdate"].ToString().Trim(), 10);//* 退件日期
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["Backdate"].ToString().Trim().Replace('/', '.'), 10);//* 退件日期
                            //* end update
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["ReasonName"].ToString().Trim(), 10);//*退件原因
                            //* update in 2010/09/01 by SIT&UAT bug list.xls(56)
                            //strFileContent1 += JobHelper.SetStrngValue(rowExport["Enddate"].ToString().Trim(),10);//*  處理日期
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["Enddate"].ToString().Trim().Replace('/', '.'), 10);//*  處理日期
                            //* end update
                            strFileContent1 += JobHelper.SetStrngValue(rowExport["EnditemName"].ToString().Trim(), 20);//*  處理方法
                            strFileContent1 += JobHelper.SetStrngValue("", 50);//*備註     
                            strFileContent1 += "\r\n";

                        }
                        //max 修正收尾的多行
                        if (strFileContent1.Length > 2)
                        {
                            strFileContent1 = strFileContent1.ToString().Substring(0, strFileContent1.ToString().Length - 2);
                        }

                        FileTools.Create(strLocalFilePath + "\\" + strFileName1 + ".txt", strFileContent1);

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
                            row["ZipPwd"] = strZipPwd;
                            dtLocalFile.Rows.Add(row);

                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000029, strFileName1), LogState.Info);

                            #region 更新CardBackStatus 寫入InformMerchDate
                            if (BRM_CardBackInfo.UpdateCardBackStatus(dtExportCardBackInfo) && BRM_CardBackInfo.UpdateInformMerchDate(dtExportCardBackInfo))
                            {
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000027, strJobId, StartTime.ToString("yyyyMMddHHmmss"), "CardBackInfo"), LogState.Info);
                            }
                            else
                            {
                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000028, strJobId, StartTime.ToString("yyyyMMddHHmmss"), "CardBackInfo"));
                            }
                            #endregion
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

                    }
                    #endregion

                    DataRow[] RowExportCardBackInfAdd = dtExportCardBackInfo.Select("Enditem in ('1','2','3','4')");

                    if (RowExportCardBackInfAdd.Length > 0)
                    {
                        #region　大宗掛號單
                        DataRow[] rowFileInfo2 = dtFileInfo.Select("FunctionFlg='2'");//*大宗掛號單
                        if (rowFileInfo2.Length > 0)
                        {
                            strFtpIp = rowFileInfo2[0]["FtpIP"].ToString();
                            strFtpUserName = rowFileInfo2[0]["FtpUserName"].ToString();
                            strFtpPwd = rowFileInfo2[0]["FtpPwd"].ToString();
                            strFtpFilePath = rowFileInfo2[0]["FtpPath"].ToString();
                            strLocalFilePath = strLocalPath + "\\" + strFolderName;
                            strFileName2 = strDate + rowFileInfo2[0]["FtpFileName"].ToString();
                            strZipPwd = RedirectHelper.GetDecryptString(rowFileInfo2[0]["ZipPwd"].ToString());

                            string strCount = string.Empty;
                            string strCountLen = "000";
                            for (int rowExportAdd1 = 0; rowExportAdd1 < RowExportCardBackInfAdd.Length; rowExportAdd1++)
                            {

                                strCount = strCountLen.Substring(0, (strCountLen.Length - (rowExportAdd1 + 1).ToString().Trim().Length)) + (rowExportAdd1 + 1).ToString().Trim();

                                strFileContent2 += strCount;//*序號
                                strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["EnditemName"].ToString().Trim(), 20);//*結案方式
                                strFileContent2 += JobHelper.SetStrngValue("", 50);//*分行別
                                //strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewName"].ToString().Trim().Substring(1, RowExportCardBackInfAdd[rowExportAdd1]["NewName"].ToString().Trim().Length - 1), 30);//*姓名
                                //strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewAdd1"].ToString().Trim() + RowExportCardBackInfAdd[rowExportAdd1]["NewZip"].ToString().Trim(), 18);//* 地址一
                                //strFileContent2 += JobHelper.SetStrngValue("", 128);//*地址二28+地址三100
                                if (RowExportCardBackInfAdd[rowExportAdd1]["NewName"].ToString().Trim() != "")
                                {
                                    strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewName"].ToString().Trim().Substring(1), 20);//*姓名
                                }
                                else
                                {
                                    strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewName"].ToString().Trim(), 20);//*姓名
                                }
                                strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewZip"].ToString().Trim(), 6);//* 郵遞區號
                                strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["NewAdd1"].ToString().Trim(), 40);//* 地址一
                                strFileContent2 += JobHelper.SetStrngValue("", 80);//*地址二40+地址三40
                                strFileContent2 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd1]["serial_no"].ToString().Trim(), 14);//*  備注 填入退件序號                       
                                strFileContent2 += "\r\n";
                            }
                            if (strFileContent2.Length > 2)
                            {
                                strFileContent2 = strFileContent2.ToString().Substring(0, strFileContent2.ToString().Length - 2);
                            }

                            FileTools.Create(strLocalFilePath + "\\" + strFileName2 + ".txt", strFileContent2);

                            if (File.Exists(strLocalFilePath + "\\" + strFileName2 + ".txt"))
                            {
                                DataRow row = dtLocalFile.NewRow();
                                row["txtStates"] = "S";
                                row["LocalFilePath"] = strLocalFilePath + "\\";
                                row["FtpFilePath"] = strFtpFilePath + "//";
                                row["FtpIP"] = strFtpIp;
                                row["FtpUserName"] = strFtpUserName;
                                row["FtpPwd"] = strFtpPwd;
                                row["FileName"] = strFileName2;
                                row["ZipPwd"] = strZipPwd;
                                dtLocalFile.Rows.Add(row);

                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000029, strFileName2), LogState.Info);
                            }
                            else
                            {
                                DataRow row = dtLocalFile.NewRow();
                                row["txtStates"] = "F";
                                row["FileName"] = strFileName2;
                                row["UploadStates"] = "F";
                                dtLocalFile.Rows.Add(row);

                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000030, strFileName2));
                            }
                        }
                        else
                        {
                            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000028));
                        }
                        #endregion

                        #region　大宗掛號地址條
                        DataRow[] rowFileInfo3 = dtFileInfo.Select("FunctionFlg='3'");//*大宗掛號單
                        if (rowFileInfo3.Length > 0)
                        {
                            strFtpIp = rowFileInfo3[0]["FtpIP"].ToString();
                            strFtpUserName = rowFileInfo3[0]["FtpUserName"].ToString();
                            strFtpPwd = rowFileInfo3[0]["FtpPwd"].ToString();
                            strFtpFilePath = rowFileInfo3[0]["FtpPath"].ToString();
                            strLocalFilePath = strLocalPath + "\\" + strFolderName;
                            strFileName3 = strDate + rowFileInfo3[0]["FtpFileName"].ToString();
                            strZipPwd = RedirectHelper.GetDecryptString(rowFileInfo3[0]["ZipPwd"].ToString());

                            for (int rowExportAdd2 = 0; rowExportAdd2 < RowExportCardBackInfAdd.Length; rowExportAdd2++)
                            {

                                strFileContent3 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd2]["EnditemName"].ToString().Trim(), 20);//*結案方式
                                //2021/01/20 陳永銘 修改成長姓名
                                strFileContent3 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd2]["NewName"].ToString().Trim(), 100);//*姓名
                                //2021/01/20 陳永銘 增加羅馬拼音
                                strFileContent3 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd2]["NewName_Roma"].ToString().Trim(), 100);//*姓名
                                strFileContent3 += JobHelper.SetStrngValue(RowExportCardBackInfAdd[rowExportAdd2]["NewZip"].ToString().Trim(), 12);//* 退件日期NewAdd1
                                strFileContent3 += JobHelper.SetStrngValue((RowExportCardBackInfAdd[rowExportAdd2]["NewAdd1"].ToString().Trim() + RowExportCardBackInfAdd[rowExportAdd2]["NewAdd2"].ToString().Trim() + RowExportCardBackInfAdd[rowExportAdd2]["NewAdd3"].ToString().Trim()).Replace(" ", ""), 120);//*地址一+地址二+地址三                       
                                strFileContent3 += "\r\n";
                            }
                            if (strFileContent3.Length > 2)
                            {
                                strFileContent3 = strFileContent3.ToString().Substring(0, strFileContent3.ToString().Length - 2);
                            }
                            FileTools.Create(strLocalFilePath + "\\" + strFileName3 + ".txt", strFileContent3);

                            if (File.Exists(strLocalFilePath + "\\" + strFileName3 + ".txt"))
                            {
                                DataRow row = dtLocalFile.NewRow();
                                row["txtStates"] = "S";
                                row["LocalFilePath"] = strLocalFilePath + "\\";
                                row["FtpFilePath"] = strFtpFilePath + "//";
                                row["FtpIP"] = strFtpIp;
                                row["FtpUserName"] = strFtpUserName;
                                row["FtpPwd"] = strFtpPwd;
                                row["FileName"] = strFileName3;
                                row["ZipPwd"] = strZipPwd;
                                dtLocalFile.Rows.Add(row);

                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000029, strFileName3), LogState.Info);
                            }
                            else
                            {
                                DataRow row = dtLocalFile.NewRow();
                                row["txtStates"] = "F";
                                row["FileName"] = strFileName3;
                                row["UploadStates"] = "F";
                                dtLocalFile.Rows.Add(row);

                                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000030, strFileName3));
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        JobHelper.SaveLog(Resources.JobResource.Job0113002.ToString(), LogState.Info);
                    }

                    #region OASA注銷
                    DataRow[] RowOASACardBackInfo = dtExportCardBackInfo.Select("EndFunction ='0' AND Enditem ='5'");

                    if (RowOASACardBackInfo.Length > 0)
                    {
                        int intSOASACount = 0;
                        int intFOASACount = 0;
                        string strCancelOASAFile = string.Empty;
                        string strOASAUserId = string.Empty;

                        string strBlkCode = rowFileInfo1[0]["BLKCode"].ToString().Trim();
                        string strMemo = rowFileInfo1[0]["MEMO"].ToString().Trim();
                        string strReasonCode = rowFileInfo1[0]["ReasonCode"].ToString().Trim();
                        string strActionCode = rowFileInfo1[0]["ActionCode"].ToString().Trim();

                        if (GetOASAFileName(ref strCancelOASAFile))
                        {
                            EntitySet<Entity_CancelOASA_Detail> SetCancelOASADetail = new EntitySet<Entity_CancelOASA_Detail>();

                            for (int intOASABack = 0; intOASABack < RowOASACardBackInfo.Length; intOASABack++)
                            {
                                string strCardNo = RowOASACardBackInfo[intOASABack]["CardNo"].ToString().Trim();
                                string strMemoLog = string.Empty;
                                string strBlockLog = string.Empty;
                                string strSFFlg = string.Empty;
                                if (this.HtgOASAAdd(strCardNo, strBlkCode, strMemo, strReasonCode, strActionCode, ref strMemoLog, ref strBlockLog, ref strOASAUserId, context))
                                {
                                    intSOASACount++;
                                    strSFFlg = "1";
                                }
                                else
                                {
                                    intFOASACount++;
                                    strSFFlg = "2";
                                }

                                Entity_CancelOASA_Detail CancelOASADetail = new Entity_CancelOASA_Detail();
                                CancelOASADetail.CancelOASAFile = strCancelOASAFile;
                                CancelOASADetail.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                                CancelOASADetail.CardNo = strCardNo;
                                CancelOASADetail.BlockCode = strBlkCode;
                                CancelOASADetail.ActionCode = strActionCode;
                                CancelOASADetail.Memo = strMemo;
                                CancelOASADetail.ReasonCode = strReasonCode;
                                CancelOASADetail.MemoLog = strMemoLog;
                                CancelOASADetail.BlockLog = strBlockLog;
                                CancelOASADetail.SFFlg = strSFFlg;
                                SetCancelOASADetail.Add(CancelOASADetail);

                                DataRow rowCardBackInfo = dtCardBackInfo.NewRow();
                                rowCardBackInfo["SerialNo"] = RowOASACardBackInfo[intOASABack]["serial_no"].ToString().Trim();
                                rowCardBackInfo["BlockCode"] = strBlockLog;
                                dtCardBackInfo.Rows.Add(rowCardBackInfo);
                            }

                            EntitySet<Entity_CancelOASA> SetCancelOASA = new EntitySet<Entity_CancelOASA>();
                            Entity_CancelOASA CancelOASA = new Entity_CancelOASA();
                            string strOASAFile = string.Empty;
                            CancelOASA.CancelOASAFile = strCancelOASAFile;
                            CancelOASA.CancelOASADate = DateTime.Now.ToString("yyyy/MM/dd");
                            CancelOASA.CancelOASAUser = strOASAUserId;
                            CancelOASA.TotalCount = intFOASACount + intSOASACount;
                            CancelOASA.SCount = intSOASACount;
                            CancelOASA.FCount = intFOASACount;
                            CancelOASA.CancelOASASource = "3";
                            SetCancelOASA.Add(CancelOASA);
                            if (BRM_CancelOASA.BatInsert(SetCancelOASA) && BRM_CardBackInfo.BLKUpdateFor0113(dtCardBackInfo))
                            {
                                string strMsgID = string.Empty;
                                BRM_CancelOASADetail.BatInsert(SetCancelOASADetail, ref strMsgID);
                            }
                        }

                        MainFrameInfoOASA.ClearHtgSessionJob(ref strSessionId, strJobId);
                    }
                    else
                    {
                        JobHelper.SaveLog(Resources.JobResource.Job0113003.ToString(), LogState.Info);
                    }
                    #endregion
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 壓縮檔案
            DataRow[] RowtxtS = dtLocalFile.Select("txtStates='S'");
            for (int rowcount = 0; rowcount < RowtxtS.Length; rowcount++)
            {
                string strFile = RowtxtS[rowcount]["LocalFilePath"].ToString().Trim() + RowtxtS[rowcount]["FileName"].ToString().Trim() + ".txt";
                string strZipFile = RowtxtS[rowcount]["LocalFilePath"].ToString().Trim() + RowtxtS[rowcount]["FileName"].ToString().Trim() + ".ZIP";
                string strZipName = "";
                string strPwd = RowtxtS[rowcount]["ZipPwd"].ToString().Trim();

                string[] arrFileList = new string[1];
                arrFileList[0] = strFile;
                int intResult = JobHelper.Zip(strZipFile, arrFileList, strZipName, strPwd, CompressToZip.CompressLevel.Level6);

                if (intResult > 0)
                {
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000031, strFile), LogState.Info);
                    RowtxtS[rowcount]["ZipStates"] = "S";
                }
                //*解壓失敗
                else
                {
                    RowtxtS[rowcount]["ZipStates"] = "F";
                    RowtxtS[rowcount]["UploadStates"] = "F";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000032, strFile));
                }

            }

            #endregion

            #region 登陸ftp上載文件
            //if (JobHelper.LoginFtp(strJobId))
            //{
            //    DataRow[] rows = dtLocalFile.Select("ZipStates='S'");
            //    for (int j = 0; j < rows.Length; j++)
            //    {
            //        if (JobHelper.Upload(dtLocalFile.Rows[j]["FtpFilePath"].ToString().Trim(), dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP", dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"))
            //        {
            //            //*更新上載狀態為S
            //            dtLocalFile.Rows[j]["UploadStates"] = "S";
            //            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000033, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"));
            //        }
            //        else
            //        {
            //            //*更新上載狀態為F
            //            dtLocalFile.Rows[j]["UploadStates"] = "F";
            //            JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000034, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"));
            //        }
            //    }
            //}
            DataRow[] rows = dtLocalFile.Select("ZipStates='S'");
            for (int j = 0; j < rows.Length; j++)
            {
                FTPFactory objFtp = new FTPFactory(dtLocalFile.Rows[j]["FtpIp"].ToString().Trim(), ".", dtLocalFile.Rows[j]["FtpUserName"].ToString().Trim(), dtLocalFile.Rows[j]["FtpPwd"].ToString().Trim(), "21", @"C:\CS09", "Y");
                if (objFtp.Upload(dtLocalFile.Rows[j]["FtpFilePath"].ToString().Trim(), dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP", dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"))
                {
                    //*更新上載狀態為S
                    dtLocalFile.Rows[j]["UploadStates"] = "S";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000033, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"), LogState.Info);
                }
                else
                {
                    //*更新上載狀態為F
                    dtLocalFile.Rows[j]["UploadStates"] = "F";
                    JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000034, dtLocalFile.Rows[j]["LocalFilePath"].ToString().Trim() + dtLocalFile.Rows[j]["FileName"].ToString().Trim() + ".ZIP"));
                }
            }

            #endregion

            #region 刪除本地上載成功文件
            DataRow[] rowDels = dtLocalFile.Select("UploadStates='S'");
            for (int k = 0; k < rowDels.Length; k++)
            {
                FileTools.DeleteFile(rowDels[k]["LocalFilePath"].ToString().Trim() + rowDels[k]["FileName"].ToString().Trim() + ".ZIP");
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
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/04
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

    #region 加載属性名
    /// <summary>
    /// 功能說明:MergeTable加載属性名
    /// 作    者:linda
    /// 創建時間:2010/06/10
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public void MergeTable(ref DataTable dtExportCardBackInfo)
    {
        string strMsgID = string.Empty;
        dtExportCardBackInfo.Columns.Add("ActionName");
        dtExportCardBackInfo.Columns.Add("ReasonName");
        dtExportCardBackInfo.Columns.Add("EnditemName");
        //*卡別(Action)
        DataTable dtExportCardBackInfotype = new DataTable();

        //*退件原因
        DataTable dtReason = new DataTable();

        //*處理方法/退件结案方式
        DataTable dtEnditem = new DataTable();
        SqlHelper sqlhelps = new SqlHelper();
        sqlhelps.AddCondition(EntityLayer.EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, "06");
        sqlhelps.AddCondition(EntityLayer.EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, "16");
        foreach (DataRow row in dtExportCardBackInfo.Rows)
        {

            //*卡別顯示Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "1", ref dtExportCardBackInfotype))
            {
                DataRow[] rowAction = dtExportCardBackInfotype.Select("PROPERTY_CODE='" + row["Action"].ToString().Trim() + "'");
                if (rowAction != null && rowAction.Length > 0)
                {
                    row["ActionName"] = rowAction[0]["PROPERTY_NAME"].ToString().Trim();
                }
                else
                {
                    row["ActionName"] = row["Action"].ToString().Trim();
                }
            }
            //*Reason顯示Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "5", ref dtReason))
            {
                DataRow[] rowReason = dtReason.Select("PROPERTY_CODE='" + row["Reason"].ToString().Trim() + "'");
                if (rowReason != null && rowReason.Length > 0)
                {
                    row["ReasonName"] = rowReason[0]["PROPERTY_NAME"].ToString().Trim();
                }
                else
                {
                    row["ReasonName"] = row["Reason"].ToString().Trim();
                }
            }
            //認同代碼顯示Name
            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "16", ref dtEnditem))
            {
                DataRow[] rowEnditem = dtEnditem.Select("PROPERTY_CODE='" + row["Enditem"].ToString().Trim() + "'");
                if (rowEnditem != null && rowEnditem.Length > 0)
                {
                    row["EnditemName"] = rowEnditem[0]["PROPERTY_NAME"].ToString().Trim();
                }
                else
                {
                    row["EnditemName"] = row["Enditem"].ToString().Trim();
                }
            }
        }

    }
    #endregion

    #region 獲取注銷檔名
    /// <summary>
    /// 功能說明:GetOASAFileName獲取注銷檔名
    /// 作    者:linda
    /// 創建時間:2010/07/08
    /// 修改記錄:
    /// </summary>
    /// <param name="dtMinusDate"></param>
    public bool GetOASAFileName(ref string strOASAFileName)
    {
        DataTable dtOASAFileName = new DataTable();
        if (BRM_CancelOASAFileName.SearchOASAFileName(ref dtOASAFileName))
        {
            if (dtOASAFileName.Rows.Count > 0)
            {
                strOASAFileName = Convert.ToString(Convert.ToDouble(dtOASAFileName.Rows[0]["CancelOASAFile"].ToString().Trim()) + 1);
            }
            else
            {
                strOASAFileName = DateTime.Now.ToString("yyyyMMdd") + "001";
            }
            if (!BRM_CancelOASAFileName.InsertOASAFileName(strOASAFileName))
            {
                JobHelper.SaveLog(string.Format(Resources.JobResource.Job0112005, strOASAFileName));
                return false;
            }
            else
            {
                return true;
            }

        }
        else
        {
            return false;
        }

    }
    #endregion

    #region 新增主機OASA資料
    /// <summary>
    /// 功能說明:新增主機OASA資料
    /// 作    者:Linda
    /// 創建時間:2010/07/07
    /// 修改記錄:
    /// <param name="htReturn">主機傳回資料</param>
    /// <param name="dtblUpdateData">更改的主機欄位信息的DataTable</param>
    /// <param name="strDesp">異動BLK CODE欄位名稱</param>
    /// <returns>true成功，false失敗</returns>
    private bool HtgOASAAdd(string strCardNo, string strBlkCode, string strMemo, string strReasonCode, string strActionCode, ref string strErrorMsg, ref string strBLCLog, ref string strUserId, Quartz.JobExecutionContext context)
    {
        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料

        string strPurgeDateReq = DateTime.Now.AddMonths(3).ToString("MMdd");

        htInput.Add("sessionId", strSessionId);

        htInput.Add("FUNCTION_CODE", "A");
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]

        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("OASA_BLOCK_CODE", strBlkCode);//*BLK CODE
        htInput.Add("OASA_MEMO", strMemo);//*MEMO
        htInput.Add("OASA_REASON_CODE", strReasonCode);//*REASON CODE
        htInput.Add("OASA_ACTION_CODE", strActionCode);//*ACTION CODE

        htInput.Add("OASA_PURGE_DATE", strPurgeDateReq);//*PURGE DATE

        //*提交OASA_P4_Submit主機資料

        Hashtable htResultA = MainFrameInfoOASA.GetMainFrameInfo(MainFrameInfoOASA.HtgType.P4_JCAX, htInput, false, "100", GetAgentInfo(context), strJobId);
        if (!htResultA.Contains("HtgMsg"))
        {
            strErrorMsg = "";//*主機返回成功訊息
            strBLCLog = "";
            strUserId = htResultA["USER_ID"].ToString().Trim();
            strSessionId = htResultA["sessionId"].ToString().Trim();
            return true;
        }
        else
        {
            strErrorMsg = htResultA["HtgMsg"].ToString().Trim();
            strSessionId = "";
            if (htResultA.Count > 2)
            {
                strBLCLog = htResultA["OASA_BLOCK_CODE"].ToString().Trim();
                strUserId = htResultA["USER_ID"].ToString().Trim();
                strSessionId = htResultA["sessionId"].ToString().Trim();
            }

            return false;
        }

    }
    #endregion

    #region 得到登陸主機信息
    /// <summary>
    /// 得到登陸主機信息
    /// </summary>
    /// <returns>EntityAGENT_INFO</returns>
    private EntityAGENT_INFO GetAgentInfo(Quartz.JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }
        return eAgentInfo;
    }
    #endregion

    //20161108 (U) by Tank, 取CardType中文
    protected string GetCardTypeName(string strCardType)
    {
        DataHelper dh = new DataHelper();
        SqlCommand sqlcmd = new SqlCommand();
        DataSet ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 240;
            sqlcmd.CommandText = @"select CardTypeName from tbl_CardType where CardType=@CardType ";

            SqlParameter ParCardType = new SqlParameter("@CardType", strCardType);
            sqlcmd.Parameters.Add(ParCardType);
            ds = dh.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "信用卡";
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}
