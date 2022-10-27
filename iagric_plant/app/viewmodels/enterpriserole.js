define(['plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'logininfo', './enterpriserole_add', './enterpriserole_edit', 'plugins/dialog', './enterpriseuser_edit', 'utils'],
function (router, ko, km, jq, loginInfo, enterpriserole_add, enterpriserole_edit, dialog, enterpriseuser_edit, utils) {
    var name = ko.observable('');
    var moduleInfo = {
        moduleID: '10300',
        parentModuleID: '10001'
    }
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            name: vm.name()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_Role/Index",
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

    var seleteRole = function (pageIndex) {
        var list = getDataKO(pageIndex);
        updateData(list);
    }

    var addRole = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        enterpriserole_add.show().then(function () {
            seleteRole(1);
        });
    }

    var deleteRole = function (id) {
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: ko.utils.unwrapObservable(id)
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Enterprise_Role/Del",
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
                                    var currentPageRow = vm.enterpriseRole.ObjList().length;
                                    var pageIndex = vm.enterpriseRole.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.enterpriseRole.pageIndex() - 1;
                                    }
                                    seleteRole(pageIndex);

                                    this.pageSize = parseInt(vm.enterpriseRole.pageSize());
                                    this.totalCounts = parseInt(vm.enterpriseRole.totalCounts());
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

    var editRole = function (id) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        //        router.navigate('#enterpriseRole_Edit');
        enterpriserole_edit.show(id).then(function () {
            seleteRole(vm.enterpriseRole.pageIndex());
        });
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.enterpriseRolePager = {
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
                        this.pageSize = parseInt(vm.enterpriseRole.pageSize());
                        this.totalCounts = parseInt(vm.enterpriseRole.totalCounts());
                    }
                }
            });
        }
    };

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.enterpriseRole.ObjList(list.ObjList());
        vm.enterpriseRole.pageSize(list.pageSize());
        vm.enterpriseRole.totalCounts(list.totalCounts());
        vm.enterpriseRole.pageIndex(list.pageIndex());
    }

    var mouseoverFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Enterprise_Role_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_Role_ID());

        btnDel.css({ "display": "" });
        btnEdit.css({ "display": "" });

    }

    var mouseoutFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.Enterprise_Role_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_Role_ID());

        btnDel.css({ "display": "none" });
        btnEdit.css({ "display": "none" });


    }

    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.enterpriseRole = getDataKO(1);
        },
        goBack: function () {
            router.navigateBack();
        },
        enterpriseRole: null,
        seleteRole: seleteRole,
        addRole: addRole,
        deleteRole: deleteRole,
        name: name,
        editRole: editRole,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});