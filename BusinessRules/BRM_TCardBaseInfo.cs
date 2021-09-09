//******************************************************************
//*  功能說明：卡片基本資料處理業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
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
using Framework.Common.Utility;
using System.Configuration;
using CSIPCommonModel.BusinessRules;

namespace BusinessRules
{
    public class BRM_TCardBaseInfo : BRBase<Entity_CardBaseInfo>
    {

        /// <summary>
        /// 功能說明:新增一筆卡片基本資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_CardBaseInfo TCardBaseInfo, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_TCardBaseInfo.AddNewEntity(TCardBaseInfo))
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:新增一筆卡片基本資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(EntitySet<Entity_CardBaseInfo> CardBaseInfos, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    foreach (Entity_CardBaseInfo TCardBaseInfo in CardBaseInfos)
                    {
                        if (!BRM_TCardBaseInfo.AddNewEntity(TCardBaseInfo))
                        {
                            strMsgID = "06_06040100_002";
                            return false;
                        }
                    }
                    ts.Complete();
                    strMsgID = "06_06040100_001";
                    return true;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:新增多筆卡片基本資料，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsertFor0103(EntitySet<Entity_CardBaseInfo> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_TCardBaseInfo.BatInsert(Set))
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }


        /// <summary>
        /// 功能說明:刪除一筆卡片基本資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Delete(Entity_CardBaseInfo TCardBaseInfo, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_TCardBaseInfo.DeleteEntityByCondition(TCardBaseInfo, strCondition))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_005";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_006";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆卡片基本資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Update(Entity_CardBaseInfo TCardBaseInfo, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_TCardBaseInfo.UpdateEntityByCondition(TCardBaseInfo, strCondition, FiledSpit))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_003";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_004";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆卡片基本資料，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool Update(Entity_CardBaseInfo TCardBaseInfo, string strCondition, params  string[] FiledSpit)
        {
            try
            {
                if (BRM_TCardBaseInfo.UpdateEntityByCondition(TCardBaseInfo, strCondition, FiledSpit))
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:大宗回饋檔中的： 身分證號、卡號、卡別、製卡日期 (YYYY.MM.DD) 條件查詢卡片基本資料表，取得 "取卡方式" 
        /// 作    者:zhiyuan
        /// 創建時間:2010/07/29
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static DataTable GetKindTableFor0102(DataTable dtDetail, string strMsg)
        {
            bool bReturn = false;
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlDataAdapter sdaBascInfo = null;

            try
            {
                string strSql = @"select top 1 ID,CARDNO,INDATE1,CARDTYPE,KIND 
                                  from tbl_Card_BaseInfo 
                                  where ID=@ID and CARDNO=@CARDNO and CARDTYPE=@CARDTYPE and card_file=@cardfile ";
                sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;
                SqlParameter sqlParmID = new SqlParameter();
                sqlParmID.ParameterName = "@ID";
                SqlParameter sqlParmCardNo = new SqlParameter();
                sqlParmCardNo.ParameterName = "@CARDNO";
                SqlParameter sqlParmCARDTYPE = new SqlParameter();
                sqlParmCARDTYPE.ParameterName = "@CARDTYPE";
                SqlParameter sqlParmcardfile = new SqlParameter();
                sqlParmcardfile.ParameterName = "@cardfile";

                sqlConn.Open();

                dtDetail.Columns.Add("KIND");
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    DataTable dtTmp = new DataTable();
                    sqlCmd.Parameters.Clear();
                    sqlParmID.Value = dtDetail.Rows[i]["IDNO"].ToString().Trim();
                    sqlParmCardNo.Value = dtDetail.Rows[i]["CARDNO"].ToString().Trim().Replace("-", "");
                    sqlParmCARDTYPE.Value = dtDetail.Rows[i]["CARDTYPE"].ToString().Trim();
                    sqlParmcardfile.Value = dtDetail.Rows[i]["cardfile"].ToString().Trim(); 
                    sqlCmd.Parameters.Add(sqlParmID);
                    sqlCmd.Parameters.Add(sqlParmCardNo);
                    sqlCmd.Parameters.Add(sqlParmCARDTYPE);
                    sqlCmd.Parameters.Add(sqlParmcardfile);
                    sqlCmd.CommandText = strSql;

                    sdaBascInfo = new SqlDataAdapter(sqlCmd);
                    sdaBascInfo.Fill(dtTmp);

                    if (dtTmp.Rows.Count > 0)
                    {
                        dtDetail.Rows[i]["KIND"] = dtTmp.Rows[0]["KIND"].ToString();
                    }
                    else
                    {
                        BRM_TCardBaseInfo.SaveLog(string.Format(strMsg, sqlParmID.Value, sqlParmCardNo.Value, sqlParmCARDTYPE.Value, sqlParmcardfile.Value));
                    }
                }
            }
            catch (System.Exception ex)
            {
                BRM_TCardBaseInfo.SaveLog(ex.Message);
                dtDetail = null;
            }
            finally
            {
                if (sdaBascInfo != null)
                {
                    sdaBascInfo.Dispose();
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
            return dtDetail;
        }

        /// <summary>
        /// 功能說明:自動化大宗檔回饋檔匯入，事務專用
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/10
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0102(DataTable dtTCardBaseInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtTCardBaseInfo.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardBaseInfo TCardBaseInfo = new Entity_CardBaseInfo();

                    TCardBaseInfo.id = dtTCardBaseInfo.Rows[i]["IDNO"].ToString().Trim();
                    TCardBaseInfo.cardno = dtTCardBaseInfo.Rows[i]["CARDNO"].ToString().Trim().Replace("-", "");
                    //因為發現廠商回饋檔中的製卡日欄位可能與卡片製卡日不同，請將對應卡片條件改為卡號、身分證號、card type、card_file BUG202
                    //TCardBaseInfo.indate1 = dtTCardBaseInfo.Rows[i]["INDATE1"].ToString().Trim().Replace(".", "/");
                    TCardBaseInfo.cardtype = dtTCardBaseInfo.Rows[i]["CARDTYPE"].ToString().Trim();
                    TCardBaseInfo.card_file = dtTCardBaseInfo.Rows[i]["cardfile"].ToString().Trim();

                    TCardBaseInfo.mailno = dtTCardBaseInfo.Rows[i]["MailNo"].ToString();
                    TCardBaseInfo.maildate = dtTCardBaseInfo.Rows[i]["MailDate"].ToString().Replace(".", "/");

                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                    //因為發現廠商回饋檔中的製卡日欄位可能與卡片製卡日不同，請將對應卡片條件改為卡號、身分證號、card type、card_file BUG202
                    //sqlhelp.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardtype, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardtype);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_card_file, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.card_file);

                    if (!Update(TCardBaseInfo, sqlhelp.GetFilterCondition(), "mailno", "maildate"))//*更新條件設置
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:自動化大宗檔回饋檔匯入
        /// 作    者:Lida 
        /// 創建時間:2010/11/18
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0102(Entity_CardBaseInfo TCardBaseInfo)
        {
            try
            {
                string strSql = "";
                //max 如果是缺卡的回饋，會更新製卡日
                if (TCardBaseInfo.card_file.Length > 6)
                {
                    if (TCardBaseInfo.card_file.Substring(TCardBaseInfo.card_file.Length - 6, 2).Equals("-1"))
                    {
                         strSql = "update tbl_Card_BaseInfo set indate1=@indate1 ,mailno=@mailno,maildate=@maildate where id = @id AND cardno = @cardno AND cardtype = @cardtype AND card_file = @card_file";
                    }
                    else
                    {
                         strSql = "update tbl_Card_BaseInfo set mailno=@mailno,maildate=@maildate where id = @id AND cardno = @cardno AND cardtype = @cardtype AND card_file = @card_file";
                    }
                }
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = strSql;
                if (TCardBaseInfo.card_file.Substring(TCardBaseInfo.card_file.Length - 6, 2).Equals("-1"))
                    sqlCmd.Parameters.Add(new SqlParameter("@indate1", TCardBaseInfo.indate1));
                sqlCmd.Parameters.Add(new SqlParameter("@mailno", TCardBaseInfo.mailno));
                sqlCmd.Parameters.Add(new SqlParameter("@maildate", TCardBaseInfo.maildate));
                sqlCmd.Parameters.Add(new SqlParameter("@id", TCardBaseInfo.id));
                sqlCmd.Parameters.Add(new SqlParameter("@cardno", TCardBaseInfo.cardno));
                sqlCmd.Parameters.Add(new SqlParameter("@cardtype", TCardBaseInfo.cardtype));
                sqlCmd.Parameters.Add(new SqlParameter("@card_file", TCardBaseInfo.card_file));
                if (BRM_TCardBaseInfo.Update(sqlCmd))
                {
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                BRM_TCardBaseInfo.SaveLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:自動化缺卡大宗檔匯入，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0103(DataTable dtTCardBaseInfo, string strCardType, string strcard_file)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtTCardBaseInfo.Rows.Count; i++)
                {
                  //  BRM_TCardBaseInfo.SaveLog("測試=" + i);

                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardBaseInfo TCardBaseInfo = new Entity_CardBaseInfo();
                    if (strCardType.Equals("1"))
                    {
                        TCardBaseInfo.id = dtTCardBaseInfo.Rows[i]["IdNo"].ToString();
                        TCardBaseInfo.cardno = dtTCardBaseInfo.Rows[i]["cardno1"].ToString().Replace("-", "");
                        TCardBaseInfo.action = dtTCardBaseInfo.Rows[i]["action"].ToString();
                    }
                    else
                    {
                        TCardBaseInfo.id = dtTCardBaseInfo.Rows[i]["IDNO"].ToString();
                        TCardBaseInfo.cardno = dtTCardBaseInfo.Rows[i]["CARD-NO-1"].ToString().Replace("-", "");
                        TCardBaseInfo.action = dtTCardBaseInfo.Rows[i]["ACTION"].ToString();
                    }
                    //*TCardBaseInfo.trandate = dtTCardBaseInfo.Rows[i]["trandate"].ToString();//*layout無轉檔日故不可以作為查詢條件
                    TCardBaseInfo.is_LackCard = "1";

                    TCardBaseInfo.card_file = strcard_file;

                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);
                    //*sqlhelp.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.trandate);

                    if (!Update(TCardBaseInfo, sqlhelp.GetFilterCondition(), "is_LackCard", "card_file"))//*更新條件設置
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:自動化異動回饋檔匯入，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtTCardBaseInfo"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0105(DataTable dtTCardBaseInfo)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtTCardBaseInfo.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardBaseInfo TCardBaseInfo = new Entity_CardBaseInfo();
                    // 因身分證字號，3~5碼亂，故通過CardNo、ACTION、Indate1 UPDATE掛號號碼（mailno）、郵寄日期（maildate）
                    //TCardBaseInfo.id = dtTCardBaseInfo.Rows[i]["Id"].ToString();
                    TCardBaseInfo.cardno = dtTCardBaseInfo.Rows[i]["CardNo"].ToString();
                    TCardBaseInfo.action = dtTCardBaseInfo.Rows[i]["ACTION"].ToString();
                    TCardBaseInfo.indate1 = dtTCardBaseInfo.Rows[i]["Indate1"].ToString().Replace('.', '/');
                    //TCardBaseInfo.name1 = dtTCardBaseInfo.Rows[i]["Name"].ToString();

                    TCardBaseInfo.mailno = dtTCardBaseInfo.Rows[i]["Newmailno"].ToString();
                    TCardBaseInfo.maildate = dtTCardBaseInfo.Rows[i]["FactMailDate"].ToString().Replace('.','/');
                    //TCardBaseInfo.zip = dtTCardBaseInfo.Rows[i]["zip"].ToString();
                    //TCardBaseInfo.add1 = dtTCardBaseInfo.Rows[i]["add1"].ToString();
                    //TCardBaseInfo.add2 = dtTCardBaseInfo.Rows[i]["add2"].ToString();
                    //TCardBaseInfo.add3 = dtTCardBaseInfo.Rows[i]["add3"].ToString();
                    //TCardBaseInfo.kind = dtTCardBaseInfo.Rows[i]["Newway"].ToString();

                    //sqlhelp.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);
                    sqlhelp.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);

                    //if (!Update(TCardBaseInfo, sqlhelp.GetFilterCondition(), "mailno", "maildate", "zip", "add1", "add2", "add3", "kind"))
                    if (!Update(TCardBaseInfo, sqlhelp.GetFilterCondition(), "mailno", "maildate"))
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢卡片基本資料帶分頁
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Search(string strCondition, ref  DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [indate1],[action],[kind],[cardtype],[photo],[affinity],[id],");
                sbSql.Append("[cardno],[cardno2],[zip],[add1],[add2],[add3],[mailno],[n_card],");
                sbSql.Append("[maildate],[expdate],[expdate2],[seq],[custname],[name1],[name2],");
                sbSql.Append("[trandate],[card_file],[disney_code],[branch_id],[Merch_Code],");
                sbSql.Append("[monlimit],[is_LackCard],[Urgency_Flg],[IntoStore_Status],[IntoStore_Date],");
                sbSql.Append("[OutStore_Status],[OutStore_Date],[SelfPick_Type],[SelfPick_Date] ");
                sbSql.Append("FROM [tbl_Card_BaseInfo] ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append(" WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append("order by Maildate desc");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢卡片基本資料帶分頁
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchByCardNo(string strCondition, ref  DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID)
        {
            try
            {
                string sql = @"SELECT [action] 
                                      ,[id]
                                      ,[cardno]  
                                      ,[add1]
                                      ,[add2]
                                      ,[add3]
                                      ,[mailno]
                                      ,[maildate]
                                      ,[custname]
                                  FROM [tbl_Card_BaseInfo]";

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:卡片基本資料查詢By CardNo
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtEndCaseFlg"></param>
        /// <returns></returns>
        public static bool SearchByCardNo(ref  DataTable dtCardNo, string strCardNo)
        {
            try
            {
                string sql = @"SELECT [action] 
                                      ,[id]
                                      ,[cardno]  
                                      ,[add1]
                                      ,[mailno]
                                      ,[maildate]
                                      ,[custname]
                                  FROM [tbl_Card_BaseInfo] where [cardno] =@cardno";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parmKey = new SqlParameter("@" + Entity_CardBaseInfo.M_cardno, strCardNo);
                sqlcmd.Parameters.Add(parmKey);
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardNo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:卡片基本資料查詢-修改時用
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/03
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtPost"></param>
        /// <returns></returns>
        public static bool SearchByCardNo(string strCondition, ref  DataTable dtCardBaseInfo, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [indate1],[action],[kind],[cardtype],[photo],[affinity]");
                sbSql.Append(",[id],[cardno],[cardno2],[zip],[add1],[add2],[add3],[mailno],[n_card]");
                sbSql.Append(",[maildate],[expdate],[expdate2],[seq],[custname],[name1],[name2]");
                sbSql.Append(",[trandate],[card_file],[disney_code],[branch_id],[Merch_Code],[monlimit]");
                sbSql.Append(",[is_LackCard],[Urgency_Flg],[IntoStore_Status],[IntoStore_Date],[OutStore_Status]");
                sbSql.Append(",[OutStore_Date],[SelfPick_Type],[SelfPick_Date] ");
                sbSql.Append("FROM [tbl_Card_BaseInfo] ");

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append(" WHERE ");
                    sbSql.Append(strCondition);
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，製卡日判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(Entity_CardBaseInfo TCardBaseInfo)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();

                Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno.Replace("-", ""));
                Sql.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);

                string strSql = @"SELECT COUNT(*) FROM [tbl_Card_BaseInfo]";
                strSql = strSql + " WHERE " + Sql.GetFilterCondition().Remove(0, 4);

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSql;
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
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
                BRM_Post.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，Action判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatByNo(Entity_CardBaseInfo TCardBaseInfo)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
                Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
                Sql.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);
                string strCondition = Sql.GetFilterCondition();

                string sql = @"SELECT COUNT(*) FROM [tbl_Card_BaseInfo]";
                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
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
                BRM_Post.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，Action，轉檔日判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatByAll(Entity_CardBaseInfo TCardBaseInfo)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
            Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
            Sql.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);
            Sql.AddCondition(Entity_CardBaseInfo.M_trandate, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.trandate);

            string strCondition = Sql.GetFilterCondition();

            string sql = @"SELECT COUNT(*) FROM [tbl_Card_BaseInfo]";
            if (strCondition != "")
            {
                strCondition = strCondition.Remove(0, 4);
                sql += " where " + strCondition;
            }

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
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

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，Action，轉檔日判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatByCard(Entity_CardBaseInfo TCardBaseInfo)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
            Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
            Sql.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.action);

            if (BRM_TCardBaseInfo.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 功能說明:根據身份證字號，卡號，Action 判断是否有卡片資料
        /// 作    者:Linda
        /// 創建時間:2010/10/26
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">string strId,string strCardNo,string strAction</param>
        /// <returns>Exist true</returns>
        public static bool IsExistByCard(string strId,string strCardNo,string strAction)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, strId);
            Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, strCardNo);
            Sql.AddCondition(Entity_CardBaseInfo.M_action, Operator.Equal, DataTypeUtils.String, strAction);

            string strCondition = Sql.GetFilterCondition();

            string sql = @"SELECT * FROM [tbl_Card_BaseInfo]";

            if (strCondition != "")
            {
                strCondition = strCondition.Remove(0, 4);
                sql += " where " + strCondition;
            }

            sql += " AND indate1>convert(varchar(10),dateAdd(day,-2,getdate()),111) ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            DataSet ds = BRM_Post.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }



        /// <summary>
        /// 功能說明:根據郵寄日與掛號號碼前6碼判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        //public static bool IsRepeatFor0111(Entity_CardBaseInfo TCardBaseInfo)
        //{
        //    try
        //    {
        //        SqlHelper Sql = new SqlHelper();

        //        Sql.AddCondition(Entity_CardBaseInfo.M_maildate, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.maildate);
        //        Sql.AddCondition(Entity_CardBaseInfo.M_mailno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.mailno);

        //        if (BRM_TCardBaseInfo.Search(Sql.GetFilterCondition()).Count > 0)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        BRM_TCardBaseInfo.SaveLog(ex);
        //    }
        //    return false;
        //}

        /// <summary>
        /// 功能說明:根據郵寄日與掛號號碼前6碼判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatFor0111(string strCondition)
        {
            try
            {
                string sql = @"SELECT count(action) FROM [dbo].[tbl_Card_BaseInfo] (nolock) ";

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                //SqlCommand sqlcmd = new SqlCommand();
                //sqlcmd.CommandType = CommandType.Text;
                //sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sql);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    int iCount = Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString());
                    if (iCount > 0)
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }


        /// <summary>
        /// 功能說明:根據郵寄日與掛號號碼前6碼判断是否有重復的卡片資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/27
        /// 修改記錄:
        /// </summary>
        /// <param name="Entity_CardBaseInfo">Entity_CardBaseInfo</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeatFor0111_1(string strCondition)
        {
            try
            {
                string sql = @"SELECT count(action) FROM [dbo].[tbl_Card_BaseInfo] (nolock) ";

                if (strCondition != "")
                {
//                    strCondition = strCondition.Remove(0, 4);
                    sql += " where " + strCondition;
                }

                //SqlCommand sqlcmd = new SqlCommand();
                //sqlcmd.CommandType = CommandType.Text;
                //sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardBackInfo.SearchOnDataSet(sql);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    int iCount = Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString());
                    if (iCount > 0)
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }


        /// <summary>
        /// 功能說明:卡片查詢多筆
        /// 作    者:zhiyuan
        /// 創建時間:2010/05/24
        /// 修改記錄:
        /// </summary>
        /// <param name="dtEndCaseFlg"></param>
        /// <returns></returns>
        public static bool selectByCardChangeInfo(ref  DataTable dtEndCaseFlg, string strCardNo)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                String[] arr = strCardNo.Split(',');
                string sql = "select indate1,id,custname,cardno,trandate from tbl_Card_BaseInfo where id in(select id from tbl_Card_BaseInfo where cardno in (";
                for (int i = 0; i < arr.Length; i++)
                {
                    sql += (i == 0 ? "" : ",") + "@strCardNo" + i;
                    sqlcmd.Parameters.Add(new SqlParameter("@strCardNo" + i, arr[i].Replace("'", "")));
                }
                sql += "))";
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd);
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    dtEndCaseFlg = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }

        public static bool SelectByCardChangeInfo(ref  DataTable dtBaseInfo, string strCardNo, string strTranDate)
        {
            try
            {
                string sql = "select indate1,id,custname,cardno,trandate from tbl_Card_BaseInfo where cardno=@cardno and trandate=@trandate and action='5' and isnull(mailno,'')='' ";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@cardno", strCardNo));
                sqlcmd.Parameters.Add(new SqlParameter("@trandate", strTranDate));
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd);
                if (ds!=null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dtBaseInfo = ds.Tables[0];
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
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明: 查詢卡號是否重複(一直開啟連接，知道查詢結束)
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDetail">含有流水號欄的DataTable</param>
        /// <param name="strColName">需判斷重複值的欄位名稱</param>
        /// <returns>true:無重複</returns>
        //public static bool BatIsRepeatByColName(DataTable dtDetail, string strColName)
        //{
        //    bool bReturn = false;
        //    SqlConnection sqlConn = null;
        //    SqlCommand sqlCmd = null;
        //    SqlParameter parm = new SqlParameter();
        //    int iRowNum = 0;

        //    try
        //    {
        //        string strSql = string.Format(@"select top 1 {0} FROM tbl_Card_BaseInfo WHERE {1}=@{2}", strColName, strColName, strColName);
        //        sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection_System"].ToString());
        //        sqlCmd = new SqlCommand();
        //        sqlCmd.Connection = sqlConn;
        //        sqlCmd.CommandType = CommandType.Text;
        //        parm.ParameterName = "@" + strColName;

        //        sqlConn.Open();

        //        foreach (DataRow drData in dtDetail.Rows)
        //        {
        //            iRowNum = 0;
        //            sqlCmd.Parameters.Clear();
        //            parm.Value = drData[strColName].ToString();
        //            sqlCmd.Parameters.Add(parm);
        //            sqlCmd.CommandText = strSql;

        //            iRowNum = sqlCmd.ExecuteNonQuery();

        //            if (iRowNum <= 0)
        //            {
        //                BRM_TCardBaseInfo.SaveLog("退件新增檔案卡號" + drData[strColName].ToString() + "在卡片基本資料表中不存在");
        //            }
        //        }
        //        if (iRowNum > 0)
        //        {
        //            bReturn = true;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        BRM_TCardBaseInfo.SaveLog(ex.Message);
        //    }
        //    finally
        //    {
        //        if (sqlCmd != null)
        //        {
        //            sqlCmd.Dispose();
        //        }
        //        if (sqlConn != null)
        //        {
        //            sqlConn.Close();
        //            sqlConn.Dispose();
        //        }
        //    }
        //    return bReturn;
        //}
        public static bool BatIsRepeatByColName(DataTable dtDetail, string strColName1, string strColName2)
        {
            bool bReturn = false;
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlParameter parm1 = new SqlParameter();
            SqlParameter parm2 = new SqlParameter();
            int iRowNum = 0;

            try
            {
                string strSql = string.Format(@"select count(0) FROM tbl_Card_BaseInfo WHERE {0}=@{0} and {1}=@{1}", strColName1, strColName2);
                sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;
                parm1.ParameterName = "@" + strColName1;
                parm2.ParameterName = "@" + strColName2;

                sqlConn.Open();

                foreach (DataRow drData in dtDetail.Rows)
                {
                    iRowNum = 0;
                    sqlCmd.Parameters.Clear();
                    parm1.Value = drData[strColName1].ToString();
                    parm2.Value = drData[strColName2].ToString();
                    sqlCmd.Parameters.Add(parm1);
                    sqlCmd.Parameters.Add(parm2);
                    sqlCmd.CommandText = strSql;

                    iRowNum = Int32.Parse(sqlCmd.ExecuteScalar().ToString());

                    if (iRowNum <= 0)
                    {
                        BRM_TCardBaseInfo.SaveLog("退件新增檔案卡號" + drData["CardNo"].ToString() + "在卡片基本資料表中不存在");
                    }
                }
                if (iRowNum > 0)
                {
                    bReturn = true;
                }
            }
            catch (System.Exception ex)
            {
                BRM_TCardBaseInfo.SaveLog(ex.Message);
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
        /// 功能說明: 按數據集查詢基本資料檔，查詢結果填充到EntitySet中并返回
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDetail"></param>
        /// <param name="strImportFileName"></param>
        /// <returns></returns>
        public static EntitySet<Entity_CardBackInfo> GetBackInfoFor0110(DataTable dtDetail, string strImportFileName)
        {
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlDataAdapter sdaBascInfo = new SqlDataAdapter();
            SqlParameter parmCardNo = new SqlParameter();
            SqlParameter parmAction = new SqlParameter();
            DataTable dtTmp = new DataTable();

            EntitySet<Entity_CardBackInfo> SetCardChange = new EntitySet<Entity_CardBackInfo>();

            try
            {
                string strSql = "SELECT top 1 id,custname,trandate,cardtype,add1,add2,add3 FROM tbl_Card_BaseInfo WHERE cardno=@CardNo and action=@Action ORDER BY trandate DESC ";
                sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = strSql;
                parmCardNo.ParameterName = "@CardNo";
                parmAction.ParameterName = "@Action";

                sqlConn.Open();
                for (int i = 0; i < dtDetail.Rows.Count; i++)
                {
                    sqlCmd.Parameters.Clear();
                    dtTmp.Rows.Clear();
                    parmCardNo.Value = dtDetail.Rows[i]["CardNo"].ToString();
                    parmAction.Value = dtDetail.Rows[i]["Action"].ToString();
                    sqlCmd.Parameters.Add(parmCardNo);
                    sqlCmd.Parameters.Add(parmAction);
                    sdaBascInfo = new SqlDataAdapter(sqlCmd);
                    sdaBascInfo.Fill(dtTmp);

                    if (dtTmp.Rows.Count > 0)
                    {
                        Entity_CardBackInfo CardBackInfo = new Entity_CardBackInfo();
                        CardBackInfo.serial_no = dtDetail.Rows[i]["Serial_no"].ToString();    //流水號
                        CardBackInfo.Id = dtTmp.Rows[0]["id"].ToString();   //身份證字號
                        CardBackInfo.CustName = dtTmp.Rows[0]["custname"].ToString();   //姓名
                        CardBackInfo.Kind = dtDetail.Rows[i]["Kind"].ToString();    //退件類別
                        CardBackInfo.CardNo = dtDetail.Rows[i]["CardNo"].ToString();    //卡號
                        CardBackInfo.Action = dtDetail.Rows[i]["Action"].ToString();    //action
                        CardBackInfo.Trandate = dtTmp.Rows[0]["trandate"].ToString();   //轉當日
                        CardBackInfo.cardtype = dtTmp.Rows[0]["cardtype"].ToString();   //卡種
                        CardBackInfo.Backdate = DateHelper.InsertTimeSpan(DateHelper.ConvertToAD(dtDetail.Rows[i]["Serial_no"].ToString().Substring(1, 7))) ;  //退件日期
                        CardBackInfo.Reason = dtDetail.Rows[i]["Reason"].ToString();    //退件原因
                        CardBackInfo.Madd1 = dtTmp.Rows[0]["add1"].ToString();    //郵寄地址1
                        CardBackInfo.Madd2 = dtTmp.Rows[0]["add2"].ToString();    //郵寄地址2
                        CardBackInfo.Madd3 = dtTmp.Rows[0]["add3"].ToString();    //郵寄地址3
                        CardBackInfo.ImportDate = DateTime.Now.ToString("yyyy/MM/dd");
                        CardBackInfo.ImportFileName = strImportFileName;
                        CardBackInfo.CardBackStatus = "0";
                        CardBackInfo.OriginalDBflg = "0";
                        CardBackInfo.Exp_Count = "0";
                        CardBackInfo.Enditem = "";
                        if (!BRM_CardBackInfo.IsRepeatBySno(CardBackInfo.serial_no))
                        {
                            SetCardChange.Add(CardBackInfo);
                        }
                    }
                    else
                    {
                        SetCardChange.Clear();
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                BRM_TCardBaseInfo.SaveLog(ex.Message);
            }
            finally
            {
                if (sdaBascInfo != null)
                {
                    sdaBascInfo.Dispose();
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
            return SetCardChange;
        }

        /// <summary>
        /// 功能說明:查詢招領資料For 0115_1
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetPOutData(ref DataTable dtDetail)
        {
            try
            {
                string sql = @"SELECT ID,CardNo,b.Maildate,b.Mailno,cardtype, M_date,b.Exp_Count,b.Exp_Date From
                                      tbl_Card_BaseInfo t inner join tbl_Post_Send b
                                      on t.maildate=b.Maildate And t.mailno=b.Mailno
                                      Where cardtype<>'018' and cardtype<>'019' And 
                                      Imp_date=@Imp_date And isnull(Exp_Count,'0')=0
                                      And Info1='240' And Send_status_Code='G2'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                //改為工作日
                //SqlParameter parm = new SqlParameter("@Imp_date", DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
                SqlParameter parm = new SqlParameter("@Imp_date", DateTime.ParseExact(BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd"));
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
    }
}
