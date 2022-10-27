define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, loginInfo) {
     var vmObj;

     var vmSetQcReportEdit = function () {
         var self = this;
         self.userName = ko.observable('').extend({
             maxLength: { params: 50, message: "用户名最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入用户名！"
             }
         });
         self.pwd = ko.observable('').extend({
             maxLength: { params: 25, message: "密码最大长度为25个字符" },
             required: {
                 params: true,
                 message: "请输入密码！"
             }
         });
         //         self.seluserName = ko.observable(false);
         //         self.selpwd = ko.observable(false);
         self.files = ko.observableArray();
         self.cancel = function () {
             self.close(this);
         }
         self.loadingImage = {
             fileUrl: '../../images/load.gif',
             fileUrls: '../../images/load.gif'
         };
     }
     vmSetQcReportEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
         try {
             $('#excel_upload').uploadify('destroy');
         } catch (Error) {
         }
         $("#excel_upload").uploadify({
             'debug': false, //开启调试
             'auto': true, //是否自动上传
             'buttonText': '',
             'buttonImage': '',
             'buttonClass': '',
             'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
             'queueID': 'uploadfileQueue', //文件选择后的容器ID
             'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
             'width': '79',
             'height': '79',
             'multi': false,
             'queueSizeLimit': 1,
             'uploadLimit': 0,
             'fileTypeDesc': '支持的格式：',
             'fileTypeExts': '*.xls;*.xlsx',
             'fileSizeLimit': '50MB',
             'removeTimeout': 0,
             'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             'onSelect': function (file) {
                 vmObj.files.push(vmObj.loadingImage);
                 //preview(file);
             },
             //返回一个错误，选择文件的时候触发
             'onSelectError': function (file, errorCode, errorMsg) {
                 switch (errorCode) {
                     case -100:
                         alert("上传的文件数量已经超出系统限制的" + $('#image_upload1').uploadify('settings', 'queueSizeLimit') + "个文件！");
                         break;
                     case -110:
                         alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload1').uploadify('settings', 'fileSizeLimit') + "大小！");
                         break;
                     case -120:
                         alert("文件 [" + file.name + "] 大小异常！");
                         break;
                     case -130:
                         alert("文件 [" + file.name + "] 类型不正确！");
                         break;
                 }
             },
             'onUploadError': function (file, errorCode, errorMsg, errorString) {

             },
             //检测FLASH失败调用
             'onFallback': function () {
                 alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
             },
             //上传到服务器，服务器返回相应信息到data里
             'onUploadSuccess': function (file, data, response) {
                 //                 vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
                 var errors = ko.validation.group(this);
                 if (errors().length <= 0 && vmObj.userName() != null && vmObj.pwd() != null && vmObj.userName() != "" && vmObj.pwd() != "") {

                     var dataObj = JSON.parse(data.replace(/\\/g, "/"));
                     if (dataObj.code == 0) {
                         //                     var single = {
                         //                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         //                         fileUrls: dataObj.sMsg
                         //                     }
                         var single = {
                             fileUrl: dataObj.Msg,
                             fileUrls: dataObj.sMsg
                         }
                         var sendData = {
                             userName: vmObj.userName(),
                             pwd: vmObj.pwd(),
                             excelurl: single.fileUrl,
                             excelpath: single.fileUrls
                         }
                         $.ajax({
                             type: "POST",
                             url: "/Admin_Material/AddExcelRQT",
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
                                         vmObj.cancel();
                                     }
                                     else {
                                         vmObj.files([]);
                                     }
                                 }
                                 }]);
                             }
                         })
                     }
                 }
                 else {
                     if (vmObj.userName() == "") {
                         alert("请输入用户名！");
                         vmObj.files([]);
                     }
                     else if (vmObj.pwd() == "") {
                         alert("请输入密码！");
                         vmObj.files([]);
                     }
                     //                     errors.showAllMessages();
                 }
             }
         });
     }
     vmSetQcReportEdit.prototype.delImage = function (data, event) {
         var index = vmObj.files.indexOf(data);
         vmObj.files.splice(index, 1);
         if (vmObj.files().length == 0) {
             //             vmObj.codeimg(true);
         }
     }
     vmSetQcReportEdit.prototype.close = function () {
         dialog.close(this);
     }
     vmSetQcReportEdit.show = function () {
         vmObj = new vmSetQcReportEdit();
         return dialog.show(vmObj);
     };
     return vmSetQcReportEdit;

 });