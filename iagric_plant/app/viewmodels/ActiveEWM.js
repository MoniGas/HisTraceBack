define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'utils', 'logininfo', 'bootstrap-datepicker', './ActiveUploadFile', './ActiveBatch'],
    function (router, ko, km, $, jq, dialog, utils, loginInfo, bdp, ActiveUploadFile, ActiveBatch) {
        var moduleInfo = {
            moduleID: '63300',
            parentModuleID: '63000'
        }
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
        var type = ko.observable(0); ;
        var OpType = function (OperationType) {
            OperationType = ko.utils.unwrapObservable(OperationType);
            if (OperationType == 1) {
                return "后台";
            } else if (OperationType == 2) {
                return "流水线";
            }
        }
        //选择批次激活
        var activeBatch = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            ActiveBatch.show().then(function () {
                searchStore();
            });
        };
        //激活上传文件
        var activeUploadFile = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            ActiveUploadFile.show().then(function () {
                searchStore();
            });
        };
        //ajax获取数据
        var getData = function (pageIndex) {
            var sendData = {
                pageIndex: pageIndex,
                type: type(),
                beginDate: beginDate(),
                endDate: endDate()
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/File/AcriveEWMList",
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
            vm.vmStore.ObjList(list.ObjList());
            vm.vmStore.pageSize(list.pageSize());
            vm.vmStore.totalCounts(list.totalCounts());
            vm.vmStore.pageIndex(list.pageIndex());
        }
        //搜索仓库库存
        var searchStore = function (data, event) {
            var list = getDataKO(1, type(), beginDate(), endDate());
            updateData(list);
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, type, beginDate, endDate) {
            var list = km.fromJS(getData(pageIndex, type, beginDate, endDate));
            return list;
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.dealerPager = {
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
                            var list = getDataKO(num);
                            updateData(list);
                            this.pageSize = parseInt(vm.vmStore.pageSize());
                            this.totalCounts = parseInt(vm.vmStore.totalCounts());
                        }
                    }
                });
            }
        };
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmStore = getDataKO(1);
                updateData(vm.vmStore);
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
            vmStore: null,
            beginDate: beginDate,
            endDate: endDate,
            type: type,
            searchStore: searchStore,
            OpType: OpType,
            activeUploadFile: activeUploadFile,
            activeBatch: activeBatch
        }
        return vm;
    });