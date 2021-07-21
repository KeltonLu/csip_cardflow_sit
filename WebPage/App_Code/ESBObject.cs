using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ESBObject 的摘要描述
/// </summary>
public class ESBObject
{
    public ESBObject()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public string ConnStatus { get; set; }
    public string StatusCode { get; set; }
    public string RspCode { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

    public virtual string getXML()
    {
        return "";
    }

    public virtual bool CheckResult(string strResult)
    {
        return false;
    }

    public virtual void getResult(string strResult)
    {

    }

    public virtual string getStatusCodeTag()
    {
        return "";
    }
    public virtual string getRspCodeTag()
    {
        return "";
    }
    public virtual string getErrorCodeTag()
    {
        return "";
    }
    public virtual string getErrorMessageTag()
    {
        return "";
    }
}