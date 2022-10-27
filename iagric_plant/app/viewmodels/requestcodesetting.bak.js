define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jquery.querystring', './materialaddsimple'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, qs, materialaddsimple) {
     var moduleInfo = {
         moduleID: '99000',
         parentModuleID: '10001'
     }
     var subId = 0;
     var getPath = function () {
         return "/ShowImage/ShowPreview?subId=" + subId;
     }
     function complementHistoryDate() {
         var date = new Date();
         var seperator1 = "-";
         var strYear = date.getFullYear();
         var strMonth = (date.getMonth() + 1).toString();
         var strDate = date.getDate();
         if (strMonth >= 1 && strMonth <= 9) {
             strMonth = "0" + strMonth;
         }
         if (strDate >= 0 && strDate <= 9) {
             strDate = "0" + strDate;
         }
         var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
         return currentdate;
     }

     var codePath = ko.observable("/newmenu/images/noewm.png");
     /*******************************第一步初始化数据*****************************************/
     var batchName = '';
     var remaining = '';
     var subBatchName = '';
     var isFirst = true;
     var notFirst = false;
     var datas = new Array();
     var materialId = 0;
     var brandId = 0;
     function init(requestid) {
         if (subId == '') {
             var sendData = {
                 requestId: requestid
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_RequestCodeSetting/One",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 0) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         batchName = jsonResult.ObjModel.batchName;
                         remaining = jsonResult.ObjModel.remaining;
                         subBatchName = jsonResult.ObjModel.subBatchName;
                         isFirst = jsonResult.ObjModel.isFirst;
                         notFirst = jsonResult.ObjModel.notFirst;
                         datas = jsonResult.ObjModel.liData;
                         materialId = jsonResult.ObjModel.materialId;
                         brandId = jsonResult.ObjModel.brandId;
                         MaterialList.SelectedId(materialId);
                         BrandList.SelectedId(brandId);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
         else {
             $("#First").hide();
             $("#Second").show();
             initTwo(subId);
         }
     }

     /*******************************第一步确定*****************************************/
     var AddSetting = function (requestid, settingCount, batchName) {
         var sendData = {
             requestId: requestid,
             count: settingCount,
             batchName: batchName
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/Add",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     $("#First").hide();
                     $("#Second").show();
                     subId = jsonResult.ObjModel.ID;
                     initTwo(jsonResult.ObjModel.ID);
                 }
             },
             error: function (e) {
             }
         });
     }

     /*******************************第二步页面配置*****************************************/
     var styleArray = ko.observableArray();
     var showArray = ko.observableArray();
     var selectStyle = ko.observable();
     function initTwo(subId) {
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/Two",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     showArray(jsonResult.ObjModel.liShowData);
                     styleArray(jsonResult.ObjModel.liStyleData);
                     selectStyle(jsonResult.ObjModel.styleId);

                     $("#selectAll").click(function () {
                         var isCheck = $('#selectAll').is(':checked');
                         if (isCheck) {
                             $("input[type='checkbox']").prop("checked", true);
                         }
                         else {
                             $("input[type='checkbox']").prop("checked", false);
                         }
                     });

                     $("[name='allCheck']").click(function () {
                         var chks = $("input:checked");
                         for (var i = 0; i < chks.length; i++) {
                             if (chks[i].id == 'selectAll') {
                                 chks.splice(chks[i], 1);
                                 break;
                             }
                         }
                         if (chks.length != $("input[type='checkbox']").length - 1) {
                             $("#selectAll").prop("checked", false);
                         }
                         else {
                             $("#selectAll").prop("checked", true);
                         }
                     });
                 }
             },
             error: function (e) {
             }
         });
     }

     var selectOption = ko.observableArray();
     /*******************************第二步确定*****************************************/
     var EditSetting = function () {
         selectOption.splice(0, selectOption().length);
         var chks = $("input:checked");
         var displayOption = '';
         for (var i = 0; i < chks.length; i++) {
             if (chks[i].id == 'selectAll') continue;
             displayOption += chks[i].id + ",";
             if (chks[i].id < 5 && chks[i].id != 0) {
                 var single = {
                     id: chks[i].id,
                     name: chks[i].value.replace('展现', '')
                 }
                 selectOption.push(single);
             }
         }
         if (displayOption == '') {
             dialog.showMessage('至少选择一项展示项目！', '系统提示', [{ title: '确定', callback: function () { } }]);
             return;
         }
         var sendData = {
             subId: subId,
             displayOption: displayOption
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/Edit",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     $("#Second").hide();
                     $("#Third").show();
                     initThree();
                 }
             },
             error: function (e) {
             }
         });
     }

     /*******************************第三步1页面配置*****************************************/
     function initThree() {
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/Two",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     if (jsonResult.ObjModel.materialId != undefined) {
                         MaterialList.SelectedId(jsonResult.ObjModel.materialId);
                     }
                     if (jsonResult.ObjModel.brandId != undefined) {
                         BrandList.SelectedId(jsonResult.ObjModel.brandId);
                     }
                     initDisplay(0);
                 }
             },
             error: function (e) {
             }
         });
     }
     //产品列表
     var MaterialList = {
         MaterialArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     //获取产品
     var GetMaterialList = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Request/SearchNameList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
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
     //添加产品
     var AddMaterial = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         materialaddsimple.show().then(function (id, brandId) {
             MaterialList.MaterialArray(GetMaterialList());
             MaterialList.SelectedId(id);
         });
     }
     //品牌列表
     var BrandList = {
         BrandArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     //获取品牌
     var GetBrandList = function () {
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
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }

     /*******************************第三步1确定*****************************************/
     var MaterialSetting = function () {
         var sendData = {
             subId: subId,
             materialId: MaterialList.SelectedId(),
             brandId: BrandList.SelectedId()
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/EditMaterial",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     initDisplay(selectOption()[0].id);
                     codePath(getPath());
                     $("#previewCode").show();
                 }
             },
             error: function (e) {
             }
         });
     }

     /*******************************第三步2页面配置*****************************************/
     var menuClick = function (data, event) {
         initDisplay(data.id);
     }
     function nextButton(id, data, event) {
         for (var i = 0; i < selectOption().length; i++) {
             if (selectOption()[i].id == id) {
                 if (selectOption().length == (i + 1)) {
                     //alert('完成');
                     //router.navigate('#requestcodema);
                     router.navigateBack();
                 }
                 else {
                     initDisplay(selectOption()[i + 1].id);
                 }
             }
         }
     }
     function initDisplay(displayOption) {
         $("[name='menuLi']").addClass("current");
         $("[name='menuLi']").each(function (index, data) {
             if ($(data)[0].id == displayOption) {
                 $(data).addClass("current");
             }
             else {
                 $(data).removeClass("current");
             }
         });
         var nowdate = complementHistoryDate();
         switch (parseInt(displayOption)) {
             case 1:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").show();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 CarNum('');
                 checkUser('');
                 InDate(nowdate);
                 Supplier('');
                 workUser('小明');
                 WorkTime(nowdate);
                 WorkContent('作业');
                 CheckUser('小明');
                 CheckTime(nowdate);
                 CheckContent('巡检');
                 ReportTitle('标题');
                 ReportTime(nowdate);
                 SettingOriginList(GetSettingOrigin());
                 OriginList.OriginArray(GetOriginList());
                 try {
                     $("#image_upload").uploadify('destroy');
                 }
                 catch (Error) { }
                 $("#image_upload").uploadify({
                     'debug': false, //开启调试
                     'auto': true, //是否自动上传
                     'buttonText': '',
                     'buttonImage': '',
                     'buttonClass': '',
                     'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                     'queueID': 'uploadfileQueue', //文件选择后的容器ID
                     'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
                     'width': '74',
                     'height': '74',
                     'multi': false,
                     'queueSizeLimit': 1,
                     'uploadLimit': 0,
                     'fileTypeDesc': '支持的格式：',
                     'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
                     'fileSizeLimit': '5MB',
                     'removeTimeout': 0,
                     'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                     'onSelect': function (file) {
                         files.push(loadingImage);
                         //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + files().length + "个文件！");
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
                     //检测FLASH失败调用
                     'onFallback': function () {
                         alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
                     },
                     //上传到服务器，服务器返回相应信息到data里
                     'onUploadSuccess': function (file, data, response) {
                         files.splice($.inArray(loadingImage, files), 1);
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 fileUrl: dataObj.Msg,
                                 fileUrls: dataObj.sMsg
                             }
                             files.push(single);
                             hasfiles(false);
                         }
                     }
                 });
                 $('#txtInDate').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 break;
             case 2:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").show();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 CarNum('冀A12345');
                 checkUser('小明');
                 InDate(nowdate);
                 Supplier('中国');
                 workUser('');
                 WorkTime(nowdate);
                 WorkContent('');
                 CheckUser('小明');
                 CheckTime(nowdate);
                 CheckContent('巡检');
                 ReportTitle('标题');
                 ReportTime(nowdate);
                 SettingWorkList(GetSettingWork());
                 try {
                     $("#A1").uploadify('destroy');
                 }
                 catch (Error) { }
                 $("#A1").uploadify({
                     'debug': false, //开启调试
                     'auto': true, //是否自动上传
                     'buttonText': '',
                     'buttonImage': '',
                     'buttonClass': '',
                     'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                     'queueID': 'uploadfileQueue', //文件选择后的容器ID
                     'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
                     'width': '74',
                     'height': '74',
                     'multi': false,
                     'queueSizeLimit': 1,
                     'uploadLimit': 0,
                     'fileTypeDesc': '支持的格式：',
                     'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
                     'fileSizeLimit': '5MB',
                     'removeTimeout': 0,
                     'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                     'onSelect': function (file) {
                         WorkImgs.push(loadingImage);
                         //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + WorkImgs().length + "个文件！");
                                 break;
                             case -110:
                                 alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A1').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                         WorkImgs.splice($.inArray(loadingImage, WorkImgs), 1);
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 fileUrl: dataObj.Msg,
                                 fileUrls: dataObj.sMsg
                             }
                             WorkImgs.push(single);
                         }
                     }
                 });
                 try {
                     $("#video_upload").uploadify('destroy');
                 }
                 catch (Error) { }
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
                         WorkVideos.push(loadingImage);
                         //preview(file);
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + WorkVideos().length + "个文件！");
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
                         WorkVideos.splice($.inArray(loadingImage, WorkVideos), 1)
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 videoUrl: dataObj.Msg,
                                 videoUrls: dataObj.sMsg
                             }
                             setTimeout(function () {
                                 WorkVideos.push(single);
                             }, 100);
                         }

                     }
                 });
                 $('#txtWorkTime').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 break;
             case 3:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").show();
                 $("#ReportInfo").hide();
                 CarNum('冀A12345');
                 checkUser('小明');
                 InDate(nowdate);
                 Supplier('中国');
                 workUser('小明');
                 WorkTime(nowdate);
                 WorkContent('巡检');
                 CheckUser('');
                 CheckTime(nowdate);
                 CheckContent('');
                 ReportTitle('标题');
                 ReportTime(nowdate);
                 SettingCheckList(GetSettingCheck());
                 try {
                     $("#A2").uploadify('destroy');
                 }
                 catch (Error) { }
                 $("#A2").uploadify({
                     'debug': false, //开启调试
                     'auto': true, //是否自动上传
                     'buttonText': '',
                     'buttonImage': '',
                     'buttonClass': '',
                     'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                     'queueID': 'uploadfileQueue', //文件选择后的容器ID
                     'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
                     'width': '74',
                     'height': '74',
                     'multi': false,
                     'queueSizeLimit': 1,
                     'uploadLimit': 0,
                     'fileTypeDesc': '支持的格式：',
                     'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
                     'fileSizeLimit': '5MB',
                     'removeTimeout': 0,
                     'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                     'onSelect': function (file) {
                         CheckImgs.push(loadingImage);
                         //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + CheckImgs().length + "个文件！");
                                 break;
                             case -110:
                                 alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A2').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                         CheckImgs.splice($.inArray(loadingImage, CheckImgs), 1);
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 fileUrl: dataObj.Msg,
                                 fileUrls: dataObj.sMsg
                             }
                             CheckImgs.push(single);
                         }
                     }
                 });
                 try {
                     $("#A3").uploadify('destroy');
                 }
                 catch (Error) { }
                 $("#A3").uploadify({
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
                         CheckVideos.push(loadingImage);
                         //preview(file);
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + CheckVideos().length + "个文件！");
                                 break;
                             case -110:
                                 alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A3').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                         CheckVideos.splice($.inArray(loadingImage, CheckVideos), 1)
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 videoUrl: dataObj.Msg,
                                 videoUrls: dataObj.sMsg
                             }
                             setTimeout(function () {
                                 CheckVideos.push(single);
                             }, 100);
                         }

                     }
                 });
                 $('#txtCheckTime').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 break;
             case 4:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").show();
                 CarNum('冀A12345');
                 checkUser('小明');
                 InDate(nowdate);
                 Supplier('中国');
                 workUser('小明');
                 WorkTime(nowdate);
                 WorkContent('巡检');
                 CheckUser('小明');
                 CheckTime(nowdate);
                 CheckContent('巡检');
                 ReportTitle('');
                 ReportTime(nowdate);
                 SettingReportList(GetSettingReport());
                 try {
                     $("#A4").uploadify('destroy');
                 }
                 catch (Error) { }
                 $("#A4").uploadify({
                     'debug': false, //开启调试
                     'auto': true, //是否自动上传
                     'buttonText': '',
                     'buttonImage': '',
                     'buttonClass': '',
                     'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                     'queueID': 'uploadfileQueue', //文件选择后的容器ID
                     'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
                     'width': '74',
                     'height': '74',
                     'multi': false,
                     'queueSizeLimit': 1,
                     'uploadLimit': 0,
                     'fileTypeDesc': '支持的格式：',
                     'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
                     'fileSizeLimit': '5MB',
                     'removeTimeout': 0,
                     'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                     'onSelect': function (file) {
                         ReportImgs.push(loadingImage);
                         //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
                     },
                     //返回一个错误，选择文件的时候触发
                     'onSelectError': function (file, errorCode, errorMsg) {
                         switch (errorCode) {
                             case -100:
                                 alert("上传的文件数量已经超出系统限制的" + ReportImgs().length + "个文件！");
                                 break;
                             case -110:
                                 alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A4').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                         ReportImgs.splice($.inArray(loadingImage, ReportImgs), 1);
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 fileUrl: dataObj.Msg,
                                 fileUrls: dataObj.sMsg
                             }
                             ReportImgs.push(single);
                             hasReportImgs(false);
                         }
                     }
                 });
                 $('#txtReportTime').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 break;
             default:
                 $("#MaterialInfo").show();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 break;
         }
         $(".validationMessage").css("display", "none");
         hasfiles(false);
         selOriginList(false);
         selWorkList(false);
         hasReportImgs(false);

     }
     //原料图片数组
     var files = ko.observableArray();
     var hasfiles = ko.observable(false);

     var loadingImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     //删除图片
     delImage = function (data, event) {
         var index = files.indexOf(data);
         files.splice(index, 1);
         if (files().length == 0) {
             //vmObj.selMaterialImgInfo(true);
         }
     }

     //原料列表
     var OriginList = {
         OriginArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var selOriginList = ko.observable(false);
     OriginList.SelectedId.subscribe(function () {
         selOriginList(OriginList.SelectedId() == undefined)
     });
     //获取原料
     var GetOriginList = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/AdminOrigin/GetOriginList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.ObjList;
             }
         });
         return data;
     }

     var SettingOriginList = ko.observableArray();
     //获取原料信息
     function GetSettingOrigin() {
         var data;
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetOriginList",
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
     //添加原料

     //ID
     var requestOriginId = ko.observable(0);
     //来源
     var Supplier = ko.observable().extend({
         required: {
             params: true,
             message: "请输入来源！"
         }
     });
     //入库时间
     var InDate = ko.observable().extend({
         required: {
             params: true,
             message: "请选择入库日期！"
         }
     });
     //运输车辆
     var CarNum = ko.observable().extend({
         required: {
             params: true,
             message: "请输入运输车辆！"
         },
         pattern: {
             params: /^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{5}$/,
             message: "请输入正确的车牌号！"
         }
     });
     //检验员
     var checkUser = ko.observable().extend({
         required: {
             params: true,
             message: "请输入检验员！"
         }
     });
     //添加原料
     var AddOrigin = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if (errors().length <= 0 && OriginList.SelectedId() != undefined && files().length > 0) {
             selOriginList(false);
             hasfiles(false);
             var sendData;
             var Action = "AddOrigin";
             if (requestOriginId() == 0) {
                 sendData = {
                     subId: subId,
                     originId: OriginList.SelectedId(),
                     carNum: CarNum(),
                     checkUser: checkUser(),
                     inDate: InDate(),
                     supplie: Supplier(),
                     img: JSON.stringify(files())
                 };
             }
             else {
                 sendData = {
                     id: requestOriginId(),
                     subId: subId,
                     originId: OriginList.SelectedId(),
                     carNum: CarNum(),
                     checkUser: checkUser(),
                     inDate: InDate(),
                     supplie: Supplier(),
                     img: JSON.stringify(files()).replace('[[', '[').replace(']]', ']')
                 };
                 Action = "EditOrigin";
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_RequestCodeSetting/" + Action,
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 1) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         requestOriginId(0);
                         OriginList.SelectedId(undefined);
                         CarNum('');
                         checkUser('');
                         InDate(complementHistoryDate());
                         Supplier('');
                         files.splice(0, files().length);
                         SettingOriginList(GetSettingOrigin());
                         $(".validationMessage").css("display", "none");
                         hasfiles(false);
                         selOriginList(false);
                         selWorkList(false);
                         hasReportImgs(false);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
         else {
             if (OriginList.SelectedId() == undefined) {
                 selOriginList(true);
             }
             if (files().length <= 0) {
                 hasfiles(true);
             }
             $(".validationMessage").css("display", "");
             errors.showAllMessages();
         }
     }
     //删除原料
     var delOrigin = function (id, data, envet) {
         var currentObj = $(event.target);
         currentObj.blur();
         var id = ko.utils.unwrapObservable(id);
         dialog.showMessage("确定删除该原料吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_RequestCodeSetting/DelOrigin",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code == "1") {
                                        requestOriginId(0);
                                        OriginList.SelectedId(undefined);
                                        CarNum('');
                                        checkUser('');
                                        InDate(complementHistoryDate());
                                        Supplier('');
                                        files.splice(0, files().length);
                                        SettingOriginList(GetSettingOrigin());
                                        $(".validationMessage").css("display", "none");
                                        hasfiles(false);
                                        selOriginList(false);
                                        selWorkList(false);
                                        hasReportImgs(false);
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
        ]);
     }
     //修改原料
     var editOrigin = function (id, data, event) {
         var subData = ko.utils.arrayFilter(SettingOriginList(), function (origin) {
             return origin.ID == id;
         })[0];
         OriginList.SelectedId(subData.OriginID);
         CarNum(subData.CarNum);
         checkUser(subData.CheckUser);
         InDate(subData.StrInDate.replace('年', '-').replace('月', '-').replace('日', ''));
         Supplier(subData.Supplier);
         files.splice(0, files().length);
         files(subData.liImg);
         requestOriginId(subData.ID);
     }

     /*******************************第三步2页面配置*****************************************/
     var SettingWorkList = ko.observableArray();
     //获取作业信息
     function GetSettingWork() {
         var data;
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetWorkList",
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
     //作业ID
     var WorkId = ko.observable(0);
     //作业类型
     var Worktype = ko.observable();
     //作业
     var WorkList = {
         WorkArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var selWorkList = ko.observable(false);
     WorkList.SelectedId.subscribe(function () {
         selWorkList(WorkList.SelectedId() == undefined)
     });
     Worktype.subscribe(function () {
         var defaultItem = { OperationTypeName: '暂无相应生产环节', Batch_ZuoYeType_ID: '-1' };
         if (!Worktype()) {
             WorkList.WorkArray(defaultItem);
             return;
         }
         var sendData = {
             selecttype: Worktype()
         };
         $.ajax({
             type: "POST",
             url: "/Admin_Batch_ZuoYe/OpTypeList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             data: JSON.stringify(sendData),
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 WorkList.WorkArray(jsonResult.ObjList);
             }
         });
     });
     //操作人
     var workUser = ko.observable().extend({
         required: {
             params: true,
             message: "请输入操作人员！"
         }
     });
     //操作时间
     var WorkTime = ko.observable().extend({
         required: {
             params: true,
             message: "请输入操作时间！"
         }
     });
     //作业内容
     var WorkContent = ko.observable().extend({
         required: {
             params: true,
             message: "请输入作业描述！"
         }
     });
     //作业图片数组
     var WorkImgs = ko.observableArray();
     //删除作业图片
     delWorkImage = function (data, event) {
         var index = WorkImgs.indexOf(data);
         WorkImgs.splice(index, 1);
     }
     //作业视频数组
     var WorkVideos = ko.observableArray();
     //删除作业视频
     delWorkVideo = function (data, event) {
         var index = WorkVideos.indexOf(data);
         WorkVideos.splice(index, 1);
     }

     //添加生产数据
     var AddWork = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if (errors().length <= 0 && WorkList.SelectedId() != undefined) {
             selWorkList(false);
             var sendData;
             var Action = "AddWork";
             if (WorkId() == 0) {
                 var batch_ZuoYeType = 0;
                 if (WorkList.SelectedId()) {
                     batch_ZuoYeType = WorkList.SelectedId();
                 }
                 var sendData = {
                     settingid: subId,
                     type: Worktype(),
                     batch_ZuoYeType_ID: batch_ZuoYeType,
                     userName: workUser(),
                     addDate: WorkTime(),
                     content: WorkContent(),
                     files: JSON.stringify(WorkImgs()),
                     video: JSON.stringify(WorkVideos())
                 }
             }
             else {
                 var batch_ZuoYeType = 0;
                 if (WorkList.SelectedId()) {
                     batch_ZuoYeType = WorkList.SelectedId();
                 }
                 var sendData = {
                     id: WorkId(),
                     type: Worktype(),
                     batch_ZuoYeType_ID: batch_ZuoYeType,
                     userName: workUser(),
                     addDate: WorkTime(),
                     content: WorkContent(),
                     files: JSON.stringify(WorkImgs()).replace('[[', '[').replace(']]', ']'),
                     video: JSON.stringify(WorkVideos()).replace('[[', '[').replace(']]', ']')
                 }
                 Action = "EditWork";
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch_ZuoYe/" + Action,
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 1) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         WorkId(0);
                         Worktype(0);
                         WorkList.SelectedId(undefined);
                         workUser('');
                         WorkTime(complementHistoryDate());
                         WorkContent('');
                         WorkImgs.splice(0, WorkImgs().length);
                         WorkVideos.splice(0, WorkVideos().length);
                         SettingWorkList(GetSettingWork());
                         $(".validationMessage").css("display", "none");
                         hasfiles(false);
                         selOriginList(false);
                         selWorkList(false);
                         hasReportImgs(false);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
         else {
             if (WorkList.SelectedId() == undefined) {
                 selWorkList(true);
             }
             $(".validationMessage").css("display", "");
             errors.showAllMessages();
         }
     }
     //删除生产数据
     var delWork = function (id, data, envet) {
         var currentObj = $(event.target);
         currentObj.blur();
         var id = ko.utils.unwrapObservable(id);
         dialog.showMessage("确定删除该生产信息吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Batch_ZuoYe/Delete",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code == "1") {
                                        WorkId(0);
                                        Worktype(0);
                                        WorkList.SelectedId(undefined);
                                        workUser('');
                                        WorkTime(complementHistoryDate());
                                        WorkContent('');
                                        WorkImgs.splice(0, WorkImgs().length);
                                        WorkVideos.splice(0, WorkVideos().length);
                                        SettingWorkList(GetSettingWork());
                                        $(".validationMessage").css("display", "none");
                                        hasfiles(false);
                                        selOriginList(false);
                                        selWorkList(false);
                                        hasReportImgs(false);
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
        ]);
     }
     //修改生产数据
     var editWork = function (id, data, event) {
         var subData = ko.utils.arrayFilter(SettingWorkList(), function (work) {
             return work.Batch_ZuoYe_ID == id;
         })[0];
         WorkId(subData.Batch_ZuoYe_ID);
         Worktype(subData.type);
         WorkList.SelectedId(subData.zuoye_typeId);
         workUser(subData.UserName);
         WorkTime(subData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
         WorkContent(subData.Content);
         WorkImgs(subData.imgs);
         WorkVideos(subData.videos);
     }

     /*******************************第三步3页面配置*****************************************/
     var SettingCheckList = ko.observableArray();
     //获取巡检信息
     function GetSettingCheck() {
         var data;
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetCheckList",
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
     //巡检ID
     var CheckId = ko.observable(0);
     //操作人
     var CheckUser = ko.observable().extend({
         required: {
             params: true,
             message: "请输入操作人员！"
         }
     });
     //操作时间
     var CheckTime = ko.observable().extend({
         required: {
             params: true,
             message: "请输入操作时间！"
         }
     });
     //巡检内容
     var CheckContent = ko.observable().extend({
         required: {
             params: true,
             message: "请输入巡检描述！"
         }
     });
     //巡检图片数组
     var CheckImgs = ko.observableArray();
     //删除巡检图片
     delCheckImage = function (data, event) {
         var index = CheckImgs.indexOf(data);
         CheckImgs.splice(index, 1);
     }
     //巡检视频数组
     var CheckVideos = ko.observableArray();
     //删除巡检视频
     delCheckVideo = function (data, event) {
         var index = CheckVideos.indexOf(data);
         CheckVideos.splice(index, 1);
     }

     //添加巡检数据
     var AddCheck = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if (errors().length <= 0) {
             var sendData;
             var Action = "AddCheck";
             if (CheckId() == 0) {
                 var sendData = {
                     settingId: subId,
                     userName: CheckUser(),
                     addDate: CheckTime(),
                     content: CheckContent(),
                     files: JSON.stringify(CheckImgs()),
                     video: JSON.stringify(CheckVideos())
                 }
             }
             else {
                 var sendData = {
                     id: CheckId(),
                     userName: CheckUser(),
                     addDate: CheckTime(),
                     content: CheckContent(),
                     files: JSON.stringify(CheckImgs()).replace('[[', '[').replace(']]', ']'),
                     video: JSON.stringify(CheckVideos()).replace('[[', '[').replace(']]', ']')
                 }
                 Action = "EditCheck";
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch_XunJian/" + Action,
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 1) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         CheckId(0);
                         CheckUser('');
                         CheckTime(complementHistoryDate());
                         CheckContent('');
                         CheckImgs.splice(0, CheckImgs().length);
                         CheckVideos.splice(0, CheckVideos().length);
                         SettingCheckList(GetSettingCheck());
                         $(".validationMessage").css("display", "none");
                         hasfiles(false);
                         selOriginList(false);
                         selWorkList(false);
                         hasReportImgs(false);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
         else {
             $(".validationMessage").css("display", "");
             errors.showAllMessages();
         }
     }
     //删除巡检数据
     var delCheck = function (id, data, envet) {
         var currentObj = $(event.target);
         currentObj.blur();
         var id = ko.utils.unwrapObservable(id);
         dialog.showMessage("确定删除该巡检信息吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Batch_XunJian/Del",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code == "1") {
                                        CheckId(0);
                                        CheckUser('');
                                        CheckTime(complementHistoryDate());
                                        CheckContent('');
                                        CheckImgs.splice(0, CheckImgs().length);
                                        CheckVideos.splice(0, CheckVideos().length);
                                        SettingCheckList(GetSettingCheck());
                                        $(".validationMessage").css("display", "none");
                                        hasfiles(false);
                                        selOriginList(false);
                                        selWorkList(false);
                                        hasReportImgs(false);
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
        ]);
     }
     //修改巡检数据
     var editCheck = function (id, data, event) {
         var subData = ko.utils.arrayFilter(SettingCheckList(), function (check) {
             return check.Batch_XunJian_ID == id;
         })[0];
         CheckId(subData.Batch_XunJian_ID);
         CheckUser(subData.UserName);
         CheckTime(subData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
         CheckContent(subData.Content);
         CheckImgs(subData.imgs);
         CheckVideos(subData.videos);
     }

     /*******************************第三步4页面配置*****************************************/
     var SettingReportList = ko.observableArray();
     //获取质检信息
     function GetSettingReport() {
         var data;
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetReportList",
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
     //质检ID
     var ReportId = ko.observable(0);
     //质检标题
     var ReportTitle = ko.observable().extend({
         required: {
             params: true,
             message: "请输入检测标题！"
         }
     });
     //检测日期
     var ReportTime = ko.observable().extend({
         required: {
             params: true,
             message: "请输入检测日期！"
         }
     });
     //检测图片数组
     var ReportImgs = ko.observableArray();
     var hasReportImgs = ko.observable(false);
     //删除检测图片
     delReportImage = function (data, event) {
         var index = ReportImgs.indexOf(data);
         ReportImgs.splice(index, 1);
         if (ReportImgs().length == 0) {
             hasReportImgs(true);
         }
     }

     //添加检测数据
     var AddReport = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if (errors().length <= 0 && ReportImgs().length > 0) {
             hasReportImgs(false);
             var sendData;
             var Action = "AddReport";
             if (ReportId() == 0) {
                 var sendData = {
                     settingId: subId,
                     content: ReportTitle(),
                     addDate: ReportTime(),
                     files: JSON.stringify(ReportImgs())
                 }
             }
             else {
                 var sendData = {
                     id: ReportId(),
                     content: ReportTitle(),
                     addDate: ReportTime(),
                     files: JSON.stringify(ReportImgs()).replace('[[', '[').replace(']]', ']')
                 }
                 Action = "EditReport";
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Batch_JianYanJianYi/" + Action,
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 1) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         ReportId(0);
                         ReportTitle('');
                         ReportTime(complementHistoryDate());
                         ReportImgs.splice(0, ReportImgs().length);
                         SettingReportList(GetSettingReport());
                         $(".validationMessage").css("display", "none");
                         hasfiles(false);
                         selOriginList(false);
                         selWorkList(false);
                         hasReportImgs(false);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
         else {
             $(".validationMessage").css("display", "");
             if (ReportImgs().length <= 0) {
                 hasReportImgs(true);
             }
             errors.showAllMessages();
         }
     }
     //删除检测数据
     var delReport = function (id, data, envet) {
         var currentObj = $(event.target);
         currentObj.blur();
         var id = ko.utils.unwrapObservable(id);
         dialog.showMessage("确定删除该检测信息吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Batch_JianYanJianYi/Delete",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                return;
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                    if (jsonResult.code == "1") {
                                        ReportId(0);
                                        ReportTitle('');
                                        ReportTime(complementHistoryDate());
                                        ReportImgs.splice(0, ReportImgs().length);
                                        SettingReportList(GetSettingReport());
                                        $(".validationMessage").css("display", "none");
                                        hasfiles(false);
                                        selOriginList(false);
                                        selWorkList(false);
                                        hasReportImgs(false);
                                    }
                                }
                                }]);
                            }
                        }
                    });
                }
            },
            {
                title: '取消',
                callback: function () {
                }
            }
        ]);
     }
     //修改检测数据
     var editReport = function (id, data, event) {
         var subData = ko.utils.arrayFilter(SettingReportList(), function (check) {
             return check.Batch_JianYanJianYi_ID == id;
         })[0];
         ReportId(subData.Batch_JianYanJianYi_ID);
         ReportTitle(subData.Content);
         ReportTime(subData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
         ReportImgs(subData.imgs);
     }


     var vmData = function (requestid) {
         var self = this;
         //申请码标识
         self.requestid = requestid;
         //主批次号
         self.batchName = ko.observable(batchName);
         //是否首次配置
         self.isFirst = ko.observable(isFirst);
         self.notFirst = ko.observable(notFirst);
         //数据列表
         self.datas = ko.observableArray(datas);
         //剩余配置数量
         self.remaining = ko.observable(remaining);
         //子批次号
         self.subBatchName = ko.observable(subBatchName).extend({
             required: {
                 params: true,
                 message: "请填写批次号！"
             }
         });
         //本次配置数量
         self.settingCount = ko.observable(remaining).extend({
             min: {
                 params: 1,
                 message: "数量最少为1！"
             },
             max: {
                 params: self.remaining(),
                 message: "最多输入" + self.remaining() + "！"
             },
             digit: {
                 params: true,
                 message: "生成数量为整数！"
             },
             required: {
                 params: true,
                 message: "请填写配置数量！"
             }
         });
         self.firstConfirm = function (type, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 if (type == 1) {
                     dialog.showMessage('您确定要将全部' + remaining + '个码统一配置为' + batchName + '批次吗？', '系统提示', [
                        { title: '确定', callback: function () { self.settingCount(remaining); AddSetting(self.requestid, self.settingCount(), batchName); } },
                        { title: '取消', callback: function () { } }
                     ]);
                 }
                 else {
                     AddSetting(self.requestid, self.settingCount(), self.subBatchName());
                 }
             }
             else {
                 self.errors.showAllMessages();
             }
         }

         //显示
         self.displayArray = ko.observableArray();
         //展示
         self.showArray = ko.observableArray();
     }
     var mouseoverFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var ShowAll = self.find("div[eleflag='ShowAll']");

         ShowAll.css({ "display": "" });
     }
     var mouseoutFun = function (data, event) {
         var self = $(event.target).closest('tr');
         var ShowAll = self.find("div[eleflag='ShowAll']");

         ShowAll.css({ "display": "none" });
     }
     var vm = {
         binding: function () {
             var requestid = qs.querystring("requestid");
             if (requestid == "") {
                 requestid = qs.querystring("requestcodeid");
             }
             subId = qs.querystring("subid");
             if (requestid == "" && subId == 0) {
                 router.navigate('#requestcodema');
             }
             else {
                 init(requestid);
                 vm.vmData = new vmData(requestid);
                 MaterialList.MaterialArray(GetMaterialList());
                 BrandList.BrandArray(GetBrandList());
             }
         },
         vmData: null,
         styleArray: styleArray,
         showArray: showArray,
         EditSetting: EditSetting,
         selectOption: selectOption,
         MaterialList: MaterialList,
         AddMaterial: AddMaterial,
         BrandList: BrandList,
         MaterialSetting: MaterialSetting,
         initThree: initThree,
         menuClick: menuClick,
         nextButton: nextButton,
         OriginList: OriginList,
         SettingOriginList: SettingOriginList,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         files: files,
         delImage: delImage,
         Supplier: Supplier,
         InDate: InDate,
         CarNum: CarNum,
         checkUser: checkUser,
         AddOrigin: AddOrigin,
         selOriginList: selOriginList,
         hasfiles: hasfiles,
         editOrigin: editOrigin,
         delOrigin: delOrigin,
         Worktype: Worktype,
         WorkList: WorkList,
         selWorkList: selWorkList,
         workUser: workUser,
         WorkTime: WorkTime,
         WorkContent: WorkContent,
         WorkImgs: WorkImgs,
         delWorkImage: delWorkImage,
         WorkVideos: WorkVideos,
         delWorkVideo: delWorkVideo,
         SettingWorkList: SettingWorkList,
         AddWork: AddWork,
         editWork: editWork,
         delWork: delWork,
         CheckUser: CheckUser,
         CheckTime: CheckTime,
         CheckContent: CheckContent,
         CheckImgs: CheckImgs,
         delCheckImage: delCheckImage,
         CheckVideos: CheckVideos,
         delCheckVideo: delCheckVideo,
         SettingCheckList: SettingCheckList,
         AddCheck: AddCheck,
         editCheck: editCheck,
         delCheck: delCheck,
         ReportTitle: ReportTitle,
         ReportTime: ReportTime,
         hasReportImgs: hasReportImgs,
         ReportImgs: ReportImgs,
         delReportImage: delReportImage,
         AddReport: AddReport,
         SettingReportList: SettingReportList,
         editReport: editReport,
         delReport: delReport,
         codePath: codePath
     }
     return vm;
 });