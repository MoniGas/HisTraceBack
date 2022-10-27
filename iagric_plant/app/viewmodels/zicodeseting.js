define(['plugins/router', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo', 'plugins/dialog', 'jquery.querystring'],
function (router, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo, dialog, qs) {
    var moduleInfo = {
        moduleID: '25000',
        parentModuleID: '20000'
    }
    var requestcodeID = ko.observable();
    var searchTitle = ko.observable();
    var mName = ko.observable();
    var bName = ko.observable();

    var complementHistoryDate = function () //获取当前日期的上个月请勿copy 出错后果自负  
    {
        var curdate = new Date();
        var seperator1 = "-";
        var curMonth = curdate.getMonth();           //上个月  数组形式来存储月份下标从0到11   0的话实际是12月  
        curMonth = (0 == curMonth) ? 12 : curMonth
        curMonth = curMonth < 10 ? ("0" + curMonth.toString()) : (curMonth.toString());
        var curDate = curdate.getDate();
        curDate = curDate < 10 ? ("0" + curDate.toString()) : (curDate.toString());
        var curYear = curdate.getYear();             //当前年     
        curYear = (12 == curMonth) ? (curYear - 1) : curYear;  //上个月所属的年 如果是上个月是12月 必定是去年  
        curYear += (curYear < 2000) ? 1900 : 0;
        curYear = curYear.toString();

        var curYYYYMM = curYear + seperator1 + curMonth + seperator1 + curDate;

        return curYYYYMM;
    }
    var beginDate = ko.observable(complementHistoryDate());
//    var complementHistoryDate = function () {
//        var date = new Date();
//        var seperator1 = "-";
//        var strYear = date.getFullYear();
//        var strMonth = date.getMonth();
//        var strDate = date.getDate();
//        if (strMonth >= 1 && strMonth <= 9) {
//            strMonth = "0" + strMonth;
//        }
//        if (strDate >= 0 && strDate <= 9) {
//            strDate = "0" + strDate;
//        }
//        if (strMonth == 1) {
//            strYear--;
//            strMonth = 12;
//        }
//        var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
//        return currentdate;
//    }
//    var beginDate = ko.observable(complementHistoryDate());
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
    var getData = function (pageIndex, searchTitle, mName, bName) {
        var sendData = {
            pageIndex: pageIndex,
            requestcodeID: requestcodeID,
            searchName: searchTitle,
            mName: mName,
            bName: bName,
            beginDate: beginDate(),
            endDate: endDate()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/RequestCodeMa/Setcodelist",
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
        var list = getDataKO(1, searchTitle(), mName(), bName());
        updateData(list);
    };
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, searchTitle, mName, bName) {
        var list = km.fromJS(getData(pageIndex, searchTitle, mName, bName));
        return list;
    }
    //跳转设置码页面
    var goTorequestcodesetting = function (subID, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        router.navigate('#requestcodesetting?subid=' + subID);
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.zicodesetingPager = {
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
                        var list = getDataKO(num, searchTitle(), mName(), bName());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmRequestCodeMa.pageSize());
                        this.totalCounts = parseInt(vm.vmRequestCodeMa.totalCounts());
                    }
                }
            });
        }
    };
    var mouseoverFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.ID());

        btnDel.css({ "display": "" });

    }

    var mouseoutFun = function (data, event) {
        // 删除
        var btnDel = $("#btnDel_" + data.ID());

        btnDel.css({ "display": "none" });


    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            requestcodeID = qs.querystring("requestcodeid");
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
        requestcodeID: requestcodeID,
        searchTitle: searchTitle,
        mName: mName,
        bName: bName,
        beginDate: beginDate,
        endDate: endDate,
        searchRequestCodeMa: searchRequestCodeMa,
        goTorequestcodesetting: goTorequestcodesetting,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun
    }
    return vm;
});