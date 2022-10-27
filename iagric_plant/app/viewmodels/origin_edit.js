define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'jquery.querystring', 'utils', 'bootbox', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo', 'webuploader'],
 function (router, ko, $, kv, qs, utils, bootbox, dialog, jqueryui, uploadify, loginInfo, webuploader) {
     var vmObj;
     var vmOriginEdit = function (id) {
         var self = this;
         self.OriginName = ko.observable().extend({
             maxLength: { params: 100, message: "名称最大长度为100个字符" },
             required: {
                 params: true,
                 message: "请输入原材料名称！"
             }
         });
         self.id = id;
         self.selTitle = ko.observable(false);
         self.Descriptions = ko.observable();
         self.files = ko.observableArray();
         self.originOriginImgInfo = ko.observable('');
         self.originImgInfo = ko.observable(false);
         self.init = function () {
             var sendData = {
                 id: self.id
             };
             $.ajax({
                 type: "POST",
                 url: "/AdminOrigin/GetModel",
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
                     self.OriginName(jsonResult.ObjModel.OriginName);
                     self.Descriptions(jsonResult.ObjModel.Descriptions);
                     self.files(jsonResult.ObjModel.imgs);
                     //                     if (jsonResult.ObjModel.OriginImgInfo != null && jsonResult.ObjModel.OriginImgInfo != '') {
                     //                         var single = {
                     //                             fileUrl: jsonResult.ObjModel.OriginImgInfo,
                     //                             fileUrls: jsonResult.ObjModel.OriginImgInfo
                     //                         }
                     //                         self.files.push(single);
                     //                     }
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
                     OriginName: self.OriginName(),
                     Descriptions: editorOrigin.html(),
                     //                     originImgInfo: self.files()[0].fileUrl
                     originOriginImgInfo: JSON.stringify(self.files())
                 }
                 //alert(JSON.stringify(sendData));
                 $.ajax({
                     type: "POST",
                     url: "/AdminOrigin/Edit",
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
                                 self.closeOK(sendData.OriginName, sendData.Descriptions, sendData.originImgInfo);
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 self.errors.showAllMessages();
                 if (self.files() == null || self.files() == "" || self.files() == "undefined") {
                     self.originImgInfo(true);
                 }
                 else {
                     self.originImgInfo(false);
                 }
             }
         }
     }
     vmOriginEdit.prototype.binding = function () {
         editorOrigin = KindEditor.create("#txtInfos", {
             cssPath: '/lib/kindeditor/plugins/code/prettify.css',
             uploadJson: '/lib/kindeditor/upload_json.ashx',
             fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
             allowFileManager: true,
             items: [
						'fontname', 'fontsize', '|', 'forecolor', 'bold', 'italic', 'underline',
						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
						'insertunorderedlist', '|', 'image'],
             afterCreate: function () { },
             afterBlur: function () { this.sync(); }
         });
         editorOrigin.html(vmObj.Descriptions());
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
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
         //                     //                     vmObj.files(new Array());
         //                     vmObj.files.push(single);
         //                     vmObj.originImgInfo(false);
         //                 }

         //             }
         //         });
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
                 //                     vmObj.files.push(single);
                 //                     vmObj.files(new Array());
                 vmObj.files.push(single);
                 vmObj.originImgInfo(false);
             }
         });

         uploader.on('uploadError', function (file, errorCode, errorMsg) {
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
             // $('#' + file.id).find('p.state').text('上传出错');
         });

         //         uploader.on('uploadComplete', function (file) {
         //             $('#' + file.id).find('.progress').fadeOut();
         //         });
     }
     vmOriginEdit.prototype.delImage = function (data, event) {
         //         vmObj.files(new Array());
         //         vmObj.logo(true);
         //         var index = vmObj.files.indexOf(data);
         //         vmObj.files.splice(index, 1);
         var index = vmObj.files.indexOf(data);
         vmObj.files.splice(index, 1);
         if (vmObj.files().length == 0) {
             vmObj.originImgInfo(true);
         }
     }
     vmOriginEdit.prototype.close = function () {
         //alert(this.province().code);
         dialog.close(this, "", "", "", 1);
     }
     vmOriginEdit.prototype.closeOK = function (OriginName, Descriptions, originImgInfo) {
         dialog.close(this, OriginName, Descriptions, originImgInfo, 2);
     }
     vmOriginEdit.show = function (id) {
         vmObj = new vmOriginEdit(id);
         vmObj.init();
         return dialog.show(vmObj);
     };
     return vmOriginEdit;

 });