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
    /// tbl_Post
    /// </summary>
    [Serializable()]
    [AttributeTable("tbl_Post")]
    public class Entity_Post : Framework.Data.OM.Entity
    {

        private int _Sno;

        /// <summary>
        /// Sno
        /// </summary>
        public static string M_Sno = "Sno";

        private string _Cardno;

        /// <summary>
        /// Cardno
        /// </summary>
        public static string M_Cardno = "Cardno";

        private string _Podate;

        /// <summary>
        /// Podate
        /// </summary>
        public static string M_Podate = "Podate";

        private string _Backdate;

        /// <summary>
        /// Backdate
        /// </summary>
        public static string M_Backdate = "Backdate";

        private string _Endcase;

        /// <summary>
        /// Endcase
        /// </summary>
        public static string M_Endcase = "Endcase";

        private string _EndCaseFlg;

        /// <summary>
        /// EndCaseFlg
        /// </summary>
        public static string M_EndCaseFlg = "EndCaseFlg";

        private string _Uid;

        /// <summary>
        /// Uid
        /// </summary>
        public static string M_Uid = "Uid";

        private string _InputDate;

        /// <summary>
        /// InputDate
        /// </summary>
        public static string M_InputDate = "InputDate";

        private string _Note;

        /// <summary>
        /// Note
        /// </summary>
        public static string M_Note = "Note";

        private string _OutPutDate;

        /// <summary>
        /// OutPutDate
        /// </summary>
        public static string M_OutPutDate = "OutPutDate";

        private string _OutPutFile;

        /// <summary>
        /// OutPutFile
        /// </summary>
        public static string M_OutPutFile = "OutPutFile";

        private string _Stateflg;

        /// <summary>
        /// Stateflg
        /// </summary>
        public static string M_Stateflg = "Stateflg";

        /// <summary>
        /// Sno
        /// </summary>
        [AttributeField("Sno", "System.Int32", false, true, true, "Int32")]
        public int Sno
        {
            get
            {
                return this._Sno;
            }
            set
            {
                this._Sno = value;
            }
        }

        /// <summary>
        /// Cardno
        /// </summary>
        [AttributeField("Cardno", "System.String", false, false, false, "String")]
        public string Cardno
        {
            get
            {
                return this._Cardno;
            }
            set
            {
                this._Cardno = value;
            }
        }

        /// <summary>
        /// Podate
        /// </summary>
        [AttributeField("Podate", "System.String", false, false, false, "String")]
        public string Podate
        {
            get
            {
                return this._Podate;
            }
            set
            {
                this._Podate = value;
            }
        }

        /// <summary>
        /// Backdate
        /// </summary>
        [AttributeField("Backdate", "System.String", false, false, false, "String")]
        public string Backdate
        {
            get
            {
                return this._Backdate;
            }
            set
            {
                this._Backdate = value;
            }
        }

        /// <summary>
        /// Endcase
        /// </summary>
        [AttributeField("Endcase", "System.String", false, false, false, "String")]
        public string Endcase
        {
            get
            {
                return this._Endcase;
            }
            set
            {
                this._Endcase = value;
            }
        }

        /// <summary>
        /// EndCaseFlg
        /// </summary>
        [AttributeField("EndCaseFlg", "System.String", false, false, false, "String")]
        public string EndCaseFlg
        {
            get
            {
                return this._EndCaseFlg;
            }
            set
            {
                this._EndCaseFlg = value;
            }
        }

        /// <summary>
        /// Uid
        /// </summary>
        [AttributeField("Uid", "System.String", false, false, false, "String")]
        public string Uid
        {
            get
            {
                return this._Uid;
            }
            set
            {
                this._Uid = value;
            }
        }

        /// <summary>
        /// InputDate
        /// </summary>
        [AttributeField("InputDate", "System.String", false, false, false, "String")]
        public string InputDate
        {
            get
            {
                return this._InputDate;
            }
            set
            {
                this._InputDate = value;
            }
        }

        /// <summary>
        /// Note
        /// </summary>
        [AttributeField("Note", "System.String", false, false, false, "String")]
        public string Note
        {
            get
            {
                return this._Note;
            }
            set
            {
                this._Note = value;
            }
        }

        /// <summary>
        /// OutPutDate
        /// </summary>
        [AttributeField("OutPutDate", "System.String", false, false, false, "String")]
        public string OutPutDate
        {
            get
            {
                return this._OutPutDate;
            }
            set
            {
                this._OutPutDate = value;
            }
        }

        /// <summary>
        /// OutPutFile
        /// </summary>
        [AttributeField("OutPutFile", "System.String", false, false, false, "String")]
        public string OutPutFile
        {
            get
            {
                return this._OutPutFile;
            }
            set
            {
                this._OutPutFile = value;
            }
        }

        /// <summary>
        /// Stateflg
        /// </summary>
        [AttributeField("Stateflg", "System.String", false, false, false, "String")]
        public string Stateflg
        {
            get
            {
                return this._Stateflg;
            }
            set
            {
                this._Stateflg = value;
            }
        }
    }

    /// <summary>
    /// tbl_Post
    /// </summary>
    [Serializable()]
    public class Entity_PostSet : EntitySet<Entity_Post>
    {
    }

}
