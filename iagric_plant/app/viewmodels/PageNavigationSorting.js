define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'knockout.validation'], 
function (dialog, ko, jqueryui, km, utils, loginInfo,kv) {

//    //自定义绑定-分页控件
//    ko.bindingHandlers.PageNavigationSortBH = {
//        init: function (element, valueAccessor, allBindingsAccessor) {
//        },
//        update: function (element, valueAccessor, allBindingsAccessor) {

//            var value = valueAccessor();
//            var allBindings = allBindingsAccessor();
//            var pageSize = parseInt(ko.utils.unwrapObservable(value));
//            var totalCounts = parseInt(ko.utils.unwrapObservable(allBindings.totalCounts));
//            var pageIndex = parseInt(ko.utils.unwrapObservable(allBindings.pageIndex));

//            $(element).jqPaginator({
//                totalCounts: totalCounts,
//                pageSize: pageSize,
//                visiblePages: 10,
//                currentPage: pageIndex,
//                first: '<li class="first"><a href="javascript:;">首页</a></li>',
//                last: '<li class="last"><a href="javascript:;">尾页</a></li>',
//                prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
//                next: '<li class="next"><a href="javascript:;">下一页</a></li>',
//                page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
//                onPageChange: function (num, type) {
//                    if (type == 'change') {
//                        var list = GetDataKO(num);
//                        UpdateData(list);
//                        this.pageSize = parseInt(ViewModel.DataList.pageSize());
//                        this.totalCounts = parseInt(ViewModel.DataList.totalCounts());
//                    }
//                }
//            });
//        }
//    };

    var GetData = function (PageIndex) {
        var sendData = {
            MaterialId: VmMaterialId,
            PageIndex: PageIndex
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_PageNavigation/GetList",
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

    //把获取的ajax数据转化为ko
    var GetDataKO = function (PageIndex) {
        var list = km.fromJS(GetData(PageIndex));
        return list;
    }

    var ViewModel = function () {
        var self = this;
        self.DataList = ko.observableArray();
        self.ModelList = ko.observableArray();
        self.SearchData = function () {
            var list = GetDataKO(1);
            self.UpdateData(list);
        }

        //分页、搜索时更新数据源
        self.UpdateData = function (list) {
            self.DataList.ObjList(list.ObjList());
            self.DataList.pageSize(list.pageSize());
            self.DataList.totalCounts(list.totalCounts());
            self.DataList.pageIndex(list.pageIndex());
        }

        self.UpMoveChange = function (ViewNum, data, event) {
            ViewNum = ko.utils.unwrapObservable(ViewNum);

            if (ViewNum <= 1) {
                return;
            }

            self.DataList.ObjList()[ViewNum - 1].ViewNum(self.DataList.ObjList()[ViewNum - 1].ViewNum() - 1);
            self.DataList.ObjList()[ViewNum - 2].ViewNum(self.DataList.ObjList()[ViewNum - 2].ViewNum() + 1);

            var ObjModel = self.DataList.ObjList()[ViewNum - 1];

            self.DataList.ObjList().splice(ViewNum - 1, 1, self.DataList.ObjList()[ViewNum - 2]);
            self.DataList.ObjList().splice(ViewNum - 2, 1, ObjModel);

            self.DataList.ObjList(self.DataList.ObjList());
        };

        self.DownMoveChange = function (ViewNum, data, event) {
            ViewNum = ko.utils.unwrapObservable(ViewNum);

            if (ViewNum >= self.DataList.ObjList().length) {
                return;
            }

            self.DataList.ObjList()[ViewNum - 1].ViewNum(self.DataList.ObjList()[ViewNum - 1].ViewNum() + 1);
            self.DataList.ObjList()[ViewNum].ViewNum(self.DataList.ObjList()[ViewNum].ViewNum() - 1);

            var ObjModel = self.DataList.ObjList()[ViewNum];

            self.DataList.ObjList().splice(ViewNum, 1, self.DataList.ObjList()[ViewNum - 1]);
            self.DataList.ObjList().splice(ViewNum - 1, 1, ObjModel);

            self.DataList.ObjList(self.DataList.ObjList());
        }

        self.MouseoverFun = function (data, event) {
            // 删除
            var BtnDel = $("#BtnDel_" + data.Id());
            // 上移
            var BtnUp = $("#BtnUp_" + data.Id());
            // 下移
            var BtnDown = $("#BtnDown_" + data.Id());

            BtnDel.css({ "display": "" });
            BtnUp.css({ "display": "" });
            BtnDown.css({ "display": "" });
        }

        self.MouseoutFun = function (data, event) {
            // 删除
            var BtnDel = $("#BtnDel_" + data.Id());
            // 上移
            var BtnUp = $("#BtnUp_" + data.Id());
            // 下移
            var BtnDown = $("#BtnDown_" + data.Id());

            BtnDel.css({ "display": "none" });
            BtnUp.css({ "display": "none" });
            BtnDown.css({ "display": "none" });
        }

        self.Save = function () {

            for (var i = 0; i < self.DataList.ObjList().length; i++) {
                var single = {
                    Id: self.DataList.ObjList()[i].Id(),
                    ViewNum: self.DataList.ObjList()[i].ViewNum()
                }
                self.ModelList.push(single);
            }

            var sendData = {
                ModelList: JSON.stringify(self.ModelList())
            }
            $.ajax({
                type: "POST",
                url: "/Admin_PageNavigation/UpdateList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                        return;
                    };
                    dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                        if (jsonResult.code != 0) {
                            self.close();
                        }
                    }
                    }]);
                }
            })
        }

        //删除生产基地
        self.DelNavigation = function (Id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(Id);
            dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    Id: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_PageNavigation/DelNavigationForEnterprise",
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
                                    var currentPageRow = self.DataList.ObjList().length;
                                    var pageIndex = self.DataList.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = self.DataList.pageIndex() - 1;
                                    }
                                    self.SearchData(pageIndex);

                                    this.pageSize = parseInt(self.DataList.pageSize());
                                    this.totalCounts = parseInt(self.DataList.totalCounts());
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
    }

    ViewModel.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

    }

    ViewModel.prototype.close = function () {
        dialog.close(this);
    }
    var vmObj;
    var VmMaterialId;
    ViewModel.show = function (MaterialId) {
        VmMaterialId = MaterialId;
        vmObj = new ViewModel();
        vmObj.DataList = GetDataKO(1);
        return dialog.show(vmObj);
    };
    return ViewModel;
});