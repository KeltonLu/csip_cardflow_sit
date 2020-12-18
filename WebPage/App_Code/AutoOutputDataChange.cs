//******************************************************************
//*  功能說明：自動化異動資料通知
//*  作    者：Simba Liu
//*  創建日期：2010/05/19
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
//20190315 (U) by Nash, 姓名欄位改為姓名全名

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using Framework.Common.IO;
using BusinessRules;
using EntityLayer;
using System.Collections;
using System.IO;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using System.Text;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;

public class AutoOutputDataChange : Quartz.IJob
//public class AutoOutputDataChange 
{

    #region job基本參數設置
    protected string strJobId = string.Empty;//*"0104"
    protected string strFunctionKey = "06";
    private string strSessionId = "";
    protected JobHelper JobHelper = new JobHelper();
    protected int SCount;
    protected int FCcunt;
    protected string strAmOrPm;
    protected string strLocalPath = string.Empty;//*ConfigurationManager.AppSettings["DownloadFilePath"] + "0104";
    protected DataTable dtLocalFile;
    protected DataTable dtFileInfo;
    protected DateTime StartTime;
    protected DateTime EndTime;
    protected FTPFactory objFtp;

    protected int iPsno = 0;
    protected DataTable dtOutData;
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/19
    /// 修改記錄:2020/11/09_Ares_Stanley-調整Log內容; 2020/12/17_Ares_Stanley-變更CardChange PSNO 儲存欄位長度Int16->Int32
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    //public void Execute()
    {
        try
        {
            #region 記錄job啟動時間
            StartTime = DateTime.Now;
            #endregion

            #region load jobid and LocalPath
            strJobId = context.JobDetail.JobDataMap["JOBID"].ToString();
            JobHelper.strJobId = strJobId;
            //strJobId = "0104";
            strLocalPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath") + "\\" + strJobId + "\\";
            //strLocalPath = UtilHelper.GetAppSettings("UpLoadFilePath") + "\\" + strJobId + "\\";
            #endregion


            #region 記錄job啟動時間的分段
            string strAmOrPm = string.Empty;
            JobHelper.SaveLog(strJobId + "JOB啟動", LogState.Info);
            JobHelper.IsAmOrPm(StartTime, ref strAmOrPm);
            #endregion

            #region 計數器歸零
            SCount = 0;
            FCcunt = 0;
            #endregion

            #region 匯出資料明細
            dtLocalFile = new DataTable();
            dtLocalFile.Columns.Add("LocalFilePath");      //本地全路徑
            dtLocalFile.Columns.Add("FtpFilePath");        //Ftp全路徑
            dtLocalFile.Columns.Add("FolderName");         //目錄名稱
            dtLocalFile.Columns.Add("TxtFileName");        //資料檔名
            dtLocalFile.Columns.Add("UploadStates");       //資料上載狀態
            dtLocalFile.Columns.Add("Merch_Code");         //卡商代號
            dtLocalFile.Columns.Add("ZipPwd");          //压缩密码
            dtLocalFile.Columns.Add("ZipName");       //压缩檔名

            dtOutData = new DataTable();
            dtOutData.Columns.Add("Indate1");   //製卡日
            dtOutData.Columns.Add("ID");        //身份證字號
            dtOutData.Columns.Add("NewName");   //新用戶名
            dtOutData.Columns.Add("CardNo");    //卡號
            dtOutData.Columns.Add("Newmonlimit"); //新額度
            dtOutData.Columns.Add("Oldway");      //原取卡方式
            dtOutData.Columns.Add("UrgencyFlg");  //緊急製卡
            dtOutData.Columns.Add("Oldmailno");   //原掛號號碼
            dtOutData.Columns.Add("Newway");    //新取卡方式
            dtOutData.Columns.Add("NewwayKey");    //新取卡方式Key
            dtOutData.Columns.Add("Newmailno"); //新掛號號碼
            dtOutData.Columns.Add("MailDate");  //郵寄日期
            dtOutData.Columns.Add("ZIP");       //郵遞區號
            dtOutData.Columns.Add("Add1");      //地址一
            dtOutData.Columns.Add("Add2");      //地址二
            dtOutData.Columns.Add("Add3");      //地址三
            dtOutData.Columns.Add("PHOTO");     //照片別
            dtOutData.Columns.Add("ACTION");    //卡別
            dtOutData.Columns.Add("PSNO");      //轉出流水號

            dtOutData.Columns.Add("SNO");       //異動流水號
            dtOutData.Columns.Add("LocalFilePath");  //本地檔名
            dtOutData.Columns.Add("TableName");      //資料來源檔名
            dtOutData.Columns.Add("Merch_Code");     //製卡廠代碼
            dtOutData.Columns.Add("TxtFileName");    //製卡廠代碼
            dtOutData.Columns.Add("UpdDate");        //更新日期
            dtOutData.Columns.Add("trandate");       //轉檔日
            #endregion

            #region 判斷job工作狀態
            if (JobHelper.SerchJobStatus(strJobId) == "" || JobHelper.SerchJobStatus(strJobId) == "0")
            {
                JobHelper.SaveLog("JOB 工作狀態為：停止！", LogState.Info);
                return;
            }
            #endregion

            #region 檢測JOB今日是否為工作日，工作日才要執行
            if (!BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
            {
                JobHelper.SaveLog("今日非工作日！", LogState.Info);
                // 返回不在執行           
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

            BRM_Log.Insert("sys", StartTime.ToString("yyyy/MM/dd HH:mm:ss"), Resources.JobResource.Job0000104, "OU");

            #region 尚未退未回轉出的異動作業單
            DataTable dtDataChange = new DataTable();
            BRM_CardDataChange.SearchCardDataChange(ref dtDataChange);
            #endregion

            #region 尚未退回未轉出的無法制卡資料
            DataTable dtUnableCard = new DataTable();
            BRM_UnableCard.SearchUnableCard(ref dtUnableCard);
            #endregion

            #region 尚未退回未轉出的年度換卡異動資料
            DataTable dtChangeCard = new DataTable();
            BRM_CardChange.SearchCardChange(ref dtChangeCard);
            #endregion

            #region 排除不用匯出的異動資料
            DataTable dtTemp = new DataTable();
            ExcludeData(ref dtDataChange, ref dtUnableCard, ref dtChangeCard);
            #endregion

            #region 待匯出資料Merge
            DataTable dtMerge = new DataTable();
            DataTableMerge(dtDataChange, dtUnableCard, dtChangeCard, ref dtMerge);
            #endregion

            #region 依據異動資料檔Layout產出異動資料文字
            ArrayList List = new ArrayList();//*記錄產生的文件明細
            string strFolderName = string.Empty;
            JobHelper.CreateFolderName(strJobId, ref strFolderName);
            //JobHelper.CreateLocalFolder(strLocalPath + strFolderName);//*建立本地存放路徑
            JobHelper.CreateLocalFolder(strLocalPath);//*建立本地存放路徑
            int Seq = 1;
            //string strPath = strLocalPath ;
            string[] strFiles = Framework.Common.IO.FileTools.GetFileList(strLocalPath);
            string strDate = DateTime.Now.ToString("yyyyMMdd");


            DataTable dtMerch = new DataTable();
            string strFileName = string.Empty;     //*文件名稱        
            string strFileContent = string.Empty;  //*文件內容 
            string strNowDate1 = System.DateTime.Now.AddDays(-3).ToString("yyyy/MM/dd");
            string strNowDate2 = System.DateTime.Now.AddDays(-7).ToString("yyyy/MM/dd");
            StringBuilder sbCardNo3 = new StringBuilder();
            StringBuilder sbCardNo7 = new StringBuilder();

            if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "21", ref dtMerch))
            {
                foreach (DataRow drMerch in dtMerch.Rows)
                {
                    string strMerch = BaseHelper.ObjToString(drMerch["PROPERTY_CODE"].ToString());

                    if (!string.IsNullOrEmpty(strMerch))
                    {
                        Seq = 1;
                        foreach (string strFile in strFiles)
                        {
                            if (strFile.Substring(strFile.Length - 3).Equals("txt"))//排除ZIP檔
                            {
                                if (strFile.Substring(strFile.LastIndexOf('\\') + 1, 8).Equals(strDate))
                                {
                                    if (strFile.Substring(strFile.LastIndexOf('.') - 1, 1).Equals(strMerch))
                                    {
                                        Seq = Seq + 1;
                                    }
                                }
                            }
                        }
                        DataRow[] drOutPut = dtOutData.Select("Merch_Code='" + strMerch + "'", "Indate1,CardNo");
                        if (null != drOutPut && drOutPut.Length > 0)
                        {
                            strFileName = strDate + "-" + Seq.ToString().PadLeft(2, '0') + "_" + drOutPut[0]["Merch_Code"] + ".txt";
                            StreamWriter sw = new StreamWriter(strLocalPath + strFileName, true, Encoding.Default);
                            //StreamWriter sw = new StreamWriter(strLocalPath + strFolderName + "\\" + strFileName, true, Encoding.Default);
                            for (int m = 0; m < drOutPut.Length; m++)
                            {
                                strFileContent = string.Empty;
                                //* update in 2010/09/01 by SIT&UAT bug list.xls(56)
                                //strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Indate1"].ToString(),10);//*製卡日
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Indate1"].ToString().Replace('/', '.'), 10);//*製卡日
                                //*end update
                                if (null != drOutPut[m]["ID"] && !string.IsNullOrEmpty(drOutPut[m]["ID"].ToString()))
                                {
                                    string strId = drOutPut[m]["ID"].ToString();
                                    strFileContent += JobHelper.SetStrngValue(strId, 11).Replace(strId.Substring(2, 3), "XXX");//*ID
                                    //strFileContent += JobHelper.SetStrngValue(strId,11);//*ID
                                }
                                else
                                {
                                    strFileContent += JobHelper.SetStrngValue(" ", 11);
                                }
                                string strFname = string.Empty;
                                string strName = string.Empty;
                                if (null != drOutPut[m]["NewName"] && !string.IsNullOrEmpty(drOutPut[m]["NewName"].ToString()))
                                {
                                    //20190315 (U) by Nash, 姓名欄位改為姓名全名
                                    //strFname = drOutPut[m]["NewName"].ToString().Substring(0, 1);
                                    //strFname = strFname.Replace(strFname, "　");
                                    //strName = drOutPut[m]["NewName"].ToString().Substring(1, drOutPut[m]["NewName"].ToString().Length - 1);
                                    //strFileContent += JobHelper.SetStrngValue(strFname + strName, 20);//*姓名
                                    strName = drOutPut[m]["NewName"].ToString().Substring(0, drOutPut[m]["NewName"].ToString().Length);
                                    strFileContent += JobHelper.SetStrngValue(strName, 20);//*姓名
                                }
                                else
                                {
                                    strFileContent += JobHelper.SetStrngValue(" ", 20);//*姓名
                                }

                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["CardNo"].ToString(), 19);//*卡號
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Newmonlimit"].ToString(), 8);//*新額度
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Oldway"].ToString(), 50);//*原取卡方式
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["UrgencyFlg"].ToString(), 1);//*緊急製卡
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Oldmailno"].ToString(), 20);//*原掛號號碼
                                if (drOutPut[m]["Newway"].ToString().Length > 2)
                                {
                                    strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Newway"].ToString(), 50);//*新取卡方式
                                }
                                else
                                {
                                    strFileContent += JobHelper.SetStrngValue(getKind(drOutPut[m]["Newway"].ToString()), 50);//*新取卡方式                                
                                }
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Newmailno"].ToString(), 20);//*新掛號號碼
                                //* update in 2010/09/01 by SIT&UAT bug list.xls(56)
                                //strFileContent += JobHelper.SetStrngValue(drOutPut[m]["MailDate"].ToString(),10);//*郵寄日期
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["MailDate"].ToString().Replace('/', '.'), 10);//*郵寄日期
                                //* end update
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["ZIP"].ToString(), 6);//*ZIP
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Add1"].ToString(), 40);//*地址一
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Add2"].ToString(), 40);//*地址二
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["Add3"].ToString(), 40);//*地址三
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["PHOTO"].ToString(), 2);//*PHOTO
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["ACTION"].ToString(), 2);//*ACTION
                                strFileContent += JobHelper.SetStrngValue(drOutPut[m]["PSNO"].ToString(), 10);//*SNO
                                //strFileContent += " ".PadRight(40, ' ');
                                sw.WriteLine(strFileContent);

                                //取卡方式為扣卡保留則發送Mail通知
                                if (drOutPut[m]["oldway"].ToString().Equals("6"))
                                {
                                    string strUpdate = drOutPut[m]["UpdDate"].ToString();

                                    if (strUpdate.Equals(strNowDate1))
                                    {
                                        if (null == sbCardNo3 || sbCardNo3.Length == 0)
                                        {
                                            sbCardNo3.Append(drOutPut[m]["CardNo"].ToString());
                                        }
                                        else
                                        {
                                            sbCardNo3.Append("、");
                                            sbCardNo3.Append(drOutPut[m]["CardNo"].ToString());
                                        }
                                    }
                                    if (strUpdate.Equals(strNowDate2))
                                    {
                                        if (null == sbCardNo7 || sbCardNo7.Length == 0)
                                        {
                                            sbCardNo7.Append(drOutPut[m]["CardNo"].ToString());
                                        }
                                        else
                                        {
                                            sbCardNo7.Append("、");
                                            sbCardNo7.Append(drOutPut[m]["CardNo"].ToString());
                                        }
                                    }
                                }
                                drOutPut[m]["LocalFilePath"] = strLocalPath + strFileName; //+ strFolderName + "\\" + strFileName;
                                drOutPut[m]["TxtFileName"] = strFileName;
                            }

                            DataRow row = dtLocalFile.NewRow();//*記錄文件名稱以便刪除之用
                            row["LocalFilePath"] = strLocalPath + strFileName;// +strFolderName + "\\" + strFileName;
                            row["TxtFileName"] = strFileName;
                            dtLocalFile.Rows.Add(row);
                            sw.Close();
                            sw.Dispose();

                            //drOutPut[m]["Merch_Code"] = dtGroup.Rows[i]["Merch_Code"].ToString();
                            Seq++;
                        }
                    }
                }
            }
            #endregion

            #region 如果有扣卡保留的資料并且滿足條件的發送Mail通知
            ArrayList alInfo = new ArrayList();
            alInfo.Add("");
            alInfo.Add("");
            bool sendFlg = false;

            if (null != sbCardNo3 && sbCardNo3.Length > 0)
            {
                alInfo[0] = sbCardNo3.ToString();
                sendFlg = true;
            }

            if (null != sbCardNo7 && sbCardNo7.Length > 0)
            {
                alInfo[1] = sbCardNo7.ToString();
                sendFlg = true;
            }
            if (sendFlg)
            {
                SendMail("5", alInfo, Resources.JobResource.Job0000042);
            }
            #endregion

            #region 壓縮檔案
            JobHelper.SaveLog("開始壓縮檔案", LogState.Info);
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案壓縮資料成功！", LogState.Info);
                for (int z = 0; z < dtLocalFile.Rows.Count; z++)
                {
                    string strFile = strLocalPath + dtLocalFile.Rows[z]["TxtFileName"].ToString();
                    string strZipFile = strLocalPath + dtLocalFile.Rows[z]["TxtFileName"].ToString().Replace(".txt", ".ZIP");
                    string strZipName = "";
                    string strPwd = RedirectHelper.GetDecryptString(dtFileInfo.Rows[0]["ZipPwd"].ToString());

                    string[] arrFileList = new string[1];
                    arrFileList[0] = strFile;
                    int intResult = JobHelper.Zip(strZipFile, arrFileList, strZipName, strPwd, CompressToZip.CompressLevel.Level6);

                    if (intResult > 0)
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000031, strFile), LogState.Info);
                        dtLocalFile.Rows[z]["ZipName"] = dtLocalFile.Rows[z]["TxtFileName"].ToString().Replace(".txt", ".ZIP");
                    }
                    //*壓縮失敗
                    else
                    {
                        JobHelper.SaveLog(string.Format(Resources.JobResource.Job0000032, strFile));
                    }
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 登陸ftp上載文件
            string strFtpPath = string.Empty;
            string strFtpIp = string.Empty;
            string strFtpUserName = string.Empty;
            string strFtpPwd = string.Empty;
            JobHelper.SaveLog("開始上傳文件", LogState.Info);
            if (JobHelper.SearchFileInfo(ref dtFileInfo, strJobId))
            {
                JobHelper.SaveLog("從DB中讀取檔案上傳資料成功！", LogState.Info);
                if (dtFileInfo.Rows.Count > 0)
                {
                    //FTP 檔名
                    //string strFileInfo = DateTime.Now.ToString("yyyyMMdd") + dtFileInfo.Rows[0]["FtpFileName"].ToString() + ".ZIP";
                    //FTP 路徑+檔名
                    //string strFtpFileInfo = dtFileInfo.Rows[0]["FtpPath"].ToString() + "//" + strFileInfo;
                    String errMsg = "";

                    for (int j = 0; j < dtLocalFile.Rows.Count; j++)
                    {
                        DataRow[] drFileInfo = dtFileInfo.Select("MerchCode='" + dtLocalFile.Rows[j]["ZipName"].ToString().Substring(12, 1) + "'");

                        strFtpPath = drFileInfo[0]["FtpPath"].ToString();
                        strFtpIp = drFileInfo[0]["FtpIP"].ToString();
                        strFtpUserName = drFileInfo[0]["FtpUserName"].ToString();
                        strFtpPwd = drFileInfo[0]["FtpPwd"].ToString();

                        objFtp = new FTPFactory(strFtpIp, ".", strFtpUserName, strFtpPwd, "21", @"C:\CS09", "Y");

                        if (objFtp.Upload(strFtpPath, dtLocalFile.Rows[j]["ZipName"].ToString(), strLocalPath + dtLocalFile.Rows[j]["ZipName"].ToString()))
                        {
                            //*更新上載狀態為S
                            dtLocalFile.Rows[j]["UploadStates"] = "S";
                            JobHelper.SaveLog(dtLocalFile.Rows[j]["ZipName"] + "上傳成功", LogState.Info);
                        }
                        else
                        {
                            errMsg += (errMsg == "" ? "" : "、") + dtLocalFile.Rows[j]["ZipName"];
                            //*更新上載狀態為F
                            dtLocalFile.Rows[j]["UploadStates"] = "F";
                            JobHelper.SaveLog(dtLocalFile.Rows[j]["ZipName"] + "上傳失敗");
                            // alInfo.Add(dtLocalFile.Rows[j]["ZipName"]);
                            //上傳檔案失敗
                            // SendMail("1", alInfo, Resources.JobResource.Job0000042);
                        }
                    }

                    if (errMsg != "")
                    {
                        alInfo[0] = errMsg;
                        //上傳檔案失敗
                        SendMail("1", alInfo, Resources.JobResource.Job0000042);
                    }
                }
            }
            else
            {
                JobHelper.SaveLog("從DB抓取檔案資料失敗！");
            }
            #endregion

            #region 更新OutputFlg
            UpdateOutputFlg();
            WriteNoteCaptions();    //*將無法製卡檔、換卡異動檔要匯出的資料寫入異動記
            //updata ch---------------------------------
            CancelOASA(context, dtFileInfo);
            //updata -----------END---------------------
            #endregion.

            #region 刪除本地上載成功文件
            //DataRow[] rows = dtLocalFile.Select("UploadStates='S'");
            //for (int k = 0; k < rows.Length; k++)
            //{
            //    FileTools.DeleteFile(rows[k]["LocalFilePath"].ToString());
            //}
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
            BRM_LBatchLog.Insert(strFunctionKey, strJobId, StartTime, DateTime.Now, "F", "CommonModel_發生錯誤");
            BRM_LBatchLog.SaveLog(ex);
        }
    }

    #endregion

    #region 排除不用匯出的異動資料
    /// <summary>
    /// 功能說明:解壓檔失敗mail通知
    /// 作   者:Simba Liu
    /// 創建時間:2010/05/24
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDataChange"></param>
    /// <param name="dtUnableCard"></param>
    /// <param name="dtChangeCard"></param>
    /// <param name="dtTemp"></param>
    public void ExcludeData(ref DataTable dtDataChange, ref DataTable dtUnableCard, ref DataTable dtChangeCard)
    {
        #region 排除dtUnableCard，dtChangeCard
        if (null != dtUnableCard && null != dtChangeCard)
        {
            DataTable dtMerge = new DataTable();
            dtMerge = dtUnableCard.DefaultView.ToTable();
            dtMerge.Merge(dtChangeCard);
            DataTable dtGroup = dtMerge.DefaultView.ToTable(true, "Cardno");
            foreach (DataRow rowGroup in dtGroup.Rows)
            {
                DataRow[] rowTempUnableCard = dtUnableCard.Select("Cardno='" + rowGroup["Cardno"] + "'", "SNO DESC");
                DataRow[] rowTempChangeCard = dtChangeCard.Select("Cardno='" + rowGroup["Cardno"] + "'", "SNO DESC");
                DateTime dtFirst = DateTime.Now;
                DateTime dtSecond = DateTime.Now;

                if (rowTempUnableCard.Length > 0 && rowTempChangeCard.Length > 0)
                {
                    //*只匯出匯入日期較近的資料
                    if (null != rowTempChangeCard[0]["ImportDate"] && string.IsNullOrEmpty(rowTempChangeCard[0]["ImportDate"].ToString()))
                    {
                        dtFirst = Convert.ToDateTime(rowTempChangeCard[0]["ImportDate"].ToString());
                    }

                    if (null != rowTempUnableCard[0]["ImportDate"] && string.IsNullOrEmpty(rowTempUnableCard[0]["ImportDate"].ToString()))
                    {
                        dtSecond = Convert.ToDateTime(rowTempUnableCard[0]["ImportDate"].ToString());
                    }
                    TimeSpan tsNum = dtFirst - dtSecond;

                    if (tsNum.Days <= 0)
                    {
                        string strMsgID = string.Empty;
                        Entity_UnableCard UnableCard = new Entity_UnableCard();
                        UnableCard.CardNo = rowTempUnableCard[0]["CardNo"].ToString();
                        UnableCard.OutputFlg = "T"; //*更新轉出狀態為退件
                        SqlHelper sqlhelp = new SqlHelper();
                        sqlhelp.AddCondition(Entity_UnableCard.M_CardNo, Operator.Equal, DataTypeUtils.String, UnableCard.CardNo);
                        BRM_UnableCard.update(UnableCard, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg");
                        //rowTempUnableCard[0].Delete();
                        foreach (DataRow drUn in rowTempUnableCard)
                        {
                            dtUnableCard.Rows.Remove(drUn);
                        }
                    }
                    else
                    {
                        string strMsgID = string.Empty;
                        Entity_CardChange CardChange = new Entity_CardChange();
                        CardChange.CardNo = rowTempChangeCard[0]["CardNo"].ToString();
                        CardChange.OutputFlg = "T";//*更新轉出狀態為退件
                        SqlHelper sqlhelp = new SqlHelper();
                        sqlhelp.AddCondition(Entity_CardChange.M_CardNo, Operator.Equal, DataTypeUtils.String, CardChange.CardNo);
                        BRM_CardChange.update(CardChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg");
                        //rowTempChangeCard[0].Delete();
                        foreach (DataRow drXhange in rowTempChangeCard)
                        {
                            dtChangeCard.Rows.Remove(drXhange);
                        }
                    }
                }
            }
        }
        #endregion

        #region 排除dtDataChange
        if (null != dtDataChange)
        {
            DataTable dtGroupDataChange = dtDataChange.DefaultView.ToTable(true, "Cardno");
            foreach (DataRow rowTempDataChange in dtGroupDataChange.Rows)
            {
                DataRow[] rowTempDataChanges = dtDataChange.Select("Cardno='" + rowTempDataChange["Cardno"] + "'");
                //*全部異動掛號號碼
                if (FaxNO(rowTempDataChanges))
                {
                    foreach (DataRow rowDataChange in rowTempDataChanges)
                    {
                        dtDataChange.Rows.Remove(rowDataChange);
                    }
                }
                //*異動其他欄位
                else
                {
                    foreach (DataRow rowDataChange in rowTempDataChanges)
                    {
                        bool bolAddNOChange1 = false;
                        bool bolAddNOChange2 = false;
                        bool bolAddNOChange3 = false;
                        //*異動姓名
                        if (!string.IsNullOrEmpty(rowDataChange["NewName"].ToString()) && !string.IsNullOrEmpty(rowDataChange["OldName"].ToString()) && rowDataChange["NewName"].ToString().Equals(rowDataChange["OldName"].ToString()))
                        {
                            dtDataChange.Rows.Remove(rowDataChange);
                            continue;
                        }
                        //*異動額度
                        if (!string.IsNullOrEmpty(rowDataChange["Newmonlimit"].ToString()) && !string.IsNullOrEmpty(rowDataChange["Oldmonlimit"].ToString()) && rowDataChange["Newmonlimit"].ToString().Equals(rowDataChange["Oldmonlimit"].ToString()))
                        {
                            dtDataChange.Rows.Remove(rowDataChange);
                            continue;
                        }
                        //*異動地址一
                        if (!string.IsNullOrEmpty(rowDataChange["NewAdd1"].ToString()) && !string.IsNullOrEmpty(rowDataChange["OldAdd1"].ToString()) && rowDataChange["NewAdd1"].ToString().Equals(rowDataChange["OldAdd1"].ToString()))
                        {
                            bolAddNOChange1 = true;
                        }
                        //*異動地址二
                        if (!string.IsNullOrEmpty(rowDataChange["NewAdd2"].ToString()) && !string.IsNullOrEmpty(rowDataChange["OldAdd2"].ToString()) && rowDataChange["NewAdd2"].ToString().Equals(rowDataChange["OldAdd2"].ToString()))
                        {
                            bolAddNOChange2 = true;
                        }
                        //*異動地址三
                        if (!string.IsNullOrEmpty(rowDataChange["NewAdd3"].ToString()) && !string.IsNullOrEmpty(rowDataChange["OldAdd3"].ToString()) && rowDataChange["NewAdd3"].ToString().Equals(rowDataChange["OldAdd3"].ToString()))
                        {
                            bolAddNOChange3 = true;
                        }

                        if (bolAddNOChange1 && bolAddNOChange2 && bolAddNOChange3)
                        {
                            dtDataChange.Rows.Remove(rowDataChange);
                            continue;
                        }
                        //*異動郵寄日
                        if (!string.IsNullOrEmpty(rowDataChange["NewMailDate"].ToString()) && !string.IsNullOrEmpty(rowDataChange["OldMailDate"].ToString()) && rowDataChange["NewMailDate"].ToString().Equals(rowDataChange["OldMailDate"].ToString()))
                        {
                            dtDataChange.Rows.Remove(rowDataChange);
                            continue;
                        }
                        //*異動取卡方式

                        if ((!string.IsNullOrEmpty(rowDataChange["Newway"].ToString()) && !string.IsNullOrEmpty(rowDataChange["Oldway"].ToString()) && 
                             rowDataChange["Newway"].ToString().Equals(rowDataChange["Oldway"].ToString())) &&
                            (rowDataChange["Urgency_Flg"].ToString() == rowDataChange["NewUrgencyFlg"].ToString()))
                        {
                            //                if (BaseHelper.ObjToString(drTempData["Urgency_Flg"]) == BaseHelper.ObjToString(drTempData["NewUrgencyFlg"]))



                            //max 增加把取退件註記  CardNo
                            //*修改異動單
                            string strMsgID = string.Empty;
                            Entity_CardDataChange CardDataChanges = new Entity_CardDataChange();
                            CardDataChanges.Sno = int.Parse(rowDataChange["Sno"].ToString());
                            CardDataChanges.OutputFlg = "T";  //*更新狀態為退件
                            SqlHelper sqlhelps = new SqlHelper();
                            sqlhelps.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChanges.Sno.ToString());
                            BRM_CardDataChange.update(CardDataChanges, sqlhelps.GetFilterCondition(), ref strMsgID, "OutputFlg");

                            dtDataChange.Rows.Remove(rowDataChange);

                            continue;
                        }
                        ////*異動銷毀註 緊急製卡?
                        //if (!string.IsNullOrEmpty(rowDataChange["Newway"].ToString()) && !string.IsNullOrEmpty(rowDataChange["old"].ToString()) && rowDataChange["Newway"].ToString().Equals(rowDataChange["old"].ToString()))
                        //{
                        //    dtDataChange.Rows.Remove(rowDataChange);
                        //}
                        //if (rowDataChange["SourceType"].ToString().Equals("0") || rowDataChange["SourceType"].ToString().ToLower().Equals("null"))
                        //{
                        //    //*dtDataChange.Rows.Remove(rowDataChange);*/
                        //}
                        //else
                        //{
                        //    dtDataChange.Rows.Remove(rowDataChange);
                        //}
                    }
                }
            }
        }
        #endregion

    }
    #endregion

    #region DataTableMerge
    /// <summary>
    /// 功能說明:DataTableMerge
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/24
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDataChange"></param>
    /// <param name="dtUnableCard"></param>
    /// <param name="dtChangeCard"></param>
    /// <param name="dtMerge"></param>
    public void DataTableMerge(DataTable dtDataChange, DataTable dtUnableCard, DataTable dtChangeCard, ref DataTable dtMerge)
    {
        iPsno = BRM_CardDataChange.GetMaxSno();
        foreach (DataRow drUnCard in dtUnableCard.Rows)
        {
            DataRow UCrow = dtOutData.NewRow();
            UCrow["Indate1"] = BaseHelper.ObjToString(drUnCard["indate1"]); //製卡日
            UCrow["ID"] = BaseHelper.ObjToString(drUnCard["id"]);//身份證字號
            UCrow["NewName"] = BaseHelper.ObjToString(drUnCard["CustName"]);//新用戶名
            UCrow["CardNo"] = BaseHelper.ObjToString(drUnCard["CardNo"]); //卡號
            UCrow["Newmonlimit"] = ""; //新額度
            UCrow["Oldway"] = getKind(BaseHelper.ObjToString(drUnCard["Kind"])); //原取卡方式中文
            UCrow["UrgencyFlg"] = ""; //緊急製卡
            UCrow["Oldmailno"] = BaseHelper.ObjToString(drUnCard["mailno"]);  //原掛號號碼
            //UCrow["Newway"] = "無法製卡" + BaseHelper.ObjToString(drUnCard["blockcode"]);  //新取卡方式
            UCrow["Newway"] = "無法製卡";  //新取卡方式
            UCrow["Newmailno"] = "";  //新掛號號碼
            UCrow["MailDate"] = "";  //郵寄日期
            UCrow["ZIP"] = "";  //郵遞區號
            UCrow["Add1"] = "";  //地址一
            UCrow["Add2"] = "";  //地址二
            UCrow["Add3"] = "";  //地址三
            UCrow["PHOTO"] = BaseHelper.ObjToString(drUnCard["photo"]);  //照片別
            UCrow["ACTION"] = BaseHelper.ObjToString(drUnCard["Action"]);  //卡別
            UCrow["PSNO"] = iPsno.ToString();  //轉出流水號
            UCrow["SNO"] = BaseHelper.ObjToString(drUnCard["SNO"]);  //異動流水號
            UCrow["Merch_Code"] = BaseHelper.ObjToString(drUnCard["Merch_Code"]);  //製卡廠代碼 

            UCrow["UpdDate"] = BaseHelper.ObjToString(drUnCard["ImportDate"]);  //匯入時間(異動日期)
            UCrow["TableName"] = "tbl_UnableCard";  //資料來源檔名
            UCrow["trandate"] = BaseHelper.ObjToString(drUnCard["trandate"]);  //轉檔日

            dtOutData.Rows.Add(UCrow);
            iPsno += 1;
        }

        foreach (DataRow drChangeCard in dtChangeCard.Rows)
        {
            iPsno += 1;
            DataRow CCrow = dtOutData.NewRow();
            CCrow["Indate1"] = BaseHelper.ObjToString(drChangeCard["indate1"]); //製卡日
            CCrow["ID"] = BaseHelper.ObjToString(drChangeCard["ID"]);//身份證字號
            CCrow["NewName"] = BaseHelper.ObjToString(drChangeCard["custName"]);//新用戶名
            CCrow["CardNo"] = BaseHelper.ObjToString(drChangeCard["CardNo"]); //卡號
            CCrow["Newmonlimit"] = ""; //新額度
            CCrow["Oldway"] = getKind(BaseHelper.ObjToString(drChangeCard["Kind"])); //原取卡方式中文
            CCrow["UrgencyFlg"] = ""; //緊急製卡
            CCrow["Oldmailno"] = BaseHelper.ObjToString(drChangeCard["mailno"]);  //原掛號號碼
            CCrow["Newway"] = "卡片碎卡" + BaseHelper.ObjToString(drChangeCard["blockcode"]); //新取卡方式
            CCrow["Newmailno"] = "";  //新掛號號碼
            CCrow["MailDate"] = "";  //郵寄日期
            CCrow["ZIP"] = "";  //郵遞區號
            CCrow["Add1"] = "";  //地址一
            CCrow["Add2"] = "";  //地址二
            CCrow["Add3"] = "";  //地址三
            CCrow["PHOTO"] = BaseHelper.ObjToString(drChangeCard["photo"]);  //照片別
            CCrow["ACTION"] = BaseHelper.ObjToString(drChangeCard["Action"]);  //卡別
            CCrow["PSNO"] = iPsno.ToString();  //轉出流水號
            CCrow["SNO"] = BaseHelper.ObjToString(drChangeCard["Sno"]);  //異動流水號
            CCrow["Merch_Code"] = BaseHelper.ObjToString(drChangeCard["Merch_Code"]);  //製卡廠代碼 
            CCrow["UpdDate"] = BaseHelper.ObjToString(drChangeCard["ImportDate"]);  //匯入時間(異動日期)
            CCrow["TableName"] = "tbl_CardChange";  //資料來源檔名
            CCrow["trandate"] = BaseHelper.ObjToString(drChangeCard["trandate"]);  //轉檔日
            dtOutData.Rows.Add(CCrow);
        }

        DataTable dtCardGroup = dtOutData.DefaultView.ToTable(true, "Cardno");
        foreach (DataRow drCardNo in dtCardGroup.Rows)
        {
            DataRow[] rowTempDataCard = dtDataChange.Select("Cardno='" + drCardNo["Cardno"] + "'");
            foreach (DataRow drTmp in rowTempDataCard)
            {
                string strMsgID = string.Empty;
                Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
                CardDataChange.Sno = int.Parse(BaseHelper.ObjToString(drTmp["Sno"]));
                CardDataChange.OutputFlg = "T";
                CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChange.UpdUser = "SYS";
                CardDataChange.CNote = "卡片碎卡";
                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.String, CardDataChange.Sno.ToString());
                if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg", "UpdDate", "UpdUser", "CNote"))
                {
                    JobHelper.SaveLog(DateTime.Now.ToString() + " " + strMsgID, LogState.Info);
                }
                dtDataChange.Rows.Remove(drTmp);
            }
        }

        DataTable dtCardGroups = dtDataChange.DefaultView.ToTable(true, "Cardno");
        foreach (DataRow drDataCard in dtCardGroups.Rows)
        {
            DataRow[] rowTempCards = dtDataChange.Select("Cardno='" + drDataCard["Cardno"] + "'", "SNO DESC");
            DataRow drOut = MargeDataCard(rowTempCards);
            //Check 新卡改址轉出異動檔將卡片取卡方式改為保留
            //max 因為現在新卡改址的異動只委管才可以操作，因此不需要這樣控管
            //UpdataKind(drOut);
            dtOutData.Rows.Add(drOut);
        }
    }
    #endregion

    public DataRow MargeDataCard(DataRow[] drDataCard)
    {
        iPsno += 1;
        DataRow CCrow = dtOutData.NewRow();
        if (drDataCard.Length > 0)
        {
            CCrow["Indate1"] = BaseHelper.ObjToString(drDataCard[0]["indate1"]); //製卡日
            CCrow["ID"] = BaseHelper.ObjToString(drDataCard[0]["id"]);//身份證字號
            CCrow["CardNo"] = BaseHelper.ObjToString(drDataCard[0]["CardNo"]); //卡號
            CCrow["Oldway"] = getKind(BaseHelper.ObjToString(drDataCard[0]["Kind"])); //原取卡方式中文

            // CCrow["UrgencyFlg"] = BaseHelper.ObjToString(drDataCard[0]["Urgency_Flg"]); //緊急製卡
            // if (null != CCrow["UrgencyFlg"])
            // {
            //     if (string.IsNullOrEmpty(CCrow["UrgencyFlg"].ToString()))
            //     {
            //         CCrow["UrgencyFlg"] = "0";
            //     }
            // }

            CCrow["Oldmailno"] = BaseHelper.ObjToString(drDataCard[0]["Mailno"]);    //原掛號號碼
            CCrow["Merch_Code"] = BaseHelper.ObjToString(drDataCard[0]["Merch_Code"]);  //製卡廠代碼 
            CCrow["PHOTO"] = BaseHelper.ObjToString(drDataCard[0]["photo"]);  //照片別
            CCrow["ACTION"] = BaseHelper.ObjToString(drDataCard[0]["ACTION"]);  //卡別
            CCrow["TableName"] = "tbl_Card_DataChange";  //資料來源檔名
            CCrow["PSNO"] = iPsno.ToString();  //轉出流水號

        }

        foreach (DataRow drTempData in drDataCard)
        {
            string strNewName = BaseHelper.ObjToString(drTempData["NewName"]);
            string strCustName = BaseHelper.ObjToString(drTempData["custName"]);

            string strMonlimit = BaseHelper.ObjToString(drTempData["NewMonlimit"]); //新額度
            string strNewway = BaseHelper.ObjToString(drTempData["newway"]);   //新取卡方式
            string strNewmailno = BaseHelper.ObjToString(drTempData["newmailno"]);   //新掛號號碼
            string strNewMailDate = BaseHelper.ObjToString(drTempData["NewMailDate"]);   //新掛號號碼
            string strZip = BaseHelper.ObjToString(drTempData["NewZip"]);   //郵遞區號
            string strAdd1 = BaseHelper.ObjToString(drTempData["NewAdd1"]);  //地址一
            string strAdd2 = BaseHelper.ObjToString(drTempData["NewAdd2"]);  //地址二
            string strAdd3 = BaseHelper.ObjToString(drTempData["NewAdd3"]);  //地址三  

            if (!string.IsNullOrEmpty(strNewName))
            {
                CCrow["NewName"] = strNewName;//新用戶名
            }
            else
            {
                CCrow["NewName"] = strCustName;//用戶名
            }

            if (!string.IsNullOrEmpty(strMonlimit))
            {
                CCrow["Newmonlimit"] = strMonlimit;
            }

            if (!string.IsNullOrEmpty(strNewway))
            {
                CCrow["Newway"] = strNewway;
                CCrow["UpdDate"] = BaseHelper.ObjToString(drTempData["UpdDate"]);  //匯入時間(異動日期)
            }

            if (BaseHelper.ObjToString(drTempData["Urgency_Flg"]) == BaseHelper.ObjToString(drTempData["NewUrgencyFlg"]))
            {
                CCrow["UrgencyFlg"] = "";//緊急製卡
            }
            else
            {
                if (CCrow["UrgencyFlg"].ToString() == "")
                {
                    CCrow["UrgencyFlg"] = BaseHelper.ObjToString(drTempData["NewUrgencyFlg"]); //緊急製卡
                }
            }

            if (!string.IsNullOrEmpty(strNewmailno))
            {
                CCrow["Newmailno"] = strNewmailno;
            }
            if (!string.IsNullOrEmpty(strNewMailDate))
            {
                CCrow["MailDate"] = strNewMailDate;
            }
            if (!string.IsNullOrEmpty(strAdd1))
            {
                CCrow["Add1"] = strAdd1;
                CCrow["ZIP"] = strZip;
            }
            if (!string.IsNullOrEmpty(strAdd2))
            {
                CCrow["Add2"] = strAdd2;
            }
            //            if (!string.IsNullOrEmpty(strAdd3))
            //修正當異動地址時，址三為空白時，就不回壓的問題 Wallace
            if (!string.IsNullOrEmpty(strAdd1) || !string.IsNullOrEmpty(strAdd2))
            {
                CCrow["Add3"] = strAdd3;
            }
            //CCrow["SNO"] = BaseHelper.ObjToString(drTempData["SNo"]);  //異動流水號            
        }

        return CCrow;
    }

    public static bool UpdataKind(DataRow drCardInfo)
    {
        bool blFlg = false;
        if (null != drCardInfo)
        {
            Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
            CardBaseInfo.indate1 = BaseHelper.ObjToString(drCardInfo["Indate1"]);
            CardBaseInfo.id = BaseHelper.ObjToString(drCardInfo["ID"]);
            CardBaseInfo.cardno = BaseHelper.ObjToString(drCardInfo["CardNo"]);
            CardBaseInfo.action = BaseHelper.ObjToString(drCardInfo["ACTION"]);
            CardBaseInfo.add1 = BaseHelper.ObjToString(drCardInfo["Add1"]);
            CardBaseInfo.add2 = BaseHelper.ObjToString(drCardInfo["Add2"]);
            CardBaseInfo.add3 = BaseHelper.ObjToString(drCardInfo["Add3"]);
            CardBaseInfo.kind = "6"; // 取卡方式：6保留
            bool UpdataFlg = false;
            if (!string.IsNullOrEmpty(CardBaseInfo.add1) || !string.IsNullOrEmpty(CardBaseInfo.add2) || !string.IsNullOrEmpty(CardBaseInfo.add3))
            {
                UpdataFlg = true;
            }

            if (CardBaseInfo.action.Equals("1") && UpdataFlg)
            {
                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
                sqlhelp.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, CardBaseInfo.indate1);
                sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
                sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);

                SqlHelper sqlhelpdc = new SqlHelper();
                sqlhelpdc.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, CardBaseInfo.id);
                sqlhelpdc.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, CardBaseInfo.indate1);
                sqlhelpdc.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, CardBaseInfo.cardno);
                sqlhelpdc.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, CardBaseInfo.action);
                sqlhelpdc.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");
                DataTable dtCardDataChange = new DataTable();
                string strMsgID = string.Empty;
                if (BRM_CardDataChange.SearchByChange(sqlhelpdc.GetFilterCondition(), ref dtCardDataChange, ref strMsgID, "Newway"))
                {
                    if (dtCardDataChange.Rows.Count > 0)
                    {
                        //*修改異動單
                        Entity_CardDataChange CardDataChanges = new Entity_CardDataChange();
                        CardDataChanges.Sno = int.Parse(dtCardDataChange.Rows[0]["Sno"].ToString());
                        CardDataChanges.OutputFlg = "T";
                        SqlHelper sqlhelps = new SqlHelper();
                        sqlhelps.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, CardDataChanges.Sno.ToString());
                        blFlg = BRM_CardDataChange.update(CardDataChanges, sqlhelps.GetFilterCondition(), ref strMsgID, "OutputFlg");
                    }
                }
                blFlg = BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelp.GetFilterCondition(), "kind");
                return blFlg;
            }
        }
        return blFlg;
    }

    /// <summary>
    /// 功能說明:MergeTable加載製卡廠名稱
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/19
    /// 修改記錄:
    /// </summary>
    /// <param name="dtPost"></param>
    public string getKind(string strKind)
    {
        string strMsgID = string.Empty;
        DataTable dtKind = new DataTable();

        //*kind Name
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("06", "2", ref dtKind))
        {
            DataRow[] rowMerch_Code = dtKind.Select("PROPERTY_CODE='" + strKind + "'");
            if (rowMerch_Code != null && rowMerch_Code.Length > 0)
            {
                strKind = rowMerch_Code[0]["PROPERTY_NAME"].ToString();
                return strKind;
            }
        }

        //return "普掛";
        return "";
    }

    #region 更新OutputFlg
    /// <summary>
    /// 功能說明:更新OutputFlg为Y(已轉)
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/24
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDataChange"></param>
    /// <param name="dtUnableCard"></param>
    /// <param name="dtChangeCard"></param>
    /// <param name="dtFilePath"></param>
    public void UpdateOutputFlg()
    {
        DataRow[] drCardData = dtOutData.Select("TableName='tbl_Card_DataChange'");
        DataRow[] drUnCardData = dtOutData.Select("TableName='tbl_UnableCard'");
        DataRow[] drChangeCard = dtOutData.Select("TableName='tbl_CardChange'");
        try
        {
            foreach (DataRow rowDataChange in drCardData)
            {
                string strMsgID = string.Empty;
                Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
                string strFilePath = rowDataChange["LocalFilePath"].ToString();
                CardDataChange.ParentSno = BaseHelper.ObjToString(rowDataChange["PSNO"]);
                CardDataChange.OutputFlg = "Y";
                CardDataChange.FilePath = strFilePath;
                CardDataChange.OutputFileName = rowDataChange["TxtFileName"].ToString();
                CardDataChange.OutputDate = DateTime.Now.ToString("yyyy/MM/dd");
                SqlHelper sqlhelp = new SqlHelper();

                if (!String.IsNullOrEmpty(BaseHelper.ObjToString(rowDataChange["ID"])))
                {
                    sqlhelp.AddCondition(Entity_CardDataChange.M_id, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowDataChange["ID"]));
                }

                sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowDataChange["CardNo"]));
                sqlhelp.AddCondition(Entity_CardDataChange.M_action, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowDataChange["ACTION"]));
                sqlhelp.AddCondition(Entity_CardDataChange.M_indate1, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowDataChange["Indate1"]));
                sqlhelp.AddCondition(Entity_CardDataChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");

                if (BRM_CardDataChange.update(CardDataChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg", "OutputDate", "FilePath", "OutputFileName", "ParentSno"))
                {
                    UpdateBaseInfo(rowDataChange);
                }
            }
            foreach (DataRow rowUnableCard in drUnCardData)
            {
                string strFilePath = rowUnableCard["LocalFilePath"].ToString();
                string strMsgID = string.Empty;
                Entity_UnableCard UnableCard = new Entity_UnableCard();
                UnableCard.Sno = int.Parse(rowUnableCard["Sno"].ToString());
                UnableCard.ParentSno = BaseHelper.ObjToString(rowUnableCard["PSNO"]);
                UnableCard.OutputFlg = "Y";
                UnableCard.FilePath = strFilePath;
                UnableCard.OutputFileName = rowUnableCard["TxtFileName"].ToString();
                UnableCard.OutputDate = DateTime.Now.ToString("yyyy/MM/dd");
                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_UnableCard.M_Sno, Operator.Equal, DataTypeUtils.String, UnableCard.Sno.ToString());
                sqlhelp.AddCondition(Entity_UnableCard.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");


                if (BRM_UnableCard.update(UnableCard, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg", "OutputDate", "FilePath", "OutputFileName", "ParentSno"))
                {
                    //無法製卡轉出異動單後，回寫卡片基本資料的取卡方式為[9]卡片碎卡
                    Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
                    CardBaseInfo.kind = "9";
                    SqlHelper sqlhelps = new SqlHelper();

                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowUnableCard["Indate1"]));
                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowUnableCard["CardNo"]));
                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowUnableCard["ACTION"]));

                    BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelps.GetFilterCondition(), "kind");

                }

            }
            foreach (DataRow rowChangeCard in drChangeCard)
            {
                string strFilePath = rowChangeCard["LocalFilePath"].ToString();
                string strMsgID = string.Empty;
                Entity_CardChange CardChange = new Entity_CardChange();
                CardChange.Sno = int.Parse(rowChangeCard["Sno"].ToString());
                if (!string.IsNullOrEmpty(BaseHelper.ObjToString(rowChangeCard["PSNO"])))
                {
                    CardChange.ParentSno = Convert.ToInt32(BaseHelper.ObjToString(rowChangeCard["PSNO"]));
                }
                CardChange.OutputFlg = "Y";
                CardChange.FilePath = strFilePath;
                CardChange.OutputFileName = rowChangeCard["TxtFileName"].ToString();
                CardChange.OutputDate = DateTime.Now.ToString("yyyy/MM/dd");
                SqlHelper sqlhelp = new SqlHelper();
                sqlhelp.AddCondition(Entity_CardChange.M_Sno, Operator.Equal, DataTypeUtils.String, CardChange.Sno.ToString());
                sqlhelp.AddCondition(Entity_CardChange.M_OutputFlg, Operator.Equal, DataTypeUtils.String, "N");

                if (BRM_CardChange.update(CardChange, sqlhelp.GetFilterCondition(), ref strMsgID, "OutputFlg", "OutputDate", "FilePath", "OutputFileName", "ParentSno"))
                {
                    // 換卡轉出異動單後，回寫卡片基本資料的取卡方式為[10]卡片碎卡
                    Entity_CardBaseInfo CardBaseInfo = new Entity_CardBaseInfo();
                    CardBaseInfo.kind = "10";
                    SqlHelper sqlhelps = new SqlHelper();

                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowChangeCard["trandate"]));
                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowChangeCard["CardNo"]));
                    sqlhelps.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, BaseHelper.ObjToString(rowChangeCard["ACTION"]));

                    BRM_TCardBaseInfo.Update(CardBaseInfo, sqlhelps.GetFilterCondition(), "kind");
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region 將無法製卡檔、換卡異動檔要匯出的資料寫入異動記錄
    /// <summary>
    /// 功能說明:將無法製卡檔、換卡異動檔要匯出的資料寫入異動記錄
    /// </summary>
    private void WriteNoteCaptions()
    {
        DataRow[] drUnCardData = dtOutData.Select("TableName='tbl_UnableCard'");    //*無法制卡檔
        DataRow[] drChangeCard = dtOutData.Select("TableName='tbl_CardChange'");    //*換卡異動檔
        try
        {
            foreach (DataRow rowCardData in drUnCardData)
            {
                Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
                CardDataChange.indate1 = rowCardData["Indate1"].ToString();
                CardDataChange.id = rowCardData["ID"].ToString();
                CardDataChange.CardNo = rowCardData["CardNo"].ToString();
                CardDataChange.action = rowCardData["ACTION"].ToString();
                CardDataChange.Trandate = rowCardData["trandate"].ToString();
                CardDataChange.CNote = BRM_UnableCard.GetBlockCode(rowCardData["SNO"].ToString());
                CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChange.NoteCaptions = DateTime.Now.ToString("yyyy/MM/dd") + "," + Resources.JobResource.Job0000047;
                CardDataChange.UrgencyFlg = "0";
                //max 增加 
                //  CardChange.OutputDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChange.OutputDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChange.NewWay = "9";
                CardDataChange.OutputFileName = rowCardData["TxtFileName"].ToString();
                // CardDataChange.FilePath = rowCardData["TxtFileName"].ToString();

                //CardDataChange.OutputFlg = "T";
                CardDataChange.OutputFlg = "Y";
                BRM_CardDataChange.AddNewEntity(CardDataChange);
            }
            foreach (DataRow rowChangeCard in drChangeCard)
            {
                Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
                CardDataChange.indate1 = rowChangeCard["Indate1"].ToString();
                CardDataChange.id = rowChangeCard["ID"].ToString();
                CardDataChange.CardNo = rowChangeCard["CardNo"].ToString();
                CardDataChange.action = rowChangeCard["ACTION"].ToString();
                CardDataChange.Trandate = rowChangeCard["trandate"].ToString();
                CardDataChange.CNote = BRM_CardChange.GetBlockCode(rowChangeCard["SNO"].ToString());
                CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                CardDataChange.NoteCaptions = DateTime.Now.ToString("yyyy/MM/dd") + "," + Resources.JobResource.Job0000048;
                CardDataChange.UrgencyFlg = "0";
                //CardDataChange.OutputFlg = "T";
                CardDataChange.OutputFlg = "Y";
                BRM_CardDataChange.AddNewEntity(CardDataChange);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    #endregion

    #region OASA注銷
    private void CancelOASA(Quartz.JobExecutionContext context, DataTable dtFileInfo)
    {
        DataRow[] RowOASACardBackInfo = dtOutData.Select("Newway ='11'");
        if (RowOASACardBackInfo.Length > 0)
        {
            int intSOASACount = 0;
            int intFOASACount = 0;
            string strCancelOASAFile = string.Empty;
            string strOASAUserId = string.Empty;

            string strBlkCode = ""; //rowFileInfo1[0]["BLKCode"].ToString().Trim();
            string strMemo = ""; //rowFileInfo1[0]["MEMO"].ToString().Trim();
            string strReasonCode = "";// rowFileInfo1[0]["ReasonCode"].ToString().Trim();
            string strActionCode = "";// rowFileInfo1[0]["ActionCode"].ToString().Trim();
            if (dtFileInfo.Rows.Count > 0)
            {
                strBlkCode = dtFileInfo.Rows[0]["BLKCode"].ToString().Trim();
                strMemo = dtFileInfo.Rows[0]["MEMO"].ToString().Trim();
                strReasonCode = dtFileInfo.Rows[0]["ReasonCode"].ToString().Trim();
                strActionCode = dtFileInfo.Rows[0]["ActionCode"].ToString().Trim();
            }


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
                        strSFFlg = "0";
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
                CancelOASA.CancelOASASource = "4";
                SetCancelOASA.Add(CancelOASA);
                if (BRM_CancelOASA.BatInsert(SetCancelOASA))
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

    private bool UpdateBaseInfo(DataRow drDetail)
    {

        Entity_CardBaseInfo BaseInfo = new Entity_CardBaseInfo();
        ArrayList UpdateColName = new ArrayList();
        try
        {
            // 增加自動抓取同卡號、同類別的最新製卡日 (解決客服新增沒有此欄位資訊) Max
            if (String.IsNullOrEmpty(drDetail["indate1"].ToString()))
            {
                BaseInfo.indate1 = " (select top 1   indate1 from tbl_Card_BaseInfo (nolock) where cardno='" + drDetail["CardNo"].ToString() + "' and ACTION='" + drDetail["ACTION"].ToString() + "' order by indate1 desc ) ";
            }
            else
            {
                BaseInfo.indate1 = "'" + drDetail["indate1"].ToString().Trim() + "'";
            }

            //BaseInfo.id = drDetail["id"].ToString();
            BaseInfo.cardno = drDetail["CardNo"].ToString();
            BaseInfo.action = drDetail["ACTION"].ToString();

            SqlHelper sqlhelp = new SqlHelper();
            //sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, BaseInfo.id);
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.Integer, BaseInfo.indate1);
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, BaseInfo.cardno);
            sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, BaseInfo.action);

            //*姓名
            if (!String.IsNullOrEmpty(drDetail["NewName"].ToString().Trim()) && !drDetail["NewName"].ToString().Equals("NULL"))
            {
                BaseInfo.custname = drDetail["NewName"].ToString().Trim();
                UpdateColName.Add("custname");
            }

            //*新額度
            if (!String.IsNullOrEmpty(drDetail["Newmonlimit"].ToString().Trim()) && !drDetail["Newmonlimit"].ToString().Equals("NULL"))
            {
                BaseInfo.monlimit = drDetail["Newmonlimit"].ToString().Trim();
                UpdateColName.Add("monlimit");
            }

            //*新取卡方式
            if (!String.IsNullOrEmpty(drDetail["Newway"].ToString().Trim()) && !drDetail["Newway"].ToString().Equals("NULL"))
            {
                BaseInfo.kind = drDetail["Newway"].ToString().Trim();
                UpdateColName.Add("kind");
            }

            //*新掛號號碼
            if (!String.IsNullOrEmpty(drDetail["Newmailno"].ToString().Trim()) && !drDetail["Newmailno"].ToString().Equals("NULL"))
            {
                BaseInfo.mailno = drDetail["Newmailno"].ToString().Trim();
                UpdateColName.Add("mailno");
            }

            //*郵寄日期
            if (!String.IsNullOrEmpty(drDetail["MailDate"].ToString().Trim()) && !drDetail["MailDate"].ToString().Equals("NULL"))
            {
                BaseInfo.maildate = drDetail["MailDate"].ToString().Trim();
                UpdateColName.Add("maildate");
            }

            //*ZIP
            if (!String.IsNullOrEmpty(drDetail["ZIP"].ToString().Trim()) && !drDetail["ZIP"].ToString().Equals("NULL"))
            {
                BaseInfo.zip = drDetail["ZIP"].ToString().Trim();
                UpdateColName.Add("zip");
            }

            //*地址一
            if (!String.IsNullOrEmpty(drDetail["Add1"].ToString().Trim()) && !drDetail["Add1"].ToString().Equals("NULL"))
            {
                BaseInfo.add1 = drDetail["Add1"].ToString().Trim();
                UpdateColName.Add("add1");
            }

            //*地址二
            if (!String.IsNullOrEmpty(drDetail["Add2"].ToString().Trim()) && !drDetail["Add2"].ToString().Equals("NULL"))
            {
                BaseInfo.add2 = drDetail["Add2"].ToString().Trim();
                UpdateColName.Add("add2");
            }

            //*地址三
            //修正當異動地址時，址三為空白時，就不回壓的問題 Wallace
            //if (!String.IsNullOrEmpty(drDetail["Add3"].ToString().Trim()) && !drDetail["Add3"].ToString().Equals("NULL"))
            if (!String.IsNullOrEmpty(drDetail["Add2"].ToString().Trim()) && !drDetail["Add2"].ToString().Equals("NULL"))
            {
                BaseInfo.add3 = drDetail["Add3"].ToString().Trim();
                UpdateColName.Add("add3");
            }

            int iItem = UpdateColName.Count;
            string[] UpdColsName = new string[iItem];
            for (int icount = 0; icount < UpdateColName.Count; icount++)
            {
                UpdColsName[icount] = UpdateColName[icount].ToString();
            }
            // 增加自動抓取同卡號、同類別的最新製卡日 (解決客服新增沒有此欄位資訊)，因為改寫要多處理字串的問題  Max
            String SQLtemp = sqlhelp.GetFilterCondition();
            SQLtemp = SQLtemp.Replace("''", "'");
            //BRM_TCardBaseInfo.Update(BaseInfo, sqlhelp.GetFilterCondition(), UpdColsName);
            BRM_TCardBaseInfo.Update(BaseInfo, SQLtemp, UpdColsName);
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #region 掛號號碼異動
    /// <summary>
    /// 功能說明:掛號號碼異動
    /// 作    者:Simba Liu
    /// 創建時間:2010/05/24
    /// 修改記錄:
    /// </summary>
    /// <param name="rowDataChange"></param>
    /// <returns></returns>
    public bool FaxNO(DataRow[] rowDataChange)
    {
        bool blnResult = true;
        foreach (DataRow row in rowDataChange)
        {
            if (string.IsNullOrEmpty(row["newMailno"].ToString()))
            {
                blnResult = false;
                break;
            }
        }
        return blnResult;
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
        string strStatus = string.Empty;
        string strMessage = string.Empty;
        DataRow[] rowStatus = dtLocalFile.Select("UploadStates='F'");
        //*匯出成功
        if (rowStatus != null && rowStatus.Length > 0)
        {
            strStatus = "F";
            strMessage = Resources.JobResource.Job010201;
        }
        //*匯出失敗
        else
        {
            strStatus = "S";
            strMessage = Resources.JobResource.Job010200;
        }
        EntityM_LBatchLog LBatchLog = new EntityM_LBatchLog();
        LBatchLog.FUNCTION_KEY = "06";
        LBatchLog.JOB_ID = "0104";
        LBatchLog.START_TIME = StartTime.ToShortTimeString();
        LBatchLog.END_TIME = EndTime.ToShortTimeString();
        LBatchLog.STATUS = strStatus;
        LBatchLog.RETURN_MESSAGE = strMessage;
        BRM_LBatchLog.insert(LBatchLog);
    }
    #endregion

    #region mail警訊通知
    /// <summary>
    /// 功能說明:mail警訊通知
    /// 作    者:HAO CHEN
    /// 創建時間:2010/06/11
    /// 修改記錄:
    /// </summary>
    /// <param name="strCallType">Mail警訊種類</param>
    /// <param name="strCallType">Mail警訊內文</param>
    /// <param name="strCallType">錯誤狀況</param>
    public void SendMail(string strCallType, ArrayList alMailInfo, string strErrorName)
    {
        DataTable dtCallMail = new DataTable();
        string strMsgID = string.Empty;
        SqlHelper sqlhelp = new SqlHelper();
        sqlhelp.AddCondition(EntityM_CallMail.M_JobID, Operator.Equal, DataTypeUtils.String, strJobId);
        sqlhelp.AddCondition(EntityM_CallMail.M_ConditionID, Operator.Equal, DataTypeUtils.String, strCallType);
        BRM_CallMail.SearchMailByNo(sqlhelp.GetFilterCondition(), ref dtCallMail, ref strMsgID);
        if (null != dtCallMail && dtCallMail.Rows.Count > 0)
        {
            string strDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strFrom = UtilHelper.GetAppSettings("MailSender");
            string[] strTo = new string[] { };
            string[] strCc = new string[] { };
            string strSubject = string.Empty;
            string strBody = string.Empty;

            switch (strCallType)
            {
                case "1":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');
                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();
                    strSubject = string.Format(strSubject, Resources.JobResource.Job0000104, alMailInfo[0]);
                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], Resources.JobResource.Job0000104, strErrorName);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
                case "5":
                    strTo = dtCallMail.Rows[0]["ToUsers"].ToString().Split(';');
                    strCc = dtCallMail.Rows[0]["CcUsers"].ToString().Split(';');

                    //格式化Mail Tittle
                    strSubject = dtCallMail.Rows[0]["MailTittle"].ToString();

                    //格式化Mail Body
                    strBody = dtCallMail.Rows[0]["MailContext"].ToString();
                    strBody = string.Format(strBody, strDateTime, alMailInfo[0], alMailInfo[1]);
                    //發送Mail
                    JobHelper.SendMail(strTo, strCc, strFrom, strSubject, strBody);
                    break;
            }
        }
    }
    #endregion

}
