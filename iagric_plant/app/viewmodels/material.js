define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', './material_add', './material_edit', 'plugins/dialog', 'utils', 'logininfo', './ewm_view', 'jquery.fileDownload', './uploadexcel', './uploadexcelqt'],
function (router, ko, km, $, jq, material_add, material_edit, dialog, utils, loginInfo, materialewm, jfd, uploadexcel, uploadexcelqt) {
    var searchTitle = ko.observable();
    var moduleInfo = {
        moduleID: '10200',
        parentModuleID: '10001'
    }

    //ajax获取数据
    var getData = function (pageIndex, materialName) {
        var sendData = {
            pageIndex: pageIndex,
            materialName: materialName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Material/Index",
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
    var showCodeInfo = function (ewm, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var ewm = ko.utils.unwrapObservable(ewm);
        materialewm.show(ewm);
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
    var addMaterial = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        material_add.show().then(function () {
            searchTitle("");
            var list = getDataKO(1, "");
            updateData(list);
        });
    }
    /***********************跳转形式添加产品***************************/
    var materialAdd = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        router.navigate('#material_add');
    }
    var editMaterial = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        router.navigate('#material_edit?materialId=' + id);
    };
    /***********************产品导出Excel***************************/
    var exportExcel = function (data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
            return;
        };
        $.fileDownload('/Admin_Material/ExportTxt')
                  .done(function () { alert('导出成功'); })
                       .fail(function () { alert('导出失败!'); });
        return;
    }
    //导入Excel
    var interExcel = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        uploadexcel.show().then(function () {
        });
    }
    //其他企业导入Excel
    var qtinterExcel = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        uploadexcelqt.show().then(function () {
        });
    }
    //    var editMaterial = function (id, data, event) {
    //        var currentObj = $(event.target);
    //        currentObj.blur();
    //        var jsonResult = loginInfo.isLoginTimeoutForServer();
    //        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
    //            return;
    //        };

    //        var id = ko.utils.unwrapObservable(id);
    //        material_edit.show(id).then(function () {
    //            var list = getDataKO(vm.vmMaterial.pageIndex(), searchTitle());
    //            updateData(list);
    //        });
    //    };
    var delMaterial = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除该产品吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Material/Delete",
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
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
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
        addMaterial: addMaterial,
        editMaterial: editMaterial,
        delMaterial: delMaterial,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        showCodeInfo: showCodeInfo,
        materialAdd: materialAdd,
        exportExcel: exportExcel,
        interExcel: interExcel,
        qtinterExcel: qtinterExcel
    }
    return vm;
});