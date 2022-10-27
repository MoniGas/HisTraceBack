define(['plugins/router', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'knockout', 'jquery-ui', 'jquery.uploadify', 'webuploader'],
 function (router, $, bdp, kv, utils, loginInfo, dialog, ko, jqueryui, uploadify, webuploader) {
     var vmObj;
     var vmJianCe_Add = function (id, beid) {
         var self = this;
         self.id = id;
         self.beid = beid;
         self.AddDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
         self.jianceDate = ko.observable(false);

         self.Content = ko.observable('').extend({
             maxLength: { params: 150, message: "名称最大长度为150个字符" },
             required: {
                 params: true,
                 message: "请输入检测标题！"
             }
         });
         self.selTitle = ko.observable(false);
         self.files = ko.observableArray();
         self.jianceimg = ko.observable(false);
         self.calcel = function () {
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
             if (self.errors().length <= 0 && !(self.files() == "" || self.files() == "[]") && !(self.AddDate() == null || self.AddDate() == "" || self.AddDate() == "undefined")) {
                 var sendData = {
                     batchid: self.id,
                     batchextid: self.beid,
                     AddDate: self.AddDate(),
                     Content: self.Content(),
                     files: JSON.stringify(self.files())
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Batch_JianYanJianYi/Add",
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
                                 self.closeOK('有');
                             }
                         }
                         }]);
                     }
                 })
             } else {
                 self.errors.showAllMessages();
                 if (self.AddDate() == null || self.AddDate() == "" || self.AddDate() == "undefined") {
                     self.jianceDate(true);
                 }
                 else {
                     self.jianceDate(false);
                 }
                 if (self.files() == "" || self.files() == "[]") {
                     self.jianceimg(true);
                 }
                 else {
                     self.jianceimg(false);
                 }
             }
         }
     }
     vmJianCe_Add.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

         $('#AddDate').datepicker({
             language: 'cn',
             autoclose: true,
             todayHighlight: true
         });
         var uploader = webuploader.create({

             // swf文件路径
             swf: '/lib/webuploader/Uploader.swf',
             //允许重复上传同一张文件
             duplicate: true,
             // 文件接收服务端。
             server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

             // 选择文件的按钮。可选。
             // 内部根据当前运行是创建，可能是input元素，也可能是flash.
             pick: { id: '#image_upload', multiple: false }, // pick: '#image_upload',
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
                 //                     vmObj.files.push(single);
                 vmObj.files(new Array());
                 vmObj.files.push(single);
                 vmObj.jianceimg(false);
             }
         });

         uploader.on('uploadError', function (file, errorCode, errorMsg) {
             //$('#' + file.id).find('p.state').text('上传出错');
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
         //             'width': '79',
         //             'height': '79',
         //             'multi': false,
         //             'queueSizeLimit': 1,
         //             //             'uploadLimit': 1,
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
         //                     vmObj.jianceimg(false);
         //                 }

         //             }
         //         });
     }
     vmJianCe_Add.prototype.delImage = function (data, event) {
         vmObj.files(new Array());
         vmObj.jianceimg(true);
     }
     vmJianCe_Add.prototype.close = function () {
         $('#image_upload').uploadify('destroy');
         dialog.close(this);
     }
     vmJianCe_Add.prototype.closeOK = function (isno) {
         dialog.close(this, isno);
     }
     vmJianCe_Add.show = function (id, beid) {
         vmObj = new vmJianCe_Add(id, beid);
         return dialog.show(vmObj);
     };
     return vmJianCe_Add;
 });