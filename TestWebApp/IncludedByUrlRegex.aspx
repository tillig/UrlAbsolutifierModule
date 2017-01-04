<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncludedByUrlRegex.aspx.cs" Inherits="TestWebApp.IncludedByUrlRegex" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Included By Url Regex</title>
</head>
<body>
    <form id="form1" runat="server">
    <h1>Included By Url Regex</h1>
    <p>This page is included via URL regex.  The following URL should be absolute:</p>
    <p id="result"></p>
    <img id="image" src="/Images/Image.gif" />
    <script type="text/javascript">
    document.getElementById("result").innerText = document.getElementById("image").outerHTML;
    </script>
    </form>
</body>
</html>
