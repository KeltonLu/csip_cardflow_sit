//******************************************************************
//*  功能說明：手動大宗檔錯誤處理業務邏輯層
//*  作    者：zhen chen
//*  創建日期：2010/07/05
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

namespace BusinessRules
{
  public class BRM_InOutFile : BRBase<Entity_JobErrorInfo>
    {
      /// <summary>
      /// 功能說明:查詢數據
      /// 作    者:zhen chen
      /// 創建時間:2010/07/1
      /// 修改記錄: 
      /// </summary>
      /// <param name="dtFileInfo"></param>
      /// <param name="strDateFrom">開始時間</param>
      /// <param name="strDateTo">結束時間</param>
      /// <returns></returns>
      public static bool SearchInOutFile(ref DataTable dtFileInfo, ref string strDateFrom, ref string strDateTo, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                string sql = @"Select ErrorID,JobID,ImportTime,ErrorContext,ImportFileName,LoadFlag,LocalFilePath
                               From tbl_JobErrorInfo
                               Where convert(datetime,ImportTime,120) 
                               between convert(datetime,@strDateFrom,120)
                               and convert(datetime,@strDateTo,120)
                               and LoadFlag is not null
                               Order by convert(datetime,ImportTime,120) desc";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@strDateFrom", strDateFrom));
                sqlcmd.Parameters.Add(new SqlParameter("@strDateTo", strDateTo));
                DataSet ds = BRM_InOutFile.SearchOnDataSet(sqlcmd,iPageIndex,iPageSize,ref iTotalCount);
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
            catch (SqlException sqlexp)
            {
                BRM_InOutFile.SaveLog(sqlexp.Message);
                return false;
            }
        }

      /// <summary>
      /// 功能說明:更新下載標記位
      /// 作    者:zhen chen
      /// 創建時間:2010/07/1
      /// 修改記錄: 
      /// </summary>
      /// <param name="strDate"></param>
      /// <param name="strFileName"></param>
      /// <returns></returns>
      public static bool UpdateLoadFlag(ref string ErrorID)
      {
          bool tag = false;
          try
          {
              string sql = @"update tbl_JobErrorInfo
                             set LoadFlag='1'
                             where ErrorID=@ErrodID";
              SqlCommand sqlcmd = new SqlCommand();
              sqlcmd.CommandType = CommandType.Text;
              sqlcmd.CommandText = sql;
              sqlcmd.Parameters.Add(new SqlParameter("@ErrodID", ErrorID));
              tag=BRM_InOutFile.Update(sqlcmd);
              if (tag)
              {
                  return tag;
              }
              else
              {
                  return tag;
              }
          }
          catch (SqlException sqlexp)
          {
              BRM_InOutFile.SaveLog(sqlexp.Message);
              return false;
          }
      }

      /// <summary>
      /// 功能說明:刪除已下載文件的信息
      /// 作    者:zhen chen
      /// 創建時間:2010/07/1
      /// 修改記錄:
      /// </summary>
      /// <param name="strDate"></param>
      /// <param name="strFileName"></param>
      /// <returns></returns>
      public static bool DeleteDownLoadFile(ref string ErrorID)
      {
          bool tag = false;
          try
          {
              string sql = @"delete from tbl_JobErrorInfo
                             where ErrorID=@ErrorID";
              SqlCommand sqlcmd = new SqlCommand();
              sqlcmd.CommandType = CommandType.Text;
              sqlcmd.CommandText = sql;
              sqlcmd.Parameters.Add(new SqlParameter("@ErrorID", ErrorID));
              tag = BRM_InOutFile.Delete(sqlcmd);
              if (tag)
              {
                  return tag;
              }
              else
              {
                  return tag;
              }
          }
          catch (SqlException sqlexp)
          {
              BRM_InOutFile.SaveLog(sqlexp.Message);
              return false;
          }

      }
   }
}
