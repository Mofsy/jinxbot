using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Channel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        const string jsBase = @"
<script type=""text/javascript"">
<!--
    var channelID = {1}{0}{1};
// -->
</script>";

        if (Request.PathInfo != null)
        {
            string name = Request.PathInfo.Substring(1).Replace('_', ' ');
            var dataContext = Services.DataConnection;
            var channel = (from c in dataContext.Channels
                           where c.ClientName == name
                           select c).FirstOrDefault();
            if (channel != null)
            {
                this.initializationScript.Text = string.Format(jsBase, channel.ChannelID, '"');
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, Request.QueryString.ToString(), true);
            }
            else
            {
                this.initializationScript.Text = string.Format(jsBase, "false", string.Empty);
            }
        }
        else
        {
            this.initializationScript.Text = string.Format(jsBase, "false", string.Empty);
        }
    }
}
