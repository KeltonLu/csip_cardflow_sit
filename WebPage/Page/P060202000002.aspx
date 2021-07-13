<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060202000002.aspx.cs" Inherits="Page_P060202000002" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>綜合資料處理修改</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript">
    
    function visabledrop()
    {
       document.getElementById("DropDownList1").style.display="none";
    }
    function unvisabledrop()
    {
       document.getElementById("DropDownList1").style.display="";
    }
        
    function IsConfirm(type)
    {
         var hidP =  document.getElementById("hidP");
         var name =  document.getElementById("hidPostData").value;
         switch(type)
         {

            case "A":
//                if (hidP.value == "xx") {
//                    return true;
//                }
//                if(hidP.value=="Trues")
//                {   
//                  return confirm("該卡已於"+name+ "郵寄，是否繼續?");
//                }
//                else
//                {
                    var hidA =  document.getElementById("hidA");
//                    if (hidA.value == "xx") {
//                        return true;
//                    }   
                    
                    if(hidA.value=="True")
                    {   
                        return confirm("已有未轉出作業單，確認修改?");
                    }
//                }
                break;
            case "C":
//                if(hidP.value=="Trues")
//                {   
//                  return confirm("該卡已於"+name+ "郵寄，是否繼續?");
//                }
//                else
//                {
                    var hidC =  document.getElementById("hidC");
                    if(hidC.value=="True")
                    {   
                        return confirm("已有未轉出作業單，確認修改?");
                    }
//                }
                break;
         }
    }
    
    function verify_date(date_str)
    {
     var myReg=new RegExp("^(?:(?:([0-9]{4}/(?:(?:0?[1,3-9]|1[0-2])/(?:29|30)|((?:0?[13578]|1[02])/31)))|([0-9]{4}/(?:0?[1-9]|1[0-2])/(?:0?[1-9]|1\\d|2[0-8]))|(((?:(\\d\\d(?:0[48]|[2468][048]|[13579][26]))|(?:0[48]00|[2468][048]00|[13579][26]00))/0?2/29))) (0?\\d|1\\d|2[0-3]))$");
     return myReg.test(date_str);
    }

    function check(type)
    {
        var hidP =  document.getElementById("hidP");
     switch(type)
     {
        case "A":
            var add2 =  document.getElementById("txtAdd2Ajax").value;
            if(add2=="")
            {  
                alert("地址二不可以為空");
                return false;
            }
            var hidA =  document.getElementById("hidA");
            if(hidA.value=="True")
            {   
                if(!confirm("已有未轉出作業單，修改後地址 :"+add2))
                {
                    document.getElementById("btnCancelA").click();
                    return false;
                }
                else
                {
                    document.getElementById("btnSureA").style.display ="none";
                    return true;
                }
            }
//            if(hidP.value=="Trues")
//            {   
//                var names =  document.getElementById("hidPostData").value;
//                if (!confirm("該卡已於"+names+ "郵寄，是否繼續"))
//                {
//                     document.getElementById("btnCancelN").click();
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }

            var lblAction = document.getElementById("lblAction");
            if(lblAction.innerText.substring(0,1) == "1")
            {
//              var UserName = document.getElementById("txtUserName");
                var Tel = document.getElementById("txtTel");
//              if(UserName.value == "")
//              {
//                 alert("連絡姓名不能為空");
//                 return false;
//              }
                if(Tel.value == "")
                {
                    alert("連絡電話不能為空");
                    return false;
                }
            }
            document.getElementById("btnSureA").style.display ="none";
        break
        case "C":
            var name =  document.getElementById("dropKindAjax");
            var index = name.selectedIndex;
            var value = name.options[index].text;
            if(value=="")
            {  
                alert("取卡方式不可以為空");
                return false;
            }
            var hidC =  document.getElementById("hidC");
            if(hidC.value=="True")
            {   
                //return confirm("已有未轉出作業單，修改後取卡方式 :"+value);
                if (!confirm("已有未轉出作業單，修改後取卡方式 :"+value))
                {
                    document.getElementById("btnCancelC").click();
                    return false;
                }
                else
                {
                    document.getElementById("btnSureC").style.display ="none";
                    return true;
                }
            }
//            if(hidP.value=="Trues")
//            {   
//                var names =  document.getElementById("hidPostData").value;
//                if (!confirm("該卡已於"+names+ "郵寄，是否繼續"))
//                {
//                     document.getElementById("btnCancelN").click();
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }
        document.getElementById("btnSureC").style.display ="none";
        break
      }  
    }
    </script>
    
      <script language="javascript" type="text/javascript">
		  window.addEventListener("scroll", scroll, false);

              function   scroll()   

              {   

                $("#divProgress").css("top",290+document.documentElement.scrollTop);

              }    

        </script> 

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_035" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_000" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel></td>
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_007" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblAction" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_002" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblname1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel><br />
                            <cc1:CustLabel ID="lblname2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" ForeColor="red" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_001" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblCardtype" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_005" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblTrandate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_009" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%" style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblAffinity" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_004" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblIndate1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel18" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_011" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblPhoto" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_006" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblMaildate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel><br />
                            <cc1:CustLabel ID="lblMaildate2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" ForeColor="red" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_008" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblMonlimit" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_012" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblCardno" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 15px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_014" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblCardno2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_013" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblExpdate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_015" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblExpdate2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_019" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_016" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblKind" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel><br />
                                <cc1:CustLabel ID="lblKind2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" ForeColor="red" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                    NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                            </div>
                            <div align="right">
                                <asp:Button ID="btnUpdateC" runat="server" Text="修改" CssClass="smallButton" Style="cursor: hand"
                                    OnClick="btnUpdateC_Click" OnClientClick="return IsConfirm('C')" />&nbsp;</div>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_074" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel><br />
                                <cc1:CustLabel ID="lblAdd1New" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" ForeColor="red" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                    NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                            </div>
                            <div align="right">
                                <asp:Button ID="btnUpdateA" runat="server" Text="修改" Style="cursor: hand" CssClass="smallButton"
                                    OnClick="btnUpdateA_Click" OnClientClick="return IsConfirm('A')" />&nbsp;</div>
                        </td>
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_018" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblMailno" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></div>
                            <div align="right">
                                &nbsp;</div>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_075" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel><br />
                            <cc1:CustLabel ID="lblAdd2New" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" ForeColor="red" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_017" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblMerch_Code" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_076" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel><br />
                            <cc1:CustLabel ID="lblAdd3New" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" ForeColor="red" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_077" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblSend_status_Name" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel86" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_078" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblSelfPick_Type" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel88" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_079" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblM_dates" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel89" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_080" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblSelfPick_Date" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel91" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_081" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblPost_Name" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="3">
                            <li>
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_083" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <cc1:CustGridView ID="grvUserView" runat="server" Width="100%" OnRowCommand="grvUserView_RowSelecting"
                                DataKeyNames="id" BorderStyle="Solid" CellSpacing="1" CellPadding="0" BorderWidth="0px"
                                PagerID="gpList" AllowPaging="False" AllowSorting="True" OnRowDataBound="grvUserView_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="Source">
                                        <itemstyle width="16%" horizontalalign="Center" wrap="True" verticalalign="Top"></itemstyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NoteCaptions">
                                        <itemstyle width="38%" horizontalalign="Left" wrap="True" verticalalign="Top"></itemstyle>
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle width="38%" horizontalalign="Left"></itemstyle>
                                        <itemtemplate>
