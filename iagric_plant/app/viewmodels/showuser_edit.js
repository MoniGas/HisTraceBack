define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jquery.querystring', 'jqPaginator', 'jquery-ui', 'webuploader'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, qs, jq, jqueryui, webuploader) {

     var vmUser = function (id) {
         var self = this;
         self.files = ko.observableArray();
         self.touxiangimg = ko.observable(false);
         self.name = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入用户名称!"
             }
         });
         self.position = ko.observable('').extend({
             maxLength: { params: 50, message: "名称最大长度为50个字符" },
             required: {
                 params: true,
                 message: "请输入职位名称!"
             }
         });
         self.telPhone = ko.observable().extend({
             maxLength: { params: 30, message: "联系电话最大长度为30个字符!" },
             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "电话格式不正确!" }
         });
         self.mail = ko.observable().extend({
             maxLength: { params: 50, message: "邮箱最大长度为50个字符!" },
             required: { params: true, message: "请输入邮箱!" },
             email: { params: true, message: "邮箱格式不正确!" }
         });
         self.qq = ko.observable('');
         self.hometown = ko.observable('');
         self.location = ko.observable('');
         self.memo = ko.observable('');
         self.selTitle = ko.observable(false);
         self.loadingImage = {
             fileUrl: '../../images/load.gif', //ko.observable(result[1])
             fileUrls: '../../images/load.gif'
         };
         self.init = function () {
             var sendData = {
                 id: id
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_ShowUser/GetModel",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (obj) {

                     if (obj.code == 0) {
                         bootbox.alert({
                             title: "提示",
                             message: obj.Msg,
                             callback: function () {

                             }
                         });
                         return;
                     }
                     var single = {
                         fileUrl: obj.ObjModel.headimg, //ko.observable(result[1])
                         fileUrls: obj.ObjModel.headimg
                     }
                     self.files.push(single);
                     self.name(obj.ObjModel.Infos);
                     self.position(obj.ObjModel.position);
                     self.telPhone(obj.ObjModel.telPhone);
                     self.mail(obj.ObjModel.mail);
                     self.qq(obj.ObjModel.qq);
                     self.hometown(obj.ObjModel.hometown);
                     self.location(obj.ObjModel.location);
                     self.memo(obj.ObjModel.memo);
                 }
             });
         }

         self.Save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && !(self.files() == "" || self.files() == "[]" || self.files == null)) {
                 var sendData = {
                     uId: id,
                     img: self.files()[0].fileUrl,
                     //                     img: JSON.stringify(self.files()),
                     name: self.name(),
                     position: self.position(),
                     telPhone: self.telPhone(),
                     mail: self.mail(),
                     qq: self.qq(),
                     hometown: self.hometown(),
                     location: self.location(),
                     memo: self.memo(),
                     infos: ""
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_ShowUser/Edit",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须      
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (obj) {
                         if (loginInfo.isLoginTimeout(obj.code, obj.Msg, 1)) {
                             return;
                         };
                         dialog.showMessage(obj.Msg, '系统提示', [{ title: '确定', callback: function () {
                             if (obj.code == 1) {
                                 self.close();
                             }
                         }
                         }]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
                 if (self.files() == "" || self.files() == "[]" || self.files == null) {
                     self.touxiangimg(true);
                 }
                 else {
                     self.touxiangimg(false);
                 }
             }
         }
     }

     vmUser.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
         var uploader = webuploader.create({

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
             threads: 1,
             accept: {
                 title: 'Images',
                 extensions: 'gif,jpg,jpeg,bmp,png',
                 mimeTypes: 'image/*'
             }
         });
         uploader.on('uploadSuccess', function (file, data, response) {
             vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1);
             var dataObj = data; //JSON.parse(data);
             if (dataObj.code == 0) {
                 var single = {
                     fileUrl: dataObj.Msg, //ko.observable(result[1])
                     fileUrls: dataObj.sMsg
                 }
                 vmObj.files(new Array());
                 vmObj.files.push(single);
                 vmObj.touxiangimg(false);
             }
         });
         //         try {
         //             $('#image_upload').uploadify('destroy');
         //         } catch (Error) {
         //         }
         //         $("#image_upload").uploadify({
         //             'debug': false, //开启调试
         //             'auto': true, //是否自动上传
         //             'buttonText': '',
         //             'buttonImage': '',
         //             'buttonClass': '',
         //             'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
         //             'queueID': 'uploadfileQueue', //文件选择后的容器ID
         //             'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
         //             'width': '74',
         //             'height': '74',
         //             'multi': false,
         //             'queueSizeLimit': 1,
         ////             'uploadLimit': 1,
         //             'fileTypeDesc': '支持的格式：',
         //             'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
         //             'fileSizeLimit': '5MB',
         //             'removeTimeout': 0,
         //             'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
         //             'onSelect': function (file) {
         //                 vmObj.files.push(vmObj.loadingImage);
         //                 //preview(file);
         //             },
         //             //返回一个错误，选择文件的时候触发
         //             'onSelectError': function (file, errorCode, errorMsg) {
         //                 switch (errorCode) {
         //                     case -100:
         //                         alert("上传的文件数量已经超出系统限制的" + vmObj.files().length + "个文件！");
         //                         break;
         //                     case -110:
         //                         alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
         //                         break;
         //                     case -120:
         //                         alert("文件 [" + file.name + "] 大小异常！");
         //                         break;
         //                     case -130:
         //                         alert("文件 [" + file.name + "] 类型不正确！");
         //                         break;
         //                 }
         //             },
         //             'onUploadError': function (file, errorCode, errorMsg, errorString) {

         //             },
         //             //检测FLASH失败调用
         //             'onFallback': function () {
         //                 alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
         //             },
         //             //上传到服务器，服务器返回相应信息到data里
         //             'onUploadSuccess': function (file, data, response) {
         //                 vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
         //                 var dataObj = JSON.parse(data);
         //                 if (dataObj.code == 0) {
         //                     var single = {
         //                         fileUrl: dataObj.Msg, //ko.observable(result[1])
         //                         fileUrls: dataObj.sMsg
         //                     }
         //                     //                     vmObj.files.push(single);
         //                     vmObj.files(new Array());
         //                     vmObj.files.push(single);
         //                     vmObj.touxiangimg(false);
         //                 }
         //             }
         //         });
     }
     vmUser.prototype.delImage = function (data, event) {
         vmObj.files(new Array());
         vmObj.touxiangimg(true);
     }
     vmUser.prototype.close = function () {
         dialog.close(this);
     }
     var vmObj;
     vmUser.show = function (id) {
         vmObj = new vmUser(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmUser;
 });