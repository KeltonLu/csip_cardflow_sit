//******************************************************************
//*  功能說明：簡訊發送查詢
//*  作    者：Simba Liu
//*  創建日期：2010/06/22
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using CSIPCommonModel.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

public partial class P060507000001 : PageBase
{

    //Talas 20191003 SOC修改
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    /// <summary>
    /// 功能說明:頁面加載事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/09
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //Talas 20191003 SOC修改
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }
    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        {
            return;
        }
        try
        {

            // 初始化報表參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 身份證字號
            String ID = this.txtID.Text.Trim().Equals("") ? "NULL" : this.txtID.Text.Trim();
            param.Add("ID", ID);

            // 處理日期起日
            String ProcessDateStart  = this.txtProcessDateStart.Text.Trim().Equals("") ? "NULL" : this.txtProcessDateStart.Text.Trim();
            param.Add("ProcessDateStart", ProcessDateStart);

            // 處理日期訖日
            String ProcessDateEnd = this.txtProcessDateEnd.Text.Trim().Equals("") ? "NULL" : this.txtProcessDateEnd.Text.Trim();
            param.Add("ProcessDateEnd", ProcessDateEnd);

            // 交寄日期起日
            String MaildateStart = this.txtMaildateStart.Text.Trim().Equals("") ? "NULL" : this.txtMaildateStart.Text.Trim();
            param.Add("MaildateStart", MaildateStart);

            // 交寄日期訖日
            String MaildateEnd = this.txtMaildateEnd.Text.Trim().Equals("") ? "NULL" : this.txtMaildateEnd.Text.Trim();
            param.Add("MaildateEnd", MaildateEnd);

            // 掛號號碼
            String Mailno = this.txtMailno.Text.Trim().Equals("") ? "NULL" : this.txtMailno.Text.Trim();
            param.Add("Mailno", Mailno);

            // 狀態
            String Status = this.dropStatus.SelectedValue.Equals("XX") ? "NULL" : this.dropStatus.SelectedValue;
            param.Add("Status", Status);

            string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0507Report(param, ref strServerPathFile, ref strMsgID);

            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else
            {
                MessageHelper.ShowMessage(this, strMsgID);
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");

        }
    }
    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/22
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*身份證字號
        if (ValidateHelper.IsChinese(this.txtID.Text.Trim()))
        {
            strMsgID = "06_06050400_001";
            txtID.Focus();
            return false;
        }
        //*掛號號碼
        if (ValidateHelper.IsChinese(this.txtMailno.Text.Trim()))
        {
            strMsgID = "06_06050400_002";
            txtMailno.Focus();
            return false;
        }
        return true;
    }
}
