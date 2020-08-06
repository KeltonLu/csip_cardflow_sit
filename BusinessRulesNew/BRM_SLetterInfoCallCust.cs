//******************************************************************
//*  �\�໡���G²�T�B�z�q���D�����޿�h
//*  �@    �̡Gzhiyuan
//*  �Ыؤ���G2010/06/07
//*  �ק�O���G
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
using BusinessRules;

namespace BusinessRulesNew
{
    public class BRM_SLetterInfoCallCust : CSIPCommonModel.BusinessRules.BRBase<Entity_SLetterInfoCallCust>
    {
        #region SQL�y�y
       // public const string UPD_LETTERINFOCALLCUST_0115_2 = @"Update tbl_sletterInfo_CallCust Set Name=@Name,Htel=@Htel,Otel=@Otel,Dob=@Dob,Zip=@Zip,Add1=@Add1,Add2=@Add2,Add3=@Add3,Mobil=@Mobil,Block_code=@Block_code,Ams=@Ams,Imp_File=@Imp_File,Imp_Date=@Imp_Date Where ID=@ID And CardNo=@CardNo";

        public const string UPD_LETTERINFOCALLCUST_0115_2 = @"Update tbl_sletterInfo_CallCust Set Name=@Name,Htel=@Htel,Otel=@Otel,Dob=@Dob,Zip=@Zip,Add1=@Add1,Add2=@Add2,Add3=@Add3,Mobil=@Mobil,Block_code=@Block_code,Ams=@Ams,Imp_File=@Imp_File,Imp_Date=@Imp_Date Where ID=@ID And CardNo=@CardNo And (Imp_Date is null or Imp_Date = '') ";

        public const string SEL_LETTERINFOCALLCUST_0115_2 = @"Select top 1 Source_Flg,Serial_no From tbl_sletterInfo_CallCust Where ID=@ID And CardNo=@CardNo  Order By Exp_Date Desc";
        public const string SEL_LETTERINFOCALLCUST_0115_1 = @"Select ID,CardNo,Exp_Date From tbl_sletterInfo_CallCust Where ID=@ID And CardNo=@CardNo And Exp_Date=@Exp_Date";
        #endregion
        
        /// <summary>
        /// �\�໡��:�۰ʤ�²�T�B�z-�D���^���ɶפJ�A�ưȱM��
        /// �@    ��:zhiyuan
        /// �Ыخɶ�:2010/06/07
        /// �ק�O��:
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
                sqlConn = new SqlConnection(UtilHelper.GetConnectionStrings("Connection_System"));
                
                sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandType = CommandType.Text;

                #region �]�m�ѼƦW��
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
                    parmName.Value = dtDetail.Rows[i]["Name"].ToString();  //�m�W
                    parmHtel.Value = dtDetail.Rows[i]["Htel"].ToString().Replace("-", "");  //��a�q��
                    parmOtel.Value = dtDetail.Rows[i]["Otel"].ToString().Replace("-", "");  //���q�q��
                    parmDob.Value = DateHelper.InsertTimeSpan(dtDetail.Rows[i]["Dop"].ToString()); //�ͤ�
                    parmZip.Value = dtDetail.Rows[i]["Zip"].ToString();    //�l���ϸ�
                    parmAdd1.Value = dtDetail.Rows[i]["City"].ToString();  //�����a��
                    parmAdd2.Value = dtDetail.Rows[i]["Add1"].ToString();  //�a�}2
                    parmAdd3.Value = dtDetail.Rows[i]["Add2"].ToString();  //�a�}3
                    parmMobil.Value = dtDetail.Rows[i]["Mobil"].ToString().Replace("-", "");   //��ʹq��
                    parmBlock_code.Value = dtDetail.Rows[i]["Block_code"].ToString();  //BlockCode
                    parmAms.Value = dtDetail.Rows[i]["Ams"].ToString();    //Ams
                    parmImp_File.Value = strImpFileName;   //�D���^���ɶפJ�ɦW
                    // //�h�[�P�_�p�G�O����n��U�@�Ӥu�@��
                    // DateTime dt = new DateTime();
                    // if (BRWORK_DATE.IS_WORKDAY("06", _jobDate.ToString("yyyyMMdd")))
                    // {
                    //     dt = _jobDate;
                    // }
                    // else
                    // {
                    //     dt = DateTime.ParseExact(CSIPCommonModel.BusinessRules.BRWORK_DATE.ADD_WORKDAY("06", _jobDate.ToString("yyyyMMdd"), 1), "yyyyMMdd", null);
                    // }
                    parmImp_Date.Value = dtDetail.Rows[i]["jobDate"];  //�D���^���ɶפJ���
                   // parmImp_Date.Value = DateTime.Now.ToString("yyyy/MM/dd");  //�D���^���ɶפJ���
                    parmID.Value = dtDetail.Rows[i]["ID"].ToString();  //ID
                    parmCardNo.Value = dtDetail.Rows[i]["CardNo"].ToString();  //�d��

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
                    Framework.Common.Logging.Logging.AddSqlLog(sqlCmd);
                    sqlCmd.ExecuteNonQuery();

                    strSrcFlg = string.Empty;
                    sqlCmd.CommandText = SEL_LETTERINFOCALLCUST_0115_2;
                    sdaValue = new SqlDataAdapter(sqlCmd);
                    sdaValue.Fill(dtTmp);
                    if (dtTmp.Rows.Count > 0)
                    {
                        if (dtTmp.Rows[0]["Source_Flg"].ToString().Equals("1"))
                        {
                            //*�ھڥD���^���ɤ�����Ƨ�s8.4�d���h���ƪ�
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

    }
}
