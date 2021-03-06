//******************************************************************
//*  功能說明：卡片基本資料表業務邏輯層
//*  作    者：xiongxiaofeng
//*  創建日期：2013/06/15
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

namespace BusinessRules
{
    public class BRM_CardBaseInfo : BRBase<Entity_CardBaseInfo>
    {
        #region SQL語句
        #endregion
        /// <summary>
        /// 功能說明:獲取卡片基本檔資料
        /// 作    者:xiongxiaofeng
        /// 創建時間:2013/06/15
        /// 修改記錄:
        /// </summary>
        /// <param name="dtCardBaseInfo"></param>
        /// <returns></returns>
        public static bool SearchExportCardBaseInfo(ref DataTable dtCardBaseInfo,string mailDate)
        {
            try
            {
                //string sql = @"select  id, cardno,CardType,Photo,Affinity,MailDate,Kind
                //             from dbo.tbl_Card_BaseInfo where  action=1 and maildate=convert(varchar(100), GETDATE(), 111) ";
                string sql = @"select  id, cardno,CardType,Photo,Affinity,MailDate,Kind
                             from tbl_Card_BaseInfo (nolock)  where  action=1 and maildate='" + mailDate + "' ";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardBaseInfo.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCardBaseInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardBaseInfo.SaveLog(exp.Message);
                return false;
            }

        }

    }
}
