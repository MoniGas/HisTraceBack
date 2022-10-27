define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo'],
function (dialog, router, ko, km, jq, utils, loginInfo) {
    var moduleInfo = {
        moduleID: '11120',
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
            url: "/SysEnterpriseManage/GetEnterpriseWareHouse",
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
            case '申请开通':
                var ShowPause = self.find("button[eleflag='ShowPass']");
                ShowPause.css({ "display": "" });
                var ShowPause = self.find("button[eleflag='ShowNoPass']");
                ShowPause.css({ "display": "" });
                break;
            case '正常':
                var ShowPause = self.find("button[eleflag='ShowPause']");
                ShowPause.css({ "display": "" });
                break;
            case '审核不通过':
                var ShowPause = self.find("button[eleflag='ShowPass']");
                ShowPause.css({ "display": "" });
                break;
            case '暂停服务':
                var ShowBegin = self.find("button[eleflag='ShowBegin']");
                ShowBegin.css({ "display": "" });
                break;
        }
    }

    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var ShowPause = self.find("button[eleflag='ShowPass']");
        ShowPause.css({ "display": "none" });
        var ShowPause = self.find("button[eleflag='ShowNoPass']");
        ShowPause.css({ "display": "none" });
        var ShowPause = self.find("button[eleflag='ShowPause']");
        ShowPause.css({ "display": "none" });
        var ShowBegin = self.find("button[eleflag='ShowBegin']");
        ShowBegin.css({ "display": "none" });
    }

    var VerifyFun = function (isVerify) {
        var isVerify = ko.utils.unwrapObservable(isVerify);
        var result = '';
        switch (isVerify) {
            case 0:
                result = "未申请";
                break;
            case 1:
                result = "申请开通";
                break;
            case 2:
                result = "正常";
                break;
            case 3:
                result = "审核不通过";
                break;
            case 4:
                result = "暂停服务";
                break;
        }
        return result;
    }
    var VerifyShop = function (enterpriseid, type, strType, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(enterpriseid);
        var type = ko.utils.unwrapObservable(type);
        var msg = '您确定要' + strType + '该企业的仓库服务吗？';
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
                        url: "/SysEnterpriseManage/VerifyWareHouse",
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
        searchEnterprise: searchEnterprise,
        name: name,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        VerifyFun: VerifyFun,
        VerifyShop: VerifyShop
    }
    return vm;

});