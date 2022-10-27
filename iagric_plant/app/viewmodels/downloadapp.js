define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'jquery.querystring', 'logininfo', './material_add'],
 function (router, ko, $, bdp, kv, utils, qs, loginInfo, material_add) {
     var parentCode;
     var moduleInfo = {
         moduleID: '26000',
         parentModuleID: '10002'
     }
     var getUrl = function () {
         var data;
         $.ajax({
             type: "POST",
             url: "/Public/GetAppUrl",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             success: function (jsonResult) {
                 data = jsonResult.fileUrl;
             }
         });
         return data;
     }
     var vmdownloadapp = function (id) {
         var self = this;
         self.id = id;

         self.url = '/ShowImage/ShowImgNoUrl?ewm=' + getUrl();
     }
     var vm = {
         binding: function () {
             parentCode = qs.querystring("pc");
             if (parentCode == undefined || parentCode == '') {
                 parentCode = moduleInfo.parentModuleID;
             }
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             id = qs.querystring("id");
             vm.vmdownloadapp = new vmdownloadapp(id);
         },
        goBack: function () {
            router.navigateBack();
        },
         vmdownloadapp: null
     }
     return vm;
 });