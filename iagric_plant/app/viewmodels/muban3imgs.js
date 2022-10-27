define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'jquery.querystring', 'kindeditor.zh-CN', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'webuploader'],
 function (router, ko, $, kv, utils, loginInfo, qs, kcn, dialog, jqueryui, uploadify, webuploader) {
     var moduleInfo = {
         moduleID: '24200',
         parentModuleID: '20000'
     }
     var vmShowMuBan3Img = function () {
         var self = this;
         //首次进入的图片
         self.firstImg = ko.observableArray([]);
         self.selfirstImg = ko.observable(false);
         self.loadingImage = {
             fileUrl: '../../images/load.gif',
             fileUrls: '../../images/load.gif'
         };
         self.delImage = function (data, event) {
             var index = self.firstImg.indexOf(data);
             self.firstImg.splice(index, 1);
         }
         self.centerImg = ko.observableArray([]);
         self.delImageCenter = function (data, event) {
             var index = self.centerImg.indexOf(data);
             self.centerImg.splice(index, 1);
         }
         self.fiveImg = ko.observableArray([]);
         self.delImageFive = function (data, event) {
             var index = self.fiveImg.indexOf(data);
             self.fiveImg.splice(index, 1);
         }
         self.init = function () {
             $.ajax({
                 type: "POST",
                 url: "/EnterpriseMuBanThreeImg/Index",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     self.firstImg(jsonResult.ObjModel.FirstImgs);
                     self.centerImg(jsonResult.ObjModel.CenterImgs);
                     self.fiveImg(jsonResult.ObjModel.FiveImgs);
                 }
             });
         }

         self.SaveInfo = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && !(self.firstImg() == null || self.firstImg() == "" || self.firstImg() == "undefined")) {
                 //             if (self.errors().length <= 0) {
                 var sendData = {
                     files: JSON.stringify(self.firstImg()),
                     centerImg: JSON.stringify(self.centerImg()),
                     fiveImg: JSON.stringify(self.fiveImg())
                 }
                 $.ajax({
                     type: "POST",
                     url: "/EnterpriseMuBanThreeImg/Edit",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         if (jsonResult.code == 1) {
                             self.init();
                         }
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         //                         self.selfirstImg = ko.observable(false);
                     }
                 })
             } else {
                 if (self.firstImg() == null || self.firstImg() == "" || self.firstImg() == "undefined") {
                     self.selfirstImg(true);
                 }
                 else {
                     self.selfirstImg(false);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     var vm = {
         activate: function () {
         },
         binding: function () {
             //初始化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             vm.vmShowMuBan3Img = new vmShowMuBan3Img();
             vm.vmShowMuBan3Img.init();
             //             try {
             //                 $('#image_upload').uploadify('destroy');
             //             } catch (Error) {
             //             }
             //             $("#image_upload").uploadify({
             //                 'debug': false, //开启调试
             //                 'auto': true, //是否自动上传
             //                 'buttonText': '',
             //                 'buttonImage': '',
             //                 'buttonClass': '',
             //                 'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
             //                 'queueID': 'uploadfileQueue', //文件选择后的容器ID
             //                 'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
             //                 'width': '79',
             //                 'height': '79',
             //                 'multi': false,
             //                 'queueSizeLimit': 1,
             //                 'uploadLimit': 0,
             //                 'fileTypeDesc': '支持的格式：',
             //                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             //                 'fileSizeLimit': '5MB',
             //                 'removeTimeout': 0,
             //                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             //                 'onSelect': function (file) {
             //                     vm.vmShowMuBan3Img.firstImg.push(vm.vmShowMuBan3Img.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.firstImg().length + "个文件！");
             //                             break;
             //                         case -110:
             //                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
             //                             break;
             //                         case -120:
             //                             alert("文件 [" + file.name + "] 大小异常！");
             //                             break;
             //                         case -130:
             //                             alert("文件 [" + file.name + "] 类型不正确！");
             //                             break;
             //                     }
             //                 },
             //                 'onUploadError': function (file, errorCode, errorMsg, errorString) {

             //                 },
             //                 //检测FLASH失败调用
             //                 'onFallback': function () {
             //                     alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
             //                 },
             //                 //上传到服务器，服务器返回相应信息到data里
             //                 'onUploadSuccess': function (file, data, response) {
             //                     vm.vmShowMuBan3Img.firstImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.firstImg), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg, //ko.observable(result[1])
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowMuBan3Img.firstImg(new Array());
             //                         vm.vmShowMuBan3Img.firstImg.push(single);
             //                     }

             //                 }
             //             });
             var imguploader = webuploader.create({
                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',

                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",
                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#image_upload', multiple: false }, //pick: '#image_upload',
                 auto: true,
                 //            formData: { guid: guid },
                 // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                 resize: false,
                 //切片
                 chunked: true,
                 //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
                 chunkSize: 2 * 1024 * 1024,
                 fileSingleSizeLimit: 5 * 1024 * 1024, // 限制在5M
                 threads: 1,
                 accept: {
                     title: 'Images',
                     extensions: 'gif,jpg,jpeg,bmp,png',
                     mimeTypes: 'image/*'
                 }
             });
             imguploader.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowMuBan3Img.firstImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.firstImg), 1)
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmShowMuBan3Img.firstImg(new Array());
                     vm.vmShowMuBan3Img.firstImg.push(single);
                 }
             });

             //             //2/3/4图片
             //             try {
             //                 $('#uploadCenterImg').uploadify('destroy');
             //             } catch (Error) {
             //             }
             //             $("#uploadCenterImg").uploadify({
             //                 'debug': false, //开启调试
             //                 'auto': true, //是否自动上传
             //                 'buttonText': '',
             //                 'buttonImage': '',
             //                 'buttonClass': '',
             //                 'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
             //                 'queueID': 'uploadfileQueue', //文件选择后的容器ID
             //                 'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
             //                 'width': '79',
             //                 'height': '79',
             //                 'multi': false,
             //                 'queueSizeLimit': 1,
             //                 'uploadLimit': 0,
             //                 'fileTypeDesc': '支持的格式：',
             //                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             //                 'fileSizeLimit': '5MB',
             //                 'removeTimeout': 0,
             //                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             //                 'onSelect': function (file) {
             //                     vm.vmShowMuBan3Img.centerImg.push(vm.vmShowMuBan3Img.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.centerImg().length + "个文件！");
             //                             break;
             //                         case -110:
             //                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#uploadCenterImg').uploadify('settings', 'fileSizeLimit') + "大小！");
             //                             break;
             //                         case -120:
             //                             alert("文件 [" + file.name + "] 大小异常！");
             //                             break;
             //                         case -130:
             //                             alert("文件 [" + file.name + "] 类型不正确！");
             //                             break;
             //                     }
             //                 },
             //                 'onUploadError': function (file, errorCode, errorMsg, errorString) {

             //                 },
             //                 //检测FLASH失败调用
             //                 'onFallback': function () {
             //                     alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
             //                 },
             //                 //上传到服务器，服务器返回相应信息到data里
             //                 'onUploadSuccess': function (file, data, response) {
             //                     vm.vmShowMuBan3Img.centerImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.centerImg), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg, //ko.observable(result[1])
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowMuBan3Img.centerImg(new Array());
             //                         vm.vmShowMuBan3Img.centerImg.push(single);
             //                     }

             //                 }
             //             });
             var uploaderCenterImg = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',
                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#uploadCenterImg', multiple: false }, //pick: '#uploadCenterImg',
                 auto: true,
                 //            formData: { guid: guid },
                 // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                 resize: false,
                 //切片
                 chunked: true,
                 //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
                 chunkSize: 2 * 1024 * 1024,
                 fileSingleSizeLimit: 5 * 1024 * 1024, // 限制在5M
                 threads: 1,
                 accept: {
                     title: 'Images',
                     extensions: 'gif,jpg,jpeg,bmp,png',
                     mimeTypes: 'image/*'
                 }
             });
             uploaderCenterImg.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowMuBan3Img.centerImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.centerImg), 1)
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmShowMuBan3Img.centerImg(new Array());
                     vm.vmShowMuBan3Img.centerImg.push(single);
                 }

             });


             //             try {
             //                 $('#uploadFiveImg').uploadify('destroy');
             //             } catch (Error) {
             //             }
             //             $("#uploadFiveImg").uploadify({
             //                 'debug': false, //开启调试
             //                 'auto': true, //是否自动上传
             //                 'buttonText': '',
             //                 'buttonImage': '',
             //                 'buttonClass': '',
             //                 'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
             //                 'queueID': 'uploadfileQueue', //文件选择后的容器ID
             //                 'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
             //                 'width': '79',
             //                 'height': '79',
             //                 'multi': false,
             //                 'queueSizeLimit': 1,
             //                 //             'uploadLimit': 1,
             //                 'fileTypeDesc': '支持的格式：',
             //                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             //                 'fileSizeLimit': '5MB',
             //                 'removeTimeout': 0,
             //                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             //                 'onSelect': function (file) {
             //                     vm.vmShowMuBan3Img.fiveImg.push(vm.vmShowMuBan3Img.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.fiveImg().length + "个文件！");
             //                             break;
             //                         case -110:
             //                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#uploadFiveImg').uploadify('settings', 'fileSizeLimit') + "大小！");
             //                             break;
             //                         case -120:
             //                             alert("文件 [" + file.name + "] 大小异常！");
             //                             break;
             //                         case -130:
             //                             alert("文件 [" + file.name + "] 类型不正确！");
             //                             break;
             //                     }
             //                 },
             //                 'onUploadError': function (file, errorCode, errorMsg, errorString) {

             //                 },
             //                 //检测FLASH失败调用
             //                 'onFallback': function () {
             //                     alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
             //                 },
             //                 //上传到服务器，服务器返回相应信息到data里
             //                 'onUploadSuccess': function (file, data, response) {
             //                     vm.vmShowMuBan3Img.fiveImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.fiveImg), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg,
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowMuBan3Img.fiveImg(new Array());
             //                         vm.vmShowMuBan3Img.fiveImg.push(single);
             //                     }
             //                 }
             //             });

             var uploaderFiveImg = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',
                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#uploadFiveImg', multiple: false }, //pick: '#uploadFiveImg',
                 auto: true,
                 //            formData: { guid: guid },
                 // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                 resize: false,
                 //切片
                 chunked: true,
                 //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
                 chunkSize: 2* 1024 * 1024,
                 fileSingleSizeLimit: 5 * 1024 * 1024, // 限制在5M
                 threads: 1,
                 accept: {
                     title: 'Images',
                     extensions: 'gif,jpg,jpeg,bmp,png',
                     mimeTypes: 'image/*'
                 }
             });
             uploaderFiveImg.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowMuBan3Img.fiveImg.splice($.inArray(vm.vmShowMuBan3Img.loadingImage, vm.vmShowMuBan3Img.fiveImg), 1)
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg,
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmShowMuBan3Img.fiveImg(new Array());
                     vm.vmShowMuBan3Img.fiveImg.push(single);
                 }
             });
         },
         vmShowMuBan3Img: null
     }
     return vm;
 });