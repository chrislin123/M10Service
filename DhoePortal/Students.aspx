<%@ Page Title="" Language="C#" MasterPageFile="~/default.Master" AutoEventWireup="true" CodeBehind="Students.aspx.cs" Inherits="DhoePortal.Students" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip({
                html: true,
                //title: $('#' + $(this).data('tip')).html(),
                title: $('#my-tip').html(),
                //title: "Hooray!", 
                trigger: "click"
            });


            //$('.btn').tooltip({ title: "<h1><strong>HTML</strong> inside <code>the</code> <em>tooltip</em></h1>", html: true, placement: "right" });
        });

        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--<br /><br /><br /><br /><br /><br /><br />--%>
    <!-- Tooltip content -->
    <%--<div id="my-tip" class="tip-content hidden">
        
        <h2>Tip title</h2>
        <p>This is my tip content</p>
        <iframe src="Photo.aspx" width="50px"></iframe>
    </div>--%>
    <div class="container">
        <a class="test" href="#" data-toggle="tooltip" data-placement="top" title="Hooray!test" >Top</a>
        <span data-toggle="tooltip"   >test</span>
        <h1><span style="font-weight: bold; color: #31708f;" data-toggle="tooltip" >在校生</span></h1>

        <asp:PlaceHolder ID="PlaceHolder1" runat="server" />

        <h1><span style="font-weight: bold; color: #31708f;">博士</span></h1>

        <asp:PlaceHolder ID="PlaceHolder2" runat="server" />

        <h1><span style="font-weight: bold; color: #31708f;">碩士</span></h1>

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
