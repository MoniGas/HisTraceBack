define(['plugins/router', 'plugins/dialog', 'knockout','jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jquery.querystring', 'jqPaginator'],
 function (router, dialog, ko,$, bdp, uploadify, kv, utils, loginInfo, km, qs, jq) {
     var vmUser = function (id) {
         var self = this;

         //初始化活动动态模块数据
         self.roleName = ko.observable('');
         self.userName = ko.observable('');
         self.userCode = ko.observable('');
         self.loginName = ko.observable('');
//         self.loginPass = ko.observable('');
         self.telephone = ko.observable('');
         self.address = ko.observable('');

         self.init = function () {
             var sendData = {
                 id: id
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Enterprise_User/GetLevelModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     };
                     if (jsonResult.code == 0) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         return;
                     }
                     self.roleName(jsonResult.ObjModel.LevelName);
                     self.userName(jsonResult.ObjModel.UserName);
                     self.userCode(jsonResult.ObjModel.UserCode);
                     self.loginName(jsonResult.ObjModel.LoginName);
//                     self.loginPass(jsonResult.ObjModel.LoginPassWord);
                     self.telephone(jsonResult.ObjModel.LinkPhone)
                     self.address(jsonResult.ObjModel.Address);
                 }
             });
         }
     }

     vmUser.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     vmUser.prototype.close = function () {
         dialog.close(this);
     }
     vmUser.show = function (id) {
         var vmObj = new vmUser(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmUser;
 });