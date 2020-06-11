//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：2010/04/09
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using Framework.Data.OM.OMAttribute;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using Framework.Common.Logging;
using System.Diagnostics;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System.Data;
using System.Data.Common;
using Framework.Data;
using CSIPCommonModel.BaseItem;
using Framework.Common.Message;

namespace BusinessRules
{
    /// <summary>
    /// BR Base
    /// </summary>
    public abstract class BRBase<T> where T : Framework.Data.OM.Entity
    {
        /// <summary>
        /// Save BR Layer Log
        /// </summary>
        /// <param name="Message">Message</param>
        public static void SaveLog(string Message)
        {
            string strErrMsg = Message + ",\r\n";
            strErrMsg = strErrMsg + "Type:" + typeof(T).ToString() + "";
            Logging.Log(strErrMsg, LogState.Error, LogLayer.BusinessRule);
        }

        /// <summary>
        /// Save BR Layer Exception Log
        /// </summary>
        /// <param name="exp">exp</param>
        public static void SaveLog(Exception exp)
        {
            string strErrMsg = exp.Message + ",\r\n";
            strErrMsg = strErrMsg + "Type:" + typeof(T).ToString() + ", \r\n";
            strErrMsg = strErrMsg + "Source:" + exp.Source.ToString() + ",\r\n";
            strErrMsg = strErrMsg + "StackTrace:" + exp.StackTrace;
            Logging.Log(strErrMsg, LogState.Error, LogLayer.BusinessRule);
        }


        /// <summary>
        /// Add by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <returns>Add ok</returns>
        public static bool Add(DbCommand command)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteNonQuery(command)>0;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }


