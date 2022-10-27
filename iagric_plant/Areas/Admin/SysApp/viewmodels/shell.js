define(['plugins/router', 'durandal/system', 'knockout', 'jquery', 'bootstrap', 'jquery.loadmask.spin', 'plugins/dialog', 'utils', 'logininfo', 'jquery.poshytip', 'knockout.mapping', 'jquery.querystring'],
function (router, system, ko, $, bs, spin, dialog, utils, loginInfo, poshytip, km, qs) {
    var leftArray = ko.observableArray();
    var tabTestArray = ko.observableArray();
    var homeArray = ko.observableArray();
    var title = ko.observable();
    var editpwd = function () {
        vmPwd.show();
    }
    var exitsign = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        dialog.showMessage("您确定退出系统吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                }
                $.ajax({
                    type: "POST",
                    url: "/SysLogin/ExitSignOut",
                    contentType: "application/json;charset=utf-8", //必须有      
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        location.href = "/Login/HomeIndex";
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
    var isLogin = loginInfo.getLoginInfo();
    if (!isLogin) {
        return;
    }
    loginInfo.getModules();
    var menuList = router.map(loginInfo.routers).buildNavigationModel().mapUnknownRoutes('viewmodels/error-404', 'not-found');
    ko.computed(function () {
        if (router.isNavigating()) {
            if (!$("#html1").isMasked()) {
                $("#html1").mask({ spinner: { lines: 10, length: 5, width: 3, radius: 10 }, 'label': "加载中..." });
            }
        }
        else {

            if ($("#html1").isMasked()) {
                $("#html1").unmask();
            }
        }
    });
    var navigateTo = function (data, event) {
        if (!data.isActive()) {
            if (data.hash == '#') return;
            if (data.parentid == 0) {
                leftArray(
                    ko.utils.arrayFilter(router.navigationModel(), function (route) {
                        return route.parentid == data.id;
                    })
                );
                if (data.id == 10000) {
                    $("#leftMenu").hide();
                    $("#tabMenu").hide();
                    $("#HomePage").show();
                    $("#mainContent").attr("style", "margin-left: 0;padding-top:20px;");
                }
                else {
                    $("#leftMenu").show();
                    $("#tabMenu").show();
                    $("#HomePage").hide();
                    $("#mainContent").removeAttr("style");
                    var subData = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                        return (route.parentid == data.id || route.rootparentid == data.id) && route.hash == data.hash;
                    })[0];
                    if (subData == undefined) {
                        subData = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                            return route.id == data.id;
                        })[0];
                    }
                    title(subData.title);
                    if ($.inArray(subData, tabTestArray()) == -1) {
                        tabTestArray.push(subData);
                    }
                }
            }
            else {
                $("#leftMenu").show();
                $("#tabMenu").show();
                $("#HomePage").hide();
                $("#mainContent").removeAttr("style");
                leftArray(
                    ko.utils.arrayFilter(router.navigationModel(), function (route) {
                        return route.parentid == data.rootparentid;
                    })
                );
                title(data.title);
                if ($.inArray(data, tabTestArray()) == -1) {
                    tabTestArray.push(data);
                }
            }
            if (data.id == "200000") {
                location.href = data.hash;
            }
            else {
                router.navigate(data.hash);
            }
            loginInfo.setActiveItemToParent(data.id, data.parentid);
        }
    }
    /****************根据确定的路径跳转********************/
    var navigateToUrl = function (urlTitle, urlHash, id, parentid, data, event) {
        if (urlHash == '#') return;
        $("#leftMenu").show();
        $("#tabMenu").show();
        $("#HomePage").hide();
        $("#mainContent").removeAttr("style");
        leftArray(
            ko.utils.arrayFilter(router.navigationModel(), function (route) {
                return route.parentid == parentid;
            })
        );
        title(urlTitle);
        //        if ($.inArray(data, tabTestArray()) == -1) {
        //            tabTestArray.push(data);
        //        }
        router.navigate(urlHash);
        loginInfo.setActiveItemToParent(id, parentid);
    }
    /***************************************************/
    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('dl');
        self.addClass("active");
        self.children(".menubar").attr('class', '');

    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('dl');
        self.removeClass("active");
        self.children(".menubar").attr('class', '');
    }
    var mouseoverNameFun = function (data, event) {
        var self = $(event.target).closest('span');
        var a = $(self).children();
        var ul = self.children("ul");
        ul.css({ "display": "" });
    }
    var mouseoutNameFun = function (data, event) {
        var self = $(event.target).closest('span');
        var ul = $(self).first('ul');
        ul.css({ "display": "none" });
    }
    /*******************获取三级菜单*********************/
    var getSubmenu = function (parentId) {
        return ko.utils.arrayFilter(router.navigationModel(), function (route) {
            return route.parentid == parentId && route.modualLevel == 3;
        });
    }
    /***********************************************/
    var test = function (hash) {
        //alert(2);
    }
    var isDisplay = function (parentId) {
        var count = ko.utils.arrayFilter(router.navigationModel(), function (route) {
            return route.parentid == parentId && route.modualLevel == 3;
        }).length;
        return count > 0;
    }
    var closeTable = function (data, event) {
        var nav = "#temp";
        var index = tabTestArray.indexOf(data);
        if (data.isActive()) {
            if (index == 0) {
                if (tabTestArray().length != 1) {
                    nav = tabTestArray()[index + 1].hash;
                }
            }
            else if (index == tabTestArray().length) {
                if (tabTestArray().length != 1) {
                    nav = tabTestArray()[index - 1].hash;
                }
            }
            else {
                nav = tabTestArray()[index - 1].hash;
            }
            router.navigate(nav);
        }
        tabTestArray.splice(index, 1);
    }

