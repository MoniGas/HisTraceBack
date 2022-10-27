define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'jquery.fileDownload'],
function (dialog, ko, jqueryui, km, utils, loginInfo, jfd) {

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
                        var list = getDataKO(num, id);
                        vmObj.updateData(list);
                        this.pageSize = parseInt(vmObj.vmDataList.pageSize());
                        this.totalCounts = parseInt(vmObj.vmDataList.totalCounts());
                    }
                }
            });
            pIndex = pageIndex;
        }
    };

    var getData = function (pageIndex, oId) {
        var sendData = {
            oId: oId,
            pageIndex: pageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/IntStorage/IntStorageDetail",
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
    var getDataKO = function (pageIndex, oId) {
        var list = km.fromJS(getData(pageIndex, oId));
        return list;
    }

    var vmCode = function (oId) {
        var self = this;
        self.vmDataList = ko.observableArray();

        self.SearchData = function () {
            var list = getDataKO(1, oId);
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
        dialog.close(this);
    }
    vmCode.prototype.initMenus = function (id) {
        return this.vmDataList();
    }
    var vmObj;
    vmCode.show = function (oId) {
        id = oId;
        vmObj = new vmCode(oId);
        vmObj.vmDataList = getDataKO(1, oId);
        return dialog.show(vmObj);
    };
    return vmCode;
})