define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'jquery.querystring', 'kindeditor.zh-CN', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.fileDownload', 'webuploader'],
 function (router, ko, $, kv, utils, loginInfo, qs, kcn, dialog, jqueryui, uploadify, jfd, webuploader) {
     var moduleInfo = {
         moduleID: '10600',
         parentModuleID: '10001'
     }
     var editorCompany;
     var vmShowCompany = function () {
         var self = this;

         self.ewm = ko.observable();
         self.url = ko.observable();
         self.createUrl = ko.observable();
         self.webUrl = ko.observable();
         self.orderingHotline = ko.observable();

         self.linkMan = ko.observable().extend({
             maxLength: { params: 10, message: "联系人最大长度为10个字符!" },
             required: { params: true, message: "请输入联系人!" }
         });

         self.videoUrl = ko.observable().extend({
             pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "视频地址不正确!" }
         });

         self.linkPhone = ko.observable().extend({
             maxLength: { params: 30, message: "联系电话最大长度为30个字符!" },
             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "电话格式不正确!" }
         });

         self.email = ko.observable().extend({
             maxLength: { params: 50, message: "邮箱最大长度为50个字符!" },
             required: { params: true, message: "请输入邮箱!" },
             email: { params: true, message: "邮箱格式不正确!" }
         });
         self.businessLicence = ko.observable().extend({
             maxLength: { params: 20, message: "营业执照号最大长度为20个字符!" },
             //             pattern: { params: /(^[0-9a-zA-Z]{20}$)/g, message: "营业执照号格式不正确!" },
             required: { params: true, message: "请输入营业执照号!" }
         });
         self.selBusinessLicence = ko.observable(true);

         self.address = ko.observable().extend({
             maxLength: { params: 100, message: "地址最大长度为100个字符!" },
             required: { params: true, message: "请输入地址!" }
         });
         self.taobaourl = ko.observable().extend({
             pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "请输入正确的商城链接！" }
         });
         self.jingdongurl = ko.observable().extend({
             pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "请输入正确的商城链接！" }
         });
         self.tianmaourl = ko.observable().extend({
             pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "请输入正确的商城链接！" }
         });
         /***************实时视频*****************/
         self.newVideoUrl = ko.observable().extend({
             pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "请输入正确的商城链接！" }
         });
         self.selNewVideoUrl = ko.observable(false);
         self.newVideoUrls = ko.observableArray();
         self.addNewVideoUrl = function (data, event) {
             if (!self.newVideoUrl()) {
                 self.selNewVideoUrl(true);
                 return;
             }
             self.selNewVideoUrl(false);
             self.newVideoUrls.push({ videoUrl: self.newVideoUrl() });
             self.newVideoUrl('');
         }
         self.delNewVideoUrl = function (data, event) {
             var index = self.newVideoUrls.indexOf(data);
             self.newVideoUrls.splice(index, 1);
         }
         self.memo = ko.observable();
         self.files = ko.observableArray([]);
         //2018-09-07新加企业微信logo
         self.wxlogo = ko.observableArray([]);
         self.delImagewxlogo = function (data, event) {
             var index = self.wxlogo.indexOf(data);
             self.wxlogo.splice(index, 1);
         }
         //2018-09-07新加微信简介
         self.WxInfo = ko.observable();
         self.ggaoimg = ko.observableArray([]);
         self.calcel = function () {
             self.close(this);
         }
         self.loadingImage = {
             fileUrl: '../../images/load.gif',
             fileUrls: '../../images/load.gif'
         };
         self.loadingImagegg = {
             jcfileUrl: '../../images/load.gif',
             jcfileUrls: '../../images/load.gif'
         };
         self.init = function () {
             $.ajax({
                 type: "POST",
                 url: "/Admin_Enterprise_Show/Index",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 success: function (jsonResult) {
                     self.businessLicence(jsonResult.ObjModel.BusinessLicence);
                     self.webUrl(jsonResult.ObjModel.WebURL);
                     self.videoUrl(jsonResult.ObjModel.VideoUrl);
                     self.linkMan(jsonResult.ObjModel.LinkMan);
                     self.linkPhone(jsonResult.ObjModel.LinkPhone);
                     self.email(jsonResult.ObjModel.Email);
                     self.address(jsonResult.ObjModel.Address);
                     self.orderingHotline(jsonResult.ObjModel.OrderingHotline);
                     self.memo(jsonResult.ObjModel.Memo);
                     self.ewm(jsonResult.ObjModel.EWM);
                     self.url(jsonResult.ObjModel.Url);
                     self.taobaourl(jsonResult.ObjModel.TaoBaoLink);
                     self.jingdongurl(jsonResult.ObjModel.JingDongLink);
                     self.tianmaourl(jsonResult.ObjModel.TianMaoLink);
                     self.files(jsonResult.ObjModel.imgs);
                     self.newVideoUrls(jsonResult.ObjModel.videoUrls);
                     self.ggaoimg(jsonResult.ObjModel.imgsgg);
                     self.createUrl('/ShowImage/ShowImgNoUrl?ewm=' + jsonResult.ObjModel.Url);
                     self.wxlogo(jsonResult.ObjModel.wxlogoimgs);
                     self.WxInfo(jsonResult.ObjModel.WXInfo);
                 }
             });
         }

         self.SaveShowCompany = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     Memo: editorCompany.html(),
                     Files: JSON.stringify(self.files()),
                     videoUrl: self.videoUrl(),
                     LinkMan: self.linkMan(),
                     LinkPhone: self.linkPhone(),
                     Email: self.email(),
                     WebUrl: self.webUrl(),
                     Address: self.address(),
                     TaoBaoLink: self.taobaourl(),
                     JingDongLink: self.jingdongurl(),
                     TianMaoLink: self.tianmaourl(),
                     OrderingHotline: self.orderingHotline(),
                     businessLicence: self.businessLicence(),
                     Filesgg: JSON.stringify(self.ggaoimg()),
                     ssvideoUrl: JSON.stringify(self.newVideoUrls()),
                     WXLogo: JSON.stringify(self.wxlogo()),
                     WXInfo: self.WxInfo()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Enterprise_Show/Edit",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
             }
         }
         self.delImage = function (data, event) {
             var index = self.files.indexOf(data);
             self.files.splice(index, 1);
         }
         self.delImagegg = function (data, event) {
             var index = self.ggaoimg.indexOf(data);
             self.ggaoimg.splice(index, 1);
         }
         self.ImageSize = function () {
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             $.fileDownload('/Public/MemberDown?oid=' + self.ewm() + '&type=1&w=150&h=150')
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });
         }
     }

     var vm = {
         activate: function () {
         },
         binding: function () {
             //初始化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             vm.vmShowCompany = new vmShowCompany();
             vm.vmShowCompany.init();
             editorCompany = KindEditor.create("#txtInfos", {
                 cssPath: '/lib/kindeditor/plugins/code/prettify.css',
                 uploadJson: '/lib/kindeditor/upload_json.ashx',
                 fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
                 allowFileManager: true,
                 //                 items: [
                 //						'fontname', 'fontsize', '|', 'forecolor', 'bold', 'italic', 'underline',
                 //						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
                 //						'insertunorderedlist', '|', 'image'],
                 afterCreate: function () { },
                 afterBlur: function () { this.sync(); }
             });
             editorCompany.html(vm.vmShowCompany.memo());


             var imageUploader = webuploader.create({

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
             imageUploader.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowCompany.files.splice($.inArray(vm.vmShowCompany.loadingImage, vm.vmShowCompany.files), 1);
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmShowCompany.files(new Array());
                     vm.vmShowCompany.files.push(single);
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
             //                     vm.vmShowCompany.files.push(vm.vmShowCompany.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.files().length + "个文件！");
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
             //                     vm.vmShowCompany.files.splice($.inArray(vm.vmShowCompany.loadingImage, vm.vmShowCompany.files), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg, //ko.observable(result[1])
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowCompany.files(new Array());
             //                         vm.vmShowCompany.files.push(single);
             //                     }

             //                 }
             //             });
             //2018-09-07新加企业微信logo

             var wxlogoUploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',
                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#uploadwxlogo', multiple: false }, // pick: '#uploadwxlogo',
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
             wxlogoUploader.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowCompany.wxlogo.splice($.inArray(vm.vmShowCompany.loadingImage, vm.vmShowCompany.wxlogo), 1);
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmShowCompany.wxlogo(new Array());
                     vm.vmShowCompany.wxlogo.push(single);
                 }

             });


             //             try {
             //                 $('#uploadwxlogo').uploadify('destroy');
             //             } catch (Error) {
             //             }
             //             $("#uploadwxlogo").uploadify({
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
             //                     vm.vmShowCompany.wxlogo.push(vm.vmShowCompany.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.wxlogo().length + "个文件！");
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
             //                     vm.vmShowCompany.wxlogo.splice($.inArray(vm.vmShowCompany.loadingImage, vm.vmShowCompany.wxlogo), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg, //ko.observable(result[1])
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowCompany.wxlogo(new Array());
             //                         vm.vmShowCompany.wxlogo.push(single);
             //                     }

             //                 }
             //             });


             var jca0Uploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',

                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#JCA0', multiple: false }, // pick: '#JCA0',
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
             jca0Uploader.on('uploadSuccess', function (file, data, response) {
                 vm.vmShowCompany.ggaoimg.splice($.inArray(vm.vmShowCompany.loadingImagegg, vm.vmShowCompany.ggaoimg), 1)
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         jcfileUrl: dataObj.Msg,
                         jcfileUrls: dataObj.sMsg
                     }
                     vm.vmShowCompany.ggaoimg.push(single);
                 }

             });
             //             try {
             //                 $('#JCA0').uploadify('destroy');
             //             } catch (Error) {
             //             }
             //             $("#JCA0").uploadify({
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
             //                     vm.vmShowCompany.ggaoimg.push(vm.vmShowCompany.loadingImagegg);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.ggaoimg().length + "个文件！");
             //                             break;
             //                         case -110:
             //                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A0').uploadify('settings', 'fileSizeLimit') + "大小！");
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
             //                     vm.vmShowCompany.ggaoimg.splice($.inArray(vm.vmShowCompany.loadingImagegg, vm.vmShowCompany.ggaoimg), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             jcfileUrl: dataObj.Msg,
             //                             jcfileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmShowCompany.ggaoimg.push(single);
             //                     }
             //                 }
             //             });
         },
         vmShowCompany: null
     }
     return vm;
 });

