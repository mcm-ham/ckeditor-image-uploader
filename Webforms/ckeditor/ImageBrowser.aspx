<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageBrowser.aspx.cs" Inherits="ImageBrowserPage" StylesheetTheme="none" Theme="none" %><!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Image Browser</title>
	<style type="text/css">
		body { margin: 0px; }
		form { width:750px; background-color: #E3E3C7; }
		h1 { padding: 15px; margin: 0px; padding-bottom: 0px; font-family:Arial; font-size: 14pt; color: #737357; }
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<h1>Image Browser</h1>
        
		<table border="1" style="background-color:#F1F1E3; margin:15px; width: 720px; border-spacing: 0px;">
			<tr>
				<td style="width: 396px; vertical-align: middle; text-align: center; padding: 10px;">
					<asp:Image ID="Image1" runat="server" Style="max-height: 450px; max-width: 380px;" />
				</td>
				<td style="width: 324px; vertical-align: top; padding: 10px;">
					Folders:<br />
					<asp:DropDownList ID="DirectoryList" runat="server" Style="width: 160px;" OnSelectedIndexChanged="ChangeDirectory" AutoPostBack="true" />
					<asp:Button ID="DeleteDirectoryButton" runat="server" Text="Delete" OnClick="DeleteFolder" OnClientClick="return confirm('Are you sure you want to delete this folder and all its contents?');" />
					<asp:HiddenField ID="NewDirectoryName" runat="server" />
					<asp:Button ID="NewDirectoryButton" runat="server" Text="New" OnClick="CreateFolder" />
					<br /><br />
							
					<asp:Panel ID="SearchBox" runat="server" DefaultButton="SearchButton">
						Search:<br />
						<asp:TextBox ID="SearchTerms" runat="server"/>
						<asp:Button ID="SearchButton" runat="server" Text="Go" OnClick="Search" UseSubmitBehavior="false" />
						<br />
					</asp:Panel>
					<asp:ListBox ID="ImageList" runat="server" Style="width: 280px; height: 180px;" OnSelectedIndexChanged="SelectImage" AutoPostBack="true" />

					<asp:HiddenField ID="NewImageName" runat="server" />
					<asp:Button ID="RenameImageButton" runat="server" Text="Rename" OnClick="RenameImage" />
					<asp:Button ID="DeleteImageButton" runat="server" Text="Delete" OnClick="DeleteImage" OnClientClick="return confirm('Are you sure you want to delete this image?');" />
					<br />
					<br />
					Resize:<br />
					<asp:TextBox ID="ResizeWidth" runat="server" Width="50" OnTextChanged="ResizeWidthChanged" />
					x
					<asp:TextBox ID="ResizeHeight" runat="server" Width="50" OnTextChanged="ResizeHeightChanged" />
					<asp:HiddenField ID="ImageAspectRatio" runat="server" />
					<asp:Button ID="ResizeImageButton" runat="server" Text="Resize Image" OnClick="ResizeImage" /><br />
					<asp:Label ID="ResizeMessage" runat="server" ForeColor="Red" />
					<br /><br />
					Upload Image: (10 MB max)
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
