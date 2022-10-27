define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'jquery.querystring', 'plugins/dialog', 'utils', 'logininfo', './brand_add', './material_add', "./greenhouse_add"],
 function (router, ko, $, bdp, kv, qs, dialog, utils, loginInfo, brand_add, material_add, greenhouse_add) {
     var parentCode;
     var moduleInfo = {
         moduleID: '14000',
         parentModuleID: '10000'
     }
     var batchId = 0;
     var vmguide = function () {
         var self = this;
         //         self.brand = ko.observable(0);
         //         self.material = ko.observable(0);
         self.brand = ko.observable(0);
         self.material = ko.observable(0);
         self.materials = ko.observableArray();
         self.brands = [];

         self.greenhouse = ko.observable(0);
         self.batch = ko.observable('').extend({
             maxLength: { params: 50, message: "批次号最大长度为50个字符！" },
             required: { params: true, message: "请输入批次号！" }
         });

         self.selbrand = ko.observable();
         self.selBatch = ko.observable(false);
         self.selMaterial = ko.observable(false);
         self.selGreenhouse = ko.observable(false);
         self.brand.subscribe(function () {
             var defaultItem = { MaterialFullName: '暂无相应产品', Material_ID: '-1' };
             self.material(undefined);
             //             if (!self.brand()) {
             //                 //self.materials(defaultItem);
             //                 return;
             //             }
             var bid = self.brand();
             var sendData = {
                 brandid: bid
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch/GuidMaterialList",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 data: JSON.stringify(sendData),
                 success: function (jsonResult) {
                     self.materials(jsonResult.ObjList);
                 }
             });
         });

         self.material.subscribe(function () {
             if (!self.material()) {
                 self.selbrand(true);
             }
             else {
                 self.selbrand(false);
             }
         });
         self.greenHouseArry = ko.observableArray();
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.greenhouse() && self.material()) {
                 var sendData = {
                     materialId: self.material(),
                     batchName: self.batch(),
                     greenhousesId: self.greenhouse()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch/Add",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (jsonResult.code != 0) {
                             var batchId = jsonResult.ObjModel.Batch_ID;
                             router.navigate('#guide2?id=' + batchId + '&pc=' + parentCode);
                         }
                         else {
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                     }
                 });
             }
             else {

                 if (self.material()) {
                     self.selMaterial(false);
                 }
                 else {
                     self.selMaterial(true);
                 }
                 if (self.greenhouse()) {
                     self.selGreenhouse(false);
                 }
                 else {
                     self.selGreenhouse(true);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     /************************************************************************/
     //获取品牌模块
     var getBrandModules = function () {
         var data;
         var sendData = {
             pageIndex: 1
         }
         $.ajax({
             type: "POST",
             url: "/Admin_Brand/SelectBrand",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     /************************************************************************/
     //获取产品模块
     var getMaterialModules = function () {
         var sendData = {
             brandid: vm.vmguide.brand()
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch/GuidMaterialList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             },
             error: function (e) {
                 alert(e);
             }
         });
         return data;
     }
     /************************************************************************/
     //获生产基地模块
     var getGreenHouseModules = function () {
         var sendData = {
     };
     var data;
     $.ajax({
         type: "POST",
         url: "/Admin_Greenhouse/GreenHouseList",
         contentType: "application/json;charset=utf-8", //必须有
         dataType: "json", //表示返回值类型，不必须
         data: JSON.stringify(sendData),
         async: false,
         success: function (jsonResult) {
             data = jsonResult.ObjList;
         },
         error: function (e) {
             alert(e);
         }
     });
     return data;
 }
 /*******************************添加品牌*****************************************/
 var addbrand = function (data, event) {
     var currentObj = $(event.target);
     currentObj.blur();
     brand_add.show().then(function (id) {
         var list = getBrandModules();
         vm.vmguide.brands = list;
         vm.vmguide.brand(id);
     });
 };
 /*******************************添加产品*****************************************/
 var addmaterial = function (data, event) {
     var currentObj = $(event.target);
     currentObj.blur();
     material_add.show(vm.vmguide.brand()).then(function (id, brandId) {
         var list = getMaterialModules();
         vm.vmguide.materials(list);

         var bid = parseInt(vm.vmguide.brand(), 10).toString();
         if (brandId == vm.vmguide.brand() || bid == "NaN") {
             vm.vmguide.material(id);
         }
     });
 }
 //添加生产基地
 var addgreenhouse = function (data, event) {
     var currentObj = $(event.target);
     currentObj.blur();
     greenhouse_add.show().then(function (id) {
         var list = getGreenHouseModules();
         vm.vmguide.greenHouseArry = list;
         vm.vmguide.greenhouse(id);
     });
 }
 var next = function (data, event) {
     router.navigate('#guide2?id=1&pc=' + parentCode);
 };
 var vm = {
     binding: function () {
         //初初化导航状态
         parentCode = qs.querystring("pc");
         if (parentCode == undefined || parentCode == '') {
             parentCode = moduleInfo.parentModuleID;
         }
         //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
         vm.vmguide = new vmguide();
         vm.vmguide.brands = getBrandModules();
         //        vm.vmguide.materialArry = getMaterialModules();
         vm.vmguide.greenHouseArry = getGreenHouseModules();
     },
     vmguide: null,
     addbrand: addbrand,
     addmaterial: addmaterial,
     addgreenhouse: addgreenhouse,
     next: next
 }
 return vm;
});