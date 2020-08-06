using System;

namespace Framework.Data.OM.OMAttribute
{
    /// <summary>
    /// Mapping Field Attribute
    /// </summary>     
    [System.Serializable]
    public class AttributeRfPage : Attribute
    {
        #region Private Field

        private string _ControlID; //對應的控制項
        /// <summary>
        ///控制項 Name ，可以多重設置，請注意以逗點分隔，且IsMutip屬性必須設為true，方可正確對應
        /// </summary>
        public string ControlID
        {
            set { _ControlID = value; }
            get { return _ControlID; }
        }

        private object _Value;
        /// <summary>
        /// 對應的控制項Value  暫未使用
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }


        private string _FieldType;
        /// <summary>
        ///對應的控制項型別  限單一型別，不須全名(目前僅以控制項類別區分，如Label，TextBox等)
        /// </summary>
        public string FieldType
        {
            get { return _FieldType; }
            set { _FieldType = value; }
        }


        private bool _IsMutip = false;
        /// <summary>
        /// 是否多選
        /// </summary>
        public bool IsMutip
        {
            get { return _IsMutip; }
            set { _IsMutip = value; }
        }
        private string _CreateFieldType;
        /// <summary>
        ///動態產生控制項類型 ，原則上與源控制項不一定依樣(CustDropDownList) 
        /// </summary>
        public string CreateFieldType
        {
            ///無設定時，預設為FieldType
            get
            {
                if (_CreateFieldType == null)
                {
                    _CreateFieldType = _FieldType;
                }
                return _CreateFieldType;
            }
            set { _CreateFieldType = value; }
        }
        private string _JC66NAME;
        /// <summary>
        ///動態產生JC66用 
        /// </summary>
        public string JC66NAME
        {
            ///無設定時，預設為FieldType
            get
            {  
                return _JC66NAME;
            }
            set { _JC66NAME = value; }
        }

        private int _JC6Len =0;
        /// <summary>
        ///動態產生JC66用 長度限制
        /// </summary>
        public int JC66Len
        {
            ///無設定時，不補空白
            get
            { 
                return _JC6Len;
            }
            set { _JC6Len = value; }
        }
        private char _JC6LenChar ;
        /// <summary>
        ///動態產生JC66用 長度限制
        /// </summary>
        public char JC6LenChar
        {
            ///無設定時，不補空白
            get
            {
                return _JC6LenChar;
            }
            set { _JC6LenChar = value; }
        }
        private bool _JC6PadLeft ;
        /// <summary>
        ///動態產生JC66用 是否靠左補齊，若非TRUE則靠右
        /// </summary>
        public bool JC6PadLeft
        {
            ///無設定時，不補空白
            get
            {
                return _JC6PadLeft;
            }
            set { _JC6PadLeft = value; }
        }
        #endregion

        #region Constructed Function

        /// <summary>
        /// Attribute Field Constructed Function +1多載
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="fieldType"></param>
        /// <param name="Ismutip"></param>
        public AttributeRfPage(string controlID, string fieldType, bool Ismutip)
        {
            this._ControlID = controlID;
            this._FieldType = fieldType;
            this._IsMutip = Ismutip;

        }
        /// <summary>
        /// Attribute Field Constructed Function +1多載
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="fieldType"></param>
        /// <param name="Ismutip"></param>
        public AttributeRfPage(string controlID, string fieldType, bool Ismutip, string CreateFieldType)
        {
            this._ControlID = controlID;
            this._FieldType = fieldType;
            this._IsMutip = Ismutip;
            this._CreateFieldType = CreateFieldType;
        }
          /// <summary>
        /// Attribute Field Constructed Function +1多載
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="fieldType"></param>
        /// <param name="Ismutip"></param>
        public AttributeRfPage(string controlID, string fieldType, bool Ismutip, string CreateFieldType,string JC66NAME)
        {
            this._ControlID = controlID;
            this._FieldType = fieldType;
            this._IsMutip = Ismutip;
            //this._CreateFieldType = CreateFieldType;
            this._JC66NAME = JC66NAME;

        }
        /// <summary>
        /// 補充HSAH電文 設定靠左或靠右捕到定長，補充字元可設定
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="fieldType"></param>
        /// <param name="Ismutip"></param>
        /// <param name="CreateFieldType"></param>
        /// <param name="JC66NAME"></param>
        /// <param name="Jc66Len"></param>
        /// <param name="Jc66LenChar"></param>
        /// <param name="isPadLeft"></param>
        public AttributeRfPage(string controlID, string fieldType, bool Ismutip, string CreateFieldType, string JC66NAME,int Jc66Len2,char Jc66LenChar,bool isPadLeft)
        {
            this._ControlID = controlID;
            this._FieldType = fieldType;
            this._IsMutip = Ismutip;
            //this._CreateFieldType = CreateFieldType;
            this._JC66NAME = JC66NAME;
            this.JC66Len = Jc66Len2;
            this.JC6LenChar = Jc66LenChar;
            this.JC6PadLeft = isPadLeft;
        }
        #endregion

    }
}
