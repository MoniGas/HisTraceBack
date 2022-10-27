define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'jquery.querystring', 'plugins/dialog', './material_add', "./operationtype_add", 'jquery.uploadify'],
 function (router, ko, $, bdp, kv, utils, loginInfo, qs, dialog, material_add, operationtype_add, uploadify) {
     var parentCode;
     var moduleInfo = {
         moduleID: '14001',
         parentModuleID: '10000'
     }
     var getUrl = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Public/GetAppUrl",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.fileUrl;
             }
         });
         return data;
     }
     var vmguide = function (id) {
         var self = this;
         self.url = '/ShowImage/ShowImgNoUrl?ewm=' + getUrl();
         self.id = id;
         self.addDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd')).extend({
             maxLength: { params: 10, message: "最大长度为10个字符" },
             required: { params: true, message: "请选择作业时间!" }
         });
         self.type = ko.observable();
         self.content = ko.observable('').extend({
             maxLength: { params: 200, message: "作业内容最大长度为200个字符" },
             required: { params: true, message: "请填写作业内容!" }
         });
         self.typeid = ko.observable();
         self.operationArry = ko.observableArray();
         self.files = ko.observableArray([]);
         self.videos = ko.observableArray([]);
         self.loadingImage = {
             fileUrl: '../../images/load.gif', //ko.observable(result[1])
             fileUrls: '../../images/load.gif'
         };
         self.loadingVideoImage = {
             videoUrl: '../../images/load.gif', //ko.observable(result[1])
             videoUrls: '../../images/load.gif'
         };

         self.type.subscribe(function () {
             var defaultItem = { OperationTypeName: '暂无相应生产环节', Batch_ZuoYeType_ID: '-1' };
             if (!self.type()) {
                 self.operationArry(defaultItem);
                 return;
             }
             var selectedCode = self.type();
             self.operationArry(getOperationModules(selectedCode));
         });
         self.init = function () {
             $('#date1').datepicker({
                 autoclose: true,
                 todayHighlight: true,
                 language: 'cn'
             });
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
                     vm.vmguide.files.push(vm.vmguide.loadingImage);
                     //preview(file);
                 },
                 //返回一个错误，选择文件的时候触发
                 'onSelectError': function (file, errorCode, errorMsg) {
                     switch (errorCode) {
                         case -100:
                             alert("上传的文件数量已经超出系统限制的" + vm.vmguide.files().length + "个文件！");
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
                     //alert(data);
                     //                                     setTimeout(function () {
                     vm.vmguide.files.splice($.inArray(vm.vmguide.loadingImage, vm.vmguide.files), 1)
                     var dataObj = JSON.parse(data);
                     if (dataObj.code == 0) {
                         var single = {
                             fileUrl: dataObj.Msg, //ko.observable(result[1])
                             fileUrls: dataObj.sMsg
                         }
                         vm.vmguide.files.push(single);
                     }
                     //                                      }, 300);
                 }
             });
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
                     vm.vmguide.videos.push(vm.vmguide.loadingVideoImage);
                     //preview(file);
                 },
                 //返回一个错误，选择文件的时候触发
                 'onSelectError': function (file, errorCode, errorMsg) {
                     switch (errorCode) {
                         case -100:
                             alert("上传的文件数量已经超出系统限制的" + vm.vmguide.videos().length + "个文件！");
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
                     //alert(data);
                     setTimeout(function () {
                         vm.vmguide.videos.splice($.inArray(vm.vmguide.loadingVideoImage, vm.vmguide.videos), 1);
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 videoUrl: dataObj.Msg,
                                 videoUrls: dataObj.sMsg
                             }
                             vm.vmguide.videos.push(single);

                         }
                     }, 300);
                 }
             });
         }

         self.selZuoYe = ko.observable(false);
         self.selAddDate = ko.observable(true);
         self.selContent = ko.observable(false);
         self.Register = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.typeid()) {
                 var sendData = {
                     batchid: self.id,
                     type: self.type(),
                     batch_ZuoYeType_ID: self.typeid(),
                     addDate: self.addDate(),
                     content: self.content(),
                     batchextid: 0,
                     files: JSON.stringify(self.files()),
                     video: JSON.stringify(self.videos())
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch_ZuoYe/Add",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (jsonResult.code != 0) {
                             router.navigate('#guide3?id=' + self.id + '&pc=' + parentCode);
                         }
                         else {
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                     },
                     error: function (e) {
                         alert(e);
                     }
                 });
             }
             else {
                 if (!self.typeid()) {
                     self.selZuoYe(true);
                 }
                 else {
                     self.selZuoYe(false);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     /************************************************************************/
     // 获取生产环节模块
     var getOperationModules = function (selectedCode) {
         var sendData = {
             selecttype: selectedCode
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Batch_ZuoYe/OpTypeList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             },
             error: function (e) {

             }
         });
         return data;
     }
     /************************************************************************/
     //添加生产环节
     var addOperationType = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         operationtype_add.show().then(function (id) {
             vm.vmguide.operationArry(getOperationModules(vm.vmguide.type()));
             vm.vmguide.typeid(id);
         });
     }
     var next = function (data, event) {
         router.navigate('#guide3?id=' + self.id + '&pc=' + parentCode);
     };
     var back = function (data, event) {
         //         router.navigate('#guide1?pc='+parentCode);
         router.navigateBack();
     };
     var delImage = function (data, event) {
         var index = vm.vmguide.files.indexOf(data);
         vm.vmguide.files.splice(index, 1);
     }
     var delVideo = function (data, event) {
         var index = vm.vmguide.videos.indexOf(data);
         vm.vmguide.videos.splice(index, 1);
     }
     var vm = {
         binding: function () {
             //初初化导航状态
             parentCode = qs.querystring("pc");
             if (parentCode == undefined || parentCode == '') {
                 parentCode = moduleInfo.parentModuleID;
             }
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             id = qs.querystring("id");
             vm.vmguide = new vmguide(id);
             vm.vmguide.init();
         },
         vmguide: null,
         next: next,
         addOperationType: addOperationType,
         back: back,
         delImage: delImage,
         delVideo: delVideo
     }
     return vm;
 });