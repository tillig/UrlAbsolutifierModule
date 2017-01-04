<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncludedByHandlerType.aspx.cs" Inherits="TestWebApp.IncludedByHandlerType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Included By Handler Type</title>
</head>
<body>
    <form id="form1" runat="server">
    <h1>Included By Handler Type</h1>
    <p>This page is included via handler type.  The following URL should be absolute:</p>
    <p id="result"></p>
    <img id="image" src="/Images/Image.gif" />
    <script type="text/javascript">
    document.getElementById("result").innerText = document.getElementById("image").outerHTML;
    </script>
    </form>
</body>
</html>
