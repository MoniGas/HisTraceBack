define(['plugins/router', 'knockout','jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'jquery.querystring', 'plugins/dialog', 'jquery.uploadify'],
 function (router, ko,$, bdp, kv, utils, qs, dialog, uploadify) {
     var vmVerify = function () {
         var self = this;
         self.file = ko.observable();
         self.files = ko.observableArray([]);
         self.Verify = function () {
             //        $("#saveData").addClass("disabled").html("正在提交，请稍候");
             var sendData = {
                 fileUrl: JSON.stringify(self.files())
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_EnterpriseInfo/Verify/",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     //$("#saveData").removeClass("disabled").html("保存");
                     var obj = JSON.parse(jsonResult);
                     dialog.showMessage(obj.Msg, '系统提示', [{ title: '确定', callback: function () {
                     }
                     }]);
                 },
                 error: function (e) {
                 }
             })
         }
         /****************************/
         self.init = function () {
             $("#file_upload").uploadify({
                 'debug': false, //开启调试
                 'auto': true, //是否自动上传
                 'buttonText': '',
                 'buttonClass': '',
                 'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                 'queueID': 'uploadfileQueue', //文件选择后的容器ID
                 'uploader': '/lib/jquery.uploadify-v3.2/handler/upload.ashx',
                 'width': '84',
                 'height': '84',
                 'multi': false,
                 'queueSizeLimit': 1,
                 'fileTypeDesc': '支持的格式：',
                 'fileTypeExts': '*.jpg;*.jpge',
                 'fileSizeLimit': '5MB',
                 'removeTimeout': 0,
                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback'],
                 'onSelect': function (file) {
                     //preview(file);
                 },
                 //返回一个错误，选择文件的时候触发
                 'onSelectError': function (file, errorCode, errorMsg) {
                     switch (errorCode) {
                         case -100:
                             alert("上传的文件数量已经超出系统限制的" + $('#file_upload').uploadify('settings', 'queueSizeLimit') + "个文件！");
                             break;
                         case -110:
                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#file_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
                             break;
                         case -120:
                             alert("文件 [" + file.name + "] 大小异常！");
                             break;
                         case -130:
                             alert("文件 [" + file.name + "] 类型不正确！");
                             break;
                     }
                 },
                 //检测FLASH失败调用
                 'onFallback': function () {
                     alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
                 },
                 //上传到服务器，服务器返回相应信息到data里
                 'onUploadSuccess': function (file, data, response) {
                     var result = data.split('|');
                     //                     alert(data);
                     //                     var dataObj = JSON.parse(data);
                     //                     if (dataObj.code == 0) {
                     var single = {
                         fileUrl: result[1]
                     }
                     self.files.push(single);
                     $("#divPreImage").show();
                     $("#preImage").attr("src", result[1]).show();
                     //                     }
                 }
             });
         }
         /****************************/
     }
     var vm = {

         binding: function () {
             vm.vmVerify = new vmVerify();
             vm.vmVerify.init();
         },
         goBack: function () {
             router.navigateBack();
         },
         vmVerify: null
     }
     return vm;
 });