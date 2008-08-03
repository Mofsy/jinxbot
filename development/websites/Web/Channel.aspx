<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Channel.aspx.cs" Inherits="Channel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="mgr" runat="server">
        <Services>
            <asp:ServiceReference InlineScript="false" Path="~/JinxBotWebClient.svc" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/BattleNetClientScript.js" />
            <asp:ScriptReference Path="~/ChatDocumentBehaviors.js" />
            <asp:ScriptReference Path="~/Sizer.js" />
        </Scripts>
    </asp:ScriptManager>
    <div>
    
    </div>
    <script type="text/javascript">
    <![CDATA[
Sys.Application.add_load(OnApplicationReady);

function OnApplicationReady()
{
    chatWindow = new JinxBotWeb.ChatDisplay(document.getElementById('enterText'), document.getElementById('scrollTo'));
    
    startServicePolling();
}
    // ]]>
    </script>
    </form>
</body>
</html>
