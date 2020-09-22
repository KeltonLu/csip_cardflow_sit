using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.ApplicationClass;

/// <summary>
/// BRBatchImport_UrgencyCard 的摘要描述
/// </summary>
public class BRBatchImport_UrgencyCard : CSIPCommonModel.BusinessRules.BRBase<BatchImport_UrgencyCard>
{
    public BRBatchImport_UrgencyCard()
    {

    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(BatchImport_UrgencyCard paramObj)
    {
        bool result = false;
        try
        {

            string sSql = @" Insert into tbl_BatchImport_UrgencyCard 
(indate1,id,cardno,kind,card_file,result,fail_reason,reason,import_time,import_user)
VALUES(@indate1,@id,@cardno,@kind,@card_file,@result,@fail_reason,@reason,@import_time,@import_user); ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;

            sqlcmd.CommandText = sSql;
            sqlcmd.Parameters.Add(new SqlParameter("@indate1", paramObj.indate1));
            sqlcmd.Parameters.Add(new SqlParameter("@id", paramObj.id));
            sqlcmd.Parameters.Add(new SqlParameter("@cardno", paramObj.cardno));
            sqlcmd.Parameters.Add(new SqlParameter("@kind", paramObj.kind));
            sqlcmd.Parameters.Add(new SqlParameter("@card_file", paramObj.card_file));
            sqlcmd.Parameters.Add(new SqlParameter("@result", paramObj.result));
            sqlcmd.Parameters.Add(new SqlParameter("@fail_reason", paramObj.fail_reason));
            sqlcmd.Parameters.Add(new SqlParameter("@reason", paramObj.reason));
            sqlcmd.Parameters.Add(new SqlParameter("@import_time", paramObj.import_time));
            sqlcmd.Parameters.Add(new SqlParameter("@import_user", paramObj.import_user));

            result = Add(sqlcmd);
            result = true;
        }
        catch (Exception ex)
        {
            string ms = ex.Message;
        }
        finally
        {

        }

        return result;
    }

    public static bool IsExistCard_DataChange(BatchImport_UrgencyCard paramObj)
    {
        bool result = false;
        try
        {
            string sql = @"select  *  from  dbo.tbl_Card_DataChange ";
            sql += " where indate1=@indate1 and id=@id and NewWay is not null ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@indate1", paramObj.indate1));
            sqlcmd.Parameters.Add(new SqlParameter("@id", paramObj.id));
            DataSet ds = BRBatchImport_UrgencyCard.SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                System.Data.DataTable dtOASACardInfo = ds.Tables[0];
                if (dtOASACardInfo.Rows.Count > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

            }
            else
            {
                result = false;
            }
        }
        catch (Exception exp)
        {
            BRBatchImport_UrgencyCard.SaveLog(exp.Message);
            result = false;
        }


