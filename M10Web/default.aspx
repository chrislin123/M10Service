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

    <style>

        th.sortasc a  
    {
        display:block; padding:0 4px 0 15px; 
        background:url(images/arrow-asc.png) no-repeat;  
    }
     
    th.sortdesc a 
    {
        display:block; padding:0 4px 0 15px; 
       background:url(images/sort-asc-alt.png) no-repeat;
   }
    </style>

</head>
<body>
    <form id="form1" runat="server">

        <div class="container">

            <h1>雨量查詢</h1>

            <div class="panel panel-info">
                <div class="panel-heading">
                    <h1 class="panel-title"></h1>
                </div>
                <div class="panel-body">
                    <div class="form-inline">
                        <label class="label label-success" style="font-size: 20px">縣市</label>
                        <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCOUNTY_SelectedIndexChanged" ></asp:DropDownList>
                        <asp:Button ID="btnQuery" runat="server" Text="查詢" CssClass="btn btn-success" OnClick="btnQuery_Click"  Visible="false" />

                        <div style="float:right;font-size: 20px"><asp:Label ID="lblDataTime" runat="server" Text="資料時間:"></asp:Label></div>
                        
                    </div>
                    <%--footable--%>
                    <%--table table-hover table-striped--%>
                    <%--CssClass="table table-striped table-bordered table-condensed"--%>
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                         AllowSorting="True" CssClass="table table-striped table-bordered table-condensed"
                        OnRowCreated="GridView1_RowCreated" OnSorting="GridView1_Sorting">
                        <Columns>
                            <asp:BoundField DataField="STNAME" HeaderText="名稱"  />
                            <asp:BoundField DataField="STID" HeaderText="編號" />
                            <asp:BoundField DataField="COUNTY" HeaderText="縣市" />
                            <asp:BoundField DataField="TOWN" HeaderText="鄉鎮" />
                            <asp:BoundField DataField="MIN10" HeaderText="10分鐘" SortExpression="MIN10" />
                            <asp:BoundField DataField="RAIN" HeaderText="1小時" SortExpression="RAIN" />
                            <asp:BoundField DataField="HOUR3" HeaderText="3小時" SortExpression="HOUR3" />
                            <asp:BoundField DataField="HOUR6" HeaderText="6小時" SortExpression="HOUR6"  />
                            <asp:BoundField DataField="HOUR12" HeaderText="12小時" SortExpression="HOUR12" />
                            <asp:BoundField DataField="HOUR24" HeaderText="24小時" SortExpression="HOUR24" />
                            <asp:BoundField DataField="NOW" HeaderText="本日" SortExpression="NOW" />
                            <asp:BoundField DataField="RT" HeaderText="Rt." />
                            <asp:BoundField HeaderText="LRTI" />
                            <asp:BoundField HeaderText="警戒LRTI" />
                            <asp:BoundField DataField="ATTRIBUTE" HeaderText="屬性" />
                        </Columns>
                        <SortedAscendingHeaderStyle CssClass="sortasc" />
                        <SortedDescendingHeaderStyle CssClass="sortdesc" />
                        
                    </asp:GridView>
                </div>
            </div>

            <div class="page-header"></div>


            <%--   <table id="grid">
           <thead>
                <tr>
                    <th>STID</th>
                    <th>STNAME</th>
                </tr>
            </thead>
        </table>--%>
        </div>





        <script type="text/javascript">
            $(function () {
                var qParams = { mode: 'Qry', id: $("#txtid").val(), name: $("#txtname").val() }; //取得查詢參數
                var oldRowIndex;
                var opt = $('#grid');
                opt.datagrid({
                    width: "auto", //自動寬度
                    height: 320,  //固定高度
                    nowrap: false, //不截斷內文
                    striped: true,  //列背景切換
                    fitColumns: true,  //自動適應欄寬
                    singleSelect: true,  //單選列
                    queryParams: qParams,  //參數
                    url: 'CRUDHandler.ashx',  //資料處理頁
                    idField: 'id',  //主索引
                    frozenColumns: [[{ field: 'ck', checkbox: true }]], //顯示核取方塊
                    pageList: [10, 15, 20], //每頁顯示筆數清單
                    pagination: true, //是否啟用分頁
                    rownumbers: true, //是否顯示列數
                    toolbar: [{
                        id: 'btnAdd',
                        text: '新增',
                        iconCls: 'icon-add',
                        handler: function () {
                            insertRow($(this));
                        }
                    }],
                    onClickRow: function (rowIndex) {
                        if (oldRowIndex == rowIndex) {
                            opt.datagrid('clearSelections', oldRowIndex);
                        }
                        var selectRow = opt.datagrid('getSelected');
                        oldRowIndex = opt.datagrid('getRowIndex', selectRow);
                    }
                }).datagrid("getPager").pagination({
                    buttons: [{
                        id: 'btnEdit',
                        iconCls: 'icon-edit',
                        text: '編輯',
                        handler: function () {
                            editRow();
                        }
                    }, '-', {
                        id: 'btnDel',
                        text: '刪除',
                        iconCls: 'icon-remove',
                        handler: function () {
                            removeRow();
                        }
                    }],
                    onBeforeRefresh: function () {
                        return true;
                    }
                });
            });

            //alert("test");

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
