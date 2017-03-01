<%@ Page Title="" Language="C#" MasterPageFile="~/default.Master" AutoEventWireup="true" CodeBehind="PhotoDF.aspx.cs" Inherits="DhoePortal.PhotoDF" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Add jQuery library -->
	<%--<script type="text/javascript" src="../lib/jquery-1.10.2.min.js"></script>--%>
    <script type="text/javascript" src="fancyBox/lib/jquery-1.10.2.min.js"></script>

	<!-- Add mousewheel plugin (this is optional) -->
	<%--<script type="text/javascript" src="../lib/jquery.mousewheel.pack.js?v=3.1.3"></script>--%>
    <script type="text/javascript" src="fancyBox/lib/jquery.mousewheel.pack.js"></script>

	<!-- Add fancyBox main JS and CSS files -->
	<%--<script type="text/javascript" src="../source/jquery.fancybox.pack.js?v=2.1.5"></script>
	<link rel="stylesheet" type="text/css" href="../source/jquery.fancybox.css?v=2.1.5" media="screen" />--%>
    <script type="text/javascript" src="fancyBox/source/jquery.fancybox.js?v=2.1.5"></script>
	<link rel="stylesheet" type="text/css" href="fancyBox/source/jquery.fancybox.css?v=2.1.5" media="screen" />

	<!-- Add Button helper (this is optional) -->
	<link rel="stylesheet" type="text/css" href="fancyBox/source/helpers/jquery.fancybox-buttons.css?v=1.0.5" />
	<script type="text/javascript" src="fancyBox/source/helpers/jquery.fancybox-buttons.js?v=1.0.5"></script>

	<script type="text/javascript">
		$(document).ready(function() {
		

			$('.fancybox-buttons').fancybox({
				openEffect  : 'none',
				closeEffect : 'none',

				prevEffect : 'none',
				nextEffect : 'none',
                autoPlay : 'true',

				closeBtn  : true,

				helpers : {
					title : {
						type : 'inside'
					},
					buttons	: {}
				},

				afterLoad : function() {
					this.title = 'Image ' + (this.index + 1) + ' of ' + this.group.length + (this.title ? ' - ' + this.title : '');
				}
			});


		});
	</script>
    



</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2><span style="font-weight: bold;color:#31708f;">Albums</span></h2>
        <%--<asp:PlaceHolder ID="phBread" runat="server" />--%>
        <%--<ol class="breadcrumb">
            <li><a href="#">Home</a></li>
            <li><a href="#">2013</a></li>
            <li class="active">十一月</li>
        </ol>--%>
        <%--<a href="Photo.aspx"></a>

        <img src="images/logo.jpg" onerror="javascript:this.src='/images/blank.gif'" border="0" class="img-thumbnail " style="width: 100%;">--%>
        <asp:DataList ID="DataList1" runat="server" OnItemDataBound="DataList1_ItemDataBound" RepeatColumns="3" RepeatDirection="Horizontal" Width="100%">
            <ItemTemplate>


                <div class="album_item inline-block">
                    <div class="fix_png">



                        <asp:PlaceHolder ID="phphoto" runat="server" />

                        <%--<a href="//photo.xuite.net/j73025448/19904265">
                            <img src="images/logo.jpg" onerror="javascript:this.src='/images/blank.gif'" border="0" class="img-thumbnail " style="width: 100%;">
                        </a>--%>
                    </div>
                    <div class="album_info">
                        <p class="album_info_title">

                            <asp:PlaceHolder ID="phtitle" runat="server" />


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


    <%--<script src="js/lightbox-plus-jquery.min.js"></script>--%>
</asp:Content>

