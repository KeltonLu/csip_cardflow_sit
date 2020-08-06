//******************************************************************
//*  作    者：林偉(Terry)
//*  功能說明：LDAP同步Job
//*  創建日期：2010/05/28
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using Framework.Common.Logging;
using CSIPCommonModel.EntityLayer;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BusinessRules;

/// <summary>
/// LdapUpdJob 的摘要描述
/// </summary>
public class JobLdapUpd : Quartz.IJob
{

    protected JobHelper JobHelper = new JobHelper();
    // Job編號
    const string strJobN = "LdapUpdJob";
    // JobID
    const string strJobID = "1";
    // FunctionKey
    const string strFunKey = "00";
    // JobName
    string strJOBNAME = "LdapUpdJob";      
    // string strJOBNAME = ConfigurationManager.AppSettings["JOBNAME_" + strJobN].Trim();      


    public JobLdapUpd()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// Job 調用入口
    /// </summary>
    /// <param name="context"></param>
    public void Execute(Quartz.JobExecutionContext context)
    {
        String jobId = context.JobDetail.JobDataMap["JOBID"].ToString();
        JobHelper.strJobId = jobId;
        // 用戶編號
        string strUsrID = string.Empty;
        // 系統信息
        string strMsgID = string.Empty;
        // 實體對像(User)
        EntityM_USER usrPropty = new EntityM_USER();
        // 信息資料(LDAP)
        DataSet dsLdapMsg = new DataSet();
        // 用戶信息表
        DataTable dtblUserInfo = new DataTable();
        // 用戶角色表
        DataTable dtblUserRole = new DataTable();
        // 獲取JOB開始時間
        DateTime dTimeStart = DateTime.Now;

        JobHelper.SaveLog(jobId + "JOB啟動", LogState.Info);
        try
        {  
            // 檢測JOB是否有執行如:執行中或、已經執行成功、其他結束了 今天不再執行
            if (!BRL_BATCH_LOG.JobStatusChk(strFunKey, strJOBNAME, dTimeStart))
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不在執行           
                return;
            }
            else
            {
                BRL_BATCH_LOG.Insert(strFunKey, strJOBNAME, dTimeStart, "S","CommonModel_" +strJOBNAME + "_開始執行");
            }
         
            // 獲取所有用戶資料(LDAP)
            dsLdapMsg = LDAPCheckManager.GetLDAPAuth();
            // 如果資料為獲取失敗，直接返回
            if (dsLdapMsg == null)
            {
                JobHelper.SaveLog("資料為獲取失敗", LogState.Info);
                return;
            }
            else
            {
                // 用戶信息表賦值
                dtblUserInfo = dsLdapMsg.Tables[0];
                // 角色信息表賦值
                dtblUserRole = dsLdapMsg.Tables[1];
            }
           
            // 開始從LDAP更新資料至CSIP資料庫
            using (OMTransactionScope tran = new OMTransactionScope("Connection_CSIP"))
            {
                // 先清空 M_USER中所有信息，然後再與LDAP同步
                if (!BRM_USER.DeleteAllUser(ref strMsgID))
                {
                    BRL_BATCH_LOG.Insert(strFunKey, strJOBNAME, DateTime.Now, "F", "CommonModel_清除用戶信息數據失敗!");
                    JobHelper.SaveLog("CommonModel_清除用戶信息數據失敗", LogState.Info);
                    return;
                }               
                
                // 遍歷所有用戶
                foreach (DataRow drowUserInfo in dtblUserInfo.Rows)
                {
                    // 獲取當前User ID
                    strUsrID = drowUserInfo[0].ToString().Trim();
                    // 查詢當前用戶所有角色
                    DataRow[] drUserInRole = dtblUserRole.Select("UserID='" + drowUserInfo[0].ToString() + "'");                  
                    for (int i = 0; i < drUserInRole.Length; i++)
                    {
                        usrPropty.USER_ID = drowUserInfo[0].ToString();                        
                        usrPropty.USER_NAME = drowUserInfo[1].ToString();                        
                        usrPropty.USER_MAIL = drowUserInfo[2].ToString();                        
                        usrPropty.ROLE_ID = drUserInRole[i][1].ToString();                        
                        usrPropty.CHANGED_USER = strJOBNAME;                        
                        usrPropty.CHANGED_TIME = DateTime.Now;
                        
                        if (!BRM_USER.Add(usrPropty, ref strMsgID))
                        {
                            // 如果用戶更新失敗那麼全部回滾
                            BRL_BATCH_LOG.Insert(strFunKey, strJOBNAME, DateTime.Now, "F", "CommonModel_LDAP同步用戶數據失敗!");
                            JobHelper.SaveLog("CommonModel_LDAP同步用戶數據失敗", LogState.Info);
                            return;
                        }
                    }
                }
                //提交事務
                tran.Complete();
                BRL_BATCH_LOG.Insert(strFunKey, strJOBNAME, DateTime.Now, "S", "CommonModel_" + strJOBNAME + "_執行完成");
            }

            JobHelper.SaveLog("JOB結束！", LogState.Info);
        }
        catch (Exception ex)
        {
            BRL_BATCH_LOG.Insert(strFunKey, strJOBNAME, DateTime.Now, "F", "CommonModel_發錯錯誤_" + ex.ToString());
            BRL_BATCH_LOG.SaveLog(ex);
            JobHelper.SaveLog(ex);
        }
        
    }
}