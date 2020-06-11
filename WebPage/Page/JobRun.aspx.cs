//******************************************************************
//*  作    者：chaoma(Wilson)
//*  功能說明：立即運行JOB
//*  創建日期：2009/08/03
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* chaoma           2010/07/29     20100009          共用模組增加手動執行 JOB功能
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.JavaScript;
using System.Text.RegularExpressions;
using Framework.Common.Cryptography;
using Quartz;
using System.Threading;
using Quartz.Impl;
public partial class Page_JobRun : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //* jobID
        string strjobID = RedirectHelper.GetDecryptString(this.Page, "JobID");

        if (string.IsNullOrEmpty(strjobID))
        {
            MessageHelper.ShowMessage(this.Page, "00_00000000_037");
        }
            //* 新建排程
            ISchedulerFactory sfr = new StdSchedulerFactory();
            IScheduler schedr = sfr.GetScheduler();


            DateTime runTimer = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow);
            runTimer = DateTime.Parse("2010-04-22 18:06:00").ToUniversalTime();
            SimpleTrigger triggerr = new SimpleTrigger("trigger1", "group1", runTimer);
            triggerr.JobName =  "job_" + strjobID;
            triggerr.JobGroup = "CSIPGroup";
            schedr.ScheduleJob(triggerr);
            schedr.Start();
        
    }
    
}
