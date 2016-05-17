<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LinkBrowser.aspx.cs" Inherits="LinkBrowserPage" StylesheetTheme="none" %><!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Link Browser</title>
    <style type="text/css">
        body { margin: 0px; }
        form { width: 500px; background-color: #E3E3C7; }
        h1 { padding: 15px; margin: 0px; padding-bottom: 0px; font-family: Arial; font-size: 14pt; color: #737357; }
        .tab-panel .ajax__tab_body { background-color: #E3E3C7; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Link Browser</h1>

            <table border="1" style="background-color: #F1F1E3; width: 100%; border-spacing: 0px;">
                <tr>
                    <td style="vertical-align: top; padding: 10px;">Folders:<br />
                        <asp:DropDownList ID="DirectoryList" runat="server" Style="width: 160px;" OnSelectedIndexChanged="ChangeDirectory" AutoPostBack="true" />
                        <asp:Button ID="DeleteDirectoryButton" runat="server" Text="Delete" OnClick="DeleteFolder" OnClientClick="return confirm('Are you sure you want to delete this folder and all its contents?');" />
                        <asp:HiddenField ID="NewDirectoryName" runat="server" />
                        <asp:Button ID="NewDirectoryButton" runat="server" Text="New" OnClick="CreateFolder" />
                        <br />
                        <br />

                        <asp:Panel ID="SearchBox" runat="server" DefaultButton="SearchButton">
                            Search:<br />
                            <asp:TextBox ID="SearchTerms" runat="server" />
                            <asp:Button ID="SearchButton" runat="server" Text="Go" OnClick="Search" UseSubmitBehavior="false" />
                            <br />
                        </asp:Panel>
                        <asp:ListBox ID="ImageList" runat="server" Style="width: 300px; height: 220px;" OnSelectedIndexChanged="SelectImage" AutoPostBack="true" />
                        <br />
                        <asp:HiddenField ID="NewImageName" runat="server" />
                        <asp:Button ID="RenameImageButton" runat="server" Text="Rename" OnClick="RenameImage" />
                        <asp:Button ID="DeleteImageButton" runat="server" Text="Delete" OnClick="DeleteImage" OnClientClick="return confirm('Are you sure you want to delete this image?');" />
                        <br />
                        <br />
                        Upload File: (10 MB max)<br />
                        <asp:FileUpload ID="UploadedImageFile" runat="server" />
                        <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="Upload" /><br />
                        <br />
                    </td>
                </tr>
            </table>

            <div style="text-align: center;">
                <asp:Button ID="OkButton" runat="server" Text="Ok" OnClick="Clear" />
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClientClick="window.top.close(); window.top.opener.focus();" OnClick="Clear" />
                <br /><br />
            </div>

        </div>
    </form>
</body>
</html>
