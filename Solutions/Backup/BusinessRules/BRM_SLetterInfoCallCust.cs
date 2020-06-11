//******************************************************************
//*  功能說明：簡訊處理通知主機檔邏輯層
//*  作    者：zhiyuan
//*  創建日期：2010/06/07
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
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;

namespace BusinessRules
{
    public class BRM_SLetterInfoCallCust : BRBase<Entity_SLetterInfoCallCust>
    {
        #region SQL語句
        public const string UPD_LETTERINFOCALLCUST_0115_2 = @"Update tbl_sletterInfo_CallCust Set Name=@Name,Htel=@Htel,Otel=@Otel,Dob=@Dob,Zip=@Zip,Add1=@Add1,Add2=@Add2,Add3=@Add3,Mobil=@Mobil,Block_code=@Block_code,Ams=@Ams,Imp_File=@Imp_File,Imp_Date=@Imp_Date Where ID=@ID And CardNo=@CardNo";
        public const string SEL_LETTERINFOCALLCUST_0115_2 = @"Select top 1 Source_Flg,Serial_no From tbl_sletterInfo_CallCust Where ID=@ID And CardNo=@CardNo  Order By Exp_Date Desc";
        public const string SEL_LETTERINFOCALLCUST_0115_1 = @"Select ID,CardNo,Exp_Date From tbl_sletterInfo_CallCust Where ID=@ID And CardNo=@CardNo And Exp_Date=@Exp_Date";
        #endregion
        
        /// <summary>
        /// 功能說明:自動化簡訊處理-主機回覆檔匯入，事務專用
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="dtCardDataChange"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0115_2(DataTable dtDetail,string strImpFileName)
        {
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlTransaction stTran = null;
            SqlParameter parmName = new SqlParameter();
            SqlParameter parmHtel = new SqlParameter();
            SqlParameter parmOtel = new SqlParameter();
            SqlParameter parmDob = new SqlParameter();
            SqlParameter parmZip = new SqlParameter();
            SqlParameter parmAdd1 = new SqlParameter();
            SqlParameter parmAdd2 = new SqlParameter();
            SqlParameter parmAdd3 = new SqlParameter();
            SqlParameter parmMobil = new SqlParameter();
            SqlParameter parmBlock_code = new SqlParameter();
            SqlParameter parmAms = new SqlParameter();
            SqlParameter parmImp_File = new SqlParameter();
            SqlParameter parmImp_Date = new SqlParameter();
            SqlParameter parmID = new SqlParameter();
            SqlParameter parmCardNo = new SqlParameter();
            SqlDataAdapter sdaValue = null;
            DataTable dtTmp = new DataTable();
            string strSrcFlg = string.Empty;
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection_System"].ToString());
                
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;

                #region 設置參數名稱
                parmName.ParameterName = "@Name";
                parmHtel.ParameterName = "@Htel";
                parmOtel.ParameterName = "@Otel";
                parmDob.ParameterName = "@Dob";
                parmZip.ParameterName = "@Zip";
                parmAdd1.ParameterName = "@Add1";
                parmAdd2.ParameterName = "@Add2";
                parmAdd3.ParameterName = "@Add3";
                parmMobil.ParameterName = "@Mobil";
                parmBlock_code.ParameterName = "@Block_code";
                parmAms.ParameterName = "@Ams";
                parmImp_Date.ParameterName = "@Imp_Date";
                parmImp_File.ParameterName = "@Imp_File";
                parmID.ParameterName = "@ID";
                parmCardNo.ParameterName = "@CardNo";
                #endregion
                bool blnResult = true;
                //string[] strFiledSpit = new string[] { "Name","Htel", "Otel", "Dob", "Zip", "City", "Add2", "Add3", "Mobil", "Block_code", "Ams", "Imp_File", "Imp_Date" };
                sqlConn.Open();
                stTran = sqlConn.BeginTransaction();
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    sqlCmd.Parameters.Clear();
                    sqlCmd.Transaction = stTran;
                    parmName.Value = dtDetail.Rows[i]["Name"].ToString();  //姓名
                    parmHtel.Value = dtDetail.Rows[i]["Htel"].ToString().Replace("-", "");  //住家電話
                    parmOtel.Value = dtDetail.Rows[i]["Otel"].ToString().Replace("-", "");  //公司電話
                    parmDob.Value = DateHelper.InsertTimeSpan(dtDetail.Rows[i]["Dop"].ToString()); //生日
                    parmZip.Value = dtDetail.Rows[i]["Zip"].ToString();    //郵遞區號
                    parmAdd1.Value = dtDetail.Rows[i]["City"].ToString();  //城市地區
                    parmAdd2.Value = dtDetail.Rows[i]["Add1"].ToString();  //地址2
                    parmAdd3.Value = dtDetail.Rows[i]["Add2"].ToString();  //地址3
                    parmMobil.Value = dtDetail.Rows[i]["Mobil"].ToString().Replace("-", "");   //行動電話
                    parmBlock_code.Value = dtDetail.Rows[i]["Block_code"].ToString();  //BlockCode
                    parmAms.Value = dtDetail.Rows[i]["Ams"].ToString();    //Ams
                    parmImp_File.Value = strImpFileName;   //主機回覆檔匯入檔名
                    //多加判斷如果是假日要抓下一個工作日
                    DateTime dt = new DateTime();
                    if (BRWORK_DATE.IS_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd")))
                    {
                        dt = DateTime.Now;
                    }
                    else
                    {
                        dt = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), 1), "yyyyMMdd", null);
                    }
                    parmImp_Date.Value = dt.ToString("yyyy/MM/dd");  //主機回覆檔匯入日期
                   // parmImp_Date.Value = DateTime.Now.ToString("yyyy/MM/dd");  //主機回覆檔匯入日期
                    parmID.Value = dtDetail.Rows[i]["ID"].ToString();  //ID
                    parmCardNo.Value = dtDetail.Rows[i]["CardNo"].ToString();  //卡號

