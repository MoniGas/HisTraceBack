define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'plugins/dialog', './material_balance_info', './requestcodesales'],
    function (router, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, dialog, sellcode_info, requestcodesales) {
        var getBatch = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Public/GetBatch",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }
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
        var searchTitle = ko.observable();
        var self;
        var moduleInfo = {
            moduleID: '30000',
            parentModuleID: '10001'
        }
        var vmAddSellCode = function () {
            self = this;
            self.materialName = ko.observable();

            self.dealerId = ko.observable();
            self.selectDealer = getDealer();

            self.birthDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd')).extend({
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


            self.batchId = ko.observable();
            self.batchExtId = ko.observable();

            self.selectbatch = getBatch();
            self.selectbatchExt = ko.observableArray();

            self.selBatch = ko.observable(false);
            self.selDealer = ko.observable(false);

            self.batchId.subscribe(function () {
                var defaultItem = { BatchExtName: '暂无子批次', BatchExt_ID: '-1' };
                self.batchExtId(undefined);
                if (!self.batchId()) {
                    //self.areas(undefined);
                    self.selectbatchExt(defaultItem);
                    self.materialName('');
                    return;
                }
                var selectedCode = self.batchId().Batch_ID;

                self.materialName(self.batchId().MaterialFullName);

                var sendData = {
                    bid: selectedCode
                };
                $.ajax({
                    type: "POST",
                    url: "/Public/GetBatchExt",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    async: false,
                    data: JSON.stringify(sendData),
                    success: function (jsonResult) {
                        self.selectbatchExt(jsonResult.ObjList);
                    }
                });
            });

            $('#birthDate').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true
            });

        }

        var SellCode = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                if (!self.batchId()) {
                    self.selBatch(true);
                }
                if (!self.dealerId()) {
                    self.selDealer(true);
                }
                if (self.batchId() && self.dealerId()) {
                    //alert(3);
                    $("#btnSell").attr("disabled", true);
                    var batchExt = 0;
                    if (self.batchExtId()) {
                        batchExt = self.batchExtId().BatchExt_ID;
                    }
                    var sendData = {
                        bId: self.batchId().Batch_ID,
                        beId: batchExt,
                        salePlaceId: self.dealerId().Dealer_ID,
                        saleDate: self.birthDate(),
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
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code != 0) {
                                        var list = getDataKO(1);
                                        updateData(list);
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            } else {
                if (!self.batchId()) {
                    self.selBatch(true);
                }
                else {
                    self.selBatch(false);
                }
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
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Material_OnlineOrder/GetBalanceList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    //alert(jsonResult);
                    //var obj = JSON.parse(jsonResult);
                    //alert(JSON.stringify(jsonResult.ObjList));
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                        return;
                    };
                    data = jsonResult;
                },
                error: function (e) {
                    alert(e);
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
                        //(jsonResult.ObjList.MainCode);
                        //self.selectbatchExt(jsonResult.ObjList);
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
        var infoSellCode = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();

            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            //            var id = ko.utils.unwrapObservable(id);
            //            requestcodesales.show(id);

            var id = ko.utils.unwrapObservable(id);
            sellcode_info.show(id);
        };
        //搜索
        var searchRequest = function (data, event) {
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            if (searchTitle()) {
                window.open('/Wap_Preview/index?ewm=' + searchTitle());
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowAll = self.find("button[eleflag='ShowAll']");

            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowAll = self.find("button[eleflag='ShowAll']");

            ShowAll.css({ "display": "none" });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                vm.vmSellCode = getDataKO(1);
                updateData(vm.vmSellCode);
                vm.vmAddSellCode = new vmAddSellCode();
                vm.vmEnterpriseMainCode = new vmEnterpriseMainCode();
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
            mouseoutFun: mouseoutFun
        }
        return vm;
    });