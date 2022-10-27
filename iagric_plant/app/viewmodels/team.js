define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', './team_add', './teamusers', './team_edit', 'plugins/dialog', 'utils'], function (router, ko, km, $, jq, loginInfo, team_add, teamusers, team_edit, dialog, utils) {
    var moduleInfo = {
        moduleID: '37000',
        parentModuleID: '10001'
    }

    var vmTeamModels = {
        AdvertModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }

    var searchTitle = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex) {
        var sendData = {
            pageIndex: pageIndex,
            teamName: searchTitle()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Team/Index",
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
        vm.vmTeam.ObjList(list.ObjList());
        vm.vmTeam.pageSize(list.pageSize());
        vm.vmTeam.totalCounts(list.totalCounts());
        vm.vmTeam.pageIndex(list.pageIndex());
    }
    //搜索班组
    var searchTeam = function (data, event) {
        var list = getDataKO(1);
        updateData(list);
    };
    var addTeam = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        team_add.show().then(function () {
            searchTeam();
        });
    };
    var teamUsers = function (teamid, data, event) {
//        var currentObj = $(event.target);
//        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var teamid = ko.utils.unwrapObservable(teamid);
        teamusers.show(teamid).then(function () {
            searchTeam();
        });
    };
    //编辑班组信息
    var editTeam = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        team_edit.show(id).then(function (TeamName, Remark) {
//            data.TeamName(TeamName);
            //            data.Remark(Remark);
            var list = getDataKO(1);
            updateData(list);
        });
    };
    //删除
    var delTeam = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        teamId: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Team/Delete",
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
                                        var currentPageRow = vm.vmTeam.ObjList().length;
                                        var pageIndex = vm.vmTeam.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmTeam.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmTeam.pageSize());
                                        this.totalCounts = parseInt(vm.vmTeam.totalCounts());
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
    ko.bindingHandlers.teamPager = {
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
                        this.pageSize = parseInt(vm.vmTeam.pageSize());
                        this.totalCounts = parseInt(vm.vmTeam.totalCounts());
                    }
                }
            });
        }
    };
    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");
        var btnTeamUsers = $("#btnTeamUsers_" + data.TeamID());
        ShowHand.css({ "display": "" });
        btnTeamUsers.css({ "display": "" });
//        var ShowAll = self.find("button[eleflag='ShowAll']");
//        ShowAll.css({ "display": "" });
    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowHand = self.find("div[eleflag='ShowHand']");
        var btnTeamUsers = $("#btnTeamUsers_" + data.TeamID());
        ShowHand.css({ "display": "none" });
                btnTeamUsers.css({ "display": "none" });
//        var ShowAll = self.find("button[eleflag='ShowAll']");
//        ShowAll.css({ "display": "none" });
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化team列表数据
            vm.vmTeam = getDataKO(1);
            updateData(vm.vmTeam);
        },
        goBack: function () {
            router.navigateBack();
        },
        vmTeam: null,
        searchTitle: searchTitle,
        searchTeam: searchTeam,
        addTeam: addTeam,
        teamUsers: teamUsers,
        editTeam: editTeam,
        delTeam: delTeam,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});