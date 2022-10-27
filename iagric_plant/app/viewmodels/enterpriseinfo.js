define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'jquery.querystring', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'logininfo', 'webuploader'],
 function (router, ko, $, kv, utils, qs, dialog, jqueryui, uploadify, loginInfo, webuploader) {
     var moduleInfo = {
         moduleID: '10400',
         parentModuleID: '10001'
     }
     var vmEnterpriseInfo = function () {
         var self = this;
         self.superiorUnit = ko.observable();
         self.unitName = ko.observable();
         self.area = ko.observable();
         self.EWM = ko.observable();
         self.trade = ko.observable();
         self.etrade = ko.observable();
         self.etrades = ko.observableArray();
         self.trades = [];
         self.selTypeArray = ko.observableArray();
         self.zjType = ko.observable();
         self.zjhm = ko.observable().extend({
             maxLength: { params: 100, message: "证件号码最大长度为100个字符!" },
             required: { params: true, message: "请输入相应的证件号码!" }
         });
         self.personName = ko.observable().extend({
             maxLength: { params: 10, message: "联系人最大长度为10个字符!" },
             required: { params: true, message: "请输入联系人!" }
         });

         self.telephone = ko.observable().extend({
             maxLength: { params: 30, message: "联系电话最大长度为30个字符!" },
             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "电话格式不正确!" }
         });

         self.email = ko.observable().extend({
             maxLength: { params: 50, message: "邮箱最大长度为50个字符!" },
             required: { params: true, message: "请输入邮箱!" },
             email: { params: true, message: "邮箱格式不正确!" }
         });

         self.address = ko.observable().extend({
             maxLength: { params: 100, message: "地址最大长度为100个字符!" },
             required: { params: true, message: "请输入地址!" }
         });

         self.memo = ko.observable();
         self.weburl = ko.observable();
         self.logo = ko.observable();
         self.file = ko.observable();
         self.files = ko.observableArray();
         self.yyzzs = ko.observableArray([]);

         self.selTradeName = ko.observable(false);

         self.selLogo = ko.observable(false);
         self.selZhiZhao = ko.observable(false);
         self.selTrade = ko.observable(false);
         self.loadingImage = {
             fileUrl: '../../images/load.gif', //ko.observable(result[1])
             fileUrls: '../../images/load.gif'
         };

         self.trade.subscribe(function () {
             var defaultItem = { TradeName: '暂无相应二级行业', Trade_ID: '-1' };
             self.etrade(undefined);
             if (!self.trade()) {
                 //self.etrades(defaultItem);
                 return;
             }
             var selectedCode = self.trade();
             var sendData = {
                 parent: selectedCode
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_EnterpriseInfo/ETradeList",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 async: false,
                 data: JSON.stringify(sendData),
                 success: function (jsonResult) {
                     self.etrades(jsonResult.ObjList);
                 }
             });
         });

         self.etrade.subscribe(function () {
             if (!self.etrade()) {
                 self.selTrade(true);
             }
             else {
                 self.selTrade(false);
             }
         });
         self.init = function () {
             var sendData = {}
             $.ajax({
                 type: "POST",
                 url: "/Admin_EnterpriseInfo/Index",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (obj) {

                     if (obj.ObjModel.CompanyName == null) {
                         self.superiorUnit('暂无');
                     }
                     else {
                         self.superiorUnit(obj.ObjModel.CompanyName);
                     }
                     self.unitName(obj.ObjModel.EnterpriseName);
                     self.area(obj.ObjModel.sheng + "-" + obj.ObjModel.shi + "-" + obj.ObjModel.qu);
                     self.EWM(obj.ObjModel.MainCode);
                     self.personName(obj.ObjModel.LinkMan);
                     self.telephone(obj.ObjModel.LinkPhone);
                     self.email(obj.ObjModel.Email);
                     self.address(obj.ObjModel.Address);
                     self.memo(obj.ObjModel.Memo);
                     self.weburl(obj.ObjModel.WebURL);
                     if (obj.ObjModel.Logo != null && obj.ObjModel.Logo != '') {
                         var single = {
                             fileUrl: obj.ObjModel.Logo, //ko.observable(result[1])
                             fileUrls: obj.ObjModel.Logo
                         }
                         self.files.push(single);
                     }
                     //alert(obj.ObjModel.zhizhao);
                     if (obj.ObjModel.zhizhao != null && obj.ObjModel.zhizhao != '') {
                         var single = {
                             fileUrl: obj.ObjModel.zhizhao, //ko.observable(result[1])
                             fileUrls: obj.ObjModel.zhizhao
                         }
                         self.yyzzs.push(single);
                     }
                     self.trade(obj.ObjModel.Trade_ID);
                     self.etrade(obj.ObjModel.Etrade_ID);
                 }
             })
         }

         self.save = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0 && self.etrade() && self.files().length > 0 && self.yyzzs().length > 0) {
                 var sendData = {
                     personName: self.personName(),
                     telephone: self.telephone(),
                     email: self.email(),
                     webUrl: self.weburl(),
                     trade: self.trade(),
                     etrade: self.etrade(),
                     address: self.address(),
                     memo: self.memo(),
                     zjType: $("#Select1").val(),
                     zjhm: self.zjhm(),
                     logo: JSON.stringify(self.files()),
                     file: self.yyzzs()[0].fileUrl
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_EnterpriseInfo/Edit",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                             return;
                         };
                         //$("#saveData").removeClass("disabled").html("保存");
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                 });
             } else {
                 if (!self.etrade()) {
                     self.selTrade(true);
                 } else {
                     self.selTrade(false);
                 }
                 if (self.files().length <= 0) {
                     self.selLogo(true);
                 }
                 else {
                     self.selLogo(false);
                 }
                 if (self.yyzzs().length <= 0) {
                     self.selZhiZhao(true);
                 }
                 else {
                     self.selZhiZhao(false);
                 }
                 self.errors.showAllMessages();
             }
         }
     }
     /*************************************************************************/
     //获取行业模块
     var getTradeModules = function () {
         var sendData = {
             level: 1
         };
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_EnterpriseInfo/TradeList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     /*************************************************************************/
     var delImage = function (data, event) {
         vm.vmEnterpriseInfo.files(new Array());
         vm.vmEnterpriseInfo.selLogo(true);
     }
     var delyyzz = function (data, event) {
         vm.vmEnterpriseInfo.yyzzs(new Array());
         vm.vmEnterpriseInfo.selZhiZhao(true);
     }
     var vm = {

         binding: function () {
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             vm.vmEnterpriseInfo = new vmEnterpriseInfo();
             vm.vmEnterpriseInfo.trades = getTradeModules();
             vm.vmEnterpriseInfo.init();
             vm.vmEnterpriseInfo.selTypeArray([{ "Text": "组织/单位机构代码", "Value": 1 }, { "Text": "统一社会信用代码", "Value": 2 }, { "Text": "个体工商户营业执照号", "Value": 3}]);
             //             try {
             //                 $("#image_upload").uploadify('destroy');
             //             } catch (e) {

             //             }
             //             try {
             //                 $("#yyzz_upload").uploadify('destroy');
             //             }
             //             catch (e) { }
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
                 vm.vmEnterpriseInfo.files.splice($.inArray(vm.vmEnterpriseInfo.loadingImage, vm.vmEnterpriseInfo.files), 1)
                 var dataObj = data; // JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     vm.vmEnterpriseInfo.files(new Array());
                     vm.vmEnterpriseInfo.files.push(single);
                     vm.vmEnterpriseInfo.selLogo(false);
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
             //                 //'uploadLimit': 1,
             //                 'fileTypeDesc': '支持的格式：',
             //                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             //                 'fileSizeLimit': '5MB',
             //                 'removeTimeout': 0,
             //                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
             //                 'onSelect': function (file) {
             //                     vm.vmEnterpriseInfo.files.push(vm.vmEnterpriseInfo.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.vmEnterpriseInfo.files().length + "个文件！");
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
             //                     //alert(data);
             //                     vm.vmEnterpriseInfo.files.splice($.inArray(vm.vmEnterpriseInfo.loadingImage, vm.vmEnterpriseInfo.files), 1)
             //                     var dataObj = JSON.parse(data);
             //                     if (dataObj.code == 0) {
             //                         var single = {
             //                             fileUrl: dataObj.Msg, //ko.observable(result[1])
             //                             fileUrls: dataObj.sMsg
             //                         }
             //                         vm.vmEnterpriseInfo.files(new Array());
             //                         vm.vmEnterpriseInfo.files.push(single);
             //                         vm.vmEnterpriseInfo.selLogo(false);
             //                     }
             //                 }
             //             });

             var yyzz_uploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',
                 //允许重复上传同一张文件
                 duplicate: true,
                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#yyzz_upload', multiple: false }, // pick: '#image_upload',
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
             yyzz_uploader.on('uploadSuccess', function (file, data, response) {
                 vm.vmEnterpriseInfo.yyzzs.splice($.inArray(vm.vmEnterpriseInfo.loadingImage, vm.vmEnterpriseInfo.yyzzs), 1)
                 //                 var result = data.split('|');
                 //                 var single = {
                 //                     fileUrl: result[1],
                 //                     fileUrls: result[1]
                 //                 }
                 //                 vm.vmEnterpriseInfo.yyzzs(new Array());
                 //                 vm.vmEnterpriseInfo.yyzzs.push(single);
                 //                 vm.vmEnterpriseInfo.selZhiZhao(false);
                 var result = data; // JSON.parse(data);
                 if (result.code == 0) {
                     var single = {
                         fileUrl: result.Msg, //ko.observable(result[1])
                         fileUrls: result.sMsg
                     }
                     vm.vmEnterpriseInfo.yyzzs(new Array());
                     vm.vmEnterpriseInfo.yyzzs.push(single);
                     vm.vmEnterpriseInfo.selZhiZhao(false);
                 }

             });

             yyzz_uploader.on('uploadError', function (file, errorCode, errorMsg) {
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
             //             $("#yyzz_upload").uploadify({
             //                 'debug': false, //开启调试
             //                 'auto': true, //是否自动上传
             //                 'buttonText': '',
             //                 'buttonClass': '',
             //                 'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
             //                 'queueID': 'uploadfileQueue', //文件选择后的容器ID
             //                 'uploader': '/lib/jquery.uploadify-v3.2/handler/upload.ashx',
             //                 'width': '84',
             //                 'height': '84',
             //                 'multi': false,
             //                 'queueSizeLimit': 1,
             //                 'fileTypeDesc': '支持的格式：',
             //                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
             //                 'fileSizeLimit': '5MB',
             //                 'removeTimeout': 0,
             //                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback'],
             //                 'onSelect': function (file) {
             //                     vm.vmEnterpriseInfo.yyzzs.push(vm.vmEnterpriseInfo.loadingImage);
             //                     //preview(file);
             //                 },
             //                 //返回一个错误，选择文件的时候触发
             //                 'onSelectError': function (file, errorCode, errorMsg) {
             //                     switch (errorCode) {
             //                         case -100:
             //                             alert("上传的文件数量已经超出系统限制的" + vm.vmEnterpriseInfo.yyzzs().length + "个文件！");
             //                             break;
             //                         case -110:
             //                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#yyzz_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
             //                             break;
             //                         case -120:
             //                             alert("文件 [" + file.name + "] 大小异常！");
             //                             break;
             //                         case -130:
             //                             alert("文件 [" + file.name + "] 类型不正确！");
             //                             break;
             //                     }
             //                 },
             //                 //检测FLASH失败调用
             //                 'onFallback': function () {
             //                     alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
             //                 },
             //                 //上传到服务器，服务器返回相应信息到data里
             //                 'onUploadSuccess': function (file, data, response) {
             //                     vm.vmEnterpriseInfo.yyzzs.splice($.inArray(vm.vmEnterpriseInfo.loadingImage, vm.vmEnterpriseInfo.yyzzs), 1)
             //                     var result = data.split('|');
             //                     var single = {
             //                         fileUrl: result[1],
             //                         fileUrls: result[1]
             //                     }
             //                     vm.vmEnterpriseInfo.yyzzs(new Array());
             //                     vm.vmEnterpriseInfo.yyzzs.push(single);
             //                     vm.vmEnterpriseInfo.selZhiZhao(false);
             //                 }
             //             });
         },
         goBack: function () {
             router.navigateBack();
         },
         vmEnterpriseInfo: null,
         delImage: delImage,
         delyyzz: delyyzz
     }
     return vm;
 });