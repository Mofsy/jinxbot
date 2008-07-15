<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClientClick="callService(); return false;" />
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/JinxBotWebClient.svc" InlineScript="false" />
            </Services>
        </asp:ScriptManager>
    </div>
    </form>
<script type="text/javascript">
<!--
function callService()
{
    var client = new JinxBotWebClient();
    client.GetArgs(serviceSuccess, serviceFailure);
}

function serviceSuccess(result)
{
    debugger;
    alert(result);
    
}

function serviceFailure(error)
{
    alert("Error!  " + error);
}
//-->
</script>
</body>
</html>
