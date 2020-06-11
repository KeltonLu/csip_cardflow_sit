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
using BusinessRules;

namespace BusinessRulesNew
{
    public class BRM_CardStockInfo : BRBase<Entity_CardStockInfo>
    {
        /// <summary>
        /// �\�໡��:�d�ߦۨ��d�����(�ۨ��O���ﭭ��)
        /// �@    ��:
        /// �Ыخɶ�:2016/01/28
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool GetSelfPickInfoPost(ref  DataTable dtSelfPickInfoPost, int iPageIndex, int iPageSize, ref int iTotalCount, string strId, string strCardNo)
        {            
            try
            {
                string sql = @"SELECT * FROM(";

                sql += " select custname,id,cardno,indate1,IntoStore_Date,action,trandate,isnull(OutStore_Date,'') as OutStore_Date from dbo.tbl_Card_BaseInfo base where (kind='1' or isnull(IntoStore_Date,'')<>'')";
                sql += " and base.cardtype<>'900'"; //���إd�����S��B�z Bug234
                sql += " and base.selfpick_type = '4'";  //�ۨ��O���ﭭ��
                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id='" + strId + "'";
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno='" + strCardNo + "'";
                }
                //��L���d�覡+�h���ۨ�
                sql += " union";
                sql += " select base.custname,base.id,base.cardno,base.indate1,base.IntoStore_Date,base.action,base.trandate,isnull(base.OutStore_Date,'') as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//���إd�����S��B�z Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date>=back.ImportDate";
                sql += " and base.selfpick_type = '4'";  //�ۨ��O���ﭭ��
                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id='" + strId + "'";
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno='" + strCardNo + "'";
                }
                //��L���d�覡+�h���ۨ� �B�� �ۨ�->�l�H�X�w->�h��->�ۨ� �����p
                sql += " union";
                sql += " select base.custname,base.id,base.cardno,base.indate1,'' as IntoStore_Date,base.action,base.trandate,'' as OutStore_Date ";
                sql += " from dbo.tbl_Card_BackInfo back,dbo.tbl_Card_BaseInfo base";
                sql += " where back.action=base.action and back.id=base.id and back.cardno=base.cardno and back.trandate=base.trandate";
                sql += " and base.cardtype<>'900'";//���إd�����S��B�z Bug234
                sql += " and isnull(base.kind,'')<>'1' and back.Enditem='0'";
                sql += " and IntoStore_Date<back.ImportDate";
                sql += " and base.selfpick_type = '4'";  //�ۨ��O���ﭭ��
                if (!strId.Equals(string.Empty))
                {
                    sql += " and base.id='" + strId + "'";
                }
                if (!strCardNo.Equals(string.Empty))
                {
                    sql += " and base.cardno='" + strCardNo + "'";
                }

                sql += ")U ";
                sql += "ORDER BY U.IntoStore_Date DESC,U.id";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CardStockInfo.SearchOnDataSet(sqlcmd, iPageIndex, iPageSize, ref iTotalCount);
                if (ds != null)
                {
                    dtSelfPickInfoPost = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }
        
        /// <summary>
        /// �\�໡��:�X�w�ާ@-��s�d���򥻫H���B�R���w�s�H��(�ۨ��O���ﭭ��)
        /// �@    ��:
        /// �Ыخɶ�:2016/01/28
        /// �ק�O��:
        /// </summary>
        /// <param name="dtLastCloseDate"></param>
        /// <returns></returns>
        public static bool OutStore(string strAction, string strId, string strCardNo, string strTrandate, string strCustname,string strIntoStoreDate)
        {
            try
            {
                string sql = @"Update dbo.tbl_Card_BaseInfo";
                sql += " Set IntoStore_Status='0',IntoStore_Date='',OutStore_Status='0',OutStore_Date='',SelfPick_Type='',SelfPick_date = ''";
                sql += " Where action='" + strAction + "' and id='" + strId + "' and cardno='" + strCardNo + "' and trandate='" + strTrandate + "'";

                sql += " Delete from dbo.tbl_Card_StockInfo";
                sql += " where IntoStore_Date='" + strIntoStoreDate + "' and cardno='" + strCardNo + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                if (BRM_CardStockInfo.Delete(sqlcmd))
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
                BRM_CardStockInfo.SaveLog(exp.Message);
                return false;
            }
        }


    }
}