var vm = {
    router: loginInfo.router,
    activate: function () {
        return menuList.activate();
    },
    binding: function () {
        var hash = "";
        $.ajax(
        {
            type: "post",
            url: "/Home/GetHash",
            contentType: "application/json;charset=utf-8",
            dataType: "",
            async: false,
            success: function (data) {
                if (data != null && data.hash != null) {
                    hash = data.hash;
                }
            }
        }
        );
        if (hash != "") {
            router.navigate(hash);
            if (hash = "#home?menu=10000") {
            }
            else {
                return;
            }
        }
        //获取工作区域高度
        var windowsHeight = $(window).height();
        //获取头部高度
        var topHeight = $("#topheader").height();
        //计算操作区域高度
        var handlerHeight = windowsHeight - topHeight;
        //给操作区域容器赋值
        $(".col_side").height(handlerHeight);
        var hash = window.location.hash.split('?');
        if (!(hash[0] == '' || hash[0] == '#home')) {
            var menuId = qs.querystring("menu");
            $("#HomePage").hide();
            $("#leftMenu").show();
            $("#tabMenu").show();
            $("#mainContent").removeAttr("style");

            var menu = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                return route.route == hash[0].replace('#', '') && route.parentid != 0;
            });
            if (menu.length > 1) {
                menu = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                    return route.id == menuId && route.parentid != 0;
                });
            }
            var parentmenu = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                return route.id == menu[0].parentid;
            });
            while (parentmenu[0].parentid != 0) {
                parentmenu = ko.utils.arrayFilter(router.navigationModel(), function (route) {
                    return route.id == parentmenu[0].parentid;
                });
            }
            leftArray(
                    ko.utils.arrayFilter(router.navigationModel(), function (route) {
                        return route.parentid == parentmenu[0].id;
                    })
                );
            title(menu[0].title);
            if ($.inArray(menu[0], tabTestArray()) == -1) {
                tabTestArray.push(menu[0]);
            }
            parentmenu[0].isActive(true);
        }
        else {
            $("#leftMenu").hide();
            $("#tabMenu").hide();
            $("#HomePage").show();
            $("#mainContent").attr("style", "margin-left: 0;padding-top:20px;");
        }
    },
    initMenus: function (parentid) {
        return ko.utils.arrayFilter(router.navigationModel(), function (route) {
            return route.parentid == parentid;
        });
    },
    editpwd: editpwd,
    exitsign: exitsign,
    loginInfo: loginInfo.loginInfo,
    modules: loginInfo.modules,
    navigateTo: navigateTo,
    navigateToUrl: navigateToUrl,
    closeTable: closeTable,
    leftArray: leftArray,
    tabTestArray: tabTestArray,
    mouseoutFun: mouseoutFun,
    mouseoverFun: mouseoverFun,
    mouseoutNameFun: mouseoutNameFun,
    mouseoverNameFun: mouseoverNameFun,
    getSubmenu: getSubmenu,
    test: test,
    isDisplay: isDisplay,
    title: title
};
return vm;
});