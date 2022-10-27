define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', "./sys_supervisionadd", "./sys_supervisionedit", 'utils', 'logininfo'],
function (dialog, router, ko, km, jq, sys_supervisionadd, sys_supervisionedit, utils, loginInfo) {
    var moduleInfo = {
        moduleID: '10080',
        parentModuleID: '10001'
    }
    //添加监管部门
    var addSupervision = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        sys_supervisionadd.show().then(function () {
            name('');
            selStatus(0);
            var list = getDataKO(1);
            updateData(list);
        });
    }

    //编辑监管部门
    var editSupervision = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        sys_supervisionedit.show(id).then(function () {
            var list = getDataKO(1);
            updateData(list);
        });
    }

    var name = ko.observable('');
    var selStatus = ko.observable(0);
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            sName: name(),
            selStatus: selStatus()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysSupervision/Index",
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

    //搜索
    var searchSupervision = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmSupervision.ObjList(list.ObjList());
        vm.vmSupervision.pageSize(list.pageSize());
        vm.vmSupervision.totalCounts(list.totalCounts());
        vm.vmSupervision.pageIndex(list.pageIndex());
    }

    //禁用监管部门
    var disableSupervision = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        var sendData = {
            userId: id
        }
        $.ajax({
            type: "POST",
            url: "/SysSupervision/DisableSupervision",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                }
                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                    if (jsonResult.code == 1) {
                        var list = getDataKO(1);
                        updateData(list);
                    }
                }
                }]);
            }
        })
    }

    //启用监管部门
    var enableSupervision = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        var sendData = {
            userId: id
        }
        $.ajax({
            type: "POST",
            url: "/SysSupervision/EnableSupervision",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                }
                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                    if (jsonResult.code == 1) {
                        var list = getDataKO(1);
                        updateData(list);
                    }
                }
                }]);
            }
        })
    }

    //重置密码
    var resetPassword = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        var sendData = {
            userId: id
        }
        $.ajax({
            type: "POST",
            url: "/SysSupervision/ResetPassword",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                }
                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {

                }
                }]);
            }
        })
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
                        var list = getDataKO(num);
                        updateData(list);
                        this.pageSize = parseInt(vm.vmSupervision.pageSize());
                        this.totalCounts = parseInt(vm.vmSupervision.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        // 修改
        var btnEdit = $("#btnEdit_" + data.PRRU_PlatForm_User_ID());
        // 修改
        var btnDisable = $("#btnDisable_" + data.PRRU_PlatForm_User_ID());
        // 修改
        var btnEnable = $("#btnEnable_" + data.PRRU_PlatForm_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.PRRU_PlatForm_User_ID());
        switch (data.UserStatus()) {
            case -2:
                btnDisable.css({ "display": "none" });
                btnEnable.css({ "display": "" });
                break;
            case 1:
                btnDisable.css({ "display": "" });
                btnEnable.css({ "display": "none" });
                break;
        }
        btnEdit.css({ "display": "" });
        btnReset.css({ "display": "" });
    }

    var mouseoutFun = function (data, event) {
        // 修改
        var btnEdit = $("#btnEdit_" + data.PRRU_PlatForm_User_ID());
        // 修改
        var btnDisable = $("#btnDisable_" + data.PRRU_PlatForm_User_ID());
        // 修改
        var btnEnable = $("#btnEnable_" + data.PRRU_PlatForm_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.PRRU_PlatForm_User_ID());

        btnEdit.css({ "display": "none" });
        btnDisable.css({ "display": "none" });
        btnEnable.css({ "display": "none" });
        btnReset.css({ "display": "none" });


    }

    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmSupervision = getDataKO(1);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmSupervision: null,
        addSupervision: addSupervision,
        searchSupervision: searchSupervision,
        name: name,
        selStatus: selStatus,
        editSupervision: editSupervision,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        disableSupervision: disableSupervision,
        enableSupervision: enableSupervision,
        resetPassword: resetPassword
    }
    return vm;

});