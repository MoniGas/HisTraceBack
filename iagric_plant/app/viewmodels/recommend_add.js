define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.querystring', 'bootbox'],
function (router, ko, km, $, kv, utils, loginInfo, dialog, jqueryui, qs, bootbox) {
    var vmObj;
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
    var getData = function (pageIndex, materialId) {
        var sendData = {
            pageIndex: pageIndex,
            materialId: materialId
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Recommend/GetSettinglist",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult;
            }
        })
        return data;
    }
    var getDataKO = function (pageIndex, materialId) {
        var list = km.fromJS(getData(pageIndex, materialId));
        return list;
    }
    //自定义绑定-分页控件
    ko.bindingHandlers.settingPager = {
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
                visiblePages: 5,
                currentPage: pageIndex,
                first: '<li class="first"><a href="javascript:;">首页</a></li>',
                last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                onPageChange: function (num, type) {
                    if (type == 'change') {
                        var list = getData(num, vmObj.MaterialList.SelectedId());
                        vmObj.updateData(list);
                        this.pageSize = parseInt(vmObj.settingList.pageSize());
                        this.totalCounts = parseInt(vmObj.settingList.totalCounts());
                    }
                }
            });
        }
    };
    var vmRecommendAdd = function () {
        var self = this;

        //产品列表
        self.MaterialList = {
            MaterialArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        self.MaterialList.SelectedId.subscribe(function () {
            if (self.MaterialList.SelectedId()) {
                var list = getDataKO(1, self.MaterialList.SelectedId());
                self.updateData(list);
            }
            else {
                self.settingList.ObjList(new Array());
            }
            self.selMaterial(!self.MaterialList.SelectedId());
        });
        self.selMaterial = ko.observable(false);

//        self.recommendName = ko.observable('').extend({
//            maxLength: { params: 5, message: "名称最大长度为5个字符" },
//            required: {
//                params: true,
//                message: "请输入推荐产品名称！"
//            }
//        });

        self.settingList = {
            ObjList: ko.observableArray(),
            pageSize: ko.observable(),
            totalCounts: ko.observable(),
            pageIndex: ko.observable()
        }
        self.selList = ko.observable(false);
        //分页、搜索时更新数据源
        self.updateData = function (list) {
            self.settingList.ObjList(list.ObjList());
            self.settingList.pageSize(list.pageSize());
            self.settingList.totalCounts(list.totalCounts());
            self.settingList.pageIndex(list.pageIndex());
        }

        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            self.selMaterial(!self.MaterialList.SelectedId());
            self.selList(!$("[name='cbx']:checked").val());
            if (self.errors().length <= 0 && !self.selMaterial() && !self.selList()) {
                var sendData = {
                    materialId: self.MaterialList.SelectedId(),
                    settingId: $("[name='cbx']:checked").val(),
                    recommendName: ''
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Recommend/Add",
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
                                self.closeOK(jsonResult.ObjModel);
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
        self.BatchType = function (Type) {
            Type = ko.utils.unwrapObservable(Type);
            if (Type == 1) {
                return "主批次";
            } else if (Type == 2) {
                return "子批次";
            }

        }
        self.RequestCodeType = function (CodeType) {
            CodeType = ko.utils.unwrapObservable(CodeType);
            if (CodeType == 1) {
                return "追溯码";
            } else if (CodeType == 2) {
                return "防伪码";
            } else if (CodeType == 3) {
                return "防伪追溯码";
            }
        }
        self.showSubBatch = function (id, data, event) {
            var data1;
            var sendData = {
                requestId: id()
            }
            $.ajax({
                type: "POST",
                url: "/Admin_Recommend/GetSublist",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    };
                    data1 = jsonResult;
                }
            });
            var subSelf;
            if (data == 'addSubBatch') {
                subSelf = $("#" + sourceObj);
            }
            else {
                subSelf = $(event.target);
            }
            var subObjList = km.fromJS(data1.ObjList);
            if (subSelf.hasClass("fa-caret-right")) {
                var currentBatch = ko.utils.arrayFilter(self.settingList.ObjList(), function (item) {
                    return item.RequestID == id;
                });
                currentBatch[0].subBatchObj(subObjList());
                $("tr[subflag='subBatchObj_" + id() + "']").fadeIn(500);
                subSelf.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
            }
            else {
                if (data == 'addSubBatch') {
                    var currentBatch = ko.utils.arrayFilter(self.settingList.ObjList(), function (item) {
                        return item.RequestID == id;
                    });
                    currentBatch[0].subBatchObj(subObjList());
                    $("tr[subflag='subBatchObj_" + id() + "']").fadeIn(500);
                }
                else {
                    $("tr[subflag='subBatchObj_" + id() + "']").hide();
                    subSelf.toggleClass("fa-caret-right").toggleClass("fa-caret-down");
                }
            }
        }
    }

    vmRecommendAdd.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmRecommendAdd.prototype.close = function () {
        dialog.close(this);
    }
    vmRecommendAdd.prototype.closeOK = function () {
        dialog.close(this);
    }
    vmRecommendAdd.show = function () {
        vmObj = new vmRecommendAdd();
        vmObj.MaterialList.MaterialArray(GetMaterialList());
        return dialog.show(vmObj);
    };
    return vmRecommendAdd;
});