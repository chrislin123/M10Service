<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="M10Web._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

    <!-- Latest compiled and minified CSS -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">

<!-- Optional theme -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap-theme.min.css">

<!-- Latest compiled and minified JavaScript -->
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>

    <%--<link rel="stylesheet" type="text/css" href="css/css/bootstrap.css">
    <link href="css/css/bootstrap-theme.min.css" rel="stylesheet">--%>
    <link rel="stylesheet" type="text/css" href="css/themes/default/easyui.css">
	<link rel="stylesheet" type="text/css" href="css/themes/icon.css">
	<link rel="stylesheet" type="text/css" href="../demo.css">

    <%--<script type="text/javascript"  src="js/bootstrap.min.js"></script>--%>
	<script type="text/javascript"  src="js/jquery.min.js"></script>
	<script type="text/javascript" src="js/jquery.easyui.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <h2>雨量查詢</h2>        <table>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCOUNTY" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCOUNTY_SelectedIndexChanged"></asp:DropDownList>
                    <asp:DropDownList ID="ddlStation" runat="server"></asp:DropDownList>
                    <asp:Button ID="btnQuery" runat="server" Text="查詢" />
                </td>
            </tr>
            <tr>
                <td></td>
            </tr>
        </table>        <div class="row">
        <div class="col-sm-4">
          <div class="panel panel-default">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
          <div class="panel panel-primary">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
        </div><!-- /.col-sm-4 -->
        <div class="col-sm-4">
          <div class="panel panel-success">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
          <div class="panel panel-info">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
        </div><!-- /.col-sm-4 -->
        <div class="col-sm-4">
          <div class="panel panel-warning">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
          <div class="panel panel-danger">
            <div class="panel-heading">
              <h3 class="panel-title">Panel title</h3>
            </div>
            <div class="panel-body">
              Panel content
            </div>
          </div>
        </div><!-- /.col-sm-4 -->
      </div>
        
        
        
              




    </form>
</body>
</html>
