define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo', './sys_recommend_check'],
    function (dialog, router, ko, km, jq, utils, loginInfo, sys_recommend_check) {
        var name = ko.observable();
        var moduleInfo = {
            moduleID: '11150',
            parentModuleID: '0'
        }
        //ajax获取数据
        var getData = function (pageIndex, name) {
            var sendData = {
                pageIndex: pageIndex,
                name: name,
                type: 2
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/SysRecommend/GetList",
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

        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, name) {
            var list = km.fromJS(getData(pageIndex, name));
            return list;
        }

        //搜索
        var searchEnterprise = function (data, event) {
            var list = getDataKO(1, name());
            updateData(list);
        };

        //分页、搜索时更新数据源
        var updateData = function (list) {
            vm.vmRecommend.ObjList(list.ObjList());
            vm.vmRecommend.pageSize(list.pageSize());
            vm.vmRecommend.totalCounts(list.totalCounts());
            vm.vmRecommend.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.recommendPager = {
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
                            var list = getDataKO(num, name());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmRecommend.pageSize());
                            this.totalCounts = parseInt(vm.vmRecommend.totalCounts());
                        }
                    }
                });
            }
        };
        var addRecommend = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            sys_recommend_check.show().then(function () {
                name('');
                searchEnterprise();
            });
        }

        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowCancel = self.find("button[eleflag='ShowCancel']");
            ShowCancel.css({ "display": "" });
            var ShowPreview = self.find("button[eleflag='ShowPreview']");
            ShowPreview.css({ "display": "" });
        }

        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowCancel = self.find("button[eleflag='ShowCancel']");
            ShowCancel.css({ "display": "none" });
            var ShowPreview = self.find("button[eleflag='ShowPreview']");
            ShowPreview.css({ "display": "none" });
        }

        var delRecommend = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定取消推荐该企业吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Recommend/Del",
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
                                        var currentPageRow = vm.vmRecommend.ObjList().length;
                                        var pageIndex = vm.vmRecommend.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmRecommend.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, name());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmRecommend.pageSize());
                                        this.totalCounts = parseInt(vm.vmRecommend.totalCounts());
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
        var vm = {
            binding: function () {
                //初初化导航状态
                loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化brand列表数据
                vm.vmRecommend = getDataKO(1, "");
            },
            goBack: function () {
                router.navigateBack();
            },
            vmRecommend: null,
            name: name,
            searchEnterprise: searchEnterprise,
            addRecommend: addRecommend,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            delRecommend: delRecommend
        }
        return vm;
    }
);