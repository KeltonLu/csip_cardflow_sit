//******************************************************************
//*  功能說明：卡片退件資料表業務邏輯層
//*  作    者：zhiyuan
//*  創建日期：2010/05/27
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using CSIPCommonModel.BusinessRules;
using Framework.Common.Utility;

namespace BusinessRules
{
    public class BRM_CardBackInfo : BRBase<Entity_CardBackInfo>
    {
        #region SQL語句
        public const string SEL_TRANDATE_OR_BACKDATE = @"SELECT b.cardno, c.cardtype, b.Backdate,b.id,b.ACTION FROM tbl_Card_BackInfo (nolock) AS b left JOIN tbl_Card_BaseInfo (nolock)  AS c on b.CardNo=c.CardNo where b.{0} <= @Date AND cast(CardBackStatus as char(1))= '0' and b.Cardtype in ( '013' , '370' , '035' ,'571' , '040 ' , '039' , '037' )  order by b.Backdate desc";
        public const string SEL_CHECK_BACKDATE = @"SELECT top 1 CardNo FROM tbl_Card_BackInfo where {0} = @SearchDate AND CardBackStatus = '0' and Backdate > @BefoerDate";
        public const string UPD_BACKSTATUS = @"UPDATE tbl_Card_BackInfo Set Closedate=@SysDate,Enduid=@ID,Enddate=@SysDate,Enditem='6',CardBackStatus='2' , endFunction='1'  WHERE {0}<=@SearchDate AND cast(CardBackStatus as char(1))= '0'  and Cardtype in ( '013' , '370' , '035' ,'571' , '040 ' , '039' , '037' ) ";
        public const string SEL_SERIAL_NO = @"Select count(0) From tbl_Card_BackInfo Where {0}=@{0}";
        public const string UPD_0115_2 = @"Update tbl_Card_BackInfo Set Blockcode=@Blockcode,Enditem='6',Enddate=@Enddate,Enduid=@Enduid,endFunction='2',CardBackStatus='2',Closedate=@Closedate Where serial_no=@serial_no";
        public const string UPD_0115_2_2 = @"Update tbl_Card_BackInfo Set Blockcode=@Blockcode,Enditem='6',Enddate=@Enddate,Enduid=@Enduid,endFunction='2',CardBackStatus='2',Closedate=@Closedate Where serial_no=@serial_no  and blockcode='' ";
        public const string SEL_COUNT_BACKDATE = @"SELECT Count(b.serial_no) FROM tbl_Card_BackInfo AS b where b.serial_no = @serial_no ";
        #endregion

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
                    DataRow[] dr = dtCardType.Select("PROPERTY_CODE=" + dtDetail.Rows[i][strCardType].ToString());
                    if (dr.Length > 0)
                    {
                        dtDetail.Rows[i][strCardType] = dr[0]["PROPERTY_NAME"].ToString();
                        continue;
                    }
                }
            }
        }


        /// <summary>
        /// 功能說明:查詢是否有大於4個月的退件日期資料
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/02
        /// 修改記錄:
        /// </summary>
        /// <param name="TrandateOrBackDate">退件日/轉當日(B/T)</param>
        /// <param name="SearchDate">查詢日期</param>
        /// <returns>true:有大於4個月1天的退件日期資料</returns>
        public static bool CheckBackDate(char TrandateOrBackDate, string SearchDate)
        {
            string strBefoerDate = DateTime.Now.AddMonths(-4).ToString("yyyy/MM/dd");
            try
            {
                string colName = string.Empty;
                string sqlText = string.Empty;
                switch (TrandateOrBackDate)
                {
                    case 'T':
                        colName = "Trandate";
                        break;
                    case 'B':
                        colName = "Backdate";
                        break;
                }

                sqlText = String.Format(SEL_CHECK_BACKDATE, colName);
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = sqlText;
                sqlcmd.CommandType = CommandType.Text;
                SqlParameter parmDate = new SqlParameter("@SearchDate", SearchDate);
                sqlcmd.Parameters.Add(parmDate);
                SqlParameter parmBefoerDate = new SqlParameter("@BefoerDate", strBefoerDate);
                sqlcmd.Parameters.Add(parmBefoerDate);
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
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
        /// 功能說明:綜合資料處理修改-查詢最後一次退件資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBackInfo"></param>
        /// <returns></returns>
        public static bool SearchByCardNo(string strCondition, ref  DataTable dtCardBackInfo, ref string strMsgID)
        {
            try
            {
                string sql = @"SELECT   [CardNo]
                                          ,[Action]
                                          ,[Id]
                                          ,[CustName]
                                          ,[Trandate]
                                          ,[Kind]
                                          ,[Reason]
                                          ,[Backdate]
                                          ,[OldZip]
                                          ,[OldAdd1]
                                          ,[OldAdd2]
                                          ,[OldAdd3]
                                          ,[NewName]
                                          ,[NewZip]
                                          ,[NewAdd1]
                                          ,[NewAdd2]
                                          ,[NewAdd3]
                                          ,[ImportDate]
                                          ,[ImportFileName]
                                          ,[Enditem]
                                          ,[Enddate]
                                          ,[Enduid]
                                          ,[Endnote]
                                          ,[EndFunction]
                                          ,[Closedate]
                                          ,[CardBackStatus]
                                          ,[Maildate]
                                          ,[Mailno]
                                          ,[InformMerchDate]
                                          ,[Blockcode]
                                          ,[Exp_Count]
                                          ,[Exp_Date]
                                      FROM [tbl_Card_BackInfo]";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }
                sql += " order by [Backdate] desc";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardBackInfo = ds.Tables[0];
                    strMsgID = "06_06040100_007";
                    return true;
                }
                else
                {
                    strMsgID = "06_06040100_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明: 查詢流水號是否重複(一直開啟連接，直到查詢結束)
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="dtValues">含有流水號欄的DataTable</param>
        /// <param name="strColName">需判斷重複值的欄位名稱</param>
        /// <returns>true:無重複</returns>
        public static bool BatIsRepeatByColName(DataTable dtValues, string strColName)
        {
            bool bReturn = false;
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlParameter parm = new SqlParameter();
            int iRowNum = 0;
            try
            {
                string strSql = string.Format(SEL_SERIAL_NO, strColName);
                sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));

                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;
                parm.ParameterName = "@" + strColName;

                sqlConn.Open();

                foreach (DataRow drData in dtValues.Rows)
                {
                    iRowNum = 0;
                    sqlCmd.Parameters.Clear();
                    parm.Value = drData[strColName].ToString();
                    sqlCmd.Parameters.Add(parm);
                    sqlCmd.CommandText = strSql;

                    iRowNum = Int32.Parse(sqlCmd.ExecuteScalar().ToString());

                    if (iRowNum > 0)
                    {
                        break;
                    }
                }

                if (iRowNum <= 0)
                {
                    bReturn = true;
                }
            }
            catch (System.Exception ex)
            {
                BRM_CardBackInfo.SaveLog(ex.Message);
            }
            finally
            {
                if (sqlCmd != null)
                {
                    sqlCmd.Dispose();
                }
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }

            return bReturn;
        }

        /// <summary>
        /// 功能說明:查詢流水號是否重複
        /// 作    者:HAO CHEN
        /// 創建時間:2010/07/09
        /// 修改記錄:
        /// </summary>
        /// <param name="strSno">流水號</param>
        /// <returns>true:有重複</returns>
        public static bool IsRepeatBySno(string strSno)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = SEL_COUNT_BACKDATE;
                SqlParameter parmSerial = new SqlParameter("@serial_no", strSno);
                sqlCmd.Parameters.Add(parmSerial);

                DataSet ds = BRM_Post.SearchOnDataSet(sqlCmd);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (null != ds.Tables[0].Rows[0][0])
                    {
                        if (Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString()) > 0)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return false;
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:新增多筆資料，事務專用
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsertFor0110(EntitySet<Entity_CardBackInfo> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    if (BRM_CardBackInfo.BatInsert(Set))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_001";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_002";
                        return false;
                    }

                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新退件狀態，0115_2專用
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="strBlockcode"></param>
        /// <param name="strSerial_no"></param>
        /// <returns></returns>
        public static bool UpdFor0115_2(SqlCommand sqlCmd, string strBlockcode, string strSerial_no)
        {
            try
            {
                sqlCmd.Parameters.Clear();
                sqlCmd.CommandType = CommandType.Text;
               // sqlCmd.CommandText = UPD_0115_2;
                sqlCmd.CommandText = UPD_0115_2_2;
                

                SqlParameter parmBlockcode = new SqlParameter("@Blockcode", strBlockcode);
                SqlParameter parmEnddate = new SqlParameter("@Enddate", DateTime.Now.ToString("yyyy/MM/dd"));
                SqlParameter parmEnduid = new SqlParameter("@Enduid", "系統");
                SqlParameter parmserial_no = new SqlParameter("@serial_no", strSerial_no);
                SqlParameter parmClosedate = new SqlParameter("@Closedate", DateTime.Now.ToString("yyyy/MM/dd"));
                

                sqlCmd.Parameters.Add(parmBlockcode);
                sqlCmd.Parameters.Add(parmEnddate);
                sqlCmd.Parameters.Add(parmEnduid);
                sqlCmd.Parameters.Add(parmserial_no);
                sqlCmd.Parameters.Add(parmClosedate);

                Framework.Common.Logging.Logging.AddSqlLog(sqlCmd);
                sqlCmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                BRM_CardBackInfo.SaveLog(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 功能說明:查詢退件資料For 0115_1
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetBOutData(ref DataTable dtDetail)
        {
            try
            {
                string sql = @"Select Id,CardNo,serial_no,cardtype,Serial_no,Exp_Count,Exp_Date From tbl_Card_BackInfo 
                                      Where Enditem='' And 
                                      ((Backdate=@Backdate And isnull(Exp_Count,'0')='0') Or
                                      (Exp_Date=@Exp_Date And isnull(Exp_Count,'0')='1'))";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
            //改為抓工作日
            //    SqlParameter parmBackdate = new SqlParameter("@Backdate", DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
            //    sqlcmd.Parameters.Add(parmBackdate);
            //    SqlParameter parmExp_Date = new SqlParameter("@Exp_Date", DateTime.Now.AddDays(-21).ToString("yyyy/MM/dd"));

                SqlParameter parmBackdate = new SqlParameter("@Backdate", DateTime.ParseExact(BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd"));
                sqlcmd.Parameters.Add(parmBackdate);
                SqlParameter parmExp_Date = new SqlParameter("@Exp_Date", DateTime.Now.AddDays(-21.0).ToString("yyyy/MM/dd"));


                sqlcmd.Parameters.Add(parmExp_Date);
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtDetail = ds.Tables[0];
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

        /// <summary>
        /// 功能說明:查詢退件未結案檔For 0115_4
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetBackData(ref DataTable dtDetail)
        {
            try
            {
                string sql = @"Select Distinct ID From tbl_Card_BackInfo
                                Where CardBackStatus <> '2'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtDetail = ds.Tables[0];
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

        /// <summary>
        /// 功能說明:查詢招領資料檔For 0115_5
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetBackDataFor0115_5(ref DataTable dtDetail, string strType)
        {
            string sql = string.Empty;
            try
            {
                if (strType.Equals("1"))
                {
                    sql = @"Select ID,B.Mailno,Post_TEL,CardNo From 
                                tbl_Card_BaseInfo B left join tbl_Post_Send P
                                on B.maildate=P.maildate And B.mailno=P.mailno
                                where P.MailDate between @bDate and @eDate and
                                Info1='240' And Send_status_Code='G2' And
                                CardNo not in
                                (select CardNo From tbl_Card_BackInfo)";
                }
                else if (strType.Equals("2"))
                {
                    sql = @"Select ID,'01' Type From 
                                tbl_Card_BaseInfo B left join tbl_Post_Send P
                                on B.maildate=P.maildate And B.mailno=P.mailno
                                where P.MailDate between @bDate and @eDate and
                                Info1='240' And Send_status_Code='G2' And
                                CardNo not in
                                (select CardNo From tbl_Card_BackInfo)
                                union 
                                Select ID,'02' Type From tbl_Card_BackInfo
                                Where CardBackStatus<>'2'";
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parmBDate = new SqlParameter("@bDate", DateTime.Now.AddDays(-8).ToString("yyyy/MM/dd"));
                sqlcmd.Parameters.Add(parmBDate);
                SqlParameter parmEDate = new SqlParameter("@eDate", DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
                sqlcmd.Parameters.Add(parmEDate);
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtDetail = ds.Tables[0];
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
        /// <summary>
        /// 功能說明:更新一筆卡片退件資料,事務專用
        /// 作    者:linda
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool Update(Entity_CardBackInfo CardBackInfo, string strCondition, params  string[] FiledSpit)
        {
            try
            {

                if (BRM_CardBackInfo.UpdateEntityByCondition(CardBackInfo, strCondition, FiledSpit))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// 功能說明:更新退件資料For0114
        /// 作    者:linda
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BackInfoUpdateFor0114(DataTable dtCardBackInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtCardBackInfo.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                    CardBackInfo.Mailno = dtCardBackInfo.Rows[i]["mailno"].ToString();
                    CardBackInfo.Maildate = DateTime.Now.ToString("yyyy/MM/dd");
                    CardBackInfo.serial_no = dtCardBackInfo.Rows[i]["serial_no"].ToString();
                    sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                    if (!Update(CardBackInfo, sqlhelp.GetFilterCondition(), "Mailno", "Maildate"))//*更新條件設置
                    {
                        blnResult = false;
                        break;
                    }
                }
                if (blnResult)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }
        /// <summary>
        /// 功能說明:更新处理状态为CardBackStatus为已结案2
        /// 作    者:Linda
        /// 創建時間:2010/06/10
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDataChange"></param>
        /// <param name="dtUnableCard"></param>
        /// <param name="dtChangeCard"></param>
        public static bool UpdateCardBackStatus(DataTable dtExportCardBackInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                foreach (DataRow row in dtExportCardBackInfo.Rows)
                {
                    string strMsgID = string.Empty;
                    Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                    CardBackInfo.serial_no = row["serial_no"].ToString();
                    CardBackInfo.CardBackStatus = "2";
                    CardBackInfo.Closedate = DateTime.Now.ToString("yyyy/MM/dd");
                    SqlHelper sqlhelp = new SqlHelper();
                    sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                    sqlhelp.AddCondition(Entity_CardBackInfo.M_CardBackStatus, Operator.NotEqual, DataTypeUtils.String, "2");
                    if (!Update(CardBackInfo, sqlhelp.GetFilterCondition(), "CardBackStatus", "Closedate"))
                    {
                        blnResult = false;
                        break;
                    }
                }
                if (blnResult)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新退件資料（BlockCode）For0113
        /// 作    者:linda
        /// 創建時間:2010/07/26
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BLKUpdateFor0113(DataTable dtCardBackInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtCardBackInfo.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                    CardBackInfo.serial_no = dtCardBackInfo.Rows[i]["SerialNo"].ToString();
                    CardBackInfo.Blockcode = dtCardBackInfo.Rows[i]["BlockCode"].ToString();
                    sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                    if (!Update(CardBackInfo, sqlhelp.GetFilterCondition(), "Blockcode"))
                    {
                        blnResult = false;
                        break;
                    }
                }
                if (blnResult)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:寫入退件通知卡廠日期InformMerchDate
        /// 作    者:Linda
        /// 創建時間:2010/06/10
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDataChange"></param>
        /// <param name="dtUnableCard"></param>
        /// <param name="dtChangeCard"></param>
        public static bool UpdateInformMerchDate(DataTable dtExportCardBackInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                foreach (DataRow row in dtExportCardBackInfo.Rows)
                {
                    string strMsgID = string.Empty;
                    Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                    CardBackInfo.serial_no = row["serial_no"].ToString();
                    CardBackInfo.InformMerchDate = DateTime.Now.ToString("yyyy/MM/dd");
                    SqlHelper sqlhelp = new SqlHelper();
                    sqlhelp.AddCondition(Entity_CardBackInfo.M_serial_no, Operator.Equal, DataTypeUtils.String, CardBackInfo.serial_no);
                    if (!Update(CardBackInfo, sqlhelp.GetFilterCondition(), "InformMerchDate"))
                    {
                        blnResult = false;
                        break;
                    }
                }
                if (blnResult)
                {
                    ts.Complete();
                    return true;
                }
                else
                {
                    ts.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:獲取退件通知檔資料
        /// 作    者:linda
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchExportCardBackInfo(ref  DataTable dtExportCardBackInfo)
        {
            try
            {
                string sql = @"SELECT back.serial_no, --流水號
                                        ISNULL(back.CustName,'') CustName,--姓名
                                        ISNULL(back.NewName,'') NewName,--新姓名
                                        back.CardType,--類別
                                        back.Action,--卡別
                                        back.CardNo,--卡號
                                        ISNULL(back.Backdate,'') Backdate,--退件日期
                                        ISNULL(back.Reason,'') Reason,--退件原因
                                        back.Enddate,--處理日期
                                        back.Enditem,--處理方法
                                        ISNULL(back.NewZip,'') NewZip,--新郵遞區號
                                        ISNULL(back.NewAdd1,'') NewAdd1,--新地址1
                                        ISNULL(back.NewAdd2,'') NewAdd2,--新地址2
                                        ISNULL(back.NewAdd3,'') NewAdd3,--新地址3
                                        back.EndFunction
                                        FROM tbl_Card_BackInfo back
                                        WHERE ISNULL(InformMerchDate,'')='' 
                                        AND ( (EndFunction ='0' AND CardBackStatus <>'2')
                                        OR EndFunction='1' 
                                        OR (EndFunction='2' AND ISNULL(Blockcode,'') <> '') ) 
                                        ORDER BY back.Enditem,back.serial_no";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtExportCardBackInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:查詢自取卡片資料
        /// 作    者:linda
        /// 創建時間:2010/06/22
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetBackInfoFor0206(ref  DataTable dtBackInfo, int iPageIndex, int iPageSize, ref int iTotalCount, string strId, string strBackStatus)
        {
            try
            {

                string sql = @"Select back.serial_no,back.CardNo,back.Id,back.Action,back.Trandate,back.CustName,back.cardtype,back.Backdate,back.Reason,base.Mailno,isnull(back.CardBackStatus,'0') as CardBackStatus,Enditem,isnull(back.Blockcode,'') as Blockcode,";
                sql += " base.zip,base.add1,base.add2,base.add3";
                sql += " From dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base ";
                sql += " Where back.Id=base.id and back.CardNo=base.cardno and back.Action=base.action and back.Trandate=base.trandate";
                sql += " And back.Id =@Id";
                if (strBackStatus != "3")
                {
                    sql += " And back.CardBackStatus=@CardBackStatus";
                }
                sql += " Order by back.Backdate desc";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@Id", strId));
                if (strBackStatus != "3")
                    sqlcmd.Parameters.Add(new SqlParameter("@CardBackStatus", strBackStatus));
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtBackInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:注銷結案
        /// 作    者:Linda
        /// 創建時間:2010/07/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDataChange"></param>
        /// <param name="dtUnableCard"></param>
        /// <param name="dtChangeCard"></param>
        public static bool CardBackInfoEnd(int intCardType, string strMinusDate)
        {
            try
            {
                string sql = "update dbo.tbl_Card_BackInfo";
                sql += " set Enditem='6',EndFunction='1',Enduid='sys',Enddate=convert(nvarchar(10),getdate(),111),CardBackStatus='2',Closedate=convert(nvarchar(10),getdate(),111)";
                sql += " where CardBackStatus='0'";
                sql += " and Backdate<=convert(nvarchar(10), dateadd(mm,-@strMinusDate,getdate()),111)";
                switch (intCardType)
                {
                    case 1: //信用卡
                        sql += " and cardtype not in ('000','013','370','018','019','039','040','038','037','041')";
                        break;
                    case 2://金融卡
                        sql += " and cardtype ='000'";
                        break;
                    case 3://現金卡
                        sql += " and cardtype ='018'";
                        break;
                    case 4://e-Cash卡
                        sql += " and cardtype ='019'";
                        break;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strMinusDate", strMinusDate));
                if (BRM_CardBackInfo.Update(sqlcmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢注銷結案資料
        /// 作    者:linda
        /// 創建時間:2010/07/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetCardEndInfo(int intCardType, string strMinusDate, ref DataTable dtCardEndInfo)
        {
            try
            {

                string sql = @"Select * from dbo.tbl_Card_BackInfo";
                sql += " where CardBackStatus='0'";
                sql += " and Backdate<=convert(nvarchar(10), dateadd(mm,-@strMinusDate,getdate()),111)";
                switch (intCardType)
                {
                    case 1: //信用卡
                        sql += " and cardtype not in ('000','013','370','018','019','039','040','038','037','041')";
                        break;
                    case 2://金融卡
                        sql += " and cardtype ='000'";
                        break;
                    case 3://現金卡
                        sql += " and cardtype ='018'";
                        break;
                    case 4://e-Cash卡
                        sql += " and cardtype ='019'";
                        break;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strMinusDate", strMinusDate));
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardEndInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBackInfo.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
