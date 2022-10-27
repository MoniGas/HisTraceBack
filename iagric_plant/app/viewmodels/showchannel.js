define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', "./showchannel_add", "./showchannel_edit", 'plugins/dialog', 'utils','logininfo'],
function (router, ko, km, $, jq, showchannel_add, showchannel_edit, dialog, utils,loginInfo) {
    var moduleInfo = {
        moduleID: '10700',
        parentModuleID: '10002'
    }
    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, channelName) {
        var sendData = {
            pageIndex: pageIndex,
            channelName: channelName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_ShowChannel/Index",
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
        })
        return data;
    }
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmShowChannel.ObjList(list.ObjList());
        vm.vmShowChannel.pageSize(list.pageSize());
        vm.vmShowChannel.totalCounts(list.totalCounts());
        vm.vmShowChannel.pageIndex(list.pageIndex());
    }
    var searchShowChannel = function (data, event) {
        var list = getDataKO(1, searchTitle());
        updateData(list);
    };
    var addShowChannel = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        showchannel_add.show().then(function () {
            searchTitle('');
            var list = getDataKO(1, "");
            updateData(list);
        });
    };
    var editShowChannel = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        showchannel_edit.show(id).then(function () {
            var list = getDataKO(vm.vmShowChannel.pageIndex(), searchTitle());
            updateData(list);
        });
    };
    var delShowChannel = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除该栏目吗？", '系统提示', [{
            title: '确定',
            callback: function () {
                var sendData = {
                    id: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowChannel/Del",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{
                            title: '确定',
                            callback: function () {
                                if (jsonResult.code != 0) {
                                    var currentPageRow = vm.vmShowChannel.ObjList().length;
                                    var pageIndex = vm.vmShowChannel.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmShowChannel.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex, searchTitle());
                                    updateData(list);
                                    this.pageSize = parseInt(vm.vmShowChannel.pageSize());
                                    this.totalCounts = parseInt(vm.vmShowChannel.totalCounts());
                                }
                            }
                        }]);
                    }
                })
            }
        }, {
            title: '取消',
            callback: function () {
            }
        }
        ])
    };

    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, channelName) {
        var list = km.fromJS(getData(pageIndex, channelName));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.channelPager = {
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
                visiblePages: 10,
                currentPage: pageIndex,
                first: '<li class="first"><a href="javascript:;">首页</a></li>',
                last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                onPageChange: function (num, type) {
                    if (type == 'change') {
                        var list = getDataKO(num, searchTitle());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmShowChannel.pageSize());
                        this.totalCounts = parseInt(vm.vmShowChannel.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");

        ShowHand.css({ "display": "" });
    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");

        ShowHand.css({ "display": "none" });
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化dealer列表数据
            vm.vmShowChannel = getDataKO(1, "");
            updateData(vm.vmShowChannel);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmShowChannel: null,
        searchTitle: searchTitle,
        searchShowChannel: searchShowChannel,
        addShowChannel: addShowChannel,
        editShowChannel: editShowChannel,
        delShowChannel: delShowChannel,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});