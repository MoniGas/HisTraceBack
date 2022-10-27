define(['plugins/router', 'bootbox', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo'],
function (router, bootbox, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo) {
    // 投诉数量
    var complaintCount = 0;
    var newsCount = ko.observable(0);
    // 温馨提醒数量
    var reminderCount = 0;
    /*****************获取投诉未读消息*************************/
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
/***********************获取投诉数量**********************************/
var getcomplaintCount = function () {
    return complaintCount;
}
/***********************修改投诉信息已读状态*******************/
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
/*************获取二维码申请未读消息**********************/
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
/******************获取企业审核是否通过*****************/
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
/*******************获取二维码审核通过信息***********************/
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
/*******************忽略***********************/
var ignoreAll = function () {
    ignoreEwm();
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
/*****************获取是否存在产品*****************/
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
/*****************获取是否存在生产环节*****************/
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
/*****************获取是否存在品牌*****************/
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
/*****************获取是否存在生产基地*****************/
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
/***********************获取是否已经完善企业信息***********************/
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
/***********************获取是否存在经销商**********************/
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
/*********************获取温馨提示数量**************************/
var getReminderCount = function () {
    return reminderCount;
}
/***************消息、温馨提示、投诉管理*******************/
var show = function (div, data, event) {
    $("#" + div).addClass('open');
}

var hide = function (div, data, event) {
    $("#" + div).removeClass('open');
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
var getHomeDataStatis = function () {
    var sendData = {
}
$.ajax({
    type: "POST",
    url: "/Complaint/GetHomeDataStatis",
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
    }
})
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
/******************路径跳转***********************/
//var navigateTo = function (routeName, moduleCode, data, event) {
//    var currentObj = $(event.target);
//    currentObj.blur();
//    var jsonResult = loginInfo.isLoginTimeoutForServer();
//    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
//        return;
//    };
//    var isPower = loginInfo.checkPower(moduleCode);
//    if (!isPower) {
//        dialog.showMessage('您没有操作权限!', '系统提示', [{ title: '确定', callback: function () {

//        }
//        }]);
//        return;
//    }
//    router.navigate('#' + routeName);
//}
/******************去掉时间中的时分秒***********************/
var newDate = function (date) {
    return date.substring(0, 10);
}
/**********************************************************/
var vm = {
    activate: function () {
    },
    binding: function () {
        vm.vmComplaintList = getComplaintList();
        vm.vmEwmNewsList = km.fromJS(getEwmNewsList());
        vm.vmEnterpriseVerify = km.fromJS(getEnterpriseVerify());
        vm.vmCodeRecord = km.fromJS(getCodeRecord());
        complaintCount = vm.vmComplaintList.totalCounts;
        reminderCount = 0;
        getMaterial();
        getGongXu();
        getGreenhouses();
        getDealer();
        getEnterprise();
        getHomeDataStatis();
        getUpdate();
        if (complaintCount > 0) {
            var btnEdit = $("#viewComplaintTipe");
            btnEdit.css({ "display": "none" });
        } else {
            var btnEdit = $("#viewComplaintTipe");
            btnEdit.css({ "display": "" });
        }
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
    vmComplaintList: null,
    getcomplaintCount: getcomplaintCount,
    getReminderCount: getReminderCount,
    newsCount: newsCount,
    updateComplaint: updateComplaint,
    newDate: newDate,
    show: show,
    hide: hide,
    ignoreAll: ignoreAll,
    RequestCodeCount: RequestCodeCount,
    RequestCodeTimes: RequestCodeTimes,
    ScanCodeTimes: ScanCodeTimes,
    BrancCount: BrancCount,
    MaterialCount: MaterialCount,
    OriginCount: OriginCount,
    ProcessCount: ProcessCount,
    Update: Update
}
return vm;
/******************************************************/
});