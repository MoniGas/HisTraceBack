define(['plugins/router', 'knockout','jquery', 'knockout.validation', 'utils', 'plugins/dialog', 'jquery-ui','logininfo'],
 function (router, ko,$, kv, utils, dialog, jqueryui,loginInfo) {
     var vmregionalbrand_Add = function () {
         var self = this;
         self.materialModelsArray = [];
         self.selectedModel = ko.observable();
         self.selmaterialmodel = ko.observable(false);
         self.selectedModel.subscribe(function () {
             if (self.selectedModel()) {
                 self.selmaterialmodel(false);
             }
             else {
                 self.selmaterialmodel(true);
             }
         });
         self.brandModelIsArray = [];
         self.selectedBrand = ko.observable();
         self.selbrandmodel = ko.observable(false);
         self.selectedBrand.subscribe(function () {
             if (self.selectedBrand()) {
                 self.selbrandmodel(false);
             }
             else {
                 self.selbrandmodel(true);
             }
         });
         self.calcel = function () {
             self.close(this);
         }
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.selectedModel() && self.selectedBrand()) {
                 var material = 0;
                 if (self.selectedModel()) {
                     material = self.selectedModel();
                 }
                 var brand = 0;
                 if (self.selectedBrand()) {
                     brand = self.selectedBrand();
                 }
                 var sendData = {
                     materialId: material,
                     brandId: brand
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Brand/RequestBrandEnterpriseAdd",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             if (jsonResult.code != 0) {
                                 self.close();
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 if (!self.selectedModel()) {
                     self.selmaterialmodel(true);
                 }
                 if (!self.selectedBrand()) {
                     self.selbrandmodel(true);
                 }
             }
         }
     }
     //获取产品模块
     var getNewsModules = function () {
         var sendData = {
     };
     var data;
     $.ajax({
         type: "POST",
         url: "/Admin_Brand/MaterialList",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             data = jsonResult.ObjList;
         }
     });
     return data;
 }
 //     vmMaterialModels.materialModelsArray(getNewsModules());
 //获取生产基地模块
 var getBrandModules = function () {
     var sendData = {

 };
 var data;
 $.ajax({
     type: "POST",
     url: "/Admin_Brand/BrandList",
     contentType: "application/json;charset=utf-8", //必须有
     dataType: "json", //表示返回值类型，不必须
     data: JSON.stringify(sendData),
     async: false,
     success: function (jsonResult) {
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         data = jsonResult.ObjList;
     }
 });
 return data;
}
// vmBrandModels.brandModelsArray(getBrandModules());
vmregionalbrand_Add.prototype.binding = function () {
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
}
vmregionalbrand_Add.prototype.close = function () {
    //alert(this.province().code);
    dialog.close(this);
}
vmregionalbrand_Add.show = function () {
    var vmObj = new vmregionalbrand_Add();
    vmObj.materialModelsArray = getNewsModules();
    vmObj.brandModelIsArray = getBrandModules();
    return dialog.show(vmObj);
};
return vmregionalbrand_Add;

});