        return result;
    }
    /// <summary>
    /// 讀取報表資料
    /// </summary>
    /// <returns></returns>
    public static List<BatchImport_UrgencyCard> getReportColl(BatchImport_UrgencyCard paramObj)
    {
        System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-TW");
        string sSQL = @"  select [indate1],[id],[cardno],[kind],[card_file],[result],[fail_reason],[reason], import_time ,[import_user] from  [dbo].[tbl_BatchImport_UrgencyCard] where 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        if (paramObj != null)
        {
            if (!String.IsNullOrEmpty(paramObj.indate1))
            {
                sSQL += " AND indate1 >= @indate1 AND indate1 <= @indate2 ";
                sqlcmd.Parameters.Add(new SqlParameter("@indate1", paramObj.indate1));
                sqlcmd.Parameters.Add(new SqlParameter("@indate2", paramObj.indate2));
            }
            if (!String.IsNullOrEmpty(paramObj.cardno))
            {
                sSQL += " AND cardno = @cardno ";
                sqlcmd.Parameters.Add(new SqlParameter("@cardno", paramObj.cardno));
            }
            if (!String.IsNullOrEmpty(paramObj.id))
            {
                sSQL += " AND id = @id ";
                sqlcmd.Parameters.Add(new SqlParameter("@id", paramObj.id));
            }
            if (!String.IsNullOrEmpty(paramObj.result))
            {
                sSQL += " AND result = @result ";
                sqlcmd.Parameters.Add(new SqlParameter("@result", paramObj.result));
            }
        }
        sqlcmd.CommandText = sSQL;
        List<BatchImport_UrgencyCard> rtnObj = new List<BatchImport_UrgencyCard>();
        System.Data.DataTable dt = new System.Data.DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            rtnObj = DataTableConvertor.ConvertCollToObj<BatchImport_UrgencyCard>(dt);
        }
        return rtnObj;
    }

    public static bool CreateExcelFile(List<BatchImport_UrgencyCard> dsData, string strAgentName, ref string strPathFile, ref string strMsgID)
    {
        System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        //  ExcelApplication excel = new ExcelApplication();
        ExcelApplication excel = new ExcelApplication();
        Workbook workbook = null;
        string excelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "EmcCardLogReport.xls";
        try
        {
            //* 檢查目錄，并刪除以前的文檔資料
            // BRExcel_File.CheckDirectory(ref strPathFile);

            //* 取要下載的資料
            string strInputDate = "";

            excel.Visible = false;//* 显示 Excel 文件,如果为 true 则显示 Excel 文件
            excel.Application.DisplayAlerts = false;
            // string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "AutoPayStatus.xls";
            workbook = excel.Workbooks.Open(excelPathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            WriteDataToSheet_Fault(ref workbook, strAgentName, dsData, strInputDate, strMsgID);

            //* 保存文件到程序运行目录下
            strPathFile = strPathFile + @"\EMCCardReport_" + DateTime.Now.ToString("yyyy") + ".xls";
            //舊檔案存在則先刪除
            if (File.Exists(strPathFile))
            {
                File.Delete(strPathFile);
            }
                  
            ((Worksheet)workbook.Sheets[1]).SaveAs(strPathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            workbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
            return true;

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            //throw ex;
            return false;
        }
        finally
        {

            excel.Quit(); //* 退出 Excel
            excel = null; //* 将 Excel 实例设置为空
        }
    }
    /// <summary>
    /// 寫入指定sheet指定欄位值
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="colName"></param>
    /// <param name="sValue"></param>
    private static void writeCells(Worksheet sheet, string colName, string sValue)
    {
        Range range = null;
        range = sheet.get_Range(colName, Type.Missing);
        range.Value2 = sValue;

    }
    private static void WriteDataToSheet_Fault(ref Workbook workbook, string strAgentName, List<BatchImport_UrgencyCard> dtblWriteData, string strInputDate, string strMsgID)
    {
        Worksheet sheet = null; //主要產出頁面
     
        sheet = (Worksheet)workbook.Sheets[1];  

        Range signRowALL = sheet.get_Range("A1:H500", Type.Missing);
        signRowALL.NumberFormatLocal = "@";

        //Type v = dtblWriteData.GetType();  //取的型別實體
        //PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        //                                          //  string LineID = ""; //若有分行，則用來帶出LINEID

        //foreach (PropertyInfo prop in props)
        //{
        //    object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
        //    AttributeRFPrint authAttr;
        //    for (int xi = 0; xi < attrs.Length; xi++)
        //    {
        //        if (attrs[xi] is AttributeRFPrint)
        //        {
        //            authAttr = attrs[xi] as AttributeRFPrint;

        //            string cellRange = authAttr.CellRange;
        //            string exVal = prop.GetValue(dtblWriteData, null) as string;
        //            //有設定欄位,將欄位值寫入
        //            if (!string.IsNullOrEmpty(cellRange))
        //            {
        //                writeCells(sheet, cellRange, exVal);
        //            }
        //        }
        //    }
        //}
        //先寫入經辦及列印日期
        writeCells(sheet, "B4", strAgentName);
        writeCells(sheet, "B5" , DateTime.Now.ToString("yyyy/MM/dd"));

        //分割總筆數，成功數，失敗數
        string[] COuntColl = strMsgID.Split(':');
        writeCells(sheet, "I4", COuntColl[0]);
        writeCells(sheet, "I5", COuntColl[1]);
        writeCells(sheet, "I6", COuntColl[2]);

        int rRoll = 8;
        foreach (BatchImport_UrgencyCard oitem in dtblWriteData)
        {
            writeCells(sheet, "A" + rRoll.ToString() , oitem.indate1);
            writeCells(sheet, "B" + rRoll.ToString(), oitem.id);
            writeCells(sheet, "C" + rRoll.ToString(), oitem.cardno);
            writeCells(sheet, "D" + rRoll.ToString(), oitem.kind);
            writeCells(sheet, "E" + rRoll.ToString(), oitem.card_file);
            writeCells(sheet, "F" + rRoll.ToString(), oitem.result);
            writeCells(sheet, "G" + rRoll.ToString(), oitem.fail_reason);
            writeCells(sheet, "H" + rRoll.ToString(), oitem.reason);
            writeCells(sheet, "I" + rRoll.ToString(), oitem.import_time);
            writeCells(sheet, "J" + rRoll.ToString(), oitem.import_user);  
            rRoll++;
        }

    }
}

            