using Framework.Data.OM.OMAttribute;
using System;

[Serializable()]
[AttributeTable("tbl_Card_BaseInfo")]
/// <summary>
/// EntityL_Card_BaseInfo 的摘要描述
/// </summary>
public class EntityL_Card_BaseInfo :   Framework.Data.OM.Entity
{

    public EntityL_Card_BaseInfo()
    {
    }

    private string _indate1;
    /// <summary>
    /// indate1
    /// </summary>
    [AttributeRfPage("txtindate1", "CustTextBox", false)]
    public string indate1
    {
        get
        {
            return this._indate1;
        }
        set
        {
            this._indate1 = value;
        }
    }
    private string _action;
    /// <summary>
    /// action
    /// </summary>
    [AttributeRfPage("txtaction", "CustTextBox", false)]
    public string action
    {
        get
        {
            return this._action;
        }
        set
        {
            this._action = value;
        }
    }
    private string _kind;
    /// <summary>
    /// kind
    /// </summary>
    [AttributeRfPage("txtkind", "CustTextBox", false)]
    public string kind
    {
        get
        {
            return this._kind;
        }
        set
        {
            this._kind = value;
        }
    }
    private string _cardtype;
    /// <summary>
    /// cardtype
    /// </summary>
    [AttributeRfPage("txtcardtype", "CustTextBox", false)]
    public string cardtype
    {
        get
        {
            return this._cardtype;
        }
        set
        {
            this._cardtype = value;
        }
    }
    private string _photo;
    /// <summary>
    /// photo
    /// </summary>
    [AttributeRfPage("txtphoto", "CustTextBox", false)]
    public string photo
    {
        get
        {
            return this._photo;
        }
        set
        {
            this._photo = value;
        }
    }
    private string _affinity;
    /// <summary>
    /// affinity
    /// </summary>
    [AttributeRfPage("txtaffinity", "CustTextBox", false)]
    public string affinity
    {
        get
        {
            return this._affinity;
        }
        set
        {
            this._affinity = value;
        }
    }
    private string _id;
    /// <summary>
    /// id
    /// </summary>
    [AttributeRfPage("txtid", "CustTextBox", false)]
    public string id
    {
        get
        {
            return this._id;
        }
        set
        {
            this._id = value;
        }
    }
    private string _cardno;
    /// <summary>
    /// cardno
    /// </summary>
    [AttributeRfPage("txtcardno", "CustTextBox", false)]
    public string cardno
    {
        get
        {
            return this._cardno;
        }
        set
        {
            this._cardno = value;
        }
    }
    private string _cardno2;
    /// <summary>
    /// cardno2
    /// </summary>
    [AttributeRfPage("txtcardno2", "CustTextBox", false)]
    public string cardno2
    {
        get
        {
            return this._cardno2;
        }
        set
        {
            this._cardno2 = value;
        }
    }
    private string _zip;
    /// <summary>
    /// zip
    /// </summary>
    [AttributeRfPage("txtzip", "CustTextBox", false)]
    public string zip
    {
        get
        {
            return this._zip;
        }
        set
        {
            this._zip = value;
        }
    }
    private string _add1;
    /// <summary>
    /// add1
    /// </summary>
    [AttributeRfPage("txtadd1", "CustTextBox", false)]
    public string add1
    {
        get
        {
            return this._add1;
        }
        set
        {
            this._add1 = value;
        }
    }
    private string _add2;
    /// <summary>
    /// add2
    /// </summary>
    [AttributeRfPage("txtadd2", "CustTextBox", false)]
    public string add2
    {
        get
        {
            return this._add2;
        }
        set
        {
            this._add2 = value;
        }
    }
    private string _add3;
    /// <summary>
    /// add3
    /// </summary>
    [AttributeRfPage("txtadd3", "CustTextBox", false)]
    public string add3
    {
        get
        {
            return this._add3;
        }
        set
        {
            this._add3 = value;
        }
    }
    private string _mailno;
    /// <summary>
    /// mailno
    /// </summary>
    [AttributeRfPage("txtmailno", "CustTextBox", false)]
    public string mailno
    {
        get
        {
            return this._mailno;
        }
        set
        {
            this._mailno = value;
        }
    }
    private string _n_card;
    /// <summary>
    /// n_card
    /// </summary>
    [AttributeRfPage("txtn_card", "CustTextBox", false)]
    public string n_card
    {
        get
        {
            return this._n_card;
        }
        set
        {
            this._n_card = value;
        }
    }
    private string _maildate;
    /// <summary>
    /// maildate
    /// </summary>
    [AttributeRfPage("txtmaildate", "CustTextBox", false)]
    public string maildate
    {
        get
        {
            return this._maildate;
        }
        set
        {
            this._maildate = value;
        }
    }
    private string _expdate;
    /// <summary>
    /// expdate
    /// </summary>
    [AttributeRfPage("txtexpdate", "CustTextBox", false)]
    public string expdate
    {
        get
        {
            return this._expdate;
        }
        set
        {
            this._expdate = value;
        }
    }
    private string _expdate2;
    /// <summary>
    /// expdate2
    /// </summary>
    [AttributeRfPage("txtexpdate2", "CustTextBox", false)]
    public string expdate2
    {
        get
        {
            return this._expdate2;
        }
        set
        {
            this._expdate2 = value;
        }
    }
    private string _seq;
    /// <summary>
    /// seq
    /// </summary>
    [AttributeRfPage("txtseq", "CustTextBox", false)]
    public string seq
    {
        get
        {
            return this._seq;
        }
        set
        {
            this._seq = value;
        }
    }
    private string _custname;
    /// <summary>
    /// custname
    /// </summary>
    [AttributeRfPage("txtcustname", "CustTextBox", false)]
    public string custname
    {
        get
        {
            return this._custname;
        }
        set
        {
            this._custname = value;
        }
    }
    private string _name1;
    /// <summary>
    /// name1
    /// </summary>
    [AttributeRfPage("txtname1", "CustTextBox", false)]
    public string name1
    {
        get
        {
            return this._name1;
        }
        set
        {
            this._name1 = value;
        }
    }
    private string _name2;
    /// <summary>
    /// name2
    /// </summary>
    [AttributeRfPage("txtname2", "CustTextBox", false)]
    public string name2
    {
        get
        {
            return this._name2;
        }
        set
        {
            this._name2 = value;
        }
    }
    private string _trandate;
    /// <summary>
    /// trandate
    /// </summary>
    [AttributeRfPage("txttrandate", "CustTextBox", false)]
    public string trandate
    {
        get
        {
            return this._trandate;
        }
        set
        {
            this._trandate = value;
        }
    }
    private string _card_file;
    /// <summary>
    /// card_file
    /// </summary>
    [AttributeRfPage("txtcard_file", "CustTextBox", false)]
    public string card_file
    {
        get
        {
            return this._card_file;
        }
        set
        {
            this._card_file = value;
        }
    }
    private string _disney_code;
    /// <summary>
    /// disney_code
    /// </summary>
    [AttributeRfPage("txtdisney_code", "CustTextBox", false)]
    public string disney_code
    {
        get
        {
            return this._disney_code;
        }
        set
        {
            this._disney_code = value;
        }
    }
    private string _branch_id;
    /// <summary>
    /// branch_id
    /// </summary>
    [AttributeRfPage("txtbranch_id", "CustTextBox", false)]
    public string branch_id
    {
        get
        {
            return this._branch_id;
        }
        set
        {
            this._branch_id = value;
        }
    }
    private string _Merch_Code;
    /// <summary>
    /// Merch_Code
    /// </summary>
    [AttributeRfPage("txtMerch_Code", "CustTextBox", false)]
    public string Merch_Code
    {
        get
        {
            return this._Merch_Code;
        }
        set
        {
            this._Merch_Code = value;
        }
    }
    private string _monlimit;
    /// <summary>
    /// monlimit
    /// </summary>
    [AttributeRfPage("txtmonlimit", "CustTextBox", false)]
    public string monlimit
    {
        get
        {
            return this._monlimit;
        }
        set
        {
            this._monlimit = value;
        }
    }
    private string _is_LackCard;
    /// <summary>
    /// is_LackCard
    /// </summary>
    [AttributeRfPage("txtis_LackCard", "CustTextBox", false)]
    public string is_LackCard
    {
        get
        {
            return this._is_LackCard;
        }
        set
        {
            this._is_LackCard = value;
        }
    }
    private string _Urgency_Flg;
    /// <summary>
    /// Urgency_Flg
    /// </summary>
    [AttributeRfPage("txtUrgency_Flg", "CustTextBox", false)]
    public string Urgency_Flg
    {
        get
        {
            return this._Urgency_Flg;
        }
        set
        {
            this._Urgency_Flg = value;
        }
    }
    private string _IntoStore_Status;
    /// <summary>
    /// IntoStore_Status
    /// </summary>
    [AttributeRfPage("txtIntoStore_Status", "CustTextBox", false)]
    public string IntoStore_Status
    {
        get
        {
            return this._IntoStore_Status;
        }
        set
        {
            this._IntoStore_Status = value;
        }
    }
    private string _IntoStore_Date;
    /// <summary>
    /// IntoStore_Date
    /// </summary>
    [AttributeRfPage("txtIntoStore_Date", "CustTextBox", false)]
    public string IntoStore_Date
    {
        get
        {
            return this._IntoStore_Date;
        }
        set
        {
            this._IntoStore_Date = value;
        }
    }
    private string _OutStore_Status;
    /// <summary>
    /// OutStore_Status
    /// </summary>
    [AttributeRfPage("txtOutStore_Status", "CustTextBox", false)]
    public string OutStore_Status
    {
        get
        {
            return this._OutStore_Status;
        }
        set
        {
            this._OutStore_Status = value;
        }
    }
    private string _OutStore_Date;
    /// <summary>
    /// OutStore_Date
    /// </summary>
    [AttributeRfPage("txtOutStore_Date", "CustTextBox", false)]
    public string OutStore_Date
    {
        get
        {
            return this._OutStore_Date;
        }
        set
        {
            this._OutStore_Date = value;
        }
    }
    private string _SelfPick_Type;
    /// <summary>
    /// SelfPick_Type
    /// </summary>
    [AttributeRfPage("txtSelfPick_Type", "CustTextBox", false)]
    public string SelfPick_Type
    {
        get
        {
            return this._SelfPick_Type;
        }
        set
        {
            this._SelfPick_Type = value;
        }
    }
    private string _SelfPick_Date;
    /// <summary>
    /// SelfPick_Date
    /// </summary>
    [AttributeRfPage("txtSelfPick_Date", "CustTextBox", false)]
    public string SelfPick_Date
    {
        get
        {
            return this._SelfPick_Date;
        }
        set
        {
            this._SelfPick_Date = value;
        }
    }
    private string _OriginalDBflg;
    /// <summary>
    /// OriginalDBflg
    /// </summary>
    [AttributeRfPage("txtOriginalDBflg", "CustTextBox", false)]
    public string OriginalDBflg
    {
        get
        {
            return this._OriginalDBflg;
        }
        set
        {
            this._OriginalDBflg = value;
        }
    }

}
/// <summary>
/// 緊急製卡匯入資料結構
/// </summary>
public class BatchImport_UrgencyCard : Framework.Data.OM.Entity
{
    private string _indate1;
    /// <summary>
    /// indate1
    /// </summary>
    [AttributeRfPage("txtindate1", "CustTextBox", false)]
    public string indate1
    {
        get
        {
            return this._indate1;
        }
        set
        {
            this._indate1 = value;
        }
    }
    /// <summary>
    /// 查詢用日期區間
    /// </summary>
    private string _indate2;
    /// <summary>
    /// indate1
    /// </summary>
    [AttributeRfPage("txtindate2", "CustTextBox", false)]
    public string indate2
    {
        get
        {
            return this._indate2;
        }
        set
        {
            this._indate2 = value;
        }
    }
    private string _id;
    /// <summary>
    /// id
    /// </summary>
    [AttributeRfPage("txtid", "CustTextBox", false)]
    public string id
    {
        get
        {
            return this._id;
        }
        set
        {
            this._id = value;
        }
    }
    private string _cardno;
    /// <summary>
    /// cardno
    /// </summary>
    [AttributeRfPage("txtcardno", "CustTextBox", false)]
    public string cardno
    {
        get
        {
            return this._cardno;
        }
        set
        {
            this._cardno = value;
        }
    }
    private string _kind;
    /// <summary>
    /// kind
    /// </summary>
    [AttributeRfPage("txtkind", "CustTextBox", false)]
    public string kind
    {
        get
        {
            return this._kind;
        }
        set
        {
            this._kind = value;
        }
    }
    private string _card_file;
    /// <summary>
    /// card_file
    /// </summary>
    [AttributeRfPage("txtcard_file", "CustTextBox", false)]
    public string card_file
    {
        get
        {
            return this._card_file;
        }
        set
        {
            this._card_file = value;
        }
    }
    private string _result;
    /// <summary>
    /// result
    /// </summary>
    [AttributeRfPage("txtresult", "CustTextBox", false)]
    public string result
    {
        get
        {
            return this._result;
        }
        set
        {
            this._result = value;
        }
    }
    private string _fail_reason;
    /// <summary>
    /// reason
    /// </summary>
    [AttributeRfPage("txtfail_reason", "CustTextBox", false)]
    public string fail_reason
    {
        get
        {
            return this._fail_reason;
        }
        set
        {
            this._fail_reason = value;
        }
    }
    private string _reason;
    /// <summary>
    /// reason
    /// </summary>
    [AttributeRfPage("txtreason", "CustTextBox", false)]
    public string reason
    {
        get
        {
            return this._reason;
        }
        set
        {
            this._reason = value;
        }
    }
    private string _import_time;
    /// <summary>
    /// import_time
    /// </summary>
    [AttributeRfPage("txtimport_time", "CustTextBox", false)]
    public string import_time
    {
        get
        {
            return this._import_time;
        }
        set
        {
            this._import_time = value;
        }
    }
    private string _import_user;
    /// <summary>
    /// import_user
    /// </summary>
    [AttributeRfPage("txtimport_user", "CustTextBox", false)]
    public string import_user
    {
        get
        {
            return this._import_user;
        }
        set
        {
            this._import_user = value;
        }
    }


}