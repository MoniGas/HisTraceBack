define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './brand_add', './brand_edit', 'plugins/dialog', 'utils', './recommend_add'],
    function (router, ko, km, $, jq, loginInfo, brand_add, brand_edit, dialog, utils, recommend_add) {

        var searchTitle = ko.observable();

        var searchRecommend = function (data, event) {
            var list = getDataKO(1);
            updateData(list);
        };

        var getDataKO = function (pageIndex) {
            var list = km.fromJS(getData(pageIndex));
            return list;
        }

        var getData = function (pageIndex) {
            var sendData = {
                pageIndex: pageIndex,
                name: searchTitle()
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Recommend/GetList",
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
                            var list = getDataKO(num, searchTitle());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmRecommend.pageSize());
                            this.totalCounts = parseInt(vm.vmRecommend.totalCounts());
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
        var addRecommend = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            if (isChecked()) {
                recommend_add.show().then(function () {
                    searchTitle('');
                    var list = getDataKO(1, "");
                    updateData(list);
                });
            }
            else {
                dialog.showMessage("请先开通推荐产品服务！", '系统提示', [{ title: '确定', callback: function () { } }]);
            }
        }
        var delRecommend = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定取消推荐该商品吗？", '系统提示', [
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
                                            var list = getDataKO(pageIndex, searchTitle());
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
        var isChecked = function () {
            var result = false;
            var sendData = {
                switchCode: 1
            }
            $.ajax({
                type: "POST",
                url: "/EnterpriseSwitch/GetIsOn",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    }
                    else {
                        result = jsonResult.ObjModel != null;
                    }
                }
            });
            return result;
        }
        var check = ko.observable(false);
        var openRecommend = function () {
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var currentObj = $(event.target);
            currentObj.blur();
            var type = $('#chkIsOn').is(':checked') ? 1 : 2;
            var strType = type == 1 ? "确定开通产品推广吗？" : "关闭产品推广将清除所有已推广的产品，您确定要关闭产品推广吗？";
            dialog.showMessage(strType, '系统提示', [
                {
                    title: '确定',
                    callback: function () {
                        var sendData = {
                            type: type,
                            switchCode: 1
                        }
                        $.ajax({
                            type: "POST",
                            url: "/EnterpriseSwitch/TrunSwitch",
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
                                            if (type == 1) {
                                                searchTitle('');
                                                var list = getDataKO(1, "");
                                                updateData(list);
                                                $("input[name='chkIsOn']").prop("checked", true);
                                            }
                                            else {
                                                $("input[name='chkIsOn']").prop("checked", false);
                                            }
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
        }
        var vm = {
            binding: function () {
                vm.vmRecommend = getDataKO(1);
                updateData(vm.vmRecommend);
                var value = isChecked();
                $("input[name='chkIsOn']").prop("checked", value);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmRecommend: null,
            searchTitle: searchTitle,
            searchRecommend: searchRecommend,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            delRecommend: delRecommend,
            addRecommend: addRecommend,
            isChecked: isChecked,
            openRecommend: openRecommend,
            check: check
        }
        return vm;
    });