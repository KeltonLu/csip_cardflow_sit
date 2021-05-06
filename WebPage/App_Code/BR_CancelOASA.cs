//******************************************************************
//*  功能說明：OASA注銷Log檔處理業務邏輯層       為了空檔寫入0及無檔案寫入NA複寫此方法
//*  作    者：zhiyuan
//*  創建日期：2010/05/27
//*  修改記錄：2021/05/06 執行後更新狀態 陳永銘
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;
using BusinessRules;

/// <summary>
/// BR_CancelOASA 的摘要描述
/// </summary>
public class BR_CancelOASA : BRBase<Entity_CancelOASA>
{
    public BR_CancelOASA()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 進入點 進入後判斷是否有資料存在，再決定是新增還是UPDDATE
    /// </summary>
    /// <param name="inData"></param>
    /// <returns></returns>
    public static bool InsertCancelOASA(Entity_CancelOASA inData)
    {
        bool result = false;
        int totalCount = 0;
        if (!IsExist(inData, ref totalCount))
        {
            result = Insert(inData);
        }
        else
        {
            //有筆數才更新，不然維持原狀
            //2021/05/06 執行後更新狀態 陳永銘
            if (inData.TotalCount > 0)
            {
                result = Update(inData);
            }
            else if (totalCount < 0)
            {
                //result = true;
                result = Update(inData);
            }
        }
        return result;
    }

    private static bool IsExist(Entity_CancelOASA inData, ref int totalCount)
    {
        bool result = false;
        string strSQL = @"SELECT TOP 1 [CancelOASAFile],[CancelOASADate],[CancelOASAUser],[TotalCount]
	                    ,[SCount],[FCount],[CancelOASASource],[Stauts] 
	                    FROM [tbl_CancelOASA]";
        try
        {
            strSQL += " where CancelOASAFile=@CancelOASAFile and  CancelOASADate=@CancelOASADate ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CancelOASAFile", inData.CancelOASAFile));
            sqlcmd.Parameters.Add(new SqlParameter("@CancelOASADate", inData.CancelOASADate));
            DataSet ds = SearchOnDataSet(sqlcmd);
            if (ds != null)
            {
                System.Data.DataTable dtOASACardInfo = ds.Tables[0];
                if (dtOASACardInfo.Rows.Count > 0)
                {
                    result = true;
                    totalCount = Convert.ToInt32(dtOASACardInfo.Rows[0]["TotalCount"]);
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
            SaveLog(exp.Message);
            result = false;
        }


        return result;
    }

    private static bool Insert(Entity_CancelOASA paramObj)
    {

        bool result = false;
        string strSQL = @"Insert into tbl_CancelOASA 
(CancelOASAFile,CancelOASADate,CancelOASAUser,TotalCount,SCount,FCount,CancelOASASource)
VALUES(@CancelOASAFile,@CancelOASADate,@CancelOASAUser,@TotalCount,@SCount,@FCount,@CancelOASASource);

";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        sqlcmd.CommandText = strSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASAFile", paramObj.CancelOASAFile));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASADate", paramObj.CancelOASADate));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASAUser", paramObj.CancelOASAUser));
        sqlcmd.Parameters.Add(new SqlParameter("@TotalCount", paramObj.TotalCount));
        sqlcmd.Parameters.Add(new SqlParameter("@SCount", paramObj.SCount));
        sqlcmd.Parameters.Add(new SqlParameter("@FCount", paramObj.FCount));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASASource", paramObj.CancelOASASource));
        result = Add(sqlcmd);
        return result;
    }

    private static bool Update(Entity_CancelOASA paramObj)
    {
        bool result = false;
        string strSQL = @" update tbl_CancelOASA
set TotalCount = @TotalCount,SCount = @SCount,FCount = @FCount,CancelOASASource = @CancelOASASource 
 
";
        strSQL += " where CancelOASAFile=@CancelOASAFile and  CancelOASADate=@CancelOASADate ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = strSQL;

        sqlcmd.Parameters.Add(new SqlParameter("@TotalCount", paramObj.TotalCount));
        sqlcmd.Parameters.Add(new SqlParameter("@SCount", paramObj.SCount));
        sqlcmd.Parameters.Add(new SqlParameter("@FCount", paramObj.FCount));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASASource", paramObj.CancelOASASource));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASAFile", paramObj.CancelOASAFile));
        sqlcmd.Parameters.Add(new SqlParameter("@CancelOASADate", paramObj.CancelOASADate));
        result = Update(sqlcmd);
        return result;
    }

}