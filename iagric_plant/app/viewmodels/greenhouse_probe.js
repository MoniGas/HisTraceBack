define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'utils', 'logininfo', 'bootstrap-datepicker', './greenhouse_probe_data', './greenhouse_probe_code'],
    function (router, ko, km, $, jq, dialog, utils, loginInfo, bdp, greenhouse_probe_data, greenhouse_probe_code) {
        var moduleInfo = {
            moduleID: '18000',
            parentModuleID: '10001'
        }
        var searchTitle = ko.observable();
        var bDate = ko.observable();
        var eDate = ko.observable();

        bDate.subscribe(function () {
            searchProbe();
        });
        eDate.subscribe(function () {
            searchProbe();
        });
        //ajax获取数据
        var getData = function (pageIndex, title, bDate, eDate) {
            var sendData = {
                pageIndex: pageIndex,
                title: title,
                bDate: bDate,
                eDate: eDate
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Greenhouses_Probe/Index",
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
            //alert(list.pageIndex());
            vm.vmProbe.ObjList(list.ObjList());
            vm.vmProbe.pageSize(list.pageSize());
            vm.vmProbe.totalCounts(list.totalCounts());
            vm.vmProbe.pageIndex(list.pageIndex());
        }
        //搜索经销商
        var searchProbe = function (data, event) {
            var list = getDataKO(1, searchTitle(), bDate(), eDate());
            updateData(list);
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, title, bDate, eDate) {
            var list = km.fromJS(getData(pageIndex, title, bDate, eDate));
            return list;
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.probePager = {
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
                            var list = getDataKO(num, searchTitle(), bDate(), eDate());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmProbe.pageSize());
                            this.totalCounts = parseInt(vm.vmProbe.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowHand = self.find("div[eleflag='ShowHand']");

            ShowHand.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowHand = self.find("div[eleflag='ShowHand']");

            ShowHand.css({ "display": "none" });
        }
        var prodedataInfo = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();

            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };

            var id = ko.utils.unwrapObservable(id);
            greenhouse_probe_data.show(id);
        };
        var showcode = function (data, event) {

            var currentObj = $(event.target);
            currentObj.blur();

            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };

            greenhouse_probe_code.show();
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmProbe = getDataKO(1, "", "", "");
                updateData(vm.vmProbe);

                $('#bDate').datepicker({
                    language: 'cn',
                    autoclose: true,
                    todayHighlight: true
                });

                $('#eDate').datepicker({
                    language: 'cn',
                    autoclose: true,
                    todayHighlight: true
                });

            },
            goBack: function () {
                router.navigateBack();
            },
            vmProbe: null,
            searchTitle: searchTitle,
            bDate: bDate,
            eDate: eDate,
            searchProbe: searchProbe,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            prodedataInfo: prodedataInfo,
            showcode: showcode
        }
        return vm;
    });