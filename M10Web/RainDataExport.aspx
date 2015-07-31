<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RainDataExport.aspx.cs" Inherits="M10Web.RainDataExport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <%--<link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">--%>

    <%----%><link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
    <link rel="stylesheet" type="text/css" href="css/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="../demo.css">


    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/easyui-lang-zh_TW.js"></script>

    <%--    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="container">
                    <h1>查詢</h1>
                    <div class="panel panel-info">
                        <div class="panel-heading">
                            <h1 class="panel-title"></h1>
                        </div>
                    </div>
                    <%--<ul class="nav nav-tabs">--%>
                    <ul class="nav nav-pills">
                        <li class="active"><a data-toggle="tab" href="#home">縣市雨量站 逐時雨量下載</a></li>
                        <li><a data-toggle="tab" href="#menu1">單站雨量站 逐時雨量下載</a></li>
                        <li><a data-toggle="tab" href="#menu2">各雨量站累積雨量SharpFile下載</a></li>
                    </ul>

                    <div class="tab-content">

                        <div id="home" class="tab-pane fade in active">
                            <br />
                            <div class="form-inline">

                                <label class="label label-success" style="font-size: 20px">縣市：</label>
                                <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <br />
                            <asp:Label ID="lblDataTime" runat="server" Text="開始日期："></asp:Label>
                            <input id="sDate" type="text" class="easyui-datebox">
                            <asp:Label ID="Label1" runat="server" Text="結束日期："></asp:Label>
                            <input id="eDate" type="text" class="easyui-datebox">
                            <br />
                            

                        </div>
                        <div id="menu1" class="tab-pane fade">
                            <br />
                            <div class="form-inline">

                                <label class="label label-success" style="font-size: 20px">縣市：</label>
                                <asp:DropDownList ID="ddlCOUNTY2" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCOUNTY2_SelectedIndexChanged"></asp:DropDownList>
                                <label class="label label-success" style="font-size: 20px">雨量站：</label>
                                <asp:DropDownList ID="ddlRainStation" runat="server" CssClass="form-control"></asp:DropDownList>

                            </div>
                            <br />

                            <asp:Label ID="Label2" runat="server" Text="開始日期："></asp:Label>
                            <input id="sDate" type="text" class="easyui-datebox">
                            <asp:Label ID="Label3" runat="server" Text="結束日期："></asp:Label>
                            <input id="eDate" type="text" class="easyui-datebox">
                            <br />
                            <asp:Button ID="Button1" runat="server" Text="資料匯出" CssClass="btn btn-success" />
                        </div>
                        <div id="menu2" class="tab-pane fade">
                            <br />
                            <asp:Label ID="Label4" runat="server" Text="開始日期："></asp:Label>
                            <input id="sDate" type="text" class="easyui-datebox">
                            <asp:Label ID="Label5" runat="server" Text="結束日期："></asp:Label>
                            <input id="eDate" type="text" class="easyui-datebox">
                            <br />
                            <asp:Button ID="Button2" runat="server" Text="資料匯出" CssClass="btn btn-success" />
                        </div>

                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:Button ID="btnExport" runat="server" Text="資料匯出" CssClass="btn btn-success" OnClick="btnExport_Click" />
    </form>
    <script type="text/javascript">
        $('#dd').datebox({
            //required: true
        });



    </script>
</body>
</html>
