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

namespace BusinessRules
{
   public class BRM_CardDailyClose : BRBase<Entity_CardDailyClose>
    {
        /// <summary>
        /// �\�໡��:�鵲�H����-�d�ߤw�鵲���̫���
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetLastCloseDate(ref  DataTable dtLastCloseDate)
        {
            try
            {
                string sql = @"Select top 1 [DailyCloseDate] From [tbl_Card_DailyClose] Order by [DailyCloseDate] Desc";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardDailyClose.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtLastCloseDate = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// �\�໡��:�鵲�H����-�d�߬Y��O�_�w�g�鵲
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
       public static bool GetCloseInfo(string strDailyCloseDate, ref  DataTable dtCloseInfo)
        {
            try
            {

                // �M�ץN��:20200031-CSIP EOS �\�໡��:�B�z��w-CSIP EOS �@��:Ares Luke �Ыخɶ�:2020/08/19
                string sql = @"Select * From [tbl_Card_DailyClose] Where  [DailyCloseDate] = @strDailyCloseDate";
                SqlCommand sqlcmd = new SqlCommand {CommandType = CommandType.Text, CommandText = sql};
                SqlParameter paramDate = new SqlParameter("@strDailyCloseDate" , strDailyCloseDate);
                sqlcmd.Parameters.Add(paramDate);

                DataSet ds = BRM_CardDailyClose.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtCloseInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// �\�໡��:�鵲�H����-���J�鵲�H��
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
       public static bool InsertDailyClose(string strCloseDate)
        {
            try
            {
                string sql = @"insert into dbo.tbl_Card_DailyClose(DailyCloseDate,IntoStoreCount)";
                sql += " select '"+strCloseDate+"',count(0)";
                sql += " from dbo.tbl_Card_StockInfo"; 
                sql += " where IntoStore_Date='"+strCloseDate+"'";

                sql += " update dbo.tbl_Card_DailyClose";
                sql += " set OutStoreFCount=(select count(0) from dbo.tbl_Card_StockInfo where OutStore_Date='"+strCloseDate+"' and substring(rtrim(ltrim(OutStore_Status)),1,2)='��d')";
                sql += " where DailyCloseDate='"+strCloseDate+"'";

                sql += " update dbo.tbl_Card_DailyClose";
                sql += " set OutStoreMCount=(select count(0) from dbo.tbl_Card_StockInfo where OutStore_Date='"+strCloseDate+"' and substring(rtrim(ltrim(OutStore_Status)),1,2)='�l�H')";
                sql += " where DailyCloseDate='"+strCloseDate+"'";

                sql += " update dbo.tbl_Card_DailyClose";
                sql += " set OutStoreDCount=(select count(0) from dbo.tbl_Card_StockInfo where OutStore_Date='" + strCloseDate + "' and substring(rtrim(ltrim(OutStore_Status)),1,2)='���P')";
                sql += " where DailyCloseDate='"+strCloseDate+"'";

                sql += " declare @LastCount int "; 
                sql += " select @LastCount=C.DailyCloseCount from (select top 1 DailyCloseCount from dbo.tbl_Card_DailyClose where DailyCloseDate<'"+strCloseDate+"' order by DailyCloseDate desc) as C";
                sql += " if @LastCount is null begin set @LastCount=0 end";
                sql += " update dbo.tbl_Card_DailyClose set DailyCloseCount=@LastCount+IntoStoreCount-OutStoreFCount-OutStoreMCount-OutStoreDCount";
                sql += " from tbl_Card_DailyClose";
                sql += " where DailyCloseDate='"+strCloseDate+"'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CardDailyClose.Add(sqlcmd))
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
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// �\�໡��:�鵲�H����-�R���鵲�H��
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
       public static bool DeleteDailyClose(string strCloseDate)
        {
            try
            {
                string sql = @"delete from tbl_Card_DailyClose ";
                sql += " where DailyCloseDate='" + strCloseDate + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CardDailyClose.Delete(sqlcmd))
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
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// �\�໡��:�鵲�w�s�H����-���J�鵲�w�s�H��
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
       public static bool InsertDailyStockInfo(string strCloseDate)
        {
            try
            {
                string sql = @"insert into dbo.tbl_Card_DailyStockInfo(DailyClose_Date,cardno,IntoStore_Date,OutStore_Status,OutStore_Date,custname)";
                sql += " select '" + strCloseDate + "',cardno,IntoStore_Date,OutStore_Status,OutStore_Date,custname ";
                sql += " from dbo.tbl_Card_StockInfo";
                sql += " where IntoStore_Date='" + strCloseDate + "' or OutStore_Date='"+ strCloseDate + "'";
                sql += " or (IntoStore_Date<'" + strCloseDate + "' and isnull(OutStore_Date,'')='')";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CardDailyClose.Add(sqlcmd))
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
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }
        /// <summary>
        /// �\�໡��:�鵲�w�s�H����-�R���鵲�w�s�H��
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/17
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool DeleteDailyStockInfo(string strCloseDate)
        {
            try
            {
                string sql = @"delete from tbl_Card_DailyStockInfo ";
                sql += " where DailyClose_Date='" + strCloseDate + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CardDailyClose.Delete(sqlcmd))
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
                BRM_CardDailyClose.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
