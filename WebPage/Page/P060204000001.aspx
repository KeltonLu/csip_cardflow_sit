<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060204000001.aspx.cs" Inherits="Page_P060204000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>人工註銷</title>
    
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
     <script type="text/javascript">
          function DisplayButton()
      {
          var rad1 =  document.getElementById("rad020501");
          var rad2 =  document.getElementById("rad020502");
          var rad3 =  document.getElementById("rad020503");
          if (rad1.checked || rad2.checked)
          {
            document.getElementById("btnPrint").style.display="";
            document.getElementById("btnPrint").style.display="";

          }
          if (rad3.checked)
          {
            document.getElementById("btnPrint").style="none";

          }         
      
      }
    </script>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnDoOASA"/>
            </Triggers>
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="9">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020400_000" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" colspan="2" style="width: 16%; height: 25px">
                            <cc1:CustRadioButton ID="rad020401" runat="server" GroupName="rad0204" Checked="true" AutoPostBack="true" OnCheckedChanged="rad020401_CheckedChanged"/><cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020400_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td  style="height: 25px; width: 84%;">
                            <cc1:CustTextBox ID="txtCardNoList" runat="server" TextMode="MultiLine" InputType="Memo" MaxLength="300" Width="149px" Height="89px" Enabled="false"></cc1:CustTextBox>
                            <cc1:CustLabel ID="lblNotice" runat="server" CurAlign="left" CurSymbol="&#163;"
                                Font-Bold="True" Font-Size="12px" ForeColor="#00C0C0" FractionalDigit="2" IsColon="False"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" ShowID="" StickHeight="False" Width="127px">卡號請用Enter鍵分隔</cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" colspan="2" style="width: 16%; height: 25px">
                            <cc1:CustRadioButton ID="rad020402" runat="server" GroupName="rad0204" AutoPostBack="true" OnCheckedChanged="rad020402_CheckedChanged"/><cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020400_002" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px; width: 84%;">
                            <asp:FileUpload ID="fulFilePath" runat="server" Enabled="false"/></td>
                    </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="trOdd">
                    <td style="width: 16%" align="right">
                        <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                        SetOmit="False" StickHeight="False" ShowID="06_06020400_003" CurAlign="left"
                        CurSymbol="£"></cc1:CustLabel>
                    </td>
                    <td style="width: 16%">
                        <cc1:CustDropDownList ID="dropBlkCode" runat="server">
                        </cc1:CustDropDownList></td>
                    <td style="width: 16%" align="right">
                        <cc1:CustLabel ID="CustLabel4" runat="server" FractionalDigit="2" IsColon="True"
                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                        SetOmit="False" StickHeight="False" ShowID="06_06020400_004" CurAlign="left"
                        CurSymbol="£"></cc1:CustLabel>
                    </td>
                    <td style="width: 10%">
                        <cc1:CustDropDownList ID="dropMemo" runat="server">
                        </cc1:CustDropDownList>
                    </td>
                    <td style="width: 10%">
                        <cc1:CustTextBox ID="txtMemo" runat="server" Width="77px" MaxLength="20"></cc1:CustTextBox></td>
                    <td style="width: 16%">
                    </td>
                    <td style="width: 16%">
                    </td>
                    </tr>
                    <tr class="trEven">
                    <td style="width: 16%" align="right">
                        <cc1:CustLabel ID="CustLabel5" runat="server" FractionalDigit="2" IsColon="True"
                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                        SetOmit="False" StickHeight="False" ShowID="06_06020400_005" CurAlign="left"
                        CurSymbol="£"></cc1:CustLabel>
                    </td>
                    <td style="width: 16%">
                        <cc1:CustTextBox ID="txtReasonCode" runat="server" MaxLength="2" Width="60px"></cc1:CustTextBox></td>
                    <td style="width: 16%" align="right">
                        <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True"
                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                        SetOmit="False" StickHeight="False" ShowID="06_06020400_006" CurAlign="left"
                        CurSymbol="£"></cc1:CustLabel>
                    </td>
                    <td style="width: 20%" colspan="2">
                        <cc1:CustTextBox ID="txtActionCode" runat="server" MaxLength="2" Width="60px"></cc1:CustTextBox></td>
                    <td style="width: 16%" align="right">
                        <cc1:CustLabel ID="CustLabel7" runat="server" FractionalDigit="2" IsColon="True"
                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                        SetOmit="False" StickHeight="False" ShowID="06_06020400_007" CurAlign="left"
                        CurSymbol="£"></cc1:CustLabel>
                    </td>
                    <td style="width: 16%">
                        <cc1:CustTextBox ID="txtCWB" runat="server" MaxLength="1" Width="60px"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="9" style="height: 25px">
                            <cc1:CustButton ID="btnDoOASA" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020400_008" OnClick="btnDoOASA_Click" />&nbsp;
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>             
    </form>
</body>
</html>
