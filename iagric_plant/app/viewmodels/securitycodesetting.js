define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './materialaddsimple', 'jquery.querystring'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, materialaddsimple, qs) {
     var moduleInfo = {
         moduleID: '25300',
         parentModuleID: '25000'
     }
     var subId = 0;
     var requestcodeID = ko.observable();
     var displayOption = '';
     var requestCodeType = ko.observable();
     var VmMaterial = {
         MaterialArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     VmMaterial.SelectedId.subscribe(function () {
         if (VmMaterial.SelectedId()) {
             SelMaterial(false);
         } else {
             SelMaterial(true);
         }
     });


     var SelMaterial = ko.observable(false);

     //获取产品
     var GetMaterialList = function () {

         var sendData = {
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/Admin_Request/SearchNameList",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                 return;
             };
             data = jsonResult.ObjList;
         }
     });
     return data;
 }

 //ajax获取数据
 var GetData = function (pageIndex) {
 }

 //分页、搜索时更新数据源
 var UpdateData = function (list) {
     ViewModel.VmGenerateCode.ObjList(list.ObjList());
     ViewModel.VmGenerateCode.pageSize(list.pageSize());
     ViewModel.VmGenerateCode.totalCounts(list.totalCounts());
     ViewModel.VmGenerateCode.pageIndex(list.pageIndex());
 }

 //把获取的ajax数据转化为ko
 var GetDataKO = function (pageIndex) {
     var list = km.fromJS(GetData(pageIndex));
     return list;
 }
 var init = true;
 var onchangeData = function () {
     if (init == false) {
         var list = GetDataKO(1);
         UpdateData(list);
     }
     init = false;
 }
 /*******************************添加产品*****************************************/
 var addmaterial = function (data, event) {
     var currentObj = $(event.target);
     currentObj.blur();
     materialaddsimple.show().then(function (id, brandId) {
         var list = GetMaterialList();
         ViewModel.VmMaterial.MaterialArray(list);
         ViewModel.VmMaterial.SelectedId(id);
     });
 }
 function firstInit() {
     var sendData = {
         subId: subId
     };
     $.ajax({
         type: "POST",
         url: "/Admin_RequestCodeSetting/BatchPart",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             if (jsonResult.code != 0) {
                 dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
             }
             else {
                 ViewModel.VmMaterial.SelectedId(jsonResult.ObjModel.materialId)
                 requestCodeType(jsonResult.ObjModel.requestCodeType)
                 if (requestCodeType() == 2) {
                     $("input[name='Check1']").prop("checked", true);
                 } else if (requestCodeType() == 1) {
                     $("input[name='Check2']").prop("checked", true);
                 } else {
                     $("input[name='Check1']").prop("checked", true);
                     $("input[name='Check2']").prop("checked", true);
                 }
             }
         },
         error: function (e) {
         }
     });
 }
 /*******************************生成码点击下一步*****************************************/
 var GenerateOne = function () {
     var currentObj = $(event.target);
     currentObj.blur();
     var errors = ko.validation.group(this);
     if ($("input[name='Check1']").prop("checked") == false && $("input[name='Check2']").prop("checked") == true) {
         displayOption = 1;
     }
     else if ($("input[name='Check1']").prop("checked") == true && $("input[name='Check2']").prop("checked") == false) {
         displayOption = 2;
     }
     else if ($("input[name='Check1']").prop("checked") == true && $("input[name='Check2']").prop("checked") == true) {
         displayOption = 3;
     }
     if (displayOption == '') {
         dialog.showMessage('至少选择一项码用途！', '系统提示', [{ title: '确定', callback: function () { } }]);
         return;
     }
     if (errors().length <= 0 && VmMaterial.SelectedId()) {
         //         $("#First").hide();
         //         $("#Second").show();
         //         initThree();
         var sendData = {
             id: subId,
             materialId: VmMaterial.SelectedId(),
             requestCodeType: displayOption
         }
         $.ajax({
             type: "POST",
             url: "/RequestCodeMa/EditTypeTwo",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     //router.navigate('#requestcodemanager');
                 }
                 else {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
             }
         })
     } else {
         if (!VmMaterial.SelectedId()) {
             SelMaterial(true);
         }
         errors.showAllMessages();
     }
 }
 /*******************************第二步页面配置*****************************************/
 var styleArray = ko.observableArray();
 var selectStyle = ko.observable();

 function initThree() {
     var sendData = {
 };
 $.ajax({
     type: "POST",
     url: "/Admin_RequestCodeSetting/GenerateThree",
     contentType: "application/json;charset=utf-8", //必须有
     dataType: "json", //表示返回值类型，不必须
     data: JSON.stringify(sendData),
     async: false,
     success: function (jsonResult) {
         if (jsonResult.code != 0) {
             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
         }
         else {
             styleArray(jsonResult.ObjModel)
         }
     },
     error: function (e) {
     }
 });
}

