define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './formula_add', './formula_edit', 'plugins/dialog', 'utils'],
    function (router, ko, km, $, jq, loginInfo, formula_add, formula_edit, dialog, utils) {
        var searchTitle = ko.observable();
        //ajax获取数据
        var getData = function (pageIndex) {
            var sendData = {
                pageIndex: pageIndex,
                name: searchTitle()
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Formula/GetList",
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
        var getDataKO = function (pageIndex) {
            var list = km.fromJS(getData(pageIndex));
            return list;
        }
        //分页、搜索时更新数据源
        var updateData = function (list) {
            vm.vmFormula.ObjList(list.ObjList());
            vm.vmFormula.pageSize(list.pageSize());
            vm.vmFormula.totalCounts(list.totalCounts());
            vm.vmFormula.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.formulaPager = {
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
                            var list = getDataKO(num);
                            updateData(list);
                            this.pageSize = parseInt(vm.vmFormula.pageSize());
                            this.totalCounts = parseInt(vm.vmFormula.totalCounts());
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
        //搜索配方
        var SearchFormula = function (data, event) {
            var list = getDataKO(1);
            updateData(list);
        };
        //添加配方
        var AddFormula = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            router.navigate('#formula_new');
            //            formula_add.show().then(function () {
            //                SearchFormula();
            //            });
        };
        //修改配方
        var EditFormula = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            router.navigate('#formula_edit?mainId=' + data.FormulaID() + '&materialId=' + data.MaterialID() + '&formulaName=' + data.FormulaName() + '&spec=' + data.Spec() + '&addDate=' + data.AddTime());
            //            var currentObj = $(event.target);
            //            currentObj.blur();
            //            var jsonResult = loginInfo.isLoginTimeoutForServer();
            //            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            //                return;
            //            };
            //            formula_edit.show(data.FormulaID(), data.MaterialID(), data.FormulaName()).then(function () {
            //                SearchFormula();
            //            });
        };
        //删除配方
        var DelFormula = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定要删除该配方吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        formulaId: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Formula/Del",
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
                                        var currentPageRow = vm.vmFormula.ObjList().length;
                                        var pageIndex = vm.vmFormula.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmFormula.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex);
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmFormula.pageSize());
                                        this.totalCounts = parseInt(vm.vmFormula.totalCounts());
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
                vm.vmFormula = getDataKO(1);
                updateData(vm.vmFormula);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmFormula: null,
            searchTitle: searchTitle,
            SearchFormula: SearchFormula,
            AddFormula: AddFormula,
            EditFormula: EditFormula,
            DelFormula: DelFormula,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun
        }
        return vm;
    }
);