<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RtiControl.aspx.cs" Inherits="M10Web.RtiControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
    <link rel="stylesheet" type="text/css" href="css/themes/icon.css">

    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container">
            <h1>崩塌警戒發報設定</h1>
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
                                <table>
                                    <tr>
                                        <td>
                                            <label class="label label-success" style="font-size: 20px">降雨延時(F)</label></td>
                                        <td>
                                           
                                            <asp:RadioButtonList ID="rRtiControl" style="font-size: 20px" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem>啟動</asp:ListItem>
                                                
                                                <asp:ListItem>關閉</asp:ListItem>
                                            </asp:RadioButtonList></td>

                                        
                                        <td>
                                            &nbsp;&nbsp;
                                            <asp:Button ID="btnsave" runat="server" Text="儲存" style="font-size: 16px" CssClass="btn btn-success" OnClick="btnsave_Click" /></td>
                                        <td>
                                            &nbsp;&nbsp;
                                            <label style="font-size: 20px">密碼：</label>
                                            &nbsp;&nbsp;
                                            <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>

                                </table>
                                <div style="display: none;">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <script type="text/javascript">

            function myrefresh() {
                //window.location.reload();
                $("#btnQuery").click();
            }

            $(function () {
                //setInterval(myrefresh, 60000); //指定10秒刷新一次
            });

        </script>
    </form>
</body>
</html>
