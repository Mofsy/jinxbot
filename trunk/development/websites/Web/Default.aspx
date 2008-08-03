<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>JinxBot[Web] - Channel Display</title>
<style type="text/css">
body
{
    background-color: black;
    font-size: 12px;
    color: #dddddd;
    font-family: Tahoma, Verdana, Sans-serif;

}

p
{
    text-indent: -3em;
    margin-left: 3em;
    margin-top: 4px;
    margin-bottom: 0px;
}

#scrollTo
{
    height: 4px;
}
</style>
<link rel="Stylesheet" type="text/css" href="DefaultStyle.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/JinxBotWebClient.svc" InlineScript="false" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="~/BattleNetClientScript.js" />
                <asp:ScriptReference Path="~/ChatDocumentBehaviors.js" />
            </Scripts>
        </asp:ScriptManager>
    </div>
    
    <div id="enterText"></div>
    <div id="scrollTo">&nbsp;</div>
    </form>
<script type="text/javascript">
<!--
var chatEvents = [];
var chatWindow = false;
var mostRecentEvent = 0;
var channelID = "539be340-302f-442c-ae1c-f9db9b40fc75";


//-->
</script>
</body>
</html>
