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
    <div class="container">
      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
          <script type="text/javascript" language="javascript">
            //Sys.Application.add_load(jScript);
          </script>

          <h1>資料匯出</h1>
          <div class="panel panel-info">
            <div class="panel-heading">

              <table style="width: 100%;">
                <tr>
                  <td></td>
                  <td style="width: 20%; text-align: right;">

                    <asp:Button ID="btnBack" runat="server" Text="返回雨量查詢" CssClass="btn btn-success" OnClick="btnBack_Click" /></td>
                </tr>
              </table>


            </div>
          </div>
          <%--<ul class="nav nav-tabs">--%>


          <div class="tab-content">
            <ul class="nav nav-pills">
              <li class="active"><a data-toggle="tab" href="#home">縣市雨量站 逐時雨量下載</a></li>
              <li><a data-toggle="tab" href="#menu1">單站雨量站 逐時雨量下載</a></li>
              <li><a data-toggle="tab" href="#menu2">各雨量站累積雨量座標下載</a></li>
              <li><a data-toggle="tab" href="#menu3">警戒發報資料下載</a></li>
            </ul>
            <div id="home" class="tab-pane fade in active">
              <br />
              <div class="form-inline">
                <label class="label label-success" style="font-size: 20px">縣市：</label>
                <asp:DropDownList ID="ddlCOUNTY" runat="server" CssClass="form-control"></asp:DropDownList>
              </div>
              <br />
              <asp:Label ID="lblDataTime" runat="server" Text="開始時間："></asp:Label>
              <input id="CountryDateS" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeCountryS" runat="server"></asp:DropDownList>
              <asp:Label ID="Label1" runat="server" Text="結束時間："></asp:Label>
              <input id="CountryDateE" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeCountryE" runat="server"></asp:DropDownList>
              <br />
              密碼：<input id="txtpass" type="text" runat="server">
              <asp:Button ID="btnExportCounty" runat="server" Text="資料匯出" CssClass="btn btn-success" OnClick="btnExportCounty_Click" />


            </div>
            <div id="menu1" class="tab-pane fade">
              <br />
              <div class="form-inline">

                <%--<label class="label label-success" style="font-size: 20px">縣市：</label>
                                <asp:DropDownList ID="ddlCOUNTY2" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCOUNTY2_SelectedIndexChanged"></asp:DropDownList>--%>
                <label class="label label-success" style="font-size: 20px">雨量站：</label>
                <asp:DropDownList ID="ddlRainStation" runat="server" CssClass="form-control"></asp:DropDownList>
                <label class="label label-success" style="font-size: 20px">搜尋(雨量站代碼)：</label>
                <asp:TextBox ID="txtStationQuery" runat="server"></asp:TextBox>
                <asp:Button ID="btnStationQuery" runat="server" Text="搜尋" CssClass="btn btn-success" OnClick="btnStationQuery_Click" />
              </div>
              <br />

              <asp:Label ID="Label2" runat="server" Text="開始日期："></asp:Label>
              <input id="RainDateS" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeRainS" runat="server"></asp:DropDownList>
              <asp:Label ID="Label3" runat="server" Text="結束日期："></asp:Label>
              <input id="RainDateE" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeRainE" runat="server"></asp:DropDownList>
              <br />
              密碼：<input id="txtpass2" type="text" runat="server">
              <asp:Button ID="btnExportStation" runat="server" Text="資料匯出" CssClass="btn btn-success" OnClick="btnExportStation_Click" />
            </div>
            <div id="menu2" class="tab-pane fade">
              <br />
              <asp:Label ID="Label4" runat="server" Text="開始日期："></asp:Label>
              <input id="DateShpS" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeShpS" runat="server"></asp:DropDownList>
              <asp:Label ID="Label5" runat="server" Text="結束日期："></asp:Label>
              <input id="DateShpE" type="text" class="easyui-datebox" runat="server">
              <asp:DropDownList ID="ddlTimeShpE" runat="server"></asp:DropDownList>
              <br />
              密碼：<input id="txtpass3" type="text" runat="server">
              <asp:Button ID="btnExportSHP" runat="server" Text="資料匯出" CssClass="btn btn-success" OnClick="btnExportSHP_Click" />
            </div>

          </div>
          <asp:HiddenField ID="HiddevActiveTab" runat="server" />

          <script type="text/javascript">
            $('#dd').datebox({
              //required: true
            });


            $('#Button2').click(
                function () {
                  Button3.click();

                }
                );



            function InitForm() {
              $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var target = $(e.target).attr("href") // activated tab
                //alert(target);
                localStorage.setItem('lastTab1', target);
              });

              var lastTab = localStorage.getItem('lastTab1');
              if (lastTab) {
                $('[href="' + lastTab + '"]').tab('show');
              }
            }

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_endRequest(function () {
              InitForm();
            });

            $(document).ready(function () {
              InitForm();
            });


          </script>
        </ContentTemplate>
        <Triggers>
          <asp:PostBackTrigger ControlID="btnExportCounty" />
          <asp:PostBackTrigger ControlID="btnExportStation" />
          <asp:PostBackTrigger ControlID="btnExportSHP" />
          <asp:PostBackTrigger ControlID="btnStationQuery" />


          <%--<asp:PostBackTrigger ControlID="ddlCOUNTY2" />--%>
        </Triggers>
      </asp:UpdatePanel>



    </div>
  </form>

</body>
</html>
