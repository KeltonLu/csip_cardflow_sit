<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060513000001.aspx.cs" Inherits="P060513000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OASA管制解管報表</title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>

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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06051300_000" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="height: 25px; width: 45%">
                            <cc1:CustRadioButton ID="rbCount" runat="server" Text="批次作業量統計表" GroupName="1" AutoPostBack="true"
                                 Checked="true" />
                        </td>
                        <td style="height: 25px; width: 55%">
                            <cc1:CustRadioButton ID="rbResult" runat="server" Text="批次結果報表" GroupName="1" AutoPostBack="true"
                                />
                            <cc1:CustDropDownList ID="ddlStatus" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="height: 25px; width: 45%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="06_06051300_003" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="height: 25px; width: 55%">
                            <cc1:DatePicker ID="txtdateStart" runat="server" Width="74">
                            </cc1:DatePicker>
                            ~
                            <cc1:DatePicker ID="txtdateEnd" runat="server" Width="74">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="06_06051300_006" OnClick="btnSearch_Click" />&nbsp;&nbsp;
								<cc1:CustButton ID="btnPrint" runat="Server" class="smallButton" Style="width: 50px;"
									ShowID="06_06051300_007" OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
				<table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
					<tr>
						<td colspan="20">
							<cc1:CustGridView ID="grvUserView" runat="server" AllowSorting="True" AllowPaging="False"
								PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="16%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="XC">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CS">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="XC+CS">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CG+SB">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CG">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SB">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label9" runat="server" Text="" />
							<br />
							<asp:Label ID="Label1" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView1" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="PERMITDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SYS_DATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label2" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView2" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="PERMITDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SYS_DATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label3" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView3" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="PERMITDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SYS_DATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label4" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView4" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="PERMITDATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SYS_DATE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label5" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView5" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="3%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="FAIL_REASON">
										<ItemStyle Width="5%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="8%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="REASON_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="ACTION_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CWB_REGIONS">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label6" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView6" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="3%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="FAIL_REASON">
										<ItemStyle Width="5%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="8%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="REASON_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="ACTION_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CWB_REGIONS">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label7" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView7" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="3%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="FAIL_REASON">
										<ItemStyle Width="5%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="8%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="REASON_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="ACTION_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CWB_REGIONS">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
							<asp:Label ID="Label8" runat="server" Text="" />
							<cc1:CustGridView ID="CustGridView8" runat="server" AllowSorting="True" AllowPaging="False"
								Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
									<asp:BoundField DataField="ROW_NUM">
										<ItemStyle Width="3%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="OTYPE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SENDDATE">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CARDNO">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="FAIL_REASON">
										<ItemStyle Width="5%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="BLKCODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="MEMO">
										<ItemStyle Width="8%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="REASON_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="ACTION_CODE">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="CWB_REGIONS">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O1">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="O2">
										<ItemStyle Width="10%" HorizontalAlign="Center" />
									</asp:BoundField>
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
    </form>
</body>
</html>
