/**
* Durandal 2.0.1 Copyright (c) 2012 Blue Spire Consulting, Inc. All Rights Reserved.
* Available via the MIT license.
* see: http://durandaljs.com or https://github.com/BlueSpire/Durandal for details.
*/
/**
* 一些常用的函数
*/
define(['plugins/dialog', 'plugins/router', 'jquery', 'jquery.querystring'], function (dialog, router, $, qs) {
    var utils;
    utils = {
        dateFormat: function (date, format) {
            if (arguments.length < 2 && !date.getTime) {
                format = date;
                date = new Date();
            }
            typeof format != 'string' && (format = 'yyyy年MM月dd日 hh时mm分ss秒');
            var week = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', '日', '一', '二', '三', '四', '五', '六'];
            return format.replace(/yyyy|YY|MM|dd|hh|mm|ss|星期|周|www|week/g, function (a) {
                switch (a) {
                    case "yyyy": return date.getFullYear();
                    case "YY": return (date.getFullYear() + "").slice(2);
                    case "MM": return date.getMonth() + 1;
                    case "dd": return date.getDate();
                    case "hh": return date.getHours();
                    case "mm": return date.getMinutes();
                    case "ss": return date.getSeconds();
                    case "星期": return "星期" + week[date.getDay() + 7];
                    case "周": return "周" + week[date.getDay() + 7];
                    case "week": return week[date.getDay()];
                    case "www": return week[date.getDay()].slice(0, 3);
                }
            });
        },
        setActiveItemToParent: function (code, defaultParentCode) {
            var parentCode = qs.querystring("pc");
            if (parentCode == undefined || parentCode == '') {
                parentCode = defaultParentCode;
            }
            var routes = shell.router.routes;
            alert(JSON.stringify(routes));
            //system.log('shell.router.navigationModel:' + JSON.stringify(routes));
            var currentRoute;
            var parentRoute;
            var currentRoutes = ko.utils.arrayFilter(routes, function (route) {
                return route.code == code;
            });
            currentRoute = currentRoutes[0];
            //system.log('currentRoute.isActive:' + currentRoute.isActive());

            if (parentCode != undefined && parentCode != '' && parentCode != '0') {
                var parentRoutes = ko.utils.arrayFilter(routes, function (route) {
                    return route.code == parentCode;
                });
                parentRoute = parentRoutes[0];
            }

            ko.utils.arrayForEach(routes, function (route) {
                //system.log('routeItem:' + JSON.stringify(route));
                if (route.isActive() == true) {
                    system.log('routeItem:' + JSON.stringify(route));
                    route.isActive(false);
                }
            });

            if (parentRoute != undefined) {
                parentRoute.isActive(true);
            }

            if (parentCode == '0') {
                currentRoute.isActive(true);
            }
            else {
                currentRoute.isActive(false);
            }
        }
    }
    return utils;
});
