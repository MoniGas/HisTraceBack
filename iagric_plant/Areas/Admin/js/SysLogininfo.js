/**
* Durandal 2.0.1 Copyright (c) 2012 Blue Spire Consulting, Inc. All Rights Reserved.
* Available via the MIT license.
* see: http://durandaljs.com or https://github.com/BlueSpire/Durandal for details.
*/
/**
* 登录相关函数
*/
define(['plugins/dialog', 'durandal/system', 'plugins/router', 'knockout', 'jquery', 'jquery.querystring'], function (dialog, system, router, ko, $, qs) {
    var logininfo;
    //获取路由名称
    var getRouteName = function (item) {
        if (item.SortOrder == 1 && item.Parent_ID == "0") {
            return ['', item.route];
        } else {
            return item.route;
        }
    }
    logininfo = {
        loginInfo: [],
        modules: [],
        routers: [],
        router: router,
        getLoginInfo: function () {        //获取登录信息
            var isLogin = false;
            var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/SysLogined/GetLoginInfo",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (logininfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    isLogin = false;
                    return;
                };
                logininfo.loginInfo = jsonResult;
                isLogin = true;
            },
            error: function (e) {
                alert(e);
            }
        });
        return isLogin;
    },
    getModules: function () {
        var sendData = {
            parentID: -1
        }
        $.ajax({
            type: "POST",
//            url: "/Logined/GetModuleList",
            url: "/SysLogined/GetSysModuleList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (logininfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                logininfo.modules = jsonResult;
            },
            error: function (e) {
                alert(e);
            }
        });
        //alert(JSON.stringify(modules.ObjList));
        $.each(logininfo.modules.ObjList, function (i, item) {
            var single = {
                route: getRouteName(item),
                moduleId: item.url,
                title: item.Title,
                nav: item.SortOrder,
                id: item.PRRU_Modual_ID,
                parentid: item.Parent_ID,
                rootparentid: item.RootParent_ID,
                icon: item.Img,
                activeClass: '',
                isNavShow: item.IsDisplay,
                hash: item.hash
            }
            logininfo.routers.push(single);
        });
        //system.log('logininfo.routers:' + JSON.stringify(logininfo.routers));
    },
    setActiveItemToParent: function (code, defaultParentCode) {
        var parentCode = qs.querystring("pc");
        if (parentCode == undefined || parentCode == '') {
            parentCode = defaultParentCode;
        }
        var routes = logininfo.router.routes;
        //system.log('shell.router.navigationModel:' + JSON.stringify(routes));
        var currentRoute;
        var parentRoute;
        var currentRoutes = ko.utils.arrayFilter(routes, function (route) {
            return route.id == code;
        });
        currentRoute = currentRoutes[0];
        //system.log('currentRoute.isActive:' + currentRoute.isActive());

        if (parentCode != undefined && parentCode != '' && parentCode != '0') {
            var parentRoutes = ko.utils.arrayFilter(routes, function (route) {
                return route.id == parentCode;
            });
            parentRoute = parentRoutes[0];
        }

        ko.utils.arrayForEach(routes, function (route) {
            //system.log('routeItem:' + JSON.stringify(route));
            if (route.isActive() == true) {
                //system.log('routeItem:' + JSON.stringify(route));
                route.isActive(false);
            }
        });

        if (parentRoute != undefined) {
            parentRoute.isActive(true);
        }

        if (currentRoute != undefined) {
            if (parentCode == '0') {
                currentRoute.isActive(true);
            }
            else {
                currentRoute.isActive(false);
            }
        }
    },
    isLoginTimeout: function (code, msg, flag) { //请重新登录
        //alert(flag);
        if (code == -100) {
            if (flag == 1) {
                dialog.showMessage(msg, '系统提示', [{ title: '确定', callback: function () {
                    location.href = "/Admin";
                }
                }]);
            }
            else {
                alert(msg);
                location.href = "/Admin";
            }
            //                bootbox.alert({
            //                    title: "提示",
            //                    message: msg,
            //                    buttons: {
            //                        ok: {
            //                            label: '确定'
            //                        }
            //                    },
            //                    callback: function () {
            //                        location.href = "/";
            //                    }
            //                });

            return true;
        }
        else {
            return false;
        }
    },
    isLoginTimeoutForServer: function () {
        var result;
        var sendData = {}
        $.ajax({
            type: "POST",
            url: "/SysLogined/IsLogin",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                result = jsonResult;
            }
        })
        return result;
    },
    checkPower: function (code) {
        var modualIDArr = logininfo.loginInfo.ObjModel.RoleModual_ID_Array.split(',');
        //system.log('modualIDArr:' + modualIDArr);
        //system.log('code:' + code);
        if ($.inArray(code, modualIDArr) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
}
return logininfo;
});
