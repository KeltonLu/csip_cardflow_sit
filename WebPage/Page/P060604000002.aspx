<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P060604000002.aspx.cs" Inherits="Page_P060604000002" %>

<%-- <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" --%>
<%--     Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %> --%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
<%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr>
                        <td>
                            <%-- <rsweb:ReportViewer ID="ReportViewer0604" runat="server" Height="550px" Width="100%" --%>
                            <%--     Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" ShowParameterPrompts="False" --%>
                            <%--     SizeToReportContent="True" ShowDocumentMapButton="False" ShowExportControls="True" --%>
                            <%--     ShowFindControls="False" ShowPageNavigationControls="False" ShowPrintButton="True" --%>
                            <%--     ShowPromptAreaButton="False" ShowRefreshButton="False" ShowZoomControl="False"> --%>
                            <%--     <ServerReport ReportServerUrl="" /> --%>
                            <%-- </rsweb:ReportViewer> --%>
                        </td>
                    </tr>
                </table>
<%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
    </form>
</body>
</html>
