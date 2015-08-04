﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="M10Web._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <%-- <meta http-equiv="refresh" content="3">--%>
    <title></title>

    <%--    <!-- Latest compiled and minified CSS -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">

<!-- Optional theme -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap-theme.min.css">

<!-- Latest compiled and minified JavaScript -->
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>--%>

    <link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
    <link rel="stylesheet" type="text/css" href="css/themes/icon.css">
    <link rel="stylesheet" type="text/css" href="../demo.css">
    
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>

    <style>
        th.sortasc a {
            display: block;
            padding: 0 4px 0 15px;
            background: url(images/arrow-asc.png) no-repeat;
        }

        th.sortdesc a {
            display: block;
            padding: 0 4px 0 15px;
            background: url(images/sort-asc-alt.png) no-repeat;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container">
            <h1>雨量查詢</h1>
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h1 class="panel-title"></h1>
                </div>
                <div class="panel-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            
                            <div class="form-inline">
                                <%--<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>--%>

                                <label class="label label-success" style="font-size: 20px">縣市</label>
                                <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCOUNTY_SelectedIndexChanged"></asp:DropDownList>
                                <div style="display:none;">
                                <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-success" OnClick="btnQuery_Click"  />
                                </div>
                                <asp:Button ID="btnExport" runat="server" Text="資料匯出" CssClass="btn btn-success" OnClick="btnExport_Click" />

                                <div style="float: right; font-size: 20px">
                                    <asp:Label ID="lblDataTime" runat="server" Text="資料時間:"></asp:Label>
                                </div>

                            </div>
                            <%--footable--%>
                            <%--table table-hover table-striped--%>
                            <%--CssClass="table table-striped table-bordered table-condensed"--%>

                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                AllowSorting="True" CssClass="table table-striped table-bordered table-condensed"
                                OnRowCreated="GridView1_RowCreated" OnSorting="GridView1_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="STNAME" HeaderText="名稱" />
                                    <asp:BoundField DataField="STID" HeaderText="編號" />
                                    <asp:BoundField DataField="COUNTY" HeaderText="縣市" SortExpression="COUNTY" />
                                    <asp:BoundField DataField="TOWN" HeaderText="鄉鎮" SortExpression="TOWN" />
                                    <asp:BoundField DataField="MIN10" HeaderText="10分鐘" SortExpression="MIN10" />
                                    <asp:BoundField DataField="RAIN" HeaderText="1小時" SortExpression="RAIN" />
                                    <asp:BoundField DataField="HOUR3" HeaderText="3小時" SortExpression="HOUR3" />
                                    <asp:BoundField DataField="HOUR6" HeaderText="6小時" SortExpression="HOUR6" />
                                    <asp:BoundField DataField="HOUR12" HeaderText="12小時" SortExpression="HOUR12" />
                                    <asp:BoundField DataField="HOUR24" HeaderText="24小時" SortExpression="HOUR24" />
                                    <asp:BoundField DataField="NOW" HeaderText="本日" SortExpression="NOW" />
                                    <asp:BoundField DataField="RT" HeaderText="Rt." SortExpression="RT" />
                                    <asp:BoundField DataField="LRTI" HeaderText="LRTI" SortExpression="LRTI" />
                                    <asp:BoundField HeaderText="警戒LRTI" />
                                    <asp:BoundField DataField="ATTRIBUTE" HeaderText="屬性" />
                                </Columns>
                                <SortedAscendingHeaderStyle CssClass="sortasc" />
                                <SortedDescendingHeaderStyle CssClass="sortdesc" />
                            </asp:GridView>
                            <%--<asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>--%>
                    </ContentTemplate>
                        
                    </asp:UpdatePanel>    
                </div>
            </div>
            <div class="page-header"></div>
        </div>

        <script type="text/javascript">

            function myrefresh() {
                //window.location.reload();
                $("#btnQuery").click();
            }            

            $(function () {
                setInterval(myrefresh, 60000); //指定10秒刷新一次
            });

        </script>
    </form>
</body>
</html>
