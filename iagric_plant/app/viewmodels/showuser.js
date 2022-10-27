define(['plugins/router', 'plugins/dialog', 'knockout', 'knockout.mapping', 'jqPaginator', 'logininfo', './showuser_add', './showuser_edit', './ewm_view', 'utils'],
function (router, dialog, ko, km, jq, loginInfo, showuser_add, showuser_edit, ewm_view, utils) {
    var moduleInfo = {
        moduleID: '11000',
        parentModuleID: '10001'
    }
    var name = ko.observable();

    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            name: name()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_ShowUser/GetList",
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

    var searchUser = function () {
        var list = getDataKO(1);
        updateData(list);
    }

    var addUser = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        //        router.navigate('#showuser_add');
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        showuser_add.show().then(function () {
            var list = getDataKO(1);
            updateData(list);
        });
    }

    var showInfo = function (id) {

    }

    var editInfo = function (id, data, event) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        //        router.navigate('#showuser_edit?id=' + id);
        showuser_edit.show(id).then(function () {
            var list = getDataKO(vm.vmUser.pageIndex());
            updateData(list);
        });
    }

    var deleteInfo = function (id, data, event) {
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: ko.utils.unwrapObservable(id)
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowUser/Del",
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
                                    var currentPageRow = vm.vmUser.ObjList().length;
                                    var pageIndex = vm.vmUser.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.vmUser.pageIndex() - 1;
                                    }
                                    var list = getDataKO(pageIndex);
                                    updateData(list);
                                    this.pageSize = parseInt(vm.vmUser.pageSize());
                                    this.totalCounts = parseInt(vm.vmUser.totalCounts());

                                }
                            }
                            }]);
                        }
                    }
                })
            }
        },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
    }

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmUser.ObjList(list.ObjList());
        vm.vmUser.pageSize(list.pageSize());
        vm.vmUser.totalCounts(list.totalCounts());
        vm.vmUser.pageIndex(list.pageIndex());
    }

    var showCodeInfo = function (ewm) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var ewm = ko.utils.unwrapObservable(ewm);
        ewm_view.show(ewm,1);
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
        var btnSee = $("#btnSee_" + data.UserID());
        var btnEdit = $("#btnEdit_" + data.UserID());
        var btnDel = $("#btnDel_" + data.UserID());
        var btnSeeCode = $("#btnSeeCode_" + data.UserID());

        btnSee.css({ "display": "none" });
        btnEdit.css({ "display": "" });
        btnDel.css({ "display": "" });
        btnSeeCode.css({ "display": "" });

    }

    var mouseoutFun = function (data, event) {
        var btnSee = $("#btnSee_" + data.UserID());
        var btnEdit = $("#btnEdit_" + data.UserID());
        var btnDel = $("#btnDel_" + data.UserID());
        var btnSeeCode = $("#btnSeeCode_" + data.UserID());

        btnSee.css({ "display": "none" });
        btnEdit.css({ "display": "none" });
        btnDel.css({ "display": "none" });
        btnSeeCode.css({ "display": "none" });
    }

    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.vmUser = getDataKO(1);
        },
        goBack: function () {
            router.navigateBack();
        },
        name: name,
        vmUser: null,
        searchUser: searchUser,
        addUser: addUser,
        showInfo: showInfo,
        editInfo: editInfo,
        deleteInfo: deleteInfo,
        showCodeInfo: showCodeInfo,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});