                    sqlCmd.Parameters.Add(parmName);
                    sqlCmd.Parameters.Add(parmHtel);
                    sqlCmd.Parameters.Add(parmOtel);
                    sqlCmd.Parameters.Add(parmDob);
                    sqlCmd.Parameters.Add(parmZip);
                    sqlCmd.Parameters.Add(parmAdd1);
                    sqlCmd.Parameters.Add(parmAdd2);
                    sqlCmd.Parameters.Add(parmAdd3);
                    sqlCmd.Parameters.Add(parmMobil);
                    sqlCmd.Parameters.Add(parmBlock_code);
                    sqlCmd.Parameters.Add(parmAms);
                    sqlCmd.Parameters.Add(parmImp_File);
                    sqlCmd.Parameters.Add(parmImp_Date);
                    sqlCmd.Parameters.Add(parmID);
                    sqlCmd.Parameters.Add(parmCardNo);

                    sqlCmd.CommandText = UPD_LETTERINFOCALLCUST_0115_2;
                    sqlCmd.ExecuteNonQuery();

                    strSrcFlg = string.Empty;
                    sqlCmd.CommandText = SEL_LETTERINFOCALLCUST_0115_2;
                    sdaValue = new SqlDataAdapter(sqlCmd);
                    sdaValue.Fill(dtTmp);
                    if (dtTmp.Rows.Count > 0)
                    {
                        if (dtTmp.Rows[0]["Source_Flg"].ToString().Equals("1"))
                        {
                            //*根據主機回覆檔中的資料更新8.4卡片退件資料表
                            if (!BRM_CardBackInfo.UpdFor0115_2(sqlCmd, dtDetail.Rows[i]["Block_code"].ToString(), dtTmp.Rows[0]["Serial_no"].ToString()))
                            {
                                stTran.Rollback();
                                return false;
                            }
                            
                        }
                    }

                }
                if (blnResult)
                {
                    stTran.Commit();
                    return true;
                }
                else
                {
                    stTran.Rollback();
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_SLetterInfoCallCust.SaveLog(exp.Message);
                stTran.Rollback();
                return false;
            }
            finally
            {
                stTran.Dispose();
                if (sdaValue != null)
                {
                    sdaValue.Dispose();
                }
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
        }


        /// <summary>
        /// 功能說明:自動化簡訊處理-通知主機檔匯出，批量寫入，事務專用
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/07
        /// 修改記錄:
        /// </summary>
        /// <param name="Set"></param>
        /// <returns></returns>
        public static bool BatInsertFor0115_1(EntitySet<Entity_SLetterInfoCallCust> Set)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_SLetterInfoCallCust.BatInsert(Set))
                    {
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_SLetterInfoCallCust.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢招領資料For 0115_3
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public static bool GetOutData(string strType,ref DataTable dtDetail)
        {
            string sql = string.Empty;
            try
            {
                if (strType.Equals("P"))
                {
                    sql = @"Select Distinct C.ID,Mobil,Name,C.Mailno,c.Maildate,P.Post_Name,substring(CardNo,1,6) CardType,P.Exp_Count,C.Exp_Date,C.CardNo
                                from tbl_sletterInfo_CallCust C left join
                                tbl_Post_Send P on C.Maildate=P.maildate And C.Mailno=P.mailno
                                Where C.Imp_Date=@Imp_Date And Mobil <> '' and Source_Flg='0' And Block_code=''";
                }
                if(strType.Equals("B"))
                {
                    sql = @"Select Distinct C.ID,Mobil,Name,substring(C.CardNo,1,6) CardType,C.Maildate,C.Mailno,C.Serial_no,B.Exp_Count,C.Exp_Date,C.CardNo
                                from tbl_sletterInfo_CallCust C left join 
                                tbl_Card_BackInfo B on C.Serial_No=B.Serial_No And C.ID=B.ID
                                Where Imp_Date=@Imp_Date And Mobil <> '' And Block_code='' and Source_Flg='1' and CardBackStatus ='0'";
                }
                

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parm = new SqlParameter("@Imp_date", DateTime.Now.ToString("yyyy/MM/dd"));
                // 目前判斷是今日的匯入日期才會抓取，因此取得當天應該沒有問題，只是要排除假日的執行
                sqlcmd.Parameters.Add(parm);
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
        /// 功能說明:匯出資料后更新簡訊處理信息檔
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="drRows"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static bool UpdCallCustFor0115_3(DataRow[] drRows,string strFileName)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                foreach (DataRow dr in drRows)
                {
                    //string strSql = "select count(*) from tbl_Card_BaseInfo Where Mailno='" + dr["Mailno"] + "' and Maildate='" + dr["Maildate"] + "'";
                    //SqlCommand sqlCmd = new SqlCommand();
                    //sqlCmd.CommandType = CommandType.Text;
                    //sqlCmd.CommandText = strSql;
                    //DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlCmd);
                    //if (ds != null)
                    //{
                    //    if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 1)
                    //    {
                    //        continue;
                    //    }
                    //}

                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_SLetterInfoCallCust CallCust = new Entity_SLetterInfoCallCust();
                    CallCust.Exp_LetterInfo_File = strFileName;
                    CallCust.Exp_LetterInfo_Date = DateTime.Now.ToString("yyyy/MM/dd");
                    CallCust.Exp_Date = dr["Exp_Date"].ToString();
                    CallCust.ID = dr["ID"].ToString().Trim();
                    CallCust.CardNo = dr["CardNo"].ToString();

                    sqlhelp.AddCondition(Entity_SLetterInfoCallCust.M_Exp_Date, Operator.Equal, DataTypeUtils.String, CallCust.Exp_Date);
                    sqlhelp.AddCondition(Entity_SLetterInfoCallCust.M_ID, Operator.Equal, DataTypeUtils.String, CallCust.ID);
                    sqlhelp.AddCondition(Entity_SLetterInfoCallCust.M_CardNo, Operator.Equal, DataTypeUtils.String, CallCust.CardNo);

                    if (!Update(CallCust, sqlhelp.GetFilterCondition(), "Exp_LetterInfo_File", "Exp_LetterInfo_Date"))//*更新條件設置
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
                BRM_SLetterInfoCallCust.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        public static bool Update(Entity_SLetterInfoCallCust CallCust, string strCondition, params  string[] FiledSpit)
        {
            try
            {
                if (BRM_SLetterInfoCallCust.UpdateEntityByCondition(CallCust, strCondition, FiledSpit))
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
                BRM_SLetterInfoCallCust.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢簡訊檔For 0115_4
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetSMSData(ref DataTable dtDetail)
        {
            try
            {
                string sql = @"Select ID,Mobil,CardNo,Source_Flg 
                                From tbl_sletterInfo_CallCust
                                Where Exp_LetterInfo_Date=@Exp_LetterInfo_Date";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parm = new SqlParameter("@Exp_LetterInfo_Date",DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
                sqlcmd.Parameters.Add(parm);
                DataSet ds = BRM_SLetterInfoCallCust.SearchOnDataSet(sqlcmd);
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

        /// 功能說明:檢測JOB是否在執行中 
        /// 作    者:Linda
        /// 創建時間:2010/05/31 
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="dtTimeSt">開始時間</param>
        /// <returns>是否成功</returns>
        public static bool GetInfoCallCust(string strId, string strCardNo, string ExpDate)
        {
            DataSet dsInfo = null;
            DataTable dtInfo = null;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_LETTERINFOCALLCUST_0115_1;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", strId));
            sqlcmd.Parameters.Add(new SqlParameter("@CardNo", strCardNo));
            sqlcmd.Parameters.Add(new SqlParameter("@Exp_Date", ExpDate));

            try
            {
                dsInfo = BRM_SLetterInfoCallCust.SearchOnDataSet(sqlcmd);
                if (dsInfo != null)
                {
                    dtInfo = dsInfo.Tables[0];
                    if (dtInfo.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_SLetterInfoCallCust.SaveLog(exp);
                return false;
            }
        }
    }
}
