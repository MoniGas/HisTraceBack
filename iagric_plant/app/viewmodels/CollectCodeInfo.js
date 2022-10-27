define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'jquery.fileDownload', './CollectCodeEWM'],
function (dialog, ko, jqueryui, km, utils, loginInfo, jfd, CollectCodeEWM) {

    var id = 0;
    var pIndex = 1;
    //自定义绑定-分页控件
    ko.bindingHandlers.ewmPagerBH = {
        init: function (element, valueAccessor, allBindingsAccessor) {
        },
        update: function (element, valueAccessor, allBindingsAccessor) {

            var value = valueAccessor();
            var allBindings = allBindingsAccessor();
            var pageSize = parseInt(ko.utils.unwrapObservable(value));
            var totalCounts = parseInt(ko.utils.unwrapObservable(allBindings.totalCounts));
            var pageIndex = parseInt(ko.utils.unwrapObservable(allBindings.pageIndex));

            $(element).jqPaginator({
                totalCounts: totalCounts,
                pageSize: pageSize,
                visiblePages: 5,
                currentPage: pageIndex,
                first: '<li class="first"><a href="javascript:;">首页</a></li>',
                last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                onPageChange: function (num, type) {
                    if (type == 'change') {
                        var list = getDataKO(num, vmObj.ewm(), id, vmObj.vmModels.selectedOption());
                        vmObj.updateData(list);
                        this.pageSize = parseInt(vmObj.vmDataList.pageSize());
                        this.totalCounts = parseInt(vmObj.vmDataList.totalCounts());
                    }
                }
            });
            pIndex = pageIndex;
        }
    };

    var getData = function (pageIndex, sId) {
        var sendData = {
            sId: sId,
            pageIndex: pageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_CollectCode/CollectCodeInfo",
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

    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, sId) {
        var list = km.fromJS(getData(pageIndex, sId));
        return list;
    }

    var vmCode = function (sId) {
        var self = this;
        self.path = ko.observable();
        self.ewm = ko.observable('');
        self.vmDataList = ko.observableArray();
        //分页、搜索时更新数据源
        self.updateData = function (list) {
            self.vmDataList.ObjList(list.ObjList());
            self.vmDataList.pageSize(list.pageSize());
            self.vmDataList.totalCounts(list.totalCounts());
            self.vmDataList.pageIndex(list.pageIndex());
        }
        self.showCode = function (code) {
            CollectCodeEWM.show(code());
        }

        self.mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var Setting = self.find("button[eleflag='Setting']");
            Setting.css({ "display": "" });
        }

        self.mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var Setting = self.find("button[eleflag='Setting']");
            Setting.css({ "display": "none" });
        }
        self.CheckVerify = function () {
            var verify = 0;
            var sendData = {}
            $.ajax({
                type: "POST",
                url: "/Login/GetEnterpriseVerify",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    verify = jsonResult.code;
                }
            });
            if (verify > 0) {
                return true;
            }
            else {
                return false;
            }
        }

    }

    vmCode.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmCode.prototype.close = function () {
        //alert(this.province().code);
        dialog.close(this);
    }
    vmCode.prototype.initMenus = function (id) {
        //alert(id);
        return this.vmDataList();
    }
    var vmObj;
    vmCode.show = function (sId) {
        id = sId;
        vmObj = new vmCode(sId);
        vmObj.vmDataList = getDataKO(1,sId);
        return dialog.show(vmObj);
    };
    return vmCode;
})