        /// <summary>
        /// Add by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Add ok</returns>
        public static bool Add(DbCommand command, string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteNonQuery(command) > 0;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>whether Add ok</returns>
        public static bool AddNewEntity(T Entity, string strConnctionName)
        {
            try
            {
                return Entity.DB_InsertEntityByConn(strConnctionName);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <returns>whether Add ok</returns>
        public static bool AddNewEntity(T Entity)
        {
            try
            {
                return Entity.DB_InsertEntity();
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns></returns>
        public static bool UpdateEntityByConn(T Entity, string strConnctionName)
        {
            try
            {
                Entity.DB_UpdateEntityByConn(strConnctionName);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <returns></returns>
        public static bool UpdateEntity(T Entity)
        {
            try
            {
                Entity.DB_UpdateEntity();
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Update ok</returns>
        public static bool Update(DbCommand command, string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                dh.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <returns>Update ok</returns>
        public static bool Update(DbCommand command)
        {
            DataHelper dh = new DataHelper();
            try
            {
                dh.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="Condition">Condition</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Update ok</returns>
        public static bool UpdateEntity(T Entity, string Condition, string strConnctionName)
        {
            try
            {
                Entity.DB_UpdateEntityByConn(Condition, strConnctionName);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="Condition">Condition</param>
        /// <returns>Update ok</returns>
        public static bool UpdateEntity(T Entity, string Condition)
        {
            try
            {
                Entity.DB_UpdateEntity(Condition);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Condition">Condition</param>
        /// <param name="Entity">Entity</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <param name="FieldSplit">Update Field</param>
        /// <returns>Update ok</returns>
        public static bool UpdateEntityByCondition(string Condition, T Entity, string strConnctionName, params string[] FieldSplit)
        {
            try
            {
                Entity.DB_UpdateEntityByConditionAndConn(Condition, strConnctionName,FieldSplit);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="Condition">Condition</param>
        /// <param name="FieldSplit">Update Field</param>
        /// <returns>Update ok</returns>
        public static bool UpdateEntityByCondition(T Entity, string Condition, params string[] FieldSplit)
        {
            try
            {
                Entity.DB_UpdateEntityByCondition(Condition, FieldSplit);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <param name="FieldSplit">Update Field</param>
        /// <returns>Update ok</returns>
        public static bool CustomFieldUpdateEntity(string strConnctionName, T Entity, params string[] FieldSplit)
        {
            try
            {
                Entity.DB_CustomeUpdateEntityByConn(strConnctionName, FieldSplit);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="FieldSplit">Update Field</param>
        /// <returns>Update ok</returns>
        public static bool CustomFieldUpdateEntity(T Entity, params string[] FieldSplit)
        {
            try
            {
                Entity.DB_CustomeUpdateEntity(FieldSplit);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Delete by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <returns>Delete ok</returns>
        public static bool Delete(DbCommand command)
        {
            DataHelper dh = new DataHelper();
            try
            {
                dh.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Delete by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Delete ok</returns>
        public static bool Delete(DbCommand command,string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                dh.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }


        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns></returns>
        public static bool DeleteEntity(T Entity, string strConnctionName)
        {
            try
            {
                Entity.DB_DeleteEntityByConn(strConnctionName);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <returns></returns>
        public static bool DeleteEntity(T Entity)
        {
            try
            {
                Entity.DB_DeleteEntity();
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="Condition">Condition</param>
        /// <returns></returns>
        public static bool DeleteEntityByCondition(T Entity, string Condition)
        {
            try
            {
                Entity.DB_DeleteEntity(Condition);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="Condition">Condition</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns></returns>
        public static bool DeleteEntityByCondition(T Entity, string Condition,string strConnctionName)
        {
            try
            {
                Entity.DB_DeleteEntityByConn(Condition,strConnctionName);
                return true;
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> Search(string strCondition)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition);

            return result;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> Search(string strCondition,string strConnctionName)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, strConnctionName);

            return result;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> Search(string strCondition, int PageIndex, int PageSize, string strConnctionName)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, PageIndex, PageSize, strConnctionName);

            return result;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> Search(string strCondition, int PageIndex, int PageSize)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, PageIndex, PageSize);

            return result;
        }

        /// <summary>
        /// sql Search
        /// </summary>
        /// <param name="strCondition">sql</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> SearchCommand(string strCondition, string strConnctionName)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, ECommandType.TableView, strConnctionName);

            return result;
        }


        /// <summary>
        /// sql Search
        /// </summary>
        /// <param name="strCondition">sql</param>
        /// <returns>Entity</returns>
        public static EntitySet<T> SearchCommand(string strCondition)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, ECommandType.TableView);

            return result;
        }

        /// <summary>
        /// sql Search
        /// </summary>
        /// <param name="strCondition">sql</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns></returns>
        public static EntitySet<T> SearchCommand(string strCondition, int PageIndex, int PageSize, string strConnctionName)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, ECommandType.TableView, PageIndex, PageSize, strConnctionName);

            return result;
        }

        /// <summary>
        /// sql Search
        /// </summary>
        /// <param name="strCondition">sql</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <returns></returns>
        public static EntitySet<T> SearchCommand(string strCondition, int PageIndex, int PageSize)
        {
            EntitySet<T> result = new EntitySet<T>(strCondition, ECommandType.TableView, PageIndex, PageSize);

            return result;
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(string sqlcmd, string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteDataSet(sqlcmd);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(string sqlcmd)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteDataSet(sqlcmd);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(DbCommand command, int PageIndex, int PageSize, ref int TotalCount, string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteDataSet(command, PageIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(DbCommand command, int PageIndex, int PageSize, ref int TotalCount)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteDataSet(command, PageIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(DbCommand command,string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteDataSet(command);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(DbCommand command)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteDataSet(command);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(string sqlcmd, int PageIndex, int PageSize, ref int TotalCount)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteDataSet(sqlcmd, PageIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <param name="PageIndex">Page Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSet(string sqlcmd, int PageIndex, int PageSize, ref int TotalCount,string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteDataSet(sqlcmd, PageIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <param name="StartIndex">Start Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSetForPage(string sqlcmd, int StartIndex, int PageSize, ref int TotalCount,string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteDataSetForPage(sqlcmd, StartIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search Dataset by SQL
        /// </summary>
        /// <param name="sqlcmd">SQL</param>
        /// <param name="StartIndex">Start Index</param>
        /// <param name="PageSize">Page Size</param>
        /// <param name="TotalCount">Total Count</param>
        /// <returns>dataset</returns>
        public static DataSet SearchOnDataSetForPage(string sqlcmd, int StartIndex, int PageSize, ref int TotalCount)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteDataSetForPage(sqlcmd, StartIndex, PageSize, ref TotalCount);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }


        /// <summary>
        /// Search A Value by SQL
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns>A Value</returns>
        public static object SearchAValue(string sqlcmd,string strConnctionName)
        {
            DataHelper dh = new DataHelper(strConnctionName);
            try
            {
                return dh.ExecuteScalar(sqlcmd);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Search A Value by SQL
        /// </summary>
        /// <param name="strCondition">SearchCondition</param>
        /// <returns>A Value</returns>
        public static object SearchAValue(string sqlcmd)
        {
            DataHelper dh = new DataHelper();
            try
            {
                return dh.ExecuteScalar(sqlcmd);
            }
            catch (Exception exp)
            {
                BRBase<T>.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// Batch Insert
        /// </summary>
        /// <param name="Set">Batch Insert EntitySet</param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<T> Set)
        {
            try
            {
                Set.BatInsert();
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Batch Insert
        /// </summary>
        /// <param name="Set">Batch Insert EntitySet</param>
        /// <param name="strConnctionName">連接字串名</param>
        /// <returns></returns>
        public static bool BatInsert(EntitySet<T> Set,string strConnctionName)
        {
            try
            {
                Set.BatInsert(strConnctionName);
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Batch Send Emails
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <param name="templateID">templateID</param>
        /// <param name="collection">collection</param>
        public static bool SendBatEmails(List<string> emails, int templateID, NameValueCollection collection)
        {
            //Batch Count
            int count = emails.Count / Configure.EmailNumberToSent;
            //The remaining Email
            int left = emails.Count % Configure.EmailNumberToSent;           

            try
            {
                //Batch Send Emails
                for (int round = 0; round < count; round++)
                {
                    List<string> emailsInputAll = new List<string>();
                    for (int num = round * Configure.EmailNumberToSent; num < (round + 1) * Configure.EmailNumberToSent; num++)
                    {
                        emailsInputAll.Add(emails[num]);  
                    }
                    MailService.MailhtmlSender(emailsInputAll.ToArray(), templateID, collection,string.Empty);
                }
                //Send The remaining Email
                if (left > 0)
                {
                    List<string> emailsInputLeft = new List<string>();
                    for (int num = count * Configure.EmailNumberToSent; num < emails.Count; num++)
                    {
                        emailsInputLeft.Add(emails[num]); 
                    }
                    MailService.MailhtmlSender(emailsInputLeft.ToArray(), templateID, collection,string.Empty);
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex);             
                return false;
            }
            return true;
        }
    }
}
