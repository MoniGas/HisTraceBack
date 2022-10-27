
function htmlencode(s) {
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(s));
    return div.innerHTML;
}
function htmldecode(s) {
    var div = document.createElement('div');
    div.innerHTML = s;
    return div.innerText || div.textContent;
}

function searchData() {
    $("head").append("<style type=\"text/css\">.Xiaoqu_list{position:absolute;background-color:#fff;border:1px solid #ccc;border-radius:0 0 5px 5px;box-shadow:2px 2px 5px #999;z-index:999999;margin:0;padding:0;list-style:none}.Xiaoqu_list li{cursor:pointer;font-size:12px;text-align:left;padding:0 24px;height:24px;line-height:24px;overflow:hidden}.Xiaoqu_list li.msvr{background-color:#fde}.Xiaoqu_list li s{display:none;}.Xiaoqu_list li b{font-weight:800;color:#F00}.Xiaoqu_list li i{color:#090}</style>");
    $(".Xiaoqu_list").remove();
    var SearchInput = $("input[SearchInput]");
    SearchInput.unbind("keydown focus keyup");
    $(window).unbind("resize load");
    $("body").unbind("click");
    var S_locallist = null;
    SearchInput.each(function () {
        var thisid = $(this).attr("id");
        $(this).attr("autocomplete", "off");
        $(this).attr("itemindex", -1);
        var thislist = $("<ul class=\"Xiaoqu_list\" id=\"l_" + thisid + "\" style=\"display:none\"></ul>");
        /*thislist.css("top", $(this).offset().top+$(this).height());
        thislist.css("left", $(this).offset().left);*/
        var thishide = $("<input type=\"hidden\" id=\"h_" + thisid + "\" />");
        thishide.insertAfter($(this));
        thislist.insertAfter($(this));
        $("body").append(thislist);
    });
    SearchInput.bind("keydown", function (e) {
        if (e.keyCode == 38 || e.keyCode == 40 || e.keyCode == 13 && $(this).attr("itemindex") >= 0 || e.keyCode == 9) {
            $this = $(this);
            var thisid = $this.attr("id");
            var thislist = $("#l_" + thisid);
            var lcount = thislist.children().size();
            if (e.keyCode == 38) {
                $this.attr("itemindex", parseInt($this.attr("itemindex")) - 1 < 0 ? lcount - 1 : parseInt($this.attr("itemindex")) - 1);
            } else if (e.keyCode == 40) {
                $this.attr("itemindex", parseInt($this.attr("itemindex")) + 1 >= lcount ? 0 : parseInt($this.attr("itemindex")) + 1);
            } else if (e.keyCode == 13) {
                thislist.children().eq($this.attr("itemindex")).trigger("click");
            } else {
                thislist.css("display", "none");
                return true;
            }
            thislist.children().removeClass("msvr");
            thislist.children().eq($this.attr("itemindex")).addClass("msvr");
            return false;
        }
    });
    SearchInput.bind("focus keyup", function (e) {
        if ($(this).attr("readonly")) {
            return false;
        }
        if (e.keyCode == 38 || e.keyCode == 40 || e.keyCode == 13) {
            return false;
        }

        var $this = $(this);
        var thisid = $this.attr("id");
        var thishide = $("#h_" + thisid);
        $this.attr("itemindex", -1);
        var key = $this.val();
        var page = $this.attr("page");
        var flag = $this.attr("flag");
        var thislist = $("#l_" + thisid);

        if (e.type == "focus") {
            thislist.css("width", $this.outerWidth() - 2);
            thislist.css("top", $this.offset().top + $(this).outerHeight() - 1);
            thislist.css("left", $this.offset().left);
        }

        //S_locallist = thislist;
        if (e.type == "keyup") {
//            thishide.attr("ShopName", "");
//            thishide.attr("CodeID", "");
//            thishide.attr("CodeValue", "");
            $this.trigger("clearselect");
        }
//        if (key) {
            $.post('/AdminOrigin/GetSearchInfo', { value: key, page: page, flag: flag }, function (data) {
                thislist.html("");
                data = data.data;
                if (data.length > 0) {
                    thislist.css("display", "block");
                } else {
                    thislist.css("display", "none");
                    $this.trigger("nofound");
                }
                $.each(data, function () {
                    var key = $this.val();
                    var xqr = this.Value.replace(key, "<b>" + key + "</b>");
                    var li = $("<li><span>" + xqr + "</span></li>");
                    var code = this.Value;
                    thislist.append(li);
                    var id = this.Id;
                    li.bind("click", function () {
                        var xq = $(this).children().eq(0).text();
                        $this.val(xq);
                        $(this).parent().css("display", "none");
                        $this.attr("itemindex", -1);
                        $this.trigger("selectok");
                    });
                    li.bind("mouseover", function () {
                        $this.attr("itemindex", $(this).index());
                        $(this).parent().children().removeClass("msvr");
                        $(this).addClass("msvr");
                    });
                    //li.on("mouseout", function () {
                    //    $(this).removeClass("msvr");
                    //});
                });
            });
//        } else {
//            thislist.html("");
//        }
    });
    //SearchInput.on("blur", function () {
    //    $(this).attr("itemindex", -1);
    //    if (S_locallist) {
    //        var locallist = S_locallist;
    //        S_locallist = null;
    //        setTimeout(function () {
    //            locallist.css("display", "none");
    //        }, 200);
    //    }
    //});

    $("body").bind("click", function (e) {
        var oEvent = e || event;
        var oLeft = oEvent.pageX;
        var oTop = oEvent.pageY;
        //console.log("鼠标坐标：(" + oLeft + "," + oTop + ")");
        SearchInput.each(function () {
            var $this = $(this);
            var thisid = $this.attr("id");
            var thislist = $("#l_" + thisid);
            if ((oLeft < $this.offset().left || oLeft > $this.offset().left + $this.outerWidth()) || (oTop < $this.offset().top || oTop > $this.offset().top + $this.outerHeight() + thislist.outerHeight())) {
                //console.log("鼠标坐标：(" + oLeft + "," + oTop + ") 左右上下：(" + $this.offset().left + ","+($this.offset().left + $this.outerWidth())+","+$this.offset().top+","+($this.offset().top + $this.outerHeight() + thislist.outerHeight())+")");
                thislist.css("display", "none");
            }
        });
    });
    $(window).bind("resize load", function () {
        SearchInput.each(function () {
            var $this = $(this);
            var thisid = $this.attr("id");
            var thislist = $("#l_" + thisid);
            thislist.css("width", $this.outerWidth() - 2);
            thislist.css("top", $this.offset().top + $(this).outerHeight() - 1);
            thislist.css("left", $this.offset().left);
        });
    });
    function addCookie(key) {
        //        var xqSeachList = $.cookie("xqSeachList") ? $.cookie("xqSeachList") : "";
        //        var cl = xqSeachList.split("|");
        //        cl.splice($.inArray('', cl), 1);
        //        var has = false;
        //        for (var f in cl) {
        //            if (cl[f] == key) {
        //                has = true;
        //                break;
        //            }
        //        }
        //        if (!has) {
        //            xqSeachList += ("|" + key);
        //            if (cl.length >= 5) {
        //                xqSeachList = xqSeachList.substring(1);
        //                xqSeachList = xqSeachList.substring(xqSeachList.indexOf("|"));
        //            }
        //        }
        //        $.cookie("xqSeachList", xqSeachList, { expires: 7, path: '/' });
    }

}
$(function () {
  
});