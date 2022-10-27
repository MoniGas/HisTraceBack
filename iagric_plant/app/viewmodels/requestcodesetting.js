define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jquery.querystring', './materialaddsimple', './requestcodesetting_origin', './requestcodesetting_work', './requestcodesetting_check', './requestcodesetting_report', './requestcodesetting_formula'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, qs, materialaddsimple, origin_add, work_add, check_add, report_add, requestcodesetting_formula) {
     var moduleInfo = {
         moduleID: '99000',
         parentModuleID: '10001'
     }
     var subId = 0;
     var type = 0;
     var getPath = function () {
         return "/ShowImage/ShowPreview?v=" + (new Date()).getTime() + "&subId=" + subId;
     }
     var codePath = ko.observable("/newmenu/images/noewm.png");
     /*******************************第一步初始化数据*****************************************/
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
     function firstInit() {
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/BatchPart",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     MaterialList.SelectedId(jsonResult.ObjModel.materialId)

                     BrandList.SelectedId(jsonResult.ObjModel.brandId);
                 }
             },
             error: function (e) {
             }
         });
     }
     function fourthInit() {
         $("#topPage").hide();
         $("#First").hide();
         $("#Second").hide();
         $("#Third").hide();
         $("#Fourth").show();
         selectOption.splice(0, selectOption().length);
         initTwo();
         for (var i = 0; i < showArray().length; i++) {
             if (showArray()[i].ischeck && parseInt(showArray()[i].value) < 9 && parseInt(showArray()[i].value) > 2) {
                 var single = {
                     id: showArray()[i].value,
                     name: showArray()[i].text.replace('展现', '')
                 }
                 selectOption.push(single);
             }
         }
         initDisplay(0);
         initMaterial();
         $("[name='menuLi']").removeClass("current");
         $("[name='menuLi']").each(function (index, data) {
             if (index == 0) {
                 $(data).addClass("current");
             }
             else {
                 $(data).removeClass("current");
             }
         });
         MaterialList.SelectedId(materialId());
     }
     /*******************************第一步确定*****************************************/
     var MaterialSetting = function () {
         var sendData = {
             subId: subId,
             materialId: MaterialList.SelectedId(),
             brandId: 0
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/EditMaterial",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
             },
             error: function (e) {
             }
         });
     }

     /*******************************第二步页面配置*****************************************/
     var styleArray = ko.observableArray();
     var showArray = ko.observableArray();
     var selectStyle = ko.observable();
     var materialId = ko.observable(0);
     function initTwo() {
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
                     $(".meun[value=" + jsonResult.ObjModel.styleId + "]").addClass("checked");
                     StyleModel = jsonResult.ObjModel.styleId;
                     selectStyle(jsonResult.ObjModel.styleId);
                     materialId(jsonResult.ObjModel.materialId);
                     $("#selectAll").click(function () {
                         var isCheck = $('#selectAll').is(':checked');
                         if (isCheck) {
                             $("input[type='checkbox']").prop("checked", true);
                         }
                         else {
                             $("input[type='checkbox']").prop("checked", false);
                             $("#1").prop("checked", true);
                         }
                     });

                     $("[name='allCheck']").click(function () {
                         if ($(this).attr("id") == "1") {
                             $(this).prop("checked", true);
                             return;
                         }
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

                     var chks = $("input:checked");
                     if (chks.length == 10) {
                         $("#selectAll").prop("checked", true);
                     }
                     $("#1").prop("checked", true);
                 }
             },
             error: function (e) {
             }
         });
     }

     var selectOption = ko.observableArray();
     /*******************************第二步确定*****************************************/
     var EditStyle = function () {
         selectOption.splice(0, selectOption().length);
         //         var chks = $("input:checked");
         var chks = $("input:checkbox[name='allCheck']:checked");
         var displayOption = '';
         for (var i = 0; i < chks.length; i++) {
             if (chks[i].id == 'selectAll') continue;
             displayOption += chks[i].id + ",";
             if (chks[i].id < 9 && chks[i].id > 2) {
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
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
             },
             error: function (e) {
             }
         });
     }
     /*******************************第三步页面确定*****************************************/
     /******************20180904新加模板4图片******************/
     var loadingmbImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     var selmbImgInfo = ko.observable(false);
     var mbfiles = ko.observableArray();
     var delmbImage = function (data, event) {
         var index = mbfiles.indexOf(data);
         mbfiles.splice(index, 1);
         if (mbfiles().length == 0) {
             selmbImgInfo(true);
         }
     }
     /******************20180904新加模板4图片链接******************/
     var mubanImgUrl = ko.observable();
     var selmubanImgUrl = ko.observable(false);
     var mubanIsShow = ko.observable();
     /******************20180905获取新加模板4图片链接******************/
     /**************初始化数据****************/
     var initGetmuban4 = function () {
         var sendData = {
             rid: subId
         }
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetMuBanInfo",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 mubanImgUrl(jsonResult.ObjModel.ImgLink);
//                 alert(jsonResult.ObjModel.ImgLink);
                 mubanIsShow(jsonResult.ObjModel.IsShow);
                 mbfiles(jsonResult.ObjModel.imgs);
             },
             error: function (e) {
             }
         });
     }
     var EditShow = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if ($(".meun[value=3]").hasClass("checked")) {
             if (errors().length <= 0 && (mubanImgUrl() != null && mubanImgUrl() != undefined && mubanImgUrl() != "")) {
                 if (mbfiles() == "") {
                     dialog.showMessage('请上传模板图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                     selmbImgInfo(true);
                 }
                 else {
                     selmbImgInfo(false);
                 }
                 var sendData = {
                     subId: subId,
                     StyleModel: StyleModel,
                     MuBanImg: JSON.stringify(mbfiles()),
                     ImgLink: mubanImgUrl(),
                     IsShow: $("input[name='shifou']:checked").val()
                 };
                 $.ajax({
                     type: "POST",
                     url: "/Admin_RequestCodeSetting/EditStyleMuBan",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (jsonResult.code != 0) {
                             dialog.showMessage('追溯页风格设置失败', '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                         else {
                             dialog.showMessage('追溯页风格设置成功', '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                     },
                     error: function (e) {
                     }
                 });
             }
             else {
                 if (mbfiles() == "") {
                     selmbImgInfo(true);
                     dialog.showMessage('请上传模板图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 if (mubanImgUrl() == null || mubanImgUrl() == undefined || mubanImgUrl() == "") {
                     selmubanImgUrl(true);
                 }
                 errors.showAllMessages();
             }
         }
         else {
             var sendData = {
                 subId: subId,
                 StyleModel: StyleModel
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_RequestCodeSetting/EditStyle",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code != 0) {
                         dialog.showMessage('追溯页风格设置失败', '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         dialog.showMessage('追溯页风格设置成功', '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                 },
                 error: function (e) {
                 }
             });
         }
     }
     var EditNewShow = function () {
         $("#Third").hide();
         $("#Fourth").show();
         initDisplay(0);
         initMaterial();
         $("#li3").removeClass('active');
         $("#li3").addClass('visited');
         $("#li4").addClass('active');
     }
     var PreviewCode = function () {
         initDisplay(9);
     }

     /*******************************第四步1页面配置*****************************************/
     var materialName = ko.observable();
     var materialMemo = ko.observable('');
     var productionDate = ko.observable();
     //     var productionDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     //品牌列表
     var BrandList = {
         BrandArray: ko.observableArray(),
         SelectedId: ko.observable(),
         AddMaterialSelectedId: ko.observable()
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
     var materialSpec = ko.observable('').extend({
         maxLength: { params: 25, message: "产品规格最大长度为25个字符" }
     });
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
     var selectedShelfs = [
                { "shefsid": 0, "shefsname": "长期" },
                { "shefsid": -1, "shefsname": "视存储环境" },
                { "shefsid": 1, "shefsname": "天" },
                { "shefsid": 2, "shefsname": "月" },
                { "shefsid": 3, "shefsname": "年" }
            ];
     var selectedShelf = ko.observable();
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
     });

     var ProcessList = {
         ProcessArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var selProcess = ko.observable(false);
     //获取生产流程
     var GetProcess = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Material/ProcessList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
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
     ProcessList.SelectedId.subscribe(function () {
         if (ProcessList.SelectedId()) {
             selProcess(false);
         }
         else {
             selProcess(true);
         }
     });

     var property = ko.observableArray([]);
     var propertyName = ko.observable();
     var propertyValue = ko.observable();
     var selPrototype = ko.observable(false);
     var AddProperty = function () {
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

     //     var editorMaterial;
     //     var materialMemo = ko.observable('');
     var selMaterialImgInfo = ko.observable(false);
     var files = ko.observableArray([]);
     var delImage = function (data, event) {
         var index = files.indexOf(data);
         files.splice(index, 1);
         if (files().length == 0) {
             selMaterialImgInfo(true);
         }
     }
     var loadingImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     MaterialList.SelectedId.subscribe(function () {
         initMaterial();
     });
     var initMaterial = function () {
         if (MaterialList.SelectedId()) {
             var sendData = {
                 id: MaterialList.SelectedId()
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_Material/Info",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     VmMeatSpec.SelectedId(jsonResult.ObjModel.MaterialSpcificationID);
                     BrandList.AddMaterialSelectedId(jsonResult.ObjModel.Brand_ID);
                     // materialSpec(jsonResult.ObjModel.MaterialSpecification);
                     materialPrice(jsonResult.ObjModel.price);
                     ProcessList.SelectedId(jsonResult.ObjModel.ProcessID);
                     var shelf = jsonResult.ObjModel.ShelfLife;
                     var shelfValue = "";
                     var shelfUnit = "长期";
                     if (shelf != null && shelf.indexOf(shelfUnit) < 0) {
                         shelfValue = shelf.match(/\d+/ig);
                         var s = shelf.split(shelf.match(/\d+/ig));
                         shelfUnit = s[1];
                         if (s[0] == '视存储环境') {
                             shelfUnit = s[0];
                         }
                     }
                     selectedShelf(shelfUnit);
                     materialShelfLife(shelfValue);
                     selMaterialShelfLife(false);

                     property(jsonResult.ObjModel.propertys);
                     files(jsonResult.ObjModel.imgs);
                     materialName(jsonResult.ObjModel.MaterialFullName);
                     //                     materialMemo(jsonResult.ObjModel.Memo);
                     try {
                         editor.html(jsonResult.ObjModel.Memo);
                     }
                     catch (Error) { }
                 }
             });
         }
         var sendData = {
             subId: subId
         }
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetProductionDate",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.ObjModel.StrProductionDate != null) {
                     productionDate(jsonResult.ObjModel.StrProductionDate.substring(0, 10));
                 }
                 else {
                     productionDate(null);
                 }
                 try {
                 }
                 catch (Error) { }
             }
         });
     }
     /*******************************第四步1确定*****************************************/
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
     var EditMaterial = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         var a = materialShelfLife()
         var b = selectedShelf();
         if (errors().length <= 0) {
             if (!(b == "长期" || b == "视存储环境") && (a == "" || a == null)) {
                 selMaterialShelfLife(true);
             }
             else {
                 selMaterialShelfLife(false);
             }
             if (files() == "") {
                 dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                 selMaterialImgInfo(true);
             }
             else {
                 selMaterialImgInfo(false);
             }
             if (!((!(b == "长期" || b == "视存储环境") && (a == "" || a == null)) || (files() == ""))) {
                 var brand = BrandList.AddMaterialSelectedId();
                 if (!BrandList.AddMaterialSelectedId()) {
                     brand = 0;
                 }
                 var process = 0;
                 if (ProcessList.SelectedId()) {
                     process = ProcessList.SelectedId();
                 }
                 var sendData = {
                     subId: subId,
                     materialId: MaterialList.SelectedId(),
                     brandId: brand, //BrandList.SelectedId()
                     productionDate: productionDate()
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
                             var sendData = {
                                 materialId: MaterialList.SelectedId(),
                                 materialName: $("#SelectMaterial").find("option:selected").text(),
                                 materialBrand: brand,
                                 processId: process,
                                 materialSpec: materialSpec(),
                                 materialShelfLife: (a + b),
                                 materialPropertyInfo: JSON.stringify(property()),
                                 materialMemo: editor.html(),
                                 materialMaterialImgInfo: JSON.stringify(files()),
                                 materialPrice: materialPrice(),
                                 materialType: 0,
                                 materialSpecId: VmMeatSpec.SelectedId()
                             };
                             $.ajax({
                                 type: "POST",
                                 url: "/Admin_Material/EditMaterial",
                                 contentType: "application/json;charset=utf-8", //必须有
                                 dataType: "json", //表示返回值类型，不必须
                                 data: JSON.stringify(sendData),
                                 async: false,
                                 success: function (jsonResult) {
                                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                                         return;
                                     };
                                     if (!(jsonResult.code != 0)) {
                                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                                     }
                                     else {
                                         codePath("/newmenu/images/noewm.png");
                                         codePath(getPath());
                                         if (selectOption().length == 0) {
                                             initDisplay(9);
                                         }
                                         else {
                                             initDisplay(selectOption()[0].id);
                                         }
                                     }
                                 },
                                 error: function (Error) {
                                 }
                             });
                         }
                     },
                     error: function (e) {
                     }
                 });
             }
         } else {
             if (!ProcessList.SelectedId()) {
                 selProcess(true);
             }
             if (!(b == "长期" || b == "视存储环境") && (a == "" || a == null)) {
                 selMaterialShelfLife(true)
             } else {
                 selMaterialShelfLife(false);
             }
             if (files() == "") {
                 dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                 selMaterialImgInfo(true);
             }
             else {
                 selMaterialImgInfo(false);
             }
             errors.showAllMessages();
         }
     }
     /*******************************第三步2页面配置*****************************************/
     var menuClick = function (data, event) {
         initDisplay(data.id);
     }
     function nextButton(id, data, event) {
         for (var i = 0; i < selectOption().length; i++) {
             if (selectOption()[i].id == id) {
                 if (selectOption().length == (i + 1)) {
                     initDisplay(9);
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
         //var nowdate = complementHistoryDate();
         switch (parseInt(displayOption) - 2) {
             case 1:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").show();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide();
                 SettingOriginList(GetSettingOrigin());
                 break;
             case 2:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").show();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide();
                 SettingWorkList(GetSettingWork());
                 break;
             case 3:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").show();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide();
                 SettingCheckList(GetSettingCheck());
                 break;
             case 4:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").show();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide();
                 SettingReportList(GetSettingReport());
                 break;
             case 5:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#LogisticsInfo").hide();
                 $("#AmbientInfo").show();
                 //                 initAmbient();
                 $('#inWareDate').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 $('#outWareDate').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 initAmbient();
                 break;
             case 6:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").show();
                 $('#startDate').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 $('#endDate').datepicker({
                     autoclose: true,
                     todayHighlight: true,
                     language: 'cn'
                 });
                 GetLoginstics();
                 break;
             case 7:
                 $("#MaterialInfo").hide();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").show();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide();
                 break;
             default:
                 $("#MaterialInfo").show();
                 $("#OriginInfo").hide();
                 $("#WorkInfo").hide();
                 $("#CheckInfo").hide();
                 $("#ReportInfo").hide();
                 $("#PreviewCode").hide();
                 $("#AmbientInfo").hide();
                 $("#LogisticsInfo").hide(); // 加入物流信息隐藏  2019-11-08  刘晓杰
                 ProcessList.ProcessArray(GetProcess());
                 editor = KindEditor.create("#txtInfos", {
                     cssPath: '/lib/kindeditor/plugins/code/prettify.css',
                     uploadJson: '/lib/kindeditor/upload_json.ashx',
                     fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
                     allowFileManager: true,
                     items: [
						'fontname', 'fontsize', '|', 'forecolor', 'bold', 'italic', 'underline',
						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
						'insertunorderedlist', '|', 'image'],
                     afterBlur: function () { this.sync(); }
                 });
                 editor.html(materialMemo());
                 $('#productionDate').datepicker({
                     language: 'cn',
                     autoclose: true,
                     todayHighlight: true
                 });
                 try {
                     $('#image_upload').uploadify('destroy');
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
                         files.splice($.inArray(loadingImage, files), 1)
                         var dataObj = JSON.parse(data);
                         if (dataObj.code == 0) {
                             var single = {
                                 fileUrl: dataObj.Msg, //ko.observable(result[1])
                                 fileUrls: dataObj.sMsg
                             }
                             files.push(single);
                             selMaterialImgInfo(false);
                         }
                     }
                 });
                 break;
         }
         $(".validationMessage").css("display", "none");
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
     var AddOrigin = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         origin_add.show(subId, 0, null).then(function () {
             SettingOriginList(GetSettingOrigin());
         });
     }
     var GetOrigin = function () {
         requestcodesetting_formula.show(subId, MaterialList.SelectedId()).then(function () {
             SettingOriginList(GetSettingOrigin());
         });
     }
     //删除原料
     var delOrigin = function (id, data, envet) {
         var currentObj = $(event.target);
         currentObj.blur();
         var id = ko.utils.unwrapObservable(id);
         dialog.showMessage("确定删除该原材料吗？", '系统提示', [
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
                                        SettingOriginList(GetSettingOrigin());
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
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var subData = ko.utils.arrayFilter(SettingOriginList(), function (origin) {
             return origin.ID == id;
         })[0];
         origin_add.show(subId, subData.ID, subData).then(function () {
             SettingOriginList(GetSettingOrigin());
         });
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

     //添加生产数据
     var AddWork = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         work_add.show(subId, 0, null).then(function () {
             SettingWorkList(GetSettingWork());
         });
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
                                        SettingWorkList(GetSettingWork());
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
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var subData = ko.utils.arrayFilter(SettingWorkList(), function (work) {
             return work.Batch_ZuoYe_ID == id;
         })[0];
         work_add.show(subId, id, subData).then(function () {
             SettingWorkList(GetSettingWork());
         });
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

     //添加巡检数据
     var AddCheck = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         check_add.show(subId, 0, null).then(function () {
             SettingCheckList(GetSettingCheck());
         });
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
                                        SettingCheckList(GetSettingCheck());
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
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var subData = ko.utils.arrayFilter(SettingCheckList(), function (check) {
             return check.Batch_XunJian_ID == id;
         })[0];
         check_add.show(subId, subData.Batch_XunJian_ID, subData).then(function () {
             SettingCheckList(GetSettingCheck());
         });
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

     //添加检测数据
     var AddReport = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         report_add.show(subId, 0, null).then(function () {
             SettingReportList(GetSettingReport());
         });
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
                                        SettingReportList(GetSettingReport());
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
         var currentObj = $(event.target);
         currentObj.blur();
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
             return;
         };
         var subData = ko.utils.arrayFilter(SettingReportList(), function (check) {
             return check.Batch_JianYanJianYi_ID == id;
         })[0];
         report_add.show(subId, subData.Batch_JianYanJianYi_ID, subData).then(function () {
             SettingReportList(GetSettingReport());
         });
     }

     /*******************************新加的存储环境*****************************************/

     //存贮温度

     var getNowFormatDate = function () {
         var date = new Date();
         var seperator1 = "-";
         var strYear = date.getFullYear();
         var strMonth = date.getMonth() + 1;
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
     var temperature = ko.observable().extend({
         maxLength: { params: 50, message: "最多可输入50个字符！" }
     });
     //     var SelTemperature = ko.observable(false);
     //入库时间
     var inWareDate = ko.observable(getNowFormatDate());
     //出库时间
     var outWareDate = ko.observable(getNowFormatDate());
     //     var SelDate = ko.observable(false);
     //备注信息
     var ambientRemark = ko.observable();
     var initAmbient = function () {
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetAmbient",
             data: JSON.stringify(sendData),
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 var data = jsonResult.ObjModel;
                 temperature(data.Temperature);
                 if (data.StrInDate == null) {
                     inWareDate(inWareDate());
                 }
                 else {
                     inWareDate(data.StrInDate);
                 }
                 if (data.StrOutDate == null) {
                     outWareDate(outWareDate());
                 }
                 else {
                     outWareDate(data.StrOutDate);
                 }
                 //                 outWareDate(data.StrOutDate);
                 ambientRemark(data.Remark);
             }
         });
     }
     /*******************************新加的存储环境点击下一步*****************************************/
     var EditAmbient = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         isSuccess = false;
         var errors = ko.validation.group(this);
         if (errors().length <= 0) {
             var sendData = {
                 subId: subId,
                 temperature: temperature(),
                 inDate: inWareDate(),
                 outDate: outWareDate(),
                 remark: ambientRemark()
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_RequestCodeSetting/AddAmbient",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                         return;
                     };
                     if (jsonResult.code == 1) {
                         isSuccess = true;
                         $("#MaterialInfo").hide();
                         $("#OriginInfo").hide();
                         $("#WorkInfo").hide();
                         $("#CheckInfo").hide();
                         $("#ReportInfo").hide();
                         $("#PreviewCode").hide();
                         $("#AmbientInfo").hide();
                         $("[name='menuLi']").each(function (index, data) {
                             if ($(data)[0].id == 8) {
                                 $(data).addClass("current");
                             }
                             else {
                                 $(data).removeClass("current");
                             }
                         });
                         $("#LogisticsInfo").show();
                     }
                     else {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                 }
             });
         } else {
             errors.showAllMessages();
         }
     }

     /*****新加的物流信息******/

     //物流单号
     var logisticsNum = ko.observable();
     //     var SelLogisticsNum = ko.observable(false);
     //物流车辆
     var logisticsCarNum = ko.observable();
     //     var SelLogisticsCarNum = ko.observable(false);
     //起始地
     var startAddress = ko.observable();
     //     var SelStartAddress = ko.observable(false);
     //起始时间
     var startDate = ko.observable(getNowFormatDate());
     //     var SelStartDate = ko.observable(false);
     //目的地
     var endAddress = ko.observable();
     //     var SelEndAddress = ko.observable(false);
     //到达时间
     var endDate = ko.observable(getNowFormatDate());
     //     var SelEndDate = ko.observable(false);
     //车辆环境
     var carAmbient = ko.observable();
     //     var SelCarAmbient = ko.observable(false);
     //物流追溯网址
     var logisticsUrl = ko.observable().extend({
         pattern: {
             params: /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/,
             message: "请输入正确的追溯网址！"
         }
     });
     //     var SelLogisticsUrl = ko.observable(false);

     //获取物流信息
     var GetLoginstics = function () {
         //     function GetLogistics() {
         var sendData = {
             subId: subId
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GetLogistics",
             data: JSON.stringify(sendData),
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 var data = jsonResult.ObjModel;
                 logisticsNum(data.BillNum);
                 logisticsCarNum(data.CarNum);
                 startAddress(data.StartAddress);
                 startDate(data.StrStartDate);
                 endAddress(data.EndAddress);
                 endDate(data.StrEndDate);
                 carAmbient(data.CarAmbient);
                 logisticsUrl(data.Url);
             }
         });
     }

     //保存物流信息
     var SaveLogistics = function () {
         isSuccess = false;
         var errors = ko.validation.group(this);
         if (errors().length <= 0) {
             var sendData = {
                 subId: subId,
                 logisticsNum: logisticsNum(),
                 carNum: logisticsCarNum(),
                 startAddress: startAddress(),
                 startDate: startDate(),
                 endAddress: endAddress(),
                 endDate: endDate(),
                 carAmbient: carAmbient(),
                 url: logisticsUrl()
             }
             $.ajax({
                 type: "POST",
                 url: "/Admin_RequestCodeSetting/AddLogistics",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                         return;
                     };
                     if (jsonResult.code == 1) {
                         isSuccess = true;
                         $("#MaterialInfo").hide();
                         $("#OriginInfo").hide();
                         $("#WorkInfo").hide();
                         $("#CheckInfo").hide();
                         $("#ReportInfo").hide();
                         $("#LogisticsInfo").hide();
                         $("#AmbientInfo").hide();
                         $("[name='menuLi']").each(function (index, data) {
                             if ($(data)[0].id == 9) {
                                 $(data).addClass("current");
                             }
                             else {
                                 $(data).removeClass("current");
                             }
                         });
                         $("#PreviewCode").show();
                     }
                     if (type == 2 || jsonResult.code != 1) {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                 }
             });
         }
         else {
             errors.showAllMessages();
         }
     }
     /**********/
     var vmData = function (requestid) {
         var self = this;
         //申请码标识
         self.requestid = requestid;
         //主批次号
         self.batchName = ko.observable(batchName);
         //是否首次配置
         self.isFirst = ko.observable(isFirst);
         self.notFirst = ko.observable(notFirst);
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
     var StyleModel = ko.observable();
     var ulClick = function (obj) {
         $(".meun").removeClass("checked");
         StyleModel = obj.value;
         $(".meun[value=" + obj.value + "]").addClass("checked");
         if ($(".meun[value=" + obj.value + "]").val() == 3) {
             $("#muban4").show();
             initGetmuban4();
             if (mubanIsShow() == 1) {
                 $("#yes").attr("checked", "checked");
                 $("#no").attr("checked", false);
             }
             else {
                 $("#no").attr("checked", "checked");
                 $("#yes").attr("checked", false);
             }
         }
         else {
             $("#muban4").hide();
         }
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
     var TopSubPageClick = function (index, divname, data, event) {
         //         if ($("#li" + index).hasClass("visited")) {
         if (index == 3) {
             initTwo();
         }
         for (var i = 1; i < 4; i++) {
             $("#li" + i).removeClass("active");
         }
         $("#li" + index).addClass("active");
         $("#First").hide();
         $("#Second").hide();
         $("#Third").hide();
         //             $("#Fourth").hide();
         $("#" + divname).show();
         //         }
         if ($(".meun[value=3]").hasClass("checked")) {
             $("#muban4").show();
             initGetmuban4();
             if (mubanIsShow() == 1) {
                 $("#yes").attr("checked", "checked");
                 $("#no").attr("checked", false);
             }
             else {
                 $("#no").attr("checked", "checked");
                 $("#yes").attr("checked", false);
             }
         }
         else {
             $("#muban4").hide();
         }
     }
     var vm = {
         binding: function () {
             try {
                 $('#uploadmbImg').uploadify('destroy');
             } catch (Error) {
             }
             $("#uploadmbImg").uploadify({
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
                 'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
                 'fileSizeLimit': '5MB',
                 'removeTimeout': 0,
                 'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                 'onSelect': function (file) {
                     files.push(loadingImage);
                     //preview(file);
                 },
                 //返回一个错误，选择文件的时候触发
                 'onSelectError': function (file, errorCode, errorMsg) {
                     switch (errorCode) {
                         case -100:
                             alert("上传的文件数量已经超出系统限制！");
                             break;
                         case -110:
                             alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#uploadmbImg').uploadify('settings', 'fileSizeLimit') + "大小！");
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
                     mbfiles.splice($.inArray(loadingmbImage, mbfiles), 1)
                     var dataObj = JSON.parse(data);
                     if (dataObj.code == 0) {
                         var single = {
                             fileUrl: dataObj.Msg, //ko.observable(result[1])
                             fileUrls: dataObj.sMsg
                         }
                         mbfiles(new Array());
                         mbfiles.push(single);
                     }

                 }
             });
             subId = qs.querystring("subid");
             type = qs.querystring("type");
             if (subId == 0) {
                 router.navigate('#requestcodemanager');
             }
             else {
                 BrandList.BrandArray(GetBrandList());
                 MaterialList.MaterialArray(GetMaterialList());
                 if (type == "1") {
                     fourthInit();
                     codePath("/newmenu/images/noewm.png");
                     codePath(getPath());
                 }
                 else {
                     firstInit();
                 }
             }
             BrandList.BrandArray(GetBrandList());
             VmMeatSpec.MeatSpecArray(getSpecModules());
             MaterialList.MaterialArray(GetMaterialList());
             initTwo();
         },
         TopSubPageClick: TopSubPageClick,
         MaterialList: MaterialList,
         AddMaterial: AddMaterial,
         MaterialSetting: MaterialSetting,
         styleArray: styleArray,
         showArray: showArray,
         EditStyle: EditStyle,
         EditShow: EditShow,
         selectOption: selectOption,
         BrandList: BrandList,
         EditNewShow: EditNewShow,
         VmMeatSpec: VmMeatSpec,
         productionDate: productionDate,
         //         materialSpec: materialSpec,
         materialPrice: materialPrice,
         materialShelfLife: materialShelfLife,
         selectedShelfs: selectedShelfs,
         selectedShelf: selectedShelf,
         selMaterialShelfLife: selMaterialShelfLife,
         ProcessList: ProcessList,
         selProcess: selProcess,
         propertyName: propertyName,
         propertyValue: propertyValue,
         AddProperty: AddProperty,
         property: property,
         selPrototype: selPrototype,
         delProperty: delProperty,
         selMaterialImgInfo: selMaterialImgInfo,
         files: files,
         delImage: delImage,
         EditMaterial: EditMaterial,
         ulClick: ulClick,
         menuClick: menuClick,
         nextButton: nextButton,
         mouseoverFun: mouseoverFun,
         mouseoutFun: mouseoutFun,
         PreviewCode: PreviewCode,
         SettingOriginList: SettingOriginList,
         AddOrigin: AddOrigin,
         GetOrigin: GetOrigin,
         editOrigin: editOrigin,
         delOrigin: delOrigin,
         SettingWorkList: SettingWorkList,
         AddWork: AddWork,
         editWork: editWork,
         delWork: delWork,
         SettingCheckList: SettingCheckList,
         AddCheck: AddCheck,
         editCheck: editCheck,
         delCheck: delCheck,
         SettingReportList: SettingReportList,
         AddReport: AddReport,
         editReport: editReport,
         delReport: delReport,
         codePath: codePath,
         EditAmbient: EditAmbient,
         temperature: temperature,
         inWareDate: inWareDate,
         outWareDate: outWareDate,
         ambientRemark: ambientRemark,
         logisticsNum: logisticsNum,
         logisticsCarNum: logisticsCarNum,
         startAddress: startAddress,
         startDate: startDate,
         endAddress: endAddress,
         endDate: endDate,
         carAmbient: carAmbient,
         logisticsUrl: logisticsUrl,
         SaveLogistics: SaveLogistics,
         loadingmbImage: loadingmbImage,
         selmbImgInfo: selmbImgInfo,
         mbfiles: mbfiles,
         delmbImage: delmbImage,
         mubanImgUrl: mubanImgUrl,
         selmubanImgUrl: selmubanImgUrl,
         mubanIsShow: mubanIsShow,
         initGetmuban4: initGetmuban4
     }
     return vm;
 });