define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'],
function (dialog, ko, jqueryui, km, utils, loginInfo) {

    var id = 0;
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
        }
    };

    var getData = function (pageIndex, ewm, rId, status) {
        var sendData = {
            ewm: ewm,
            rId: rId,
            status: status,
            pageIndex: pageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Request/SearchCode",
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
    var getDataKO = function (pageIndex, ewm, rId, status) {
        var list = km.fromJS(getData(pageIndex, ewm, rId, status));
        return list;
    }

    var vmCode = function (rId) {
        var self = this;
        self.path = ko.observable();
        self.ewm = ko.observable('');
        self.vmDataList = ko.observableArray();

        self.ViewType = function (Type) {
            Type = ko.utils.unwrapObservable(Type);
            if (Type == 1) {
                return "套标组码";
            } else if (Type == 2) {
                return "套标子码";
            } else if (Type == 3) {
                return "单品码";
            }
        }

        self.status = function (index) {
            var index = ko.utils.unwrapObservable(index);
            switch (index) {
                case 1040000008:
                    return "未销售";
                case 1040000009:
                    return "已销售";
            }
        }

        self.SearchData = function () {
            var list = getDataKO(1, self.ewm(), rId, self.vmModels.selectedOption());
            self.updateData(list);
        }

        //分页、搜索时更新数据源
        self.updateData = function (list) {
            self.vmDataList.ObjList(list.ObjList());
            self.vmDataList.pageSize(list.pageSize());
            self.vmDataList.totalCounts(list.totalCounts());
            self.vmDataList.pageIndex(list.pageIndex());
        }

        self.vmModels = {
            statusArray: ko.observableArray(),
            selectedOption: ko.observable(0)
        }

        self.vmModels.statusArray([{ name: "全部", id: 0 }, { name: "未销售", id: 1040000008 }, { name: "已销售", id: 1040000009}]);

//        self.mouseoverFun = function (data, event) {
//            var tr = $(event.target).closest('tr');
//            var ShowCode = tr.find("span[eleflag='ShowCode']");
//            self.path("/ShowImage/ShowImg?ewm=" + ShowCode[0].textContent);

//            var objDiv = $("#mydiv1");
//            $(objDiv).css("display", "block");
//            $(objDiv).css("left", 175);
//            $(objDiv).css("top", event.offsetX + 10);
//        }

//        self.mouseoutFun = function (data, event) {
//            var self = $(event.target).closest('tr');
//            var ShowCode = self.find("button[eleflag='ShowCode']");

//            var objDiv = $("#mydiv1");
//            $(objDiv).css("display", "none");
//        }
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
        vmObj.vmDataList = getDataKO(1, vmObj.ewm(), rId, vmObj.vmModels.selectedOption());
        return dialog.show(vmObj);
    };
    return vmCode;
})