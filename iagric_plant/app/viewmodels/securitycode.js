define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './materialaddsimple'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, materialaddsimple) {
     var moduleInfo = {
         moduleID: '22000',
         parentModuleID: '20000'
     }
     var requestcodeID = ko.observable();
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
     //套标规格
     var SelTypeModel = {
         SelTypeArray: ko.observable(),
         SelectedId: ko.observable()
     }
     var SpecificationsModel = {
         SpecificationsArray: ko.observableArray(),
         SelectedId: ko.observable()
     }

     SpecificationsModel.SelectedId.subscribe(function () {
         if (SpecificationsModel.SelectedId()) {
             SelSpecification(false);
         }
         else {
             SelSpecification(true);
         }
     });
     var Number = ko.observable('').extend({
         min: {
             params: 1,
             message: "数量最少为1！"
         },
         max: {
             params: 100000,
             message: "最多输入100000！"
         },
         digit: {
             params: true,
             message: "生成数量为整数！"
         },
         required: {
             params: true,
             message: "请输入生成数量！"
         }
     });
     var SelNumber = ko.observable(false);
     var AddDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     var shengchengDate = ko.observable(false);
     var SelMaterial = ko.observable(false);
     //获取套标规格
     var SelSpecification = ko.observable(false);
     var SelType = ko.observable(false);


     //获取规格
     var GetSpecificationsList = function () {
         var sendData = {
     }
     var data;
     $.ajax({
         type: "POST",
         url: "/Admin_Specification/GetSelectList",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                 return;
             };
             data = jsonResult.ObjList;
             //alert(JSON.stringify(data));
         }
     });
     return data;
 }
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
 /*******************************生成码点击下一步*****************************************/
 var GenerateOne = function () {
     var currentObj = $(event.target);
     currentObj.blur();
     var errors = ko.validation.group(this);
     if (errors().length <= 0 && SelTypeModel.SelectedId() && (ko.utils.unwrapObservable(SelTypeModel.SelectedId) == 9 ? SpecificationsModel.SelectedId() : 1 == 1) && VmMaterial.SelectedId()) {
         $("#First").hide();
         $("#Second").show();
         initThree();
     } else {
         if (!VmMaterial.SelectedId()) {
             SelMaterial(true);
         }
         if (!SpecificationsModel.SelectedId()) {
             SelSpecification(true);
         }
         if (Number() != null && Number() != "") {
             SelNumber(false);
         }
         else {
             SelNumber(true);
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
             styleArray(jsonResult.ObjModel)
         }
     },
     error: function (e) {
     }
 });
}

/*******************************完成*****************************************/
var Generate = function () {
    var currentObj = $(event.target);
    currentObj.blur();
    var errors = ko.validation.group(this);
    if (errors().length <= 0 && VmMaterial.SelectedId()) {
        var guigeid = 0;
        if (SelTypeModel.SelectedId() == 9) {
            guigeid = SpecificationsModel.SelectedId();
        }
        var type = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
        var sendData = {
            Material_ID: VmMaterial.SelectedId(),
            RequestCount: Number(),
            guigeId: guigeid,
            type: type,
            requestDate: AddDate()
        }
        $.ajax({
            type: "POST",
            url: "/Admin_Request/SecurityCode",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                if (jsonResult.code != 0) {
                    Number('');
                    SelNumber(false);
                    VmMaterial.SelectedId(undefined);
                    SelMaterial(false);
                    //套标规格
                    SelType(false);
                    SelSpecification(false);
                    SpecificationsModel.SelectedId(undefined);
                    SelTypeModel.SelectedId(undefined);
                    router.navigate('#requestsettingmanage');
                    //router.navigate('#GenerateSuccess?subid=' + jsonResult.ObjModel.ID);
                    //dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
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
var TopSubPageClick = function (self, data, event) {
    $("#First").hide();
    $("#Second").hide();
    $("#" + self).show();
}
var ViewModel = {
    binding: function () {
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        SelNumber(false);
        VmMaterial.SelectedId(undefined);
        SelMaterial(false);

        VmMaterial.MaterialArray(GetMaterialList());
        ViewModel.VmGenerateCode = GetDataKO(1);
        $('#AddDate').datepicker({
            language: 'cn',
            autoclose: true,
            todayHighlight: true
        });
        //套标规格
        SelType(false);
        SelSpecification(false);
        SpecificationsModel.SelectedId(undefined);
        SelTypeModel.SelectedId(undefined);
        SpecificationsModel.SpecificationsArray(GetSpecificationsList());
        SelTypeModel.SelTypeArray([{ "Text": "套标产品码", "Value": 9 }, { "Text": "单品产品码", "Value": 3}]);

        SelTypeModel.SelectedId.subscribe(function () {
            var TypeIndex = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
            if (TypeIndex == 9) {
                SelType(false);
                $("#DivSpecifications").css({ "display": "" });
                $("#divTao").css({ "display": "" });
                $("#divCodeCount").css({ "display": "none" });
            } else if (TypeIndex == 3) {
                SelType(false);
                $("#DivSpecifications").css({ "display": "none" });
                $("#divTao").css({ "display": "none" });
                $("#divCodeCount").css({ "display": "" });
            } else {
                SelType(true);
            }
        });

        $("#DivSpecifications").css({ "display": "none" });
    },
    goBack: function () {
        router.navigateBack();
    },
    VmGenerateCode: null,
    requestcodeID: requestcodeID,
    VmMaterial: VmMaterial,
    AddDate: AddDate,
    shengchengDate: shengchengDate,
    Number: Number,
    SelNumber: SelNumber,
    onchangeData: onchangeData,
    addmaterial: addmaterial,
    Generate: Generate,
    SelMaterial: SelMaterial,
    GenerateOne: GenerateOne,
    styleArray: styleArray,
    TopSubPageClick: TopSubPageClick,
    //套标规格
    SelTypeModel: SelTypeModel,
    SpecificationsModel: SpecificationsModel,
    SelSpecification: SelSpecification,
    SelType: SelType
}
return ViewModel;
});