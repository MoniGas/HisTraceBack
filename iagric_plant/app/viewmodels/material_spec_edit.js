define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, loginInfo) {
     var array = new Array();
     var vmObj;
     var vmMaterialSpecEdit = function (id) {
         var self = this;
         self.MaterialSpecification = ko.observable().extend({
             minLength: { params: 2, message: "规格最小长度为2个字符" },
             maxLength: { params: 50, message: "规格最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入商品规格！"
             }
         });
         self.ExpressPrice = ko.observable('').extend({
             min: {
                 params: 0,
                 message: "输入价格必须大于0！"
             },
             pattern: {
                 params: /^\d+[\.]?\d{0,2}$/g,
                 message: "必须是数字，并且最多两位小数！"
             }
         });
         self.Price = ko.observable('').extend({
             min: {
                 params: 0.001,
                 message: "输入价格必须大于0！"
             },
             max: {
                 params: 1000000,
                 message: "输入价格必须小于100万！"
             },
             required: {
                 params: true,
                 message: "请输入价格！"
             },
             pattern: {
                 params: /^\d+[\.]?\d{0,2}$/g,
                 message: "必须是数字，并且最多两位小数！"
             }
         });
         self.id = id;
         self.selTitle = ko.observable(false);
         self.selprice = ko.observable(false);
         self.selExpressPrice = ko.observable(false);
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

         self.materialProperty = ko.observable('');
         self.materialPropertys = [];
         self.selectedProperty = ko.observable('0');

         self.init = function () {
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Material_Spec/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     //alert(jsonResult.code);
                     //var obj = JSON.parse(jsonResult);
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
                     self.MaterialSpecification(jsonResult.ObjModel.MaterialSpecification);
                     self.Price(jsonResult.ObjModel.Price);
                     self.selectedModel(jsonResult.ObjModel.Material_ID);
                     self.ExpressPrice(jsonResult.ObjModel.ExpressPrice);
                 }
             });

             sendData = {
                 MaterialSpecId: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Material_Spec/GetMaterialProperty",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     array = new Array();
                     if (jsonResult.length == 1) {
                         self.selectedProperty(jsonResult[0].Material_Property_ID);
                         //alert(self.selectedProperty());
                         if (jsonResult[0].Condition != null) {
                             try {
                                 array = jsonResult[0].Condition.split(',');
                             } catch (Error) {
                                 array = new Array();
                             }
                         }
                     }
                 },
                 error: function (Error) {
                     alert(Error);
                 }
             });
         }
         self.cancel = function () {
             self.close(this);
         }
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.selectedModel()) {
                 var material = 0;
                 if (self.selectedModel()) {
                     material = self.selectedModel();
                 }
                 var Condition = "";
                 $("input[name='ChkCondition']:checkbox:checked").each(function () {
                     Condition += $(this).val() + ',';
                 });
                 var sendData = {
                     id: self.id,
                     materialID: material,
                     materialSpecification: self.MaterialSpecification(),
                     price: self.Price(),
                     Propertys: self.selectedProperty(),
                     Condition: Condition,
                     ExpressPrice: self.ExpressPrice()
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Material_Spec/Edit",
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
                                 self.closeOK(materialName, sendData.materialSpecification, sendData.price);
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
                 self.errors.showAllMessages();
             }
         }
         var flag = false; ;
         self.isConditionChecked = function (ConditionId) {
             if (flag) { return; }
             if ($.inArray(ConditionId.toString(), array) == -1) {
                 return false;
             } else {
                 return true;
             }
         }
         self.isConditionDisabled = function () {
             if (self.selectedProperty() == undefined) {
                 $("#ChkCondition").attr("disabled", "disabled");
                 $("#ChkCondition").removeAttr("checked");
             }
             else {
                 $("#ChkCondition").removeAttr("disabled");
             }
         }
         self.selectedProperty.subscribe(function () {
             if (self.selectedProperty() == undefined) {
                 flag = true;
                 $("#ChkCondition").attr("disabled", "disabled");
                 $("#ChkCondition").removeAttr("checked");
             }
             else {
                 flag = false;
                 $("#ChkCondition").removeAttr("disabled");
             }
         });
     }
     //获取产品模块
     var getNewsModules = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch/MaterialList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
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
     vmMaterialSpecEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     var SearchProperty = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Material_Spec/GetMaterialPropertyList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             },
             error: function (Error) {
                 alert(Error);
             }
         });
         return data;
     }
     vmMaterialSpecEdit.prototype.close = function () {
         //alert(this.province().code);
         dialog.close(this, "", "", "", 1);
     }
     vmMaterialSpecEdit.prototype.closeOK = function (materialName, materialSpecification, price) {
         dialog.close(this, materialName, materialSpecification, price, 2);
     }
     vmMaterialSpecEdit.show = function (id) {

         vmObj = new vmMaterialSpecEdit(id);
         vmObj.materialPropertys = SearchProperty();
         vmObj.init();
         vmObj.materialModelsArray = getNewsModules();
         return dialog.show(vmObj);
     };
     return vmMaterialSpecEdit;

 });