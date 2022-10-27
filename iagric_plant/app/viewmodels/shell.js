define(['plugins/router', 'durandal/system', 'knockout', 'jquery', 'bootstrap', 'jquery.loadmask.spin', 'plugins/dialog', 'utils', 'logininfo', 'jquery.poshytip', 'knockout.mapping', 'jquery.querystring', 'charts','webuploader'],
function (router, system, ko, $, bs, spin, dialog, utils, loginInfo, poshytip, km, qs, charts, webuploader) {
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
                        url: "/Login/ExitSignOut",
                        contentType: "application/json;charset=utf-8", //必须有      
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (jsonResult.code == "1") {
                            window.location.href = "/";
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
    var clickUp = function (data, event) {
        var self = $(event.target).closest('dl');
        var a = $(self).children();
        var ul = self.children("ul");
        var b = $(event.target);
        if (b.hasClass("icon_treemenu_arrow")) {
            ul.css({ "display": "none" });
            b.attr({ "class": "icon_treemenu_arrowUp" });
        } else {
            ul.css({ "display": "" });
            b.attr({ "class": "icon_treemenu_arrow" });
        }
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
    //ajax获取数据
    var getData = function (pageIndex, helpTitle) {
        var sendData = {
            helpTitle: helpTitle,
            pageIndex: pageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysHelp/GetHelpList",
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
    var getDataKO = function (pageIndex, helpTitle) {
        var list = km.fromJS(getData(pageIndex, helpTitle));
        return list;
    }
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmHelp.ObjList(list.ObjList());
    }
    var show = function (data, event) {
        $("#Div1").addClass('open');
    }

    var hide = function (div, data, event) {
        $("#Div1").removeClass('open');
    }
    // 投诉数量
    var complaintCount = 0;
    var newsCount = ko.observable(0);
    // 温馨提醒数量
    var reminderCount = 0;
    /******************去掉时间中的时分秒***********************/
    var newDate = function (date) {
        return date.substring(0, 10);
    }
    /*****************获取投诉未读消息*************************/
    var getComplaintList = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Complaint/GetList?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                data = jsonResult;
            }
        })
        return data;
    }
    /*****************修改投诉信息已读状态****************************/
    var updateComplaint = function (id) {
        var sendData = {
            id: id
        }
        $.ajax({
            type: "POST",
            url: "/Complaint/UpdateStatus",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
            }
        })
    }
    /***********************获取投诉数量**********************************/
    var getcomplaintCount = function () {
        return complaintCount;
    }
    /*****************获取是否存在产品*****************/
    var getMaterial = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetMaterial?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_Material").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_Material").css({ "display": "none" });
                }
            }
        })
    }
    /*****************获取是否存在生产环节*****************/
    var getGongXu = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetZuoYe?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_GongXu").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_GongXu").css({ "display": "none" });
                }
            }
        })
    }

    /*****************获取是否存在品牌*****************/
    var getBrand = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetBrand?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_Brand").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_Brand").css({ "display": "none" });
                }
            }
        })
    }
    /*****************获取是否存在生产基地*****************/
    var getGreenhouses = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetGreenhouses?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_Greenhouses").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_Greenhouses").css({ "display": "none" });
                }
            }
        })
    }
    /***********************获取是否已经完善企业信息***********************/
    var getEnterprise = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/CompleteEnterpriseInfo?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (obj) {
                if (obj.ObjModel.IsAuthen == 0 || obj.ObjModel.IsAuthen == -1) {
                    if (obj.ObjModel.SurplusTime != null) {
                        dayNum = obj.ObjModel.SurplusTime;
                    }
                    reminderCount = reminderCount + 1;
                    $("#li_enterpriseInfo").css({ "display": "" });
                } else if (obj.ObjModel.IsAuthen == 100 || obj.ObjModel.IsAuthen == 1) {
                    $("#li_enterpriseInfo").css({ "display": "none" });
                }
            }
        })
    }
    /***********************获取码数量是否触发阀值***********************/
    var getThresholdWarning = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetThresholdWarning?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {

                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_ThresholdWarning").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_ThresholdWarning").css({ "display": "none" });
                }
            }
        })
    }
    /***********************获取是否存在经销商**********************/
    var getDealer = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetDealer?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                if (jsonResult.code == 0) {
                    reminderCount = reminderCount + 1;
                    $("#li_Dealer").css({ "display": "" });
                } else if (jsonResult.code == 1) {
                    $("#li_Dealer").css({ "display": "none" });
                }
            }
        })
    }
    /*********************获取温馨提示数量**************************/
    var getReminderCount = function () {
        return reminderCount;
    }
    /*****************升级*************************/
    var Update = ko.observable('');
    var getUpdate = function () {
        var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/Complaint/GetUpdate",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                Update(jsonResult.Msg);
            }
        })
    }
    /*************获取二维码申请未读消息**********************/
    var getEwmNewsList = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetEwmNewsList?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                data = jsonResult;
            }
        });
        return data;
    }
    /******************获取企业审核是否通过*****************/
    var getEnterpriseVerify = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetEnterpriseVerifyList?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                data = jsonResult;
            }
        });
        return data;
    }
    /*******************获取二维码审核通过信息***********************/
    var getCodeRecord = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetCodeRecord?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                }
                data = jsonResult;
            }
        });
        return data;
    }
    /*******************忽略***********************/
    var ignoreAll = function () {
        ignoreEwm();
        vm.newsCount(0);
        vm.vmEnterpriseVerify = km.fromJS(getEnterpriseVerify());
        vm.newsCount(parseInt(vm.vmEnterpriseVerify.totalCounts()));
    }
    var ignoreEwm = function () {
        var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/News/IgnoreEwm",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                } else if (jsonResult.code == 1) {
                    vm.vmEwmNewsList.ObjList([]);
                }
            }
        })
    }
    /*****************获取首页统计数据*************************/
    var RequestCodeCount = ko.observable('');
    var RequestCodeTimes = ko.observable('');
    var ScanCodeTimes = ko.observable('');
    var BrancCount = ko.observable('');
    var MaterialCount = ko.observable('');
    var OriginCount = ko.observable('');
    var ProcessCount = ko.observable('');
    var EnterpriseName = ko.observable('');
    var LogoUrl = ko.observable('');
    var getHomeDataStatis = function () {
        var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/Complaint/GetHomeDataStatis?v=" + (new Date()).getTime(),
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                RequestCodeCount(jsonResult.ObjModel.RequestCodeCount);
                RequestCodeTimes(jsonResult.ObjModel.RequestCodeTimes);
                ScanCodeTimes(jsonResult.ObjModel.ScanCodeTimes);
                BrancCount(jsonResult.ObjModel.BrancCount);
                MaterialCount(jsonResult.ObjModel.MaterialCount);
                OriginCount(jsonResult.ObjModel.OriginCount);
                ProcessCount(jsonResult.ObjModel.ProcessCount);
                EnterpriseName(jsonResult.Msg);
                LogoUrl(jsonResult.ObjModel.LogoUrl);
            }
        })
    }
    /*****************报表*************************/
    function GetChartData() {
        var container = 'chartDIV';
        var title = '';
        var units = '拍码次数';
        var lineName = '日拍码情况';
        var chart1 = new Highcharts.Chart({
            chart: {
                renderTo: container,
                width: 683,
                hight: 100,
                //            margin: [0, 0, 0, 0], //距离上下左右的距离值
                type: 'line'
            },
            title: {
                text: title
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: []
            },
            yAxis: {
                title: {
                    text: units
                },
                allowDecimals: false //是否允许刻度有小数
            },
            tooltip: {
                enabled: false,
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' + this.x + ': ' + this.y + '°C';
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            },
            series: [{
                name: lineName,
                data: []
            }]
        });
        var xatrnames = [];
        var data1 = [];
        var data2 = [];
        var StrRequest = '';
        var StrSale = '';
        $.ajax({
            type: "POST",
            url: "/Complaint/GetChartData",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                try {
                    for (var i = 0; i < jsonResult.ObjList.length; i++) {
                        xatrnames.push([jsonResult.ObjList[i].title]);
                        data1.push([parseInt(jsonResult.ObjList[i].value)]);
                    }
                    chart1.xAxis[0].setCategories(xatrnames);
                    chart1.series[0].setData(data1);
                }
                catch (e) { }
            }
        })
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
            vm.vmHelp = getDataKO(1, "");
            updateData(vm.vmHelp);
            $('#Div1').poshytip(
            {
                content: Div2,
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 3,
                offsetY: 0
            });
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
            vm.vmEwmNewsList = km.fromJS(getEwmNewsList());
            vm.vmEnterpriseVerify = km.fromJS(getEnterpriseVerify());
            vm.vmCodeRecord = km.fromJS(getCodeRecord());
            vm.newsCount(
                parseInt(vm.vmEwmNewsList.totalCounts())
                + parseInt(vm.vmEnterpriseVerify.totalCounts())
                + parseInt(vm.vmCodeRecord.totalCounts())
            );
            if (vm.newsCount() > 0) {
                var btnEdit = $("#viewNewsTipe");
                btnEdit.css({ "display": "none" });
                var btnEdit = $("#theViewMore");
                btnEdit.css({ "display": "" });
            } else {
                var btnEdit = $("#viewNewsTipe");
                btnEdit.css({ "display": "" });

                var btnEdit = $("#theViewMore");
                btnEdit.css({ "display": "none" });
            }
            vm.vmComplaintList = getComplaintList();
            complaintCount = vm.vmComplaintList.totalCounts;
            if (complaintCount > 0) {
                var btnEdit = $("#viewComplaintTipe");
                btnEdit.css({ "display": "none" });
            } else {
                var btnEdit = $("#viewComplaintTipe");
                btnEdit.css({ "display": "" });
            }
            reminderCount = 0;
            getEnterprise();
            getBrand();
            getMaterial();
            getGongXu();
            getDealer();
            getHomeDataStatis();
            getUpdate();
            getThresholdWarning();
            if (reminderCount > 0) {
                var btnEdit = $("#viewReminderTipe");
                btnEdit.css({ "display": "none" });
            } else {
                var btnEdit = $("#viewReminderTipe");
                btnEdit.css({ "display": "" });
            }
            $('#divComplaint').poshytip(
            {
                content: div1,
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 0,
                offsetY: 3
            });
            $('#divMessage').poshytip(
            {
                content: div2,
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 0,
                offsetY: 3
            });
            $('#divWarm').poshytip(
            {
                content: div3,
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 0,
                offsetY: 3
            });
            GetChartData();
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
        show: show,
        hide: hide,
        leftArray: leftArray,
        tabTestArray: tabTestArray,
        mouseoutFun: mouseoutFun,
        mouseoverFun: mouseoverFun,
        mouseoutNameFun: mouseoutNameFun,
        mouseoverNameFun: mouseoverNameFun,
        clickUp: clickUp,
        getSubmenu: getSubmenu,
        test: test,
        isDisplay: isDisplay,
        getReminderCount: getReminderCount,
        getcomplaintCount: getcomplaintCount,
        newsCount: newsCount,
        RequestCodeCount: RequestCodeCount,
        RequestCodeTimes: RequestCodeTimes,
        ScanCodeTimes: ScanCodeTimes,
        BrancCount: BrancCount,
        MaterialCount: MaterialCount,
        OriginCount: OriginCount,
        ProcessCount: ProcessCount,
        Update: Update,
        ignoreAll: ignoreAll,
        updateComplaint: updateComplaint,
        newDate: newDate,
        EnterpriseName: EnterpriseName,
        LogoUrl: LogoUrl,
        title: title
    };
    return vm;
});