<asp:TextBox id="txtcNote" runat="server" Text='<%# Bind("CNote") %>' Width="400px" TextMode="MultiLine" onkeydown="textCounter(this,200)" onkeyup="textCounter(this,200)" Height="51px"  MaxLength="200"></asp:TextBox>
</itemtemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemstyle width="10%" horizontalalign="Center"></itemstyle>
                                        <itemtemplate>
<cc1:CustButton id="btnSure" class="smallButton" runat="server" ShowID="06_06020101_023" Width="64px" CommandName="select" CommandArgument='<%# Bind("SNo") %>'></cc1:CustButton> 
</itemtemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <itemstyle width="40%" horizontalalign="Left"></itemstyle>
                                        <itemtemplate>
<asp:TextBox id="txtUser" runat="server" Text='<%# Bind("UpdUser") %>' Width="20px"></asp:TextBox> 
</itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <%--<tr class="itemTitle">
                        <td colspan="3">
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>--%>
                </table>
                <%--                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="3">
                            <li>
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_037" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" width="25%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_025" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblbackdate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td width="25%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_026" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblreason" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td width="50%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_022" StickHeight="False"></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="left" width="40%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_028" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblm_date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td width="40%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_029" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblpreenditem" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td width="20%" align="left" style="height: 25px">
                            <label>
                                <asp:TextBox ID="txtRemarkFB" runat="server" Height="60px" Width="288px" TextMode="MultiLine"></asp:TextBox></label></td>
                    </tr>
                </table>--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr align="center" class="itemTitle">
                        <td colspan="3" align="center" style="height: 25px">
                            <span style="height: 25px; width: 30%;">
                                <cc1:CustButton ID="btnBack" runat="server" class="smallButton" Style="width: 50px;"
                                    ShowID="06_06020101_034" OnClick="btnBack_Click" />
                            </span>
                        </td>
                    </tr>
                </table>
                <!--地址一-->
                <asp:ImageButton ID="AddButtonA" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderA" runat="server" TargetControlID="AddButtonA"
                    PopupControlID="PanA" BackgroundCssClass="modal" CancelControlID="btnCancelA"
                    DropShadow="False" Y="150" />
                <asp:Panel ID="PanA" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="683px" Height="51px">
                    <table id="tbContextMailAdd" width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td colspan="4" style="height: 30px">
                                <li id="li7">
                                    <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06020101_066" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="center" colspan="2" style="width: 50%; height: 25px">
                                <cc1:CustLabel ID="CustLabel79" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_067" StickHeight="False"></cc1:CustLabel></td>
                            <td align="center" colspan="2" style="width: 50%; height: 25px">
                                <cc1:CustLabel ID="CustLabel80" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_068" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd" id ="category_tr" runat="server">
                            <td colspan="2" style="width:50%;"></td>
                            <td align="right" colspan="1" style="width:20%; height: 25px">
                                <cc1:CustLabel ID="category" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_086" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" colspan="1" style="width:30%; height: 25px">
                                <cc1:CustRadioButton ID="newCardnewAccount" runat="server" Checked="false" BorderStyle="None"
								GroupName="categroy"/>
                                <cc1:CustRadioButton ID="newCardoldAccount" runat="server" Checked="false" BorderStyle="None"
								GroupName="categroy" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel70" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_069" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblOldzipAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel75" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_069" StickHeight="False"></cc1:CustLabel>
                                <label>
                                </label>
                            </td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblNewzipAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel71" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_070" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblAdd1Ajax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel76" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_070" StickHeight="False"></cc1:CustLabel>
                                <label>
                                </label>
                            </td>
                            <td align="left" style="width: 30%; height: 25px">
                                &nbsp;<uc1:CustAddress ID="CustAdd1" runat="server" OnChangeValues="CustAdd1_ChangeValues" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel72" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_071" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblAdd2Ajax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_071" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <asp:TextBox ID="txtAdd2Ajax" onkeydown="textCounter(this,40)" onkeyup="textCounter(this,40)" runat="server"
                                        Width="164px"></asp:TextBox></label></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel73" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_072" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <cc1:CustLabel ID="lblAdd3Ajax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel78" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_072" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <label>
                                    <asp:TextBox ID="txtAdd3Ajax" onkeydown="textCounter(this,40)" onkeyup="textCounter(this,40)" runat="server"
                                        Width="164px"></asp:TextBox></label></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%; height: 25px">
                            </td>
                            <td align="left" style="width: 30%; height: 25px">
                            </td>
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_02020003_029" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 30%; height: 25px">
                                <asp:TextBox ID="txtTel" runat="server" onkeydown="textCounter(this,40)" onkeyup="textCounter(this,40)" Width="164px"></asp:TextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 20%; height: 25px">
                                <cc1:CustLabel ID="CustLabel74" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_073" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" colspan="3" style="width: 35%; height: 25px">
                                <span style="width: 80%; height: 25px">
                                    <asp:TextBox ID="txtCNoteA" runat="server" Height="60px" TextMode="MultiLine" onkeydown="textCounter(this,200)" onkeyup="textCounter(this,200)"
                                        Width="288px" MaxLength="200"></asp:TextBox></span></td>
                        </tr>
                        <tr align="left" class="itemTitle">
                            <td colspan="4" align="center">
                                &nbsp;<asp:Button ID="btnSureA" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureA_Click"
                                    OnClientClick="return check('A')" />
                                <asp:Button ID="btnCancelA" runat="server" Text="取消" CssClass="smallButton" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                <!--取卡方式-->
                <asp:ImageButton ID="AddButtonC" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderC" runat="server" TargetControlID="AddButtonC"
                    PopupControlID="PanC" BackgroundCssClass="modal" CancelControlID="btnCancelC"
                    DropShadow="False" />
                <asp:Panel ID="PanC" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="336px" Height="83px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 122%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li id="li5">
                                    <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06020101_058" StickHeight="False"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel60" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_059" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <cc1:CustLabel ID="lblKindAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_060" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <asp:DropDownList ID="dropKindAjax" runat="server" Width="160px">
                                </asp:DropDownList></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_061" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <asp:TextBox ID="txtCNoteC" runat="server" Height="60px" TextMode="MultiLine" onkeydown="textCounter(this,200)" onkeyup="textCounter(this,200)"
                                    Width="288px" MaxLength="200"></asp:TextBox></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center">
                                <asp:Button ID="btnSureC" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureC_Click"
                                    OnClientClick="return check('C')" />
                                <asp:Button ID="btnCancelC" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:HiddenField ID="hidN" runat="server" />
                <!--寄件人姓名-->
                <asp:HiddenField ID="hidP" runat="server" />
                <!--郵寄日期-->
                <asp:HiddenField ID="hidA" runat="server" />
                <!--郵寄地址-->
                <asp:HiddenField ID="hidM" runat="server" />
                <!--額度-->
                <asp:HiddenField ID="hidC" runat="server" />
                <!--取卡方式-->
                <asp:HiddenField ID="hidG" runat="server" />
                <!--掛號號碼-->
                <asp:HiddenField ID="hidPostData" runat="server" />
                <!--實際郵寄日期-->
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <asp:UpdateProgress ID="updateProgress1" runat="server">

            <ProgressTemplate>

                <div id="divProgress" align="center" class="progress" style="position: absolute;

                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center; z-index:10003;">

                    <div id="divProgress2" align="center" class="progress" style="background-color: White;

                        width: 50%; margin: 0px auto; z-index:10004;">

                        <br />

                        <img alt="Please Wait..." src="../Common/images/Waiting.gif" />

                        <br />

                        <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"

                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"

                            SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False"></cc1:CustLabel>

                    </div>

                </div>

            </ProgressTemplate>
        </asp:UpdateProgress>
        
    </form>
</body>
</html>
