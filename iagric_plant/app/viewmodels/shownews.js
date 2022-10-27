define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', './shownews_add', './shownews_edit', 'utils','logininfo'],
function (router, ko, km, $, jq, dialog, shownews_add, shownews_edit, utils,loginInfo) {
    var moduleInfo = {
        moduleID: '10800',
        parentModuleID: '10001'
    }
    var searchTitle = ko.observable();

    var getData = function (pageIndex /*,channelId, title*/) {
        var sendData = {
            pageIndex: pageIndex,
            title: searchTitle()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_ShowNews/Index",
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
        vm.vmShowNews.ObjList(list.ObjList());
        vm.vmShowNews.pageSize(list.pageSize());
        vm.vmShowNews.totalCounts(list.totalCounts());
        vm.vmShowNews.pageIndex(list.pageIndex());
    }
    var searchShowNews = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    var addShowNews = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        shownews_add.show().then(function () {
            searchTitle('');
            var list = getDataKO(1, 0, searchTitle());
            updateData(list);
        });
    };
    var editShowNews = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        shownews_edit.show(id).then(function () {
            var list = getDataKO(vm.vmShowNews.pageIndex(), searchTitle());
            updateData(list);
        });
    };

    var delShowNews = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除该新闻吗？", '系统提示', [{
            title: '确定',
            callback: function () {
                var sendData = {
                    id: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowNews/Del",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {

                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        }

                        dialog.showMessage(jsonResult.Msg, '系统提示', [{
                            title: '确定',
                            callback: function () {
                                if (jsonResult.code != 0) {
                                    var currentPageRow = vm.vmShowNews.ObjList().length;
                                    var pageIndex = vm.vmShowNews.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmShowNews.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex, searchTitle());
                                    updateData(list);

                                    this.pageSize = parseInt(vm.vmShowNews.pageSize());
                                    this.totalCounts = parseInt(vm.vmShowNews.totalCounts());
                                }
                            }
                        }]);
                    }
                });
            }
        }, {
            title: '取消',
            callback: function () {
            }
        }
        ])
    };

    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex ) {
        var list = km.fromJS(getData(pageIndex));
        return list;
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.newsPager = {
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
                        this.pageSize = parseInt(vm.vmShowNews.pageSize());
                        this.totalCounts = parseInt(vm.vmShowNews.totalCounts());
                    }
                }
            });
        }
    };
    var gethannel = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_ShowChannel/Index",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
            },
            error: function (Error) {
                alert(Error);
            }
        })
        return data;
    }

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
            vm.vmShowNews = getDataKO(1, 0, "");
            updateData(vm.vmShowNews);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmShowNews: null,
        searchTitle: searchTitle,
        searchShowNews: searchShowNews,
        addShowNews: addShowNews,
        editShowNews: editShowNews,
        delShowNews: delShowNews,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});