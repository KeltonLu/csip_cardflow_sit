<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060205000003.aspx.cs" Inherits="P060205000003" %>

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
       
    function verify_date(date_str)
    {
     var myReg=new RegExp("^(?:(?:([0-9]{4}/(?:(?:0?[1,3-9]|1[0-2])/(?:29|30)|((?:0?[13578]|1[02])/31)))|([0-9]{4}/(?:0?[1-9]|1[0-2])/(?:0?[1-9]|1\\d|2[0-8]))|(((?:(\\d\\d(?:0[48]|[2468][048]|[13579][26]))|(?:0[48]00|[2468][048]00|[13579][26]00))/0?2/29))) (0?\\d|1\\d|2[0-3]))$");
     return myReg.test(date_str);
    }

    function check(type)
    {
     switch(type)
     {
        case "N":
            var name =  document.getElementById("txtName1Ajax").value;
            if(name=="")
            {  
                alert("寄件人姓名不可以為空");
                return false;
            }

        break
        case "P":
            var name =  document.getElementById("txtMaildateAjax").value;
            if(name=="")
            {  
                alert("郵寄日不可以為空");
                return false;
            }
            if(verify_date(name))
            {
                alert("郵寄日格式必須為yyyy/mm/dd");
                return false;
            }

        break
        case "A":
            var add2 =  document.getElementById("txtAdd2Ajax").value;
            if(add2=="")
            {  
                alert("地址二不可以為空");
                return false;
            }

        break
        case "M":
            var name =  document.getElementById("dropSelfPickAjax");
            var index = name.selectedIndex;
            var value = name.options[index].text;
            if(value=="")
            {  
                alert("領卡方式不可以為空");
                return false;
            }

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

        break
        case "G":
            var name =  document.getElementById("txtMailnoAjax").value;
            if(name=="")
            {  
                alert("掛號號碼不可以為空");
                return false;
            }

        break 
      }  
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
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_035" StickHeight="False"></cc1:CustLabel></li></td>
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
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblname1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel></div>
                            <div align="right">
                                &nbsp;</div>
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
                        <td width="35%" style="height: 25px ; width: 35%;">
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
                        <td style="height: 25px ; width: 35%;">
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
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblMaildate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></div>
                            <div align="right">
                                &nbsp;</div>
                        </td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_008" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px; width: 35%;">
                            <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblMonlimit" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></div>
                            <div align="right">
                                &nbsp;</div></td>
                    </tr>
                    <tr class="trEven">
                        <td style="height: 25px; width:15%;" align="right">
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
                        <td  style="height: 25px; width: 35%;">
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
                        <td  style="height: 25px; width: 35%;">
                           <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                                <cc1:CustLabel ID="lblKind" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></div>
                            <div align="right">
                           <asp:Button ID="btnUpdateC" runat="server" Text="修改" CssClass="smallButton" OnClick="btnUpdateC_Click" style="cursor:hand" Visible="False"/>&nbsp;</div></td>
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
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></div>
                            <div align="right">
                                <asp:Button ID="btnUpdateA" runat="server" Text="修改" CssClass="smallButton" OnClick="btnUpdateA_Click" style="cursor:hand" Visible="False"/>&nbsp;</div>
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
                                <asp:Button ID="btnUpdateG" runat="server" Text="修改" CssClass="smallButton" OnClick="btnUpdateG_Click" style="cursor:hand" Visible="False"/>&nbsp;</div></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_075" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
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
                                SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 15%;" align="right">
                            <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_077" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                            <cc1:CustLabel ID="lblSend_status_Name" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel86" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_078" StickHeight="False"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 35%;">
                             <div align="left" style="float: left; padding-top: 2; padding-bottom: 2; height: 100%;">
                            <cc1:CustLabel ID="lblSelfPick_Type" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></div>
                            <div align="right">
                            <asp:Button ID="btnUpdateM" runat="server" Text="修改" CssClass="smallButton" OnClick="btnUpdateM_Click" style="cursor:hand"/>
                            &nbsp;</div></td>                        
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
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_036" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <cc1:CustGridView ID="grvUserView" runat="server" Width="100%" OnRowEditing="grvUserView_RowEditing"
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
<asp:TextBox id="txtcNote" runat="server" Text='<%# Bind("CNote") %>' Width="400px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)" Height="51px"  MaxLength="200"></asp:TextBox>
</itemtemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <itemstyle width="10%" horizontalalign="Center"></itemstyle>
                                        <itemtemplate>
<cc1:CustButton id="btnSure" class="smallButton" runat="server" ShowID="06_06020101_023" Width="64px" CommandName="edit" CommandArgument='<%# Bind("SNo") %>'></cc1:CustButton> 
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
                </table>
               <table width="100%" border="0" cellpadding="0" cellspacing="1">
                <tr class="itemTitle">
                    <td colspan="3">
                        <li>
                            <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_037" StickHeight="False"></cc1:CustLabel></li></td>
                </tr>
                </table>
               <cc1:CustPanel ID="pnlBackInfo1" runat="server" Width="100%">
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
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
                        <td align="left" style="height: 25px; width: 24%;">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_022" StickHeight="False"></cc1:CustLabel>
                                <asp:TextBox ID="txtRemarkFB" runat="server" Height="60px"  Width="241px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)"></asp:TextBox></td>
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
                        <td align="left" style="height: 25px; width: 24%;">
                            <label>
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_082" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblEndDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></label></td>
                    </tr>
                </table>
               </cc1:CustPanel>
                <cc1:CustPanel ID="pnlBackInfo2" runat="server" Width="100%">
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="trOdd">
                        <td align="left" width="25%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_025" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblbackdate2" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="25%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_026" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblreason2" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td align="left" style="height: 25px; width: 24%;">
                            <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_022" StickHeight="False"></cc1:CustLabel>
                                <asp:TextBox ID="txtRemarkFB2" runat="server" Height="60px"  Width="241px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)"></asp:TextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="left" width="40%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_028" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblm_date2" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="40%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_029" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblpreenditem2" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td align="left" style="height: 25px; width: 24%;">
                            <label>
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_082" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblEndDate2" runat="server" CurSymbol="£"></cc1:CustLabel></label></td>
                    </tr>
                </table>
               </cc1:CustPanel>
                <cc1:CustPanel ID="pnlBackInfo3" runat="server" Width="100%">
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="trOdd">
                        <td align="left" width="25%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_025" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblbackdate3" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="25%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_026" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblreason3" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td align="left" style="height: 25px; width: 24%;">
                            <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_022" StickHeight="False"></cc1:CustLabel>
                                <asp:TextBox ID="txtRemarkFB3" runat="server" Height="60px"  Width="241px" TextMode="MultiLine" onPropertyChange="textCounter(this,200)"></asp:TextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="left" width="40%" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_028" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblm_date3" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td width="40%" align="left" style="height: 25px">
                            <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020101_029" StickHeight="False"></cc1:CustLabel><cc1:CustLabel
                                    ID="lblpreenditem3" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                        <td align="left" style="height: 25px; width: 24%;">
                            <label>
                                <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_082" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblEndDate3" runat="server" CurSymbol="£"></cc1:CustLabel></label></td>
                    </tr>
                </table>
               </cc1:CustPanel>
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
                    DropShadow="False" />
                <asp:Panel ID="PanA" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="683px" Height="51px">
                    <table id="tbContextMailAdd" width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td colspan="4" style="height: 30px">
                                <li id="li7">
                                    <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06020101_066" StickHeight="False"></cc1:CustLabel></li></td>
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
                                    <asp:TextBox ID="txtAdd2Ajax" onPropertyChange="textCounter(this,40)" runat="server" Width="164px"></asp:TextBox></label></td>
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
                                    <asp:TextBox ID="txtAdd3Ajax" onPropertyChange="textCounter(this,40)" runat="server" Width="164px"></asp:TextBox></label></td>
                        </tr>
                        <tr align="left" class="itemTitle">
                            <td colspan="4" align="center">
                                &nbsp;<asp:Button ID="btnSureA" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureA_Click" OnClientClick="return check('A')" />
                                <asp:Button ID="btnCancelA" runat="server" Text="取消" CssClass="smallButton" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                 <!--領卡方式-->
                <asp:ImageButton ID="AddButtonM" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderM" runat="server" TargetControlID="AddButtonM"
                    PopupControlID="PanM" BackgroundCssClass="modal" CancelControlID="btnCancelM"
                    DropShadow="False" />
                <asp:Panel ID="PanM" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="336px" Height="83px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 122%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li id="li3">
                                    <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06020502_000" StickHeight="False"></cc1:CustLabel></li></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 50%; height: 25px;">
                                <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020502_001" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 51%; height: 25px;"><asp:DropDownList ID="dropSelfPickAjax" runat="server" Width="160px">
                            </asp:DropDownList></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020502_002" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 51%;">
                                <cc1:CustLabel ID="lblSelfPickDateAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 50%; height: 25px;">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020502_005" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 51%; height: 25px;">
                                <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="20"></cc1:CustTextBox>&nbsp;<br />
                                <cc1:CustLabel ID="lblNotice" runat="server" CurSymbol="£" Font-Bold="True" Font-Size="12px"
                                    ForeColor="#00C0C0" Width="198px">自取逾期轉限掛時請輸入掛號號碼</cc1:CustLabel></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center" style="height: 25px">
                                <asp:Button ID="btnSureM" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureM_Click" OnClientClick="return check('M')" />
                                <asp:Button ID="btnCancelM" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
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
                                        SetBreak="False" SetOmit="False" ShowID="06_06020101_058" StickHeight="False"></cc1:CustLabel></li></td>
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
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center">
                                <asp:Button ID="btnSureC" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureC_Click" OnClientClick="return check('C')" />
                                <asp:Button ID="btnCancelC" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!--掛號號碼-->
                 <asp:ImageButton ID="AddButtonG" Style="display: none" runat="server" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderG" runat="server" TargetControlID="AddButtonG"
                    PopupControlID="PanG" BackgroundCssClass="modal" CancelControlID="btnCancelG"
                    DropShadow="False" />
                <asp:Panel ID="PanG" CssClass="workingArea" runat="server" Style="display: none;"
                    Width="336px" Height="83px">
                    <table border="0" cellpadding="0" cellspacing="1" style="width: 122%;">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li id="li6">
                                    <cc1:CustLabel ID="CustLabel64" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="06_06020101_062" StickHeight="False"></cc1:CustLabel></li></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel65" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_063" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <cc1:CustLabel ID="lblMailnoAjax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 50%;">
                                <cc1:CustLabel ID="CustLabel67" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020101_064" StickHeight="False"></cc1:CustLabel></td>
                            <td align="left" style="width: 50%;">
                                <cc1:CustTextBox ID="txtMailnoAjax" runat="server" MaxLength="20"></cc1:CustTextBox></td>
                        </tr>
                        <tr align="center" class="itemTitle">
                            <td colspan="2" align="center">
                                <asp:Button ID="btnSureG" runat="server" Text="確定" CssClass="smallButton" OnClick="btnSureG_Click" OnClientClick="return check('G')" />
                                <asp:Button ID="btnCancelG" runat="server" Text="取消" CssClass="smallButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
