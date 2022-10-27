define(['plugins/router','jquery', 'knockout.validation', 'utils','logininfo', 'jquery.querystring', 'plugins/dialog', 'knockout', 'jquery-ui'],
 function (router,$, kv, utils,loginInfo, qs, dialog, ko, jqueryui) {
     var vmBatch_Edit = function () {
         var self = this;
         self.id;
         self.BatchName = ko.observable('').extend({
             minLength: { params: 2, message: "名称最小长度为2个字符" },
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入批次号！"
             }
         });
         self.selTitle = ko.observable(false);
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
         self.greenHouseModelsArray = [];
         self.selectedGreenhouse = ko.observable();
         self.selgreenhousemodel = ko.observable(false);
         self.selectedGreenhouse.subscribe(function () {
             if (self.selectedGreenhouse()) {
                 self.selgreenhousemodel(false);
             }
             else {
                 self.selgreenhousemodel(true);
             }
         });
         self.init = function (id) {
             self.id = id;
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch/GetModelB",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     obj = JSON.parse(jsonResult);
                     self.BatchName(obj.ObjModel.BatchName);
                     self.selectedModel(obj.ObjModel.Material_ID);
                     self.selectedGreenhouse(obj.ObjModel.Greenhouses_ID);

                 }
             })
         }
         self.calcel = function () {
             self.close(this);
         }
         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.selectedModel() && self.selectedGreenhouse()) {
                 var material = 0;
                 if (self.selectedModel()) {
                     material = self.selectedModel();
                 }
                 var greenhouses = 0;
                 if (self.selectedGreenhouse()) {
                     greenhouses = self.selectedGreenhouse();
                 }
                 var sendData = {
                     id: self.id,
                     materialId: material,
                     batchName: self.BatchName(),
                     greenhousesId: greenhouses
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch/Edit",
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
                                 var materialName = $("#ddlMaterialList").find("option:selected").text();
                                 var house = $("#ddlGreenHouseList").find("option:selected").text();
                                 self.closeOK(materialName, sendData.batchName, house);
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
                 if (!self.selectedGreenhouse()) {
                     self.selgreenhousemodel(true);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     //获取产品模块
     var getNewsModules = function () {
         var sendData = {
             mc: '10007'
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch/MaterialList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 //                 alert(jsonResult);
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     //获取生产基地模块
     var getGreenHouseModules = function () {
         var sendData = {
             greenName: "",
             greenewm: ""
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch/GreenHouseList",
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
     vmBatch_Edit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmBatch_Edit.prototype.close = function () {
         dialog.close(this, "", "", "", 1);
     }
     vmBatch_Edit.prototype.closeOK = function (materialName, batchName, house) {
         dialog.close(this, materialName, batchName, house, 2);
     }
     var vmObj;
     vmBatch_Edit.show = function (id) {
         vmObj = new vmBatch_Edit();
         vmObj.materialModelsArray = getNewsModules();
         vmObj.greenHouseModelsArray = getGreenHouseModules();
         vmObj.init(id);

         return dialog.show(vmObj);
     };
     return vmBatch_Edit;

 });
