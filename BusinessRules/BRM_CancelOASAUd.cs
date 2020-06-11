using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;

namespace BusinessRules
{
    public class BRM_CancelOASAUd : BRBase<Entity_CancelOASAUd>
    {
        /// <summary>
        /// �\�໡��:�s�W�h���A�ưȱM��
        /// �@    ��:Linda
        /// �Ыخɶ�:2010/07/13
        /// �ק�O��:
        /// </summary>
        /// <param name="EntitySetCancelOASAUd"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<Entity_CancelOASAUd> Set, ref string strMsgID)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (BRM_CancelOASAUd.BatInsert(Set))
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
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// �\�໡��:�P�_�ި�Ѻ�/�ʱ��ɱ��`�P�d����ƬO�_�w�W�D��
        /// �@    ��:Carolyn
        /// success_flag = 0 : ��ܥ��W�D��
        /// �^�� : true : �ӧ��ɮפw�W�D��(�ӧ���,�u�n���@�����W�e�L�D��,�h�������Ҥw�W�e)
        ///        false : �ӧ��ɮץ��W�D��
        /// �ק�O��:
        public static bool IS_OASAUDCardInfo_UpToMF(string strFileName, string strFileDate)
        {
            try
            {
                string strCount = "";
                string sql = @"select count(*) from dbo.tbl_CancelOASA_UD";
                sql += " where FileName_Real='"+strFileName+"' and File_Date='"+strFileDate+"'";
                sql += " and success_flag <> '0'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASAUd.SearchOnDataSet(sqlcmd);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    strCount = ds.Tables[0].Rows[0][0].ToString();
                    if (int.Parse(strCount) > 0) //���W�e�L�D��
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
                    return true;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return true;
            }

        }

        /// <summary>
        /// �\�໡��:����ި�Ѻ�/�ʱ��ɱ��`�P�d�����
        /// �@    ��:linda
        /// �Ыخɶ�:2010/07/14
        /// �ק�O��:
        /// </summary>
        /// <param name="dtFileInfo"></param>
        /// <param name="strJobId"></param>
        /// <returns></returns>
        public static bool SearchOASAUDCardInfo(string strFileName, string strFileDate, ref DataTable dtOASAUDCardInfo)
        {
            try
            {
                string sql = @"select * from dbo.tbl_CancelOASA_UD";
                sql += " where FileName_Real='" + strFileName + "' and File_Date='" + strFileDate + "'";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_CancelOASAUd.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    dtOASAUDCardInfo = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }

        }

        /// <summary>
        /// �\�໡��:��s�@��OASAUD���
        /// �@    ��:linda
        /// �Ыخɶ�:2010/06/08
        /// �ק�O��:
        /// </summary>
        /// <param name="TCardBaseInfo"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool Update(Entity_CancelOASAUd CardBackInfo, string strCondition, params  string[] FiledSpit)
        {
            try
            {

                if (BRM_CancelOASAUd.UpdateEntityByCondition(CardBackInfo, strCondition, FiledSpit))
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
                BRM_CancelOASAUd.SaveLog(exp.Message);
                return false;
            }
        }
    }
}
