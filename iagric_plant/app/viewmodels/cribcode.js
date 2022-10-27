define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './cribcodeadd', './cribcodeedit', 'plugins/dialog', 'utils', 'logininfo', './ewm_view'],
    function (router, ko, km, $, jq, cribcodeadd, cribcodeedit, dialog, utils, loginInfo, ewm_view) {
        var moduleInfo = {
            moduleID: '61400',
            parentModuleID: '61000'
        }
        var searchTitle = ko.observable();

        //ajax获取数据
        var getData = function (pageIndex, cribName) {
            var sendData = {
                cribName: cribName,
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Store/GetCribList",
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
            //alert(list.pageIndex());
            vm.vmCribCode.ObjList(list.ObjList());
            vm.vmCribCode.pageSize(list.pageSize());
            vm.vmCribCode.totalCounts(list.totalCounts());
            vm.vmCribCode.pageIndex(list.pageIndex());
        }
        //搜索垛位
        var searchCribCode = function (data, event) {
            var list = getDataKO(1, searchTitle());
            updateData(list);
        };
        //添加垛位
        var addCribCode = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            cribcodeadd.show().then(function () {
                searchTitle('');
                var list = getDataKO(1, "");
                updateData(list);
            });
        };
        //编辑垛位
        var editCribCode = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            cribcodeedit.show(id).then(function () {
                var list = getDataKO(vm.vmCribCode.pageIndex(), searchTitle());
                updateData(list);
            });
        };

        //查看垛位码
        var showCodeInfo = function (ewm) {
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var ewm = ko.utils.unwrapObservable(ewm);
            ewm_view.show(ewm);
        }
     
        var delCribCode = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定删除该仓库吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Store/Delete",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code != 0) {
                                        var currentPageRow = vm.vmCribCode.ObjList().length;
                                        var pageIndex = vm.vmCribCode.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmCribCode.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmCribCode.pageSize());
                                        this.totalCounts = parseInt(vm.vmCribCode.totalCounts());
                                    }
                                }
                                }]);
                            }
                        }

                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, cribName) {
            var list = km.fromJS(getData(pageIndex, cribName));
            return list;
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.dealerPager = {
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
                            this.pageSize = parseInt(vm.vmCribCode.pageSize());
                            this.totalCounts = parseInt(vm.vmCribCode.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "none" });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmCribCode = getDataKO(1, "");
                updateData(vm.vmCribCode);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmCribCode: null,
            searchTitle: searchTitle,
            searchCribCode: searchCribCode,
            addCribCode: addCribCode,
            editCribCode: editCribCode,
            delCribCode: delCribCode,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            showCodeInfo: showCodeInfo
        }
        return vm;
    });