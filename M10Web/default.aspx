<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="M10Web._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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

    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/jquery.easyui.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">





        <div class="container">
            <div class="page-header">
                <h1>查詢</h1>
            </div>

            <div class="form-inline">
                <label class="label label-success" style="font-size: 20px">病歷號　</label>
                <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlCOUNTY_SelectedIndexChanged"></asp:DropDownList>
                <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-success" />
            </div>
        </div>

        <table id="grid">
            <thead>
                <tr>
                    <th>STID</th>
                    <th>STNAME</th>
                </tr>
            </thead>
        </table>

        

        <script  type="text/javascript">

            



            $('#grid').datagrid({
                url: 'CRUDHandler.ashx',  //處理資料面程式   
                columns: [[   //設定欄位
                    { field: 'id', title: 'ID', width: 100 },
                    { field: 'name', title: 'Name', width: 100 },
                    { field: 'age', title: 'Age', width: 100, align: 'right' },
                    { field: 'address', title: 'Address', width: 100 }
                ]]
            });


            function Query() {
                var qid, qname;
                if ($("#txtid").val() != "")
                    qid = $("#txtid").val();
                else
                    qid = "";
                if ($("#txtname").val() != "")
                    qname = $("#txtname").val();
                else
                    qname = "";

                qParams = { id: qid, name: qname };
                $('#grid').datagrid('options').queryParams = qParams;
                $('#grid').datagrid('options').pageNumber = 1;
                var p = $('#grid').datagrid('getPager');
                if (p) {
                    $(p).pagination({ pageNumber: 1 });
                }
                $("#grid").datagrid('reload');
                return false;
            }


        </script>



        <%--        <div class="row">
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
            </div>
            <!-- /.col-sm-4 -->
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
            </div>
            <!-- /.col-sm-4 -->
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
            </div>
            <!-- /.col-sm-4 -->
        </div>


        --%>
    </form>
</body>
</html>
