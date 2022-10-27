define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils','logininfo'],
function (dialog, ko, jqueryui, km, utils,loginInfo) {

    var id = 0;
    //自定义绑定-分页控件
    ko.bindingHandlers.salePagerBH = {
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
                        var list = getDataKO(num, vmObj.ewm(), id);
                        vmObj.updateData(list);
                        this.pageSize = parseInt(vmObj.vmDataList.pageSize());
                        this.totalCounts = parseInt(vmObj.vmDataList.totalCounts());
                    }
                }
            });
        }
    };

    var getData = function (pageIndex, ewm, rId) {
        var sendData = {
            ewm: ewm,
            rId: rId,
            pageIndex: pageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Request/GetSaleList",
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
    var getDataKO = function (pageIndex, ewm, rId) {
        var list = km.fromJS(getData(pageIndex, ewm, rId));
        return list;
    }

    var vmCode = function (rId) {
        var self = this;
        self.ewm = ko.observable('');
        self.vmDataList = ko.observableArray();

        self.status = function (index) {
            var index = ko.utils.unwrapObservable(index);
            switch (index) {
                case 1040000008:
                    return "未使用";
                case 1040000009:
                    return "已使用";
            }
        }

        self.subDate = function (date) {
            var date = ko.utils.unwrapObservable(date);
            return date.substring(0, 10);
        }

        self.SearchData = function () {
            var list = getDataKO(1, self.ewm(), rId);
            self.updateData(list);
        }

        //分页、搜索时更新数据源
        self.updateData = function (list) {
            self.vmDataList.ObjList(list.ObjList());
            self.vmDataList.pageSize(list.pageSize());
            self.vmDataList.totalCounts(list.totalCounts());
            self.vmDataList.pageIndex(list.pageIndex());
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
    vmCode.show = function (rId) {
        id = rId;
        vmObj = new vmCode(rId);
        vmObj.vmDataList = getDataKO(1, vmObj.ewm(), rId);
        return dialog.show(vmObj);
    };
    return vmCode;
})