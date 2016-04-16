<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rti3Data.aspx.cs" Inherits="M10Web.Rti3Data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
    <link rel="stylesheet" type="text/css" href="css/themes/icon.css">

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
            <h1>崩塌警戒基準值(RTI3)</h1>
            <div class="panel panel-info">
                <div class="panel-heading">
                    <%--<h1 class="panel-title"></h1>--%>
                    <table style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td style="width: 20%; text-align: right;">
                                        
                                        <asp:Button ID="btnBack" runat="server" Text="返回雨量查詢" CssClass="btn btn-success" OnClick="btnBack_Click" /></td>
                                </tr>
                            </table>
                </div>
                <div class="panel-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <div class="form-inline">
                                <%--<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>--%>

                                <table>
                                    <tr>
                                        <td>
                                            <label class="label label-success" style="font-size: 20px">降雨延時(F)</label></td>
                                        <td>
                                            <asp:RadioButtonList ID="rDelaytime" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rDelaytime_SelectedIndexChanged" >
                                                <asp:ListItem>0</asp:ListItem>
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                            </asp:RadioButtonList></td>
                                        <td>
                                            <%--<asp:Label ID="lblDataTime" runat="server" Text="資料時間:"></asp:Label>--%></td>
                                    </tr>
                                </table>



                                <%--<asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCOUNTY_SelectedIndexChanged"></asp:DropDownList>--%>
                                <div style="display: none;">
                                    <%--<asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-success" OnClick="btnQuery_Click"  />--%>
                                </div>

                             <%--   <div style="float: right; font-size: 20px">
                                </div>--%>



                            </div>
                            <%--footable--%>
                            <%--table table-hover table-striped--%>
                            <%--CssClass="table table-striped table-bordered table-condensed"--%>

                            <%----%>
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                AllowSorting="True" CssClass="table table-striped table-bordered table-condensed"
                                OnRowCreated="GridView1_RowCreated" OnSorting="GridView1_Sorting" OnRowDataBound="GridView1_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="station" HeaderText="站號" SortExpression="station" />
                                    <asp:BoundField DataField="totalcount" HeaderText="筆數" SortExpression="totalcount" />
                                    <asp:BoundField DataField="startdate" HeaderText="開始時間" SortExpression="startdate" />
                                    <asp:BoundField DataField="enddate" HeaderText="最終時間" SortExpression="enddate" />
                                    <asp:BoundField DataField="rti10" HeaderText="RTI10" SortExpression="rti10" />
                                    <asp:BoundField DataField="rti30" HeaderText="RTI30" SortExpression="rti30" />
                                    <asp:BoundField DataField="rti50" HeaderText="RTI50" SortExpression="rti50" />
                                    <asp:BoundField DataField="rti70" HeaderText="RTI70" SortExpression="rti70" />
                                    <asp:BoundField DataField="rti90" HeaderText="RTI90" SortExpression="rti90" />                                    
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
