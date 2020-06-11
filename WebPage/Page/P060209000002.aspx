<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060209000002.aspx.cs" Inherits="Page_P060209000002" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>註銷記錄處理>記錄確認</title>
    
 <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />   
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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020901_000" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width:20%;">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020901_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td  style="height: 25px;width:80%;" >
                        &nbsp;<cc1:CustLabel ID="lblDate" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width:20%;">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020901_002" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px; width:80%;">
                        &nbsp;<cc1:CustLabel ID="lblSource" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 20%;">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020901_003" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px; width:80%;">
                            &nbsp;<cc1:CustLabel ID="lblFile" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>                         
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 20%;">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020901_004" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px; width:80%;">
                            <cc1:CustDropDownList ID="dropSFFlg" runat="server">
                            </cc1:CustDropDownList></td>                         
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="2" style="height: 25px">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020901_005" OnClick="btnSearch_Click"/>&nbsp;
                            <cc1:CustButton ID="btnSub" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020901_006" OnClick="btnSub_Click"/>
                            <cc1:CustButton ID="btnPrint" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020901_007" OnClick="btnPrint_Click"/>
                            <cc1:CustButton ID="btnBack" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020901_008" OnClick="btnBack_Click"/></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td>
                                 <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                               PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" 
                                OnRowDataBound="grvUserView_RowDataBound" >
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="CardNo">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BlockLog">
                                        <itemstyle width="15%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MemoLog">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                        <cc1:CustLabel ID="lblSFFlgName" runat="server" CurAlign="left" CurSymbol="&#163;"
                                            FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                            NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"
                                            Text='<%# Bind("SFFlgName") %>'>                                     
                                        </cc1:CustLabel> 
                                        <cc1:CustDropDownList ID="dropSFFlgDetail" runat="server"  Visible="false">
                                        </cc1:CustDropDownList>                
                                 </itemtemplate>
                                        <headerstyle width="20%" />
                                        <itemstyle horizontalalign="Center" width="20%" />
                                    </asp:TemplateField>
                                </Columns>
                            </cc1:CustGridView>
                           
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
                <cc1:CustHiddenField ID="hidSource" runat="server" />
                <cc1:CustHiddenField ID="hidVisible" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel> 
        <asp:UpdateProgress ID="updateProgress1" runat="server">

            <ProgressTemplate>

                <div id="divProgress" align="center" class="progress" style="position: absolute;

                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                    &nbsp;<div id="divProgress2" align="center" class="progress" style="background-color: White;

                        width: 50%; margin: 0px auto;">

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