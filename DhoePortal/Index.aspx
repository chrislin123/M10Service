<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="DhoePortal.Index" %>

<!DOCTYPE html >

<%--<html xmlns="http://www.w3.org/1999/xhtml" class="full" >--%>
<html xmlns="http://www.w3.org/1999/xhtml"  >
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="css/the-big-picture.css" rel="stylesheet">
    

    <style type='text/css'>
        .container_div {
            background-color: #CCC;
            width: 200px;
            height: 200px;
        }

        .empty_div {
            float: left;
            height: 50%; /*父元素高度的50%*/
            margin-bottom: -65px; /*置中元素高度的一半*/
            *width:: 1px; /*only for IE7*/
        }

        .content_div {
            clear: both; /*清除浮動*/
            height: 130px;
            position: relative;
        }
    </style>
</head>
    
<body >
    <a href="Professor.aspx">
    <div style="width:100%;padding:0px 0px 0px 0px;" > 

        <img src="images/index.jpg" class="img-rounded" alt="Cinque Terre" width="100%" >



    </div></a>
    
        <%--<img src="images/index.jpg" alt="Smiley face" height="768" width="1024" style="display: block; margin: auto;">--%>
    

    <%--<div style="text-align: center; background-color: #FFAC55; width: 500px; height: 50px;">
        test
    </div>--%>
</body>
        
</html>
