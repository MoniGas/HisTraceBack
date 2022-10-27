define(['plugins/router', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo', 'plugins/dialog', 'jquery.querystring', './CollectCodeInfo'],
function (router, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo, dialog, qs, CollectCodeInfo) {
    var moduleInfo = {
        moduleID: '120000',
        parentModuleID: '20000'
    }
    var userName = ko.observable();
    var status = ko.observable();
    var complementHistoryDate = function () //获取当前日期的上个月请勿copy 出错后果自负  
    {
        var AddDayCount = -1;
        var dd = new Date();
        dd.setDate(dd.getDate() + AddDayCount); //获取AddDayCount天后的日期 
        var y = dd.getFullYear();
        var m = dd.getMonth() + 1; //获取当前月份的日期 
        var d = dd.getDate();
        m = m < 10 ? "0" + m : m;
        d = d < 10 ? "0" + d : d;
        return y + "-" + m + "-" + d;
    }
    var beginDate = ko.observable(complementHistoryDate());
    var getNowFormatDate = function () {
        var date = new Date();
        var seperator1 = "-";
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = year + seperator1 + month + seperator1 + strDate;
        return currentdate;
    }
    var endDate = ko.observable(getNowFormatDate());
    //ajax获取数据
    var getData = function (pageIndex, status, userName) {
        var sendData = {
            pageIndex: pageIndex,
            collectUser: userName,
            status: status,
            beginDate: beginDate(),
            endDate: endDate()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_CollectCode/Index",
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
        vm.vmRequestCodeMa.ObjList(list.ObjList());
        vm.vmRequestCodeMa.pageSize(list.pageSize());
        vm.vmRequestCodeMa.totalCounts(list.totalCounts());
        vm.vmRequestCodeMa.pageIndex(list.pageIndex());
    }
    //搜索
    var searchRequestCodeMa = function (data, event) {
        if ($("#li0").hasClass("active")) {
            var list = getDataKO(1, 0, userName());
            updateData(list);
        }
        else {
            var list = getDataKO(1, 1, userName());
            updateData(list);
        }
    };
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, status, userName) {
        status = ko.utils.unwrapObservable(status);
        var list = km.fromJS(getData(pageIndex, status, userName));
        return list;
    }
    var TopSubPageClick = function (index, data, event) {
        if (index == 1) {
            $("#li1").addClass("active");
            $("#li0").removeClass("active");
        }
        else {
            $("#li0").addClass("active");
            $("#li1").removeClass("active");
        }
        //        getDataKO(1, index, "");
        var list = getDataKO(1, index, userName());
        updateData(list);
    }
    // 查看产品二维码
    var ProductEwm = function (sId) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(sId);
        CollectCodeInfo.show(id);
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.requestsettingmanagePager = {
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
                        var list = getDataKO(num, status(), userName());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmRequestCodeMa.pageSize());
                        this.totalCounts = parseInt(vm.vmRequestCodeMa.totalCounts());
                    }
                }
            });
        }
    };
    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var SearchCode = self.find("button[eleflag='SearchCode']");
        SearchCode.css({ "display": "" });
        if ($("#li0").hasClass("active")) {
            var SearchCode = self.find("button[eleflag='DownCode']");
            SearchCode.css({ "display": "" });
            $("button[eleflag='DownCode']").text("下载");
        }
        else {
            //            var SearchCode = self.find("button[eleflag='DownCode']");
            //            SearchCode.css({ "display": "none" });
            var SearchCode = self.find("button[eleflag='DownCode']");
            SearchCode.css({ "display": "" });
            $("button[eleflag='DownCode']").text("重新下载");
        }
    }

    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var SearchCode = self.find("button[eleflag='SearchCode']");
        SearchCode.css({ "display": "none" });
        if ($("#li0").hasClass("active")) {
            var SearchCode = self.find("button[eleflag='DownCode']");
            SearchCode.css({ "display": "none" });
        }
        else {
            var SearchCode = self.find("button[eleflag='DownCode']");
            SearchCode.css({ "display": "none" });
        }
    }
    var DownLoad = function (sId) {
        var sId = ko.utils.unwrapObservable(sId);
        $.fileDownload('/Admin_CollectCode/ExportTxt?sID=' + sId)
                          .done(function () {
                              alert('下载成功');
                          }).fail(function () {
                              alert('下载失败!');
                          });
        var sendData = {
            sId: sId
        }
        $.ajax({
            type: "POST",
            url: "/Admin_CollectCode/UpdateStatus",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                if (jsonResult.code == 1) {
                    if ($("#li0").hasClass("active")) {
                        var list = getDataKO(1, 0, userName());
                        updateData(list);
                    }
                    else {
                        var list = getDataKO(1, 1, userName());
                        updateData(list);
                    }
                }
            }
        })
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.vmRequestCodeMa = getDataKO(1);
            updateData(vm.vmRequestCodeMa);
            $('#date1').datepicker({
                autoclose: true,
                todayHighlight: true,
                language: 'cn'
            });
            $('#date2').datepicker({
                autoclose: true,
                todayHighlight: true,
                language: 'cn'
            });
        },
        goBack: function () {
            router.navigateBack();
        },
        vmRequestCodeMa: null,
        userName: userName,
        status: status,
        beginDate: beginDate,
        endDate: endDate,
        searchRequestCodeMa: searchRequestCodeMa,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        TopSubPageClick: TopSubPageClick,
        ProductEwm: ProductEwm,
        DownLoad: DownLoad
    }
    return vm;
});