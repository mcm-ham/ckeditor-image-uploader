using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

public partial class _default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Content.Value = "<h1>Test</h1>Content.";
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        string html = Content.Value;
    }
}
