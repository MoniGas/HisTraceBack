define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'plugins/dialog', './sellcode_info', './requestcodesales'],
    function (router, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, dialog, sellcode_info, requestcodesales) {
        var getDealer = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Public/GetDealer",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                }
            })
            return data;
        }
        var getNowFormatDate = function () {
            var date = new Date();
            var seperator1 = "-";
            var strYear = date.getFullYear();
            var strMonth = date.getMonth() + 1;
            var strDate = date.getDate();
            if (strMonth >= 1 && strMonth <= 9) {
                strMonth = "0" + strMonth;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
            return currentdate;
        }
        var beginDate = ko.observable(getNowFormatDate());
        var endDate = ko.observable(getNowFormatDate());

        var searchTitle = ko.observable();
        var self;
        var moduleInfo = {
            moduleID: '10070',
            parentModuleID: '10001'
        }
        var vmAddSellCode = function () {
            self = this;
            self.materialName = ko.observable();
            self.dealerId = ko.observable();
            self.dealerId.subscribe(function () {
                if (!self.dealerId()) {
                    self.selDealer(true);
                } else {
                    self.selDealer(false);
                }
            });
            self.selectDealer = getDealer();
            self.birthDate = ko.observable(getNowFormatDate()).extend({
                required: { params: true, message: "请选择生产日期!" }
            });
            self.selBirthDate = ko.observable(false);
            self.beginCode = ko.observable('').extend({
                required: { params: true, message: "请输入起始码!" }
            });
            self.selBeginCode = ko.observable(false);
            self.endCode = ko.observable('').extend({
                required: { params: true, message: "请输入结束码!" }
            });
            self.selEndCode = ko.observable(false);
            self.selDealer = ko.observable(false);
            $('#birthDate').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true
            });
        }
        var isJump = false;
        var SellCode = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.dealerId()) {
                if (self.dealerId()) {
                    $("#btnSell").attr("disabled", true);
                    var sendData = {
                        productionTime: self.birthDate(),
                        dealerId: self.dealerId().Dealer_ID,
                        beginCode: self.beginCode(),
                        endCode: self.endCode()
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_SellCode/SellCode",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            $("#btnSell").removeAttr("disabled");
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                                return;
                            }
                            else {
                                if (isJump) {
                                    dialog.showMessage(jsonResult.Msg + ',是否返回下载列表继续激活.', '系统提示', [{ title: '确定', callback: function () {
                                        vm.goBack();
                                    }
                                    }, { title: '取消', callback: function () {
                                        if (jsonResult.code != 0) {
                                            var list = getDataKO(1);
                                            updateData(list);
                                        }
                                    }
                                    }]);
                                } else {
                                    dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                        if (jsonResult.code != 0) {
                                            var list = getDataKO(1);
                                            updateData(list);
                                        }
                                    }
                                    }]);
                                }
                            }
                        }
                    });
                }
            } else {
                if (!self.dealerId()) {
                    self.selDealer(true);
                }
                else {
                    self.selDealer(false);
                }
                self.errors.showAllMessages();
            }
        }
        //ajax获取数据
        var getData = function (pageIndex) {
            var sendData = {
                beginDate: beginDate(),
                endDate: endDate(),
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_SellCode/Index",
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
            });
            return data;
        }
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex) {
            var list = km.fromJS(getData(pageIndex));
            return list;
        }
        //分页、搜索时更新数据源
        var updateData = function (list) {
            vm.vmSellCode.ObjList(list.ObjList());
            vm.vmSellCode.pageSize(list.pageSize());
            vm.vmSellCode.totalCounts(list.totalCounts());
            vm.vmSellCode.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.sellcodePager = {
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
                    currentPage: 1,
                    first: '<li class="first"><a href="javascript:;">首页</a></li>',
                    last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                    prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                    next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                    page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                    onPageChange: function (num, type) {
                        if (type == 'change') {
                            var list = getDataKO(num);
                            updateData(list);
                            this.pageSize = parseInt(vm.vmSellCode.pageSize());
                            this.totalCounts = parseInt(vm.vmSellCode.totalCounts());
                        }
                    }
                });
            }
        };
        var vmEnterpriseMainCode = function (id) {
            var self = this;
            self.id = id;
            self.MainCode = ko.observable();
            self.init = function () {
                var sendData = {
                    eid: self.id
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_SellCode/EnterpriseMainCode",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    async: false,
                    data: JSON.stringify(sendData),
                    success: function (jsonResult) {
                        if (jsonResult.code == 0) {
                            bootbox.alert({
                                title: "提示",
                                message: "获取数据失败！",
                                buttons: {
                                    ok: {
                                        label: '确定'
                                    }
                                }
                            });
                            return;
                        };
                    }
                });
            }
        }
        var infoSellCode = function (id, EwmTableIdArray, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();

            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            id = ko.utils.unwrapObservable(id);
            EwmTableIdArray = ko.utils.unwrapObservable(EwmTableIdArray);
            sellcode_info.show(id, EwmTableIdArray);
        }
        //搜索
        var searchRequest = function (data, event) {
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            if (searchTitle()) {
                window.open('/Wap_index/index?ewm=' + searchTitle() + '.10');
            }
        };
        var mouseoverFun = function (data, event) {
//            var self = $(event.target).closest('tr');
//            var ShowAll = self.find("button[eleflag='ShowAll']");

//            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
//            var self = $(event.target).closest('tr');
//            var ShowAll = self.find("button[eleflag='ShowAll']");
//            ShowAll.css({ "display": "none" });
        }
        var GetCode = function () {
            var sendData = {
        };
        $.ajax({
            type: "POST",
            url: "/Admin_SellCode/GetCode",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            data: JSON.stringify(sendData),
            success: function (jsonResult) {
                if (jsonResult.code == 1) {
                    vm.vmAddSellCode.beginCode(jsonResult.ObjModel[1]);
                    vm.vmAddSellCode.endCode(jsonResult.ObjModel[2]);
                    isJump = true;
                };
            }
        });
    }
    var init1 = true;
    var init2 = true;
    var onchangeData1 = function () {
        if (init1 == false) {
            var list = getDataKO(1);
            updateData(list);
        }
        init1 = false;
    }
    var onchangeData2 = function () {
        if (init2 == false) {
            var list = getDataKO(1);
            updateData(list);
        }
        init2 = false;
    }

    var ViewUnit = function (Type) {
        Type = ko.utils.unwrapObservable(Type);
        if (Type == 5) {
            return "套";
        } else if (Type == 3) {
            return "个";
        }
    }

    var vm = {
        binding: function () {

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
            $("#select1").css({ "display": "" });
            $("#select2").css({ "display": "none" });
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.vmSellCode = getDataKO(1);
            updateData(vm.vmSellCode);
            vm.vmAddSellCode = new vmAddSellCode();
            vm.vmEnterpriseMainCode = new vmEnterpriseMainCode();
            // 获取默认的开始和结束二维码
            GetCode();
        },
        goBack: function () {
            router.navigateBack();
        },
        vmAddSellCode: vmAddSellCode,
        vmEnterpriseMainCode: vmEnterpriseMainCode,
        SellCode: SellCode,
        vmSellCode: null,
        infoSellCode: infoSellCode,
        searchTitle: searchTitle,
        searchRequest: searchRequest,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        beginDate: beginDate,
        endDate: endDate,
        onchangeData1: onchangeData1,
        onchangeData2: onchangeData2,
        ViewUnit: ViewUnit
    }
    return vm;
});