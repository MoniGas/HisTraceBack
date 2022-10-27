define(['plugins/router', 'knockout','jquery', 'bootstrap-datepicker', 'knockout.validation', 'utils', 'jquery.querystring', 'logininfo', './material_add'],
 function (router, ko,$, bdp, kv, utils, qs, loginInfo, material_add) {
     var parentCode;
     var moduleInfo = {
         moduleID: '14003',
         parentModuleID: '10001'
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
     var vmguide = function (id) {
         var self = this;
         self.id = id;

         self.url = '/ShowImage/ShowImgNoUrl?ewm=' + getUrl();
     }
     var next = function (data, event) {
         router.navigate('#guide5?id=' + self.id+'&pc='+parentCode);
     };
     var back = function (data, event) {
         //         router.navigate('#guide3?id=' + self.id+'&pc='+parentCode);
         router.navigateBack();
     };
     var downloadFile = function (downLoadURL) {
         var jsonResult = loginInfo.isLoginTimeoutForServer();
         if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
             return;
         };
         var downLoadURL = ko.utils.unwrapObservable(downLoadURL);
         $.fileDownload('/Public/DownloadFile?downLoadURL=' + downLoadURL)
                  .done(function () { alert('下载成功'); })
                       .fail(function () { alert('下载失败!'); });
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
         next: next,
         back: back
     }
     return vm;
 });