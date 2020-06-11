<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060205000001.aspx.cs" Inherits="P060205000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>自取庫存日結</title>
    
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
          var rad4 =  document.getElementById("rad020504");         
          if (rad1.checked || rad2.checked)
          {
            document.getElementById("btnPrint").style.display="";
            document.getElementById("btnPrint").style.display="";

          }
          if (rad3.checked || rad4.checked)
          {
            document.getElementById("btnPrint").style="none";

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
                        <td colspan="5">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06020500_000" StickHeight="False"></cc1:CustLabel></li></td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 1%; height: 25px;">
                            <cc1:CustRadioButton ID="rad020501" runat="server" GroupName="rad0205"/></td>
                        <td align="right" style="height: 25px; width:15%;">
                            <cc1:CustLabel ID="CustLabel1" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06020500_001" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td  style="height: 25px"><cc1:DatePicker ID="dpMerchDate"  runat="server" Width="74" >
                        </cc1:DatePicker>
                        </td>
                       <td align="right" style="height: 25px; width: 15%">
                            <cc1:CustLabel ID="CustLabel6" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="06_02030001_026" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="height: 25px; width: 35%">
                            &nbsp;<cc1:CustDropDownList ID="ddlFactory" runat="server">
                            </cc1:CustDropDownList></td>
                    </tr>
                    <tr class="trEven">
                        <td style="width: 1%;height: 25px;">
                            <cc1:CustRadioButton ID="rad020502" runat="server" GroupName="rad0205"/></td>
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_002" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="height: 25px" colspan="3"><cc1:DatePicker ID="dpFetchDate"  runat="server" Width="74" >
                        </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 1%; height: 25px;">
                            <cc1:CustRadioButton ID="rad020503" runat="server" GroupName="rad0205"/></td>
                                                
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td  style="height: 25px; width: 10%;">
                            <cc1:CustTextBox ID="txtID" MaxLength="12" runat="server" InputType="String" Width="102px"></cc1:CustTextBox>
                        </td>
                        <td  align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_004" StickHeight="False"></cc1:CustLabel>    
                        </td>
                        <td style="height: 25px;width: 15%;">
                            <cc1:CustTextBox ID="txtCardNo" MaxLength="16" runat="server" InputType="String" Width="134px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    
                    <tr class="trEven">
                        <td style="width: 1%; height: 25px;"></td>
                         <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td  style="height: 25px; width: 18%;" colspan="3">
                            <cc1:DatePicker ID="dpFrom"  runat="server" Width="74" ></cc1:DatePicker> ~ 
                            <cc1:DatePicker ID="dpTo"  runat="server" Width="74" ></cc1:DatePicker>
                        </td>   
                    </tr>
                    
                    <tr class="trOdd">
                        <td style="width: 1%; height: 25px;">
                            <cc1:CustRadioButton ID="rad020504" runat="server" GroupName="rad0205"/></td>
                             
                        <td align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_016" StickHeight="False"></cc1:CustLabel>
                        </td>                                                                         
                        <td align="left" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_003" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustTextBox ID="txtID2" MaxLength="12" runat="server" InputType="String" Width="102px"></cc1:CustTextBox>
                        </td>
                        <td  align="right" style="height: 25px; width: 15%;">
                            <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06020500_004" StickHeight="False"></cc1:CustLabel>    
                        </td>
                        <td style="height: 25px;width: 15%;">
                            <cc1:CustTextBox ID="txtCardNo2" MaxLength="16" runat="server" InputType="String" Width="134px"></cc1:CustTextBox>
                        </td>
                    </tr>                    
                    
                    <tr align="center" class="itemTitle">
                        <td colspan="5">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020500_006" OnClick="btnSearch_Click" />&nbsp;
                            <cc1:CustButton ID="btnInOutStore" runat="server" class="smallButton" Style="width: 80px;"
                                ShowID="06_06020500_007" OnClick="btnInOutStore_Click"/>
                            <cc1:CustButton ID="btnPrint" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06020500_008" OnClick="btnPrint_Click"/>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
                    <tr>
                        <td colspan="20">
                                 <cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
                               PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0"
                                CellSpacing="1" BorderStyle="Solid" OnRowEditing="grvUserView_RowEditing" DataKeyNames="cardno"
                                >
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="SerialNo">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="custname">
                                        <itemstyle width="15%" horizontalalign="Center" wrap="True" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="id">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="30%" />
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lkbDetail" runat="server" CommandName="Edit" Text='<%# Bind("cardno") %>'　></cc1:CustLinkButton> 
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="indate1">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IntoStore_Date">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="5%" />
                                        <itemtemplate>
                                         <input id="chkIntoStoreFlg" type="checkbox" runat="server"  class="ChoiceButton"/> 
                                        </itemtemplate>
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
            </ContentTemplate>
        </asp:UpdatePanel> 
                <script language="javascript" type="text/javascript">
					window.addEventListener("scroll", scroll, false);

              function   scroll()   

              {   

                $("#divProgress").css("top",290+document.documentElement.scrollTop);

              }    

        </script> 

        <asp:UpdateProgress ID="updateProgress1" runat="server">

            <ProgressTemplate>

                <div id="divProgress" align="center" class="progress" style="position: absolute;

                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">

                    <div id="divProgress2" align="center" class="progress" style="background-color: White;

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
