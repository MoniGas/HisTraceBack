define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', './materialaddsimple', 'webuploader'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, materialaddsimple, webuploader) {
     var moduleInfo = {
         moduleID: '23000',
         parentModuleID: '10001'
     }
     //20191031新加医疗器械需要
     var bzspec = ko.observable(0);
     //var shengchanPH = ko.observable();
     var shengchanPH = ko.observable('').extend({
         maxLength: { params: 15, message: "生产批号最大长度为15个字符" },
         required: {
             params: true,
             message: "请输入生产批号!"
         }
     });
     var SelshengchanPH = ko.observable(false);
     var YouXiaoDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     var ShiXiaoDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     var requestcodeID = ko.observable();
     var StyleModel = ko.observable();
     var selectDisplayOption = '';
     var VmMaterial = {
         MaterialArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     VmMaterial.SelectedId.subscribe(function () {
         if (VmMaterial.SelectedId()) {
             SelMaterial(false);
         } else {
             SelMaterial(true);
         }
     });
     //套标规格
     var SelTypeModel = {
         SelTypeArray: ko.observable(),
         SelectedId: ko.observable()
     }
     var SpecificationsModel = {
         SpecificationsArray: ko.observableArray(),
         SelectedId: ko.observable()
     }
     var SpeMaterialModel = {
         SpeMateiralArray: ko.observableArray(),
         SelectedId: ko.observable()
     }

     SpecificationsModel.SelectedId.subscribe(function () {
         if (SpecificationsModel.SelectedId()) {
             SelSpecification(false);
         }
         else {
             SelSpecification(true);
         }
     });
     SpeMaterialModel.SelectedId.subscribe(function () {
         if (SpeMaterialModel.SelectedId()) {
             SelSpeMaterial(false);
         }
         else {
             SelSpeMaterial(true);
         }
     });
     var Number = ko.observable('').extend({
         //         min: {
         //             params: 1,
         //             message: "数量最少为1！"
         //         },
         //         max: {
         //             params: 100000,
         //             message: "最多输入100000！"
         //         },
         //         digit: {
         //             params: true,
         //             message: "生成数量为整数！"
         //         },
         //         required: {
         //             params: true,
         //             message: "请输入生成数量！"
         //         }
     });
     var SelNumber = ko.observable(false);
     //20181109添加简码随机位数
     var SJCodeNum = ko.observable('').extend({
         //         min: {
         //             params: 1,
         //             message: "数量最少为1！"
         //         },
         //         max: {
         //             params: 10,
         //             message: "最大输入10！"
         //         },
         //         digit: {
         //             params: true,
         //             message: "简码随机位数为整数！"
         //         },
         //         required: {
         //             params: true,
         //             message: "请输入简码随机位数！"
         //         }
     });
     var SelSJCodeNum = ko.observable(false);
     //
     var AddDate = ko.observable(utils.dateFormat(new Date(), 'yyyy-MM-dd'));
     var shengchengDate = ko.observable(false);
     var SelMaterial = ko.observable(false);
     //获取套标规格
     var SelSpecification = ko.observable(false);
     var SelType = ko.observable(false);
     var SelBZSpecType = ko.observable(false);
     var SelSpeMaterial = ko.observable(false);
     var SelSclx = ko.observable(false);
     var scleixing = ko.observable(0);
     var TraceEnMainCode = ko.observable('');
     /******************20180904新加模板4图片******************/
     var loadingImage = {
         fileUrl: '../../images/load.gif',
         fileUrls: '../../images/load.gif'
     };
     var selImgInfo = ko.observable(false);
     var files = ko.observableArray();
     var delImage = function (data, event) {
         var index = files.indexOf(data);
         files.splice(index, 1);
         if (files().length == 0) {
             selImgInfo(true);
         }
     }
     /******************20180904新加模板4图片链接******************/
     var mubanImgUrl = ko.observable();
     var selmubanImgUrl = ko.observable(false);
     //获取企业信息
     var GetEnterpriseModel = function () {
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/Admin_EnterpriseInfo/GetEnterpriseModel",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 TraceEnMainCode(jsonResult.ObjModel.TraceEnMainCode);
             }
         })
     }
     //获取规格
     var GetSpecificationsList = function () {
         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Specification/GetSelectList",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                     return;
                 };
                 data = jsonResult.ObjList;
                 //alert(JSON.stringify(data));
             }
         });
         return data;
     }
     //获取产品规格
     var GetMaterialSpecificationsList = function () {
         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Material_Spec/GetSelectList",
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
     //获取产品
     var GetMaterialList = function () {

         var sendData = {
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/Admin_Request/SearchNameList",
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

     //ajax获取数据
     var GetData = function (pageIndex) {
     }

     //分页、搜索时更新数据源
     var UpdateData = function (list) {
         ViewModel.VmGenerateCode.ObjList(list.ObjList());
         ViewModel.VmGenerateCode.pageSize(list.pageSize());
         ViewModel.VmGenerateCode.totalCounts(list.totalCounts());
         ViewModel.VmGenerateCode.pageIndex(list.pageIndex());
     }

     //把获取的ajax数据转化为ko
     var GetDataKO = function (pageIndex) {
         var list = km.fromJS(GetData(pageIndex));
         return list;
     }
     var init = true;
     var onchangeData = function () {
         if (init == false) {
             var list = GetDataKO(1);
             UpdateData(list);
         }
         init = false;
     }
     /*******************************添加产品*****************************************/
     var addmaterial = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         materialaddsimple.show().then(function (id, brandId) {
             var list = GetMaterialList();
             ViewModel.VmMaterial.MaterialArray(list);
             ViewModel.VmMaterial.SelectedId(id);
         });
     }
     /*******************************获取企业简码4位简码*****************************************/
     var GetEntepriseMainJCode = function (data, event) {
         var currentObj = $(event.target);
         currentObj.blur();
         var sendData = {
         }
         $.ajax({
             type: "POST",
             url: "/Public/EditEnJMainCode",
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
                         SelNumber(false);
                         SelSJCodeNum(false);
                         VmMaterial.SelectedId(undefined);
                         SelMaterial(false);
                         //套标规格
                         SelType(false);
                         SelBZSpecType(false);
                         //获取企业信息
                         GetEnterpriseModel();
                         SelSpecification(false);
                         SpecificationsModel.SelectedId(undefined);
                         SpeMaterialModel.SelectedId(undefined);
                         SelTypeModel.SelectedId(undefined);
                         SpecificationsModel.SpecificationsArray(GetSpecificationsList());
                         SpeMaterialModel.SpeMateiralArray(GetMaterialSpecificationsList());
                         SelTypeModel.SelTypeArray([{ "Text": "套标产品码", "Value": 9 }, { "Text": "单品产品码", "Value": 3 }, { "Text": "农药二维码", "Value": 10}]);
                     }
                 }
                 }]);
             },
             error: function (Error) {
             }
         })
     }
     /*******************************生成码点击下一步*****************************************/
     var GenerateOne = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         var val = $('input:radio[name="CodeTypeCheck"]:checked').val();
         if (val == 1) {
             SJCodeNum(1)
             $("#SCodeLength").hide();
             SelSJCodeNum(false);
         }
         if (val == 2) {
             if (TraceEnMainCode() == null) {
                 alert("请先获取企业简码！");
                 return;
             }
         }
         //         else {
         //             $("#SCodeLength").show();
         //         }
         if (ko.utils.unwrapObservable(SelTypeModel.SelectedId) == 10) {
             SJCodeNum(1)
             SelSJCodeNum(false);
         }
         if (errors().length <= 0 && VmMaterial.SelectedId()) {
             var sendData = {
                 Material_ID: VmMaterial.SelectedId(),
                 bzSpecType: $("#bzspec").val(),
                 shengchanPH: shengchanPH()
             };
             $.ajax({
                 type: "POST",
                 url: "/Admin_Request/YanZhengPH",
                 contentType: "application/json;charset=utf-8", //必须有
                 dataType: "json", //表示返回值类型，不必须
                 data: JSON.stringify(sendData),
                 async: false,
                 success: function (jsonResult) {
                     if (jsonResult.code == "0") {
                         dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                     }
                     else {
                         $("#First").hide();
                         $("#Second").show();
                         initTwo();
                     }
                 },
                 error: function (e) {
                 }
             });
             //             $("#First").hide();
             //             $("#Second").show();
             //             initTwo();
         } else {
             if (!VmMaterial.SelectedId()) {
                 SelMaterial(true);
             }
             //             if (!SelTypeModel.SelectedId()) {
             //                 SelType(true);
             //             }
             if (bzspec() != null && bzspec() != "") {
                 SelBZSpecType(false);
             }
             if (shengchanPH() == null || shengchanPH() == "") {
                 SelshengchanPH(true);
             }
             //             if (!SpecificationsModel.SelectedId()) {
             //                 SelSpecification(true);
             //             }
             //             if (!SpeMaterialModel.SelectedId()) {
             //                 SelSpeMaterial(true);
             //             }
             //             if (scleixing() != null && scleixing() != "0") {
             //                 SelSclx(false);
             //             }
             //             if (Number() != null && Number() != "") {
             //                 SelNumber(false);
             //             }
             //             else {
             //                 SelNumber(true);
             //             }
             //             if (Number() == null || Number() == "") {
             //                 SelNumber(true);
             //             }
             //             if (SJCodeNum() != null && SJCodeNum() != "") {
             //                 SelSJCodeNum(false);
             //             }
             //             else {
             //                 SelSJCodeNum(true);
             //             }
             errors.showAllMessages();
         }
     }
     /*******************************第二步页面配置*****************************************/
     var styleArray = ko.observableArray();
     var showArray = ko.observableArray();
     var selectStyle = ko.observable();
     function initTwo() {
         var sendData = {
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GenerateTwo",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     showArray(jsonResult.ObjModel)

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
                         var chks = $("#Second input:checked");
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

                     $("#1").prop("checked", true);
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
         var chks = $("#Second input:checked");
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

         if (Number() == null || Number() == "") {
             dialog.showMessage('生成码数量不能为空！', '系统提示', [{ title: '确定', callback: function () { } }]); return;
         }
         if (!/^\d+$/.test(Number())) {
             dialog.showMessage('生成数量为整数！', '系统提示', [{ title: '确定', callback: function () { } }]); return;
         }
         if (parseInt(Number()) > 100000) {
             dialog.showMessage('生成码数量最多输入100000！', '系统提示', [{ title: '确定', callback: function () { } }]); return;
         }
         if (displayOption == '') {
             dialog.showMessage('至少选择一项展示项目！', '系统提示', [{ title: '确定', callback: function () { } }]);
             return;
         }

         var sendData = {
         };
         selectDisplayOption = displayOption;
         $("#Second").hide();
         $("#Third").show();
         initThree();
     }
     var MyFunction = function () {
         var x = document.getElementById("sjnum").value;
         document.getElementById("tishiyu").innerHTML = "你输入的是: " + x;
     }
     var ulClick = function (obj) {
         //         $("ul#ulmuban").on("click", "li", function () {      //只需要找到你点击的是哪个ul里面的就行
         $(".meun").removeClass("checked");
         StyleModel = obj.value;
         $(".meun[value=" + obj.value + "]").addClass("checked");
         if ($(".meun[value=" + obj.value + "]").val() == 3) {
             $("#muban4").show();
         }
         else {
             $("#muban4").hide();
         }
         //         });
     }
     function initThree() {
         var sendData = {
         };
         $.ajax({
             type: "POST",
             url: "/Admin_RequestCodeSetting/GenerateThree",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (jsonResult.code != 0) {
                     dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 else {
                     styleArray(jsonResult.ObjModel);
                     $(".meun[value=0]").addClass("checked");
                 }
             },
             error: function (e) {
             }
         });
     }

     /*******************************完成*****************************************/
     var Generate = function () {
         var currentObj = $(event.target);
         currentObj.blur();
         var errors = ko.validation.group(this);
         if ($(".meun[value=3]").hasClass("checked")) {
             if (errors().length <= 0 && VmMaterial.SelectedId() && (mubanImgUrl() != null && mubanImgUrl() != undefined && mubanImgUrl() != "")) {
                 if (files() == "") {
                     dialog.showMessage('请上传模板图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                     selImgInfo(true);
                 }
                 else {
                     selImgInfo(false);
                 }
                 var guigeid = 0;
                 if (SelTypeModel.SelectedId() == 9) {
                     guigeid = SpecificationsModel.SelectedId();
                 }
                 if (SelTypeModel.SelectedId() == 10) {
                     guigeid = SpeMaterialModel.SelectedId();
                 }
                 var type = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
                 var sendData = {
                     Material_ID: VmMaterial.SelectedId(),
                     RequestCount: Number(),
                     displayOption: selectDisplayOption,
                     StyleModel: StyleModel,
                     guigeId: guigeid,
                     type: 3,
                     scleixing: scleixing(),
                     requestDate: AddDate(),
                     codeOfType: $("input[name='CodeTypeCheck']:checked").val(),
                     MuBanImg: JSON.stringify(files()),
                     ImgLink: mubanImgUrl(),
                     IsShow: $("input[name='shifou']:checked").val(),
                     sCodeLength: SJCodeNum()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Request/GenerateMuBan",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                             return;
                         };
                         if (jsonResult.code == 1) {
                             Number('');
                             SelNumber(false);
                             SJCodeNum('');
                             SelSJCodeNum(false);
                             VmMaterial.SelectedId(undefined);
                             SelMaterial(false);
                             //套标规格
                             SelType(false);
                             SelBZSpecType(false);
                             SelSpecification(false);
                             SpecificationsModel.SelectedId(undefined);
                             SpeMaterialModel.SelectedId(undefined);
                             SelTypeModel.SelectedId(undefined);
                             router.navigate('#GenerateSuccess?subid=' + jsonResult.ObjModel.ID);
                             //dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                         else {
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                     }
                 })
             } else {
                 if (!VmMaterial.SelectedId()) {
                     SelMaterial(true);
                 }
                 if (files() == "") {
                     selImgInfo(true);
                     dialog.showMessage('请上传模板图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                 }
                 if (mubanImgUrl() == null || mubanImgUrl() == undefined || mubanImgUrl() == "") {
                     selmubanImgUrl(true);
                 }
                 errors.showAllMessages();
             }
         }
         else {
             if (errors().length <= 0 && VmMaterial.SelectedId()) {
                 var guigeid = 0;
                 if (SelTypeModel.SelectedId() == 9) {
                     guigeid = SpecificationsModel.SelectedId();
                 }
                 if (SelTypeModel.SelectedId() == 10) {
                     guigeid = SpeMaterialModel.SelectedId();
                 }
                 var type = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
                 var sendData = {
                     Material_ID: VmMaterial.SelectedId(),
                     RequestCount: Number(),
                     displayOption: selectDisplayOption,
                     StyleModel: StyleModel,
                     guigeId: guigeid,
                     type: 3,
                     scleixing: scleixing(),
                     requestDate: AddDate(),
                     //                codeOfType: $("input[name='CodeTypeCheck']:checked").val(),
                     codeOfType: 1,
                     sCodeLength: SJCodeNum(),
                     bzSpecType: $("#bzspec").val(),
                     shengchanPH: shengchanPH(),
                     YouXiaoDate: YouXiaoDate(),
                     ShiXiaoDate: ShiXiaoDate()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/Admin_Request/Generate",
                     contentType: "application/json;charset=utf-8", //必须有
                     dataType: "json", //表示返回值类型，不必须
                     data: JSON.stringify(sendData),
                     async: false,
                     success: function (jsonResult) {
                         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                             return;
                         };
                         if (jsonResult.code == 1) {
                             Number('');
                             SelshengchanPH(false);
                             SelNumber(false);
                             SJCodeNum('');
                             SelSJCodeNum(false);
                             VmMaterial.SelectedId(undefined);
                             SelMaterial(false);
                             //套标规格
                             SelType(false);
                             SelBZSpecType(false)
                             SelSpecification(false);
                             SpecificationsModel.SelectedId(undefined);
                             SpeMaterialModel.SelectedId(undefined);
                             SelTypeModel.SelectedId(undefined);
                             router.navigate('#GenerateSuccess?subid=' + jsonResult.ObjModel.ID);
                             //dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                         else {
                             dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                         }
                     }
                 })
             } else {
                 if (!VmMaterial.SelectedId()) {
                     SelMaterial(true);
                 }
                 errors.showAllMessages();
             }
         }
     }
     var TopSubPageClick = function (self, data, event) {
         $("#First").hide();
         $("#Second").hide();
         $("#Third").hide();
         $("#" + self).show();
     }
     var ViewModel = {
         binding: function () {
             //初初化导航状态
             loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
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
                 threads: 1,
                 accept: {
                     title: 'Images',
                     extensions: 'gif,jpg,jpeg,bmp,png',
                     mimeTypes: 'image/*'
                 }
             });
             uploader.on('uploadSuccess', function (file, data, response) {
                 files.splice($.inArray(loadingImage, files), 1)
                 var dataObj = JSON.parse(data);
                 if (dataObj.code == 0) {
                     var single = {
                         fileUrl: dataObj.Msg, //ko.observable(result[1])
                         fileUrls: dataObj.sMsg
                     }
                     files(new Array());
                     files.push(single);
                 }
             });
             SelshengchanPH(false);
             SelNumber(false);
             SelSJCodeNum(false);
             VmMaterial.SelectedId(undefined);
             SelMaterial(false);
             //套标规格
             SelType(false);
             SelBZSpecType(false)
             //获取企业信息
             GetEnterpriseModel();
             SelSpecification(false);
             SpecificationsModel.SelectedId(undefined);
             SpeMaterialModel.SelectedId(undefined);
             SelTypeModel.SelectedId(undefined);
             SpecificationsModel.SpecificationsArray(GetSpecificationsList());
             SpeMaterialModel.SpeMateiralArray(GetMaterialSpecificationsList());
             SelTypeModel.SelTypeArray([{ "Text": "套标产品码", "Value": 9 }, { "Text": "单品产品码", "Value": 3 }, { "Text": "农药二维码", "Value": 10}]);

             SelTypeModel.SelectedId.subscribe(function () {
                 var TypeIndex = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
                 if (TypeIndex == 9) {
                     SelType(false);
                     $("#DivSpecifications").css({ "display": "" });
                     $("#divTao").css({ "display": "" });
                     $("#divCodeCount").css({ "display": "none" });
                     $("#DivMaterialSpe").css({ "display": "none" });
                     $("#DivSclx").css({ "display": "none" });
                     $("#SCodeLength").css({ "display": "" });
                     $("#codetype").css({ "display": "" });
                     $("input[name='CodeTypeCheck'][value=2]").prop("checked", true);
                     $("input[name='CodeTypeCheck'][value=1]").prop("checked", false);
                 } else if (TypeIndex == 3) {
                     SelType(false);
                     $("#DivSpecifications").css({ "display": "none" });
                     $("#divTao").css({ "display": "none" });
                     $("#divCodeCount").css({ "display": "" });
                     $("#DivMaterialSpe").css({ "display": "none" });
                     $("#DivSclx").css({ "display": "none" });
                     $("#SCodeLength").css({ "display": "" });
                     $("#codetype").css({ "display": "" });
                     $("input[name='CodeTypeCheck'][value=2]").prop("checked", true);
                     $("input[name='CodeTypeCheck'][value=1]").prop("checked", false);
                 } else if (TypeIndex == 10) {
                     SelType(false);
                     $("#DivSpecifications").css({ "display": "none" });
                     $("#divTao").css({ "display": "none" });
                     $("#divCodeCount").css({ "display": "" });
                     $("#DivMaterialSpe").css({ "display": "" });
                     $("#DivSclx").css({ "display": "" });
                     $("#codetype").css({ "display": "none" });
                     $("#SCodeLength").css({ "display": "none" });
                     $("input[name='CodeTypeCheck'][value=2]").prop("checked", true);
                     $("input[name='CodeTypeCheck'][value=1]").prop("checked", false);
                 } else {
                     SelType(true);
                 }
             });
             $("#DivSpecifications").css({ "display": "none" });
             $("#DivMaterialSpe").css({ "display": "none" });
             $("#DivSclx").css({ "display": "none" });
             $('input:radio[name=CodeTypeCheck]').change(function () {
                 if (this.value == '1') {
                     //                     $("input:radio[name=CodeTypeCheck][value=1]").attr("checked", true);
                     //                     $("input:radio[name=CodeTypeCheck][value=2]").attr("checked", false);
                     $("#SCodeLength").hide();
                     SelSJCodeNum(false);

                 }
                 else if (this.value == '2') {
                     //                     $("input:radio[name=CodeTypeCheck][value=2]").attr("checked", true);
                     //                     $("input:radio[name=CodeTypeCheck][value=1]").attr("checked", false);
                     $("#SCodeLength").show();
                 }
             })
             //输入框正在输入时
             $("#sjnum").on('input', function () {
                 if (!($('#sjnum').val() == '')) {
                     $("#TiShi").show();
                     $("#s1").text($("#sjnum").val())
                     var sum = 15;
                     var b = $("#sjnum").val();
                     sum = sum + b * 1
                     $("#s2").text(sum)
                 } else {
                     $$("#TiShi").hide();
                 }
             })
             VmMaterial.MaterialArray(GetMaterialList());
             ViewModel.VmGenerateCode = GetDataKO(1);
             $('#AddDate').datepicker({
                 language: 'cn',
                 autoclose: true,
                 todayHighlight: true
             });
             $('#YouXiaoDate').datepicker({
                 language: 'cn',
                 autoclose: true,
                 todayHighlight: true
             });
             $('#ShiXiaoDate').datepicker({
                 language: 'cn',
                 autoclose: true,
                 todayHighlight: true
             });
         },
         goBack: function () {
             router.navigateBack();
         },
         VmGenerateCode: null,
         requestcodeID: requestcodeID,
         selectDisplayOption: selectDisplayOption,
         VmMaterial: VmMaterial,
         AddDate: AddDate,
         shengchengDate: shengchengDate,
         Number: Number,
         SelNumber: SelNumber,
         SJCodeNum: SJCodeNum,
         SelSJCodeNum: SelSJCodeNum,
         onchangeData: onchangeData,
         addmaterial: addmaterial,
         Generate: Generate,
         SelMaterial: SelMaterial,
         GenerateOne: GenerateOne,
         styleArray: styleArray,
         showArray: showArray,
         EditSetting: EditSetting,
         selectOption: selectOption,
         TopSubPageClick: TopSubPageClick,
         ulClick: ulClick,
         //套标规格
         StyleModel: StyleModel,
         SelTypeModel: SelTypeModel,
         SpecificationsModel: SpecificationsModel,
         SpeMaterialModel: SpeMaterialModel,
         SelSpecification: SelSpecification,
         SelSpeMaterial: SelSpeMaterial,
         scleixing: scleixing,
         SelSclx: SelSclx,
         SelType: SelType,
         SelBZSpecType: SelBZSpecType,
         TraceEnMainCode: TraceEnMainCode,
         GetEntepriseMainJCode: GetEntepriseMainJCode,
         loadingImage: loadingImage,
         selImgInfo: selImgInfo,
         files: files,
         delImage: delImage,
         mubanImgUrl: mubanImgUrl,
         selmubanImgUrl: selmubanImgUrl,
         MyFunction: MyFunction,
         shengchanPH: shengchanPH,
         SelshengchanPH: SelshengchanPH,
         YouXiaoDate: YouXiaoDate,
         ShiXiaoDate: ShiXiaoDate,
         bzspec: bzspec
     }
     return ViewModel;
 });