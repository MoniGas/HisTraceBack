define(['plugins/router', 'plugins/dialog', 'knockout','jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jquery.querystring', 'jqPaginator', 'jquery-ui'],
 function (router, dialog, ko,$, bdp, uploadify, kv, utils, loginInfo, km, qs, jq, jqueryui) {

     var editorCompany;
     var vmDept = function (id) {
         var self = this;

         self.name = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入部门名称!"
             }
         });
         self.selTitle = ko.observable(false);
         self.infos = ko.observable('');
         self.init = function () {
             var sendData = {
                 id: id
             }

             $.ajax({
                 type: "POST",
                 url: "/Admin_ShowDept/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (obj) {
                     if (obj.code == 0) {
                         dialog.showMessage(obj.Msg, '系统提示', [{ title: '确定', callback: function () {
                             self.close();
                         }
                         }]);

                         return;
                     }

                     self.name(obj.ObjModel.DeptName);
                     self.infos(obj.ObjModel.Infos);
                 }
             });
         }

         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     id: id,
                     name: self.name(),
                     brief: editorCompany.html()
                 }

                 $.ajax({
                     type: "POST",
                     url: "/Admin_ShowDept/Edit",
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
                                 self.closeOK(sendData.name);
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
         editorCompany.html(vmObj.infos());
     }
     vmDept.prototype.close = function () {
         dialog.close(this, "", 1);
     }
     vmDept.prototype.closeOK = function (name) {
         dialog.close(this, name, 2);
     }
     var vmObj;
     vmDept.show = function (id) {
         vmObj = new vmDept(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmDept;
 });