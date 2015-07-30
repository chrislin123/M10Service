<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RainDataExport.aspx.cs" Inherits="M10Web.RainDataExport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <%--<link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
    <link rel="stylesheet" type="text/css" href="css/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="../demo.css">

    
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>--%>

    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <ul class="nav nav-tabs">
                <li class="active"><a data-toggle="tab" href="#home">Home</a></li>
                <li><a data-toggle="tab" href="#menu1">Menu 1</a></li>
                <li><a data-toggle="tab" href="#menu2">Menu 2</a></li>
            </ul>

            <div class="tab-content">
                <div id="home" class="tab-pane fade in active">
                    <h3>HOME</h3>
                    <p>Some content.</p>
                </div>
                <div id="menu1" class="tab-pane fade">
                    <h3>Menu 1</h3>
                    <p>Some content in menu 1.</p>
                </div>
                <div id="menu2" class="tab-pane fade">
                    <h3>Menu 2</h3>
                    <p>Some content in menu 2.</p>
                </div>
            </div>
        </div>

        <div class="container">

            <h1>查詢</h1>

            <div class="panel panel-info">
                <div class="panel-heading">
                    <h1 class="panel-title"></h1>
                </div>
                <div class="panel-body">

                    <div class="form-inline">
                        <%--<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>--%>
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#home">Home</a></li>
                            <li><a href="#menu1">Menu 1</a></li>
                            <li><a href="#menu2">Menu 2</a></li>
                        </ul>

                        <div class="tab-content">
                            <div id="home" class="tab-pane fade in active">
                                <h3>HOME</h3>
                                <p>Some content.</p>
                            </div>
                            <div id="menu1" class="tab-pane fade">
                                <h3>Menu 1</h3>
                                <p>Some content in menu 1.</p>
                            </div>
                            <div id="menu2" class="tab-pane fade">
                                <h3>Menu 2</h3>
                                <p>Some content in menu 2.</p>
                            </div>
                        </div>

                        <label class="label label-success" style="font-size: 20px">縣市</label>
                        <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                        <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-success" Visible="false" />

                        <div style="float: right; font-size: 20px">
                            <asp:Label ID="lblDataTime" runat="server" Text="資料時間:"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>


        </div>
    </form>
</body>
</html>
