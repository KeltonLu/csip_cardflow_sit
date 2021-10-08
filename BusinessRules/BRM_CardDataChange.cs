//******************************************************************
//*  功能說明：尚未退回轉出的異動作業單
//*  作    者：Simba Liu
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//  Joe               20210120        RQ-2019-008159-003     配合長姓名作業修改
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


namespace BusinessRules
{
    public class BRM_CardDataChange : BRBase<Entity_CardDataChange>
    {


        /// <summary>
        /// 功能說明:新增一筆尚未退回轉出的異動作業單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="Post"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Insert(Entity_CardDataChange CardDataChange, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CardDataChange.AddNewEntity(CardDataChange))
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
                BRM_Post.SaveLog(exp.Message);
                strMsgID = "06_06040100_002";
                return false;
            }
        }



        /// <summary>
        /// 功能說明:更新一筆尚未退回轉出的異動作業單
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="CardDataChange"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(Entity_CardDataChange CardDataChange, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CardDataChange.UpdateEntityByCondition(CardDataChange, strCondition, FiledSpit))
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
                BRM_CardDataChange.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆異動作業單，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool update(Entity_CardDataChange CardDataChange, string strCondition, params  string[] FiledSpit)
        {
            try
            {
                if (BRM_CardDataChange.UpdateEntityByCondition(CardDataChange, strCondition, FiledSpit))
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
                BRM_CardDataChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:自動化異動回饋檔匯入更新異動作業單，事務專用
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/09
        /// 修改記錄:
        /// </summary>
        /// <param name="dtCardDataChange"></param>
        /// <returns></returns>
        public static bool BatUpdateFor0105(DataTable dtCardDataChange)
        {
            OMTransactionScope ts = new OMTransactionScope();
            try
            {
                bool blnResult = true;
                for (int i = 0; i < dtCardDataChange.Rows.Count; i++)
                {
                    SqlHelper sqlhelp = new SqlHelper();
                    Entity_CardDataChange CardDataChange = new Entity_CardDataChange();

                    CardDataChange.CardNo = dtCardDataChange.Rows[i]["CardNo"].ToString().Trim();
                    CardDataChange.ParentSno = dtCardDataChange.Rows[i]["SNO"].ToString().Trim();

                    CardDataChange.OutputFlg = "S";

                    //*更新條件設置 CardNo ParentSno OutputFileName
                    sqlhelp.AddCondition(Entity_CardDataChange.M_CardNo, Operator.Equal, DataTypeUtils.String, CardDataChange.CardNo);
                    sqlhelp.AddCondition(Entity_CardDataChange.M_ParentSno, Operator.Equal, DataTypeUtils.String, CardDataChange.ParentSno);

                    if (!update(CardDataChange, sqlhelp.GetFilterCondition(), "OutputFlg"))
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
                BRM_CardDataChange.SaveLog(exp.Message);
                ts.Dispose();
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢異動作業單卡片資料帶分頁
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/17
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
                sbSql.Append("SELECT * FROM( ");
                sbSql.Append("SELECT Distinct Cbase.[indate1],Cbase.[id],Cbase.[CardNo],Cbase.[action] ");
                sbSql.Append(",Cbase.[Kind],Cbase.[Trandate],Cbase.[MailDate],Cbase.[MailNo],Cbase.[photo] ");
                sbSql.Append(",Cbase.[Urgency_Flg],Cbase.[affinity], '' as TYPE_flg ");
                sbSql.Append(" FROM ");
                sbSql.Append("[dbo].[tbl_Card_BaseInfo] Cbase  ");
                sbSql.Append(" UNION ALL ");
                sbSql.Append("SELECT Distinct CData.[indate1],CData.[id],CData.[CardNo],CData.[action] ");
                sbSql.Append(",Cbase.[Kind],Cbase.[Trandate],Cbase.[MailDate],Cbase.[MailNo],Cbase.[photo] ");
                sbSql.Append(",Cbase.[Urgency_Flg],Cbase.[affinity],case when CData.[Type_flg] IS NULL then '' else CData.[Type_flg]  end as TYPE_flg ");
                sbSql.Append(" FROM ");
                sbSql.Append("[dbo].[tbl_Card_DataChange]  CData left join ");
                sbSql.Append("[dbo].[tbl_Card_BaseInfo] Cbase ON ");
                sbSql.Append("CData.id =Cbase.id AND ");
                sbSql.Append("CData.CardNo = Cbase.CardNo AND ");
                sbSql.Append("CData.action = Cbase.action ");
                sbSql.Append(" where ");
                sbSql.Append("CData.Type_flg = 'A' ");
                sbSql.Append(" ) T1 ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append(" WHERE ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append(" ORDER BY T1.Maildate DESC");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    strMsgID = "06_02020000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02020000_005";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }



        /// <summary>
        /// 功能說明:查詢異動作業單卡片資料帶分頁
        /// 作    者:HAO CHEN
        /// 創建時間:2010/06/25
        /// 修改記錄:
        /// </summary>
        /// <param name="sql">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardBaseInfo"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool SearchFor0203(string strCondition, ref  DataTable dtCardBaseInfo, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID, bool isReport)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT Distinct a.id,a.CardNo,a.action,a.Trandate ");
                sbSql.Append(",[NewName],[NewMailDate],[NewMonlimit],[NewWay],[newmailno],[NewZip] ");
                sbSql.Append(",CASE OutputFlg WHEN 'N' THEN '未處理' WHEN 'Y' THEN '已處理' WHEN 'T' THEN ");
                sbSql.Append("'退單' WHEN 'S' THEN '成功' ELSE '' END AS OutputFlg ");
                sbSql.Append(",[CNote],[UpdUser],[UpdDate],[OutputFileName],'' as Datachange,[sno]");
                sbSql.Append("FROM [dbo].[tbl_Card_DataChange] a ");
                sbSql.Append("left join tbl_Card_BaseInfo b ");
                sbSql.Append("on a.Action=b.Action and a.Id=b.Id and ");
                sbSql.Append("a.Cardno=b.Cardno and a.Trandate=b.Trandate");
                sbSql.Append(" WHERE ");
                sbSql.Append("[SourceType] is null ");

                if (!string.IsNullOrEmpty(strCondition))
                {
                    sbSql.Append(strCondition);
                }
                sbSql.Append(" ORDER BY Sno DESC");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();

                DataSet ds = isReport ? BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd) : BRM_TCardBaseInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    strMsgID = "06_02030000_004";
                    return true;
                }
                else
                {
                    strMsgID = "06_02030000_003";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_TCardBaseInfo.SaveLog(exp.Message);
                strMsgID = "06_02020000_005";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢卡號是否存在(一直開啟連接，直到查詢結束)
        ///          如果不存在則做退單處理。   
        /// 作    者:HAO CHEN 
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtDetail">異動檔DataTable</param>
        /// <returns>void</returns>
        public static void SetCardOutputFlg(DataTable dtDetail)
        {
            SqlConnection sqlConn = null;
            SqlCommand sqlCmd = null;
            SqlParameter parmId = new SqlParameter();
            SqlParameter parmCardNo = new SqlParameter();
            SqlParameter parmAction = new SqlParameter();
            SqlParameter parmTrandate = new SqlParameter();

            string strUpdDate = string.Empty;
            string strUpdDateTime=string.Empty;
            string strNowDate=DateTime.Now.ToString("yyyy/MM/dd");
//            string strNowDateTime=DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd")+"15:00";
            string strNowDateTime = DateTime.ParseExact(BRWORK_DATE.ADD_WORKDAY("06", DateTime.Now.ToString("yyyyMMdd"), -1), "yyyyMMdd", null).ToString("yyyy/MM/dd") + "15:00";

            string strAction = string.Empty;
            string OutputFlg = "N";
            string strBindate1 = "";
            string Is_LackCard = "";

            try
            {
                //--------------------------------------------------
                //foreach (DataRow drData in dtDetail.Rows)
                //{
                //    strUpdDate = drData["UpdDate"].ToString();
                //    strUpdDateTime = drData["UpdDate"].ToString() + drData["UpdTime"].ToString();
                //    strAction = drData["Action"].ToString();
                //    strBindate1 = drData["Bindate1"].ToString();
                //    Is_LackCard = drData["Is_LackCard"].ToString();
                //    OutputFlg = "N";

                //    //作業單無法比對到可異動卡片且：1）作業單卡別為新卡/年度換卡，且異動日期<匯檔當日，則退單
                //    if ((strAction == "1" || strAction == "5") && string.Compare(strUpdDate, strNowDate) < 0)
                //    {
                //        //*更新異動作業單為退單
                //        SqlHelper sql = new SqlHelper();
                //        Entity_CardDataChange eCardDataChange = new Entity_CardDataChange();
                //        eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                //        eCardDataChange.OutputFlg = "T";
                //        sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                //        update(eCardDataChange, sql.GetFilterCondition(), "OutputFlg");
                //    }
                //    //作業單無法比對到可異動卡片且：2）作業單卡別為掛補/毀補，且異動時間<匯檔前一日15:00，則退單
                //    if ((strAction == "2" || strAction == "3") && string.Compare(strUpdDateTime, strNowDateTime) < 0)
                //    {
                //        //*更新異動作業單為退單
                //        SqlHelper sql = new SqlHelper();
                //        Entity_CardDataChange eCardDataChange = new Entity_CardDataChange();
                //        eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                //        eCardDataChange.OutputFlg = "T";
                //        sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                //        update(eCardDataChange, sql.GetFilterCondition(), "OutputFlg");
                //    }
                //}
                //------------------------------------------------------------------
                foreach (DataRow drData in dtDetail.Rows)
                {
                    strUpdDate = drData["UpdDate"].ToString();
                    strUpdDateTime = drData["UpdDate"].ToString() + drData["UpdTime"].ToString();
                    strAction = drData["Action"].ToString();
                    strBindate1 = drData["Bindate1"].ToString();
                    Is_LackCard = drData["Is_LackCard"].ToString();
                    OutputFlg = "N";
                    if (!Is_LackCard.Equals("0"))
                    {
                        SqlHelper sql;
                        Entity_CardDataChange eCardDataChange;
                        if (((strAction == "1") || (strAction == "5")) && strBindate1.Equals(""))
                        {
                            OutputFlg = "T";
                            sql = new SqlHelper();
                            eCardDataChange = new Entity_CardDataChange();
                            eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                            eCardDataChange.OutputFlg = "T";
                            sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                            update(eCardDataChange, sql.GetFilterCondition(), new string[] { "OutputFlg" });
                        }
                        //作業單無法比對到可異動卡片且：1）作業單卡別為新卡/年度換卡，且異動日期<匯檔當日，則退單
                        else if (((strAction == "1") || (strAction == "5")) && (string.Compare(strUpdDate, strBindate1) < 0))
                        {
                            OutputFlg = "T";
                            sql = new SqlHelper();
                            eCardDataChange = new Entity_CardDataChange();
                            eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                            eCardDataChange.OutputFlg = "T";
                            sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                            update(eCardDataChange, sql.GetFilterCondition(), new string[] { "OutputFlg" });
                        }
                        //作業單無法比對到可異動卡片且：2）作業單卡別為掛補/毀補，且異動時間<匯檔前一日15:00，則退單
                        if (((strAction == "2") || (strAction == "3")) && (string.Compare(strUpdDateTime, strNowDateTime) < 0))
                        {
                            //*更新異動作業單為退單
                            OutputFlg = "T";
                            sql = new SqlHelper();
                            eCardDataChange = new Entity_CardDataChange();
                            eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                            eCardDataChange.OutputFlg = "T";
                            sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                            update(eCardDataChange, sql.GetFilterCondition(), new string[] { "OutputFlg" });
                        }
                        if ((((strAction != "2") && (strAction != "3")) && OutputFlg.Equals("N")) && strBindate1.Equals(""))
                        {
                            //*更新異動作業單為退單
                            OutputFlg = "T";
                            sql = new SqlHelper();
                            eCardDataChange = new Entity_CardDataChange();
                            eCardDataChange.Sno = int.Parse(drData["Sno"].ToString());
                            eCardDataChange.OutputFlg = "T";
                            sql.AddCondition(Entity_CardDataChange.M_Sno, Operator.Equal, DataTypeUtils.Integer, eCardDataChange.Sno.ToString());
                            update(eCardDataChange, sql.GetFilterCondition(), new string[] { "OutputFlg" });
                        }
                    }
                }


            }
            catch (System.Exception ex)
            {
                BRM_CardDataChange.SaveLog(ex.Message);
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
        }

        /// <summary>
        /// 功能說明:查詢異動作業檔未退單的卡片資料并且去重
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/19
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchForCard(ref DataTable dtCardDataChange)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT T1.Sno,T1.id,T1.CardNo,T1.ACTION,T1.Trandate,T1.UpdDate,T1.UpdTime ");
                sbSql.Append("FROM tbl_Card_DataChange T1 ");
                sbSql.Append("Where T1.OutputFlg='N' and isnull(T1.Type_flg,'')='A' and isnull(T1.indate1,'')='' ");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardDataChange = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDataChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:查詢異動作業檔未退單的卡片資料并且去重
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/19
        /// 修改記錄:2020/12/22_Ares_Stanley-修正使用Int32取代Int16避免長度不足問題
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static int GetMaxSno()
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT max(convert(int,ParentSno)) AS Psno ");
                sbSql.Append("FROM   tbl_Card_DataChange ");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (null != ds)
                {
                    if (null != ds.Tables[0].Rows[0][0] && !string.IsNullOrEmpty(ds.Tables[0].Rows[0][0].ToString()))
                    {
                        //return Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString());
                        return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDataChange.SaveLog(exp.Message);
                return -1;
            }
        }


        /// <summary>
        /// 功能說明:尚未退回轉出的異動作業單
        /// 作    者:Simba Liu
        /// 創建時間:2010/05/19
        /// 修改記錄:
        /// 2021/01/21 Joe 增加羅馬拼音
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchCardDataChange(ref  DataTable dtCardDataChange)
        {
            try
            {
                string sql = @"SELECT distinct T2.indate1, --制卡日
                                        T2.id,--身分證字號
                                        T2.custName,--客戶姓名
                                        T1.NewName,--新姓名
                                        T2.custname_roma,--客戶姓名_羅馬拼音
                                        T1.NewName_Roma,--新姓名_羅馬拼音
                                        T1.OldName_Roma,--舊姓名_羅馬拼音
                                        T2.CardNo,--卡號
                                        T1.NewMonlimit,--新額度
                                        T2.Kind as oldway,--原取卡方式 改為取baseinfo
                                        T2.Urgency_Flg,--緊急製卡
                                        T1.UrgencyFlg as NewUrgencyFlg, --異動後緊急製卡
                                        T2.Mailno,--原掛號號碼
                                        T1.newway,--新取卡方式
                                        T1.newmailno,--新掛號號碼
                                        T1.NewMailDate,--郵寄日期
                                        T1.NewZip,--ZIP
                                        T1.NewAdd1,--地址一
                                        T1.NewAdd2,--地址二
                                        T1.NewAdd3,--地址三
                                        T2.photo,--PHOTO
                                        T2.ACTION,--ACTION
                                        T1.SNo,--SNO
                                        T2.Merch_Code,
                                        T2.Kind,
                                        T1.UpdDate,
                                        T1.OldName,
                                        T1.Oldmonlimit,
                                        T1.OldAdd1,
                                        T1.OldAdd2,
                                        T1.OldAdd3,
                                        T1.OldMailDate 
                                        FROM (select * from tbl_Card_DataChange where isnull(OutputFlg,'')='N'
                                        and isnull(SourceType,'') not in('1','2','3')) T1 left join
                                        tbl_Card_BaseInfo T2
                                        on T1.action=T2.action and T1.ID=T2.ID and T1.cardno=T2.cardno and T1.Trandate=T2.trandate
                                        where T2.indate1 is not null and T2.id is not null and T2.custName is not null 
                                        and T2.is_LackCard = '1' ";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardDataChange = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDataChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:綜合資料處理修改-查詢異動單
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardDataChange"></param>
        /// <returns></returns>
        public static bool SearchByCardNo(string strCondition, ref  DataTable dtCardDataChange, ref string strMsgID)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [Sno],[ParentSno],[indate1],[id],[CardNo],[action]");
                sbSql.Append(",[Trandate],[OldName],[NewName],[OldMailDate],[NewMailDate]");
                sbSql.Append(",[OldMonlimit],[NewMonlimit],[OldWay],[NewWay],[OldSelfPickType]");
                sbSql.Append(",[NewSelfPickType],[enditem],[newitem],[oldMailno],[newmailno]");
                sbSql.Append(",[OldZip],[OldAdd1],[OldAdd2],[OldAdd3],[NewZip],[NewAdd1],[NewAdd2]");
                sbSql.Append(",[NewAdd3],[CNote],[photo],[UpdUser],[UpdDate],[UpMark],[NoteCaptions]");
                sbSql.Append(",[CloseFlg],[MerchCode],[UrgencyFlg],[OutputFlg],[OutputDate],[OutputFileName]");
                sbSql.Append(",[SourceType],[Type_flg] ");
                //增加異動來源欄位 start
                sbSql.Append(",case isnull([SourceType],'') when '2' then '退件' when '' then '作業單' end Source ");
                //增加異動來源欄位 end
                sbSql.Append("FROM [tbl_Card_DataChange] ");

                if (strCondition != "")
                {
                    strCondition = strCondition.Remove(0, 4);
                    sbSql.Append(" where ");
                    sbSql.Append(strCondition);
                }
                sbSql.Append(" order by Sno desc");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardDataChange = ds.Tables[0];
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
                BRM_CardDataChange.SaveLog(exp.Message);
                strMsgID = "06_06040100_008";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:綜合資料處理修改-是否已有異動作業單
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardDataChange"></param>
        /// <returns></returns>
        public static bool CheckByChange(string strCondition, ref  DataTable dtCardDataChange)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [Sno],[ParentSno],[indate1],[id],[CardNo],[action],[Trandate],[OldName]");
                sbSql.Append(",[NewName],[OldMailDate],[NewMailDate],[OldMonlimit],[NewMonlimit],[OldWay]");
                sbSql.Append(",[NewWay],[OldSelfPickType],[NewSelfPickType],[enditem],[newitem],[oldMailno]");
                sbSql.Append(",[newmailno],[OldZip],[OldAdd1],[OldAdd2],[OldAdd3],[NewZip],[NewAdd1],[NewAdd2]");
                sbSql.Append(",[NewAdd3],[CNote],[photo],[UpdUser],[UpdDate],[UpMark],[NoteCaptions],[CloseFlg]");
                sbSql.Append(",[MerchCode],[UrgencyFlg],[OutputFlg],[OutputDate],[OutputFileName],[SourceType]");
                sbSql.Append(" FROM [tbl_Card_DataChange] where OutputFlg='N' ");

                if (strCondition != "")
                {
                    sbSql.Append(strCondition);
                }
                sbSql.Append(" order by Sno desc");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardDataChange = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDataChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:綜合資料處理修改-是否已有異動作業單
        /// 作    者:Simba Liu
        /// 創建時間:2010/06/04
        /// 修改記錄:
        /// </summary>
        /// <param name="strCondition">SQL語句，若遇到條件查詢需要拼寫SQL</param>
        /// <param name="dtCardDataChange"></param>
        /// <returns></returns>
        public static bool SearchByChange(string strCondition, ref  DataTable dtCardDataChange, ref string strMsgID, string strField)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT [Sno],[ParentSno],[indate1],[id],[CardNo],[action],[Trandate],[OldName]");
                sbSql.Append(",[NewName],[OldMailDate],[NewMailDate],[OldMonlimit],[NewMonlimit],[OldWay]");
                sbSql.Append(",[NewWay],[OldSelfPickType],[NewSelfPickType],[enditem],[newitem],[oldMailno]");
                sbSql.Append(",[newmailno],[OldZip],[OldAdd1],[OldAdd2],[OldAdd3],[NewZip],[NewAdd1],[NewAdd2]");
                sbSql.Append(",[NewAdd3],[CNote],[photo],[UpdUser],[UpdDate],[UpMark],[NoteCaptions],[CloseFlg]");
                sbSql.Append(",[MerchCode],[UrgencyFlg],[OutputFlg],[OutputDate],[OutputFileName],[SourceType],[BaseFlg]");
                //2020/12/30 Joe 新增欄位:羅馬拼音 BEGIN
                //舊姓名_羅馬拼音,新姓名_羅馬拼音
                sbSql.Append(",[OldName_Roma],[NewName_Roma]");
                //2020/12/30 Joe 新增欄位:羅馬拼音 END
                sbSql.Append(" FROM [tbl_Card_DataChange] where OutputFlg='N' and isnull(SourceType,'') not in ('1','2','3')");

                if (strField != "")
                {
                    sbSql.Append(" AND ");
                    sbSql.Append(strField);
                    sbSql.Append(" IS NOT NULL ");
                }
                if (strCondition != "")
                {
                    sbSql.Append(strCondition);
                }
                sbSql.Append(" order by Sno desc");

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sbSql.ToString();
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardDataChange = ds.Tables[0];
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
                BRM_CardDataChange.SaveLog(exp.Message);
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
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(Entity_CardBaseInfo.M_id, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.id);
            Sql.AddCondition(Entity_CardBaseInfo.M_cardno, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.cardno);
            Sql.AddCondition(Entity_CardBaseInfo.M_indate1, Operator.Equal, DataTypeUtils.String, TCardBaseInfo.indate1);

            if (BRM_TCardBaseInfo.Search(Sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 功能說明:查詢重寄異動紀錄檔For 0115_4
        /// 作    者:zhiyuan
        /// 創建時間:2010/06/08
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <returns></returns>
        public static bool GetChgData(ref DataTable dtDetail)
        {
            try
            {
                string sql = @"Select back.Id,back.Enddate as UpdDate,back.Enduid as UpdUser,
                            base.add1 as OldAdd1,base.add2 as OldAdd2,base.add3 as OldAdd3,
                            back.NewAdd1,back.NewAdd2,back.NewAdd3,back.Endtime
                            From tbl_Card_BackInfo back left join tbl_Card_BaseInfo base on
                            back.id=base.id and back.CardNo=base.CardNo and back.action=base.action and back.Trandate=base.Trandate 
                            Where back.Enditem in ('1','2','3','4') 
                            And back.EndFunction='0'
                            And back.Enddate=@UpdDate";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                SqlParameter parm = new SqlParameter("@UpdDate", DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
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
        /// 功能說明:查詢資料異動明細檔中轉出時間在匯檔日期區間內的資料
        /// 作    者:zhen chen
        /// 創建時間:2010/06/25
        /// 修改記錄:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strDateFrom"></param>
        /// <param name="strDateTo"></param>
        /// <returns></returns>
        public static bool SearchFileInfo(ref DataTable dtFileInfo, ref string strDateFrom, ref string strDateTo, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                string sql = @"select distinct OutputDate,OutputFileName,FilePath
                                from tbl_Card_DataChange
                                where  convert(datetime,OutputDate,120) between convert(datetime,@strDateFrom,120) and convert(datetime,@strDateTo,120)
                                       and FilePath is not null
                                union 
                                select distinct OutputDate,OutputFileName,FilePath
                                from tbl_CardChange
                                where convert(datetime,OutputDate,120) between convert(datetime,@strDateFrom,120) and convert(datetime,@strDateTo,120)
                                and FilePath is not null 
                                union 
                                select distinct OutputDate,OutputFileName,FilePath
                                from tbl_UnableCard
                                where convert(datetime,OutputDate,120) between convert(datetime,@strDateFrom,120) and convert(datetime,@strDateTo,120)
                                     and FilePath is not null
                                     order by OutputDate asc";

                SqlCommand sqlcmd = new SqlCommand();

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strDateFrom", strDateFrom));
                sqlcmd.Parameters.Add(new SqlParameter("@strDateTo", strDateTo));
                DataSet ds = BRM_CardDataChange.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);

                if (ds != null)
                {
                    dtFileInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDataChange.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明：導入大宗當，捕上制卡日
        /// </summary>
        /// <param name="CardDataChange"></param>
        /// <returns></returns>
        public static bool UpdateIndate(Entity_CardBaseInfo CardDataChange)
        {
            try
            {
                string strSql = "update tbl_Card_DataChange set indate1=@indate1,trandate=@trandate where indate1 is null and trandate='' and id=@id and cardno=@cardno and action=@action";
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = strSql;
                sqlCmd.Parameters.Add(new SqlParameter("@indate1", CardDataChange.indate1));
                sqlCmd.Parameters.Add(new SqlParameter("@trandate", CardDataChange.trandate));
                sqlCmd.Parameters.Add(new SqlParameter("@id", CardDataChange.id));
                sqlCmd.Parameters.Add(new SqlParameter("@cardno", CardDataChange.cardno));
                sqlCmd.Parameters.Add(new SqlParameter("@action", CardDataChange.action));
                if (BRM_CardDataChange.Update(sqlCmd))
                {
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                BRM_CardDataChange.SaveLog(ex.Message);
                return false;
            }
        }
    }
}
