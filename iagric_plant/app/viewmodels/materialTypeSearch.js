define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'],
function (dialog, ko, jqueryui, km, utils, loginInfo) {
    var vmSearch = function () {
        var self = this;
        self.first = ko.observable();
        self.second = ko.observable();
        self.third = ko.observable();
        self.selectValue = ko.observable();
        self.selectName = ko.observable();
        self.selTitle = ko.observable('').extend({
            required: {
                params: true,
                message: "请输入产品类别再查询!"
            }
        });
        self.property = ko.observableArray([]);
        /****************查询所有相关品类*********************/
        self.searchMaterial = function () {
            var test = getData(self.selTitle());
            self.property(new Array());
            self.property(test);
        }
        /***************选择某个品类****************/
        self.Search = function (value, name, data, event) {
            self.selectValue(value);
            self.selectName(name);
        };
        /*********************确定*****************/
        self.Selected = function (data, envnt) {
            self.closeOK(self.selectValue());
        }

    }
    var getData = function (name) {
        var sendData = {
            typeName: name
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Material/SearchType",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult;
            }
        });
        return data;
    }
    vmSearch.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmSearch.prototype.close = function () {
        dialog.close(this);
    }
    vmSearch.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmSearch.show = function () {
        var vmObj = new vmSearch();
        return dialog.show(vmObj);
    };
    return vmSearch;
});