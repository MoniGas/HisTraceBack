define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation',
'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', 'jquery.querystring', "./materialTypeSearch", 'kindeditor.zh-CN', './materialCategorySearch', 'webuploader'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, qs, typeSearch, kcn, materialCategorySearch, webuploader) {
     /******************规格********************/
     var materialSpec = ko.observable('').extend({
         maxLength: { params: 25, message: "产品规格最大长度为25个字符" }
     });
     /**2018年4月16号新加农药的数据***/
     var nyzhenghao = ko.observable('').extend({
         maxLength: { params: 20, message: "农药登记证号最大长度为20个字符" }
     });
     var nytype = ko.observable(0);
     /******************产品名称*************/
     var materialName = ko.observable('').extend({
         maxLength: { params: 50, message: "名称最大长度为50个字符" },
         required: {
             params: true,
             message: "请输入产品名称!"
         }
     });
     /******************产品别名*************/
     var materialAliasName = ko.observable('');
     /*******************价格****************/
     var materialPrice = ko.observable('').extend({
         min: {
             params: 0.001,
             message: "输入价格必须大于0！"
         },
         max: {
             params: 1000000,
             message: "输入价格必须小于100万！"
         },
         pattern: {
             params: /^\d+[\.]?\d{0,2}$/g,
             message: "必须是数字，并且最多两位小数！"
         }
     });
     /******************产品编码*************/
     //     var materialCode = ko.observable('').extend({
     //         maxLength: { params: 20, message: "名称最大长度为20个字符" }
     //     });
     //     var selmaterialCode = ko.observable(false);
     var pingjia1 = ko.observable('').extend({
         maxLength: { params: 6, message: "请输入6个字以内的评价！" }
     });
     var pingjia2 = ko.observable('').extend({
         maxLength: { params: 6, message: "请输入6个字以内的评价！" }
     });
     var pingjia3 = ko.observable('').extend({
         maxLength: { params: 6, message: "请输入6个字以内的评价！" }
     });
     var pingjia4 = ko.observable('').extend({
         maxLength: { params: 6, message: "请输入6个字以内的评价！" }
     });
     var pingjia5 = ko.observable('').extend({
         maxLength: { params: 6, message: "请输入6个字以内的评价！" }
     });
     //获取品牌
     var getBrandModules = function () {
         var data;
         var sendData = {
             pageIndex: 1
         }
         $.ajax({
             type: "POST",
             url: "/Admin_Brand/SelectBrand",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     //获取生产流程
     var getProcessModules = function () {
         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Material/ProcessList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }
     var VmBrand = {
         MaterialBrandArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var VmProcess = {
         ProcessArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     /******************保质期***************/
     var materialShelfLife = ko.observable('');
     var selMaterialShelfLife = ko.observable(false);
     materialShelfLife.subscribe(function () {
         if (materialShelfLife() == "") {
             selMaterialShelfLife(true);
         }
         else {
             selMaterialShelfLife(false);
         }
         var reg = new RegExp("^[0-9]*$");
         if (!reg.test(materialShelfLife())) {
             selMaterialShelfLife(true);
         }
         else {
             selMaterialShelfLife(false);
         }
     })
     var selectedShelfs = [
                { "shefsid": 0, "shefsname": "长期" },
                { "shefsid": -1, "shefsname": "视存储环境" },
                { "shefsid": 1, "shefsname": "天" },
                { "shefsid": 2, "shefsname": "月" },
                { "shefsid": 3, "shefsname": "年" }
            ];
     var selectedShelf = ko.observable();
     /***************实时视频*****************/
     var newVideoUrl = ko.observable();
     var newVideoName = ko.observable();
     var selNewVideoUrl = ko.observable(false);
     var newVideoUrls = ko.observableArray();
     var addNewVideoUrl = function (data, event) {
         if (!newVideoUrl()) {
             selNewVideoUrl(true);
             return;
         }
         if (!newVideoName()) {
             selNewVideoUrl(true);
             return;
         }
         selNewVideoUrl(false);
         var singleVideo = {
             fileUrl: newVideoName(),
             videoUrl: newVideoUrl()
         }
         newVideoUrls.push(singleVideo);
         newVideoUrl('');
         newVideoName('');
     }
     var delNewVideoUrl = function (data, event) {
         var index = newVideoUrls.indexOf(data);
         newVideoUrls.splice(index, 1);
     }
     /***************实时视频*****************/
     /*************自定义属性****************/
     var property = ko.observableArray();
     var propertyName = ko.observable();
     var propertyValue = ko.observable();
     var selPrototype = ko.observable(false);
     var AddProperty = function (data, event) {
         if (!propertyName()) {
             selPrototype(true);
             return;
         }
         if (!propertyValue()) {
             selPrototype(true);
             return;
         }
         selPrototype(false);
         var all = propertyName() + "：" + propertyValue();
         var single = {
             pName: propertyName(),
             pValue: propertyValue(),
             allprototype: all
         }
         property.push(single);
         propertyName('');
         propertyValue('');
     }
     var delProperty = function (data, event) {
         var index = property.indexOf(data);
         property.splice(index, 1);
     }
     /******************广告图片******************/
     var loadingAdImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     var AdFileUrls = ko.observableArray();
     var delAdImage = function (data, event) {
         var index = AdFileUrls.indexOf(data);
         AdFileUrls.splice(index, 1);
     }
     /******************图片******************/
     var loadingImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     var selMaterialImgInfo = ko.observable(false);
     var files = ko.observableArray();
     var delImage = function (data, event) {
         var index = files.indexOf(data);
         files.splice(index, 1);
         if (files().length == 0) {
             selMaterialImgInfo(true);
         }
     }
     /******************视频**********************/
     var loadingvideo = {
         videoUrl: '../../images/load.gif', //ko.observable(result[1])
         videoUrls: '../../images/load.gif'
     };
     var videos = ko.observableArray();
     var delVideo = function (data, event) {
         var index = videos.indexOf(data);
         videos.splice(index, 1);
     }
     /*************产品购买链接*************/
     var tburl = ko.observable().extend({
         pattern: { params: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/, message: "请输入正确的购买链接！" }
     });
     var seltburl = ko.observable(false);
     var materialURL = function () {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var currentObj = $(event.target);
         currentObj.blur();
         var type = $('#buyUrl').is(':checked') ? 1 : 2;
         //      var strType = type == 2 ? "确定取消填写产品购买链接吗？" : "关闭产品推广将清除所有已推广的产品，您确定要关闭产品推广吗？";
         //            dialog.showMessage(strType, '系统提示', [
         if (type == 1) {
             //         $("input[name='buyUrl']").prop("checked", true);
             //         $("input[name='tburl']").prop("readonly", false);
             var c1 = $("#buyUrl");
             c1.css({ "display": "none" });
             var c2 = $("#Checkbox1");
             c2.css({ "display": "" });
             $("input[name='Checkbox1']").prop("checked", true);
             $("input[name='tburl']").prop("readonly", false);

         }
         else if (type == 2) {
             dialog.showMessage("确定取消填写产品购买链接吗？", '系统提示', [
                {
                    title: '确定',
                    callback: function () {
                        var c1 = $("#buyUrl");
                        c1.css({ "display": "" });
                        var c2 = $("#Checkbox1");
                        c2.css({ "display": "none" });
                        $("input[name='buyUrl']").prop("checked", false);
                        $("input[name='tburl']").prop("readonly", true);
                        $("input[name='Checkbox1']").prop("checked", false);
                        $("#tburl").val("");
                        tburl = ko.observable('');
                    }
                },
                {
                    title: '取消',
                    callback: function () {
                    }
                }
                ]);
             //         var c1 = $("#buyUrl");
             //         c1.css({ "display": "" });
             //         var c2 = $("#Checkbox1");
             //         c2.css({ "display": "none" });
             //         $("input[name='buyUrl']").prop("checked", false);
             //         $("input[name='tburl']").prop("readonly", true);
             //         $("#tburl").val("");
         }
     }
     /*************选择品类*************/
     var SelectCategroy = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         materialCategorySearch.show().then(function (value, name) {
             if (value != undefined) {
                 meatCategoryName(name);
                 var arr = value.split(',');
                 meatCategory(arr[arr.length - 2]);
             }
         });
         //        router.navigate('#brand_Add');
     };
     /****************文本编辑器*****************/
     var editorCompany;
     var memo = ko.observable();
     /*************添加功能****************/
     var AddMaterial = function (data, event) {
         var typeurl = $('#Checkbox1').is(':checked') ? 1 : 2;
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         var a = materialShelfLife();
         var b = selectedShelf();
         if (errors().length <= 0) {
             if (typeurl == 1) {
                 if ((tburl() != null && tburl() != "" && tburl() != undefined)) {
                     seltburl(false);
                 } else {
                     seltburl(true);
                     return;
                 }
             }
             else {
                 seltburl(false);
             }
             if (!(b == "长期" || b == "视存储环境") && a == "") {
                 selMaterialShelfLife(true);
             }
             else {
                 selMaterialShelfLife(false);
             }
             if (meatCategory() == "") {
                 SelCategoryMeat(true);
                 return;
             }
             else {
                 SelCategoryMeat(false);
             }
             if (files() == "") {
                 dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                 selMaterialImgInfo(true);
             }
             else {
                 selMaterialImgInfo(false);
             }
             if (!((!(b == "长期" || b == "视存储环境") && a == "") || (files() == ""))) {
                 var brand = VmBrand.SelectedId();
                 if (!VmBrand.SelectedId()) {
                     brand = 0;
                 }
                 var process = 0;
                 if (VmProcess.SelectedId()) {
                     process = VmProcess.SelectedId();
                 }
                 selmaAttribute(!VmMeatProCategory.SelectedId() > 0);
                 var sendData = {
                     materialName: materialName(),
                     materialAliasName: materialAliasName(),
                     materialBrand: brand,
                     processId: process,
                     materialSpec: materialSpec(),
                     materialShelfLife: (a + b),
                     materialPropertyInfo: JSON.stringify(property()),
                     materialMemo: editor.html(),
                     materialMaterialImgInfo: JSON.stringify(files()),
                     materialPrice: materialPrice(),
                     tburl: tburl(),
                     categoryID: meatCategory(),
                     //                     codeUser: materialCode(),
                     meatCategoryName: meatCategoryName(),
                     meatType: 1,
//                     pingjia1: pingjia1(),
//                     pingjia2: pingjia2(),
//                     pingjia3: pingjia3(),
//                     pingjia4: pingjia4(),
//                     pingjia5: pingjia5(),
//                     materialTaste: materialTaste(),
                     materialPlace: materialPlace(),
                     video: JSON.stringify(videos()),
                     materialSpecId: VmMeatSpec.SelectedId(),
                     materialjj: editorJJ.html(),
                     ntbUrl: ntbUrl(),
                     jdUrl: jdUrl(),
                     tmUrl: tmUrl(),
                     wdUrl: wdUrl(),
                     videoUrl: JSON.stringify(newVideoUrls()),
                     adFiles: JSON.stringify(AdFileUrls())
//                     nytype: nytype(),
//                     nyzhenghao: nyzhenghao()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Material/Add",
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
                                 materialName('');
                                 materialAliasName('');
                                 if (VmBrand.SelectedId()) {
                                     VmBrand.SelectedId(undefined);
                                 }
                                 materialSpec('');
                                 materialPrice('');
                                 materialShelfLife('');
                                 meatCategory('');
                                 meatCategoryName('');
                                 if (VmProcess.SelectedId()) {
                                     VmProcess.SelectedId(undefined);
                                 }
                                 if (VmMeatSpec.SelectedId()) {
                                     VmMeatSpec.SelectedId(undefined);
                                 }
                                 if (VmMeatProCategory.SelectedId()) {
                                     VmMeatProCategory.SelectedId(undefined);
                                 }
                                 pingjia1('');
                                 pingjia2('');
                                 pingjia3('');
                                 pingjia4('');
                                 pingjia5('');
                                 //                                 materialCode('');
                                 materialPlace('');
                                 materialTaste('');
                                 meatCategory('');
                                 meatCategoryName('');
                                 ntbUrl('');
                                 jdUrl('');
                                 tmUrl('');
                                 wdUrl('');
                                 propertyName('');
                                 propertyValue('');
                                 newVideoUrl('');
                                 newVideoName('');
                                 newVideoUrls([]);
                                 property([]);
                                 for (var i = 0; i < property.length; i++) {
                                     property.splice(i, 1);
                                 }
                                 VmMeatSpec.SelectedId(undefined);
                                 selMaterialImgInfo(false);
                                 selmaAttribute(false);
                                 SelCategoryMeat(false);
                                 files(new Array());
                                 AdFileUrls(new Array());
                                 editor.html('');
                                 editorJJ.html('');
                                 router.navigate('#material');
                             }
                         }
                         }]);
                     }
                 })
             }
         }
         else {
             if (!(b == "长期" || b == "视存储环境") && a == "") {
                 selMaterialShelfLife(true);
             } else {
                 selMaterialShelfLife(false);
             }
             //             if (materialCode() == null || materialCode() == undefined || materialCode() == "") {
             //                 selmaterialCode(true);
             //             }
             if (typeurl == 1) {
                 if ((tburl() != null && tburl() != "" && tburl() != undefined)) {
                     seltburl(false);
                 } else {
                     seltburl(true);
                     return;
                 }
             }
             else {
                 seltburl(false);
             }
             if (files() == "") {
                 selMaterialImgInfo(true);
                 dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
             }
             else {
                 selMaterialImgInfo(false);
             }
             errors.showAllMessages();
         }
     };
     /***************************新添加内容开始****************************************************/
     var SelCategoryMeat = ko.observable(false);
     var meatCategory = ko.observableArray();
     var meatCategoryName = ko.observableArray();
     //产品产地
     var materialPlace = ko.observableArray();
     //获取产品规格
     var VmMeatSpec = {
         MeatSpecArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var getSpecModules = function () {
         var data;
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/Admin_Batch/MaterialSpecList",
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
     //获取肉类产品类别
     var VmMeatProCategory = {
         MeatProCategoryArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var getmaAttribute = function () {
         var data;
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/MaterialType/Index",
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
     var selmaAttribute = ko.observable(false);
     VmMeatProCategory.SelectedId.subscribe(function () {
         if (VmMeatProCategory.SelectedId()) {
             selmaAttribute(false);
         }
         else {
             selmaAttribute(false);
         }
     });
     //产品口味
     var materialTaste = ko.observable();
     //产品评价
     var selPj = ko.observable();
     selPj(false);
     //产品简介
     var materialjj = ko.observable();
     //产品编码
     //     var materialCode = ko.observable();
     //产品商城链接
     var ntbUrl = ko.observable();
     var tmUrl = ko.observable();
     var jdUrl = ko.observable();
     var wdUrl = ko.observable();
     /***************************新添加内容结束****************************************************/
     var vm = {
         binding: function () {
             VmBrand.MaterialBrandArray(getBrandModules());
             VmProcess.ProcessArray(getProcessModules());
             VmMeatSpec.MeatSpecArray(getSpecModules());
             VmMeatProCategory.MeatProCategoryArray(getmaAttribute());
             editor = KindEditor.create("#txtInfos", {
                 cssPath: '/lib/kindeditor/plugins/code/prettify.css',
                 uploadJson: '/lib/kindeditor/upload_json.ashx',
                 fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
                 allowFileManager: true,
                 afterCreate: function () { },
                 afterBlur: function () { this.sync(); }
             });
             editor.html(memo());
             //产品简介
             editorJJ = KindEditor.create("#jianjieInfos", {
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
             editorJJ.html(materialjj());
             //产品视频
             var video_uploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',

                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#video_upload', multiple: false },
                 auto: true,
                 //            formData: { guid: guid },
                 // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                 resize: false,
                 //切片
                 chunked: true,
                 //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
                 chunkSize: 2 * 1024 * 1024,
                 fileSingleSizeLimit: 200 * 1024 * 1024, // 限制在200M
                 threads: 1,
                 accept: {
                     title: 'Videos',
                     extensions: 'mp4',
                     mimeTypes: 'video/*'
                 }
             });
             video_uploader.on('uploadSuccess', function (file, data, response) {
                 videos.splice($.inArray(loadingvideo, videos), 1);
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     //alert(dataObj.sMsg)
                     var single = {
                         videoUrl: dataObj.Msg,
                         videoUrls: dataObj.sMsg
                     }
                     setTimeout(function () {
                         videos(new Array());
                         videos.push(single);
                     }, 100);
                 }
             });
             video_uploader.on('uploadStart', function (file, data, response) {
                 videos.push(loadingvideo);
             });

             //图片上传
             var image_uploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',

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
                 chunked: false,
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
             image_uploader.on('uploadSuccess', function (file, data, response) {
                 files.splice($.inArray(loadingImage, files), 1);
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     files.push(single);
                 }

             });
             image_uploader.on('uploadStart', function (file, data, response) {
                 files.push(loadingImage);
             });


             var adimageUploader = webuploader.create({

                 // swf文件路径
                 swf: '/lib/webuploader/Uploader.swf',

                 // 文件接收服务端。
                 server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

                 // 选择文件的按钮。可选。
                 // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                 pick: { id: '#Adimage_upload', multiple: false }, //pick: '#Adimage_upload',
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
             adimageUploader.on('uploadSuccess', function (file, data, response) {
                 AdFileUrls.splice($.inArray(loadingAdImage, AdFileUrls), 1);
                 var dataObj = data; //JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     AdFileUrls.push(single);
                 }
             });
         },
         goBack: function () {
             router.navigateBack();
         },
         materialTaste: materialTaste,
         materialjj: materialjj,
         selPj: selPj,
         ntbUrl: ntbUrl,
         tmUrl: tmUrl,
         jdUrl: jdUrl,
         wdUrl: wdUrl,
         pingjia1: pingjia1,
         pingjia2: pingjia2,
         pingjia3: pingjia3,
         pingjia4: pingjia4,
         pingjia5: pingjia5,
         VmMeatSpec: VmMeatSpec,
         materialPlace: materialPlace,
         VmMeatProCategory: VmMeatProCategory,
         SelCategoryMeat: SelCategoryMeat,
         materialSpec: materialSpec,
         materialName: materialName,
         materialAliasName: materialAliasName,
         materialPrice: materialPrice,
         VmBrand: VmBrand,
         vmProcess: VmProcess,
         materialShelfLife: materialShelfLife,
         selMaterialShelfLife: selMaterialShelfLife,
         selectedShelfs: selectedShelfs,
         selectedShelf: selectedShelf,
         property: property,
         propertyName: propertyName,
         propertyValue: propertyValue,
         //         materialCode: materialCode,
//         selmaterialCode: selmaterialCode,
         selPrototype: selPrototype,
         AddProperty: AddProperty,
         delProperty: delProperty,
         tburl: tburl,
         seltburl: seltburl,
         materialURL: materialURL,
         loadingImage: loadingImage,
         selMaterialImgInfo: selMaterialImgInfo,
         files: files,
         delImage: delImage,
         delAdImage: delAdImage,
         AdFileUrls: AdFileUrls,
         loadingAdImage: loadingAdImage,
         loadingvideo: loadingvideo,
         videos: videos,
         delVideo: delVideo,
         memo: memo,
         AddMaterial: AddMaterial,
         selmaAttribute: selmaAttribute,
         newVideoUrl: newVideoUrl,
         newVideoName: newVideoName,
         newVideoUrls: newVideoUrls,
         selNewVideoUrl: selNewVideoUrl,
         addNewVideoUrl: addNewVideoUrl,
         delNewVideoUrl: delNewVideoUrl,
         SelectCategroy: SelectCategroy,
         meatCategory: meatCategory,
         meatCategoryName: meatCategoryName,
         nytype: nytype,
         nyzhenghao: nyzhenghao
     }
     return vm;
 });