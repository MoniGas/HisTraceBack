define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', "./greenhouse_add", "./greenhouse_edit", 'utils', 'logininfo', './ewm_view'],
function (dialog, router, ko, km, jq, greenhouse_add, greenhouse_edit, utils, loginInfo, ewm_view) {
    var moduleInfo = {
        moduleID: '10080',
        parentModuleID: '10001'
    }
    //添加生产基地
    var addGreenhouse = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        greenhouse_add.show().then(function () {
            ewm('');
            name('');
            var list = getDataKO(1, name(), ewm());
            updateData(list);
        });
    }

    //编辑生产基地
    var editGreenhouse = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        greenhouse_edit.show(id).then(function (name) {
            if (name != undefined) {
                data.GreenhousesName(name);
            }
        });
    }

    var name = ko.observable();
    var ewm = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, name, ewm) {
        var sendData = {
            pageIndex: pageIndex,
            greenName: name,
            greenewm: ewm
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Greenhouse/Index",
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
    var getDataKO = function (pageIndex, name, ewm) {
        var list = km.fromJS(getData(pageIndex, name, ewm));
        return list;
    }

    //搜索生产基地
    var searchGreenhouse = function (data, event) {
        var list = getDataKO(1, name(), ewm());
        updateData(list);
    };

    //刷新生产基地
    var refreshGreenhouse = function (data, event) {
        location.reload();
    };

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmGreenhouse.ObjList(list.ObjList());
        vm.vmGreenhouse.pageSize(list.pageSize());
        vm.vmGreenhouse.totalCounts(list.totalCounts());
        vm.vmGreenhouse.pageIndex(list.pageIndex());
    }

    //删除生产基地
    var delGreenhouse = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    operationTypeId: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Greenhouse/Del",
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
                                    var currentPageRow = vm.vmGreenhouse.ObjList().length;
                                    var pageIndex = vm.vmGreenhouse.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmGreenhouse.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex, name(), ewm());
                                    updateData(list);

                                    this.pageSize = parseInt(vm.vmGreenhouse.pageSize());
                                    this.totalCounts = parseInt(vm.vmGreenhouse.totalCounts());
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

    //自定义绑定-分页控件
    ko.bindingHandlers.greenhousePagerBH = {
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
                        var list = getDataKO(num, name(), ewm());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmGreenhouse.pageSize());
                        this.totalCounts = parseInt(vm.vmGreenhouse.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Greenhouses_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Greenhouses_ID());

        // 查看码信息
        var btnSee = $("#btnSee_" + data.Greenhouses_ID());

        btnDel.css({ "display": "" });
        btnEdit.css({ "display": "" });
        btnSee.css({ "display": "" });

    }

    var mouseoutFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Greenhouses_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Greenhouses_ID());

        // 查看码信息
        var btnSee = $("#btnSee_" + data.Greenhouses_ID());

        btnDel.css({ "display": "none" });
        btnEdit.css({ "display": "none" });
        btnSee.css({ "display": "none" });


    }

    var showCodeInfo = function (ewm) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var ewm = ko.utils.unwrapObservable(ewm);
        ewm_view.show(ewm);
    }

    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmGreenhouse = getDataKO(1, "", "");
        },
        goBack: function () {
            router.navigateBack();
        },
        vmGreenhouse: null,
        addGreenhouse: addGreenhouse,
        searchGreenhouse: searchGreenhouse,
        name: name,
        ewm: ewm,
        refreshGreenhouse: refreshGreenhouse,
        delGreenhouse: delGreenhouse,
        editGreenhouse: editGreenhouse,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        showCodeInfo: showCodeInfo

    }
    return vm;

});