/*******************************第二步页面配置*****************************************/
var styleArray2 = ko.observableArray();
var selectStyle2 = ko.observable();

function initThree2() {
    var sendData = {
};
$.ajax({
    type: "POST",
    url: "/Admin_RequestCodeSetting/GenerateThreeTwo",
    contentType: "application/json;charset=utf-8", //必须有
    dataType: "json", //表示返回值类型，不必须
    data: JSON.stringify(sendData),
    async: false,
    success: function (jsonResult) {
        if (jsonResult.code != 0) {
            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
        }
        else {
            styleArray2(jsonResult.ObjModel)
        }
    },
    error: function (e) {
    }
});
}

/*******************************完成*****************************************/
var Generate = function () {
    dialog.showMessage('页面风格设置成功！', '系统提示', [{ title: '确定', callback: function () { } }]);
}
var GenerateTwo = function () {
    dialog.showMessage('页面风格设置成功！', '系统提示', [{ title: '确定', callback: function () { } }]);
}
//var TopSubPageClick = function (self, data, event) {
//    $("#First").hide();
//    $("#" + self).show();
//    $("#Second").hide();
//}
var TopSubPageClick2 = function (index, divname, data, event) {
        if (index == 2) {
            initThree2();
        }
        for (var i = 1; i < 2; i++) {
            $("#li" + i).removeClass("active");
        }
        $("#li" + index).addClass("active");
        $("#First").hide();
        $("#Second").hide();
        $("#Third").hide();
        $("#" + divname).show();
}
//var TopSubPageClick = function (index, divname, data, event) {
//    if (index == 2) {
//        initThree2();
//    }
//    for (var i = 1; i < 2; i++) {
//        $("#li" + i).removeClass("active");
//    }
//    $("#li" + index).addClass("active");
//    $("#First").hide();
//    $("#Second").hide();
//    $("#" + divname).show();
//}
var TopSubPageClick = function (index, divname, data, event) {
    if ($("input[name='Check1']").prop("checked") == false && $("input[name='Check2']").prop("checked") == true) {
        displayOption = 1;
    }
    else if ($("input[name='Check1']").prop("checked") == true && $("input[name='Check2']").prop("checked") == false) {
        displayOption = 2;
    }
    else if ($("input[name='Check1']").prop("checked") == true && $("input[name='Check2']").prop("checked") == true) {
        displayOption = 3;
    }
    if (displayOption == 2) {
        index = 3;
        divname = Third;
        initThree2();
        $("#li" + index).addClass("active");
        $("#First").hide();
        $("#Second").hide();
        $("#Third").hide();
        $(divname).show();
    }
    else if (displayOption == 1 || displayOption == 3) {
        index = 2;
        divname = Second;
        initThree();
        $("#li" + index).addClass("active");
        $("#First").hide();
        $("#Second").hide();
        $("#Third").hide();
        $(divname).show();
    }
}
var ViewModel = {
    binding: function () {
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        //subId = qs.querystring("subid");
        subId = qs.querystring("subid");
        firstInit();
        VmMaterial.MaterialArray(GetMaterialList());
        ViewModel.VmGenerateCode = GetDataKO(1);
    },
    goBack: function () {
        router.navigateBack();
    },
    VmGenerateCode: null,
    subId: subId,
    requestcodeID: requestcodeID,
    displayOption: displayOption,
    requestCodeType: requestCodeType,
    VmMaterial: VmMaterial,
    onchangeData: onchangeData,
    addmaterial: addmaterial,
    Generate: Generate,
    SelMaterial: SelMaterial,
    GenerateOne: GenerateOne,
    styleArray: styleArray,
    styleArray2: styleArray2,
    TopSubPageClick2: TopSubPageClick2,
    TopSubPageClick: TopSubPageClick,
    GenerateTwo: GenerateTwo
}
return ViewModel;
});