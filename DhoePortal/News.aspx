<%@ Page Title="" Language="C#" MasterPageFile="~/default.Master" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="DhoePortal.News" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h1><span style="font-weight: bold;color:#31708f;">最新消息</span></h1>

        <asp:PlaceHolder ID = "PlaceHolder1" runat="server" />


        <%--<table class="table">
            <tbody>
                <tr style="width: 100px;" class="active">
                    <td>2016/01/19</td>
                    <td>
                        <a href="https://www.1111.com.tw/discuss/discussTopic.asp?cat=NCKU&id=45839">[新聞]成大詹錢登、謝正倫教授榮獲總統頒「莫拉克風災重建紀念章證書」</a>
                    </td>
                </tr>
                <tr>
                    <td>2015/09/05</td>
                    <td>
                        <a href="https://ctld.ntu.edu.tw/fd/teaching_resource/page1-1_detail.php?bgid=1&gid=1&nid=547">[採訪] 走出教室學習體驗傳授水利工程武林秘笈(財團法人高等教育評鑑中心)</a>
                    </td>
                </tr>
                
                <tr class="active">
                    <td>2015/10/05</td>
                    <td>
                        <a href="http://www3.hyd.ncku.edu.tw/?p=7109">[新聞]詹錢登教授接受財團法人高等教育評鑑中心採訪—走出教室，學習體驗</a>
                    </td>
                </tr>
                <tr >
                    <td>2014/09/15</td>
                    <td>
                        <a href="http://www3.hyd.ncku.edu.tw/?p=4848">[新聞]詹錢登老師榮獲美國土木工程師學會(ASCE)會士</a>
                    </td>
                </tr>
                <tr class="active">
                    <td>2012/11/20</td>
                    <td>
                        <a href="https://www.youtube.com/watch?v=VMvSr5zUcJk">[新聞] 成大詹錢登教授救災有功獲行政院三等功績獎章</a>
                    </td>
                </tr>
                
            </tbody>
        </table>--%>
    </div>
</asp:Content>
