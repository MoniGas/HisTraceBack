define(['plugins/router', 'plugins/dialog', 'knockout','jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'kindeditor.zh-CN'],
 function (router, dialog, ko,$, bdp, kv, utils, loginInfo, kcn) {

     var editorCompany;
     var vmDept = function () {
         var self = this;

         self.name = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入部门名称!"
             }
         });
         self.selTitle = ko.observable(false);
         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     name: self.name(),
                     brief: editorCompany.html()
                 }

                 $.ajax({
                     type: "POST",
                     url: "/Admin_ShowDept/Add",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             if (jsonResult.code == 1) {
                                 self.close();
                             }
                         }
                         }]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
             }
         };
     }
     vmDept.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

         editorCompany = KindEditor.create("#txtInfos", {
             cssPath: '/lib/kindeditor/plugins/code/prettify.css',
             uploadJson: '/lib/kindeditor/upload_json.ashx',
             fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
             allowFileManager: true,
             afterCreate: function () { },
             afterBlur: function () { this.sync(); }
         });
     }
     vmDept.prototype.close = function () {
         dialog.close(this);
     }
     vmDept.show = function () {
         var vmObj = new vmDept();

         return dialog.show(vmObj);
     };
     return vmDept;
 });