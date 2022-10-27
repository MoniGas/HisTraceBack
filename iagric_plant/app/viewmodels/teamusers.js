define(['plugins/router', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'knockout', 'jquery-ui', 'utils', 'jquery.fileDownload', 'logininfo', 'jquery.uploadify', 'bootstrap-datepicker', 'knockout.validation', './teamusersedit'],
 function (router, km, $, jq, dialog, ko, jqueryui, utils, jfd, loginInfo, uploadify, bdp, kv, teamusersedit) {
     //ajax获取数据
     var initDate = true;
     var getData = function (teamid, pageIndex) {
         var sendData = {
             teamid: teamid,
             pageIndex: pageIndex
         }
         var data;
         $.ajax({
             type: "POST",
             url: "/TeamUsers/Index",
             contentType: "application/json;charset=utf-8", //必须有
             dataType: "json", //表示返回值类型，不必须
             data: JSON.stringify(sendData),
             async: false,
             success: function (jsonResult) {
                 if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                     return;
                 };
                 data = jsonResult;
             },
             error: function (Error) {
                 alert(Error);
             }
         })
         return data;
     }
     //分页、搜索时更新数据源
     var updateData = function (list) {
         vmObj.vmTeamUsers.ObjList(list.ObjList());
         vmObj.vmTeamUsers.pageSize(list.pageSize());
         vmObj.vmTeamUsers.totalCounts(list.totalCounts());
         vmObj.vmTeamUsers.pageIndex(list.pageIndex());
     }
     //把获取的ajax数据转化为ko
     var getDataKO = function (teamid, pageIndex) {
         var list = km.fromJS(getData(teamid, pageIndex));
         return list;
     }
     //自定义绑定-分页控件
     ko.bindingHandlers.TeamUsersPager = {
         init: function (element, valueAccessor, allBindingsAccessor) {
         },
         update: function (element, valueAccessor, allBindingsAccessor) {
             var value = valueAccessor();
             var allBindings = allBindingsAccessor();
             var pageSize = parseInt(ko.utils.unwrapObservable(value));
             var totalCounts = parseInt(ko.utils.unwrapObservable(allBindings.totalCounts));
             var pageIndex = parseInt(ko.utils.unwrapObservable(allBindings.pageIndex));
             $(element).jqPaginator({
                 totalCounts: totalCounts,
                 pageSize: pageSize,
                 visiblePages: 10,
                 currentPage: pageIndex,
                 first: '<li class="first"><a href="javascript:;">首页</a></li>',
                 last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                 prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                 next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                 page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                 onPageChange: function (num, type) {
                     if (type == 'change') {
                         var list = getDataKO(vmObj.teamid, num);
                         updateData(list);
                         this.pageSize = parseInt(vmObj.vmTeamUsers.pageSize());
                         this.totalCounts = parseInt(vmObj.vmTeamUsers.totalCounts());
                     }
                 }
             });
         }
     };

     //定义主模型gensuitcode==teamusers
     var teamusers = function (teamid) {
         var self = this;
         self.teamid = teamid;
//         self.userName = ko.observable('').extend({
//             minLength: { params: 2, message: "名称最小长度为2个字符" },
//             maxLength: { params: 50, message: "名称最大长度为50个字符" },
//             required: {
//                 params: true,
//                 message: "请输入人员名称！"
//             }
//         });
         self.userName = ko.observable().extend({
             maxLength: { params: 10, message: "名称最大长度为10个字符!" },
             required: { params: true, message: "请输入人员名称!" }
         });
         self.userPhone = ko.observable().extend({
             maxLength: { params: 30, message: "联系电话最大长度为30个字符!" },
//             required: { params: true, message: "请输入联系电话!" },
             pattern: { params: /(^[0-9]{3,4}\-[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}\-[0-9]{3,4}$)|(^[0-9]{7,11}$)/, message: "电话格式不正确!" }
         });
         self.userNumber = ko.observable('');
         self.vmTeamUsers = ko.observableArray();
         self.onchange = function () {
             var list = getDataKO(teamid, 1);
             self.a(list);
         }
         self.a = function (list) {
             self.vmTeamUsers.ObjList(list.ObjList());
         }
         self.init = function (a) {
             var list = getDataKO(teamid, 1);
             return list;
         }
         self.deleteteamuser = function (id, data, event) {
             dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
             {
                title: '确定',
                callback: function () {
                    var sendData = {
                        teamUsersID: ko.utils.unwrapObservable(id)
                     }
                $.ajax({
                    type: "POST",
                    url: "/TeamUsers/Delete",
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
                                if (jsonResult.code != 0) {
                                    var currentPageRow = vmObj.vmTeamUsers.ObjList().length;
                                    var pageIndex = vmObj.vmTeamUsers.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vmObj.vmTeamUsers.pageIndex() - 1;
                                    }
                                    var list = getDataKO(vmObj.teamid, pageIndex);
                                    updateData(list);
                                    this.pageSize = parseInt(vmObj.vmTeamUsers.pageSize());
                                    this.totalCounts = parseInt(vmObj.vmTeamUsers.totalCounts());

                                }
                            }
                            }]);
                        }
                    }
                })
            }
        },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
         }
         self.editteamuser = function (id, data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             var jsonResult = loginInfo.isLoginTimeoutForServer();
             if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                 return;
             };
             var id = ko.utils.unwrapObservable(id);
             teamusersedit.show(id).then(function () {
                 var list = getDataKO(teamid, 1);
                 updateData(list);
             });
         }
         self.calcel = function () {
             this.close();
         }
         //鼠标滑过************************************************************************/
         self.mouseoverFun = function (data, event) {
             var self = $(event.target).closest('tr');
             var ShowAll = self.find("button[eleflag='ShowAll']");
             ShowAll.css({ "display": "" });
         }
         //鼠标离开************************************************************************/
         self.mouseoutFun = function (data, event) {
             var self = $(event.target).closest('tr');
             var ShowAll = self.find("button[eleflag='ShowAll']");
             ShowAll.css({ "display": "none" });
         }
         //生成码************************************************************************/
         self.adduser = function (data, event) {
             var currentObj = $(event.target);
             currentObj.blur();
             self.errors = ko.validation.group(self);
             if (self.errors().length <= 0) {
                 var sendData = {
                     teamid: self.teamid,
                     userName: self.userName(),
                     userPhone: self.userPhone(), 
                     userNumber: self.userNumber()
                 }
                 $.ajax({
                     type: "POST",
                     url: "/TeamUsers/Add",
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
                                 var list = getDataKO(vmObj.teamid, 1);
                                 updateData(list);
                             }
                         }
                         }]);
                     }
                 })
             }
             else {
                 self.errors.showAllMessages();
             }
         }
     }
     //绑定属性************************************************************************/
     teamusers.prototype.binding = function () {
         $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
     }
     teamusers.prototype.close = function (a) {
         //alert(this.province().code);
         dialog.close(this, a);
     }
     teamusers.prototype.closeOK = function (id) {
         dialog.close(this, id);
     }
     var vmObj;
     teamusers.show = function (teamid) {
         vmObj = new teamusers(teamid);
         vmObj.vmTeamUsers = vmObj.init(0);
         return dialog.show(vmObj);
     };
     return teamusers;
 });
