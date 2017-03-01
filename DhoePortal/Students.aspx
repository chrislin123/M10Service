<%@ Page Title="" Language="C#" MasterPageFile="~/default.Master" AutoEventWireup="true" CodeBehind="Students.aspx.cs" Inherits="DhoePortal.Students" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip({
                html: true,
                //title: $('#' + $(this).data('tip')).html(),
                title: $('#my-tip').html(),
                //title: "Hooray!", 
                //trigger: "click"
                trigger: "hover"

            });

            $('[rel="popover"]').popover();
            //$('.btn').tooltip({ title: "<h1><strong>HTML</strong> inside <code>the</code> <em>tooltip</em></h1>", html: true, placement: "right" });
        });


        //$(window).load(function () {
        //    //var img = '<iframe frameborder="0" scrolling="no" height="220" width="420" src="http://dxlite.g7vjr.org/?dx=LU5DX&limit=10"></iframe>';
        //    //$("#blob").popover({ title: 'Last 10 spots for the selected station', content: img, html: true });
        //    $('[rel="popover"]').popover();
        //})

    </script>
    <style>
        /* Popover */
        .popover {
            max-width: 100%;
            width: auto;
            /*border: 2px dotted red;*/
        }
        

        /* Popover Header */
        .popover-title {
            /*background-color: #73AD21;*/
            /*color: #FFFFFF;*/
            font-size: 28px;
            text-align: center;
        }

        /* Popover Body */
        .popover-content {
            /*background-color: coral;
            color: #FFFFFF;
            padding: 25px;*/
        }

        /* Popover Arrow */
        .arrow {
            /*border-right-color: red !important;*/
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <%--<a href="#" id="blob" class="btn large primary" rel="popover" style="margin-top:100px">hover for popover</a>--%>

    <%--<a href="#" id="blob2" class="btn large primary" data-trigger="hover" rel="popover" data-html="true" data-content='<iframe frameborder="0" scrolling="no" height="220" width="420"src="http://dxlite.g7vjr.org/?dx=LU5DX&limit=10"></iframe>' style="margin-top: 100px">hover for popover</a>--%>
    <%--<a href="#" id="blob2" data-trigger="hover" rel="popover" data-html="true" title="我的測試" data-content='<iframe frameborder="0" scrolling="no" height="500" width="500" src="Photo.aspx"></iframe>'>hover for popover</a>--%>
    <%--<a href="#" id="blob2" data-trigger="focus" rel="popover" data-html="true" title="我的測試" data-content='<iframe frameborder="0" scrolling="no" height="220" width="420" src="Photo.aspx"></iframe>'>hover for popover</a>--%>

    <%--<br /><br /><br /><br /><br /><br /><br />--%>
    <!-- Tooltip content -->
    <%--<div id="my-tip" class="tip-content hidden" style="width: 500px">

        <h2>Tip title</h2>
        <p>This is my tip content</p>
        <iframe src="Photo.aspx" width="250px"></iframe>
    </div>--%>
    <div class="container">
        <%--<a class="test" href="#" data-toggle="tooltip" data-placement="right" title="Hooray!test">Top</a>
        <span data-toggle="tooltip" data-placement="right">test</span>--%>
        <h2><span style="font-weight: bold; color: #31708f;" data-toggle="tooltip">在學生</span></h2>

        <asp:PlaceHolder ID="PlaceHolder1" runat="server" />

        <h2><span style="font-weight: bold; color: #31708f;">博士</span></h2>

        <asp:PlaceHolder ID="PlaceHolder2" runat="server" />

        <h2><span style="font-weight: bold; color: #31708f;">碩士</span></h2>

        <asp:PlaceHolder ID="PlaceHolder3" runat="server" />


        <%--        <div class="panel panel-info">
            <div class="panel-heading">
                <h3><span style="font-weight: bold;">在校生</span></h3>
              
            </div>
            <div class="panel-body">
                <table class="table">                   
                    <tbody>
                        <tr>
                            <td style="width: 100px;">博士班</td>
                            <td>黃聰憲 楊斯堯 蕭凱文 楊致遠</td>

                        </tr>
                        <tr>
                            <td>碩士班</td>
                            <td>黃亭茵 侯創耀 鍾佳育 吳宗祐 陳筱惟 Annie 林睦容 洪子傑 顏欣玫 林恩如</td>

                        </tr>

                    </tbody>
                </table>
            </div>
        </div>

        <div class="panel panel-info">
            <div class="panel-heading">
                <h3><span style="font-weight: bold;">博士</span></h3>
                
            </div>
            <div class="panel-body">
                <table class="table">
                    <tbody>
                        <tr>
                            <td style="width: 100px;">2016</td>
                            <td>蘇郁文</td>
                        </tr>
                        <tr>
                            <td>2015</td>
                            <td>吳慶現</td>
                        </tr>
                        <tr>
                            <td>2014</td>
                            <td>彭大雄 徐郁超</td>
                        </tr>
                        <tr>
                            <td>2012</td>
                            <td>黃信茗 黃文舜</td>
                        </tr>
                        <tr>
                            <td>2011</td>
                            <td>阮光長</td>
                        </tr>
                        <tr>
                            <td>2010</td>
                            <td>郭峰豪 葉昭龍</td>
                        </tr>
                        <tr>
                            <td>2008</td>
                            <td>張家榮</td>
                        </tr>
                        <tr>
                            <td>2007</td>
                            <td>李明熹 陳宗顯 王志賢</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="panel panel-info">
            <div class="panel-heading">
                <h3><span style="font-weight: bold;">碩士</span></h3>
                
            </div>
            <div class="panel-body">
                <table class="table">
                    <tbody>
                        <tr>
                            <td style="width: 100px;">2016</td>
                            <td>許喬凱 何秋燕 陳新霖 李芷穎 Roy Daniel</td>
                        </tr>
                        <tr>
                            <td>2015</td>
                            <td>洪唯峰 鄧鈺潔 蔡宜潔 劉家齊 段青新 鍾卓翰</td>
                        </tr>
                        <tr>
                            <td>2014</td>
                            <td>王彥程 向韋誠 張添鑫 簡鍾凱 蕭凱文 田愷捷</td>
                        </tr>
                        <tr>
                            <td>2013</td>
                            <td>李家純 林曉萱 張家豪 林書豪 粘為勇 蘇一誠</td>
                        </tr>
                        <tr>
                            <td>2012</td>
                            <td>黃信茗 黃文舜</td>
                        </tr>
                        <tr>
                            <td>2011</td>
                            <td>簡佐伊 曾國訓 葛清富</td>
                        </tr>
                        <tr>
                            <td>2010</td>
                            <td>彭仲文 張良亦 莊瑞安 楊斯堯 梁繼友</td>
                        </tr>
                        <tr>
                            <td>2009</td>
                            <td>蔡孟芳 林程翰 蘇俊霖</td>
                        </tr>
                        <tr>
                            <td>2008</td>
                            <td>徐郁超 李亮廷 徐惠亭 張雅雯 盧建成 俞俊賓</td>
                        </tr>
                        <tr>
                            <td>2007</td>
                            <td>黃聰憲 陳軍豪 陳建宇 陳建豪 王品臻 邱神錫 彭大雄 林志銘</td>
                        </tr>
                        <tr>
                            <td>2006</td>
                            <td>許斐芳 翁佐戎 葉昭龍 蔡宏洋</td>
                        </tr>
                        <tr>
                            <td>2005</td>
                            <td>黃文舜 張智瑜 徐福燦</td>
                        </tr>

                    </tbody>
                </table>
            </div>
        </div>--%>
    </div>

</asp:Content>
