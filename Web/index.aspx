<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Web.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form method="post" enctype="multipart/form-data" runat="server">
        <input type="file" name="file"/>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
    </form>
</body>
</html>
