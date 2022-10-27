define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './material_add', './material_edit', 'plugins/dialog','logininfo'],
function (router, ko, km, $, jq, material_add, material_edit, dialog,loginInfo) {
    var searchTitle = ko.observable();
    var moduleInfo = {
        moduleID: '17000',
        parentModuleID: '10001'
    }
    //ajax获取数据
    var getData = function (pageIndex, materialName) {
        var sendData = {
            pageIndex: pageIndex,
            search: materialName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Complaint/Index",
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
        vm.vmMaterial.ObjList(list.ObjList());
        vm.vmMaterial.pageSize(list.pageSize());
        vm.vmMaterial.totalCounts(list.totalCounts());
        vm.vmMaterial.pageIndex(list.pageIndex());
    }
    //搜索经销商
    var searchMaterial = function (data, event) {
        var list = getDataKO(1, searchTitle());
        updateData(list);
    };
    var delMaterial = function (id, data, event) {
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除该条投诉信息吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Complaint/Delete",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                if (jsonResult.code != 0) {
                                    var currentPageRow = vm.vmMaterial.ObjList().length;
                                    var pageIndex = vm.vmMaterial.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmMaterial.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex, searchTitle());
                                    updateData(list);
                                    this.pageSize = parseInt(vm.vmMaterial.pageSize());
                                    this.totalCounts = parseInt(vm.vmMaterial.totalCounts());
                                }
                            }
                            }]);
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
    var getDataKO = function (pageIndex, materialName) {
        var list = km.fromJS(getData(pageIndex, materialName));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.materialPager = {
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
                        this.pageSize = parseInt(vm.vmMaterial.pageSize());
                        this.totalCounts = parseInt(vm.vmMaterial.totalCounts());
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
            vm.vmMaterial = getDataKO(1, "");
            updateData(vm.vmMaterial);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmMaterial: null,
        searchTitle: searchTitle,
        searchMaterial: searchMaterial,
        delMaterial: delMaterial,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});