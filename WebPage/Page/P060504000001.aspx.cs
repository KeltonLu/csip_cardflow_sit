//******************************************************************
//*  功能說明：郵局退件資料查詢
//*  作    者：Simba Liu
//*  創建日期：2010/06/17
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using EntityLayer;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Cryptography;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using Framework.Data.OM.Collections;
using System.Configuration;
using System.IO;
using CSIPCommonModel.EntityLayer;

public partial class P060504000001 : PageBase
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
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtid.Text;
        log.Account_Nbr = this.txtCardno.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsgID = string.Empty;

        if (!CheckCondition(ref strMsgID))
        { 
            MessageHelper.ShowMessage(this, strMsgID); 
            return;
        }

        try
        {
            #region 查詢條件參數
            Dictionary<string, string> param = new Dictionary<string, string>();

            // 退件日期起日
            String backDateStart = txtBackdateStart.Text.Trim();
            if (txtBackdateStart.Text.Trim().Equals(""))
                backDateStart = "NULL";
            param.Add("backDateStart",backDateStart);

            // 退件日期訖日
            String backDateEnd = txtBackdateEnd.Text.Trim();
            if (txtBackdateEnd.Text.Trim().Equals(""))
                backDateEnd = "NULL";
            param.Add("backDateEnd",backDateEnd);

            // 結案日期起日
            String closeDateStart = txtClosedateStart.Text.Trim();
            if (txtClosedateStart.Text.Trim().Equals(""))
                closeDateStart = "NULL"; 
            param.Add("closeDateStart",closeDateStart);
            
            // 結案日期訖日
            String closeDateEnd = txtClosedateEnd.Text.Trim();
            if (txtClosedateEnd.Text.Trim().Equals(""))
                closeDateEnd = "NULL"; 
            param.Add("closeDateEnd",closeDateEnd);

            // 退件流水編號
            String serialNo = txtserial_no.Text.Trim();
            if (txtserial_no.Text.Trim().Equals(""))
                serialNo = "NULL"; 
            param.Add("serial_no",serialNo);
            
            // 身份證字號 
            String id = txtid.Text.Trim();
            if (txtid.Text.Trim().Equals(""))
                id = "NULL";
            param.Add("id",id);
            
            // 卡號
            String cardNo = txtCardno.Text.Trim();
            if (txtCardno.Text.Trim().Equals(""))
                cardNo = "NULL";
            param.Add("cardNo",cardNo);

            #endregion
            
            string strServerPathFile = this.Server.MapPath(ConfigurationManager.AppSettings["ExportExcelFilePath"].ToString());

            //產生報表
            bool result = BR_Excel_File.CreateExcelFile_0504Report(param, ref strServerPathFile, ref strMsgID);
            
            if (result)
            {
                FileInfo fs = new FileInfo(strServerPathFile);
                Session["ServerFile"] = strServerPathFile;
                Session["ClientFile"] = fs.Name;
                string urlString = @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
            else {
                MessageHelper.ShowMessage(this, strMsgID);
            }

        }
        catch (Exception exp)
        {
            //this.ReportViewer0504.Visible = false;
            Logging.Log(exp, LogLayer.BusinessRule);
            MessageHelper.ShowMessage(this, "06_06050400_003");

        }
    }

    /// <summary>
    /// 功能說明:頁面數據驗證
    /// 作    者:Simba Liu
    /// 創建時間:2010/06/17
    /// 修改記錄:
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public bool CheckCondition(ref string strMsgID)
    {
        //*退件流水編號
        if (ValidateHelper.IsChinese(this.txtserial_no.Text.Trim()))
        {
            strMsgID = "06_06050400_000";
            txtserial_no.Focus();
            return false;
        }
        //*身份證字號
        if (ValidateHelper.IsChinese(this.txtid.Text.Trim()))
        {
            strMsgID = "06_06050400_001";
            txtid.Focus();
            return false;
        }
        //*卡號
        if (ValidateHelper.IsChinese(this.txtCardno.Text.Trim()))
        {
            strMsgID = "06_06050400_002";
            txtCardno.Focus();
            return false;
        }


        return true;
    }
}
