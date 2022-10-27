define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'jquery.querystring', 'logininfo'],
 function (router, ko, $, bdp, kv, utils, qs, loginInfo) {
     var parentCode;
     var moduleInfo = {
         moduleID: '14004',
         parentModuleID: '10001'
     }
     var getUrl = function (id) {
         var data;
         var sendData = {
             ewm: id
         }
         $.ajax({
             type: "POST",
             url: "/Public/GetPreviewUrl",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             async: false,
             data: JSON.stringify(sendData),
             success: function (jsonResult) {
                 data = jsonResult.fileUrl;
             }
         });
         return data;
     }
     var vmguide = function (id) {
         var self = this;
         self.id = id;
         self.url = '/ShowImage/ShowImgNoUrl?ewm=' + getUrl(self.id);
     }
     /*******************************添加产品*****************************************/
     var back = function (data, event) {
         //         router.navigate('#guide4?id=' + self.id+'&pc='+parentCode);
         router.navigateBack();
     };
     var vm = {
         binding: function () {
             parentCode = qs.querystring("pc");
             if (parentCode == undefined || parentCode == '') {
                 parentCode = moduleInfo.parentModuleID;
             }
             //初初化导航状态
             //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
             id = qs.querystring("id");
             vm.vmguide = new vmguide(id);
         },
         vmguide: null,
         back: back,
         getUrl: getUrl
     }
     return vm;
 });