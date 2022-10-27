define(['plugins/router', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo', './batch_add', './batchext_add', './batch_zuoye_add', './batch_xunjian_add', './batch_jiance_add', 'plugins/dialog', './batch_edit', './batchext_edit', './batch_zuoye', './batch_xunjian', './batch_jiance'],
function (router, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo, batch_add, batchext_add, batch_zuoye_add, batch_xunjian_add, batch_jiance_add, dialog, batch_edit, batchext_edit, batch_zuoye, batch_xunjian, batch_jiance) {
    var batchObj;
    var newsCount = ko.observable(0);
    var searchName = ko.observable();
    var mName = ko.observable();
    var bName = ko.observable();
    //    var complementHistoryDate = function () {
    //        var complDate = [];
    //        var curDate = new Date();
    //        var y = curDate.getFullYear();
    //        var m = curDate.getMonth() + 1;
    //        var d = curDate.getDate();
    //        if (m >= 1 && m <= 9) {
    //            m = "0" + m;
    //        }
    //        if (d >= 0 && d <= 9) {
    //            d = "0" + d;
    //        }
    //        //第一次装入当前的前一个月(格式yyyy-MM-dd)  
    //        complDate[0] = y + "-" + ((m - 1).toString().length == 1 ? "0" + (m - 1) : (m - 1)) + "-" + d;
    //        if (m == 1) {
    //            //到1月后,后推一年  
    //            y--;
    //            m = 12; //再从12月往后推  
    //            complDate[0] = y + "-" + (m.toString().length == 1 ? "0" + m : m) + "-" + d;
    //        }
    //        return complDate;
    //    }
    var complementHistoryDate = function () {
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
        if (strMonth == 1) {
            strYear--;
            strMonth = 12;
        }
        var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
        return currentdate;
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
    //var endDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
    var moduleInfo = {
        moduleID: '10010',
        parentModuleID: '10000'
    }
    //ajax获取数据
    var getData = function (pageIndex, searchName, mName, bName) {
        var sendData = {
            pageIndex: pageIndex,
            searchName: searchName,
            mName: mName,
            bName: bName,
            beginDate: beginDate(),
            endDate: endDate()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Batch/Index",
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
        system.log('batchList:' + JSON.stringify(data));
        return data;
    }
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmBatch.ObjList(list.ObjList());
        vm.vmBatch.pageSize(list.pageSize());
        vm.vmBatch.totalCounts(list.totalCounts());
        vm.vmBatch.pageIndex(list.pageIndex());
    }
    var updatedata = function (list) {
        vm.vmBatch.ObjList(list.ObjList());
    }
    //查询条件
    var searchBatch = function (data, event) {
        var list = getDataKO(1, searchName(), mName(), bName());
        updateData(list);
    };
    //添加批次信息
    var addBatch = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        batch_add.show().then(function () {
            searchBatch();
        });
    };
    //编辑批次信息
    var editBatch = function (id, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }
        var id = ko.utils.unwrapObservable(id);
        batch_edit.show(id).then(function (materialName, batchName, house, type) {
            if (type == 2) {
                data.BatchName(batchName);
                data.MaterialFullName(materialName);
                data.Greenhouseslist(house);
            }
        });
    }
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, searchName, mName, bName) {
        var list = km.fromJS(getData(pageIndex, searchName, mName, bName));
        return list;
    }
    //删除批次
    var delBatch = function (id, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    id: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Batch/Del",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == -100) {
                                window.location.href = "/";
                                return;
                            }
                            if (jsonResult.code != 0) {
                                var currentPageRow = vm.vmBatch.ObjList().length;
                                var pageIndex = vm.vmBatch.pageIndex();
                                if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                    pageIndex = vm.vmBatch.pageIndex() - 1;
                                }
                                var list = getDataKO(pageIndex, searchName(), mName(), bName());
                                updateData(list);
                                this.pageSize = parseInt(vm.vmBatch.pageSize());
                                this.totalCounts = parseInt(vm.vmBatch.totalCounts());
                            }
                        }
                        }]);
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
    var navigateTo = function (routeName, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }
        var row = data;
        switch (routeName) {
            case "batch_add":
                batch_add.show().then(function () {
                    searchBatch();
                });
                break;
            case "AddSubBatch":
                batchext_add.show(row.Batch_ID()).then(function (result) {
                    if (result != undefined && result.code != 0) {
                        //1.计算子批次数量,以后需要改为ajax获取数量显示
                        row.ExtCount(row.ExtCount() + 1);
                        //2.展开子批次面板并刷新数据
                        vm.showSubBatch(row.Batch_ID, 'addSubBatch', undefined, 'iShowSub');
                    }
                });
                break;
            case "AddGongXu":
                batch_zuoye_add.show(row.Batch_ID()).then(function (gongxuName) {
                    if (gongxuName != undefined) {
                        row.nearType(gongxuName);
                    }
                });
                break;
            case "AddXunJian":
                batch_xunjian_add.show(row.Batch_ID()).then(function (isno) {
                    if (isno != undefined) {
                        row.xjcount(isno);
                    }
                });
                break;
            case "AddJianCe":
                batch_jiance_add.show(row.Batch_ID()).then(function (isno) {
                    if (isno != undefined) {
                        row.jyjccount(isno);
                    }
                });
                break;
            case "ZuoYeList":
                var nearType = data.nearType();
                batch_zuoye.show(row.Batch_ID(), 0, nearType).then(function (lastGongXuName) {
                    row.nearType(lastGongXuName);
                });
                break;
            case "XunJianList":
                batch_xunjian.show(row.Batch_ID(), 0).then(function (listCount) {
                    if (listCount == 0) {
                        row.xjcount('无');
                    }
                });
                break;
            case "JianCeList":
                batch_jiance.show(row.Batch_ID(), 0).then(function (listCount) {
                    if (listCount == 0) {
                        row.jyjccount('无');
                    }
                });
                break;
            default:
                router.navigate('#' + routeName);
        }
    }


    //点击展开子批次
    var showSubBatch = function (parentBatchNumber, data, event, sourceObj) {
        var data1;
        var sendData = {
            id: parentBatchNumber()
        }
        $.ajax({
            type: "POST",
            url: "/Admin_BatchExt/Index",
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
            var currentBatch = ko.utils.arrayFilter(vm.vmBatch.ObjList(), function (item) {
                return item.Batch_ID == parentBatchNumber;
            });
            currentBatch[0].subBatchObj(subObjList());
            $("tr[subflag='subBatchObj_" + parentBatchNumber() + "']").fadeIn(500);
            //var trs = $("tr[subflag='subBatchObj_" + parentBatchNumber() + "']");
            self.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
        }
        else {
            if (data == 'addSubBatch') {
                var currentBatch = ko.utils.arrayFilter(vm.vmBatch.ObjList(), function (item) {
                    return item.Batch_ID == parentBatchNumber;
                });
                currentBatch[0].subBatchObj(subObjList());
                $("tr[subflag='subBatchObj_" + parentBatchNumber() + "']").fadeIn(500);
            }
            else {
                $("tr[subflag='subBatchObj_" + parentBatchNumber() + "']").hide();
                self.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
            }
        }
    }
    //子批次添加检测报告
    var subAddJianCe = function (id, beid, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var obj = $(event.target);
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        batch_jiance_add.show(id, beid).then(function (isno) {
            if (isno != undefined) {
                data.jyjccount(isno);
            }
        });
    };
    //子批次添加作业信息
    var subAddZuoYe = function (id, beid, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var obj = $(event.target);
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        batch_zuoye_add.show(id, beid).then(function (selectedModel) {
            if (selectedModel != undefined) {
                //$($(obj.siblings("a")[0]).find("span")[0]).html(selectedModel);
                data.nearOTName(selectedModel);
            }
        });
    }
    //子批次添加巡检信息
    var subAddXunJian = function (id, beid, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var obj = $(event.target);
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        batch_xunjian_add.show(id, beid).then(function (isno) {
            if (isno != undefined) {
                //$($(obj.siblings("a")[0]).find("span")[0]).html(isno);
                data.xjcount(isno);
            }
        });
    }
    //编辑子批次信息
    var editBatchExt = function (id, bid, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        var bid = ko.utils.unwrapObservable(bid);
        batchext_edit.show(id, bid).then(function (batchName) {
            data.BatchExtName(batchName);
        });
    }
    //删除子批次
    var delBatchExt = function (id, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var id = ko.utils.unwrapObservable(id);
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    batchextId: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_BatchExt/Delete",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        }
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code != 0) {
                                //1.排除刚删除的数据
                                var currentBatch = ko.utils.arrayFilter(vm.vmBatch.ObjList(), function (item) {
                                    return item.Batch_ID() == data.Batch_ID();
                                });
                                var currentSubBatchs = currentBatch[0].subBatchObj();
                                var currentSubBatchs1 = ko.utils.arrayFilter(currentSubBatchs, function (item) {
                                    return item.BatchExt_ID() != data.BatchExt_ID();
                                });
                                currentBatch[0].subBatchObj(currentSubBatchs1);
                                //2.更新总批次数
                                currentBatch[0].ExtCount(currentSubBatchs1.length);

                                //alert($.inArray(data, currentSubBatchs));
                                //currentSubBatchs.splice($.inArray(data, currentSubBatchs), 1);
                            }
                        }
                        }]);
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
    //子批次巡检列表
    var xunjianlistBE = function (id, beid, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        batch_xunjian.show(id, beid).then(function (listCount) {
            if (listCount == 0) {
                data.xjcount('无');
                //alert(data.xjcount());
                //var obj = $(event.target);
                //$(obj).html('无');
            }
        });
    }
    //子批次作业列表
    var zuoyelistBE = function (id, beid, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        var nearType = data.nearOTName();
        batch_zuoye.show(id, beid, nearType).then(function (lastGongXuName) {
            data.nearOTName(lastGongXuName);
        });
    }
    //子批次检测报告列表
    var jiancelistBE = function (id, beid, moduleCode, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var isPower = loginInfo.checkPower(moduleCode);
        if (!isPower) {
            dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

            }
            }]);
            return;
        }

        var jsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
            return;
        };
        var id = ko.utils.unwrapObservable(id);
        var beid = ko.utils.unwrapObservable(beid);
        batch_jiance.show(id, beid).then(function (listCount) {
            if (listCount == 0) {
                data.jyjccount('无');
            }
        });
    }
    var mouseoverFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var btnAddSubBatch = self.find("button[eleflag='AddSubBatch']");
        var btnAddGongXu = self.find("button[eleflag='AddGongXu']");
        var btnAddXunJian = self.find("button[eleflag='AddXunJian']");
        var btnAddJianCe = self.find("button[eleflag='AddJianCe']");
        var btnAddXiaoShou = self.find("button[eleflag='AddXiaoShou']");
        var ShowHand = self.find("div[eleflag='ShowHand']");


        btnAddSubBatch.css({ "display": "" });
        btnAddGongXu.css({ "display": "" });
        btnAddXunJian.css({ "display": "" });
        btnAddJianCe.css({ "display": "" });
        btnAddXiaoShou.css({ "display": "" });
        ShowHand.css({ "display": "" });
    }
    var mouseoutFun = function (data, event) {
        var self = $(event.target).closest('tr');
        var btnAddSubBatch = self.find("button[eleflag='AddSubBatch']");
        var btnAddGongXu = self.find("button[eleflag='AddGongXu']");
        var btnAddXunJian = self.find("button[eleflag='AddXunJian']");
        var btnAddJianCe = self.find("button[eleflag='AddJianCe']");
        var btnAddXiaoShou = self.find("button[eleflag='AddXiaoShou']");
        var ShowHand = self.find("div[eleflag='ShowHand']");

        btnAddSubBatch.css({ "display": "none" });
        btnAddGongXu.css({ "display": "none" });
        btnAddXunJian.css({ "display": "none" });
        btnAddJianCe.css({ "display": "none" });
        btnAddXiaoShou.css({ "display": "none" });
        ShowHand.css({ "display": "none" });
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.batchPager = {
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
                        var list = getDataKO(num, searchName(), mName(), bName());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmBatch.pageSize());
                        this.totalCounts = parseInt(vm.vmBatch.totalCounts());
                    }
                }
            });
        }
    };

    /***************消息、温馨提示、投诉管理*******************/
    var show = function (div, data, event) {
        $("#" + div).addClass('open');
    }

    var hide = function (div, data, event) {
        $("#" + div).removeClass('open');
    }
    // 获取二维码申请未读消息
    var getEwmNewsList = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetEwmNewsList",
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
    // 获取加入区域品牌未读消息
    var getBrandNewsList = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetBrandNewsList",
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
    //获取企业审核是否通过
    var getEnterpriseVerify = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetEnterpriseVerifyList",
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
    var getCodeRecord = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/News/GetCodeRecord",
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
    // 去掉时间中的时分秒
    var newDate = function (date) {
        return date.substring(0, 10);
    }
    // 修改申请二维码已读状态
    var updateEwmStatus = function (id) {
        var id = ko.utils.unwrapObservable(id);
        var sendData = {
            id: id
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/News/UpdateEwmStatus",
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
    }
    // 修改加入区域品牌已读状态
    var updateBrandStatus = function (id) {
        var id = ko.utils.unwrapObservable(id);
        var sendData = {
            id: id
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/News/UpdateBrandStatus",
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
    }
    // 温馨提醒数量
    var reminderCount = 0;
    // 投诉数量
    var complaintCount = 0;

    // 获取是否存在产品
    var getMaterial = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetMaterial",
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
    // 获取是否存在生产环节
    var getGongXu = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetZuoYe",
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
    // 获取是否存在品牌
    var getBrand = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetBrand",
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
    // 获取是否存在生产基地
    var getGreenhouses = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetGreenhouses",
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
    // 获取是否存在经销商
    var getDealer = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/GetDealer",
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

    var dayNum = 0;
    var getDayNum = function () {
        return dayNum;
    }

    // 获取是否已经完善企业信息
    var getEnterprise = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Reminder/CompleteEnterpriseInfo",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (obj) {
                if (obj.ObjModel.IsAuthen == 0) {
                    if (obj.ObjModel.SurplusTime != null) {
                        dayNum = obj.ObjModel.SurplusTime;
                    }
                    reminderCount = reminderCount + 1;
                    $("#li_enterpriseInfo").css({ "display": "" });
                } else if (obj.ObjModel.IsAuthen == 1) {
                    $("#li_enterpriseInfo").css({ "display": "none" });
                }
            }
        })
    }

    // 获取温馨提示数量
    var getReminderCount = function () {
        return reminderCount;
    }

    // 获取投诉未读消息
    var getComplaintList = function () {
        var sendData = {
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Complaint/GetList",
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
    // 修改投诉信息已读状态
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

    var getcomplaintCount = function () {
        return complaintCount;
    }

    var ignoreAll = function () {
        ignoreEwm();
        ignoreBrand();
        vm.newsCount(0);
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

    var ignoreBrand = function () {
        var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/News/IgnoreBrand",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == -100) {
                    window.location.href = "/";
                    return;
                } else if (jsonResult.code == 1) {
                    vm.vmBrandNewsList.ObjList([]);
                }
            }
        })
    }

    /************************************/


    var vm = {
        activate: function () {
            //router.navigate('#guide1');
        },
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化dealer列表数据
            vm.vmBatch = getDataKO(1, "");
            vm.vmEwmNewsList = km.fromJS(getEwmNewsList());
            vm.vmBrandNewsList = km.fromJS(getBrandNewsList());
            vm.vmComplaintList = getComplaintList();
            vm.vmEnterpriseVerify = km.fromJS(getEnterpriseVerify());
            vm.vmCodeRecord = km.fromJS(getCodeRecord());

            vm.newsCount(
                parseInt(vm.vmEwmNewsList.totalCounts())
                + parseInt(vm.vmBrandNewsList.totalCounts())
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

            complaintCount = vm.vmComplaintList.totalCounts;

            if (complaintCount > 0) {
                var btnEdit = $("#viewComplaintTipe");
                btnEdit.css({ "display": "none" });
            } else {
                var btnEdit = $("#viewComplaintTipe");
                btnEdit.css({ "display": "" });
            }

            reminderCount = 0;
            getMaterial();
            getGongXu();
            getBrand();
            getGreenhouses();
            getDealer();
            getEnterprise();

            if (reminderCount > 0) {
                var btnEdit = $("#viewReminderTipe");
                btnEdit.css({ "display": "none" });
            } else {

                var btnEdit = $("#viewReminderTipe");
                btnEdit.css({ "display": "" });
            }

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
                content: div2, //$("#div2").html(),
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 0,
                offsetY: 3

            });

            $('#divTip').poshytip(
            {
                content: div3,
                alignTo: 'target',
                alignX: 'center',
                alignY: 'bottom',
                offsetX: 0,
                offsetY: 3

            });
        },
        vmBatch: null,
        vmEwmNewsList: null,
        vmBrandNewsList: null,
        vmComplaintList: null,
        vmEnterpriseVerify: null,
        vmCodeRecord: null,
        searchBatch: searchBatch,
        searchName: searchName,
        mName: mName,
        bName: bName,
        beginDate: beginDate,
        endDate: endDate,
        addBatch: addBatch,
        editBatch: editBatch,
        delBatch: delBatch,
        editBatchExt: editBatchExt,
        delBatchExt: delBatchExt,
        navigateTo: navigateTo,
        showSubBatch: showSubBatch,
        subAddZuoYe: subAddZuoYe,
        subAddJianCe: subAddJianCe,
        subAddXunJian: subAddXunJian,
        xunjianlistBE: xunjianlistBE,
        zuoyelistBE: zuoyelistBE,
        jiancelistBE: jiancelistBE,
        mouseoverFun: mouseoverFun,
        mouseoutFun: mouseoutFun,
        show: show,
        hide: hide,
        newDate: newDate,
        updateBrandStatus: updateBrandStatus,
        updateEwmStatus: updateEwmStatus,
        newsCount: newsCount,
        getReminderCount: getReminderCount,
        updateComplaint: updateComplaint,
        getcomplaintCount: getcomplaintCount,
        loginInfo: loginInfo,
        ignoreAll: ignoreAll,
        getDayNum: getDayNum,
        complementHistoryDate: complementHistoryDate,
        getNowFormatDate: getNowFormatDate
    }
    return vm;
});