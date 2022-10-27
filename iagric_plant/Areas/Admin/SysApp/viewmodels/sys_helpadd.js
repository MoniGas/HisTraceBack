define(['plugins/router', 'plugins/dialog', 'knockout', 'jquery', 'bootstrap-datepicker', 'jquery.uploadify', 'knockout.validation', 'utils', 'logininfo', 'knockout.mapping', 'jqPaginator', 'jquery.fileDownload', 'jquery.querystring', 'kindeditor.zh-CN'],
 function (router, dialog, ko, $, bdp, uploadify, kv, utils, loginInfo, km, jq, jfd, qs, kcn) {
     var moduleInfo = {
         moduleID: '11131',
         parentModuleID: '11130'
     }
     var helpTitle = ko.observable('').extend({
         maxLength: { params: 50, message: "帮助标题最大长度为50个字符" },
         required: { params: true, message: "请输入标题!" }
     });
     var helpDescriptions = ko.observable('');
     var SelType = ko.observable(false);
     var HelpTypeModel = {
         HelpTypeArray: ko.observable(),
         SelectedId: ko.observable()
     }
     var getHelpType = function () {
         var data;
         var sendData = {
     }
     $.ajax({
         type: "POST",
         url: "/SysHelp/GetList",
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

 var AddHelp = function (data, event) {
     var currentObj = $(event.target);
     currentObj.blur();
     var errors = ko.validation.group(this);
     if (errors().length <= 0) {
         var sendData = {
             helpTitle: helpTitle(),
             //                        descriptions: helpDescriptions()
             descriptions: editorHelp.html(),
             type: ko.utils.unwrapObservable(HelpTypeModel.SelectedId),
         }
         $.ajax({
             type: "POST",
             url: "/SysHelp/Add",
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
                                 router.navigate('#sys_help');
                     }
                 }
                 }]);
             }
         })
     } else {
         errors.showAllMessages();
     }
 };


 var vm = {
     binding: function () {
         //初初化导航状态
         loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
         editorHelp = KindEditor.create("#txtInfos", {
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
         editorHelp.html(vm.helpDescriptions());
         HelpTypeModel.HelpTypeArray(getHelpType());
     },
     goBack: function () {
         router.navigateBack();
     },
     vmHelp: null,
     helpTitle: helpTitle,
     helpDescriptions: helpDescriptions,
     SelType: SelType,
     HelpTypeModel: HelpTypeModel,
     getHelpType: getHelpType,
     AddHelp: AddHelp
 }
 return vm;
});