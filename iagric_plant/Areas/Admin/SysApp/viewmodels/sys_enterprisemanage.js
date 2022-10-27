define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo', "./sys_addenterprise", './sys_setrequestcount', './sys_updatepassordEn'],
function (dialog, router, ko, km, jq, utils, loginInfo, sys_addenterprise, sys_setrequestcount, sys_updatepassordEn) {
    var moduleInfo = {
        moduleID: '11030',
        parentModuleID: '10000'
    }

    var name = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, name) {
        var sendData = {
            pageIndex: pageIndex,
            name: name
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysEnterpriseManage/Index",
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
    var getDataKO = function (pageIndex, name) {
        var list = km.fromJS(getData(pageIndex, name));
        return list;
    }

    //搜索生产基地
    var searchEnterprise = function (data, event) {
        var list = getDataKO(1, name());
        updateData(list);
    };

    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmEnterprise.ObjList(list.ObjList());
        vm.vmEnterprise.pageSize(list.pageSize());
        vm.vmEnterprise.totalCounts(list.totalCounts());
        vm.vmEnterprise.pageIndex(list.pageIndex());
    }

    var addEnterprise = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        sys_addenterprise.show().then(function () {
            name('');
            searchEnterprise();
        });
    }
    //设置审核码数量
    var setRequestCount = function (id, name, count, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        id = ko.utils.unwrapObservable(id);
        name = ko.utils.unwrapObservable(name);
        count = ko.utils.unwrapObservable(count);
        sys_setrequestcount.show(id, name, count).then(function () {
            searchEnterprise();
        });
    }
    //重置密码
    var setUpdatePassWord = function (id, name, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        id = ko.utils.unwrapObservable(id);
        name = ko.utils.unwrapObservable(name);
        sys_updatepassordEn.show(id, name).then(function () {
            searchEnterprise();
        });
    }
    //解析设置
    var goSetAnalysis = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        id = ko.utils.unwrapObservable(id);
        router.navigate('#sys_setAnalysis?Enterprise_Info_ID=' + id);
    }
    //跳转
    var codeStatistics = function (eid, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        eid = ko.utils.unwrapObservable(eid);
        router.navigate('#sys_codestatis?eid=' + eid);
        //router.navigate('#sys_codestatis');
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
                        var list = getDataKO(num, name());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmEnterprise.pageSize());
                        this.totalCounts = parseInt(vm.vmEnterprise.totalCounts());
                    }
                }
            });
        }
    };

    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var VerifyTest = self.find("span[eleflag='VerifyTest']");
        switch (VerifyTest[0].textContent) {
            case '审核通过':
                var ShowAll = self.find("button[eleflag='ShowAll']");
                ShowAll.css({ "display": "" });
                var ShowPause = self.find("button[eleflag='ShowPause']");
                ShowPause.css({ "display": "" });
                var ShowAnalysis = self.find("button[eleflag='ShowAnalysis']");
                ShowAnalysis.css({ "display": "" });
                var ShowSetCount = self.find("button[eleflag='ShowSetCount']");
                ShowSetCount.css({ "display": "" });
                var ShowUpdatePassWord = self.find("button[eleflag='ShowUpdatePassWord']");
                ShowUpdatePassWord.css({ "display": "" });
                break;
            case '暂停使用':
                var ShowPass = self.find("button[eleflag='ShowPass']");
                ShowPass.css({ "display": "" });
                var ShowAll = self.find("button[eleflag='ShowAll']");
                ShowAll.css({ "display": "" });
                var ShowPass = self.find("button[eleflag='ShowPass']");
                ShowPass.css({ "display": "" });
                var ShowBegin = self.find("button[eleflag='ShowBegin']");
                ShowBegin.css({ "display": "" });
                var ShowSetCount = self.find("button[eleflag='ShowSetCount']");
                ShowSetCount.css({ "display": "" });
                var ShowUpdatePassWord = self.find("button[eleflag='ShowUpdatePassWord']");
                ShowUpdatePassWord.css({ "display": "" });
                break;
            case '审核不通过':
                var ShowPass = self.find("button[eleflag='ShowPass']");
                ShowPass.css({ "display": "" });
                var ShowAll = self.find("button[eleflag='ShowAll']");
                ShowAll.css({ "display": "" });
                var ShowPass = self.find("button[eleflag='ShowPass']");
                ShowPass.css({ "display": "" });
                var ShowSetCount = self.find("button[eleflag='ShowSetCount']");
                ShowSetCount.css({ "display": "" });
                var ShowUpdatePassWord = self.find("button[eleflag='ShowUpdatePassWord']");
                ShowUpdatePassWord.css({ "display": "" });
                break;
            default:
                var ShowPass = self.find("button[eleflag='ShowPass']");
                ShowPass.css({ "display": "" });
                var ShowNoPass = self.find("button[eleflag='ShowNoPass']");
                ShowNoPass.css({ "display": "" });
                var ShowAll = self.find("button[eleflag='ShowAll']");
                ShowAll.css({ "display": "" });
                var ShowSetCount = self.find("button[eleflag='ShowSetCount']");
                ShowSetCount.css({ "display": "" });
                var ShowUpdatePassWord = self.find("button[eleflag='ShowUpdatePassWord']");
                ShowUpdatePassWord.css({ "display": "" });
                break;
        }
    }

    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowPass = self.find("button[eleflag='ShowPass']");
        ShowPass.css({ "display": "none" });
        var ShowNoPass = self.find("button[eleflag='ShowNoPass']");
        ShowNoPass.css({ "display": "none" });
        var ShowAll = self.find("button[eleflag='ShowAll']");
        ShowAll.css({ "display": "none" });
        var ShowLong = self.find("button[eleflag='ShowLong']");
        ShowLong.css({ "display": "none" });
        var ShowPause = self.find("button[eleflag='ShowPause']");
        ShowPause.css({ "display": "none" });
        var ShowBegin = self.find("button[eleflag='ShowBegin']");
        ShowBegin.css({ "display": "none" });
        var ShowAnalysis = self.find("button[eleflag='ShowAnalysis']");
        ShowAnalysis.css({ "display": "none" });
        var ShowSetCount = self.find("button[eleflag='ShowSetCount']");
        ShowSetCount.css({ "display": "none" });
        var ShowUpdatePassWord = self.find("button[eleflag='ShowUpdatePassWord']");
        ShowUpdatePassWord.css({ "display": "none" });
    }

    var VerifyFun = function (isVerify) {
        var isVerify = ko.utils.unwrapObservable(isVerify);
        var result = '';
        switch (isVerify) {
            case -3:
                result = '试用户';
                break;
            case -2:
                result = '暂停使用';
                break;
            case -1:
                result = '审核不通过';
                break;
            case 0:
                result = '未审核';
                break;
            case 1:
                result = '审核通过';
                break;
        }
        return result;
    }
    var VerifyEnterprise = function (enterpriseid, type, strType, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(enterpriseid);
        var type = ko.utils.unwrapObservable(type);
        var msg = '您确定要' + strType + '该企业吗？';
        dialog.showMessage(msg, '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        enterpriseid: id,
                        type: type
                    }
                    $.ajax({
                        type: "POST",
                        url: "/SysEnterpriseManage/VerifyEnterprise",
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
                                        name('');
                                        searchEnterprise();
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
    var getTryDays = function (enterpriseId) {
        var result = "永久";
        var sendData = {
            enterpriseid: ko.utils.unwrapObservable(enterpriseId)
        }
        $.ajax({
            type: "POST",
            url: "/SysEnterpriseManage/GetTryDays",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                result = jsonResult.ResultMsg;
            }
        });
        return result;
    };
    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            vm.vmEnterprise = getDataKO(1, "");
        },
        goBack: function () {
            router.navigateBack();
        },
        vmEnterprise: null,
        addEnterprise: addEnterprise,
        goSetAnalysis: goSetAnalysis,
        searchEnterprise: searchEnterprise,
        name: name,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        loginInfo: loginInfo,
        VerifyFun: VerifyFun,
        VerifyEnterprise: VerifyEnterprise,
        getTryDays: getTryDays,
        setRequestCount: setRequestCount,
        setUpdatePassWord: setUpdatePassWord,
        codeStatistics:codeStatistics
    }
    return vm;

});