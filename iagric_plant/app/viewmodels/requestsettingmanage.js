define(['plugins/router', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo', 'plugins/dialog', 'jquery.querystring', './requestsettingmanage_part', './requestcodesettingCode', './PacketEwm', './ActiveUploadFile', './requestsettingmanagememo'],
function (router, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo, dialog, qs, batch_part, requestcodesettingCode, PacketEwm, ActiveUploadFile, requestsettingmanagememo) {
    var moduleInfo = {
        moduleID: '25000',
        parentModuleID: '20000'
    }
    var searchTitle = ko.observable();
    var mName = ko.observable();
    var bName = ko.observable();
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
    var getData = function (pageIndex, searchTitle, mName, bName) {
        var sendData = {
            pageIndex: pageIndex,
            searchName: searchTitle,
            mName: mName,
            bName: bName,
            beginDate: beginDate(),
            endDate: endDate()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/RequestCodeMa/RequestCodeSettinglist",
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
    //跳转至设置页面
    var codesetting = function (subID, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        router.navigate('#requestcodesetting?subid=' + subID);
    }
    //激活上传文件
    var activeUploadFile = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        ActiveUploadFile.show().then(function () {
        });
        //        router.navigate('#brand_Add');
    };
    //同步PI
    var GetPIInfo = function (data, event) {
//        $.post("/RequestCodeMa/GetPIInfo", function (data) {
//            searchRequestCodeMa();
        //        });
        dialog.showMessage("确定要同步PI数据吗?", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {

                }
                $.ajax({
                    type: "POST",
                    url: "/RequestCodeMa/GetPIInfo",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        }
                        else {
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                if (jsonResult.code != 0) {
                                    searchRequestCodeMa();
                                }
                            }
                            }]);
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
    }
    //跳转至设置页面
    var fwcodesetting = function (subID, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        router.navigate('#securitycodesetting?subid=' + subID);
    }
    //跳转至设置页面
    var codelastsetting = function (subID, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        $.post("/Admin_RequestCodeSetting/IsExistSettingInfo", { settingId: subID }, function (data) {
            if (data.ok == 2) {
                if (confirm("已经存在该产品的追溯码信息，是否复制？")) {
                    $.post("/Admin_RequestCodeSetting/CopySettingInfo", { settingId: subID }, function (data) {
                        if (data.ok) {
                            if (confirm("配置信息已经复制完成，是否去调整？")) {
                                router.navigate('#requestcodesetting?subid=' + subID + '&type=1');
                            }
                        }
                    });
                }
                else {
                    router.navigate('#requestcodesetting?subid=' + subID + '&type=1');
                }
            }
            else {
                router.navigate('#requestcodesetting?subid=' + subID + '&type=1');
            }
        });

        //        router.navigate('#requestcodesetting?subid=' + subID + '&type=1');
    }
    //红包跳转页面
    var redPacket = function (subId, data, event, enteriseId, state) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subId);
        $.post("/RequestCodeMa/GetPacketState", { settingId: subID }, function (data) {
            var packetState = data;
            var enterId = ko.utils.unwrapObservable(enteriseId);
            if (packetState == 1) {
                PacketEwm.show(subID);
            }
            else {
                router.navigate("#RedPacket?enterpriseId=" + enterId + "&settingId=" + subID);
            }
        });
    }
    //拆分批次
    var batchpart = function (subID, packetSet, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        packetSet = ko.utils.unwrapObservable(packetSet);
        if (packetSet == 1) {
            alert("该批码已配置过活动信息，不可拆分！");
        }
        else {
            batch_part.show(subID).then(function () {
                searchRequestCodeMa();
                //重新加载数据
            });
        }
    }
    var BatchType = function (Type) {
        Type = ko.utils.unwrapObservable(Type);
        if (Type == 1) {
            return "主批次";
        } else if (Type == 2) {
            return "子批次";
        }

    }
    var RequestCodeType = function (CodeType) {
        CodeType = ko.utils.unwrapObservable(CodeType);
        if (CodeType == 1) {
            return "追溯码";
        } else if (CodeType == 2) {
            return "防伪码";
        } else if (CodeType == 3) {
            return "防伪追溯码";
        }

    }
    var CodeType = function (type) {
        type = ko.utils.unwrapObservable(type);
        if (type == 9) {
            return "套标码";
        } else if (type == 3) {
            return "产品码";
        } else if (type == 10) {
            return "农药码";
        }
        else {
            return "";
        }
    }
    // 查看产品二维码
    var ProductEwm = function (sId, status) {
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var status = ko.utils.unwrapObservable(status);
        var id = ko.utils.unwrapObservable(sId);
        //        if (status == 1040000001 || status == 1040000006 || status == 1040000008) {
        //            alert("该批码还未审核生成！");
        //            return;
        //        }
        //        else {
        requestcodesettingCode.show(id);
        //        }
    }
    //添加/编辑备注信息2018-09-14添加
    var settingmemo = function (subID, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        subID = ko.utils.unwrapObservable(subID);
        requestsettingmanagememo.show(subID).then(function () {
            searchRequestCodeMa();
            //重新加载数据
        });
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
        var self = $(event.target).closest('tr');
        var Setting = self.find("a[eleflag='Setting']");
        var Part = self.find("a[eleflag='Part']");
        var SearchCode = self.find("a[eleflag='SearchCode']");
        var zhuisu = self.find("a[eleflag='zhuisu']");
        var fangwei = self.find("a[eleflag='fangwei']");
        var CodeSetting = self.find("a[eleflag='CodeSetting']");
        var Packet = self.find("a[eleflag='Packet']");
        Setting.css({ "display": "" });
        SearchCode.css({ "display": "" });
        Packet.css({ "display": "" });
        var SettingMemo = self.find("a[eleflag='SettingMemo']");
        SettingMemo.css({ "display": "" });
        var BatchType = self.find("p[eleflag='BatchType']");
        if (BatchType[0].textContent == "主批次") {
            Part.css({ "display": "" });
        }
        var Count = self.find("span[eleflag='SettingCount']");
        if (Count[0].textContent == '0') {
            Part.css({ "display": "none" });
        }
        var codeType = self.find("p[eleflag='CodeType']");
        if (codeType[0].textContent == "农药码") {
            Part.css({ "display": "none" });
        }
        var RequestCodeType = self.find("p[eleflag='RequestCodeType']");
        if (RequestCodeType[0].textContent == "防伪码") {
            Setting.css({ "display": "none" });
            zhuisu.css({ "display": "" });
            CodeSetting.css({ "display": "" });
        }
        else if (RequestCodeType[0].textContent == "追溯码") {
            fangwei.css({ "display": "" });
        }
        else {
            zhuisu.css({ "display": "none" });
            fangwei.css({ "display": "none" });
            CodeSetting.css({ "display": "none" });
        }
    }

    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var Setting = self.find("a[eleflag='Setting']");
        var Part = self.find("a[eleflag='Part']");
        var SearchCode = self.find("a[eleflag='SearchCode']");
        var zhuisu = self.find("a[eleflag='zhuisu']");
        var fangwei = self.find("a[eleflag='fangwei']");
        var CodeSetting = self.find("a[eleflag='CodeSetting']");
        var Packet = self.find("a[eleflag='Packet']");
        Setting.css({ "display": "none" });
        Part.css({ "display": "none" });
        SearchCode.css({ "display": "none" });
        zhuisu.css({ "display": "none" });
        fangwei.css({ "display": "none" });
        CodeSetting.css({ "display": "none" });
        Packet.css({ "display": "none" });
        var SettingMemo = self.find("a[eleflag='SettingMemo']");
        SettingMemo.css({ "display": "none" });
    }
    var showSubBatch = function (id, data, event) {
        var data1;
        var sendData = {
            requestId: id()
        }
        $.ajax({
            type: "POST",
            url: "/RequestCodeMa/RequestCodeSettinglistSub",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                };
                data1 = jsonResult;
            }
        });
        var self;
        if (data == 'addSubBatch') {
            self = $("#" + sourceObj);
        }
        else {
            self = $(event.target);
        }
        var subObjList = km.fromJS(data1.ObjList);
        if (self.hasClass("fa-caret-right")) {
            var currentBatch = ko.utils.arrayFilter(vm.vmRequestCodeMa.ObjList(), function (item) {
                return item.RequestID == id;
            });
            currentBatch[0].subBatchObj(subObjList());
            $("tr[subflag='subBatchObj_" + id() + "']").fadeIn(500);
            self.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
        }
        else {
            if (data == 'addSubBatch') {
                var currentBatch = ko.utils.arrayFilter(vm.vmRequestCodeMa.ObjList(), function (item) {
                    return item.RequestID == id();
                });
                currentBatch[0].subBatchObj(subObjList());
                $("tr[subflag='subBatchObj_" + id() + "']").fadeIn(500);
            }
            else {
                $("tr[subflag='subBatchObj_" + id() + "']").hide();
                self.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
            }
        }
    }
    var myNavigateTo = function (data) {
        //alert(1);
        //router.navigate('requestcodesetting?subid=' + subid + '&type=1');
    }
    //开通追溯/开通防伪
    var editCodeType = function (id, type, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);
        var type = ko.utils.unwrapObservable(type);
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var Message = '确定要开通防伪吗？';
        if (type == 1) {
            Message = '确定要开通防伪吗？';
        }
        else if (type == 2) {
            Message = '确定要开通追溯吗？';
        }
        dialog.showMessage(Message, '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id,
                        requestCodeType: type
                    }
                    $.ajax({
                        type: "POST",
                        url: "/RequestCodeMa/EditType",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code != 0) {
                                        searchRequestCodeMa();
                                    }
                                }
                                }]);
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
        searchTitle: searchTitle,
        mName: mName,
        bName: bName,
        beginDate: beginDate,
        endDate: endDate,
        searchRequestCodeMa: searchRequestCodeMa,
        codesetting: codesetting,
        fwcodesetting: fwcodesetting,
        codelastsetting: codelastsetting,
        BatchType: BatchType,
        RequestCodeType: RequestCodeType,
        CodeType: CodeType,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        batchpart: batchpart,
        ProductEwm: ProductEwm,
        showSubBatch: showSubBatch,
        editCodeType: editCodeType,
        myNavigateTo: myNavigateTo,
        redPacket: redPacket,
        activeUploadFile: activeUploadFile,
        settingmemo: settingmemo,
        GetPIInfo: GetPIInfo
    }
    return vm;
});