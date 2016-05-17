<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %><!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sample Site</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <uc1:Ckeditor ID="Content" runat="server" BaseHref="~/ckeditor/" Height="400" />
        <br />
        
        <asp:Button runat="server" Text="Save" OnClick="Save" />
    </div>
    </form>
</body>
</html>
