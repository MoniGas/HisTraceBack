define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'jquery.querystring', 'kindeditor.zh-CN', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'webuploader'],
 function (router, ko, $, kv, utils, loginInfo, qs, kcn, dialog, jqueryui, uploadify, webuploader) {

     var vmShowMuBan3Img = function () {
         var self = this;
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
         /******************黑名单码***********************/
         self.property = ko.observableArray([]);
         self.propertyName = ko.observable();
         self.AddProperty = function (data, event) {
             var name = self.propertyName();
             if (name === "" || typeof (name) == "undefined") {
                 alert("不能不为空");
                 return;
             }
             var single = {
                 pName: self.propertyName()
             }
             self.property.push(single);
             self.propertyName('');
         }
         self.delProperty = function (data, event) {
             var index = self.property.indexOf(data);
             self.property.splice(index, 1);
         }
         /*****************************************/
         self.init = function () {
             $.ajax({
                 type: "POST",
                 url: "/BackList/Index",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     self.firstImg(jsonResult.ObjModel.BackImgs);
                     self.property(jsonResult.ObjModel.propertys);
                 }
             });
         }

         self.SaveInfo = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && !(self.firstImg() == null || self.firstImg() == "" || self.firstImg() == "undefined")) {
                 var sendData = {
                     files: JSON.stringify(self.firstImg()),
                     codeInfo: JSON.stringify(self.property())
                 }
                 $.ajax({
                     type: "POST",
                     url: "/BackList/Edit",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         if (jsonResult.code == 1) {
                             //                             self.init();
                         }
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
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
             vm.vmShowMuBan3Img = new vmShowMuBan3Img();
             vm.vmShowMuBan3Img.init();
             var uploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',
                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#image_upload', multiple: false },
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
             uploader.on('uploadSuccess', function (file, data, response) {
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
         },
         vmShowMuBan3Img: null
     }
     return vm;
 });