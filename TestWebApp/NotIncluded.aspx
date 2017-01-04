<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotIncluded.aspx.cs" Inherits="TestWebApp.NotIncluded" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Not Included</title>
</head>
<body>
    <form id="form1" runat="server">
    <h1>Not Included</h1>
    <p>This page is not included by the rules.  The following URL should be rooted, but not fully qualified:</p>
    <p id="result"></p>
    <img id="image" src="/Images/Image.gif" />
    <script type="text/javascript">
    document.getElementById("result").innerText = document.getElementById("image").outerHTML;
    </script>
    </form>
</body>
</html>
