define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'jquery.querystring', 'logininfo', './material_add', 'knockout.mapping'],
 function (router, ko, $, bdp, kv, utils, qs, loginInfo, material_add, km) {
     var parentCode;
     var moduleInfo = {
         moduleID: '14002',
         parentModuleID: '10000'
     }
     var vmguide = function (id) {
         var self = this;
         self.id = id;
         self.material = ko.observable();
         //         self.count = ko.observable('');
         self.count = ko.observable().extend({
             number: {
                 params: true,
                 message: "申请数量为整数！"
             },
             min: {
                 params: 1,
                 message: "申请数量最少为1！"
             },
             max: {
                 params: 100000,
                 message: "申请数量最多为100000！"
             },
             required: {
                 params: true,
                 message: "请填写申请数量！"
             }
         });
         self.materialArry = [];
         self.selMaterial = ko.observable(false);
         self.selCount = ko.observable(false);
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.material()) {
                 var sendData = {
                     Type: 2,
                     Specifications: 0,
                     Material_ID: self.material(),
                     RequestCount: self.count()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Request/Generate",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (jsonResult.code != 0) {
                             router.navigate('#guide4?id=' + self.id + '&pc=' + parentCode);
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
                 self.errors.showAllMessages();
             }
         }
     }
     /************************************************************************/
     //获取产品模块
     var getMaterialModules = function () {
         var sendData = {
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
             data = jsonResult.ObjList;
         }
     });
     return data;
 }
 /************************************************************************/
 var next = function (data, event) {
     router.navigate('#guide4?id=' + self.id + '&pc=' + parentCode);
 };
 var back = function (data, event) {
     //     router.navigate('#guide2?id=' + self.id+'&pc='+parentCode);
     router.navigateBack();
 };
 var vm = {
     binding: function () {
         parentCode = qs.querystring("pc");
         if (parentCode == undefined || parentCode == '') {
             parentCode = moduleInfo.parentModuleID;
         }
         //初初化导航状态
         //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
         id = qs.querystring("id");
         vm.vmguide = new vmguide(id);
         vm.vmguide.materialArry = getMaterialModules();

         //限制文本框只能输入正整数
         //         $("#txtApplyCount").keyup(function () {
         //             //如果输入非数字，则替换为''，如果输入数字，则在每4位之后添加一个空格分隔
         //             this.value = this.value.replace(/[^\d]/g, '');
         //         })
     },
     vmguide: null,
     next: next,
     back: back
 }
 return vm;
});