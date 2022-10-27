define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', 'jquery-ui'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, jqueryui) {
     //自定义绑定-复选框级联选择
     ko.bindingHandlers.checkBoxCascade = {
         init: function (element, valueAccessor, allBindingsAccessor) {
             $(element).on('click', "input[name='CbxFile']:checkbox", function (e) {
                 if (this.checked == false) {
                     $("#FilePassword").css({ "display": "none" });
                 } else {
                     $("#FilePassword").css({ "display": "" });
                 }
             });
         },
         update: function (element, valueAccessor, allBindingsAccessor) {

         }
     }

     var ViewModel = function (RequestCodeId, Type) {
         var self = this;
         self.FilePassword = ko.observable();
         self.VisibleType = ko.observable(true);
         self.CbxEwm = ko.observable(false);
         self.CbxFile = ko.observable(false);
         self.CbxImage = ko.observable(false);


         if (Type == 1) { // 有下载操作
             self.VisibleType(true);
         } else if (Type == 2) { // 没有下载操作
             self.VisibleType(false);
         }

         self.DownloadFile = function (downLoadURL) {
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var downLoadURL = ko.utils.unwrapObservable(downLoadURL);
             $.fileDownload('/Admin_Request/DownloadFile?RequestCodeId =' + RequestCodeId + '&downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });

             var sendData = {
                 RequestCodeId: RequestCodeId
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/UpdateDownLoadNum",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                         return;
                     };
                 }
             })
         }

         self.Package = function () {
             var sendData = {
                 RequestCodeId: RequestCodeId,
                 CbxEwm: self.CbxEwm(),
                 CbxFile: self.CbxFile(),
                 FilePassword: self.FilePassword(),
                 CbxImage: self.CbxImage()
             }
             var value;
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/Packaging",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     }
                     else {
                         if (jsonResult.code == 1) {
                             self.DownloadFile(jsonResult.Msg);
                         } else {
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                             }
                             }]);
                         }
                     }
                 }
             })
         }

         self.init = function () {
             var sendData = {
                 RequestCodeId: RequestCodeId
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/GetRequestCodelModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     }
                     else {
                         if (jsonResult.code == 1) {
                             self.CbxEwm(jsonResult.ObjModel.EwmEncryption);
                             self.CbxFile(jsonResult.ObjModel.FileEncryption);
                             if (jsonResult.ObjModel.FileEncryption == true) {
                                 self.FilePassword(jsonResult.ObjModel.FilePassword);
                             }
                             self.CbxImage(jsonResult.ObjModel.ImageCounterfeit);
                         }
                     }
                 }
             })
         }

         self.Save = function () {
             var sendData = {
                 RequestCodeId: RequestCodeId,
                 CbxEwm: self.CbxEwm(),
                 CbxFile: self.CbxFile(),
                 FilePassword: self.FilePassword(),
                 CbxImage: self.CbxImage()
             }
             var value;
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/Packaging",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     }
                     else {
                         dialog.showMessage('设置成功！', '系统提示', [{ title: '确定', callback: function () {
                             if (jsonResult.code == 1) {
                                 self.close();
                             }
                         }
                         }]);
                     }
                 }
             })
         }
     }

     ViewModel.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

     }

     ViewModel.prototype.close = function () {
         dialog.close(this);
     }

     ViewModel.show = function (RequestCodeId, Type) {
         var vmObj = new ViewModel(RequestCodeId, Type);
         vmObj.init();
         return dialog.show(vmObj);
     }
     return ViewModel;
 });