define(['plugins/router', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'knockout', 'jquery-ui', 'bootstrap-datepicker', 'jquery.uploadify'],
 function (router, $, kv, utils, loginInfo, dialog, ko, jqueryui, bdp, uploadify) {
     var vmObj;
     var vmZuoYe_Add = function (id, beid) {
         var self = this;
         self.Content = ko.observable('').extend({
             minLength: { params: 2, message: "作业内容最小长度为2个字符" },
             maxLength: { params: 500, message: "作业内容最大长度为500个字符" },
             required: {
                 params: true,
                 message: "请输入作业内容！"
             }
         });
         self.id = id;
         self.beid = beid;
         self.AddDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
         self.zuoyeDate = ko.observable(false);
         self.selTitle = ko.observable(false);
         self.type = ko.observable(0);
         self.opTypeModelsArray = ko.observableArray();
         self.selectedModel = ko.observable();
         self.seloptypemodel = ko.observable(false);
         self.selectedModel.subscribe(function () {
             if (self.selectedModel()) {
                 self.seloptypemodel(false);
             }
             else {
                 self.seloptypemodel(true);
             }
         });
         self.type.subscribe(function () {
             var defaultItem = { OperationTypeName: '暂无相应生产环节', Batch_ZuoYeType_ID: '-1' };
             if (!self.type()) {
                 self.opTypeModelsArray(defaultItem);
                 return;
             }
             var selectedCode = self.type();
             var sendData = {
                 selecttype: selectedCode
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch_ZuoYe/OpTypeList",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 data: JSON.stringify(sendData),
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                         return;
                     };
                     self.opTypeModelsArray(jsonResult.ObjList);
                 }
             });
         });
         self.files = ko.observableArray([]);
         self.video = ko.observableArray([]);
         self.calcel = function () {
             self.close(this);
         }
         self.loadingImage = {
             fileUrl: '../../images/load.gif', //ko.observable(result[1])
             fileUrls: '../../images/load.gif'
         };
         self.loadingvideo = {
             videoUrl: '../../images/load.gif', //ko.observable(result[1])
             videoUrls: '../../images/load.gif'
         };
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.selectedModel() && (self.AddDate() != null && self.AddDate() != "" && self.AddDate() != "undefined")) {
                 var batch_ZuoYeType = 0;
                 if (self.selectedModel()) {
                     batch_ZuoYeType = self.selectedModel();
                 }
                 var sendData = {
                     batchid: self.id,
                     batchextid: self.beid,
                     addDate: self.AddDate(),
                     type: self.type(),
                     content: self.Content(),
                     batch_ZuoYeType_ID: batch_ZuoYeType,
                     files: JSON.stringify(self.files()),
                     video: JSON.stringify(self.video())
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch_ZuoYe/Add",
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
                                 //获取生产环节名称
                                 var gongxuName = $("#selGongXuName").find("option:selected").text();
                                 //alert(gongxuName);
                                 self.closeOK(gongxuName);
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 if (self.AddDate() == null || self.AddDate() == "" || self.AddDate() == "undefined") {
                     self.zuoyeDate(true);
                 }
                 else {
                     self.zuoyeDate(false);
                 }
                 if (!self.selectedModel()) {
                     self.seloptypemodel(true);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     vmZuoYe_Add.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

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
             'uploadLimit': 0,
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
                     vmObj.files.push(single);
                 }

             }
         });
         /*************************************/
         $("#video_upload").uploadify({
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
             'fileTypeExts': '*.avi;*.mp4',
             'fileSizeLimit': '200MB',
             'removeTimeout': 0,
             'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             'onSelect': function (file) {
                 vmObj.video.push(vmObj.loadingvideo);
                 //preview(file);
             },
             //返回一个错误，选择文件的时候触发
             'onSelectError': function (file, errorCode, errorMsg) {
                 switch (errorCode) {
                     case -100:
                         alert("上传的文件数量已经超出系统限制的" + vmObj.video().length + "个文件！");
                         break;
                     case -110:
                         alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#video_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                 vmObj.video.splice($.inArray(vmObj.loadingvideo, vmObj.video), 1)
                 var dataObj = JSON.parse(data);
                 if (dataObj.code == 0) {
                     //alert(dataObj.sMsg)
                     var single = {
                         videoUrl: dataObj.Msg,
                         videoUrls: dataObj.sMsg
                     }
                     setTimeout(function () {
                         vmObj.video.push(single);
                     }, 100);
                 }

             }
         });
         /*************************************/
         $('#AddDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
     }
     vmZuoYe_Add.prototype.delImage = function (data, event) {
         var index = vmObj.files.indexOf(data);
         vmObj.files.splice(index, 1);
         //var currentLimitCount = $('#image_upload').uploadify('settings', 'uploadLimit');
         //$('#image_upload').uploadify('settings', 'uploadLimit', currentLimitCount + 1);
     }
     vmZuoYe_Add.prototype.delVideo = function (data, event) {
         var index = vmObj.video.indexOf(data);
         vmObj.video.splice(index, 1);
     }
     vmZuoYe_Add.prototype.close = function () {
         $('#image_upload').uploadify('destroy');
         dialog.close(this);
     }
     vmZuoYe_Add.prototype.closeOK = function (selectedModel) {
         dialog.close(this, selectedModel);
     }
     vmZuoYe_Add.show = function (id, beid) {
         vmObj = new vmZuoYe_Add(id, beid);
         return dialog.show(vmObj);
     };
     return vmZuoYe_Add;

 });