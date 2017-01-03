<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="DhoePortal.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>


    <%--<style type='text/css'>
        .container {
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
    </style>--%>

    <style type='text/css'>
.container_div {
    background-color:#CCC;
    width:200px;
    height:200px;
    }
 
.empty_div {
    float:left;
    height:50%; /*父元素高度的50%*/
    margin-bottom:-65px;/*置中元素高度的一半*/
    *width::1px; /*only for IE7*/
    }
.content_div {
    clear:both; /*清除浮動*/
    height:130px;
    position:relative
    }
 
</style>
</head>
<body>

    <a href="default.aspx">
        <img src="images/index.jpg" alt="Smiley face" height="768" width="1024" style="display: block; margin: auto;">
    </a>

    <div class="container_div">
    <div class="empty_div">
    </div>
    <div class="content_div">
    <img src="http://www.ez2o.com/LIB/ezThumbnail/images/164/1.jpg" />
    </div>
</div>
<div class="container_div">
    <div class="empty_div">
    </div>
    <div class="content_div">
    這行應該要居中
    </div>
</div>
    <div class="content_div">


        <div id="footer" class="container">
            <%--<div class="qrcode">
            <img src="/Content/images/internetReg.jpg" alt="" width="65px" height="65px" /></div>--%>

            <p>
                預約掛號電話:(04)22013333 轉分機 9 &nbsp;&nbsp; 門診時間:週一至週五&nbsp;&nbsp; 院址:台中市北區五權路482號<br>
                Copyright (c) 2014 勝美醫院. All rights reserved. 網站管理:資訊課
            </p>
        </div>
    </div>
    <%--<div style="text-align: center; background-color: #FFAC55; width: 500px; height: 50px;">
        test
    </div>--%>
</body>
</html>
