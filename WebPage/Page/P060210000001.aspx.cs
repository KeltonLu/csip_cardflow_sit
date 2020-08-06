using System;
using System.Data;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.IO;
using System.Collections.Generic;
using System.Text;

public partial class Page_P060210000001 : PageBase
{
    #region table
    JobHelper jobHelper = new JobHelper();

    #endregion

    #region 事件
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {



        }
    }

    /// <summary>
    /// 功能說明:資料上傳
    /// 作    者:zhen chen
    /// 創建時間:2010/07/06
    /// 修改記錄: 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            string strLocalPath = string.Empty;                             //文件路徑
            string strFileName = string.Empty;                              //文件名稱
            string strFuncName = BaseHelper.GetShowText("06_06030210_000"); //功能名稱
            string strCardType = string.Empty;                              //卡片類別
            string strFactory = string.Empty;                               //廠商
            string strTimeFlag = string.Empty;                              //標記上午或下午時間
            string strFileType = string.Empty;                                //文檔類別
            string strMsgID = string.Empty;                                 //記錄錯誤ID
            AutoImportFiles getTrandate = new AutoImportFiles();
            int CountAll = 0;       //總筆數
            int CountSucess = 0;    //成功數
            int CountFail = 0;      //失敗數
            string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
            string strLogMsg = BaseHelper.GetShowText("06_06030210_000") + "：" + BaseHelper.GetShowText("06_06021000_003");
            BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "A");

            if (fupFile.HasFile)
            {
                if (fupFile.PostedFile.ContentLength <= 0)
                {
                    //MessageHelper.ShowMessage(this.UpdatePanel1, "06_06030200_022");
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_022") + "');");
                    return;
                }
                strLocalPath = this.fupFile.PostedFile.FileName;
                strFileName = this.fupFile.FileName;
                Entity_CardBaseInfo eCardBaseInfo = new Entity_CardBaseInfo();

                ////檢核檔名  只限當日檔名
                string strFDate = strFileName.Substring(0, 8);
                //string strToday = DateTime.Today.ToString("yyyyMMdd");
                //string strBatNO = strFileName.Substring(8, 3);
                //if (strFDate != strToday)
                //{
                //    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06021000_002") + "');");
                //    jobHelper.SaveLog(strFileName + "06_06021000_002");
                //    return;
                //}
                string strServerPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath");
                strServerPath = strServerPath + "\\" + strFileName;
                // 檢查文檔是否已經存在
                if (File.Exists(strServerPath))
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06021000_001") + "');");
                    jobHelper.SaveLog(strFileName + "06_06021000_001");
                    return;
                }
                string strFilePath = "";// FileUpload(fupFile.PostedFile, ref strMsgID);
                int[] kindColl = new int[] { 0, 1, 3, 5 };
                //載入檔案內容
                //載入檔案內容
                string[] fileLine = File.ReadAllLines(strFilePath, Encoding.Default);
                //檢核同一檔案是否有相同ID
                List<string> ImpIDColl = new List<string>();

                string strUserName = "";
                BRM_User.GetUserName(((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, ref strUserName);

                foreach (string strObj in fileLine)
                {
                    CountAll++;
                    //空白行不處理
                    if (strObj.Length < 1)
                    {
                        continue;
                    }
                    BatchImport_UrgencyCard tmpObj = new BatchImport_UrgencyCard();
                    string[] tmpstr = strObj.Split('\t');
                    tmpObj.indate1 = strFDate.Substring(0, 4) + "/" + strFDate.Substring(4, 2) + "/" + strFDate.Substring(6, 2);  // DateTime.Today.ToString("yyyy/MM/dd");
                                                                                                                                  //  tmpObj.kind = strBatNO;
                    tmpObj.id = tmpstr[1];
                    tmpObj.kind = tmpstr[2].Substring(0, 1);
                    tmpObj.card_file = strFileName;
                    tmpObj.import_time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    tmpObj.import_user = strUpUser;
                    tmpObj.cardno = "";
                    //檢查郵寄別
                    switch (tmpObj.kind)
                    {
                        case "0":
                        case "1":
                        case "3":
                        case "5":
                            break;
                        default:
                            tmpObj.result = "失敗";
                            tmpObj.fail_reason = "取卡方式有誤(限1自取,0普掛,3限掛,5快遞)";
                            tmpObj.reason = "取卡方式(限1自取,0普掛,3限掛,5快遞)";
                            WriteToImportLog(tmpObj);
                            CountFail++;
                            continue;
                    }
                    //檢核是否有相同ID
                    if (ImpIDColl.IndexOf(tmpObj.id) > -1)
                    {
                        tmpObj.result = "失敗";
                        tmpObj.fail_reason = "資料重複";
                        tmpObj.reason = "同一批匯入檔ID重複";
                        WriteToImportLog(tmpObj);
                        CountFail++;
                        continue;
                    }

                    ImpIDColl.Add(tmpObj.id);

                    //因為需要單筆處理
                    DataTable dtCardBaseInfo = new DataTable();
                    ///取得所有BASEINFO
                    BRCard_BaseInfo.GetBaseinfoByID(tmpObj.id, tmpObj.indate1, ref dtCardBaseInfo, ref strMsgID);

                    //制卡檔無此身分證字號
                    if (dtCardBaseInfo.Rows.Count == 0)
                    {
                        tmpObj.result = "失敗";
                        tmpObj.fail_reason = strFDate + "製卡檔無此身分證字號(限正卡ID)";
                        tmpObj.reason = "附卡ID 或\"未核卡\"";
                        WriteToImportLog(tmpObj);
                        CountFail++;
                        continue;
                    }
                    //以「製卡日」、「身分證字號」及「取卡方式」等三個值做檢核條件，查詢tbl_Card_DataChange是否有相符合之資料
                    if (BRBatchImport_UrgencyCard.IsExistCard_DataChange(tmpObj))
                    {
                        tmpObj.result = "失敗";
                        tmpObj.fail_reason = "資料已存在";
                        tmpObj.reason = "已存在緊急製卡異動資料";
                        WriteToImportLog(tmpObj);
                        CountFail++;
                        continue;
                    }
                    //填寫結果，原因，卡號  需逐筆處理 ，但前頭是相同的
                    foreach (DataRow dr in dtCardBaseInfo.Rows)
                    {
                        //新增異動單
                        Entity_CardDataChange CardDataChange = new Entity_CardDataChange();
                        CardDataChange.action = dr["action"].ToString();
                        CardDataChange.id = tmpObj.id;
                        CardDataChange.indate1 = tmpObj.indate1;
                        CardDataChange.CardNo = dr["cardno"].ToString();
                        CardDataChange.Trandate = dr["trandate"].ToString();
                        CardDataChange.OldWay = dr["kind"].ToString();//*取卡方式只能寫入Code
                        CardDataChange.NewWay = tmpObj.kind;
                        CardDataChange.UrgencyFlg = "1"; //緊急製卡，固定值
                        string strUrgencyFlg = MessageHelper.GetMessage("06_06020104_007");
                        CardDataChange.CNote = "批次匯入";
                        CardDataChange.NoteCaptions = MessageHelper.GetMessage("06_06020104_004", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id, DateTime.Now.ToString("yyyy/MM/dd"), dr["kind"].ToString(), tmpObj.kind + strUrgencyFlg, strUserName);//*異動記錄說明
                        CardDataChange.UpdDate = DateTime.Now.ToString("yyyy/MM/dd");
                        CardDataChange.UpdTime = DateTime.Now.ToString("HH:mm");
                        CardDataChange.BaseFlg = "1";
                        CardDataChange.OutputFlg = "N";
                        CardDataChange.UpdUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
                        bool blnResult = true;
                        blnResult = BRM_CardDataChange.Insert(CardDataChange, ref strMsgID);
                        strLogMsg = BaseHelper.GetShowText("06_06020101_035") + "：" + BaseHelper.GetShowText("06_06020101_058");
                        BRM_Log.Insert(CardDataChange.UpdUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
                        //成功
                        if (blnResult)
                        {
                            tmpObj.cardno = dr["cardno"].ToString();
                            tmpObj.result = "成功";
                            tmpObj.fail_reason = "";
                            tmpObj.reason = "";
                            WriteToImportLog(tmpObj);
                            CountSucess++;
                        }
                        else //寫入資料庫失敗
                        {
                            tmpObj.cardno = dr["cardno"].ToString();
                            tmpObj.result = "失敗";
                            tmpObj.fail_reason = "資料庫寫入失敗";
                            tmpObj.reason = "資料庫寫入失敗";
                            WriteToImportLog(tmpObj);
                            CountFail++;
                        }
                    }

                }
                jsBuilder.RegScript(this.Page, "alert('總匯入筆數:" + CountAll.ToString() + "\\r\\n 成功數:" + CountSucess.ToString() + "\\r\\n 失敗數:" + CountFail.ToString() + "');");
                jobHelper.SaveLog(strFileName + "06_06030200_023");
            }
            //上傳的文件不存在
            else
            {
                jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("06_06030200_014") + "');");
                jobHelper.SaveLog(strFileName + "06_06030200_014");
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return;
        }

    }

    #endregion
    #region 方法

    private bool WriteToImportLog(BatchImport_UrgencyCard insObj)
    {
        return BRBatchImport_UrgencyCard.Insert(insObj);
    }

    #endregion
}
