define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'],
function (dialog, ko, jqueryui, km, utils, loginInfo) {
    var vmSearch = function () {
        var self = this;
        self.selectValue = ko.observable();
        self.selectName = ko.observable();
        /*********************确定*****************/
        self.Selected = function (data, envnt) {
            self.closeOK(self.selectValue());
        }
    }
    var getData = function (cid) {
        var sendData = {
        cId:cid
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_Category/SelectCategoryNew",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            data = jsonResult.ObjList;
        }
    });
    return data;
}
var SetHtml = function (obj,cid,name) {
    var $this = obj == null ? null : obj.closest(".style1");
    var title = obj == null ? "请选择<span class='type'>医疗器械</span>品类" : "请选择<span class='type'>" + name + "</span>分类";
    var data = getData(cid);
    var html = "";
    if (data.length > 0) {
        html = "<div class='style1 bgB'><h2 class='ldtit'>"+title+"</h2><div class='ulStyle height500'><ul class='ulData'>";
    }
    $.each(data, function (index, item) {
        html = html + "<li code='" + item.industrycategory_code + "' cid='" + item.industrycategory_id + "'>" + item.industrycategory_name + "</li>";
    });
    if (data.length > 0) {
        html = html + "</ul></div></div>";
    }
    if (obj != null && $this.next().length > 0) {
        $this.nextAll().remove();
    }
    $("#divData").append(html);
    $(".ulData").on("click", "li", function () {
        $(this).parent().find("li").removeClass("select");
        $(this).addClass("select");
        var content = "";
        var code = "";
        $.each($("li.select"), function (index, item) {
            content = content + $.trim($(item).text()) + ">>";
            code = code + $.trim($(item).attr("cid")) + ",";
        });
        content = content.substring(0, content.length - 2);
        $("#txtSelect").val(content);
        $("#txtSelectCode").val(code);
        SetHtml($(this), $(this).attr("cid"),$.trim($(this).text()));
    });
    if ($(".ulData").length > 3) {
        $(".divHandle").show();
    }
    else {
        $(".divHandle").hide();
    }
}
vmSearch.prototype.binding = function () {
    /*****************************左右滑动****************************************/
//    $(".divHandle").click(function () {
//        var oldLeft = $(".divMove").css("left").replace("px", "");
//        var count = $(".ulData").length - 3;
//        var liWidth = $(".ulData li").eq(0).width();
//        var width = 0;
//        if ($(this).attr("flag") == "prev") {
//            width = (parseInt(Math.abs(oldLeft)) + liWidth);
//        }
//        else {
//            width = (parseInt(Math.abs(oldLeft)) - liWidth);
//        }
//        if (width <= count * liWidth) {
//            $(".divMove").animate({ left: '-' + width + 'px' });
//        }

//        if (width == count * liWidth) {
//            $(".divHandle").eq(0).addClass("disAble");
//            $(".divHandle").eq(1).removeClass("disAble");
//        }
//        else if (width == 0) {
//            $(".divHandle").eq(1).addClass("disAble");
//            $(".divHandle").eq(0).removeClass("disAble");
//        }
//    });
    /*****************************左右滑动****************************************/
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    SetHtml(null,"","");
    $(".ulData").on("click", "li", function () {
        $(this).parent().find("li").removeClass("select");
        $(this).addClass("select");
        var content = "";
        var code = "";
        $.each($("li.select"), function (index, item) {
            content = content + $.trim($(item).text()) + ">>";
            code = code + $.trim($(item).attr("cid")) + ",";
        });
        content = content.substring(0, content.length - 2);
        $("#txtSelect").val(content);
        $("#txtSelectCode").val(code);
        SetHtml($(this),$(this).attr("cid"),$.trim($(this).text()));
    });
}
vmSearch.prototype.close = function () {
    dialog.close(this);
}
vmSearch.prototype.closeOK = function (id) {
    dialog.close(this, $("#txtSelectCode").val(), $("#txtSelect").val());
}
vmSearch.show = function () {
    var vmObj = new vmSearch();
    return dialog.show(vmObj);
};
return vmSearch;
});