//******************************************************************
//*  功能說明：VD卡整批結案 
//*  作    者：zhiyuan
//*  創建日期：2010/05/31
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using Framework.Common.Logging;
using Framework.WebControls;
using BusinessRules;
using Framework.Common.Message;
using Framework.Common.Utility;
using System.Data.SqlClient;

public partial class P060208000001 : PageBase
{
    private string FunctionKey = "06";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvResult.Visible = false;
            InitPage();
        }
    }

    /// <summary>
    /// 功能說明:標題列印
    /// 作    者:zhiyuan
    /// 創建時間:2010/05/31
    /// 修改記錄:
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.grvResult.Columns[0].HeaderText = BaseHelper.GetShowText("06_06020800_005");
        this.grvResult.Columns[1].HeaderText = BaseHelper.GetShowText("06_06020800_006");
        this.grvResult.Columns[2].HeaderText = BaseHelper.GetShowText("06_06020800_007");

        //*顯示按鈕默認按鈕的中文名稱
        this.btnBatch.Text = BaseHelper.GetShowText(this.btnBatch.Text);

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvResult.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (!PageValueCheck())
        {
            return;
        }
        BindGridView();
    }

    /// <summary>
    /// 功能說明:綁定查詢結果
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/02
    /// 修改記錄:
    /// </summary>
    private void BindGridView()
    {
        int iTotalCount = 0;
        string strMsgID = string.Empty;
        DataTable dtResult = new DataTable();
        try
        {
            char cTranddateOrBackdate = char.Parse(ViewState["TranddateOrBackdate"].ToString());
            string strSearchDate = ViewState["SearchDate"].ToString();
            IsAlert.Value = string.Empty;
            ShowMsg.Value = string.Empty;
            if (SearchVDCard(cTranddateOrBackdate, strSearchDate, ref dtResult, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref iTotalCount, ref strMsgID))
            {
                //* 查詢成功
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                if (dtResult.Rows.Count > 0)
                {
                    this.grvResult.Visible = true;
                    this.btnBatch.Enabled = true;
                    this.grvResult.DataSource = dtResult;
                    this.grvResult.DataBind();

                    if (BRM_CardBackInfo.CheckBackDate(cTranddateOrBackdate, strSearchDate))
                    {
                        IsAlert.Value = "y";
                        ShowMsg.Value = MessageHelper.GetMessage("06_06020800_006");
                    }
                }
                else
                {
                    //查無資料
                    this.grvResult.Visible = false;
                    this.btnBatch.Enabled = false;
                }
            }
            else
            {
                //* 查詢失敗
                this.gpList.RecordCount = 0;
                this.gpList.Visible = false;
                this.grvResult.DataSource = null;
                this.grvResult.DataBind();
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                return;
            }
        }
        catch (System.Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            MessageHelper.ShowMessage(this.UpdatePanel1, "06_06020800_001");
            return;
        }
    }
    
    /// <summary>
    /// 功能說明:查詢VD卡整批結案資料
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    /// <param name="TrandateOrBackDate">退件日/轉當日(B/T)</param>
    /// <param name="Date">查詢日期</param>
    /// <param name="dtResult">查詢結果</param>
    /// <param name="iPageIndex"></param>
    /// <param name="iPageSize"></param>
    /// <param name="iTotalCount"></param>
    /// <param name="strMsgID"></param>
    /// <returns>成功/失敗</returns>
    public static bool SearchVDCard(char TrandateOrBackDate, string Date, ref DataTable dtResult, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
    {
        try
        {
            string colName = string.Empty;
            string sqlText = string.Empty;
            string SEL_TRANDATE_OR_BACKDATE = @"SELECT b.cardno, c.cardtype, b.Backdate,b.id,b.ACTION FROM tbl_Card_BackInfo (nolock) AS b left JOIN tbl_Card_BaseInfo (nolock)  AS c on b.CardNo=c.CardNo where b.{0} <= @Date AND cast(CardBackStatus as char(1))= '0' and b.Cardtype in (select CardType from tbl_CardType where BankCardType='2')  order by b.Backdate desc";
            switch (TrandateOrBackDate)
            {
                case 'T':
                    colName = "Trandate";
                    break;
                case 'B':
                    colName = "Backdate";
                    break;
            }

            sqlText = String.Format(SEL_TRANDATE_OR_BACKDATE, colName);
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlText;
            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmDate = new SqlParameter("@Date", Date);
            sqlcmd.CommandTimeout = 240;
            sqlcmd.Parameters.Add(parmDate);

            DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
            if (ds != null)
            {
                dtResult = ds.Tables[0];
                SetCardType(ref dtResult, "cardtype");
                strMsgID = "06_06020800_002";
                return true;
            }
            else
            {
                strMsgID = "06_06020800_003";
                return true;
            }
        }
        catch (System.Exception ex)
        {
            BRM_CardBackInfo.SaveLog(ex.Message);
            strMsgID = "06_06020800_001";
            return false;
        }

    }

    /// <summary>
    /// 功能說明:將DataTable中的CardType Code轉換為中文
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/02
    /// 修改記錄:
    /// </summary>
    /// <param name="dtDetail"></param>
    /// <param name="strCardType"></param>
    /// <returns></returns>
    public static void SetCardType(ref DataTable dtDetail, string strCardType)
    {
        DataTable dtCardType = new DataTable();

        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetProperty("06", "19", ref dtCardType))
        {
            for (int i = 0; i < dtDetail.Rows.Count; i++)
            {
                DataRow[] dr = dtCardType.Select("PROPERTY_CODE='" + dtDetail.Rows[i][strCardType].ToString() + "'");
                if (dr.Length > 0)
                {
                    dtDetail.Rows[i][strCardType] = dr[0]["PROPERTY_NAME"].ToString();
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// 功能說明:整批結案
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/02
    /// 修改記錄:
    /// </summary>
    /// <param name="TrandateOrBackDate">退件日/轉當日(B/T)</param>
    /// <param name="SearchDate">查詢日期</param>
    /// <param name="AgentId">當前使用者ID</param>
    /// <returns></returns>
    public static bool UpdateBackStatus(char TrandateOrBackDate, string SearchDate, string AgentId)
    {
        try
        {
            string colName = string.Empty;
            string sqlText = string.Empty;
            string strSysDate = DateTime.Now.ToString("yyyy/MM/dd");
            string UPD_BACKSTATUS = @"UPDATE tbl_Card_BackInfo Set Closedate=@SysDate,Enduid=@ID,Enddate=@SysDate,Enditem='6',CardBackStatus='2' , endFunction='1'  WHERE {0}<=@SearchDate AND cast(CardBackStatus as char(1))= '0'  and Cardtype in (select CardType from tbl_CardType where BankCardType='2') ";
            switch (TrandateOrBackDate)
            {
                case 'T':
                    colName = "Trandate";
                    break;
                case 'B':
                    colName = "Backdate";
                    break;
            }

            sqlText = String.Format(UPD_BACKSTATUS, colName);
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandTimeout = 240;
            sqlcmd.CommandText = sqlText;
            sqlcmd.CommandType = CommandType.Text;
            SqlParameter parmSysDate = new SqlParameter("@SysDate", strSysDate);
            sqlcmd.Parameters.Add(parmSysDate);
            SqlParameter parmAgentId = new SqlParameter("@ID", AgentId);
            sqlcmd.Parameters.Add(parmAgentId);
            SqlParameter parmSearchDate = new SqlParameter("@SearchDate", SearchDate);
            sqlcmd.Parameters.Add(parmSearchDate);

            if (BRM_CardBackInfo.Update(sqlcmd))
            {
                return true;
            }
            return false;
        }
        catch (System.Exception ex)
        {
            BRM_CardBackInfo.SaveLog(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 功能說明:結案事件
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBatch_Click(object sender, EventArgs e)
    {
        if (!PageValueCheck())
        {
            return;
        }

        char cTranddateOrBackdate = char.Parse(ViewState["TranddateOrBackdate"].ToString());
        string strSearchDate = ViewState["SearchDate"].ToString();
        string strAgID = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        
        string strUpUser = ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"]).agent_id;
        string strLogMsg = BaseHelper.GetShowText("06_06020800_000");
        BRM_Log.Insert(strUpUser, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strLogMsg, "U");
        
        if (UpdateBackStatus(cTranddateOrBackdate, strSearchDate, strAgID))
        {
            //*成功
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020800_004");
            BindGridView();
        }
        else
        {
            //*失敗
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020800_005");
        }  
    }

    /// <summary>
    /// 功能說明:分頁事件
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// <summary>
    /// 功能說明:初始化頁面控件
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    private void InitPage()
    {
        //初始化下拉VDCard下拉列表
        DataTable dtVDCard = null;
        if (CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(FunctionKey, "11", ref dtVDCard))
        {
            for (int i = 0; i < dtVDCard.Rows.Count; i++)
            {
                this.dropVDCard.Items.Insert(i, new ListItem(dtVDCard.Rows[i]["PROPERTY_NAME"].ToString(), dtVDCard.Rows[i]["PROPERTY_CODE"].ToString()));
            }
            this.dropVDCard.SelectedIndex = 0;
        }

        //帶入系統日期4個月1天前
        this.dpDate.Text = DateTime.Now.AddMonths(-4).ToString("yyyy/MM/dd");
    }

    /// <summary>
    /// 功能說明:驗證頁面輸入控件
    /// 作    者:zhiyuan
    /// 創建時間:2010/06/01
    /// 修改記錄:
    /// </summary>
    private bool PageValueCheck()
    {
        if (this.dpDate.Text.Equals(""))
        {
            MessageHelper.ShowMessage(UpdatePanel1, "06_06020800_000");
            return false;
        }

        ViewState["TranddateOrBackdate"] = dropVDCard.SelectedValue;
        ViewState["SearchDate"] = dpDate.Text;
        return true;
    }
}
