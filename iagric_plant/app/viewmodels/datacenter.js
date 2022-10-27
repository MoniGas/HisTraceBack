define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'logininfo', 'plugins/dialog', './openshop'],
function (router, ko, km, $, jq, utils, loginInfo, dialog, openshop) {
    var searchTitle = ko.observable('1');
    var moduleInfo = {
        moduleID: '10001',
        parentModuleID: '0'
    }

    var navigateTo = function (routeName, data, event) {
        switch (routeName) {
            case 'guide1':
                router.navigate('#' + routeName + '?pc=10001');
                break;
            case 'openshop':
                var isOpenShop = false;
                $.ajax({
                    type: "POST",
                    url: "/Admin_EnterpriseInfo/Index",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    async: false,
                    success: function (obj) {
                        isOpenShop = obj.ObjModel.IsOpenShop;
                    }
                });

                if (isOpenShop) {
                    dialog.showMessage("确定要关闭在线商城吗?", '系统提示', [
                    {
                        title: '确定',
                        callback: function () {
                            $.ajax({
                                type: "POST",
                                url: "/Admin_EnterpriseInfo/OpenShop",
                                contentType: "application/json;charset=utf-8", //必须有
                                dataType: "json", //表示返回值类型，不必须
                                async: false,
                                success: function (jsonResult) {
                                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                        return;
                                    }
                                    else {
                                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                            if (jsonResult.code != 0) {
                                                searchTitle('开通商城');
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
                    }]);
                }
                else {
                    openshop.show().then(function (isUpdate) {
                        if (isUpdate) {
                            searchTitle('关闭商城');
                        }
                    });
                }
                break;
            default:
                router.navigate('#' + routeName);
        }
    }

    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);

            $.ajax({
                type: "POST",
                url: "/Admin_EnterpriseInfo/Index",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (obj) {
                    if (obj.ObjModel.IsOpenShop) {
                        searchTitle('关闭商城');
                    }
                    else {
                        searchTitle('开通商城');
                    }
                }
            });

        },
        navigateTo: navigateTo,
        loginInfo: loginInfo,
        searchTitle: searchTitle
    }
    return vm;
});