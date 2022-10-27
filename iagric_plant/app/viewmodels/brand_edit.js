define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, loginInfo) {
     var vmObj;
     var vmBrandEdit = function (id) {
         var self = this;
         self.BrandName = ko.observable().extend({
             minLength: { params: 2, message: "名称最小长度为2个字符" },
             maxLength: { params: 100, message: "名称最大长度为100个字符" },
             required: {
                 params: true,
                 message: "请输入品牌名称！"
             }
         });
         self.id = id;
         self.selTitle = ko.observable(false);
         self.Descriptions = ko.observable();
         self.files = ko.observableArray();
         self.logo = ko.observable(false);
         self.init = function () {
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Brand/GetModel",
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
                     self.BrandName(jsonResult.ObjModel.BrandName);
                     self.Descriptions(jsonResult.ObjModel.Descriptions);
                     if (jsonResult.ObjModel.Logo != null && jsonResult.ObjModel.Logo != '') {
                         var single = {
                             fileUrl: jsonResult.ObjModel.Logo,
                             fileUrls: jsonResult.ObjModel.Logo
                         }
                         self.files.push(single);
                     }
                 }
             })
         }
         self.cancel = function () {
             self.close(this);
         }
         self.loadingImage = {
             fileUrl: '../../images/load.gif', //ko.observable(result[1])
             fileUrls: '../../images/load.gif'
         };
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && !(self.files() == null || self.files() == "" || self.files() == "undefined")) {
                 var sendData = {
                     id: self.id,
                     BrandName: self.BrandName(),
                     Descriptions: self.Descriptions(),
                     logo: self.files()[0].fileUrl
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Brand/Edit",
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
                                 self.closeOK(sendData.BrandName, sendData.Descriptions, sendData.logo);
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 self.errors.showAllMessages();
                 if (self.files() == null || self.files() == "" || self.files() == "undefined") {
                     self.logo(true);
                 }
                 else {
                     self.logo(false);
                 }
             }
         }
     }
     vmBrandEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
         try {
             $('#image_upload').uploadify('destroy');
         } catch (Error) {
         }
         $("#image_upload").uploadify({
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
             //             'uploadLimit': 1,
             'fileTypeDesc': '支持的格式：',
             'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             'fileSizeLimit': '5MB',
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
                         alert("上传的文件数量已经超出系统限制的" + vmObj.files().length + "个文件！");
                         break;
                     case -110:
                         alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                 vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
                 var dataObj = JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     //                     vmObj.files.push(single);
                     vmObj.files(new Array());
                     vmObj.files.push(single);
                     vmObj.logo(false);
                 }

             }
         });
     }
     vmBrandEdit.prototype.delImage = function (data, event) {
         vmObj.files(new Array());
         vmObj.logo(true);
         //         var index = vmObj.files.indexOf(data);
         //         vmObj.files.splice(index, 1);
     }
     vmBrandEdit.prototype.close = function () {
         //alert(this.province().code);
         dialog.close(this, "", "", "", 1);
     }
     vmBrandEdit.prototype.closeOK = function (BrandName, Descriptions, logo) {
         dialog.close(this, BrandName, Descriptions, logo, 2);
     }
     vmBrandEdit.show = function (id) {
         vmObj = new vmBrandEdit(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmBrandEdit;

 });