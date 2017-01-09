<%@ Page Title="" Language="C#" MasterPageFile="~/default.Master" AutoEventWireup="true" CodeBehind="Photo.aspx.cs" Inherits="DhoePortal.Photo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">



    <link rel="stylesheet" href="css/fix.css">

    <style>
        A.album_info_title_hy:link {
            color: #936f4a;
            text-decoration: none;
        }

        A.album_info_title_hy:visited {
            color: #936f4a;
            text-decoration: none;
        }
        /*.album_info {
            PADDING: 0px 5px 5px;
            FONT-SIZE: 16px;
            WIDTH: auto;
            text-align: left;
        }

        .album_cover {
            text-align: center;
            width: 169px;
            height: 169px;
            padding: 6px 13px 13px 6px;
            background: url(/images/p_bg01.png) no-repeat;
        }

        .album_item {
            width: 200px;
            margin: 0px 0px 0px 0px;
            padding: 5px 0px;
            text-align: center;
            vertical-align: top;
            position: relative;
        }

        album_info_title {
            margin: 2px;
            font-weight: bold;
            font-size: 15px;
        }

        

        
        /*p {
            display: block;
            -webkit-margin-before: 1em;
            -webkit-margin-after: 1em;
            -webkit-margin-start: 0px;
            -webkit-margin-end: 0px;
        }*/ */
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <div class="container">

        <asp:DataList ID="DataList1" runat="server" OnItemDataBound="DataList1_ItemDataBound" RepeatColumns="3" RepeatDirection="Horizontal" Width="100%">
            <ItemTemplate>


                <div class="album_item inline-block">
                    <div class="fix_png">



                        <asp:PlaceHolder ID = "phphoto" runat="server" />
                      
                        <%--<a href="//photo.xuite.net/j73025448/19904265">
                            <img src="images/logo.jpg" onerror="javascript:this.src='/images/blank.gif'" border="0" class="img-thumbnail " style="width: 100%;">
                        </a>--%>
                    </div>
                    <div class="album_info">
                        <p class="album_info_title">

                            <asp:PlaceHolder ID = "phtitle" runat="server" />


                            <%--<asp:HyperLink ID="HyperLink1" runat="server" class="album_info_title_hy">HyperLink</asp:HyperLink>--%>
                            <%--<a href="//photo.xuite.net/j73025448/19904265" class="album_info_title_hy">簡-實踐
                               　共46張
                                <p class="album_info_date">
                                </p>
                            </a>--%>
                        </p>

                    </div>
                </div>


                <%--<div class='fix_png'>
                    <a href="//photo.xuite.net/j73025448/19899742">
                        <img src="//7.share.photo.xuite.net/j73025448/178197c/19899742/1134127390_Q.jpg" onerror="javascript:this.src='/images/blank.gif'" border="0" class="album_cover "></a>
                </div>
                <div class="album_info">
                    <p class="album_info_title">
                        <a href="//photo.xuite.net/j73025448/19899742">妙妙-妙的事</a>
                    </p>
                    <p class="album_info_date">
                        2016-08-01　共71張
                    </p>
                </div>--%>
            </ItemTemplate>

        </asp:DataList>




    </div>
</asp:Content>
