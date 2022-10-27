define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils', './material_spec_add' , './material_spec_edit'],
function (router, ko, km, $, jq, loginInfo, dialog, utils, material_spec_add, material_spec_edit) {
    var vmMaterialSpecModels = {
        AdvertModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }

    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            materialName: searchTitle()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Material_Spec/Index",
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
        vm.vmMaterialSpec.ObjList(list.ObjList());
        vm.vmMaterialSpec.pageSize(list.pageSize());
        vm.vmMaterialSpec.totalCounts(list.totalCounts());
        vm.vmMaterialSpec.pageIndex(list.pageIndex());
    }
    //搜索
    var searchMaterialSpec = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    //添加产品规格
    var addMaterialSpec = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        material_spec_add.show().then(function () {
            searchMaterialSpec();
        });
    };
    //编辑产品规格
    var editMaterialSpec = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        material_spec_edit.show(id).then(function (materialName, materialSpecification, price, type) {
            if (type == 2) {
                data.MaterialFullName(materialName);
                data.MaterialSpecification(materialSpecification);
                data.Price(price);
            }
            searchMaterialSpec();
        });
    };
    //删除规格
    var delMaterialSpec = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Material_Spec/Delete",
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
                                        var currentPageRow = vm.vmMaterialSpec.ObjList().length;
                                        var pageIndex = vm.vmMaterialSpec.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmMaterialSpec.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmMaterialSpec.pageSize());
                                        this.totalCounts = parseInt(vm.vmMaterialSpec.totalCounts());
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
    //自定义绑定-分页控件
    ko.bindingHandlers.materialSpecPager = {
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
                        this.pageSize = parseInt(vm.vmMaterialSpec.pageSize());
                        this.totalCounts = parseInt(vm.vmMaterialSpec.totalCounts());
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
            //初始化brand列表数据
            vm.vmMaterialSpec = getDataKO(1);
            updateData(vm.vmMaterialSpec);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmMaterialSpec: null,
        searchTitle: searchTitle,
        searchMaterialSpec: searchMaterialSpec,
        addMaterialSpec: addMaterialSpec,
        editMaterialSpec: editMaterialSpec,
        delMaterialSpec: delMaterialSpec,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});