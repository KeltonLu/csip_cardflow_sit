//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.42
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace EntityLayer
{
    
    
    /// <summary>
    /// tbl_Card_DailyClose
    /// </summary>
    [Serializable()]
    [AttributeTable("tbl_Card_DailyClose")]
    public class Entity_CardDailyClose : Framework.Data.OM.Entity
    {
        
        private int _DailyCloseId;
        
        /// <summary>
        /// DailyCloseId
        /// </summary>
        public static string M_DailyCloseId = "DailyCloseId";
        
        private string _DailyCloseDate;
        
        /// <summary>
        /// DailyCloseDate
        /// </summary>
        public static string M_DailyCloseDate = "DailyCloseDate";
        
        private string _DailyCloseCount;
        
        /// <summary>
        /// DailyCloseCount
        /// </summary>
        public static string M_DailyCloseCount = "DailyCloseCount";
        
        /// <summary>
        /// DailyCloseId
        /// </summary>
        [AttributeField("DailyCloseId", "System.Int32", false, false, false, "Int32")]
        public int DailyCloseId
        {
            get
            {
                return this._DailyCloseId;
            }
            set
            {
                this._DailyCloseId = value;
            }
        }
        
        /// <summary>
        /// DailyCloseDate
        /// </summary>
        [AttributeField("DailyCloseDate", "System.String", false, false, false, "String")]
        public string DailyCloseDate
        {
            get
            {
                return this._DailyCloseDate;
            }
            set
            {
                this._DailyCloseDate = value;
            }
        }
        
        /// <summary>
        /// DailyCloseCount
        /// </summary>
        [AttributeField("DailyCloseCount", "System.String", false, false, false, "String")]
        public string DailyCloseCount
        {
            get
            {
                return this._DailyCloseCount;
            }
            set
            {
                this._DailyCloseCount = value;
            }
        }
    }
    
    /// <summary>
    /// tbl_Card_DailyClose
    /// </summary>
    [Serializable()]
    public class Entity_CardDailyCloseSet : EntitySet<Entity_CardDailyClose>
    {
    }
}
