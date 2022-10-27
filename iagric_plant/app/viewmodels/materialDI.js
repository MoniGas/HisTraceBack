define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils', './materialDIExcel'],
function (router, ko, km, $, jq, loginInfo, dialog, utils, materialDIExcel) {
    var moduleInfo = {
        moduleID: '26300',
        parentModuleID: '20000'
    }
    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            searchName: searchTitle()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/MaterialDI/Index",
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
        vm.vmMarerialDI.ObjList(list.ObjList());
        vm.vmMarerialDI.pageSize(list.pageSize());
        vm.vmMarerialDI.totalCounts(list.totalCounts());
        vm.vmMarerialDI.pageIndex(list.pageIndex());
    }
    //搜索
    var searchMaDI = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    var CodeType = loginInfo.loginInfo.ObjModel.CodeType;
    var getCreatetype = function (createtype) {
        var createtype = ko.utils.unwrapObservable(createtype);
        if (createtype == null) {
            return '客户端生成';
        }
        switch (createtype) {
            case 0:
                return "官网生成";
            case 5:
                return "Excel导入";
            default:
                return "客户端生成";
        }
    }
    var tongbuUDIDI = function (data, event) {
        //        var currentObj = $(event.target);
        //        currentObj.blur();
        dialog.showMessage("确定要同步UDI-DI吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {

                }
                $.ajax({
                    type: "POST",
                    url: "/MaterialDI/SyncUDIDI",
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
                                    var currentPageRow = vm.vmMarerialDI.ObjList().length;
                                    var pageIndex = vm.vmMarerialDI.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmMarerialDI.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex, searchTitle());
                                    updateData(list);
                                    this.pageSize = parseInt(vm.vmMarerialDI.pageSize());
                                    this.totalCounts = parseInt(vm.vmMarerialDI.totalCounts());
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
    var getDataKO = function (pageIndex) {
        var list = km.fromJS(getData(pageIndex));
        return list;
    }
    //导入Excel
    var interExcel = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        materialDIExcel.show().then(function () {
            var list = getDataKO(1);
            updateData(list);
        });
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.brandPager = {
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
                    //alert(type + '：' + num);
                    if (type == 'change') {
                        var list = getDataKO(num, searchTitle());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmMarerialDI.pageSize());
                        this.totalCounts = parseInt(vm.vmMarerialDI.totalCounts());
                    }
                }
            });
        }
    };
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmMarerialDI = getDataKO(1);
            updateData(vm.vmMarerialDI);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmMarerialDI: null,
        searchTitle: searchTitle,
        searchMaDI: searchMaDI,
        tongbuUDIDI: tongbuUDIDI,
        interExcel: interExcel,
        CodeType: CodeType,
        getCreatetype: getCreatetype
    }
    return vm;
});