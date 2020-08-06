using System;
using Framework.Common.Utility;

public partial class LogonOut : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.RemoveAll();
        Response.Redirect(UtilHelper.GetAppSettings("LOGIN"));
    }
}
