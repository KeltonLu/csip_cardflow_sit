using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Configuration;
using Framework.Common;
using Framework.Common.Logging;
using System.Reflection;

/// <summary>
/// BRCard_BaseInfo 的摘要描述
/// </summary>
public class BRCard_BaseInfo :CSIPCommonModel.BusinessRules.BRBase<EntityL_Card_BaseInfo>
{
    public BRCard_BaseInfo()
    {      
    }

    
    /// <summary>
    /// 以身分證字號集合及日期挑出要制卡的資料
    /// </summary>
    /// <param name="splidID"></param>
    /// <param name="intDate"></param>
    /// <returns></returns>
    public static bool GetBaseinfoByID(string splidID,string intDate, ref DataTable dtCardBaseInfo, ref string strMsgID)
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
            sbSql.Append(" WHERE  [indate1] = '" + intDate  + "' and [id] = '" + splidID  + "' ");  

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sbSql.ToString();
            DataSet ds = BRCard_BaseInfo.SearchOnDataSet(sqlcmd);
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
            BRCard_BaseInfo.SaveLog(exp.Message);
            strMsgID = "06_06040100_008";
            return false;
        }

    }

     
}