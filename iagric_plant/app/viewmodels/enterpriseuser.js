define(['plugins/router', 'plugins/dialog', 'knockout', 'knockout.mapping', 'jqPaginator', 'logininfo', './euserresetpass', './enterpriseuser_add', './enterpriseuser_edit', './enterpriseuser_see', "./enterpriseuser_level_see", 'utils'],
function (router, dialog, ko, km, jq, loginInfo, euserresetpass, enterpriseuser_add, enterpriseuser_edit, enterpriseuser_see, enterpriseuser_level_see, utils) {
    var moduleInfo = {
        moduleID: '10500',
        parentModuleID: '10001'
    }
    var vmUserModels = {
        newsUserArray: ko.observableArray(),
        selectedOption: ko.observable()
    }
    var userName = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            userName: vm.userName(),
            userRole: vmUserModels.selectedOption()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_User/GetList",
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

    var getLevelUser = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_User/GetLevelUser",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                data = km.fromJS(jsonResult);
            }
        })
        return data;
    }

    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex) {
        var list = km.fromJS(getData(pageIndex));
        return list;
    }

    var searchUser = function (pageIndex) {
        var list = getDataKO(pageIndex);
        updateData(list);
    }

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmUser.ObjList(list.ObjList());
        vm.vmUser.pageSize(list.pageSize());
        vm.vmUser.totalCounts(list.totalCounts());
        vm.vmUser.pageIndex(list.pageIndex());
    }

    // 添加用户
    var addUser = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        //        router.navigate('#enterpriseUser_Add');
        enterpriseuser_add.show().then(function () {
            searchUser(1);
            //初始化活动动态模块数据
            vmUserModels.newsUserArray(getUserModules());
        });
    }
    // 查看用户信息
    var seeUser = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        enterpriseuser_see.show(id);
    }

    // 查看用户信息
    var seeUser1 = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        enterpriseuser_level_see.show(id);
    }

    // 修改用户信息
    var editUser = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);

        //        router.navigate('#enterpriseUser_Edit?id=' + id);
        enterpriseuser_edit.show(id).then(function () {
            searchUser(vm.vmUser.pageIndex());
            //初始化活动动态模块数据
            vmUserModels.newsUserArray(getUserModules());
        });
    }
    // 修改用户密码
    var editPass = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        //alert(id);
        euserresetpass.show(id);
    }

    var getLoginInfo = function (id) {
        var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_User/GetLoginInfo",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (id == jsonResult) {
                    window.location.href = "/";
                }
            }
        });
    }

    // 重置密码
    var resetPass = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        dialog.showMessage("确定重置密码？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: ko.utils.unwrapObservable(id)
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Enterprise_User/ResetPas",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            getLoginInfo(ko.utils.unwrapObservable(id));
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
    }
    // 删除用户
    var deleteUser = function (id, data, event) {
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: ko.utils.unwrapObservable(id)
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Enterprise_User/Del",
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
                                var currentPageRow = vm.vmUser.ObjList().length;
                                var pageIndex = vm.vmUser.pageIndex();
                                if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                    pageIndex = vm.vmUser.pageIndex() - 1;
                                }
                                searchUser(pageIndex);
                                //初始化活动动态模块数据
                                vmUserModels.newsUserArray(getUserModules());
                                this.pageSize = parseInt(vm.vmUser.pageSize());
                                this.totalCounts = parseInt(vm.vmUser.totalCounts());
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

    //获取活动动态模块
    var getUserModules = function () {

        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_User/GetRoleList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
            }
        });
        return data;
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.userPager = {
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
                        this.pageSize = parseInt(vm.vmUser.pageSize());
                        this.totalCounts = parseInt(vm.vmUser.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        // 查看
        var btnSee = $("#btnSee_" + data.Enterprise_User_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_User_ID());
        // 修改密码
        var btnEditPass = $("#btnEditPass_" + data.Enterprise_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.Enterprise_User_ID());
        // 删除
        var btnDel = $("#btnDel_" + data.Enterprise_User_ID());

        btnSee.css({ "display": "" });
        btnEdit.css({ "display": "" });
        btnEditPass.css({ "display": "" });
        btnReset.css({ "display": "" });
        btnDel.css({ "display": "" });
    }

    var mouseoverFun1 = function (data, event) {
        // 查看
        var btnSee = $("#btnSee_" + data.Enterprise_User_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_User_ID());
        // 修改密码
        var btnEditPass = $("#btnEditPass_" + data.Enterprise_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.Enterprise_User_ID());
        var self = $(event.target).closest('tr');
        var userType = self.find("span[eleflag='UserType']");
        if (userType[0].textContent == "默认") {
            btnEditPass.css({ "display": "none" });
        }
        else {
            btnEditPass.css({ "display": "" });
         }
        btnSee.css({ "display": "" });
        btnEdit.css({ "display": "" });
        
        btnReset.css({ "display": "" });
    }

    var mouseoutFun = function (data, event) {
        // 查看
        var btnSee = $("#btnSee_" + data.Enterprise_User_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_User_ID());
        // 修改密码
        var btnEditPass = $("#btnEditPass_" + data.Enterprise_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.Enterprise_User_ID());
        // 删除
        var btnDel = $("#btnDel_" + data.Enterprise_User_ID());

        btnSee.css({ "display": "none" });
        btnEdit.css({ "display": "none" });
        btnEditPass.css({ "display": "none" });
        btnReset.css({ "display": "none" });
        btnDel.css({ "display": "none" });
    }

    var mouseoutFun1 = function (data, event) {
        // 查看
        var btnSee = $("#btnSee_" + data.Enterprise_User_ID());
        // 修改
        var btnEdit = $("#btnEdit_" + data.Enterprise_User_ID());
        // 修改密码
        var btnEditPass = $("#btnEditPass_" + data.Enterprise_User_ID());
        // 重置密码
        var btnReset = $("#btnReset_" + data.Enterprise_User_ID());

        btnSee.css({ "display": "none" });
        btnEdit.css({ "display": "none" });
        btnEditPass.css({ "display": "none" });
        btnReset.css({ "display": "none" });
    }

    //初始化活动动态模块数据
    vmUserModels.newsUserArray(getUserModules());
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.vmUser = getDataKO(1);
            vm.vmLevelUser = getLevelUser();
            document.onkeydown = function (e) {
                var theEvent = window.event || e;
                var code = theEvent.keyCode || theEvent.which;
                if (code == 13) {
                    $("#btnSearch").click();
                }
            }
        },
        compositionComplete: function (view) {
            //        $("#userRole").chosen({ allow_single_deselect: true });
        },
        goBack: function () {
            router.navigateBack();
        },
        vmUser: null,
        vmLevelUser: null,
        userName: userName,
        searchUser: searchUser,
        addUser: addUser,
        seeUser: seeUser,
        seeUser1: seeUser1,
        editUser: editUser,
        editPass: editPass,
        resetPass: resetPass,
        deleteUser: deleteUser,
        vmUserModels: vmUserModels,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        mouseoverFun1: mouseoverFun1,
        mouseoutFun1: mouseoutFun1
    }
    return vm;
});