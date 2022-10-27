define(['plugins/router', 'knockout','jquery', 'bootstrap-datepicker', 'knockout.validation', 'jquery.querystring', 'utils','logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify'],
 function (router, ko,$, bdp, kv, qs, utils,loginInfo, dialog, jqueryui, uploadify) {
     var vmObj
     var vmXunJianEdit = function () {
         var self = this;
         self.id;
         self.AddDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
         self.xunjianDate = ko.observable(false);
         self.Content = ko.observable().extend({
             maxLength: { params: 500, message: "巡检内容最大长度为500个字符" },
             required: {
                 params: true,
                 message: "请输入巡检内容!"
             }
         });
         self.selTitle = ko.observable(false);
         self.files = ko.observableArray();
         self.video = ko.observableArray();
         self.init = function (id) {
             self.id = id;
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch_XunJian/GetModelXJ",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     self.AddDate(jsonResult.ObjModel.StrAddDate.substring(0, 10));
                     self.Content(jsonResult.ObjModel.Content);
                     self.files(jsonResult.ObjModel.imgs);
                     self.video(jsonResult.ObjModel.videos);
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
         self.loadingvideo = {
             videoUrl: '../../images/load.gif', //ko.observable(result[1])
             videoUrls: '../../images/load.gif'
         };
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && (self.AddDate() != null && self.AddDate() != "" && self.AddDate() != "undefined")) {
                 var sendData = {
                     id: self.id,
                     addDate: self.AddDate(),
                     content: self.Content(),
                     files: JSON.stringify(self.files()),
                     video: JSON.stringify(self.video())
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch_XunJian/Edit",
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
                                 self.close();
                             }
                         }
                         }]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
                 if (self.AddDate() == null || self.AddDate() == "" || self.AddDate() == "undefined") {
                     self.xunjianDate(true);
                 }
                 else {
                     self.xunjianDate(false);
                 }
             }
         }
     }
     vmXunJianEdit.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
         $('#AddDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
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
                     vmObj.video.push(single);
                 }

             }
         });
         /*************************************/
     }
     vmXunJianEdit.prototype.delImage = function (data, event) {
         var index = vmObj.files.indexOf(data);
         vmObj.files.splice(index, 1);
         //var currentLimitCount = $('#image_upload').uploadify('settings', 'uploadLimit');
         //$('#image_upload').uploadify('settings', 'uploadLimit', currentLimitCount + 1);
     }
     vmXunJianEdit.prototype.delVideo = function (data, event) {
         var index = vmObj.video.indexOf(data);
         vmObj.video.splice(index, 1);
     }
     vmXunJianEdit.prototype.close = function () {
         //alert(this.province().code);
         dialog.close(this);
     }
     vmXunJianEdit.show = function (id) {
         vmObj = new vmXunJianEdit();
         vmObj.init(id);
         return dialog.show(vmObj);
     };
     return vmXunJianEdit;

 });